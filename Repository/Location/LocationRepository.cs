using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Location.Interface;
using Microsoft.EntityFrameworkCore;

namespace DatingWeb.Repository.Location
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;
        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddLocation(AddLocationRequest addLocationRequest)
        {
            var newLocation = new Data.DbModel.Location
            {
                City = addLocationRequest.City,
                Latitude = addLocationRequest.Latitude,
                Longitude = addLocationRequest.Longitude
            };
            await _context.AddAsync(newLocation);
            var val = await _context.SaveChangesAsync();
            return val == 1 ? true : false;
        }

        public async Task<LocationResponse> GetLocation(int id)
        {
            try
            {
                return await _context.Location.Where(x => x.Id == id).Select(x =>
                    new LocationResponse
                    {
                        City = x.City,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        LocationId = x.Id
                    }
                ).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return new LocationResponse();
            }
            
        }

        public async Task<List<LocationResponse>> GetLocations()
        {
            try
            {
                return await _context.Location.Select(x => new LocationResponse
                {
                    City = x.City,
                    Latitude = x.Latitude,
                    LocationId = x.Id,
                    Longitude = x.Longitude
                }).ToListAsync();
            }
            catch (Exception)
            {
                return new List<LocationResponse>();
            }
        }
    }
}

