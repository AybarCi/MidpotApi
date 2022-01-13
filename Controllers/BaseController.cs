using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingWeb.Controllers
{
    public class BaseController : ControllerBase
    {
        protected long GetUserId => (long.Parse(User.FindFirst("UserId")?.Value));
        protected string GetPersonName => (User.FindFirst("PersonName")?.Value);
        protected DateTime GetBirthDate => (DateTime.Parse(User.FindFirst("Birthdate")?.Value));
        protected bool GetGender => (bool.Parse(User.FindFirst("Gender")?.Value));
        protected bool GetPreferredGender => (bool.Parse(User.FindFirst("PreferredGender")?.Value));
    }
}
