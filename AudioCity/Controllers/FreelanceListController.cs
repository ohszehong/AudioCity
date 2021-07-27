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
    [Authorize]
    public class FreelanceListController : Controller
    {

        private readonly UserManager<AudioCityUser> _userManager;
        public FreelanceListController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string gigName = null, string gigCategory = null, string gigMinBudget = null, string gigMaxBudget = null, string gigDeliveryDays = null)
        {
            var SearchQueries = new string[5] { gigName, gigCategory, gigMinBudget, gigMaxBudget, gigDeliveryDays };

            ViewBag.SearchQueries = SearchQueries;

            double MinBudget;
            double MaxBudget;
            int DeliveryDays;

            if (gigMinBudget == null)
            {
                MinBudget = 0;
            }

            else
            {
                MinBudget = Convert.ToDouble(gigMinBudget);
            }

            if(gigMaxBudget == null)
            {
                MaxBudget = 1.7976931348623157E+308;
            }

            else
            {
                MaxBudget = Convert.ToDouble(gigMaxBudget);
            }

            if(gigDeliveryDays == null)
            {
                DeliveryDays = -1;
            }

            else
            {
                DeliveryDays = Convert.ToInt32(gigDeliveryDays);
            }

            if(gigCategory == "All Category")
            {
                gigCategory = null;
            }

            System.Diagnostics.Debug.WriteLine("data: " + gigName + " " + gigCategory + " " + MinBudget + " " + MaxBudget + " " + DeliveryDays);
             
            //get gigs using GigModelHelper 
            List<Gig> Gigs = GigModelHelper.SearchGigs(gigName, gigCategory, MinBudget, MaxBudget, DeliveryDays);

            AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

            //convert to GigDetailViewModel instances 
            CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();
            List<GigDetailViewModel> GigsDetailViewModel = new List<GigDetailViewModel>();

            foreach (Gig Gig in Gigs)
            {
                //Get list of blob items 
                CloudBlockBlob Portfolio = Container.GetBlockBlobReference(Gig.PortfolioFilePath);
                CloudBlockBlob Thumbnail = Container.GetBlockBlobReference(Gig.ThumbnailFilePath);

                //round down rating score to display number of stars 
                int RoundedRating = (int)Math.Floor(Gig.Rating);

                GigDetailViewModel GigDetail = new GigDetailViewModel { Gig = Gig, Portfolio = Portfolio, Thumbnail = Thumbnail, User = AudioCityUser, RoundedRating = RoundedRating };

                GigsDetailViewModel.Add(GigDetail);
            }

            return View(GigsDetailViewModel);

            //get all gigs using GigModelHelper 
            /* List<Gig> AllGigs = GigModelHelper.GetAllGigs();

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
             }*/
        }


        public async Task<IActionResult> GigDetail(string GigId)
        {
            //get the specific Gig 
            Gig SelectedGig = GigModelHelper.GetGig(GigId);
            AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

            //convert to GigDetailViewModel instance
            GigDetailViewModel SelectedGigDetail = GigModelHelper.ConvertToGigDetailViewModel(SelectedGig, AudioCityUser);

            //retrieve all comments for the selected gig 
            List<CustomerReviewEntity> CustomerReviews = CustomerReviewEntityHelper.GetGigReviews(GigId);
            SelectedGigDetail.CustomerReviews = CustomerReviews;

            if(TempData["ExceedMaxOrdersCount"] != null)
            {
                ViewBag.ExceedMaxOrdersCount = TempData["ExceedMaxOrdersCount"];
                ViewBag.GigActiveOrdersCount = TempData["GigActiveOrdersCount"];
            }
            return View(SelectedGigDetail);
        }

    }
}
