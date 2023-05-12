using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.Controllers;
using BITPay.DBL;
using BITPay.DBL.Models;
using BITPay.Models;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BITPay.Areas.ReportViewer.Controllers
{
    [Area("ReportViewer")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin,reportviewer")]
    public class RptController : BaseController
    {
        public IJsReportMVCService JsRService { get; }
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;

        public RptController(IOptions<AppConfig> appSett, IJsReportMVCService jsReportMVCService)
        {
            JsRService = jsReportMVCService;
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        //[MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> Receipt(int code = 0)
        {
            try
            {
                ReceiptReportModels r = new ReceiptReportModels();
                var reportData = await bl.GetTaxPaymentReceipt(code);
                if (reportData.StatusCode == 0)
                {
                    r.CustomMessage = "Waiting for response from OBR";
                    return View("NoData", r);
                }
                if (reportData.StatusCode == 5)
                {
                    r.CustomMessage = "Waiting for response from OBR";
                    return View("NoData", r);
                }
                if (reportData.StatusCode == 6)
                {
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    //HttpContext.JsReportFeature().Recipe(Recipe.PhantomPdf).Configure((x) =>
                    //{
                    //    x.Template.Phantom = new Phantom
                    //    {
                    //        Header = header,
                    //        Footer = footer,
                    //        FooterHeight = "80px"
                    //    };
                    //});

                    if (reportData == null)
                    {
                        return View("NoData", r);
                    }

                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.Amount, 0).ToString());
                    return View(reportData);
                }
                if (reportData.StatusCode == 7)
                {
                    r.CustomMessage = "Notification response from OBR failed!";
                    return View("NoData", r);
                }
                r.CustomMessage = "";
                return View("NoData", r);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Report.Receipt()", ex);
                return View("Error", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Main()
        {
            // var reports = await bl.GetReports();
            LoadParams();
            return View();
        }



        [HttpPost]
        //[MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> Generate(ReportGenModel model)
        {
            try
            {
                var reportData = await bl.ProcessReportData(model);
                Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Report No" + model.ReportCode + "Report Type" + model.ReportType, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                var data = reportData.DataList;
                return View(reportData.ReportName, reportData);
            }
            catch (Exception ex)
            {
                Danger(ex.InnerException.Message);
                LogUtil.Error(logFile, "Report.Generate", ex);
                return View("Error", ex);
            }
        }

        [HttpGet]
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> TestPdf()
        {
            string title = SessionUserData.Title + " - BITPay";
            var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
            var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

            HttpContext.JsReportFeature().Recipe(Recipe.PhantomPdf).Configure((x) =>
            {
                x.Template.Phantom = new Phantom
                {
                    Header = header,
                    Footer = footer,
                    FooterHeight = "80px"
                };
            });

            return View("Test");
        }

        #region other methods
        private void LoadParams()
        {
            var list = bl.GetListModel(DBL.Enums.ListModelType.User).Result.Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            ViewData["Users"] = list;

            list = bl.GetListModel(DBL.Enums.ListModelType.Branch).Result.Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            ViewData["Branches"] = list;

            list = bl.GetReports().Result.Select(x => new SelectListItem
            {
                Text = x.ReportTitle,
                Value = x.ReportCode.ToString()
            }).ToList();
            ViewData["Reports"] = list;
        }
        #endregion
    }
}

