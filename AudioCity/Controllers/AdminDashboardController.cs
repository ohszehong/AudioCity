using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioCity.Models;

namespace AudioCity.Controllers
{
    public class AdminDashboardController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            List<Order> PO = GetPendingOrder();
            List<Order> CO = GetCompleteOrder();

            return View(Tuple.Create(PO, CO));
        }

        public List<Order> GetPendingOrder()
        {
            List<Order> PO = new List<Order>
            {
                new Order{ OrderID = "123456", OrderDesc = "5 Song", Buyer = "Demo", Seller = "Test", Price = 123.00 },
                new Order{ OrderID = "123457", OrderDesc = "5 Song", Buyer = "Demo", Seller = "Test", Price = 123.00 }
            };

            return PO;
        }

        public List<Order> GetCompleteOrder()
        {
            List<Order> CO = new List<Order>
            {
                new Order{ OrderID = "123458", OrderDesc = "5 Song", Buyer = "Demo", Seller = "Test", Price = 123.00 },
                new Order{ OrderID = "123459", OrderDesc = "5 Song", Buyer = "Demo", Seller = "Test", Price = 123.00 }
            };

            return CO;
        }
    }
}
