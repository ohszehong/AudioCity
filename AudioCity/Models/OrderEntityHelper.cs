using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class OrderEntityHelper
    {

        public async static Task<bool> GenerateOrder(string GigId, string CustomerId, string CustomerName, string OrderNote)
        {
            //get particular gig info 
            Gig Gig = GigModelHelper.GetGig(GigId);

            Guid guid = Guid.NewGuid();

            DateTime OrderDueDate = DateTime.Now.AddDays((double)Gig.EstimatedDeliveryDays);

            OrderEntity NewOrder = new OrderEntity
            {
                PartitionKey = GigId,
                RowKey = guid.ToString(),
                CustomerId = CustomerId,
                CustomerName = CustomerName,
                SellerId = Gig.CreatedBy,
                SellerName = Gig.ArtistName,
                UploadedContentFilePath = null,
                OrderNote = OrderNote,
                OrderDate = DateTime.Now,
                OrderDueDate = OrderDueDate,
                OrderCompleteDate = null,
                OrderPayment = (double)Gig.Price,
                OrderRejectReason = null,
                OrderStatus = OrderStatus.PendingAccept.ToString()
            };

            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            try
            {
                TableOperation insertToOrderTable = TableOperation.Insert(NewOrder);
                TableResult insertionResult = await Table.ExecuteAsync(insertToOrderTable);
                return true;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when uploading order entity to table storage: ", Ex.ToString());
                return false;
            }
        }

        public static List<OrderEntity> GetCustomerOrders(string CustomerId, string OrderStatus)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            List<OrderEntity> CustomerOrders = new List<OrderEntity>();

            try
            {
                string CustomerIdFilter = TableQuery.GenerateFilterCondition("CustomerId", QueryComparisons.Equal, CustomerId);
                string OrderStatusFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus);

                string CombinedFilter = TableQuery.CombineFilters(CustomerIdFilter, TableOperators.And, OrderStatusFilter);

                TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(CombinedFilter);

                TableContinuationToken continuationToken = null;
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                    CustomerOrders.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return CustomerOrders;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return CustomerOrders;
        }

        public static List<OrderEntity> GetGigActiveOrders(string GigId)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            List<OrderEntity> GigActiveOrders = new List<OrderEntity>();

            try
            {
                string CustomerIdFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GigId);
                string PendingOrderFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.PendingAccept.ToString());
                string OngoingOrderFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.Ongoing.ToString());

                string CombinedOrderFilters = TableQuery.CombineFilters(PendingOrderFilter, TableOperators.Or, OngoingOrderFilter);

                string CombinedFilter = TableQuery.CombineFilters(CustomerIdFilter, TableOperators.And, CombinedOrderFilters);

                TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(CombinedFilter);

                TableContinuationToken continuationToken = null;
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                    GigActiveOrders.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return GigActiveOrders;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return GigActiveOrders;
        }

    }
}
