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
        [Required(ErrorMessage = "Portfolio is required.")]
        public string PortfolioFilePath { get; set; }

        public double Rating { get; set; }
        public Gig()
        {
            Rating = 0.00;
        }

        //auto generate unique id 
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        [Display(Name = "Title")]
        public string Title {get; set;}

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 20)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Estimated delivery days is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum of 1 day or larger.")]
        public int? EstimatedDeliveryDays { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public byte[] Thumbnail { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Category { get; set; }
        
    }
}
