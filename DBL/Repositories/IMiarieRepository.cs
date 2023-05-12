using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public interface IMiarieRepository
    {
        Task<GenericModel> CreateTaxAsync(MiarieTaxFile tax);
        Task<BaseEntity> UpdateStatusAsync(int paymentCode, int respStatus, string message, string extra1 = "", string extra2 = "", string extra3 = "");
        Task<TaxToPostModel> MakePaymentAsync(MiariePayment payment);
        Task<IEnumerable<MiariePaymentModel>> GetPaymentsAsync(int status, string dateFrom, string dateTo);
        TaxToPostModel SuperviseMiarieTax(int fileCode, int action, string reason, int userCode);
        MiarieReportModels GetMiariePaymentReceipt(int paymentCode);
        
        Task<GenericModel> GetRepostTaxAsync(int fileCode);
    }
}
