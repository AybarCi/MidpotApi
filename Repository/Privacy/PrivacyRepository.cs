using System;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.Data;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Privacy.Interface;
using Microsoft.EntityFrameworkCore;

namespace DatingWeb.Repository.Privacy
{
    public class PrivacyRepository : IPrivacyRepository
    {
        private readonly ApplicationDbContext _context;
        public PrivacyRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
        }

        public async Task<GetPrivacyResponse> GetContent(string languageCode, string contentKey)
        {
            var privacy = await _context.Privacy.Where(x => x.LanguageCode == languageCode && x.ContentKey == contentKey).FirstOrDefaultAsync();
            if (privacy == null)
            {
                return new GetPrivacyResponse();
            }
            var getPrivacyResponse = new GetPrivacyResponse
            {
                ContentKey = privacy.ContentKey,
                Content = privacy.Content
            };
            return getPrivacyResponse;
        }
    }
}

