using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BITPay.Controllers
{
    public class DataController : BaseController
    {
        private Bl bl;
        private string logFile;
        public DataController(IOptions<AppConfig> appSett)
        {
            bl = new Bl(appSett.Value);
            logFile = appSett.Value.LogFile;
        }

        public async Task<IActionResult> DBoardData()
        {
            var data = await bl.GetDBoardData(SessionUserData.UserCode);
           // Audit.AuditAction(_appSett, GetUserBrowser(), "Get All Statisticall Values  ", 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return Json(data);
        }
    }
}