using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BITPay.DBL.Entities;
using BITPay.DBL.Models;

namespace BITPay.DBL.Repositories
{
    public interface IDomesticRepository
    {
        TaxToPostModel MakePayment(Payment payment);
        Task<GenericModel> ValidateDeclarationAsync(IncomeTaxDecl model, int userCode);
        IEnumerable<DomesticTaxFileMiniModel> GetDomesticFiles(string dateRange, int fileStatus, string assesNo);
        GenericModel ValidateDomesticTaxPayment(DomesticTaxPayment domesticTaxPayment);
        Task<BaseEntity> UpdateDomesticPaymentStatusAsync(int paytype,int paymentCode, int status, string message, string content);
        GenericModel GetCharge(int payMode);

        Task<DomesticReportModels> GetDomesticPaymentReceiptAsync(int paymentCode);
        Task<GenericModel> CreateFileAsync(DomesticTaxFiles taxFile);
        TaxToPostModel MakePayment(DomesticPayments payment);
        IEnumerable<DomesticTaxPayment> GetDomesticTaxFiles();
        Task<DomesticTaxPayment> GetDomesticTaxFileAsync(int code);
        Task<TaxToPostModel> SuperviseDomesticTaxAsync(int FileCode, int action, string reason, int userCode);
        Task<IEnumerable<DomesticReportModels>> GetDomesticTaxPaymentsAsync(int usercode, string assesNo, string dateFrom, string dateTo);
        IEnumerable<DomesticReportModels> ApprovedDomesticPayments(int usercode, string assesNo);
        BaseEntity UpdateDomesticTaxChargeStatus(int chargeCode, int status, string message);
        Task<DomesticTaxPayment> GetDomesticTaxDataAsync(int fileCode);

        Task<BaseEntity> ValidateObrOffice(string officecode);

    }
}
