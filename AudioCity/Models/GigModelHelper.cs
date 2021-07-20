using AudioCity.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
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

        private static readonly UserManager<AudioCityUser> _userManager;
     

        public static void CreateGig(Gig GigModel)
        {
            using (var conn = new SqlConnection(Configure["ConnectionStrings:AudioCityGigDatabaseConnection"]))
            using (var cmd = new SqlCommand("",connection:conn))
            {
                cmd.CommandText = "insert into Gigs values (@Id, @CreatedBy, @Title, @Description, @EstimatedDeliveryDays, @Price, @Category, @Thumbnail, @PortfolioFilePath, @PublishedOn, @ArtistName)";
                cmd.Parameters.AddWithValue("@Id", GigModel.Id);
                cmd.Parameters.AddWithValue("@CreatedBy", GigModel.CreatedBy);
                cmd.Parameters.AddWithValue("@Title", GigModel.Title);
                cmd.Parameters.AddWithValue("@Description", GigModel.Description);
                cmd.Parameters.AddWithValue("@EstimatedDeliveryDays", GigModel.EstimatedDeliveryDays);
                cmd.Parameters.AddWithValue("@Price", GigModel.Price);
                cmd.Parameters.AddWithValue("@Category", GigModel.Category);
                cmd.Parameters.AddWithValue("@Thumbnail", GigModel.ThumbnailFilePath);
                cmd.Parameters.AddWithValue("@PortfolioFilePath", GigModel.PortfolioFilePath);
                cmd.Parameters.AddWithValue("@PublishedOn", GigModel.PublishedOn);
                cmd.Parameters.AddWithValue("@ArtistName", GigModel.ArtistName);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static List<Gig> GetGigsFromSeller(string UserId)
        {
            List<Gig> Gigs = new List<Gig>();
            using (var conn = new SqlConnection(Configure["ConnectionStrings:AudioCityGigDatabaseConnection"]))
            using (var cmd = new SqlCommand("", connection: conn))
            {
                cmd.CommandText = "select * from Gigs where CreatedBy=@UserId";
                cmd.Parameters.AddWithValue("@UserId", UserId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Gig Gig = new Gig();
                    Gig.Id = reader[0].ToString();
                    Gig.CreatedBy = reader[1].ToString();
                    Gig.Title = reader[2].ToString();
                    Gig.Description = reader[3].ToString();
                    Gig.EstimatedDeliveryDays = reader.GetInt32(4);
                    Gig.Price = reader.GetDouble(5);
                    Gig.Category = reader[6].ToString();
                    Gig.ThumbnailFilePath = reader[7].ToString();
                    Gig.PortfolioFilePath = reader[8].ToString();
                    Gig.PublishedOn = reader.GetDateTime(9);
                    Gig.ArtistName = reader[10].ToString();
  
                    Gigs.Add(Gig);
                }

                conn.Close();
                return Gigs;
            }
        }

        public static List<Gig> GetAllGigs()
        {
            List<Gig> Gigs = new List<Gig>();
            using (var conn = new SqlConnection(Configure["ConnectionStrings:AudioCityGigDatabaseConnection"]))
            using (var cmd = new SqlCommand("", connection: conn))
            {
                cmd.CommandText = "select * from Gigs";

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Gig Gig = new Gig();
                    Gig.Id = reader[0].ToString();
                    Gig.CreatedBy = reader[1].ToString();
                    Gig.Title = reader[2].ToString();
                    Gig.Description = reader[3].ToString();
                    Gig.EstimatedDeliveryDays = reader.GetInt32(4);
                    Gig.Price = reader.GetDouble(5);
                    Gig.Category = reader[6].ToString();
                    Gig.ThumbnailFilePath = reader[7].ToString();
                    Gig.PortfolioFilePath = reader[8].ToString();
                    Gig.PublishedOn = reader.GetDateTime(9);
                    Gig.ArtistName = reader[10].ToString();

                    Gigs.Add(Gig);
                }

                conn.Close();
                return Gigs;
            }
        }

        public static Gig GetGig(string GigId)
        {
            using (var conn = new SqlConnection(Configure["ConnectionStrings:AudioCityGigDatabaseConnection"]))
            using (var cmd = new SqlCommand("", connection: conn))
            {
                cmd.CommandText = "select * from Gigs where Id=@GigId";
                cmd.Parameters.AddWithValue("@GigId", GigId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                Gig Gig = new Gig();

                while (reader.Read())
                {
                    Gig.Id = reader[0].ToString();
                    Gig.CreatedBy = reader[1].ToString();
                    Gig.Title = reader[2].ToString();
                    Gig.Description = reader[3].ToString();
                    Gig.EstimatedDeliveryDays = reader.GetInt32(4);
                    Gig.Price = reader.GetDouble(5);
                    Gig.Category = reader[6].ToString();
                    Gig.ThumbnailFilePath = reader[7].ToString();
                    Gig.PortfolioFilePath = reader[8].ToString();
                    Gig.PublishedOn = reader.GetDateTime(9);
                    Gig.ArtistName = reader[10].ToString();
                }

                conn.Close();
                return Gig;
            }
        }

        public static GigDetailViewModel ConvertToGigDetailViewModel(Gig Gig, AudioCityUser User)
        {
            //convert to GigDetailViewModel instance
            CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();

            //Get list of blob items 
            CloudBlockBlob Portfolio = Container.GetBlockBlobReference(Gig.PortfolioFilePath);
            CloudBlockBlob Thumbnail = Container.GetBlockBlobReference(Gig.ThumbnailFilePath);

            GigDetailViewModel GigDetail = new GigDetailViewModel { Gig = Gig, Portfolio = Portfolio, Thumbnail = Thumbnail, User = User };

            return GigDetail;
        }
        
    }
}
