using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace DatingWeb.Data.DbModel
{
    public class ApplicationUser : IdentityUser<long>
    {
        public bool Gender { get; set; }
        public bool PreferredGender { get; set; }
        public DateTime BirthDate { get; set; }
        public string ConfirmCode { get; set; }
        public string PersonName { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public string ProfilePhoto { get; set; }
        public string DeviceToken { get; set; }
        public bool Platform { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int FromAge { get; set; }
        public int UntilAge { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
        [MaxLength(250)]
        public string School { get; set; }
        [MaxLength(250)]
        public string Job { get; set; }
        public bool IsDelete { get; set; } = false;
        public DateTime CreateDate { get; set; }
        public bool GhostMode { get; set; }
        
        // Event System Fields
        public int MissedEventCount { get; set; } = 0;
        public DateTime? IsSuspendedUntil { get; set; }
    }
}
