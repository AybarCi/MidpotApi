using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Extension;
using DatingWeb.Helper;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.User.Interface;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DatingWeb.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;
        private readonly ICache _cache;
        private readonly IRedisCache _redisCache;

        public UserRepository(ApplicationDbContext context, IBlobService blobService, ICache cache, IRedisCache redisCache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _blobService = blobService;
            _cache = cache;
            _redisCache = redisCache;
        }

        /// <summary>
        /// GetProfile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserResponse> GetProfile(long userId)
        {
            string cacheKey = $"user_profile_{userId}";

            // Redis cache'ten kontrol et
            var cachedProfile = await _redisCache.GetAsync<UserResponse>(cacheKey);
            if (cachedProfile != null)
            {
                return cachedProfile;
            }

            // Cache'te yoksa veritabanından getir
            var profile = await _context.ApplicationUsers.Where(x => x.Id == userId && x.LockoutEnabled == false).Select(x => new UserResponse
            {
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                PersonName = x.PersonName,
                PreferredGender = x.PreferredGender,
                UserId = x.Id,
                Email = x.Email,
                GhostMode = x.GhostMode
            }).FirstOrDefaultAsync();

            // Veriyi Redis'e cache'le
            if (profile != null)
            {
                await _redisCache.SetAsync(cacheKey, profile, TimeSpan.FromMinutes(30)); // 30 dakika cache
            }

            return profile;
        }

        /// <summary>
        //  UpdateProfileSettings
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<bool> UpdateProfileSettings(long userId, string description, int fromAge, int untilAge, string school, string job, string deviceToken, bool ghostMode, string personName)
        {
            var user = await _context.ApplicationUsers.Where(u => u.Id == userId && u.LockoutEnabled == false).FirstOrDefaultAsync();
            user.Description = description;
            user.FromAge = fromAge;
            user.UntilAge = untilAge;
            user.School = school;
            user.Job = job;
            user.DeviceToken = deviceToken;
            user.GhostMode = ghostMode;
            user.PersonName = personName;
            int val = await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(deviceToken))
                _cache.Remove(userId.ToString());

            // Redis cache'ini temizle
            string cacheKey = $"user_profile_{userId}";
            await _redisCache.RemoveAsync(cacheKey);

            return val > 0;
        }

        /// <summary>
        /// UpdateProfilePhoto
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task UpdateProfilePhoto(long userId, string fileName)
        {
            var user = await _context.ApplicationUsers.Where(u => u.Id == userId && u.LockoutEnabled == false).FirstOrDefaultAsync();
            user.ProfilePhoto = fileName;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// UpdateProfilePhotoNew
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> UpdateProfilePhotoNew(long userId, IFormFile file)
        {
            string fileName = String.Empty;
            string uri = String.Empty;

            var user = await _context.ApplicationUsers.Where(u => u.Id == userId && u.LockoutEnabled == false).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(user.ProfilePhoto))
                fileName = Guid.NewGuid().ToString();
            else
                fileName = user.ProfilePhoto.ToProfilePhotoId();

            var response = await _blobService.UploadFileBlobAsync("firstcontainer", file.OpenReadStream(), file.ContentType, string.Format("{0}.jpeg", fileName));

            if (string.IsNullOrEmpty(user.ProfilePhoto))
            {
                user.ProfilePhoto = fileName.ToProfilePhoto();
                await _context.SaveChangesAsync();
            }

            return fileName.ToProfilePhoto();
        }

        /// <summary>
        /// GetDeviceToken
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetDeviceToken(long userId)
        {
            return await _context.ApplicationUsers.Where(x => x.Id == userId && x.LockoutEnabled == false).Select(x => x.DeviceToken).FirstOrDefaultAsync();
        }

        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(long userId)
        {
            var user = await _context.ApplicationUsers.Where(x => x.Id == userId && x.LockoutEnabled == false && x.IsDelete == false).FirstOrDefaultAsync();
            if (user != null)
            {
                user.IsDelete = true;
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var matches = await _context.Match.Where(x => x.MaleUser == userId || x.FemaleUser == userId).ToListAsync();
                if (matches != null)
                {
                    matches.ForEach(x =>
                    {
                        x.IsActive = false;
                        x.UpdateDate = DateTime.UtcNow;
                    });
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            else
                throw new ApiException("Kullanıcı bulunamadı!");
        }

        public async Task<List<UserResponse>> GetAll()
        {
            return await _context.ApplicationUsers.OrderBy(x => x.CreateDate).Select(x => new UserResponse
            {
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                PersonName = x.PersonName,
                PreferredGender = x.PreferredGender,
                UserId = x.Id,
                Email = x.Email,
                GhostMode = x.GhostMode,
                ProfilePhoto = x.ProfilePhoto,
                IsDelete = x.IsDelete
            }).ToListAsync();
        }

        public async Task<List<UserResponse>> GetUsersWeekly()
        {
            return await _context.ApplicationUsers.Where(x => x.CreateDate > DateTime.Now.AddDays(-7))
                .OrderBy(x => x.CreateDate)
                .Select(x => new UserResponse
                {
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    PersonName = x.PersonName,
                    PreferredGender = x.PreferredGender,
                    UserId = x.Id,
                    Email = x.Email,
                    GhostMode = x.GhostMode
                }).ToListAsync();
        }

        public async Task<int> GetAllUsersCount()
        {
            return await _context.ApplicationUsers.CountAsync();
        }

        public async Task<int> GetUsersWeeklyCount()
        {
            return await _context.ApplicationUsers.Where(x => x.CreateDate > DateTime.Now.AddDays(-7)).CountAsync();
        }
        public async Task<List<UserResponse>> GetDeletedUsers()
        {
            return await _context.ApplicationUsers.Where(x => x.IsDelete == true).Select(x => new UserResponse
            {
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                PersonName = x.PersonName,
                PreferredGender = x.PreferredGender,
                UserId = x.Id,
                Email = x.Email,
                GhostMode = x.GhostMode
            }).ToListAsync();
        }
        public async Task<int> GetDeletedUsersCount()
        {
            return await _context.ApplicationUsers.Where(x => x.IsDelete == true).CountAsync();
        }
        public async Task<bool> AddUser(List<AddUserRequest> addUserRequest)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            foreach (var item in addUserRequest)
            {
                applicationUsers.Add(new ApplicationUser
                {
                    PersonName = item.PersonName,
                    BirthDate = item.BirthDate,
                    CreateDate = DateTime.Now,
                    EmailConfirmed = true,
                    FromAge = item.FromAge,
                    Gender = item.Gender,
                    Latitude = item.Latitude,
                    LockoutEnabled = false,
                    Longitude = item.Longitude,
                    NormalizedUserName = item.PersonName,
                    PreferredGender = item.PreferredGender,
                    UntilAge = item.UntilAge,
                    UserName = item.PersonName,
                    IsDelete = false,
                    PhoneNumber = "0506-938-44-13",
                    PhoneNumberConfirmed = true,
                    Platform = true,
                    ProfilePhoto = "https://midpotstorage.blob.core.windows.net/firstcontainer/481245f0-ce3c-4caf-8855-8258c808c420.jpeg"
                });
            }

            await _context.AddRangeAsync(applicationUsers);
            var val = await _context.SaveChangesAsync();
            return val > 0 ? true : false;
        }
    }
}
