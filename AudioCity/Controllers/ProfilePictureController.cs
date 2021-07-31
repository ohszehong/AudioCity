using AudioCity.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    public class ProfilePictureController : Controller
    {
        
        private readonly UserManager<AudioCityUser> _userManager;

        public ProfilePictureController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile ProfilePicture)
        {
            AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

            using (var ms = new MemoryStream())
            {
                await ProfilePicture.CopyToAsync(ms);
                byte[] ProfilePictureBytes = ms.ToArray();

                AudioCityUser.ProfilePicture = ProfilePictureBytes;

                await _userManager.UpdateAsync(AudioCityUser);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
