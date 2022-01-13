using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.BLL;

namespace DatingWeb.Extension
{
    public static class Extensions
    {
        public static int ToBirthDateFormat(this DateTime birthDate)
        {
            return ((DateTime.UtcNow.Year - birthDate.Year) * 372 + (DateTime.UtcNow.Month - birthDate.Month) * 31 + (DateTime.UtcNow.Day - birthDate.Day)) / 372;
        }

        public static DateTime ToBirthDateFormat(this int birthDate)
        {
            return DateTime.UtcNow.AddYears(-birthDate);
        }

        /// <summary>
        /// create profile photo link
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToProfilePhoto(this string input)
        {
            return Constants.PhotoStorage(input);
        }

        /// <summary>
        /// profile photo link convert to profile photo guid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToProfilePhotoId(this string input)
        {
            return Constants.PhotoStorageId(input);
        }
    }
}
