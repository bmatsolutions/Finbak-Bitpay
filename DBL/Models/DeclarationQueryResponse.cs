using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class DeclarationQueryResponse
    {

        public string Result { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }


        //-----Type of Payment being made
        public int PayType { get; set; }

        //----- Found for payment (38)
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public string DeclarantCode { get; set; }
        public string DeclarantName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string AssessmentYear { get; set; }
        public string AssessmentSerial { get; set; }
        public string AssessmentNumber { get; set; }
        public decimal AmountToBePaid { get; set; }
        public string RegistrationSerial { get; set; }
        public string RegistrationNumber { get; set; }

        //----- Already paid (34)
        public string ReceiptSerial { get; set; }
        public string ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }

        //----- Not found (22)

       public int declCode { get; set; }

        //-----Credit Payment
        public string AccountHolder { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionRef { get; set; }
        public string TaxPayerName { get; set; }
        public string ReferenceNumber { get; set; }
        public string AccountReference { get; set; }
        public string Currency { get; set; }
    }
}
