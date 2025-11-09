using DatingWeb.CacheService.Interface;
using DatingWeb.Helper;
using DatingWeb.Model.Request;
using DatingWeb.Repository.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRedisCache _redisCache;

        public AuthController(IAuthRepository userRepository, IRedisCache redisCache)
        {
            _authRepository = userRepository;
            _redisCache = redisCache;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest model)
        {
            if (!string.IsNullOrEmpty(model.PhoneNumber) || !string.IsNullOrEmpty(model.Password))
            {
                if (model.PhoneNumber.Length != 10)
                    throw new ApiException("Telefon 10 karakter olmalı!");

                if (!Regex.Match(model.PhoneNumber, @"^[0-9]*$").Success)
                    throw new ApiException("Sadece rakam giriniz!");

                if (model.Password.Length < 6)
                    throw new ApiException("Şifre minimum 6 karakter olmalı!");

                if ((model.FromAge < 18 && model.FromAge > 99) && (model.UntilAge < 17 && model.UntilAge > 99) && model.FromAge > model.UntilAge)
                    throw new ApiException("Yaş aralığı hatalı");

                await _authRepository.Register(model.PersonName, model.Password, model.PhoneNumber, model.BirthDate,
                    model.Gender, model.PreferredGender, model.DeviceToken, model.Platform, model.FromAge, model.UntilAge, model.School, model.Job);
            }
            else
                throw new ApiException("Telefon veya şifre boş geçilemez!");
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            if (!string.IsNullOrEmpty(model.PhoneNumber) || !string.IsNullOrEmpty(model.Password))
            {
                if (model.PhoneNumber.Length != 10)
                    throw new ApiException("Telefon 10 karakter olmalı!");

                if (!Regex.Match(model.PhoneNumber, @"^[0-9]*$").Success)
                    throw new ApiException("Sadece rakam giriniz!");

                if (model.Password.Length < 6)
                    throw new ApiException("Şifre minimum 6 karakter olmalı!");

                string cacheKey = $"login_attempt_{model.PhoneNumber}";

                // Rate limiting için cache kontrolü
                var cachedAttempt = await _redisCache.GetAsync<int>(cacheKey);
                if (cachedAttempt >= 5) // 5 denemeden fazla
                {
                    throw new ApiException("Çok fazla giriş denemesi. Lütfen 15 dakika sonra tekrar deneyin.");
                }

                try
                {
                    var result = await _authRepository.Login(model.PhoneNumber, model.Password, model.DeviceToken, model.Platform);

                    // Başarılı girişte cache'i temizle
                    await _redisCache.RemoveAsync(cacheKey);

                    return Ok(result);
                }
                catch (ApiException)
                {
                    // Başarısız girişte cache'i artır
                    await _redisCache.SetAsync(cacheKey, cachedAttempt + 1, TimeSpan.FromMinutes(15));
                    throw;
                }
            }
            else
                throw new ApiException("Telefon veya şifre boş geçilemez!");

        }

        [HttpPost("request-code-again")]
        public async Task<IActionResult> RequestCodeAgain([FromBody] RequestCodeAgainRequest model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new ApiException("Telefon boş geçilemez!");

            return Ok(await _authRepository.RequestCodeAgain(model.PhoneNumber));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new ApiException("Telefon boş geçilemez!");

            return Ok(await _authRepository.ForgotPassword(model.PhoneNumber));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.NewPassword))
                throw new ApiException("Şifre alanları boş geçilemez!");

            return Ok(await _authRepository.ChangePassword(this.GetUserId, model.Password, model.NewPassword));
        }

        [HttpPost("forgot-password-confirm")]
        public async Task<IActionResult> ForgotPasswordConfirm([FromBody] ForgotPasswordConfirmRequest model)
        {
            if (string.IsNullOrEmpty(model.ConfirmCode) || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.Password))
                throw new ApiException("Telefon numarası veya onay kodu boş geçilemez");

            return Ok(await _authRepository.ForgotPasswordConfirm(model.PhoneNumber, model.ConfirmCode, model.Password));
        }

        [HttpPost("ConfirmPhone")]
        public async Task<IActionResult> ConfirmPhone([FromBody] ConfirmPhoneRequest model)
        {
            if (string.IsNullOrEmpty(model.ConfirmCode) || string.IsNullOrEmpty(model.PhoneNumber))
                throw new ApiException("Telefon numarası veya onay kodu boş geçilemez");
            return Ok(await _authRepository.ConfirmPhone(model.PhoneNumber, model.ConfirmCode));
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(await _authRepository.LogOut(this.GetUserId));
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            if (string.IsNullOrEmpty(model.RefreshToken) || model.RefreshToken.Length != 44)
                throw new ApiException("null");

            return Ok(await _authRepository.RefreshToken(model.RefreshToken));
        }
    }
}
