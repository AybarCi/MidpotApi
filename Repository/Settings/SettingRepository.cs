using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Settings.Interface;
using DatingWeb.BLL;
using Microsoft.EntityFrameworkCore;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// AddSetting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty", nameof(value));

            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            await _context.Setting.AddAsync(new Setting { Key = key, Value = value });
            var val = await _context.SaveChangesAsync();

            _cache.Set(key, value, TimeSpan.FromHours(1000));

            return val > 0;
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
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty", nameof(value));

            var setting = await _context.Setting.Where(x => x.Key == key).FirstOrDefaultAsync();
            if (setting == null)
                throw new KeyNotFoundException($"Setting with key '{key}' not found");

            setting.Value = value;
            int val = await _context.SaveChangesAsync();

            _cache.Set(key, setting.Value, TimeSpan.FromHours(1000));

            return val > 0;
        }

        public async Task<string> UnlockPhone(string phoneNumber)
        {
            return await Task.FromResult(phoneNumber.Decrypt(_configuration));
        }
    }
}
