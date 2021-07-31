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
            return View();
        }

        public List<OrderEntity> GetPendingOrder()
        {
            List<OrderEntity> PO = OrderEntityHelper.GetAllPendingOrder();

            return PO;
        }

        public List<OrderEntity> GetCompleteOrder()
        {
            List<OrderEntity> CO = OrderEntityHelper.GetAllCompleteOrder();

            return CO;
        }

        public PartialViewResult SearchPendingOrder(string SearchText)
        {
            List<OrderEntity> PO = GetPendingOrder();
            if(SearchText == null)
            {
                return PartialView("_OrderPartial", PO);
            } else {
                var result = PO.Where(a => a.PartitionKey.ToLower().Contains(SearchText) || a.RowKey.ToLower().Contains(SearchText) || 
                a.OrderCompleteDate.ToString().Contains(SearchText) || a.CustomerName.ToLower().Contains(SearchText) || 
                a.SellerName.ToLower().Contains(SearchText) || a.OrderPayment.ToString().Contains(SearchText)).ToList();
                return PartialView("_OrderPartial", result);
            }
        }

        public PartialViewResult SearchCompleteOrder(string SearchText)
        {
            List<OrderEntity> CO = GetCompleteOrder();
            if (SearchText == null)
            {
                return PartialView("_CompleteOrderPartial", CO);
            } else
            {
                var result = CO.Where(a => a.PartitionKey.ToLower().Contains(SearchText) || a.RowKey.ToLower().Contains(SearchText) ||
                a.OrderDueDate.ToString().Contains(SearchText) || a.CustomerName.ToLower().Contains(SearchText) ||
                a.SellerName.ToLower().Contains(SearchText) || a.OrderPayment.ToString().Contains(SearchText)).ToList();
                return PartialView("_CompleteOrderPartial", result);
            }
        }
    }
}
