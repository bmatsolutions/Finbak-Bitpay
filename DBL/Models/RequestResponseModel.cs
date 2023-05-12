using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class RequestResponseModel
    {
        //0-Success, 1-Failed, 2-Exception, 3-Webexception
        public int Status { get; set; }
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }

    public class DomesticResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public TinDets content { get; set; }
    }
    public class NIFResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public NIFdets content { get; set; }
    }
    public class NIFdets
    {
        public string CompanyName { get; set; }
    }
    public class DeclarantResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public Declarantdets content { get; set; }
    }
    public class Declarantdets
    {
        public string DeclarantName { get; set; }
    }
    public class TinDets
    {
        public string pName { get; set; }
    }

    public class DomesticPaymentResponse
    {
        public int id { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public PaymentResponseDetails content { get; set; }
    }
    public class PaymentResponseDetails
    {
        public int id { get; set; }
        public string resId { get; set; }
    }

    public class DomesticQueryResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public PaymentDetails content { get; set; }
    }
    public class PaymentDetails
    {
        public string code { get; set; }
        public string cusName { get; set; }
        public string date { get; set; }
        public string tranStat { get; set; }
    }


    public class CreditResponseModel
    {
        //0-Success, 1-Failed, 2-Exception, 3-Webexception
        public int Status { get; set; }
        public string Message { get; set; }
        public CreditQueryResponse Content { get; set; }
    }

    public class CreditQueryResponse
    {
        public string accHolder { get; set; }
        public string cmpyName { get; set; }
        public string offCode { get; set; }
        public string offName { get; set; }
        public string ttAmt { get; set; }
        public string recNo { get; set; }
        public string recSerial { get; set; }
        public string date { get; set; }
    }

    public class FetchTaxList
    {
        public int status { get; set; }
        public string message { get; set; }
        public TaxListContent content { get; set; }
    }

    public class TaxListContent
    {
        public List<TaxList> taxList { get; set; }
    }

    public class TaxList
    {
        public string code { get; set; }
        public string label { get; set; }
    }

    public class OtherQueryResponse
    {
        public string tranCode { get; set; }
        public string tranName { get; set; }
        public string tranAmt { get; set; }
        public string currCode { get; set; }
        public string ttAmt { get; set; }
    }
}
