using AudioCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    [Authorize(Roles = "Seller")]
    public class CreateNewGigController : Controller
    {

        [HttpGet("CreateNewGig/{id}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("CreateNewGig/{id}")]
        public ActionResult Index(Gig GigModel)
        {
            if (ModelState.IsValid)
            {
                //return to previewGig view along with the model...
            }
            return View();
        }

    }
}
