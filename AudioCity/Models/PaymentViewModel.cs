using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Models
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        [Display(Name = "Order note")]
        public string OrderNote { get; set; }

        [Required(ErrorMessage = "{0} is  required.")]
        [DataType(DataType.CreditCard)]
        [RegularExpression("\\d{16}|\\d{4}[- ]\\d{4}[- ]\\d{4}[- ]\\d{4}", ErrorMessage = "Invalid Card Number.")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "{0} is  required.")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Invalid name.")]
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "{0} is  required.")]
        [RegularExpression("^(0[1-9]|1[0-2])/?([0-9]{2})$", ErrorMessage = "Invalid expiry date.")]
        [Display(Name = "Expiry Date")]
        public string CardExpiryDate { get; set; }

        [Required(ErrorMessage = "{0} is  required.")]
        [RegularExpression("\\d{3}", ErrorMessage = "Invalid CVV.")]
        [Display(Name = "CVV")]
        public string CardCVV { get; set; }

    }
}
