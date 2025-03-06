using System;
using System.Threading.Tasks;
using DatingWeb.Model.Response;

namespace DatingWeb.Repository.Privacy.Interface
{
	public interface IPrivacyRepository
	{
		Task<GetPrivacyResponse> GetContent(string languageCode, string contentKey);
	}
}

