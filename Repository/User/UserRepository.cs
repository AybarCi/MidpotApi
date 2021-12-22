using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Extension;
using DatingWeb.Helper;
using DatingWeb.Model.Response;
using DatingWeb.Repository.User.Interface;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;
        private readonly ICache _cache;

        public UserRepository(ApplicationDbContext context, IBlobService blobService, ICache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _blobService = blobService;
            _cache = cache;
        }

        /// <summary>
        /// GetProfile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserResponse> GetProfile(long userId)
        {
            return await _context.ApplicationUsers.Where(x => x.Id == userId && x.LockoutEnabled == false).Select(x => new UserResponse
            {
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                PersonName = x.PersonName,
                PreferredGender = x.PreferredGender,
                UserId = x.Id,
                Email = x.Email
            }).FirstOrDefaultAsync();
        }

        /// <summary>
        //  UpdateProfileSettings
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<bool> UpdateProfileSettings(long userId, string description, int fromAge, int untilAge, string school, string job, string deviceToken)
        {
            var user = await _context.ApplicationUsers.Where(u => u.Id == userId && u.LockoutEnabled == false).FirstOrDefaultAsync();
            user.Description = description;
            user.FromAge = fromAge;
            user.UntilAge = untilAge;
            user.School = school;
            user.Job = job;
            user.DeviceToken = deviceToken;
            int val = await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(deviceToken))
                _cache.Remove(userId.ToString());

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
    }
}
