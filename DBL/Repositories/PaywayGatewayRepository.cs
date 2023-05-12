using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BITPay.DBL.Entities;
using BITPay.DBL.Enums;
using BITPay.DBL.Models;
using Dapper;

namespace BITPay.DBL.Repositories
{
    public class PaywayGatewayRepository : BaseRepository, IPaywayGatewayRepository
    {
        public PaywayGatewayRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<PowerPaymentModel> GetApprovalQueue()
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = GetAllStatement("vw_PowerPaymentApprovalQueue");
                return Connection.Query<PowerPaymentModel>(
                  sql
              ).ToList();
            }
        }

        public PowerPaymentModel GetApprovalQueueItem(int code)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = FindStatement("vw_PowerPaymentApprovalQueue", "PaymentCode");
                return Connection.Query<PowerPaymentModel>(
                  sql,
                  param: new { Id = code }
              ).FirstOrDefault();
            }
        }

        public GenericModel GetDetails(PaywayType paywayType)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@SettingType", paywayType);

                return connection.Query<GenericModel>("sp_GetSystemSetting", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public TokenReportModels GetPaymentReceipt(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_PowerTokenReceipts", "PaymentCode");

                return connection.Query<TokenReportModels>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public GenericModel GetPayMod(int payMode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@payMode", payMode);

                return connection.Query<GenericModel>("sp_GetpayMode", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IEnumerable<TokenReportModels> GetPowerPayments(int usercode, string assesNo, string dateFrom, string dateTo)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                parameters.Add("@dateFrom", dateFrom);
                parameters.Add("@dateTo", dateTo);
                return connection.Query<TokenReportModels>("sp_GetPowerPayments", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public RetailerTopUpModel GetRetailerApprovalQueueItem(int code)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = FindStatement("vw_RetailerTopUpPaymentApprovalQueue", "PaymentCode");
                return Connection.Query<RetailerTopUpModel>(
                  sql,
                  param: new { Id = code }
              ).FirstOrDefault();
            }
        }

        public IEnumerable<RetailerTopUpModel> GetTopUpApprovalQueue()
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                string sql = GetAllStatement("vw_RetailerTopUpPaymentApprovalQueue");
                return Connection.Query<RetailerTopUpModel>(
                  sql
              ).ToList();
            }
        }

        public TopUpReportModels GetTopUpPaymentReceipt(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_RetailerReceipts", "PaymentCode");

                return connection.Query<TopUpReportModels>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public IEnumerable<TopUpReportModels> GetTopUpPayments(int usercode, string assesNo, string dateFrom, string dateTo)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                parameters.Add("@dateFrom", dateFrom);
                parameters.Add("@dateTo", dateTo);
                return connection.Query<TopUpReportModels>("sp_GetTopUpPayments", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public TaxToPostModel MakePayment(PowerPayments payment)
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
                parameters.Add("@CustomerNo", payment.CustomerNo);
                parameters.Add("@PhoneNo", payment.PhoneNo);
                parameters.Add("@AccountName", payment.AccountName);

                return connection.Query<TaxToPostModel>("sp_MakePowerPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public TaxToPostModel RMakePayment(RetailerPayments payment)
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
                parameters.Add("@CustomerNo", payment.CustomerNo);
                parameters.Add("@PhoneNo", payment.PhoneNo);
                parameters.Add("@AccountName", payment.AccountName);

                return connection.Query<TaxToPostModel>("sp_MakeRetailerPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel SavePayment(PowerPaymentModel powerPayment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TransactionId", powerPayment.TransactionId);
                parameters.Add("@UserCode", powerPayment.UserCode);
                parameters.Add("@Amount", powerPayment.Amount);
                parameters.Add("@CustomerNumber", powerPayment.CustomerNumber);
                parameters.Add("@ContactInfo", powerPayment.ContactInfo);
                parameters.Add("@ModeCode", powerPayment.mode);
                parameters.Add("@ModeCode", powerPayment.PayMode);
                parameters.Add("@DR_Account", powerPayment.AccountNo);
                parameters.Add("@ChequeNo", powerPayment.ChequeNo);
                parameters.Add("@NoCharge", powerPayment.NoCharge);
                parameters.Add("@SortCode", powerPayment.SortCode);

                return connection.Query<GenericModel>("sp_PowerPayments", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        
        public GenericModel SavePowerPayment(PowerToPost powerToken,int paymentcode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ReceiptNo", powerToken.ReceiptNo);
                parameters.Add("@ExtInfoId", powerToken.ExtInfoId);
                parameters.Add("@TokenNo", powerToken.TokenNo);
                parameters.Add("@ContactInfoNo", powerToken.ContactInfoNo);
                parameters.Add("@CustomerNo", powerToken.CustomerNo);
                parameters.Add("@Amount", powerToken.Amount);
                parameters.Add("@MeterNo", powerToken.MeterNo);
                parameters.Add("@ConsumerName", powerToken.ConsumerName);
                parameters.Add("@TotalUnits", powerToken.TotalUnits);
                parameters.Add("@TokenValue", powerToken.TokenValue);
                parameters.Add("@KwhValue", powerToken.KwhValue);
                parameters.Add("@paymentcode", paymentcode);
                parameters.Add("@vat", powerToken.Vat);

                return connection.Query<GenericModel>("sp_PowerToken", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        
        public TaxToPostModel SupervisePayment(int paymentCode, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Action", action);
                parameters.Add("@Reason", reason);
                parameters.Add("@UserCode", userCode);

                return connection.Query<TaxToPostModel>("sp_SupervisePowerPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public TaxToPostModel SuperviseTopUpPayment(int paymentCode, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Action", action);
                parameters.Add("@Reason", reason);
                parameters.Add("@UserCode", userCode);

                return connection.Query<TaxToPostModel>("sp_SuperviseRetailTopUpPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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

                return connection.Query<BaseEntity>("sp_UpdatePowerPaymentStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public BaseEntity UpdatePowerChargeStatus(int chargeCode, int postStat, string postMsg)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ChargeCode", chargeCode);
                parameters.Add("@Stat", postStat);
                parameters.Add("@Msg", postMsg);

                return connection.Query<BaseEntity>("sp_UpdateTaxChargeStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public BaseEntity UpdateTopUpStatus(int paymentCode, int respStatus, string message, string topUpStatus, string topUpNarration, string topUpTxid)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Stat", respStatus);
                parameters.Add("@Msg", message);
                parameters.Add("@topupstat", topUpStatus);
                parameters.Add("@narration", topUpNarration);
                parameters.Add("@txid", topUpTxid);

                return connection.Query<BaseEntity>("sp_UpdateRetailerTopUpStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel ValidatePayment(PowerPaymentModel model)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", model.TransactionId);
                parameters.Add("@Amount", model.Amount);
                parameters.Add("@ModeCode", model.PayMode);
                parameters.Add("@AccountNo", model.AccountNo);
                parameters.Add("@ChequeNo", model.ChequeNo);
                parameters.Add("@NoCharge", model.NoCharge);
                parameters.Add("@SortCode", model.SortCode);

                return connection.Query<GenericModel>("sp_ValidatePowerPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
        

        public GenericModel ValidateRetailerPayment(RetailerTopUpModel model)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", model.TransactionId);
                parameters.Add("@Amount", model.Amount);
                parameters.Add("@ModeCode", model.PayMode);
                parameters.Add("@AccountNo", model.AccountNo);
                parameters.Add("@ChequeNo", model.ChequeNo);
                parameters.Add("@NoCharge", model.NoCharge);
                parameters.Add("@SortCode", model.SortCode);

                return connection.Query<GenericModel>("sp_ValidateRetailerPayment", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
    }
}
