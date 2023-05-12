using System;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Models;
using BITPay.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "maker,checker")]
    public class MiarieController : BaseController
    {
        private Bl bl;
        private string logFile;
        public MiarieController(IOptions<AppConfig> appSett)
        {
            bl = new Bl(appSett.Value);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public IActionResult Pay()
        {
            return View();
        }

        [HttpGet]
        public IActionResult QueryTax()
        {
            return PartialView("_QueryTax");
        }

        [HttpPost]
        public async Task<IActionResult> Query(QueryMarie model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var queryResult = await bl.QueryMiarieTaxAsync(model);
                if (queryResult.Success)
                {
                    if (queryResult.TaxNotes.Length == 1)
                    {
                        var tax = queryResult.TaxNotes.FirstOrDefault();
                        tax.TaxNoteType = queryResult.TaxType;
                        if (tax.Amount == 0)
                            tax.Amount = tax.TaxAmount;
                        return PartialView("_TaxDet", tax);
                    }
                    return PartialView("_TaxDets", queryResult);
                }
                else
                {
                    errorModel.ErrorMessage = queryResult.RespMessage;
                    return PartialView("_QueryError", errorModel);
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Miarie.Query", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(MairieTaxNoteModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var payResult = await bl.ConfirmMiarieTaxAsync(model, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    var fileCode = Convert.ToInt32(payResult.Data1);
                    TaxPaymentModel taxPayment = new TaxPaymentModel
                    {
                        FileCode = fileCode,
                        ExpectedAmount = model.Amount,
                        Amount = model.Amount,
                        TaxRef = model.RefNo,
                        ReferenceNo = model.RefNo,
                        TaxPayerName = model.TaxPayerName,
                        TaxPeriod = model.Period,
                        PrintReceipt = true,
                        NoCharge = true,
                        ChargeAmount = Convert.ToDecimal(payResult.Data2),
                        ItemType = model.TaxNoteType,
                        Details = model.TaxNoteDescr,
                        PayPartial = Convert.ToBoolean(payResult.Data3),
                        PostToCBS = Convert.ToInt32(payResult.Data4)
                    };

                    // Audit.AuditAction(_appSett, GetUserBrowser(), "Confirm Tax Dets Payment Declarant Name " + model.DeclarantName + " Company " + model.OfficeName + " Amount " + model.AmountToBePaid + " File Code " + taxPayment.FileCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    var lists = await bl.GetListModel(DBL.Enums.ListModelType.PaymentModes);
                    var list = lists.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value,
                    }).ToList();

                    taxPayment.PaymentModes = list;
                    if (taxPayment.PostToCBS == 0)
                    {
                        return PartialView("_CBSQuery", taxPayment);
                    }

                    return PartialView("_Payment", taxPayment);
                }
                else
                {
                    if (payResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = payResult.RespMessage;
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "Miarie.Confirm", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Miarie.Confirm", ex);
            }

            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(TaxPaymentModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                if (string.IsNullOrEmpty(model.ReceivedFrom))
                {
                    errorModel.ErrorMessage = "Received from connot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                model.ReceivedFrom = model.ReceivedFrom.ToUpper();
                if (model.NoCharge)
                {
                    model.ChargeAmount = 0.00M;
                }

                model.TaxType = 12;
                var validResult = await bl.ValidateTaxPayment(model);
                if (validResult.RespStatus == 0)
                {
                    var valAmount = Convert.ToDecimal(validResult.Data5);
                    model.PayModeName = validResult.Data1;
                    model.CrAccountName = validResult.Data2;
                    model.CustAccountName = validResult.Data3;
                    model.ExpectedAmount = valAmount == 0 ? model.ExpectedAmount : valAmount;
                    // Audit.AuditAction(_appSett, GetUserBrowser(), "Validate Tax Payment File Code " + model.FileCode + " Received From " + model.ReceivedFrom + " Amount " + model.Amount + " Cheque No " + model.ChequeNo, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return PartialView("_Confirm", model);
                }
                else
                {
                    if (validResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = validResult.RespMessage;
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "Miarie.MakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Miarie.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(TaxPaymentModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                model.TaxType = 12;
                var payResult = await bl.MakeTaxPayment(model, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    FinishModel finishModel = new FinishModel
                    {
                        Code = Convert.ToInt32(payResult.Data1),
                        PaymentCode = Convert.ToInt32(payResult.Data4),
                        AutoPrint = Convert.ToInt32(payResult.Data2),
                        NeedApproval = payResult.Data3.Equals("1"),
                        Message = string.IsNullOrEmpty(payResult.RespMessage) ? "Tax payment completed successfully." :
                        payResult.RespMessage
                    };
                    // Audit.AuditAction(_appSett, GetUserBrowser(), "Confirm Tax Payment" + model.FileCode, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return PartialView("_Finish", finishModel);
                }
                else
                {
                    if (payResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = payResult.RespMessage;
                        LogUtil.Error(logFile, "Miarie.ConfirmPayment", payResult.RespMessage);
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "Miarie.ConfirmPayment", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Miarie.ConfirmPayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxNote()
        {
            var taxnote = await bl.GetListModel(DBL.Enums.ListModelType.TaxNote);
            return Json(taxnote);
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await bl.GetListModel(DBL.Enums.ListModelType.Bank);
            return Json(banks);
        }

        [HttpGet]
        public IActionResult TaxQuery()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTax(QueryMarie model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var queryResult = await bl.QueryMiarieTaxAsync(model);
                if (queryResult.Success)
                {
                    var tax = queryResult.TaxNotes.FirstOrDefault();
                    return PartialView("_TaxData", tax);
                }
                else
                {
                    errorModel.ErrorMessage = queryResult.RespMessage;
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Miarie.GetTax", ex);
            }

            return PartialView("_ErrorView", errorModel);
        }

        [HttpGet]
        public IActionResult Taxpayer()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxpayer(MairieTaxpayerQueryModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var queryResult = await bl.QueryMiarieTaxpayerAsync(model);
                if (queryResult.Success)
                {
                    return PartialView("_TaxpayerData", queryResult);
                }
                else
                {
                    errorModel.ErrorMessage = queryResult.RespMessage;
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Miarie.GetTax", ex);
            }

            return PartialView("_ErrorView", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> Approved(string dateRange = "")
        {
            var data = await bl.GetMiariePaymentsAsync(6, dateRange);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Failed(string dateRange = "")
        {
            var data = await bl.GetMiariePaymentsAsync(9, dateRange);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentStatus(int code = 0, int tcode = 0)
        {
            var data = await bl.GetPaymentStatusAsync(code);
            PaymentStatusModel statModel = new PaymentStatusModel
            {
                StatusCode = Convert.ToInt32(data.Data4),
                StatusName = data.Data5,
                Code = tcode
            };
            return PartialView("_PaymentStat", statModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxTypes(int id = 1)
        {
            var data = await bl.GetMiarieTaxTypesAsync(id);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTypeFields(int id = 1)
        {
            var data = await bl.GetMiarieTaxTypeFieldsAsync(id);
            return PartialView("_TaxTypeFields", data);
        }

        [HttpGet]
        public async Task<IActionResult> Repost(int code = 0)
        {
            try
            {
                var result = await bl.RepostMiarieTaxAsync(code, SessionUserData.UserCode);
                if (result.RespStatus == 0)
                {
                    Success("Transaction has been processed successfully.");
                }
                else
                {
                    if (result.RespStatus == 1)
                    {
                        Danger(result.RespMessage);
                    }
                    else
                    {
                        Danger("Operation failed due to a database error!");
                        LogUtil.Error(logFile, "Miarie.Repost", result.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Danger("Operation failed due to an error!");
                LogUtil.Error(logFile, "Miarie.Repost", ex);
            }
            return Json("Ok");
        }
    }
}