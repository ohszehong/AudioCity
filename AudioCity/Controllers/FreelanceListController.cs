using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    public class FreelanceListController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
