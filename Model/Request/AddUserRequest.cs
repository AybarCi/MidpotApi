using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DatingWeb.Model.Request
{
	public class AddUserRequest
	{
        public string PersonName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public bool PreferredGender { get; set; }
        public int FromAge { get; set; }
        public int UntilAge { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string UserName { get; set; }
    }    
}

