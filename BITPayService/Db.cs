using BITPayService.Models;
using Dapper;
using OBRClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BITPayService
{
    public class Db
    {
        private string connString;
        public Db(string dbConnString)
        {
            this.connString = dbConnString;
        }

        public GenericModel GetSettings(int type)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("SettingType", type);

                    return conn.Query<GenericModel>("sp_GetSystemSetting", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("GetSettings", ex);
            }

            return null;
        }

        public List<MakePaymentRequestModel> GetPayments()
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = "Select * From vw_GetOBRPendingPayments";
                    DynamicParameters parameters = new DynamicParameters();
                    return conn.Query<MakePaymentRequestModel>(sql, parameters, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("GetPayments", ex);
            }
            return new List<MakePaymentRequestModel>();
        }
        public List<DomesticTaxPayment> GetDomesticPayments()
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = "Select * From vw_Pending_DomesticTaxFiles";
                    DynamicParameters parameters = new DynamicParameters();
                    return conn.Query<DomesticTaxPayment>(sql, parameters, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("GetDomesticPayments", ex);
            }
            return new List<DomesticTaxPayment>();
        }
        public List<MakePaymentRequestModel> Getmultipayments()
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = "Select * From vw_GetOBRPendingMultiplePayments";
                    DynamicParameters parameters = new DynamicParameters();
                    return conn.Query<MakePaymentRequestModel>(sql, parameters, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("Getmultipayments", ex);
            }
            return new List<MakePaymentRequestModel>();
        }

        public List<BulkData> GetBulkDataInfo(int fileCode)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    string sql = "Select * From vw_GetOBRPendingBulkPayments where FileCode=" + fileCode;
                    return conn.Query<BulkData>(sql, parameters, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("GetBulkDataInfo", ex);
            }
            return new List<BulkData>();
        }

        public GenericModel UpdateFileStatus(int fileCode, int stat, string statMessage, string recNo, string recDate, string cmpName, string dclntName)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@FileCode", fileCode);
                    parameters.Add("@Stat", stat);
                    parameters.Add("@Msg", statMessage);
                    parameters.Add("@RecNo", recNo);
                    parameters.Add("@RecDate", recDate);
                    parameters.Add("@cmpName", cmpName);
                    parameters.Add("@dclntName", dclntName);

                    return conn.Query<GenericModel>("sp_UpdateTaxFileStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("GetSettings", ex);
            }

            return null;
        }

        public DomesticResp UpdateDomesticPaymentStatusAsync(int paytype, int paymentCode, int status, string message, string content)
        {
            using (var connection = new SqlConnection(connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PayType", paytype);
                parameters.Add("@PaymentCode", paymentCode);
                parameters.Add("@Stat", status);
                parameters.Add("@Msg", message);
                parameters.Add("@ObrCode", content);
                return connection.Query<DomesticResp>("sp_UpdateDomesticTaxPaymentStatus", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
        public GenericModel InsertCreditPayment(DataTable fileData, int fileCode)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@FileCode", fileCode);
                    parameters.Add("@FileData", fileData.AsTableValuedParameter("dbo.CreditStatementData"));

                    return conn.Query<GenericModel>("sp_InsertCreditState", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("InsertCreditPayment", ex);
            }

            return null;
        }

        public async Task<List<MairieReportDataItem>> GetMairieReportDataAsync()
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();

                    return(await conn.QueryAsync<MairieReportDataItem>("sp_GetMairieReportData", parameters, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                Util.LogError("GetMairieReportDataAsync", ex);
            }
            return new List<MairieReportDataItem>();
        }
    }
}
