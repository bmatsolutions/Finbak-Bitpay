using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using Dapper;

namespace BITPay.DBL.Repositories
{
    public class TaxRepository : BaseRepository, ITaxRepository
    {
        public TaxRepository(string connectionString) : base(connectionString)
        {
        }

        public GenericModel CreateFile(TaxFile tax)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", tax.UserCode);
                parameters.Add("@TaxAmount", tax.TaxAmount);
                parameters.Add("@OfficeCode", (tax.OfficeCode ?? "").ToUpper());
                parameters.Add("@OfficeName", (tax.OfficeName ?? "").ToUpper());
                parameters.Add("@DclntCode", tax.DclntCode);
                parameters.Add("@DclntName", tax.DclntName);
                parameters.Add("@CompanyCode", tax.CompanyCode);
                parameters.Add("@CompanyName", tax.CompanyName);
                parameters.Add("@RegYear", tax.RegYear);
                parameters.Add("@AsmtSerial", (tax.AsmtSerial ?? "").ToUpper());
                parameters.Add("@AsmtNumber", (tax.AsmtNumber ?? "").ToUpper());
                parameters.Add("@RegSerial", (tax.RegSerial ?? "").ToUpper());
                parameters.Add("@RegNumber", (tax.RegNumber ?? "").ToUpper());
                parameters.Add("@Extra1", tax.Extra1);
                parameters.Add("@Extra2", tax.Extra2);
                parameters.Add("@Extra3", tax.Extra3);
                parameters.Add("@Extra4", tax.Extra4);
                parameters.Add("@PayType", tax.PayType);
                parameters.Add("@AccHolder", tax.AccountHolder);
                parameters.Add("@PayerName", tax.TaxPayerName);
                parameters.Add("@TranCode", tax.TransactionCode);
                parameters.Add("@TranRef", tax.TransactionRef);
                parameters.Add("@Currency", tax.Currency);

