using AudioCity.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index(string PartialPage = "_ActiveGigsPartial")
        {
            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.PartialPage = PartialPage;
            return View();
        }

        //[Route("SellerDashboard/NewOrders")]
    }
}
