using AudioCity.Areas.Identity.Data;
using AudioCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    [Authorize]
    public class OrderPaymentController : Controller
    {
        private readonly UserManager<AudioCityUser> _userManager;

        public OrderPaymentController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(PaymentViewModel PaymentDetail, string GigId, string FromGigDetail)
        {
            ViewBag.GigId = GigId;

            //check if the user has exceed the gig max order count 
            if (FromGigDetail == "true")
            {
                List<OrderEntity> GigActiveOrders = OrderEntityHelper.GetGigActiveOrders(GigId);
                Gig Gig = GigModelHelper.GetGig(GigId);

                if (GigActiveOrders.Count >= Gig.MaxOrderCount)
                {
                    //display error 
                    TempData["ExceedMaxOrdersCount"] = true;
                    TempData["GigActiveOrdersCount"] = GigActiveOrders.Count;
                    return RedirectToAction("GigDetail", "FreelanceList", new { GigId = GigId });
                }
                else
                {
                    return View();
                }

            }

            else if (ModelState.IsValid)
            {
                //generate order and store to table storage 

                //first, create a new OrderEntity 
                AudioCityUser User = await _userManager.GetUserAsync(HttpContext.User);
                CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();
                string CustomerId = User.Id;
                string CustomerName = User.FullName;

                string GigThumbnailFilePath = GigModelHelper.GetGig(GigId).ThumbnailFilePath;
                string GigThumbnailUri = Container.GetBlockBlobReference(GigThumbnailFilePath).Uri.ToString();

                bool GenerateSuccess = await OrderEntityHelper.GenerateOrder(GigId, CustomerId, CustomerName, PaymentDetail.OrderNote, GigThumbnailUri);

                if(GenerateSuccess)
                {
                    return RedirectToAction("Index", "CustomerDashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Something went wrong, please try again later.";
                    return View();
                }
            }
            return View();
        }
    }
}
