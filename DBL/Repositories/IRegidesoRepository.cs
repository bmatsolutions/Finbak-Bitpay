using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public interface IRegidesoRepository
    {
        Task<GenericModel> CreatePostPaymentAsync(Bills bill);
        Task<GenericModel> CreatePrePaymentAsync(PrePaidModel bill);
        Task<GenericModel> UpdatePrePayment(PrePaidModel bill);
        BuyTokenReportModels GetPrePayReceipt(int paymentCode);
        PrePaidModel GetPrePay(int paymentCode);
        IEnumerable<BuyTokenReportModels> GetPrePayApprovalList(int type, int stat,int code, string assesNo, string dateFrom, string dateTo);
        PostPayReportModels GetPostPay(int paymentCode);
        IEnumerable<PostPayReportModels> GetPostPayApprovalList(int type, int stat,int code,string assesNo, string dateFrom, string dateTo);
        PostPayReportModels GetPostPayReceipt(string paymentCode);
        Task<GenericModel> ValidatePostBillPaymentAsync(Bills payment);
        Task<GenericModel> ValidateBillPaymentAsync(PrePaidModel payment);
        Task<BuyTokenPostModel> MakePrePaymentAsync(PrePaidModel payment);
        Task<BuyTokenPostModel> MakePostPaymentAsync(Bills payment);
        Task<BuyTokenPostModel> MakeApprovalPostPaymentAsync(PostPayReportModels payment);
        IEnumerable<PostPayReportModels> GetPayBillList();
        Task<BaseEntity> UpdatePostPaymentStatusAsync(int level, int paymentCode, int status, string CbsStat, string Cbsref, string Cbsmessage, string Rgmessage,int receivedcode,string installationcode);
        Task<BaseEntity> UpdatePrePaymentStatusAsync(int level, int paymentCode, int status, string CbsStat, string Cbsref, string Cbsmessage, string Rgmessage);
        IEnumerable<BuyTokenReportModels> GetPrePayList();
        IEnumerable<PostPayReportModels> GetPayBillListPayments(int stat);
        IEnumerable<BuyTokenReportModels> GetPrePayListPayments(int stat);
        Task<IEnumerable<BuyTokenReportModels>> GetPrePayListPaymentsAsync(int stat,int usercode, string assesNo, string dateFrom, string dateTo);
        Task<IEnumerable<PostPayReportModels>> GetPayBillListPaymentsAsync(int stat,int usercode, string assesNo, string dateFrom, string dateTo);
    }
}
