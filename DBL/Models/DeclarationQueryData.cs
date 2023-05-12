using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class DeclarationQueryData
    {
        [Required]
        [Display(Name = "Office Code")]
        public string OfficeCode { get; set; }

        ///[Required]
        [Display(Name = "Registration Year")]
        public string RegistrationYear { get; set; }

        [Display(Name = "Registration Serial")]
       /// [Required]
        public string RegistrationSerial { get; set; }

        [Display(Name = "Registration Number")]
       /// [Required]
        public string RegistrationNumber { get; set; }
               
        [Display(Name = "Assessment Serial")]
        public string AssessmentSerial { get; set; }

        [Display(Name = "Assessment Number")]
        public string AssessmentNumber { get; set; }

        [Display(Name = "Account Reference")]
        [Required]
        public string AccountReference { get; set; }

        [Display(Name = "Reference Year")]
        [Required]
        public string ReferenceYear { get; set; }

        [Display(Name = "Reference Number")]
        [Required]
        public string ReferenceNumber { get; set; }

        [Display(Name = "Pay Type")]
        [Required]
        public string TaxType { get; set; }

        [Display(Name = "Currency")]
        //[Required]
        public string Currencies { get; set; }

        [Display(Name = "Transaction Code")]
        //[Required]
        public string TranCode { get; set; }

        [Display(Name = "Transaction Reference")]
        //[Required]
        public string TranRef { get; set; }
        [Display(Name = "Tax Payer Name")]
        //[Required]
        public string TaxPayer { get; set; }
        [Display(Name = "Declarant Code")]
        //[Required]
        public string DeclCode { get; set; }
        [Display(Name = "Company Nif")]
        //[Required]
        public string CompCode { get; set; }
        [Display(Name = "Amount")]
        //[Required]
        public decimal Amount { get; set; }
        public int status { get; set; }
        public int status2 { get; set; }
        public int mode { get; set; }

    }
}
