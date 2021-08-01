using AudioCity.Areas.Identity.Data;
using AudioCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    [Authorize(Roles = "Seller")]
    public class CreateNewGigController : Controller
    {

        private readonly UserManager<AudioCityUser> _userManager;

        public CreateNewGigController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [HttpGet("CreateNewGig/{id}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("CreateNewGig/{id}")]
        [RequestSizeLimit(100_000_000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(GigViewModel GigViewModel)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            foreach (ModelError error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                //return to previewGig view along with the model...

                //In here, first we convert IFormFile of thumbnail to byte[] to be stored in database.
                //Then, upload the image and video to blob storage as video and image size are huge.
                //The blob directory to store the image and video should be named to Gig Id + extension 
                //use IO library to get extension 

                string UserId = _userManager.GetUserId(HttpContext.User);
                AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

                Guid guid = Guid.NewGuid();
                GigViewModel.Id = guid.ToString();

                string PortfolioExt = System.IO.Path.GetExtension(GigViewModel.Portfolio.FileName);
                string ThumbnailExt = System.IO.Path.GetExtension(GigViewModel.Thumbnail.FileName);

                //upload video and image to blob storage and get the reference
                CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();

                CloudBlockBlob BlobItem = null;
                string PortfolioBlobReference = GigViewModel.Id + PortfolioExt;
                string ThumbnailBlobReference = GigViewModel.Id + ThumbnailExt;

                try
                {
                    BlobItem = Container.GetBlockBlobReference(PortfolioBlobReference);
                    var PortfolioStream = GigViewModel.Portfolio.OpenReadStream();
                    BlobItem.UploadFromStreamAsync(PortfolioStream).Wait();

                    BlobItem = Container.GetBlockBlobReference(ThumbnailBlobReference);
                    var ThumbnailStream = GigViewModel.Thumbnail.OpenReadStream();
                    BlobItem.UploadFromStreamAsync(ThumbnailStream).Wait();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured when uploading video to blob storage: ", Ex.ToString());
                }


                //finally, create Gig instance and pass it to GigModelHelper to deal with CRUD
                Gig NewGig = new Gig {Id = GigViewModel.Id, CreatedBy = UserId, Title = GigViewModel.Title, Description = GigViewModel.Description, EstimatedDeliveryDays = GigViewModel.EstimatedDeliveryDays, Price = GigViewModel.Price, Category = GigViewModel.Category, ThumbnailFilePath = ThumbnailBlobReference, PortfolioFilePath = PortfolioBlobReference, PublishedOn = DateTime.Now, ArtistName = AudioCityUser.FullName, MaxOrderCount = GigViewModel.MaxOrderCount };

                System.Diagnostics.Debug.WriteLine("valid...");

                return RedirectToAction("PreviewGig", "CreateNewGig", NewGig);
            }

            return View();

        }

        [RequestSizeLimit(100_000_000)]
        public async  Task<IActionResult> PreviewGig(Gig NewGig)
        {
            CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();

            //Get list of blob items 
            CloudBlockBlob Portfolio = Container.GetBlockBlobReference(NewGig.PortfolioFilePath);
            CloudBlockBlob Thumbnail = Container.GetBlockBlobReference(NewGig.ThumbnailFilePath);

            AudioCityUser AudioCityUser = await _userManager.GetUserAsync(User);

            int RoundedRating = (int)Math.Floor(NewGig.Rating);

            GigDetailViewModel GigDetail = new GigDetailViewModel { Gig = NewGig, Portfolio = Portfolio, Thumbnail = Thumbnail, User = AudioCityUser, RoundedRating = RoundedRating };
            return View(GigDetail);
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public IActionResult CreateGig(Gig NewGig )
        {

            GigModelHelper.CreateGig(NewGig);

            return RedirectToAction("Index", "SellerDashboard");
        }

        [HttpPost]
        public IActionResult CancelCreateGig(Gig NewGig)
        {
            //delete portfolio and thumbnail blob from azure blob storage
            CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();

            try
            {
                CloudBlockBlob Portfolio = Container.GetBlockBlobReference(NewGig.PortfolioFilePath);
                CloudBlockBlob Thumbnail = Container.GetBlockBlobReference(NewGig.ThumbnailFilePath);
                Portfolio.DeleteIfExistsAsync().Wait();
                Thumbnail.DeleteIfExistsAsync().Wait();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error when deleting blob from azure blob storage: " + Ex.ToString());
            }

            return RedirectToAction("Index", "SellerDashboard");
        }

    }
}
