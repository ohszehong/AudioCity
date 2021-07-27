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
    [Authorize]
    public class OrderPaymentController : Controller
    {
        private readonly UserManager<AudioCityUser> _userManager;

        public OrderPaymentController(UserManager<AudioCityUser> UserManager)
        {
            _userManager = UserManager;
        }

        [HttpGet("[controller]/{GigId}")]
        public IActionResult Index(string GigId)
        {
            //check if the user has exceed the gig max order count 
            List<OrderEntity> GigActiveOrders = OrderEntityHelper.GetGigActiveOrders(GigId);
            Gig Gig = GigModelHelper.GetGig(GigId);

            if(GigActiveOrders.Count >= Gig.MaxOrderCount)
            {
                //display error 
                TempData["ExceedMaxOrdersCount"] = true;
                TempData["GigActiveOrdersCount"] = GigActiveOrders.Count;
                return RedirectToAction("GigDetail", "FreelanceList", new { GigId = GigId });
            }
            else
            {
                ViewBag.GigId = GigId;
                return View();
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(PaymentViewModel PaymentDetail, string GigId)
        {
            if(ModelState.IsValid)
            {
                //generate order and store to table storage 

                //first, create a new OrderEntity 
                AudioCityUser User = await _userManager.GetUserAsync(HttpContext.User);
                string CustomerId = User.Id;
                string CustomerName = User.FullName;

                bool GenerateSuccess = await OrderEntityHelper.GenerateOrder(GigId, CustomerId, CustomerName, PaymentDetail.OrderNote);

                if(GenerateSuccess)
                {
                    return RedirectToAction("Index", "CustomerDashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Something went wrong, please try again later.";
                    return View("Index");
                }
            }

            //else back to index 
            return View("Index");
        }
    }
}
