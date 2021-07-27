using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class Gig
    {
        public string Id { get; set; }

        public string CreatedBy { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? EstimatedDeliveryDays { get; set; }

        public double? Price { get; set; }

        public string Category { get; set; }

        public string ThumbnailFilePath { get; set; }

        public string PortfolioFilePath { get; set; }

        public DateTime PublishedOn { get; set; }

        public string ArtistName { get; set; }

        public double Rating { get; set; }

        public int MaxOrderCount { get; set; }

    }
}
