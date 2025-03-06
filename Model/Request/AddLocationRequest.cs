using System;
namespace DatingWeb.Model.Request
{
	public class AddLocationRequest
	{
		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public string City { get; set; }
	}
}

