using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;

namespace DatingWeb.Repository.Location.Interface
{
    public interface ILocationRepository
    {
        Task<LocationResponse> GetLocation(int id);
        Task<List<LocationResponse>> GetLocations();
        Task<bool> AddLocation(AddLocationRequest addLocationRequest);
    }
}

