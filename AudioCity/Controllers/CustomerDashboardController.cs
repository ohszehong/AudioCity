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
                try
                {
                    string CustomerIdFilter = TableQuery.GenerateFilterCondition("CustomerId", QueryComparisons.Equal, CustomerId);
                    string OrderStatusFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.PendingAccept.ToString());

                    string combinedFilter = TableQuery.CombineFilters(CustomerIdFilter, TableOperators.And, OrderStatusFilter);

                    TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(combinedFilter);


                    List<OrderEntity> ActiveOrders = new List<OrderEntity>();

                    TableContinuationToken continuationToken = null;
                    do
                    {
                        // Retrieve a segment (up to 1,000 entities).
                        TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                        ActiveOrders.AddRange(TableQueryResult.Results);

                        continuationToken = TableQueryResult.ContinuationToken;
                    } while (continuationToken != null);

                    return View(ActiveOrders);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
                }

            }
            return View();
        }
    }
}
