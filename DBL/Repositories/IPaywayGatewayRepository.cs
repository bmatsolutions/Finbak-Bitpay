using System;
using System.Collections.Generic;
using System.Text;
using BITPay.DBL.Entities;
using BITPay.DBL.Enums;
using BITPay.DBL.Models;

namespace BITPay.DBL.Repositories
{
    public interface IPaywayGatewayRepository
    {
        GenericModel GetDetails(PaywayType paywayType);
        GenericModel GetPayMod(int payMode);
        GenericModel ValidatePayment(PowerPaymentModel model);
        GenericModel ValidateRetailerPayment(RetailerTopUpModel model);
        TaxToPostModel MakePayment(PowerPayments payment);
        BaseEntity UpdatePaymentStatus(int paymentCode, int status, string message);
        IEnumerable<PowerPaymentModel> GetApprovalQueue();
        PowerPaymentModel GetApprovalQueueItem(int code);
        TaxToPostModel SupervisePayment(int paymentCode, int action, string reason, int userCode);
        TaxToPostModel RMakePayment(RetailerPayments payment);
        IEnumerable<RetailerTopUpModel> GetTopUpApprovalQueue();
        RetailerTopUpModel GetRetailerApprovalQueueItem(int code);
        TaxToPostModel SuperviseTopUpPayment(int paymentCode, int action, string reason, int userCode);
        GenericModel SavePowerPayment(PowerToPost powerToken, int paymentCode);
        BaseEntity UpdateTopUpStatus(int paymentCode, int respStatus, string message, string topUpStatus, string topUpNarration, string topUpTxid);
        TokenReportModels GetPaymentReceipt(int paymentCode);
        TopUpReportModels GetTopUpPaymentReceipt(int paymentCode);
        IEnumerable<TokenReportModels> GetPowerPayments(int usercode, string assesNo, string dateFrom, string dateTo);
        IEnumerable<TopUpReportModels> GetTopUpPayments(int usercode, string assesNo, string dateFrom, string dateTo);
        BaseEntity UpdatePowerChargeStatus(int chargeCode, int postStat, string postMsg);
    }
}
