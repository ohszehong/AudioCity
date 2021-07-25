using AudioCity.Areas.Identity.Data;
using AudioCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    public class SellerDashboardController : Controller
    {

        private readonly UserManager<AudioCityUser> _userManager;

        public SellerDashboardController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [Authorize(Roles = "Seller")]
        [Route("SellerDashboard/{Id}")]
        [Route("SellerDashboard")]
        public async Task<IActionResult> Index(string PartialPage = "_ActiveGigsPartial")
        {
            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.PartialPage = PartialPage;

            if (PartialPage == "_ActiveGigsPartial")
            {
                //read gigs data created by this seller 
                string UserId = _userManager.GetUserId(HttpContext.User);
                List<Gig> Gigs = GigModelHelper.GetGigsFromSeller(UserId);


                AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

                //create GigDetailViewModel instances and pass them to SellerDashboard index page 
                List<GigDetailViewModel> GigDetails = new List<GigDetailViewModel>();

                foreach (Gig Gig in Gigs)
                {
                    CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();

                    //Get list of blob items 
                    CloudBlockBlob Portfolio = Container.GetBlockBlobReference(Gig.PortfolioFilePath);
                    CloudBlockBlob Thumbnail = Container.GetBlockBlobReference(Gig.ThumbnailFilePath);

                    GigDetailViewModel GigDetail = new GigDetailViewModel { Gig = Gig, Portfolio = Portfolio, Thumbnail = Thumbnail, User = AudioCityUser };

                    GigDetails.Add(GigDetail);
                }
                return View(GigDetails);
            }

            return View();
        }

    }
}
