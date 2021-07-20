using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class Order : Controller
    {
        public string OrderID { get; set; }
        public string OrderDesc { get; set; }
        public string Seller { get; set; }
        public string Buyer { get; set; }
        public double Price { get; set; }
    }
}
