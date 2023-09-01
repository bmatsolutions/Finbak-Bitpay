using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BITPay.Controllers;
using BITPay.DBL;
using BITPay.DBL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BITPay.Areas.ReportViewer.Controllers
{
    [Area("ReportViewer")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin,reportviewer")]
    public class NewReportController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;

        public NewReportController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public async Task<IActionResult> Main()
        {
            var report = new ReportFilterModel { };
            var reports = await bl.GetReportsAsync();
            report.Reports = reports.Select(x => new SelectListItem
            {
                Text = x.ReportTitle,
                Value = x.ReportCode.ToString()
            }).ToList();

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> Filters(int code = 0)
        {
            var filter = await bl.GetReportFiltersAsync(code);
            return PartialView("_Filters", filter);
        }

        [HttpPost, HttpGet]
        public async Task<IActionResult> Generate(ReportFilterModel model, int typ = 0)
        {
            var result = new CreateFileResultModel();
            try
            {
                result = await bl.GenerateReportAsync(model, typ);
                if (result.Successful)
                {
                    return File(result.FileData, result.ContentType, result.DownloadName);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Report.Generate()", ex);
                result.Message = "Report generation failed due to an error!";
            }

            return View(result);
        }
        [HttpPost, HttpGet]
        public async Task<IActionResult> GenerateBuyToken(ReportFilterModel model, int typ = 0)
        {
            var result = new CreateFileResultModel();
            try
            {
                result = await bl.GenerateReportAsync(model, typ);
                if (result.Successful)
                {
                    return File(result.FileData, result.ContentType, result.DownloadName);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Report.Generate()", ex);
                result.Message = "Report generation failed due to an error!";
            }

            return View(result);
        }
        [HttpPost, HttpGet]
        public async Task<IActionResult> GeneratePayBill(ReportFilterModel model, int typ = 0)
        {
            var result = new CreateFileResultModel();
            try
            {
                result = await bl.GenerateReportAsync(model, typ);
                if (result.Successful)
                {
                    return File(result.FileData, result.ContentType, result.DownloadName);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Report.Generate()", ex);
                result.Message = "Report generation failed due to an error!";
            }

            return View(result);
        }
    }
}
