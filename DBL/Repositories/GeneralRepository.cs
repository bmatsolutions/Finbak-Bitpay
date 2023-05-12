using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL.Entities;
using BITPay.DBL.Enums;
using BITPay.DBL.Models;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace BITPay.DBL.Repositories
{
    public class GeneralRepository : BaseRepository, IGeneralRepository
    {
        public GeneralRepository(string connectionString) : base(connectionString)
        {
        }

        public DBoardModel GetDBoardData(int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);

                
                return connection.Query<DBoardModel>("sp_GetDBoardData", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IEnumerable<ListModel> GetListModel(ListModelType listType)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Type", (int)listType);

                return connection.Query<ListModel>("sp_GetListModel", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public IEnumerable<ListModel> GetPayMode(int mode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
               
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@code", mode);

                return connection.Query<ListModel>("sp_GetIncomeTaxCatergories", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public async Task< GenericModel> GetSystemSettingAsync(SettingType sType)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@SettingType", (int)sType);

                return await connection.QueryFirstOrDefaultAsync<GenericModel>("sp_GetSystemSetting", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<BaseEntity> CheckDuplicate(int filecode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filecode", filecode);
                return await connection.QueryFirstOrDefaultAsync<BaseEntity>("sp_CheckDuplicate", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public DataTable ValidateCBSRef(string connection, string cbsref, decimal expectedAmount, string dclntName, string accredit, string schema, string date,string currency)
        {
            DataTable dt = new DataTable();

            string sql = "Select * from " + schema + ".Orion_OBR_Txn where Reference=" + cbsref + " and Amount=" + expectedAmount + " and ValueDate='" + date + "' and Devise='" + currency + "' and ACC_CREDIT in (" + accredit+")";
            try
            {
                using (OracleConnection scon = new OracleConnection(connection))
                {
                    scon.Open();
                    OracleDataAdapter da = new OracleDataAdapter(sql, scon);
                    da.Fill(dt);
                    scon.Close();
                    return dt;
                };

            }
            catch (Exception ex)
            {
                return dt;
            }
        }

    }
}
