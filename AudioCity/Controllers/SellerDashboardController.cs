using AudioCity.Areas.Identity.Data;
using AudioCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            string UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.PartialPage = PartialPage;

            //get all the orders 
            //maybe can use cache at here 
            List<OrderEntity> PendingOrders = OrderEntityHelper.GetSellerOrders(UserId, OrderStatus.PendingAccept.ToString());
            List<OrderEntity> OngoingOrders = OrderEntityHelper.GetSellerOrders(UserId, OrderStatus.Ongoing.ToString());

            ViewBag.PendingOrdersCount = PendingOrders.Count;
            ViewBag.OngoingOrdersCount = OngoingOrders.Count;

            if (PartialPage == "_ActiveGigsPartial")
            {
                //read gigs data created by this seller 
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

            else if(PartialPage == "_NewOrdersPartial")
            {
                return View(PendingOrders);
            }

            else if(PartialPage == "_OnGoingOrdersPartial")
            {
                return View(OngoingOrders);
            }

            else if(PartialPage == "_OrderHistoryPartial")
            {
                List<OrderEntity> CompletedOrders = OrderEntityHelper.GetSellerOrders(UserId, OrderStatus.Completed.ToString());
                return View(CompletedOrders);
            }

            else if(PartialPage == "_RevenuesPartial")
            {
                //do your things
                //partition key = gig id 
                //row key = order id 
                List<OrderEntity> CompletedOrders = OrderEntityHelper.GetAllRevenueOrder(UserId);
                return View(CompletedOrders);
            }

            return View();
        }

        [HttpGet("[controller]/AcceptOrder/{GigId}/{OrderId}")]
        public IActionResult AcceptOrder(string GigId, string OrderId)
        {
            //set order status to ongoing 
            OrderEntity CurrentOrder = OrderEntityHelper.GetOrder(GigId, OrderId);
            OrderEntityHelper.UpdateOrderStatus(CurrentOrder, OrderStatus.Ongoing.ToString());

            return RedirectToAction("Index", "SellerDashboard", new { PartialPage = "_NewOrdersPartial"});
        }

        [HttpGet("[controller]/RejectingOrder/{GigId}/{OrderId}")]
        public IActionResult RejectingOrderForm(string GigId, string OrderId)
        {
            ViewBag.GigId = GigId;
            ViewBag.OrderId = OrderId;

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                TempData["ErrorMessage"] = null;
            }

            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult RejectOrder(string GigId, string OrderId, string RejectReason)
        {
            if (RejectReason != null)
            {
                if(RejectReason.Length >= 20)
                {
                   
                    OrderEntity Order = OrderEntityHelper.GetOrder(GigId, OrderId);
                    Order.OrderRejectReason = RejectReason;

                    OrderEntityHelper.UpdateOrderStatus(Order, OrderStatus.Rejected.ToString());

                    //redirect back to completed order partial 
                    return RedirectToAction("Index", "SellerDashboard", new { PartialPage = "_NewOrdersPartial" });
                }
                else
                {
                    TempData["ErrorMessage"] = "Reason is too short, please provides a comprehensive statements.";
                    return RedirectToAction("RejectingOrderForm", "SellerDashboard", new { GigId = GigId, OrderId = OrderId });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill up your reason for rejecting this order.";
                return RedirectToAction("RejectingOrderForm", "SellerDashboard", new { GigId = GigId, OrderId = OrderId });
            }
        }

        [HttpGet]
        public IActionResult CompletingOrder(string GigId, string OrderId)
        {
            return RedirectToAction("CompletingOrderForm", "SellerDashboard", new { GigId = GigId, OrderId = OrderId });
        }

        public IActionResult CompletingOrderForm(string GigId, string OrderId)
        {
            ViewBag.GigId = GigId;
            ViewBag.OrderId = OrderId;

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                TempData["ErrorMessage"] = null;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompleteOrder(IFormFile Content, string GigId, string OrderId)
        {
            if (Content != null)
            {
                string ContentExt = System.IO.Path.GetExtension(Content.FileName);

                //upload video/image/compressed file to blob storage and get the reference
                CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();

                CloudBlockBlob BlobItem;
                string ContentBlobReference = OrderId + ContentExt;

                try
                {
                    BlobItem = Container.GetBlockBlobReference(ContentBlobReference);
                    var ContentStream = Content.OpenReadStream();
                    BlobItem.UploadFromStreamAsync(ContentStream).Wait();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured when uploading video to blob storage: ", Ex.ToString());
                }

                //after upload, change the order status to completed
                OrderEntity Order = OrderEntityHelper.GetOrder(GigId, OrderId);
                //also, update the content file path and completed date 
                Order.UploadedContentFilePath = ContentBlobReference;
                Order.OrderCompleteDate = DateTime.Now;
                OrderEntityHelper.UpdateOrderStatus(Order, OrderStatus.Completed.ToString());

                //redirect back to ongoing order partial 
                return RedirectToAction("Index", "SellerDashboard", new { PartialPage = "_OnGoingOrdersPartial" });
            }
            else
            {
                TempData["ErrorMessage"] = "Please upload content before completing order.";
                return RedirectToAction("CompletingOrderForm", "SellerDashboard", new { GigId = GigId, OrderId = OrderId });
            }
        }
    }
}
