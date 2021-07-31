using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class OrderEntityHelper
    {

        public async static Task<bool> GenerateOrder(string GigId, string CustomerId, string CustomerName, string OrderNote, string OrderThumbnailUri)
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
                OrderStatus = OrderStatus.PendingAccept.ToString(),
                OrderThumbnailUri = OrderThumbnailUri
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
                string CombinedFilter = null;
                string CustomerIdFilter = TableQuery.GenerateFilterCondition("CustomerId", QueryComparisons.Equal, CustomerId);
                string OrderStatusFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus);

                if (OrderStatus == "Archived")
                {
                    string OrderStatusFilter2 = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, "Rejected");
                    string OrderStatusCombinedFilter = TableQuery.CombineFilters(OrderStatusFilter, TableOperators.Or, OrderStatusFilter2);
                    CombinedFilter = TableQuery.CombineFilters(CustomerIdFilter, TableOperators.And, OrderStatusCombinedFilter);

                }
                else
                {
                    CombinedFilter = TableQuery.CombineFilters(CustomerIdFilter, TableOperators.And, OrderStatusFilter);
                }

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

        public static List<OrderEntity> GetSellerOrders(string SellerId, string OrderStatus)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            List<OrderEntity> SellerOrders = new List<OrderEntity>();

            try
            {
                string SellerIdFilter = TableQuery.GenerateFilterCondition("SellerId", QueryComparisons.Equal, SellerId);
                string CombinedFilter = null;

                if (OrderStatus == "Completed")
                {
                    string OrderStatusFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus);
                    string OrderStatusFilter2 = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, "Archived");
                    string OrderStatusFilter3 = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, "Rejected");

                    string OrderStatusCombinedFilter = TableQuery.CombineFilters(OrderStatusFilter, TableOperators.Or, OrderStatusFilter2);
                    string OrderStatusCombinedFilter2 = TableQuery.CombineFilters(OrderStatusCombinedFilter, TableOperators.Or, OrderStatusFilter3);
                    CombinedFilter = TableQuery.CombineFilters(SellerIdFilter, TableOperators.And, OrderStatusCombinedFilter2);

                }
                else
                {
                    string OrderStatusFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus);
                    CombinedFilter = TableQuery.CombineFilters(SellerIdFilter, TableOperators.And, OrderStatusFilter);
                }

                TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(CombinedFilter);

                TableContinuationToken continuationToken = null;
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                    SellerOrders.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return SellerOrders;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return SellerOrders;
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

        public static void UpdateOrderStatus(OrderEntity Order, string OrderStatus)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            Order.OrderStatus = OrderStatus;
            Order.ETag = "*";
            try
            {
                TableOperation UpdateOrderStatusOperation = TableOperation.Replace(Order);
                Table.ExecuteAsync(UpdateOrderStatusOperation);
            }
            catch(Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when updating data in table storage: ", Ex.ToString());
            }
        }

        public static OrderEntity GetOrder(string GigId, string OrderId)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            try
            {
                TableOperation RetrieveOrderOperation = TableOperation.Retrieve<OrderEntity>(GigId, OrderId);
                TableResult RetrieveOrderResult = Table.ExecuteAsync(RetrieveOrderOperation).Result;

                System.Diagnostics.Debug.WriteLine("e-tag: " + RetrieveOrderResult.Etag.ToString());

                if (RetrieveOrderResult.Etag != null)
                {
                    var Order = RetrieveOrderResult.Result as OrderEntity;
                    return Order;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving order from table storage: ", Ex.ToString());
                return null;
            }
        }

        public static List<OrderEntity> GetAllOngoingOrder()
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            List<OrderEntity> OngoingOrders = new List<OrderEntity>();

            try
            {
                string OrderStatusFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, "Ongoing");

                TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(OrderStatusFilter);

                TableContinuationToken continuationToken = null;
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                    OngoingOrders.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return OngoingOrders;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return OngoingOrders;
        }

        public static List<OrderEntity> GetAllCompleteOrder()
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            List<OrderEntity> CompleteOrders = new List<OrderEntity>();

            try
            {
                string CompletedOrderFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.Completed.ToString());
                string ArchivedOrderFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.Archived.ToString());

                string CombinedOrderFilters = TableQuery.CombineFilters(CompletedOrderFilter, TableOperators.Or, ArchivedOrderFilter);

                TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(CombinedOrderFilters);

                TableContinuationToken continuationToken = null;

                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                    CompleteOrders.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return CompleteOrders;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return CompleteOrders;
        }

        public static List<OrderEntity> GetAllRevenueOrder(string SellerId)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetTableContainerInformation();

            List<OrderEntity> PendingOrders = new List<OrderEntity>();

            try
            {
                string CompletedOrderFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.Completed.ToString());
                string ArchivedOrderFilter = TableQuery.GenerateFilterCondition("OrderStatus", QueryComparisons.Equal, OrderStatus.Archived.ToString());
                string SellerIdFilter = TableQuery.GenerateFilterCondition("SellerId", QueryComparisons.Equal, SellerId);

                string CombinedOrderFilters = TableQuery.CombineFilters(CompletedOrderFilter, TableOperators.Or, ArchivedOrderFilter);
                string CombinedFilters = TableQuery.CombineFilters(SellerIdFilter, TableOperators.And, CombinedOrderFilters);

                TableQuery<OrderEntity> RetrieveActiveOrderQuery = new TableQuery<OrderEntity>().Where(CombinedFilters);

                TableContinuationToken continuationToken = null;
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<OrderEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveActiveOrderQuery, continuationToken).Result;

                    PendingOrders.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return PendingOrders;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return PendingOrders;
        }

    }
}
