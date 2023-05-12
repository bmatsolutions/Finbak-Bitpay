using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text;

namespace BITPay.DBL.Models
{
    public class PowerPaymentModel
    {
        public string TransactionId { get; set; }
        public string CustomerNumber { get; set; }
        public string AccountName { get; set; }
        public string CustomerNo { get; set; }
        public decimal Amount { get; set; }
        public string ContactInfo { get; set; }
        public string PhoneNo { get; set; }
        public int UserCode { get; set; }
        public int BankCode { get; set; }
        public string branchCode { get; set; }
        public int mode { get; set; }
        public string PayModeName { get; set; }
        public decimal ChargeAmount { get; set; }
        public dynamic PaymentModes { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public int PaymentCode { get; set; }
        public string Remarks { get; set; }
        public decimal ExpectedAmount { get; set; }
        

        [Display(Name = "Print Receipt")]
        public bool PrintReceipt { get; set; }

        [Required]
        [Display(Name = "Mode of Payment")]
        public int PayMode { get; set; }

        [Required]
        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Required]
        [Display(Name = "Cheque Number")]
        public string ChequeNo { get; set; }

        [Display(Name = "Waive Charge")]
        public bool NoCharge { get; set; }

        [Required]
        [Display(Name = "Received From")]
        public string ReceivedFrom { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public string SortCode { get; set; }

        public string PaywayAccountName { get; set; }
    }

    

}
