using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using Dapper;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace BITPay.DBL.Repositories
{
    public class RegidesoRepository : BaseRepository, IRegidesoRepository
    {
        public RegidesoRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<GenericModel> CreatePostPaymentAsync(Bills bill)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@InvoiceNo", bill.Invoice_no);
                parameters.Add("@PhoneNo", bill.PhoneNo);
                parameters.Add("@Amount", bill.Amnt);
                parameters.Add("@AmountPaid", bill.Amnt_paid);
                parameters.Add("@ClientName", bill.Cust_name);
                parameters.Add("@PayMode", bill.PayMode);
                parameters.Add("@Stat", bill.stat);
                parameters.Add("@Maker", bill.Maker);
                parameters.Add("@SortCode", bill.SortCode);
                parameters.Add("@ChequeNo", bill.ChequeNo);
                parameters.Add("@ReceivedFrom", bill.ReceivedFrom);
                parameters.Add("@Remarks", bill.Remarks);
                parameters.Add("@DRAccount", bill.Accnt_no);
                parameters.Add("@DeductionAmount", bill.Amnt);
                parameters.Add("@InvoiceAmount", bill.Amnt_paid);
                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_AddPostPayTrns", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<GenericModel> CreatePrePaymentAsync(PrePaidModel bill)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@MeterNo", bill.Meter_No);
                parameters.Add("@AccountNo", bill.Accnt_no);
                parameters.Add("@Amount", bill.Amnt);
                parameters.Add("@PayAmount", bill.Amnt);
                parameters.Add("@Consumption", bill.Consumption);
                parameters.Add("@AdvanceCredit", bill.AdvanceCredit);
                parameters.Add("@Royalty", bill.Royalty);
                parameters.Add("@PrimeFixed", bill.PrimeFixed);
               parameters.Add("@AmountFine", bill.AmountFined);
               parameters.Add("@SurCharge", bill.SurCharge);
                parameters.Add("@Tax", bill.Tax);
                parameters.Add("@Vat", bill.vat);
               parameters.Add("@LastReadingDate", bill.Lastreadingdate);
                parameters.Add("@NewReadingDate", bill.Newreadingdate);
                parameters.Add("@CustName", bill.Cust_Name);
                parameters.Add("@CustCode", bill.Cust_Code);
               parameters.Add("@Token1", bill.Token1);
                parameters.Add("@Token2", bill.Token2);
                parameters.Add("@Token3", bill.Token3);
                parameters.Add("@ReceivedCode", bill.receivedCode);
                parameters.Add("@checkoutsite", bill.CheckoutSite);
                parameters.Add("@ErrorCode", bill.ErrorCode);
                parameters.Add("@Maker", bill.Maker);
                parameters.Add("@SortCode", bill.SortCode);
                parameters.Add("@ChequeNo", bill.ChequeNo);
                parameters.Add("@PayMode", bill.PayMode);
                parameters.Add("@ReceivedFrom", bill.ReceivedFrom);
                parameters.Add("@Remarks", bill.Remarks);
                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_AddRGPrePaidTrns", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<GenericModel> UpdatePrePayment(PrePaidModel bill)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@BillCode", bill.BillCode);
                parameters.Add("@MeterNo", bill.Meter_No);
                parameters.Add("@AccountNo", bill.Accnt_no);
                parameters.Add("@Amount", bill.Amnt);
                parameters.Add("@PayAmount", bill.PayAmount);
                parameters.Add("@Consumption", bill.Consumption);
                parameters.Add("@AdvanceCredit", bill.AdvanceCredit);
                parameters.Add("@Royalty", bill.Royalty);
                parameters.Add("@PrimeFixed", bill.PrimeFixed);
                parameters.Add("@AmountFine", bill.AmountFined);
                parameters.Add("@SurCharge", bill.SurCharge);
                parameters.Add("@Tax", bill.Tax);
                parameters.Add("@Vat", bill.vat);
                parameters.Add("@LastReadingDate", bill.Lastreadingdate);
                parameters.Add("@NewReadingDate", bill.Newreadingdate);
                parameters.Add("@CustName", bill.Cust_Name);
                parameters.Add("@CustCode", bill.Cust_Code);
                parameters.Add("@Token1", bill.Token1);
                parameters.Add("@Token2", bill.Token2);
                parameters.Add("@Token3", bill.Token3);
                parameters.Add("@Stat", bill.Stat);
                parameters.Add("@ReceivedCode", bill.receivedCode);
                parameters.Add("@checkoutsite", bill.CheckoutSite);
                parameters.Add("@ErrorCode", bill.ErrorCode);
                parameters.Add("@Maker", bill.Maker);
                parameters.Add("@SortCode", bill.SortCode);
                parameters.Add("@ChequeNo", bill.ChequeNo);
                parameters.Add("@PayMode", bill.PayMode);
                parameters.Add("@ReceivedFrom", bill.ReceivedFrom);
                parameters.Add("@Remarks", bill.Remarks);
                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_UpdateRGPrePaidTrns", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public BuyTokenReportModels GetPrePayReceipt(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_RGPrePayReceipts", "BillCode");

                return connection.Query<BuyTokenReportModels>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }
        public PostPayReportModels GetPostPayReceipt(string paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_RGPostPayReceipts", "InvoiceNo");

                return connection.Query<PostPayReportModels>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }
        public PrePaidModel GetPrePay(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_RGPrePayReceipts", "BillCode");

                return connection.Query<PrePaidModel>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }
        public IEnumerable<BuyTokenReportModels>  GetPrePayApprovalList(int stat)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", stat);

                string sql = FindStatement("vw_RGPrePayReceipts", "Stat");

                return connection.Query<BuyTokenReportModels>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }
        public Bills GetPostPay(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_RGPostPayReceipts", "BillCode");

                return connection.Query<Bills>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }
        public IEnumerable<PostPayReportModels>  GetPostPayApprovalList(int stat)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", stat);

                string sql = FindStatement("vw_RGPostPayReceipts", "Stat");

                return connection.Query<PostPayReportModels>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }
        public IEnumerable<BuyTokenReportModels> GetPrePayListPayments(int stat)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", stat);

                string sql = FindStatement("vw_RGPrePayReceipts", "Stat");

                return connection.Query<BuyTokenReportModels>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }
        public async Task<GenericModel> ValidatePostBillPaymentAsync(Bills payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@BillCode", payment.txnId);
                parameters.Add("@Amount", payment.Amnt);
                parameters.Add("@ModeCode", payment.PayMode);
                parameters.Add("@AccountNo", payment.Accnt_no);
                parameters.Add("@ChequeNo", payment.ChequeNo);
                parameters.Add("@SortCode", payment.SortCode);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_ValidatePostBillPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<GenericModel> ValidateBillPaymentAsync(PrePaidModel payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Invoice_no", payment.Invoice_no);
                parameters.Add("@Amount", payment.Amnt);
                parameters.Add("@ModeCode", payment.PayMode);
                parameters.Add("@AccountNo", payment.Accnt_no);
                parameters.Add("@ChequeNo", payment.ChequeNo);
                parameters.Add("@SortCode", payment.SortCode);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_ValidateBillPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<PostPayReportModels> GetPayBillListPayments(int stat)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", stat);

                string sql = FindStatement("vw_RGPostPayReceipts", "Stat");

                return connection.Query<PostPayReportModels>(sql, parameters, commandType: CommandType.Text).ToList();
            }
        }
        public async Task<BuyTokenPostModel> MakePrePaymentAsync(PrePaidModel payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", payment.Maker);
                parameters.Add("@BillCode", payment.BillCode);
                parameters.Add("@Amount", payment.Amnt);
                parameters.Add("@ModeCode", payment.PayMode);
                parameters.Add("@Remarks", payment.Remarks);
                parameters.Add("@Extra1", payment.ChequeNo);
                parameters.Add("@Extra2", payment.SortCode);
                parameters.Add("@Extra3", payment.Extra1);
                parameters.Add("@Extra4", payment.Extra2);
                parameters.Add("@Dr_Account", payment.DrAccount);

                return await connection.QueryFirstOrDefaultAsync<BuyTokenPostModel>("sp_RGPrePaidPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<BuyTokenPostModel> MakePostPaymentAsync(Bills payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", payment.Maker);
                parameters.Add("@BillCode", payment.BillCode);
                parameters.Add("@Amount", payment.Amnt);
                parameters.Add("@ModeCode", payment.PayMode);
                parameters.Add("@Remarks", payment.Remarks);
                parameters.Add("@Extra1", payment.ChequeNo);
                parameters.Add("@Extra2", payment.SortCode);
                parameters.Add("@Extra3", payment.Extra1);
                parameters.Add("@Extra4", payment.Extra2);
                parameters.Add("@Dr_Account", payment.DrAccount);

                return await connection.QueryFirstOrDefaultAsync<BuyTokenPostModel>("sp_RGPostPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<BuyTokenPostModel> MakeApprovalPostPaymentAsync(Bills payment)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", payment.Maker);
                parameters.Add("@BillCode", payment.BillCode);
                parameters.Add("@Amount", payment.Amount);
                parameters.Add("@ModeCode", payment.PayMode);
                parameters.Add("@Remarks", payment.Remarks);
                parameters.Add("@Extra1", payment.ChequeNo);
                parameters.Add("@Extra2", payment.SortCode);
                parameters.Add("@Extra3", payment.Extra1);
                parameters.Add("@Extra4", payment.Extra2);
                parameters.Add("@Dr_Account", payment.Accnt_no);

                return await connection.QueryFirstOrDefaultAsync<BuyTokenPostModel>("sp_RGPostPayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<BaseEntity> UpdatePostPaymentStatusAsync(int paymentCode, int status, string message)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@BillCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);

                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_UpdatePostPayStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<BaseEntity> UpdatePrePaymentStatusAsync(int paymentCode, int status, string message)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@BillCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);

                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_UpdatePrePayStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public IEnumerable<PostPayReportModels> GetPayBillList()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                return connection.Query<PostPayReportModels>(GetAllStatement("vw_RGPostPayReceipts")).ToList();
            }
        }
        public IEnumerable<BuyTokenReportModels> GetPrePayList()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<BuyTokenReportModels>(GetAllStatement("vw_RGPrePayReceipts")).ToList();
            }
        }

    }
}
