using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Enums
{
    public enum UserLoginStatus { Ok = 0, ChangePassword = 1, PassExpired = 2, UserLocked = 3, AttemptsExceeded = 4 }
    public enum ListModelType
    {
        PaymentModes = 0,
        FileStatus = 1,
        Branch = 2,
        User = 3,
        Bank = 4,
        Office =5,
        Currency=6,
        Payway = 7,
        IncomeTax = 8,
        TaxNote = 9,
        CustomTax = 10,
        OtherTaxes =11
    }
    public enum SettingType
    {
        QueryTax = 0,
        TaxPayment = 1,
        DomesticPayment = 4,
        DomesticLookup = 5,
        Miarie = 6,
        QueryCredit=7,
        QueryCBS=8,
        QueryNIF = 9,
        QueryDeclarant = 10,
        RegidesoPayBill = 11,
        RegidesoBuyToken = 12

    }

    public enum PaywayType
    {
        BuyToken= 2,
        RetailerTopUp = 3,
    }

    public enum OBRFunctionType
    {
        QueryTax = 0,
        TaxPayment = 1,
        LookUp = 2,
        AddTran = 3,
        ValidateTin = 4,
        QueryCredit = 5,
        QueryTranRef = 6,
        QueryNIF = 9,
        QueryDeclarant = 10
    }

    public enum Miarie
    {
        QueryTaxNote = 500,
        QueryByRef = 501,
        MakeTaxPay = 502,
    }
}
