using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public static class GigModelHelper
    {
        private static IConfigurationBuilder Builder = new ConfigurationBuilder().SetBasePath(Directory
       .GetCurrentDirectory())
       .AddJsonFile("appsettings.json");

        private static IConfiguration Configure = Builder.Build();

        public static void CreateGig(Gig GigModel)
        {
            using (var conn = new SqlConnection(Configure["ConnectionStrings:AudioCityGigDatabaseConnection"]))
            using (var cmd = new SqlCommand("",connection:conn))
            {
                cmd.CommandText = "insert into Gigs values (@Id, @CreatedBy, @Title, @Description, @EstimatedDeliveryDays, @Price, @Category, @Thumbnail, @PortfolioFilePath, @Rating, @PublishedOn)";
                cmd.Parameters.AddWithValue("@Id", GigModel.Id);
                cmd.Parameters.AddWithValue("@CreatedBy", GigModel.CreatedBy);
                cmd.Parameters.AddWithValue("@Title", GigModel.Title);
                cmd.Parameters.AddWithValue("@Description", GigModel.Description);
                cmd.Parameters.AddWithValue("@EstimatedDeliveryDays", GigModel.EstimatedDeliveryDays);
                cmd.Parameters.AddWithValue("@Price", GigModel.Price);
                cmd.Parameters.AddWithValue("@Category", GigModel.Category);
                cmd.Parameters.AddWithValue("@Thumbnail", GigModel.Thumbnail);
                cmd.Parameters.AddWithValue("@PortfolioFilePath", GigModel.PortfolioFilePath);
                cmd.Parameters.AddWithValue("@PublishedOn", GigModel.PublishedOn);
                cmd.ExecuteNonQuery();
            }
        }

        
    }
}
