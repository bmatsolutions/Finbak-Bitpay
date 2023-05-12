using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public class MiarieRepository : BaseRepository, IMiarieRepository
    {
        public MiarieRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<TaxToPostModel> MakePaymentAsync(MiariePayment payment)
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

                return await connection.QueryFirstOrDefaultAsync<TaxToPostModel>("sp_MakeMiariePayment", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<GenericModel> CreateTaxAsync(MiarieTaxFile tax)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", tax.UserCode);
                parameters.Add("@TaxAmount", tax.TaxAmount);
                parameters.Add("@NoteNo", tax.NoteNo);
                parameters.Add("@NoteType", tax.NoteType);
                parameters.Add("@Payer", tax.PayerName);
                parameters.Add("@Period", tax.Period);
                parameters.Add("@RefNo", tax.RefNo);
                parameters.Add("@Descr", tax.Descr);
                parameters.Add("@Title", tax.Title);
                parameters.Add("@TypeName", tax.TypeName);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_CreateMiarieFile", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<BaseEntity> UpdateStatusAsync(int paymentCode, int status, string message, string extra1 = "", string extra2 = "", string extra3 = "")
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);
                parameters.Add("@Extra1", extra1);
                parameters.Add("@Extra2", extra2);
                parameters.Add("@Extra3", extra3);

                return await connection.QuerySingleOrDefaultAsync<BaseEntity>("sp_UpdateMiarieFileStatus", parameters, commandType: CommandType.StoredProcedure);
            }
        }
       
        public TaxToPostModel SuperviseMiarieTax(int FileCode, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", FileCode);
                parameters.Add("@Action", action);
                parameters.Add("@Reason", reason);
                parameters.Add("@UserCode", userCode);

                return connection.Query<TaxToPostModel>("sp_SuperviseMiarieTax", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public MiarieReportModels  GetMiariePaymentReceipt(int paymentCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", paymentCode);

                string sql = FindStatement("vw_MiarieReceipts", "PaymentCode");

                return connection.Query<MiarieReportModels>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }
        

        public async Task<IEnumerable<MiariePaymentModel>> GetPaymentsAsync(int status, string dateFrom, string dateTo)
        {
            using (IDbConnection Connection = new SqlConnection(_connString))
            {
                Connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Stat", status);
                parameters.Add("@From", dateFrom);
                parameters.Add("@To", dateTo);

                var sql = "Select top 20 * From vw_MiarieTaxPayments Where StatusCode = @Stat and PayDate Between @From and @To Order By PayDate Desc";

                return (await Connection.QueryAsync<MiariePaymentModel>(sql, parameters)).ToList();
            }
        }

        public async Task<GenericModel> GetRepostTaxAsync(int fileCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FileCode", fileCode);


                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_GetRepostMiarieTax", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
