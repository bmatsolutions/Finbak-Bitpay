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
using Newtonsoft.Json;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "maker,checker")]
    public class RegidesoPrePayController : BaseController
    {
        private Bl bl;
        private string logFile;
        public RegidesoPrePayController(IOptions<AppConfig> appSett)
        {
            bl = new Bl(appSett.Value);
            logFile = appSett.Value.LogFile;
        }


        #region Prepay
        [HttpGet]
        public IActionResult PrePay()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> QueryPrePay()
        {
            ErrorModel errorModel = new ErrorModel();
            PrePaidModel model = new PrePaidModel();
            try
            {
                var lists = await bl.GetListModel(DBL.Enums.ListModelType.PaymentModes);
                var list = lists.Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Value,
                }).ToList();
                model.PaymentModes = list;
                return PartialView("_PrePayQuery",model);
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "RegidesoPrePay.QueryPrePay", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> PayBillList(int stat = 1)
        {
            var model = new List<PostPayReportModels>();
            try
            {
                var data = await bl.GetPayBillListPayments(stat);
                return View(data);

            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Regideso.PrePayList", ex);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> query(PrePaidModel model)
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
                model.PaymentModes = list;
                var queryResult = await bl.QueryCustDetAsync(model);
                if (queryResult.Success)
                {
                    model.Cust_Code = queryResult.Cust_Code;
                    model.Cust_Name = queryResult.Cust_Name;
                    return PartialView("_PaymentDet", model);
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
                LogUtil.Error(logFile, "Regideso.QueryBills", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(PrePaidModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                if (string.IsNullOrEmpty(model.ReceivedFrom))
                {
                    errorModel.ErrorMessage = "Received from cannot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                var lists = await bl.GetListModel(DBL.Enums.ListModelType.PaymentModes);
                var list = lists.Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Value,
                }).ToList();
                var selectedItem = list.Find(option => option.Value == model.PayMode.ToString());
                model.PayModeName = selectedItem.Text;
                model.Maker = SessionUserData.UserCode;
                var queryResult = await bl.CreatePrePaymentAsync(model);
                model.BillCode = queryResult.Data20;
                if (queryResult.RespStatus == 0)
                {
                    return PartialView("_Confirm", model);
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
        public async Task<IActionResult> GetToken(decimal amount,string meterno)
        {
                var queryResult = await bl.QueryTokenAsync(amount,meterno);
                return Json(queryResult);
        }
        [HttpPost]
        public async Task<IActionResult> GetAmount(int units, string meterno)
        {
            var queryResult = await bl.QueryAmountAsync(units, meterno);
            return Json(queryResult);
        }
        [HttpPost] 
        public async Task<IActionResult> MakePayment(PrePaidModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                model.Maker = SessionUserData.UserCode;
                var queryResult = await bl.BuyTokenAsync(model);
                if (queryResult.RespStatus == 0)
                {
                    model.Token3 = queryResult.Data1;
                    model.units = queryResult.Data22;
                    model.NeedApproval = queryResult.Data21;
                    model.Msg = queryResult.RespMessage;
                    model.BillCode = queryResult.Data20;
                    model.units = queryResult.Data19;
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

        [HttpGet]
        public async Task<IActionResult> GetCharge(int payMode = 0)
        {
            var data = await bl.GetCharge(payMode);
            return Json(data.Data1);
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await bl.GetListModel(DBL.Enums.ListModelType.Bank);
            return Json(banks);
        }

        #endregion




    }
}