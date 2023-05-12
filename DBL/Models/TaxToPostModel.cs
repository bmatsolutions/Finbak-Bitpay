using BITPay.DBL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class TaxToPostModel : BaseEntity
    {
        public int PaymentCode { get; set; }
        public int ChargeCode { get; set; }
        public int PaymentMode { get; set; }
        public bool ApplyCharge { get; set; }
        public string CurrencyCode { get; set; }
        public string CBSOfficer { get; set; }
        public string TransactorName { get; set; }
        public string PostUrl { get; set; }
        public string BalanceUrl { get; set; }
        public int FileCode { get; set; }
        public string IncomeTaxCatergory { get; set; }
        public string Period { get; set; }
        public string PayerName { get; set; }
        public decimal Amount { get; set; }
        
        public string MainCrAccount { get; set; }
        public string MainDrAccount { get; set; }
        public string MainNarration { get; set; }
        public string MainRefNo { get; set; }
        public string MainTxnCode { get; set; }
        public string MainTxnType { get; set; }
        public string MainFlag { get; set; }
        public decimal MainAmount { get; set; }        

        public string ChargeCrAccount { get; set; }
        public string ChargeDrAccount { get; set; }
        public string ChargeNarration { get; set; }
        public string ChargeRefNo { get; set; }
        public string ChargeTxnCode { get; set; }
        public string ChargeTxnType { get; set; }
        public string ChargeFlag { get; set; }
        public decimal ChargeAmount { get; set; }

        public bool ApprovalNeeded { get; set; }
        public string ChequeNo { get; set; }
        public int StatusCode { get; set; }
        public string BrachCode { get; set; }
        public int Filecode { get; set; }

        public string ContactInfo { get; set; }
        public string CustomerNo { get; set; }
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public string ReimbursementNarration { get; set; }
        public string ReimbursementRefNo { get; set; }
        public string Tin { get; set; }
        public string TaxAmount { get; set; }

        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public string Data3 { get; set; }
        public string Data4 { get; set; }
        public string Data5 { get; set; }
        public string Data6 { get; set; }
        public string Data7 { get; set; }
        public string Data8 { get; set; }
        public string Data9 { get; set; }
        public string Data10 { get; set; }
    }

    public class BuyTokenPostModel : BaseEntity
    {
        public int PaymentCode { get; set; }
        public int ChargeCode { get; set; }
        public int PaymentMode { get; set; }
        public bool ApplyCharge { get; set; }
        public string CurrencyCode { get; set; }
        public string CBSOfficer { get; set; }
        public string TransactorName { get; set; }
        public string PostUrl { get; set; }
        public string BalanceUrl { get; set; }
        public int BillCode { get; set; }
        public string IncomeTaxCatergory { get; set; }
        public string Period { get; set; }
        public string PayerName { get; set; }
        public decimal Amount { get; set; }

        public string MainCrAccount { get; set; }
        public string MainDrAccount { get; set; }
        public string MainNarration { get; set; }
        public string MainRefNo { get; set; }
        public string MainTxnCode { get; set; }
        public string MainTxnType { get; set; }
        public string MainFlag { get; set; }
        public decimal MainAmount { get; set; }

        public string ChargeCrAccount { get; set; }
        public string ChargeDrAccount { get; set; }
        public string ChargeNarration { get; set; }
        public string ChargeRefNo { get; set; }
        public string ChargeTxnCode { get; set; }
        public string ChargeTxnType { get; set; }
        public string ChargeFlag { get; set; }
        public decimal ChargeAmount { get; set; }

        public bool ApprovalNeeded { get; set; }
        public string ChequeNo { get; set; }
        public int StatusCode { get; set; }
        public string BrachCode { get; set; }
        public int Filecode { get; set; }

        public string ContactInfo { get; set; }
        public string CustomerNo { get; set; }
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public string ReimbursementNarration { get; set; }
        public string ReimbursementRefNo { get; set; }
        public string Tin { get; set; }
        public string TaxAmount { get; set; }

        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public string Data3 { get; set; }
        public string Data4 { get; set; }
        public string Data5 { get; set; }
        public string Data6 { get; set; }
        public string Data7 { get; set; }
        public string Data8 { get; set; }
        public string Data9 { get; set; }
        public string Data10 { get; set; }
    }

    public class PowerToCBSPost : BaseEntity
    {
        public decimal MainAmount { get; set; }
        public decimal ChargeAmount { get; set; }
    }

    public class PowerToPost : BaseEntity
    {
        public string ReceiptNo { get; set; }
        public string ExtInfoId { get; set; }
        public string TokenNo { get; set; }
        public string ContactInfoNo { get; set; }
        public string CustomerNo { get; set; }
        public decimal Amount { get; set; }
        public string MeterNo { get; set; }
        public string ConsumerName { get; set; }
        public string TotalUnits { get; set; }
        public string TokenValue { get; set; }
        public string Status { get; set; }
        public string KwhValue { get; set; }
        public string Vat { get; set; }
        
    }
}
