using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ReportController : BaseController
    {
        public IJsReportMVCService JsRService { get; }
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;

        public ReportController(IOptions<AppConfig> appSett, IJsReportMVCService jsReportMVCService)
        {
            JsRService = jsReportMVCService;
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
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
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Tax Receipt No" + reportData.ReceiptNo + ": Company Code " + reportData.CompanyCode + "/" + reportData.CompanyName + " Declarant Code " + reportData.DeclarantCode + "/" + reportData.DeclarantName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        return View("NoData", r);
                    }
                    string words = NumberUtil.ToWords(Math.Round(reportData.Amount, 0).ToString());
                    words = words.Replace("Un Cent", "Cent");
                    words = words.Replace("Un Mille", "Mille");

                    reportData.AmountWords = words;
                    return View(reportData);
                }
                if (reportData.StatusCode == 7)
                {
                    r.CustomMessage = "Notification response from OBR failed! \n" + reportData.StatusMsg;
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
        public async Task<IActionResult> TokenReceipt(int code = 0)
        {
            try
            {
                ReceiptReportModels noDataModel = new ReceiptReportModels();
                TokenReportModels r = new TokenReportModels();
                var reportData = await bl.GetPowerPaymentReceipt(code);
                if (reportData.StatusCode == 0)
                {
                    noDataModel.CustomMessage = "Waiting for response from Payway";
                    return View("NoData", noDataModel);
                }
                if (reportData.StatusCode == 5)
                {
                    noDataModel.CustomMessage = "Waiting for response from Payway";
                    return View("NoData", noDataModel);
                }
                if (reportData.StatusCode == 1)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Cash Power ReceiptNo " + reportData.ReceiptNo + ": Customer " + reportData.ContactInfoNo + "/" + reportData.CustomerName + " Recieved from " + reportData.ReceivedFrom, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        return View("NoData", noDataModel);
                    }
                    if (reportData.Mode == 0 || reportData.Mode == 3)
                    {
                        reportData.AccountDebit = "";
                    }

                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.ReceivedAmount, 0).ToString());
                    return View(reportData);
                }
                if (reportData.StatusCode == 7)
                {
                    noDataModel.CustomMessage = "Notification response from Payway failed!";
                    return View("NoData", noDataModel);
                }
                noDataModel.CustomMessage = "";
                return View("NoData", noDataModel);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "TokenReport.Receipt()", ex);
                return View("Error", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TopUpReceipt(int code = 0)
        {
            try
            {
                ReceiptReportModels noDataModel = new ReceiptReportModels();
                TopUpReportModels r = new TopUpReportModels();
                var reportData = await bl.GetTopUpPaymentReceipt(code);

                if (reportData.StatusCode == 0)
                {
                    noDataModel.CustomMessage = "Waiting for response from Payway";
                    return View("NoData", noDataModel);
                }
                if (reportData.StatusCode == 5)
                {
                    noDataModel.CustomMessage = "Waiting for response from Payway";
                    return View("NoData", noDataModel);
                }
                if (reportData.StatusCode == 1)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Cash Power Receipt No" + reportData.ReceiptNo + ": Customer " + reportData.CustomerNo + "/" + reportData.RetailerName + " Received From " + reportData.ReceivedFrom, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        return View("NoData", noDataModel);
                    }
                    if (reportData.Mode == 0 || reportData.Mode == 3)
                    {
                        reportData.AccountDebit = "";
                    }
                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.Amount, 0).ToString());
                    return View(reportData);
                }
                if (reportData.StatusCode == 7)
                {
                    noDataModel.CustomMessage = "Notification response from Payway failed!";
                    return View("NoData", noDataModel);
                }
                noDataModel.CustomMessage = "";
                return View("NoData", noDataModel);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "TopUpReport.Receipt()", ex);
                return View("Error", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Main()
        {
            LoadParams();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DomesticTaxReceipt(int code = 0)
        {
            try
            {
                ReceiptReportModels noDataModel = new ReceiptReportModels();
                DomesticReportModels r = new DomesticReportModels();
                var reportData = await bl.GetDomesticPaymentReceipt(code);
                if (reportData.StatusCode == 0)
                {
                    noDataModel.CustomMessage = "Waiting for response from Payway";
                    return View("NoData", noDataModel);
                }
                if (reportData.StatusCode == 5)
                {
                    noDataModel.CustomMessage = "Waiting for response from Payway";
                    return View("NoData", noDataModel);
                }
                if (reportData.StatusCode == 3)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Domestic Tax ReceiptNo " + reportData.ReferenceNo + "/" + reportData.CustomerName + " Recieved from " + reportData.ReceivedFrom, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        noDataModel.CustomMessage = "Report data is empty";
                        return View("NoData", noDataModel);
                    }

                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.Amount, 0).ToString());
                    return View(reportData);
                }
                if (reportData.StatusCode == 7)
                {
                    noDataModel.CustomMessage = "Notification response from Payway failed!";
                    return View("NoData", noDataModel);
                }

                noDataModel.CustomMessage = "Failed!";
                return View("NoData", noDataModel);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "DomesticTaxReport.Receipt()", ex);
                return View("Error", ex);
            }
        }

        [HttpGet]
        //[MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> MiarieTaxReceipt(int code = 0)
        {
            try
            {
                MiarieReportModels r = new MiarieReportModels();
                var reportData = await bl.GetMiariePaymentReceipt(code);
                if (reportData.StatusCode == 0)
                {
                    r.CustomMessage = "Waiting for response from Miarie";
                    return View("NoData", r);
                }
                if (reportData.StatusCode == 5)
                {
                    r.CustomMessage = "Waiting for response from Miarie";
                    return View("NoData", r);
                }
                if (reportData.StatusCode == 1)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Miarie Tax ReceiptNo " + reportData.ReferenceNo + "/" + reportData.PayerName + " Recieved from " + reportData.ReceivedFrom, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        return View("NoData", r);
                    }

                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.ReceivedAmount, 0).ToString());
                    return View(reportData);
                }
                if (reportData.StatusCode == 7)
                {
                    r.CustomMessage = "Notification response from Payway failed!";
                    return View("NoData", r);
                }
                r.CustomMessage = "";
                return View("NoData", r);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "MiarieTaxReport.Receipt()", ex);
                return View("Error", ex);
            }
        }

        [HttpGet]
        //[MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> PrePayReceipt(int code = 0)
        {
            try
            {
                BuyTokenReportModels r = new BuyTokenReportModels();
                var reportData = await bl.GetPrePayReceipt(code);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate PrePayReceipt ReceiptNo " + reportData.ReceivedCode + "/" + reportData.CustName + " Recieved from " + reportData.CustName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        return View("NoData", r);
                    }

                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.Amount, 0).ToString());
                    return View(reportData);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "PrePayReceipt.Receipt()", ex);
                return View("Error", ex);
            }
        }
        [HttpGet]
        //[MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<IActionResult> PostPayReceipt(string code)
        {
            try
            {
                PostPayReportModels r = new PostPayReportModels();
                var reportData = await bl.GetPostPayReceipt(code);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Generate PostPayReceipt ReceiptNo " + reportData.InvoiceNo + "/" + reportData.ClientName + " Recieved from " + reportData.ClientName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    string title = SessionUserData.Title + " - BITPay";
                    var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                    var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });

                    if (reportData == null)
                    {
                        return View("NoData", r);
                    }

                    reportData.AmountWords = NumberUtil.ToWords(Math.Round(reportData.PaidAmount, 0).ToString());
                    return View(reportData);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "PostPayReceipt.Receipt()", ex);
                return View("Error", ex);
            }
        }

        [HttpPost]
        //[AccessPermission(15, PermissionLevel.View)]
        public async Task<IActionResult> GenerateReport(ReportGenModel model)
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
        public async Task<IActionResult> Generate(ReportGenModel model)
        {
            try
            {

                var reportData = await bl.ProcessReportData(model);
                Audit.AuditAction(_appSett, GetUserBrowser(), "Generate Report No" + model.ReportCode + "Report Type" + model.ReportType, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                string title = SessionUserData.Title + " - BITPay";
                var header = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportHeader", new ReportHeader { Title = title });
                var footer = await JsRService.RenderViewToStringAsync(HttpContext, RouteData, "_ReportFooter", new { });
                var contentDisposition = "attachment; " + "filename=\"" + reportData.ReportName + ".pdf\"";
                if (model.ReportType == 0)
                {
                    HttpContext.JsReportFeature().Recipe(Recipe.PhantomPdf).Configure((x) =>
                    {
                        x.Template.Phantom = new Phantom
                        {
                            Header = header,
                            Footer = footer,
                            FooterHeight = "80px"
                        };
                    }).OnAfterRender((r) => HttpContext.Response.Headers["Content-Disposition"] = contentDisposition);
                }
                if (model.ReportType == 200)
                {
                    HttpContext.JsReportFeature().Recipe(Recipe.HtmlToXlsx).OnAfterRender((r) => HttpContext.Response.Headers["Content-Disposition"] = contentDisposition);
                }
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