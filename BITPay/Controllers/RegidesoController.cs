using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Models;
using BITPay.Models;
using FastReport.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NPOI.POIFS.NIO;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "maker,checker")]
    public class RegidesoController : BaseController
    {
        private Bl bl;
        private string logFile;
        public RegidesoController(IOptions<AppConfig> appSett)
        {
            bl = new Bl(appSett.Value);
            logFile = appSett.Value.LogFile;
        }


        #region Postpay
        [HttpGet]
        public IActionResult PayBill1()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PayBill()
        {
            return View();
        }
        [HttpGet]
        public IActionResult QueryPostPay()
        {
            return PartialView("_QueryBill");
        }
        


        [HttpGet]
        public async Task<IActionResult> PrePayList(int stat= 1, string assesNo = "", string dateRange = "")
        {
            var model = new List<BuyTokenReportModels>();
            try
            {
                var data = await bl.GetPrePayListPaymentsAsync(stat, SessionUserData.UserCode,assesNo,dateRange);
                return View(data);

            }
            catch(Exception ex) {
                LogUtil.Error(logFile, "Regideso.PrePayList", ex);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> FailedPayBill(int type = 2, int stat = 4, string assesNo = "", string dateRange = "")
        {
            var model = new List<PostPayReportModels>();
            try
            {
                int code = SessionUserData.UserCode;
                var data = await bl.GetPayBillApprovalList(type, stat, code,assesNo,dateRange);
                return View(data);

            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Regideso.FailedBuyToken", ex);
            }
            return View(model);
        }
          [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> ManagePostPay(int code = 0)
        {
            var item = await bl.GetPostPay(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("FailedPayBill");
            }
            return View(item);
        }
        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> ResendPostPay(SuperviseModel model)
        {
            try
            {

                var result = await bl.ResendPostPayAsync(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);

                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    return RedirectToAction("FailedPayBill");
                }
                else if (result.RespStatus == 0)
                {
                    model.Title = "Resend Payment Successful.";
                    Success(result.RespMessage);return RedirectToAction("FailedPayBill");
                }
                
                else
                {
                    LogUtil.Error(logFile, "ResendPostPay.Regideso", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "ResendPostPay.Regideso", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("FailedPayBill");
        }

        [HttpGet]
        public async Task<IActionResult> QueryBills(RegidesoModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            var modl = new RegidesoModel();
            try
            {
                var queryResult = await bl.QueryBillsAsync(model);
                if (queryResult.Success )
                {
                    if(queryResult.bills != null)
                    {
                        return PartialView("_Bills", queryResult.bills);
                    }
                    else
                    {
                        errorModel.ErrorMessage = "No Bills Within the Account No Provided";
                        return PartialView("_QueryError", errorModel);
                    }
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
                LogUtil.Error(logFile, "Regideso.QueryBills", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> BillConfirm(decimal amnt_paid,decimal amnt,string accnt_no,string cust_no,int year,int month,string invoice_no,string cust_name,string activity)
        {
            ErrorModel errorModel = new ErrorModel();
            var model = new Bills();
            try
            {
                var lists = await bl.GetListModel(DBL.Enums.ListModelType.PaymentModes);
                var list = lists.Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Value,
                }).ToList();
                model.Amnt_paid = amnt_paid;
                model.Amnt = amnt;
                model.Accnt_no = accnt_no;
                model.Cust_no = cust_no;
                model.Cust_name = cust_name;
                model.Year = year;
                model.Month = month;
                model.Invoice_no = invoice_no;
                model.Cust_name= cust_name;
                model.Activity = activity;
                model.PaymentModes = list;
                
                return PartialView("_PaymentDet", model);
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Regideso.ConfirmBillAsync", ex);
            }
            return PartialView("_QueryError", errorModel);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(Bills model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var lists = await bl.GetListModel(DBL.Enums.ListModelType.PaymentModes);
                var list = lists.Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Value,
                }).ToList();
                var selectedItem = list.Find(option => option.Value == model.PayMode.ToString());
                model.PayModeName = selectedItem.Text;
                if (string.IsNullOrEmpty(model.ReceivedFrom))
                {
                    errorModel.ErrorMessage = "Received from cannot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                if (string.IsNullOrEmpty(model.Remarks))
                {
                    errorModel.ErrorMessage = "Remarks cannot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                if (string.IsNullOrEmpty(model.PhoneNo))
                {
                    errorModel.ErrorMessage = "Phone No. cannot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                model.Maker = SessionUserData.UserCode;
                return PartialView("_Confirm1", model);
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Regideso.QueryBills", ex);
            }
            return PartialView("_QueryError", errorModel);
        }
        [HttpGet]
        public async Task<IActionResult> BillConfirmResult(Bills model)
        {
            ErrorModel errorModel = new ErrorModel();
            if (ModelState.IsValid)
            {
            try
            {
                    model.Maker = SessionUserData.UserCode;
                var queryResult = await bl.ConfrimBillsAsync(model);
                    if (queryResult.Success)
                    {
                        return PartialView("_Finish", model);
                    }
                    else
                    {
                        errorModel.ErrorMessage = queryResult.Msg;
                        return PartialView("_QueryError", errorModel);
                    }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Regideso.ConfirmBillAsync", ex);
            }
            return PartialView("_QueryError", errorModel);
            }
            else
            {
                errorModel.ErrorMessage = "Please input  phone number!";
                return PartialView("_QueryError", errorModel);
            }
                
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(Bills model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                model.Maker = SessionUserData.UserCode;
                var queryResult = await bl.PostPayAsync(model);
                if (queryResult.RespStatus == 0)
                {
                    model.NeedApproval = queryResult.Data21;
                    model.Msg = queryResult.RespMessage;
                    model.ClientName = queryResult.Data1;
                    model.Amnt_paid = queryResult.Data18;
                    model.Amnt = queryResult.Data19;
                    model.Invoice_no = queryResult.Data2;
                    model.BillCode = queryResult.Data20;
                    return PartialView("_Finish", model);
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
                LogUtil.Error(logFile, "Regideso.QueryBills", ex);
            }
            return PartialView("_QueryError", errorModel);
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
        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await bl.GetListModel(DBL.Enums.ListModelType.Bank);
            return Json(banks);
        }
        [HttpGet]
        public async Task<IActionResult> GetCharge(int payMode = 0)
        {
            var data = await bl.GetCharge(payMode);
            return Json(data.Data1);
        }

        #endregion


    }
}