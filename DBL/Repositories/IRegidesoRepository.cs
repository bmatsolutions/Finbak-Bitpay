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
        IEnumerable<BuyTokenReportModels> GetPrePayApprovalList(int stat);
        Bills GetPostPay(int paymentCode);
        IEnumerable<PostPayReportModels> GetPostPayApprovalList(int stat);
        PostPayReportModels GetPostPayReceipt(string paymentCode);
        Task<GenericModel> ValidatePostBillPaymentAsync(Bills payment);
        Task<GenericModel> ValidateBillPaymentAsync(PrePaidModel payment);
        Task<BuyTokenPostModel> MakePrePaymentAsync(PrePaidModel payment);
        Task<BuyTokenPostModel> MakePostPaymentAsync(Bills payment);
        Task<BuyTokenPostModel> MakeApprovalPostPaymentAsync(Bills payment);
        IEnumerable<PostPayReportModels> GetPayBillList();
        Task<BaseEntity> UpdatePostPaymentStatusAsync(int paymentCode, int status, string message);
        Task<BaseEntity> UpdatePrePaymentStatusAsync(int paymentCode, int status, string message);
        IEnumerable<BuyTokenReportModels> GetPrePayList();
        IEnumerable<PostPayReportModels> GetPayBillListPayments(int stat);
        IEnumerable<BuyTokenReportModels> GetPrePayListPayments(int stat);
    }
}
