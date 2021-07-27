using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class CustomerReviewEntityHelper
    {
        public async static void GenerateReview(string GigId, string CustomerId, string CustomerName, double Rating, string Comment)
        {
            Guid guid = Guid.NewGuid();

            CustomerReviewEntity NewCustomerReview = new CustomerReviewEntity
            {
                PartitionKey = GigId,
                RowKey = guid.ToString(),
                CustomerId = CustomerId,
                CustomerName = CustomerName,
                Comment = Comment,
                ReviewDate = DateTime.Now,
                ReviewScore = Rating
            };

            CloudTable Table = ConfigureAudioCityAzureTable.GetReviewTableContainerInformation();

            try
            {
                TableOperation insertToOrderTable = TableOperation.Insert(NewCustomerReview);
                TableResult insertionResult = await Table.ExecuteAsync(insertToOrderTable);

                //after that, update gig rating in sql database 
                //get particular gig info 
                Gig Gig = GigModelHelper.GetGig(GigId);
                int GigNewTotalReviews = Gig.TotalReviews + 1;
                double GigNewTotalRating = Gig.Rating + Rating;

                GigModelHelper.UpdateGigRating(GigId, GigNewTotalRating, GigNewTotalReviews);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when uploading customer review entity to table storage: ", Ex.ToString());
            }
        }

        public static List<CustomerReviewEntity> GetGigReviews(string GigId)
        {
            CloudTable Table = ConfigureAudioCityAzureTable.GetReviewTableContainerInformation();

            List<CustomerReviewEntity> CustomerReviews = new List<CustomerReviewEntity>();

            try
            {
                string CustomerIdFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GigId);

                TableQuery<CustomerReviewEntity> RetrieveGigReviewsQuery = new TableQuery<CustomerReviewEntity>().Where(CustomerIdFilter);

                TableContinuationToken continuationToken = null;
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<CustomerReviewEntity> TableQueryResult = Table.ExecuteQuerySegmentedAsync(RetrieveGigReviewsQuery, continuationToken).Result;

                    CustomerReviews.AddRange(TableQueryResult.Results);

                    continuationToken = TableQueryResult.ContinuationToken;
                } while (continuationToken != null);

                return CustomerReviews;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when retrieving data from table storage: ", Ex.ToString());
            }

            return CustomerReviews;
        }

    }
}
