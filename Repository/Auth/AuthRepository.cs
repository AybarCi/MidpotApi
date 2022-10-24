using DatingWeb.BLL;
using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Extension;
using DatingWeb.Helper;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Auth.Interface;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ICache _cache;
        private readonly ISmsService _smsService;
        public AuthRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext context, ICache cache, ISmsService smsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _cache = cache;
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _smsService = smsService;
        }

        public async Task Register(string personName, string password, string phoneNumber, string birthDate, bool gender, bool preferredGender, string deviceToken, bool platform, int fromAge, int untilAge, string school, string job)
        {
            string confirmCode = new Random().Next(0, 999999).ToString("D6");

            var checkUser = await _context.ApplicationUsers.Where(x => x.PhoneNumber == phoneNumber.Encrypt(_configuration) && x.IsDelete == true).FirstOrDefaultAsync();
            if (checkUser != null)
            {
                if (checkUser.LockoutEnd < DateTime.UtcNow.AddHours(8))
                    throw new ApiException("Kullanıcı silme işlemi devam ediyor yarın tekrar deneyiniz!");
                else
                {
                    checkUser.PersonName = personName;
                    checkUser.BirthDate = Convert.ToDateTime(birthDate);
                    checkUser.ConfirmCode = confirmCode;
                    checkUser.Gender = gender;
                    checkUser.PreferredGender = preferredGender;
                    checkUser.LockoutEnabled = true;
                    checkUser.LockoutEnd = DateTime.UtcNow;
                    checkUser.PhoneNumberConfirmed = false;
                    checkUser.Description = "";
                    checkUser.ProfilePhoto = "";
                    checkUser.DeviceToken = deviceToken;
                    checkUser.Platform = platform;
                    checkUser.LastLoginDate = DateTime.UtcNow;
                    checkUser.RefreshTokenExpireDate = DateTime.UtcNow;
                    checkUser.FromAge = fromAge;
                    checkUser.UntilAge = untilAge;
                    checkUser.School = school;
                    checkUser.Job = job;
                    checkUser.IsDelete = false;
                    checkUser.ProfilePhoto = string.Format("{0}", Guid.NewGuid().ToString()).ToProfilePhoto();
                    checkUser.CreateDate = DateTime.UtcNow;
                    
                    await _context.SaveChangesAsync();

                    await SendSms(phoneNumber, confirmCode);
                }

                return;
            }

            var user = new ApplicationUser
            {
                UserName = phoneNumber.Encrypt(_configuration),
                Email = null,
                PersonName = personName,
                PhoneNumber = phoneNumber.Encrypt(_configuration),
                BirthDate = Convert.ToDateTime(birthDate),
                ConfirmCode = confirmCode,
                Gender = gender,
                PreferredGender = preferredGender,
                LockoutEnabled = true,
                LockoutEnd = DateTime.UtcNow,
                PhoneNumberConfirmed = false,
                Description = "",
                ProfilePhoto = string.Format("{0}", Guid.NewGuid().ToString()).ToProfilePhoto(),
                DeviceToken = deviceToken,
                Platform = platform,
                LastLoginDate = DateTime.UtcNow,
                RefreshTokenExpireDate = DateTime.UtcNow,
                FromAge = fromAge,
                UntilAge = untilAge,
                School = school,
                Job = job,
                IsDelete = false,
                CreateDate = DateTime.UtcNow,
                GhostMode = false
        };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await SendSms(phoneNumber, confirmCode);
            else
            {
                if (result.Errors.First().Code == "DuplicateUserName")
                    throw new ApiException("Kullanıcı Mevcut!");

                string errors = string.Empty;
                foreach (var item in result.Errors)
                    errors += string.Format("{0} - {1} | ", item.Code, item.Description);

                throw new ApiException(errors);
            }
        }

        public async Task<TokenResponse> ConfirmPhone(string phoneNumber, string confirmCode)
        {
            var user = await _context.ApplicationUsers.Where(x => x.PhoneNumber == phoneNumber.Encrypt(_configuration) && x.LockoutEnd < DateTime.UtcNow.AddDays(5)).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.ConfirmCode.Equals(confirmCode))
                {
                    user.PhoneNumberConfirmed = true;
                    user.LockoutEnabled = false;
                    user.RefreshToken = CreateRefreshToken();
                    user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:RefreshTokenLifetime"));
                    await _context.SaveChangesAsync();
                    return GenerateToken(user);
                }
                else
                    throw new ApiException("Onay kodu hatalı!");
            }
            else
                throw new ApiException("Kullanıcı bulunamadı!");
        }

        public async Task<TokenResponse> Login(string phoneNumber, string password, string deviceToken, bool platform)
        {
            var user = await _context.ApplicationUsers.Where(x => x.PhoneNumber == phoneNumber.Encrypt(_configuration) && x.IsDelete == false).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.LockoutEnabled)
                    throw new ApiException("Kısıtlanmış hesap!");

                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    user.Platform = platform;
                    if (!string.IsNullOrEmpty(deviceToken))
                        user.DeviceToken = deviceToken;

                    user.RefreshToken = CreateRefreshToken();
                    user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:RefreshTokenLifetime"));
                    user.LastLoginDate = DateTime.UtcNow;
                    int val = await _context.SaveChangesAsync();
                    _cache.Remove(user.Id.ToString());
                    return GenerateToken(user);
                }
                else
                    throw new ApiException("Telefon veya şifre hatalı!");
            }
            else
                throw new ApiException("Kullanıcı bilgisi bulunamadı!");
        }

        private async Task<bool> SendSms(string phoneNumber, string confirmCode)
        {
            //180 sn de bir at 5 haktan sonra 1 gün iptal
            return (await _smsService.SendSms(new SendSmsRequest { PhoneNumber = phoneNumber, ConfirmCode = confirmCode }));
        }

        public async Task<string> RequestCodeAgain(string phoneNumber)
        {
            var user = await _context.ApplicationUsers.Where(x => x.PhoneNumber == phoneNumber.Encrypt(_configuration)).FirstOrDefaultAsync();
            if (user != null)
            {
                string confirmCode = new Random().Next(0, 999999).ToString("D6");

                user.ConfirmCode = confirmCode;
                user.LockoutEnd = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                //returnu sil
                await SendSms(phoneNumber, confirmCode);
                return confirmCode;
            }
            else
                throw new ApiException("Kullanıcı bilgisi bulunamadı!");
        }

        public async Task<bool> ForgotPassword(string phoneNumber)
        {
            var user = await _context.ApplicationUsers.Where(x => x.PhoneNumber == phoneNumber.Encrypt(_configuration)).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.LockoutEnabled)
                    throw new ApiException("Kısıtlanmış hesap!");

                string confirmCode = new Random().Next(0, 999999).ToString("D6");

                user.ConfirmCode = confirmCode;
                user.LockoutEnd = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                //sms at
                await SendSms(phoneNumber, confirmCode);
                return true;
            }
            else
                throw new ApiException("Kullanıcı bilgisi bulunamadı!");
        }

        public async Task<TokenResponse> ForgotPasswordConfirm(string phoneNumber, string confirmCode, string password)
        {
            var user = await _context.ApplicationUsers.Where(x => x.PhoneNumber == phoneNumber.Encrypt(_configuration) && x.LockoutEnd < DateTime.UtcNow.AddDays(5)).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.ConfirmCode.Equals(confirmCode))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var val = await _userManager.ResetPasswordAsync(user, token, password);

                    user.PhoneNumberConfirmed = true;
                    user.LockoutEnabled = false;
                    user.RefreshToken = CreateRefreshToken();
                    user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:RefreshTokenLifetime"));
                    await _context.SaveChangesAsync();
                    return GenerateToken(user);
                }
                else
                    throw new ApiException("Onay kodu hatalı!");
            }
            else
                throw new ApiException("Kullanıcı bulunamadı!");
        }

        public async Task<bool> ChangePassword(long userId, string password, string newPassword)
        {
            var user = await _context.ApplicationUsers.Where(x => x.Id == userId && x.LockoutEnabled == false).FirstOrDefaultAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, password, newPassword);
                if (result.Succeeded)
                    return true;
                else
                    throw new ApiException("Kullanıcı bilgisi hatalı!");
            }
            throw new ApiException("Kullanıcı bilgisi bulunamadı!");
        }

        public async Task<bool> LogOut(long userId)
        {
            var user = await _context.ApplicationUsers.Where(x => x.Id == userId && x.LockoutEnabled == false).FirstOrDefaultAsync();
            if (user != null)
            {
                user.DeviceToken = "";
                await _context.SaveChangesAsync();
            }
            else
                throw new ApiException("Kullanıcı bilgisi bulunamadı!");

            return true;
        }

        public async void RefreshSignInAsync(ApplicationUser user)
        {
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task<TokenResponse> RefreshToken(string refreshToken)
        {
            var user = await _context.ApplicationUsers.Where(x => x.RefreshToken == refreshToken && x.LockoutEnabled == false && x.RefreshTokenExpireDate > DateTime.UtcNow).FirstOrDefaultAsync();
            if (user != null)
            {
                user.RefreshToken = CreateRefreshToken();
                user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:RefreshTokenLifetime"));
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return GenerateToken(user);
            }
            throw new ApiException("Kimlik doğrulanamadı!");
        }

        private static string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }

        private TokenResponse GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<String>("Tokens:Key"));

            ClaimsIdentity getClaims()
            {
                return new ClaimsIdentity(getClaims());

                Claim[] getClaims()
                {
                    var claims = new Claim[]
                    {
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("PersonName", user.PersonName),
                        new Claim("BirthDate", user.BirthDate.ToString()),
                        new Claim("Gender", user.Gender.ToString()),
                        new Claim("PreferredGender", user.PreferredGender.ToString()),
                        new Claim("ProfilePhoto", user.ProfilePhoto.ToString())
                    };
                    return claims.ToArray();
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = getClaims(),
                Expires = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:Lifetime")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = user.RefreshToken,
                Expiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:Lifetime")),
                UserId = user.Id,
                PersonName = user.PersonName,
                BirthDate = user.BirthDate.ToBirthDateFormat(),
                ProfilePhoto = user.ProfilePhoto,
                Gender = user.Gender,
                PreferredGender = user.PreferredGender,
                FromAge = user.FromAge,
                UntilAge = user.UntilAge,
                Description = user.Description,
                School = user.School,
                Job = user.Job,
                CreateDate = user.CreateDate,
                GhostMode = user.GhostMode
            };
        }

        //private string GetToken(IEnumerable<Claim> claims)
        //{
        //    var utcNow = DateTime.UtcNow;
        //    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<String>("Tokens:Key")));
        //    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        //    var jwt = new JwtSecurityToken(
        //        signingCredentials: signingCredentials,
        //        claims: claims,
        //        notBefore: utcNow,
        //        expires: utcNow.AddMinutes(_configuration.GetValue<int>("Tokens:Lifetime")),
        //        audience: _configuration.GetValue<String>("Tokens:Audience"),
        //        issuer: _configuration.GetValue<String>("Tokens:Issuer")
        //    );
        //    return new JwtSecurityTokenHandler().WriteToken(jwt);
        //}

        //private TokenResponse GenerateTokenOld(ApplicationUser user)
        //{
        //    var claims = new Claim[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //        new Claim(JwtRegisteredClaimNames.NameId, user.UserName /*TODO => Şifrele*/),
        //        new Claim(JwtRegisteredClaimNames.GivenName, user.PersonName),
        //        new Claim(JwtRegisteredClaimNames.Gender, user.Gender.ToString()),
        //        new Claim(JwtRegisteredClaimNames.Birthdate, user.BirthDate.ToBirthDateFormat().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Prn, user.PreferredGender.ToString()),
        //        new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString()),
        //    };
        //    return new TokenResponse
        //    {
        //        Token = GetToken(claims),
        //        RefreshToken = CreateRefreshToken(),
        //        Expiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Tokens:Lifetime")),
        //        BirthDate = user.BirthDate.ToBirthDateFormat(),
        //        PersonName = user.PersonName,
        //        UserId = user.Id,
        //        Description = user.Description,
        //        ProfilePhoto = user.ProfilePhoto,
        //        Gender = user.Gender,
        //        PreferredGender = user.PreferredGender
        //    };
        //}
    }
}
