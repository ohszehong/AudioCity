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

            return View();
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

        public PartialViewResult SearchPendingOrder(string SearchText)
        {
            List<Order> PO = GetPendingOrder();
            if(SearchText == null)
            {
                return PartialView("_PendingOrderPartial", PO);
            } else {
                var result = PO.Where(a => a.OrderID.ToLower().Contains(SearchText) || a.Buyer.ToLower().Contains(SearchText) 
                || a.Seller.ToLower().Contains(SearchText) || a.Price.ToString().Contains(SearchText)).ToList();
                return PartialView("_PendingOrderPartial", result);
            }
        }
        public PartialViewResult SearchCompleteOrder(string SearchText)
        {
            List<Order> CO = GetCompleteOrder();
            if (SearchText == null)
            {
                return PartialView("_CompleteOrderPartial", CO);
            } else
            {
                var result = CO.Where(a => a.OrderID.ToLower().Contains(SearchText) || a.Buyer.ToLower().Contains(SearchText) 
                || a.Seller.ToLower().Contains(SearchText) || a.Price.ToString().Contains(SearchText)).ToList();
                return PartialView("_CompleteOrderPartial", result);
            }
        }
    }
}
