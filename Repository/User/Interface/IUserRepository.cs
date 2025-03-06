using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Repository.User.Interface
{
    public interface IUserRepository
    {
        Task<UserResponse> GetProfile(long userId);
        Task<bool> UpdateProfileSettings(long userId, string description, int fromAge, int untilAge, string school, string job, string deviceToken, bool ghostMode, string personName);
        Task UpdateProfilePhoto(long userId, string fileName);
        Task<string> GetDeviceToken(long userId);
        Task<string> UpdateProfilePhotoNew(long userId, IFormFile file);
        Task<bool> DeleteUser(long userId);
        Task<List<UserResponse>> GetAll();
        Task<int> GetAllUsersCount();
        Task<List<UserResponse>> GetUsersWeekly();
        Task<int> GetUsersWeeklyCount();
        Task<List<UserResponse>> GetDeletedUsers();
        Task<int> GetDeletedUsersCount();
        Task<bool> AddUser(List<AddUserRequest> addUserRequest);
    }
}
