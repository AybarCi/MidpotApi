using DatingWeb.Model.Response;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Settings.Interface
{
    public interface ISettingRepository
    {
        Task<bool> AddSetting(string key, string value);
        Task<SettingResponse> GetSetting(string key);
        Task<bool> UpdateSetting(string key, string value);
        Task<string> UnlockPhone(string phoneNumber);
    }
}
