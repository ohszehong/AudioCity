using AudioCity.Areas.Identity.Data;
using AudioCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerDashboardController : Controller
    {

        private readonly UserManager<AudioCityUser> _userManager;

        public CustomerDashboardController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        public IActionResult Index(string PartialPage = "_PendingOrdersPartial")
        {
            string CustomerId = _userManager.GetUserId(HttpContext.User);
            ViewBag.UserId = CustomerId;
            ViewBag.PartialPage = PartialPage;

            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            if (PartialPage == "_PendingOrdersPartial")
            {
                List<OrderEntity> PendingOrders = OrderEntityHelper.GetCustomerOrders(CustomerId, OrderStatus.PendingAccept.ToString());

                ViewBag.PendingOrdersCount = PendingOrders.Count;
                return View(PendingOrders);
            }

            else if (PartialPage == "_RejectedOrdersPartial")
            {
                List<OrderEntity> RejectedOrders = OrderEntityHelper.GetCustomerOrders(CustomerId, OrderStatus.Rejected.ToString());

                ViewBag.RejectedOrdersCount = RejectedOrders.Count;
                return View(RejectedOrders);
            }

            else if (PartialPage == "_ActiveOrdersPartial")
            {
                List<OrderEntity> OngoingOrders = OrderEntityHelper.GetCustomerOrders(CustomerId, OrderStatus.Ongoing.ToString());
                ViewBag.OngoingOrdersCount = OngoingOrders.Count;
                return View(OngoingOrders);
            }

            else if (PartialPage == "_CompletedOrdersPartial")
            {
                List<OrderEntity> CompletedOrders = OrderEntityHelper.GetCustomerOrders(CustomerId, OrderStatus.Completed.ToString());
                ViewBag.CompletedOrdersCount = CompletedOrders.Count;
                return View(CompletedOrders);
            }

            else if (PartialPage == "_OrderHistoryPartial")
            {
                List<OrderEntity> ArchivedOrders = OrderEntityHelper.GetCustomerOrders(CustomerId, OrderStatus.Archived.ToString());
                ViewBag.ArchivedOrdersCount = ArchivedOrders.Count;
                return View(ArchivedOrders);
            }


            return View();
        }

        public IActionResult ReviewingOrderForm(string GigId, string OrderId)
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
        public async Task<IActionResult> ReviewOrder(string GigId, string OrderId, string RatingScore, string Comment)
        {
            //convert RatingScore to double 
            double Rating = double.Parse(RatingScore);

            if (Rating > 0)
            {
                //insert to table storage 
                AudioCityUser Customer = await _userManager.GetUserAsync(HttpContext.User);
                CustomerReviewEntityHelper.GenerateReview(GigId, Customer.Id, Customer.FullName, Rating, Comment);

                //change the order status to archived
                OrderEntity Order = OrderEntityHelper.GetOrder(GigId, OrderId);
                OrderEntityHelper.UpdateOrderStatus(Order, OrderStatus.Archived.ToString());

                //redirect back to completed order partial 
                return RedirectToAction("Index", "CustomerDashboard", new { PartialPage = "_CompletedOrdersPartial" });
            }
            else
            {
                TempData["ErrorMessage"] = "Please provides rating score.";
                return RedirectToAction("ReviewingOrderForm", "CustomerDashboard", new { GigId = GigId, OrderId = OrderId });
            }
        }
    }
}
