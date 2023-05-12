using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public interface IReportRepository 
    {
        IEnumerable<ReportModel> GetReports();
        BaseReportModels GetReportHeader(int reportCode);
        IEnumerable<GeneralReportData> GetReportData(int reportCode, DateTime dateFrom, DateTime dateTo, int branch, int user);
        IEnumerable<GeneralReportData> GetObrReportData(int reportCode, DateTime dateFrom, DateTime dateTo, string office);

        Task<PrintReportModel> GetSettAsync(int reportCode);
        Task<DataTable> GetDataAsync(ReportFilterModel filter, int setNo);
        Task<IEnumerable<ReportParams>> GetParamsAsync(ReportFilterModel filter);
        Task<IEnumerable<ReportModel>> GetListAsync();
        Task<IEnumerable<ReportFilterItemModel>> GetFiltersAsync(int reportCode);
    }
}
