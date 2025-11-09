using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Settings.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.BLL;
using Microsoft.Extensions.Configuration;

namespace DatingWeb.Repository.Settings
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICache _cache;
        readonly IConfiguration _configuration;

        public SettingRepository(ApplicationDbContext context, ICache cache, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _configuration = configuration;
            _cache = cache;
        }

        /// <summary>
        /// AddSetting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddSetting(string key, string value)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            await _context.Setting.AddAsync(new Setting { Key = key, Value = value });
            var val = await _context.SaveChangesAsync();

            _cache.Set(key, value, TimeSpan.FromHours(1000));

            return val > 0 ? true : false;
        }

        /// <summary>
        /// GetAgreement
        /// </summary>
        /// <returns></returns>
        public async Task<SettingResponse> GetSetting(string key)
        {
            var value = _cache.Get<string>(key);
            if (!string.IsNullOrEmpty(value))
                return new SettingResponse { Key = key, Value = value };

            var setting = await _context.Setting.Where(x => x.Key == key).FirstOrDefaultAsync();
            if (setting != null)
            {
                _cache.Set(key, setting.Value, TimeSpan.FromHours(1000));
                return new SettingResponse { Key = key, Value = setting.Value };
            }
            return new SettingResponse { };
        }

        /// <summary>
        /// UpdateSetting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSetting(string key, string value)
        {
            var setting = await _context.Setting.Where(x => x.Key == key).FirstOrDefaultAsync();
            setting.Value = value;
            int val = 0;
            try
            {
                val = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }


            _cache.Set(key, setting.Value, TimeSpan.FromHours(1000));

            return val > 0 ? true : false;
        }

        public async Task<string> UnlockPhone(string phoneNumber)
        {
            return await Task.FromResult(phoneNumber.Decrypt(_configuration));
        }
    }
}
