using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public enum OrderStatus
    {
        PendingAccept,
        Rejected,
        Ongoing,
        Completed,
        Archived
    }


    public class OrderEntity:TableEntity
    {
        //PartitionKey is the Order Id 
        //RowKey is  the Gig Id 
        //Generate them at backend 

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }
        
        public string SellerId { get; set; }
        public string SellerName { get; set; }

        public string UploadedContentFilePath { get; set; }

        public string OrderNote { get; set; }

        public string OrderRejectReason { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime OrderDueDate { get; set; }

        public DateTime? OrderCompleteDate { get; set; }

        //PendingAccept
        //Rejected
        //Ongoing
        //Completed
        public string OrderStatus { get; set; }

        public double OrderPayment { get; set; }

        public string OrderThumbnailUri { get; set; }

    }
}
