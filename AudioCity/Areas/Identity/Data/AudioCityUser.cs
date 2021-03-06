using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AudioCity.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AudioCityUser class
    public class AudioCityUser : IdentityUser
    {

        [PersonalData]
        public string FullName { get; set; }

        [PersonalData]
        public DateTime Dob { get; set; }
       
        [PersonalData]
        public string ContactNo { get; set; }

        [PersonalData]
        public string Role { get; set; }

        [PersonalData]
        public byte[] ProfilePicture { get; set; }

    }
}
