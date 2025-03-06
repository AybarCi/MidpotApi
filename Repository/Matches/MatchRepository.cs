using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Extension;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Matches.Interface;
using DatingWeb.Repository.User.Interface;
using DatingWeb.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Matches
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICache _cache;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public MatchRepository(ApplicationDbContext context, ICache cache, IConfiguration configuration,
            INotificationService notificationService, IUserRepository userRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _cache = cache;
            _configuration = configuration;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// User id tokendan al
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<MatchResponse>> MatchMachine(long userId, bool gender, bool preferredGender, DateTime lastMatchDate, double latitude, double longitude)
        {
            var returnedUser = new List<MatchResponse>();

            //Aktif match var mı
            //true erkek false kız
            var checkMatchUser = await GetCheckMatch(gender, preferredGender, userId, lastMatchDate);

            //match gelmiş ise dönecek değere ekle
            if (checkMatchUser.Count > 0)
            {
                List<long> tempUser = new List<long>();

                foreach (var item in checkMatchUser)
                    tempUser.Add(item.MaleUser == userId ? item.FemaleUser : item.MaleUser);

                //user listesine join ol o userları çek
                returnedUser = await _context.ApplicationUsers.Where(x => tempUser.Contains(x.Id) && x.LockoutEnabled == false).Select(x => new MatchResponse
                {
                    BirthDate = x.BirthDate,
                    PersonName = x.PersonName,
                    Id = x.Id,
                    ProfilePhoto = x.ProfilePhoto,
                    Description = x.Description,
                    School = x.School,
                    Job = x.Job,
                    DeviceToken = x.DeviceToken
                }).ToListAsync();

                for (int i = 0; i < returnedUser.Count; i++)
                {
                    var mId = checkMatchUser.Where(x => x.FemaleUser == returnedUser[i].Id || x.MaleUser == returnedUser[i].Id).FirstOrDefault();
                    if (mId != null)
                    {
                        returnedUser[i].MatchId = mId.MatchId;
                        returnedUser[i].CreateDate = mId.CreateDate;
                    }
                }
            }

            //yeni match gelmiş mi
            if (lastMatchDate.AddDays(1) < DateTime.UtcNow)
            {
                //eğer başka eşleşmelerin varsa tarihi uyuyorsa
                if (checkMatchUser.Count > 0 && checkMatchUser.OrderByDescending(x => x.MatchId).FirstOrDefault().CreateDate.AddDays(1) > DateTime.UtcNow)
                    return returnedUser;

                MatchResponse tempUserMatches = await GetMatch(gender, preferredGender, userId, latitude, longitude);

                if (tempUserMatches != null)
                {
                    //match i kaydet
                    _context.ChangeTracker.AutoDetectChangesEnabled = false;
                    if (gender)
                    {
                        var matchTemp = new Match { MaleUser = userId, FemaleUser = tempUserMatches.Id, CreateDate = DateTime.UtcNow, IsActive = true, UpdateDate = DateTime.UtcNow };
                        await _context.Match.AddAsync(matchTemp);
                        await _context.SaveChangesAsync();

                        tempUserMatches.MatchId = matchTemp.MatchId;
                        tempUserMatches.CreateDate = matchTemp.CreateDate;
                    }
                    else
                    {
                        var matchTemp = new Match { FemaleUser = userId, MaleUser = tempUserMatches.Id, CreateDate = DateTime.UtcNow, IsActive = true, UpdateDate = DateTime.UtcNow };
                        await _context.Match.AddAsync(matchTemp);
                        await _context.SaveChangesAsync();

                        tempUserMatches.MatchId = matchTemp.MatchId;
                        tempUserMatches.CreateDate = matchTemp.CreateDate;
                    }

                    //Matchi kaydettikten sonra notification atiyor
                    await SendNotification(tempUserMatches.MatchId.ToString(), tempUserMatches.Id);

                    returnedUser.Add(new MatchResponse
                    {
                        Id = tempUserMatches.Id,
                        BirthDate = tempUserMatches.BirthDate,
                        PersonName = tempUserMatches.PersonName,
                        ProfilePhoto = tempUserMatches.ProfilePhoto,
                        Description = tempUserMatches.Description,
                        CreateDate = tempUserMatches.CreateDate,
                        School = tempUserMatches.School,
                        Job = tempUserMatches.Job,
                        MatchId = tempUserMatches.MatchId
                    });
                }
            }

            return returnedUser;
        }

        /// <summary>
        /// Aktif match var mı? 
        /// true erkek false kız.
        /// hangi cinssen sadece o alana bak matchin var mı?
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="preferredGender"></param>
        /// <param name="userId"></param>
        /// <param name="lastMatchDate"></param>
        /// <returns></returns>
        private async Task<List<Match>> GetCheckMatch(bool gender, bool preferredGender, long userId, DateTime lastMatchDate)
        {
            if (gender && !preferredGender)
                return await _context.Match.Where(m => m.MaleUser == userId && m.IsActive == true && m.CreateDate > lastMatchDate).ToListAsync();
            else if (!gender && preferredGender)
                return await _context.Match.Where(m => m.FemaleUser == userId && m.IsActive == true && m.CreateDate > lastMatchDate).ToListAsync();
            else
                return await _context.Match.Where(m => (m.MaleUser == userId || m.FemaleUser == userId) && m.IsActive == true && m.CreateDate > lastMatchDate).ToListAsync();
        }

        private async Task<MatchResponse> GetMatch(bool gender, bool preferredGender, long userId, double latitude, double longitude)
        {
            //gelen userı çek lastlogini datetimenow ver ve lat longunu güncelle

            var user = await _context.ApplicationUsers.Where(x => x.Id == userId).FirstOrDefaultAsync();
            user.LastLoginDate = DateTime.UtcNow;
            user.Latitude = latitude;
            user.Longitude = longitude;
            int val = await _context.SaveChangesAsync();

            //STOREDPROCEDURE EKLE YOKSA ÇALIŞMAZ!!!
            //erkek kız
            if (gender == true && preferredGender == false)
            {
                using (var cmd = new NpgsqlCommand(string.Format("SELECT * FROM get_male_match({0}, {1}, {2}, {3}, {4}, DATE '{5}', DATE '{6}')", userId.ToString(), latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."), user.BirthDate.ToBirthDateFormat().ToString(), _configuration.GetValue<string>("DistanceLimit"), user.FromAge.ToBirthDateFormat().ToString("u"), user.UntilAge.ToBirthDateFormat().ToString("u")), new NpgsqlConnection(_configuration.GetConnectionString("PostgreConnection"))))
                {
                    await cmd.Connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            return new MatchResponse
                            {
                                Id = long.Parse(reader[0].ToString()),
                                PersonName = reader[1].ToString(),
                                BirthDate = DateTime.Parse(reader[2].ToString()),
                                ProfilePhoto = reader[3].ToString(),
                                Description = reader[4].ToString(),
                                School = reader[5].ToString(),
                                Job = reader[6].ToString(),
                                DeviceToken = reader[7].ToString()
                            };
                        }
                    };
                    await cmd.Connection.CloseAsync();
                }
            }
            //kız erkek
            else if (gender == false && preferredGender == true)
            {
                using (var cmd = new NpgsqlCommand(string.Format("SELECT * FROM get_female_match({0}, {1}, {2}, {3}, {4}, DATE '{5}', DATE '{6}')", userId.ToString(), latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."), user.BirthDate.ToBirthDateFormat().ToString(), _configuration.GetValue<string>("DistanceLimit"), user.FromAge.ToBirthDateFormat().ToString("u"), user.UntilAge.ToBirthDateFormat().ToString("u")), new NpgsqlConnection(_configuration.GetConnectionString("PostgreConnection"))))
                {
                    await cmd.Connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            return new MatchResponse
                            {
                                Id = long.Parse(reader[0].ToString()),
                                PersonName = reader[1].ToString(),
                                BirthDate = DateTime.Parse(reader[2].ToString()),
                                ProfilePhoto = reader[3].ToString(),
                                Description = reader[4].ToString(),
                                School = reader[5].ToString(),
                                Job = reader[6].ToString(),
                                DeviceToken = reader[7].ToString()
                            };
                        }
                    };
                    await cmd.Connection.CloseAsync();
                }
            }
            //ec
            else if ((gender == true && preferredGender == true) || (gender == false && preferredGender == false))
            {
                using (var cmd = new NpgsqlCommand(string.Format("SELECT * FROM get_ec_match({0}, {1}, {2}, {3}, {4}, DATE '{5}', DATE '{6}')", userId.ToString(), latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."), user.BirthDate.ToBirthDateFormat().ToString(), _configuration.GetValue<string>("DistanceLimit"), user.FromAge.ToBirthDateFormat().ToString("u"), user.UntilAge.ToBirthDateFormat().ToString("u")), new NpgsqlConnection(_configuration.GetConnectionString("PostgreConnection"))))
                {
                    await cmd.Connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            return new MatchResponse
                            {
                                Id = long.Parse(reader[0].ToString()),
                                PersonName = reader[1].ToString(),
                                BirthDate = DateTime.Parse(reader[2].ToString()),
                                ProfilePhoto = reader[3].ToString(),
                                Description = reader[4].ToString(),
                                School = reader[5].ToString(),
                                Job = reader[6].ToString(),
                                DeviceToken = reader[7].ToString()
                            };
                        }
                    };
                    await cmd.Connection.CloseAsync();
                }
            }

            return null;
        }


        /// <summary>
        /// RemoveMatch
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveMatch(long userId, long matchId)
        {
            var match = await _context.Match.Where(u => u.MatchId == matchId && (u.MaleUser == userId || u.FemaleUser == userId)).FirstOrDefaultAsync();
            match.IsActive = false;
            await _context.SaveChangesAsync();

            _cache.Set(match.MatchId.ToString(), matchId);
            return true;
        }
        public async Task SendNotification(string matchId, long to)
        {
            if (_cache.Get(matchId) == null)
            {
                var cacheModel = _cache.Get<string>(to.ToString());
                string deviceToken = string.Empty;

                if (cacheModel == null)
                {
                    deviceToken = await _userRepository.GetDeviceToken(to);
                    _cache.Set(to.ToString(), deviceToken);
                }
                else
                    deviceToken = cacheModel.ToString();

                if (!string.IsNullOrEmpty(deviceToken))
                {
                    await _notificationService.SendNotification(new NotificationMessageHubRequest
                    {
                        to = deviceToken,
                        data = new MessageDataHub
                        {
                            matchId = matchId
                        },
                        notification = new Notification
                        {
                            title = "Midpot",
                            body = "Yeni bir eşleşmeniz var."
                        }
                    });
                }
            }
        }
    }
}
