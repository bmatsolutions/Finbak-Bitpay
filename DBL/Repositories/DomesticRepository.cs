using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using Dapper;

namespace BITPay.DBL.Repositories
{
    public class DomesticRepository : BaseRepository, IDomesticRepository
    {
        public DomesticRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task< GenericModel> CreateFileAsync(DomesticTaxFiles tax)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", tax.UserCode);
                parameters.Add("@TransactionCode", tax.TransactionCode);
                parameters.Add("@TransactionType", tax.TransactionType);
                parameters.Add("@Amount", tax.Amount);
                parameters.Add("@CustomerName", tax.CustomerName);
                parameters.Add("@TaxPeriod", tax.TaxPeriod);
                parameters.Add("@DeclNo", tax.DeclNo);
                parameters.Add("@Tin", tax.Tin);
                parameters.Add("@CommuneName", tax.CommuneName);
                parameters.Add("@Delay", tax.Delay);
                parameters.Add("@Adjustment", tax.Adjustment);
                parameters.Add("@TaxAdjustment", tax.TaxAdjustment);
                parameters.Add("@Service", tax.Service);
                parameters.Add("@Chasis", tax.Chasis);
                parameters.Add("@Imma", tax.Imma);
                parameters.Add("@CarOnwer", tax.CarOnwer);
                parameters.Add("@Contracavation", tax.Contracavation);
                parameters.Add("@Document", tax.Document);
                parameters.Add("@DriverName", tax.DriverName);
                parameters.Add("@Education", tax.Education);
                parameters.Add("@Infraction", tax.Infraction);
                parameters.Add("@LicenceType", tax.LicenceType);
                parameters.Add("@Copies", tax.Copies);
                parameters.Add("@Vehicle", tax.Vehicle);
                parameters.Add("@Word", tax.Word);
                parameters.Add("@Product", tax.Product);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_CreateDomesticTaxFile", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public TaxToPostModel MakePayment(DomesticPayments payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", payment.UserCode);
                parameters.Add("@FileCode", Convert.ToInt32(payment.FileCode));
                parameters.Add("@Amount", payment.Amount);
                parameters.Add("@ModeCode", payment.ModeCode);
                parameters.Add("@ApplyCharge", payment.ApplyCharge);
                parameters.Add("@Dr_Account", payment.AccountNo);
                parameters.Add("@Remarks", payment.Remarks);
                parameters.Add("@Extra1", payment.Extra1);
                parameters.Add("@Extra2", payment.Extra2);
                parameters.Add("@Extra3", payment.Extra3);
                parameters.Add("@Extra4", payment.Extra4);
                parameters.Add("@Dr_Account", payment.Dr_Account);

                var rs = connection.Query<TaxToPostModel>("sp_MakeDomesticTaxPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return rs;
            }
        }

        public GenericModel GetCharge(int payMode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@payMode", payMode);

                return connection.Query<GenericModel>("sp_GetpayMode", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task< TaxToPostModel> SuperviseDomesticTaxAsync(int FileCode, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", FileCode);
                parameters.Add("@Action", action);
                parameters.Add("@Reason", reason);
                parameters.Add("@UserCode", userCode);

                return await connection.QueryFirstOrDefaultAsync<TaxToPostModel>("sp_SuperviseDomesticTax", parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public BaseEntity UpdateDomesticTaxChargeStatus(int chargeCode, int status, string message)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ChargeCode", chargeCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);

                return connection.Query<BaseEntity>("sp_UpdateDomesticTaxChargeStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public BaseEntity UpdatePaymentStatus(int paymentCode, int status, string message)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);

                return connection.Query<BaseEntity>("sp_UpdateTaxPaymentStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task<BaseEntity> UpdateDomesticPaymentStatusAsync(int paytype, int paymentCode, int status, string message, string content)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PayType", paytype);
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);
                parameters.Add("@ObrCode", content);

                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_UpdateDomesticTaxPaymentStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<BaseEntity> ValidateObrOffice(string officecode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@officecode", officecode);
                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_GetObrOffice", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task< DomesticReportModels> GetDomesticPaymentReceiptAsync(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_DomesticTaxReceipts", "PaymentCode");

                return await connection.QueryFirstOrDefaultAsync<DomesticReportModels>(sql, parameters, commandType: CommandType.Text);
            }
        }

        public IEnumerable<DomesticTaxFileMiniModel> GetDomesticFiles(string dateRange, int fileStatus, string assesNo)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = "Select top 50 * From vw_DomesticTaxFiles";
                return Connection.Query<DomesticTaxFileMiniModel>(
                  sql
              ).ToList();
            }
        }

        public GenericModel ValidateDomesticTaxPayment(DomesticTaxPayment domesticTaxPayment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@FileCode", domesticTaxPayment.FileCode);
                parameters.Add("@Amount", domesticTaxPayment.Amount);
                parameters.Add("@ModeCode", domesticTaxPayment.PayMode);
                parameters.Add("@AccountNo", domesticTaxPayment.AccountNo);
                parameters.Add("@ChequeNo", domesticTaxPayment.ChequeNo);
                parameters.Add("@NoCharge", domesticTaxPayment.NoCharge);
                parameters.Add("@SortCode", domesticTaxPayment.SortCode);
                parameters.Add("@PostToCBS", domesticTaxPayment.PostToCBS);
                parameters.Add("@CBSRef", domesticTaxPayment.CBSref);

                return connection.Query<GenericModel>("sp_ValidateDomesticTaxPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }


        public IEnumerable<DomesticTaxPayment> GetDomesticTaxFiles()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                string sql = GetAllStatement("vw_DomesticTaxFiles");
                return connection.Query<DomesticTaxPayment>(
                  sql
              ).ToList();
            }
        }

        public async Task<DomesticTaxPayment> GetDomesticTaxFileAsync(int code)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = FindStatement("vw_DomesticTaxFiles", "FileCode");
                return await Connection.QueryFirstOrDefaultAsync<DomesticTaxPayment>(sql, param: new { Id = code });
            }
        }

        public async Task<GenericModel> ValidateDeclarationAsync(IncomeTaxDecl model, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TransactionType", model.DomesticTaxName);
                parameters.Add("@TaxPeriod", model.Period);
                parameters.Add("@Maker", userCode);
                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_validateDomesticDecl", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task< IEnumerable<DomesticReportModels>> GetDomesticTaxPaymentsAsync(int usercode, string assesNo, string dateFrom, string dateTo)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                parameters.Add("@dateFrom", dateFrom);
                parameters.Add("@dateTo", dateTo);

                return (await connection.QueryAsync<DomesticReportModels>("sp_GetDomesticPayments", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public IEnumerable<DomesticReportModels> ApprovedDomesticPayments(int usercode = 0, string assesNo = "")
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                return connection.Query<DomesticReportModels>("sp_ApprovedDomesticPayments", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public TaxToPostModel MakePayment(Payment payment)
        {
            throw new NotImplementedException();
        }

        public async Task< DomesticTaxPayment> GetDomesticTaxDataAsync(int fileCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@code", fileCode);

                return await connection.QueryFirstOrDefaultAsync<DomesticTaxPayment>("sp_getDomesticDetails", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
