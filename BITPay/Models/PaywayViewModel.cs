using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BITPay.Models
{
    public class ApiAuthModel
    {
        [Required]
        [Display(Name = "Gateway Username", Prompt = "Gateway Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Gateway Password", Prompt = "Gateway Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "App ID", Prompt = "App ID")]
        public string AppID { get; set; }

        [Required]
        [Display(Name = "App Token", Prompt = "App Token")]
        public string AppToken { get; set; }
    }


    public class GetProvider : ApiAuthModel
    {
        [Required]
        [Display(Name = "Provider Id", Prompt = "Service Provider Id")]
        public string ProviderId { get; set; }
    }

    public class GetCustomer : ApiAuthModel
    {
        [Required]
        [Display(Name = "Provider Id", Prompt = "Service Provider Id")]
        public string ProviderId { get; set; }
        [Display(Name = "Account Number", Prompt = "Account Number")]
        public string CustomerNumber { get; set; }
        [Display(Name = "Amount", Prompt = "Amount")]
        public string Amount { get; set; }
        [Display(Name = "Customer Number", Prompt = "Phone Number")]
        public string ContactInfo { get; set; }
    }

    public class BuyToken : ApiAuthModel
    {
        [Required]
        [Display(Name = "Provider Id", Prompt = "Service Provider Id")]
        public string ProviderId { get; set; }
        [Required]
        [Display(Name = "Amount", Prompt = "Amount")]
        public string Amount { get; set; }
        [Required]
        [Display(Name = "Phone Number", Prompt = "Phone Number")]
        public string ContactInfo { get; set; }
        [Required]
        [Display(Name = "Meter Number", Prompt = "Meter Number")]
        public string CustomerNumber { get; set; }
        [Required]
        [Display(Name = "Txid Number", Prompt = "Txid Number")]
        public string TxId { get; set; }
    }

    public class ValidateRetailer : ApiAuthModel
    {
        [Required]
        [Display(Name = "Customer No", Prompt = "Customer Number")]
        public string CustomerNo { get; set; }
        [Required]
        [Display(Name = "Provider Id", Prompt = "Service Provider Id")]
        public string ProviderId { get; set; }
    }

    public class TopUp : ApiAuthModel
    {
        [Required]
        [Display(Name = "Provider Id", Prompt = "Service Provider Id")]
        public string ProviderId { get; set; }
        [Display(Name = "Amount", Prompt = "Amount")]
        public string Amount { get; set; }
        [Display(Name = "Phone Number", Prompt = "Phone Number")]
        public string ContactInfo { get; set; }
        [Display(Name = "Retailer Code", Prompt = "Retailer Code")]
        public string CustomerNumber { get; set; }
        [Display(Name = "Txid Number", Prompt = "Txid Number")]
        public string TxId { get; set; }
    }
}
