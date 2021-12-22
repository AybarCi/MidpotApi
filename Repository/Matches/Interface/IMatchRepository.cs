using DatingWeb.Model.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Matches.Interface
{
    public interface IMatchRepository
    {
        Task<List<MatchResponse>> MatchMachine(long userId, bool gender, bool preferredGender, DateTime lastMatchDate, double latitude, double longitude);
        Task<bool> RemoveMatch(long userId, long matchId);
    }
}
