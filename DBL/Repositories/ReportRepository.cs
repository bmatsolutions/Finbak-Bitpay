using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BITPay.DBL.Models;
using Dapper;

namespace BITPay.DBL.Repositories
{
    public class ReportRepository : BaseRepository, IReportRepository
    {
        public ReportRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<GeneralReportData> GetReportData(int reportCode, DateTime dateFrom, DateTime dateTo,int branch,int user)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ReportCode", reportCode);
                parameters.Add("@DateFrom", dateFrom);
                parameters.Add("@DateTo", dateTo);
                parameters.Add("@branch", branch);
                parameters.Add("@user", user);
                return connection.Query<GeneralReportData>("sp_GetReportData", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }
        
        public IEnumerable<GeneralReportData> GetObrReportData(int reportCode, DateTime dateFrom, DateTime dateTo,string office)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ReportCode", reportCode);
                parameters.Add("@DateFrom", dateFrom);
                parameters.Add("@DateTo", dateTo);
                parameters.Add("@office",  office);
                return connection.Query<GeneralReportData>("sp_GetObrReportData", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public BaseReportModels GetReportHeader(int reportCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ReportCode", reportCode);

                return connection.Query<BaseReportModels>("sp_GetReportHeader", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IEnumerable<ReportModel> GetReports()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<ReportModel>(GetAllStatement("Reports")).ToList();
            }
        }

        #region New Reports
        public async Task<DataTable> GetDataAsync(ReportFilterModel filter, int setNo)
        {
            using (var conn = new SqlConnection(_connString))
            {
                using (var cmd = new SqlCommand("sp_GetReportSetData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //---- Add parameters
                    cmd.Parameters.AddWithValue("ReportCode", filter.ReportCode);
                    cmd.Parameters.AddWithValue("DateFrom", filter.DateFrom);
                    cmd.Parameters.AddWithValue("DateTo", filter.DateTo);
                    cmd.Parameters.AddWithValue("DataSet", setNo);
                    cmd.Parameters.AddWithValue("Filter1", filter.Filter1);
                    cmd.Parameters.AddWithValue("Filter2", filter.Filter2);
                    cmd.Parameters.AddWithValue("Filter3", filter.Filter3);
                    cmd.Parameters.AddWithValue("Filter4", filter.Filter4);

                    await conn.OpenAsync();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    return dataTable;
                }
            }
        }

        public async Task<IEnumerable<ReportFilterItemModel>> GetFiltersAsync(int reportCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", reportCode);

                return (await connection.QueryAsync<ReportFilterItemModel>(FindStatement("ReportFilters", "ReportCode"), parameters)).ToList();
            }
        }

        public async Task<IEnumerable<ReportModel>> GetListAsync()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                string sql = "Select ReportCode, ReportTitle From Reports Where Show = 1 Order By ReportTitle";
                return (await connection.QueryAsync<ReportModel>(sql)).ToList();
            }
        }

        public async Task<IEnumerable<ReportParams>> GetParamsAsync(ReportFilterModel filter)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ReportCode", filter.ReportCode);
                parameters.Add("DateFrom", filter.DateFrom);
                parameters.Add("DateTo", filter.DateTo);
                parameters.Add("Filter1", filter.Filter1);
                parameters.Add("Filter2", filter.Filter2);
                parameters.Add("Filter3", filter.Filter3);
                parameters.Add("Filter4", filter.Filter4);
                parameters.Add("Filter5", filter.Filter5);
                parameters.Add("Filter6", filter.Filter6);

                return (await conn.QueryAsync<ReportParams>("sp_GetReportParams", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task<PrintReportModel> GetSettAsync(int reportCode)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ReportCode", reportCode);

                return (await conn.QueryAsync<PrintReportModel>("sp_GetReportSetting", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            }
        }
        #endregion
    }
}
