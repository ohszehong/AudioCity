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
    public class FreelanceListController : Controller
    {

        private readonly UserManager<AudioCityUser> _userManager;
        public FreelanceListController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            //get all gigs using GigModelHelper 
            List<Gig> AllGigs = GigModelHelper.GetAllGigs();

            AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

            //convert to GigDetailViewModel instances 
            CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();
            List<GigDetailViewModel> AllGigsDetailViewModel = new List<GigDetailViewModel>();

            foreach(Gig Gig in AllGigs)
            {
                //Get list of blob items 
                CloudBlockBlob Portfolio = Container.GetBlockBlobReference(Gig.PortfolioFilePath);
                CloudBlockBlob Thumbnail = Container.GetBlockBlobReference(Gig.ThumbnailFilePath);

                GigDetailViewModel GigDetail = new GigDetailViewModel { Gig = Gig, Portfolio = Portfolio, Thumbnail = Thumbnail, User = AudioCityUser };

                AllGigsDetailViewModel.Add(GigDetail);
            }

            return View(AllGigsDetailViewModel);
        }


        [Authorize]
        public async Task<IActionResult> GigDetail(string GigId)
        {
            //get the specific Gig 
            Gig SelectedGig = GigModelHelper.GetGig(GigId);
            AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

            //convert to GigDetailViewModel instance
            GigDetailViewModel SelectedGigDetail = GigModelHelper.ConvertToGigDetailViewModel(SelectedGig, AudioCityUser);

            return View(SelectedGigDetail);
        }

    }
}