                return connection.Query<GenericModel>("sp_CreateTaxFile", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public TaxPaymentModel CustSearch(string CBSref)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@payMode", CBSref);

                return connection.Query<TaxPaymentModel>("sp_GetpayMode", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<TaxPaymentModel>> GetApprovalQueueAsync(int taxType)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TaxType", taxType);

                return (await Connection.QueryAsync<TaxPaymentModel>("sp_GetPaymentApprovalQueue", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task<TaxPaymentModel> GetApprovalQueueItemAsync(int code)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", code);

                return await Connection.QueryFirstOrDefaultAsync<TaxPaymentModel>("sp_GetPaymentApprovalItem", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<TaxFileMiniModel> GetFiles(string dateRange, int fileStatus, string assesNo)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = "Select top 50 * From vw_TaxFilesMini";
                return Connection.Query<TaxFileMiniModel>(
                  sql
              ).ToList();
            }
        }

        public IEnumerable<TaxFile> GetFilesByStatus(int status)
        {
            throw new NotImplementedException();
        }

        public async Task< IEnumerable<ReceiptReportModels>> GetTaxPaymentsAsync(int usercode, string assesNo, string dateFrom, string dateTo)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                parameters.Add("@dateFrom", dateFrom);
                parameters.Add("@dateTo", dateTo);

                return(await connection.QueryAsync<ReceiptReportModels>("sp_GetPayments", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        

        public ReceiptReportModels GetPaymentReceipt(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_PaymentReceipts", "PaymentCode");

                return connection.Query<ReceiptReportModels>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public IEnumerable<CreditSlipData> GetCreditSlipData(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);
                string sql = ("Select a.Office,a.AssessmentYear,a.AssessmentSerial,a.AssessmentSerial RegYear,a.AssessmentNumber,a.Amount from CreditSlipPaymentsData a Inner Join Payments b on b.FileCode=a.FileCode where b.PaymentCode=@Id");

                return connection.Query<CreditSlipData>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }

        public IEnumerable<CreditSlipData> GetBulkData(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);
                string sql = ("Select a.officeCode Office,a.AssessmentYear RegYear,a.AssessmentSerial,a.Registrationnumber RegNo,a.RegistrationSerial RegSerial,a.AssessmentNumber,a.amountToBePaid Amount from TempTaxDets_Arch a Inner Join Payments b on b.FileCode=a.FileCode where b.PaymentCode=@Id");

                return connection.Query<CreditSlipData>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }

        public IEnumerable<CreditSlipData> GetBulkOther(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);
                string sql = ("Select a.officeCode Office,a.AssessmentYear RegYear,a.AssessmentSerial RegSerial,a.AssessmentNumber RegNo,a.TranCode TransactionCode,a.TranRef TransactionRef,a.payName PayerName,a.amountToBePaid Amount,(Select ItemName from SysSettings where Descr='Report' and ItemValue=a.Currency) as Currency from TempTaxDets_Arch a Inner Join Payments b on b.FileCode=a.FileCode where b.PaymentCode=@Id");

                return connection.Query<CreditSlipData>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }

        public async Task< GenericModel> GetPayModeAsyc(int payMode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@payMode", payMode);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_GetpayMode", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task< GenericModel> GetPaymentStatusAsync(int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@code", code);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_GetPaymentStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<GenericModel> GetOBRStatusAsync(int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@code", code);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_GetFileStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<ApprovedPayments> ApprovedPayments(int usercode = 0, string assesNo = "")
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                return connection.Query<ApprovedPayments>("sp_ApprovedPayments", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }
        public async Task< TaxToPostModel> MakePaymentAsync(Payment payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", payment.UserCode);
                parameters.Add("@FileCode", payment.FileCode);
                parameters.Add("@Amount", payment.Amount);
                parameters.Add("@ModeCode", payment.ModeCode);
                parameters.Add("@Remarks", payment.Remarks);
                parameters.Add("@Extra1", payment.Extra1);
                parameters.Add("@Extra2", payment.Extra2);
                parameters.Add("@Extra3", payment.Extra3);
                parameters.Add("@Extra4", payment.Extra4);
                parameters.Add("@Dr_Account", payment.Dr_Account);
                parameters.Add("@ApplyCharge", payment.ApplyCharge);
                parameters.Add("@TaxType", payment.TaxType);

                return await connection.QueryFirstOrDefaultAsync<TaxToPostModel>("sp_MakeTaxPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<TaxToPostModel> SupervisePaymentAsync(int paymentCode, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Action", action);
                parameters.Add("@Reason", reason);
                parameters.Add("@UserCode", userCode);

                return await connection.QueryFirstOrDefaultAsync<TaxToPostModel>("sp_SuperviseTaxPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<BaseEntity> UpdatePaymentStatusAsync(int paymentCode, int status, string message)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);

                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_UpdateTaxPaymentStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public BaseEntity UpdateTaxChargeStatus(int chargeCode, int status, string message)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ChargeCode", chargeCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);

                return connection.Query<BaseEntity>("sp_UpdateTaxChargeStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task< GenericModel> ValidateTaxPaymentAsync(TaxPaymentModel payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", payment.FileCode);
                parameters.Add("@Amount", payment.Amount);
                parameters.Add("@ModeCode", payment.PayMode);
                parameters.Add("@AccountNo", payment.AccountNo);
                parameters.Add("@ChequeNo", payment.ChequeNo);
                parameters.Add("@NoCharge", payment.NoCharge);
                parameters.Add("@SortCode", payment.SortCode);
                parameters.Add("@PostToCBS", payment.PostToCBS);
                parameters.Add("@CBSRef", payment.CBSRef);
                parameters.Add("@TaxType", payment.TaxType);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_ValidateTaxPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task< GenericModel> TempTaxDeclAsync(DeclarationQueryResponse tax, int mode = 0, int userCode = 0)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@mode", mode);
                parameters.Add("@PayType",tax.PayType);
                parameters.Add("@userCode", userCode);
                parameters.Add("@declCode", tax.declCode);
                parameters.Add("@officeCode", tax.OfficeCode);
                parameters.Add("@officeName", tax.OfficeName);
                parameters.Add("@declarantCode", tax.DeclarantCode);
                parameters.Add("@declarantName", tax.DeclarantName);
                parameters.Add("@companyCode", tax.CompanyCode);
                parameters.Add("@companyName", tax.CompanyName);
                parameters.Add("@assessmentYear", tax.AssessmentYear);
                parameters.Add("@assessmentSerial", tax.AssessmentSerial);
                parameters.Add("@assessmentNumber", tax.AssessmentNumber);
                parameters.Add("@RegistrationNumber", tax.RegistrationNumber);
                parameters.Add("@RegistrationSerial", tax.RegistrationSerial);
                parameters.Add("@amountToBePaid", tax.AmountToBePaid);
                parameters.Add("@receiptSerial", tax.ReceiptSerial);
                parameters.Add("@receiptNumber", tax.ReceiptNumber);
                parameters.Add("@receiptDate", tax.ReceiptDate);
                parameters.Add("@payerName", tax.TaxPayerName);
                parameters.Add("@tranCode", tax.TransactionCode);
                parameters.Add("@tranRef", tax.TransactionRef);
                parameters.Add("@currency", tax.Currency);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_TempTaxDecl", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<DeclarationQueryResponse> Get_TempTaxDets(int mode = 0, int ucode = 0)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@mode", mode);
                parameters.Add("@userCode", ucode);
                return connection.Query<DeclarationQueryResponse>("sp_GetTempTaxDets", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }
        public DeclarationQueryResponse GetTempTaxDets(int mode, int Code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@mode", mode);
                parameters.Add("@Code", Code);
                return connection.Query<DeclarationQueryResponse>("sp_GetTempTaxDets", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
        public DeclarationQueryResponse GetTempTaxDets_uCode(int mode, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@mode", mode);
                parameters.Add("@userCode", userCode);
                return connection.Query<DeclarationQueryResponse>("sp_GetTempTaxDets", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
        public DeclarationQueryResponse GetAllTempTaxDets(int mode, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@mode", mode);
                parameters.Add("@userCode", userCode);
                return connection.Query<DeclarationQueryResponse>("sp_GetTempTaxDets", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task< BaseEntity> UpdateFileCodeTempDeclAsync(int Filecode, int usercode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Filecode", Filecode);
                parameters.Add("@usercode", usercode);

                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_UpdateFileCodeTempDecl", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public BaseEntity ArchTempDecl(int Filecode)
        {
          
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Filecode", Filecode);
                return connection.Query<BaseEntity>("sp_ArchTempDecl", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel Val_Declaration(DeclarationQueryData model, int UserCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@OfficeCode", model.OfficeCode);
                parameters.Add("@RegYear", model.RegistrationYear);
                parameters.Add("@RegSerial", model.RegistrationSerial);
                parameters.Add("@RegNumber", model.RegistrationNumber);
                parameters.Add("@PayType", Convert.ToInt32(model.TaxType));
                parameters.Add("@AccRef", model.AccountReference);
                parameters.Add("@RefNo", model.ReferenceNumber);
                parameters.Add("@RefYear", model.ReferenceYear);
                parameters.Add("@Maker", UserCode);
                return connection.Query<GenericModel>("sp_validateDecl", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        
    }
}
