using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class CustomerReviewEntity:TableEntity
    {
        //GigId as PartitionKey 
        //ReviewId as RowKey 

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        public double ReviewScore { get; set; }
    }
}
