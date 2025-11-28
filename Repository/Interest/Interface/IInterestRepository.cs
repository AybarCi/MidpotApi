using DatingWeb.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Interest.Interface
{
    public interface IInterestRepository
    {
        Task<IEnumerable<DatingWeb.Data.DbModel.Interest>> GetAllInterestsAsync();
        Task<DatingWeb.Data.DbModel.Interest> GetInterestByIdAsync(Guid id);
    }
}
