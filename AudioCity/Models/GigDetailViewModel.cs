using AudioCity.Areas.Identity.Data;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class GigDetailViewModel
    {
        public Gig Gig { get; set; }
        public CloudBlockBlob Portfolio { get; set; }
        public CloudBlockBlob Thumbnail { get; set; }

        public AudioCityUser User { get; set; }

        public int RoundedRating { get; set; }
    }
}
