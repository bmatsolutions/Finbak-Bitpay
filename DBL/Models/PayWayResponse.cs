using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BITPay.DBL.Models
{
    public class ApiAuthModels
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AppID { get; set; }
        public string AppToken { get; set; }
    }


    public class GetProviders : ApiAuthModels
    {
        public string ProviderId { get; set; }
    }

    public class GetCustomers : ApiAuthModels
    {
        public string ProviderId { get; set; }
        public string CustomerNumber { get; set; }
        public string Amount { get; set; }
        public string ContactInfo { get; set; }
    }

    public class BuyTokens : ApiAuthModels
    {
        public string ProviderId { get; set; }
        public string Amount { get; set; }
        public string ContactInfo { get; set; }
        public string CustomerNumber { get; set; }
        public string TxId { get; set; }
    }

    public class ValidateRetailers : ApiAuthModels
    {
        public string CustomerNo { get; set; }
        public string ProviderId { get; set; }
    }

    public class TopUps : ApiAuthModels
    {
        public string ProviderId { get; set; }
        public string Amount { get; set; }
        public string ContactInfo { get; set; }
        public string CustomerNumber { get; set; }
        public string TxId { get; set; }
    }
    public class ServiceProviderDetailsData
    {
        public int stat { get; set; }
        public string msg { get; set; }
        public ServiceProviderDetails content { get; set; }
    }

    public class ServiceProviderDetails
    {
        public string msg { get; set; }
        public string Availability { get; set; }
        public decimal MaxPay { get; set; }
        public decimal MinPay { get; set; }
        public string CategoryId { get; set; }
        public string CategoryLabel { get; set; }
        public string InputPattern { get; set; }
        public string Notes { get; set; }
    }

    public class CustomerDetailsData
    {
        public int stat { get; set; }
        public string msg { get; set; }
        public CustomerDetails content { get; set; }
    }

    public class CustomerDetails
    {
        public bool Valid { get; set; }
        public string Notes { get; set; }
        public int Status { get; set; }
        public string message { get; set; }
    }


    public class PowerTokenData
    {
        public decimal MainAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int stat { get; set; }
        public string msg { get; set; }
        public PowerToken content { get; set; }
    }

    public class PowerToken
    {
        public string TokenStatus { get; set; }
        public string ContactPhone { get; set; }
        public string ExtInfo { get; set; }
        public string TxnId { get; set; }
        public string Provider { get; set; }
        public string ReceiptNo { get; set; }
        public string TokenNumber { get; set; }
        public string Operator { get; set; }
        public string StrangeNumbers { get; set; }
        public string LastRow { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string MeterNo { get; set; }
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public string Destination { get; set; }
        public string ConsumerName { get; set; }
        public string TaxNo { get; set; }
        public string TotalValue { get; set; }
        public string KwhLabelValue { get; set; }
        public string TvaLabelValue { get; set; }
        public string TotalUnits { get; set; }
        public string TokeValue { get; set; }
        public string Sgc { get; set; }
        public string Ti { get; set; }
        public string Krn { get; set; }
        public string TokenDescription { get; set; }
        public string KwhValue { get; set; }
        public string Vat { get; set; }
        public string AmountTrendered { get; set; }
        public string AmountAccepted { get; set; }
        public string KwhLabel { get; set; }
        public string TokenValue13 { get; set; }
        public string TokenValue45 { get; set; }
    }

    public class RetailerValidData
    {
        public int stat { get; set; }
        public string msg { get; set; }
        public RetailerValid content { get; set; }
    }

    public class RetailerValid 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public bool Valid { get; set; }
    }

    

    public class RetailerTopUPData
    {
        public int stat { get; set; }
        public string msg { get; set; }
        public RetailerTopUP content { get; set; }
    }

    public class RetailerTopUP
    {
        public string Result { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string outTxId { get; set; }
        public string status { get; set; }
    }
}
