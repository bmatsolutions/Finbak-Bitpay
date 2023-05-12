using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public interface ITaxRepository
    {
        GenericModel CreateFile(TaxFile tax);
        TaxPaymentModel CustSearch(string CBSref);
        IEnumerable<TaxFileMiniModel> GetFiles(string dateRange, int fileStatus, string assesNo);
        IEnumerable<TaxFile> GetFilesByStatus(int status);

        Task<GenericModel> ValidateTaxPaymentAsync(TaxPaymentModel payment);
        
        Task<TaxToPostModel> MakePaymentAsync(Payment payment);
        Task<BaseEntity> UpdatePaymentStatusAsync(int paymentCode, int status, string message);
        BaseEntity UpdateTaxChargeStatus(int chargeCode, int status, string message);

        ReceiptReportModels GetPaymentReceipt(int paymentCode);

        Task<IEnumerable<TaxPaymentModel>> GetApprovalQueueAsync(int taxType);
        Task<TaxPaymentModel> GetApprovalQueueItemAsync(int code);
        Task<TaxToPostModel> SupervisePaymentAsync(int paymentCode, int action, string reason, int userCode);

        Task<GenericModel> GetPayModeAsyc(int payMode);
        Task<GenericModel> GetPaymentStatusAsync(int code);
        IEnumerable<ApprovedPayments> ApprovedPayments(int usercode = 0, string assesNo = "");

        Task<GenericModel> TempTaxDeclAsync(DeclarationQueryResponse tax, int mode = 0, int userCode = 0);
        IEnumerable<DeclarationQueryResponse> Get_TempTaxDets(int mode = 0, int ucode = 0);
        DeclarationQueryResponse GetTempTaxDets(int mode, int Code);
        DeclarationQueryResponse GetTempTaxDets_uCode(int mode, int userCode);
        DeclarationQueryResponse GetAllTempTaxDets(int mode, int userCode);
        Task<BaseEntity> UpdateFileCodeTempDeclAsync(int Filecode, int usercode);
        BaseEntity ArchTempDecl(int Filecode);
        GenericModel Val_Declaration(DeclarationQueryData model, int UserCode);
        Task<IEnumerable<ReceiptReportModels>> GetTaxPaymentsAsync(int usercode, string assesNo, string dateFrom, string dateTo);
        IEnumerable<CreditSlipData> GetCreditSlipData(int paymentCode);
        IEnumerable<CreditSlipData> GetBulkData(int paymentCode);
        IEnumerable<CreditSlipData> GetBulkOther(int paymentCode);
        Task<GenericModel> GetOBRStatusAsync(int code);
    }
}
