using System;
using System.Collections.Generic;
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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin,checker")]
    public class SupervisorController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public SupervisorController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public async Task<IActionResult> List(int typ = 0)
        {
            var data = await bl.GetPaymentsApprovalQueue(typ);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Tax Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> TaxPayments(int taxType = 0)
        {
            var data = await bl.GetPaymentsApprovalQueue(taxType);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Tax Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());

            LoadTaxTypes(taxType);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> PowerList()
        {
            var data = await bl.GetPowerPaymentsApprovalQueue();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get CashPower Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }
        #region regideso
        [HttpGet]
        public async Task<IActionResult> RegidesoPostPay(int stat = 0)
        {
            var data = await bl.GetPayBillApprovalList(stat);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Domestic Tax Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> RegidesoPrePay(int stat = 0)
        {
            var data = await bl.GetPrePayApprovalList(stat);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Domestic Tax Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }
        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> ManagePostPay(int code = 0)
        {
            var item = await bl.GetPostPay(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("RegidesoPostPay");
            }
            return View(item);
        }
        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> ManagePrePay(int code = 0)
        {
            var item = await bl.GetPrePay(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("RegidesoPrePay");
            }
            return View(item);
        }
        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> SupervisePostPay(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("Manage", new { code = model.Code });
                }

                var result = await bl.ApprovePostPayAsync(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);

                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    model.MyAction = 1;
                    model.Title = "Approval Failed.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Failed Approve PostPayment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("ManagePostPay", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = result.RespMessage });
                }
                else if (result.RespStatus == 0)
                {
                    model.MyAction = 0;
                    model.Title = "Approval Successful.";
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Successful Post Payment Approval " + model.Code, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("RegidesoPostPay");
                }
                else if (result.RespStatus == 2)
                {
                    model.MyAction = 2;
                    model.Title = result.RespMessage;
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Error in Approve Post Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("ManagePostPay", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else
                {
                    LogUtil.Error(logFile, "SupervisePostPay.Supervise", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "SupervisePostPay.Supervise", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("DomesticTaxManage", new { code = model.Code });
        }
        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> SupervisePrePay(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("Manage", new { code = model.Code });
                }

                var result = await bl.ApproveBuyTokenAsync(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);

                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    model.MyAction = 1;
                    model.Title = "Approval Failed.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Failed Approve Buy Token Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("ManagePrePay", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = result.RespMessage });
                }
                else if (result.RespStatus == 0)
                {
                    model.MyAction = 0;
                    model.Title = "Approval Successful.";
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Successful Approve Buy Token Payment " + model.Code, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("RegidesoPrePay");
                }
                else if (result.RespStatus == 2)
                {
                    model.MyAction = 2;
                    model.Title = result.RespMessage;
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Error in Approve Buy Token Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("RegidesoPrePay", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else
                {
                    LogUtil.Error(logFile, "SupervisePrePay.Supervise", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "SuperviseDomesticTax.Supervise", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("ManagePrePay", new { code = model.Code });
        }
        #endregion


        [HttpGet]
        public async Task<IActionResult> DomesticTaxFiles()
        {
            var data = await bl.GetDomesticTaxFiles();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Domestic Tax Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> DomesticTaxManage(int code = 0)
        {
            var item = await bl.GetDomesticTaxFile(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("DomesticTaxFiles");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage Domestic Tax Payment " + item.AccountNo + " Cheque No " + item.ChequeNo + "/" + item.Amount, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }


        [HttpGet]
        public async Task<IActionResult> RetailerList()
        {
            var data = await bl.GetTopUpPaymentsApprovalQueue();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Agent TopUp Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> Manage(int code=0)
        {
            var item = await bl.GetPaymentApprovalQueueItem(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("List");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage Tax Payment " + item.AccountNo+" Cheque No "+item.ChequeNo+"/"+item.Amount, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> ManageTax(int code = 0)
        {
            var item = await bl.GetPaymentApprovalQueueItem(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("taxpayments");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage Tax Payment " + item.AccountNo + " Cheque No " + item.ChequeNo + "/" + item.Amount, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> PowerManage(int code = 0)
        {
            var item = await bl.GetPowerPaymentApprovalQueueItem(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("PowerList");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage CashPower Payment MeterNo " + item.CustomerNumber+" Customer Phone Number "+item.ContactInfo+" Account No"+item.AccountNo+" Cheque No "+item.ChequeNo+"/"+item.Amount, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> RetailerManage(int code = 0)
        {
            var item = await bl.GetRetailerTopUpPaymentApprovalQueueItem(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("PowerList");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage EValue Payment AgentNo " + item.CustomerNumber + " Customer Phone Number " + item.ContactInfo + " Account No" + item.AccountNo + " Cheque No " + item.ChequeNo + "/" + item.Amount + code, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> Supervise(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("Manage", new { code = model.Code });
                }

                var result = await bl.SupervisePaymentItem(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);
                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    model.MyAction = 1;
                    model.Title = "Approval Failed.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Failed Approve Tax Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("Approved", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = result.RespMessage });
                }
                else if (result.RespStatus == 0)
                {
                    var itemCode = model.Code;
                    if (result.Data1 == "12")
                        itemCode = Convert.ToInt32(result.Data2);

                    model.MyAction = 0;
                    model.Title = "Approval Successful.";
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Successful Approve Tax Payment " + model.Code, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("Approved", new { code = itemCode, stat = model.MyAction, title = model.Title, msg = "", typ = result.Data1 });
                }
                else if (result.RespStatus == 3)
                {
                    model.MyAction = 2;
                    model.Title = result.RespMessage;
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Error in Approve Tax Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("Approved", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else
                {
                    LogUtil.Error(logFile, "Supervisor.Supervise", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Supervisor.Supervise", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("Manage", new { code = model.Code });
        }

        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> SuperviseDomesticTax(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("Manage", new { code = model.Code });
                }

                var result = await bl.SuperviseDomesticTax(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);

                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    model.MyAction = 1;
                    model.Title = "Approval Failed.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Failed Approve Domestic Tax Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("DomesticApproved", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = result.RespMessage });
                }
                else if (result.RespStatus == 0)
                {
                    model.MyAction = 0;
                    model.Title = "Approval Successful.";
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Successful Approve Domestic Tax Payment " + model.Code, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("DomesticApproved", new { code = result.Data1, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else if (result.RespStatus == 2)
                {
                    model.MyAction = 2;
                    model.Title = result.RespMessage;
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Error in Approve Domestic Tax Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("DomesticApproved", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else
                {
                    LogUtil.Error(logFile, "SuperviseDomesticTax.Supervise", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "SuperviseDomesticTax.Supervise", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("DomesticTaxManage", new { code = model.Code });
        }



        [HttpGet]
        [Authorize(Roles = "checker")]
        public IActionResult DomesticApproved(int code = 0, int stat = 0, string msg = "", string title = "")
        {
            string message = stat == 1 ? msg : "Payments approved successfully.";
            FinishModel approvedModel = new FinishModel
            {
                Code = code,
                AutoPrint = 0,
                Status = stat,
                Message = message,
                Title = title,
                OBRMsg = ""
            };

            return View(approvedModel);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public IActionResult MiarieApproved(int code = 0, int stat = 0, string msg = "", string title = "")
        {
            string message = stat == 1 ? msg : "Payments approved successfully.";
            FinishModel approvedModel = new FinishModel
            {
                Code = code,
                AutoPrint = 0,
                Status = stat,
                Message = message,
                Title = title,
                OBRMsg = ""
            };

            return View(approvedModel);
        }

        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> BuyTokenSupervise(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("Manage", new { code = model.Code });
                }

                var result = await bl.BuyTokenSupervisePaymentItem(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);
                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    model.MyAction = 1;
                    model.Title = "Approval Failed.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Failed Approve CashPower Payment " + model.Code+ " Reason "+result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("TokenApproval", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = result.RespMessage });
                }
                else if (result.RespStatus == 0)
                {
                    model.MyAction = 0;
                    model.Title = "Approval Successful.";
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Successful Approve CashPower Payment " + model.Code, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("TokenApproval", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else if (result.RespStatus == 2)
                {
                    model.MyAction = 2;
                    model.Title = result.RespMessage;
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(),"Error in Approve CashPower Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("TokenApproval", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else
                {
                    LogUtil.Error(logFile, "Supervisor.BuyTokenSupervise", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Supervisor.BuyTokenSupervise", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("Manage", new { code = model.Code });
        }

        [HttpPost]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> RetailerTopUpSupervise(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("Manage", new { code = model.Code });
                }

                var result = await bl.RetailerTopUpSupervisePaymentItem(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);
                if (result.RespStatus == 1)
                {
                    Danger(result.RespMessage);
                    model.MyAction = 1;
                    model.Title = "Approval Failed.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Failed Approve Agent TopUp Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("TopUpApproval", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = result.RespMessage });
                }
                else if (result.RespStatus == 0)
                {
                    model.MyAction = 0;
                    model.Title = "Approval Successful.";
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Successful Approve Agent TopUp Payment " + model.Code, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success(result.RespMessage);
                    return RedirectToAction("TopUpApproval", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else if (result.RespStatus == 2)
                {
                    model.MyAction = 2;
                    model.Title = result.RespMessage;
                    Success(result.RespMessage);
                    Audit.AuditAction(_appSett, GetUserBrowser(),"Error in  Approve Agent TopUp Payment " + model.Code + " Reason " + result.RespMessage, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return RedirectToAction("TopUpApproval", new { code = model.Code, stat = model.MyAction, title = model.Title, msg = "" });
                }
                else
                {
                    LogUtil.Error(logFile, "Supervisor.RetailerTopUpSupervise", result.RespMessage);
                    Danger("Request failed due to a database error!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Supervisor.RetailerTopUpSupervise", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("RetailerManage", new { code = model.Code });
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public IActionResult Approved(int code = 0, int stat = 0,string msg="", string title = "", int typ=10)
        {
            string message = stat == 1 ? msg : "Payments approved successfully.";
            FinishModel approvedModel = new FinishModel
            {
                Code = code,
                AutoPrint = 0,
                Status = stat,
                Message = message,
                Title = title,
                OBRMsg = "",
                TaxType = typ
            };

            return View(approvedModel);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public IActionResult TokenApproval(int code = 0, int stat = 0, string msg = "", string title = "")
        {
            string message = stat == 1 ? msg : "Payments approved successfully.";
            FinishModel approvedModel = new FinishModel
            {
                Code = code,
                AutoPrint = 0,
                Status = stat,
                Message = message,
                Title = title,
                OBRMsg = ""
            };

            return View(approvedModel);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public IActionResult TopUpApproval(int code = 0, int stat = 0, string msg = "", string title = "")
        {
            string message = stat == 1 ? msg : "Payments approved successfully.";
            FinishModel approvedModel = new FinishModel
            {
                Code = code,
                AutoPrint = 0,
                Status = stat,
                Message = message,
                Title = title,
                OBRMsg = ""
            };

            return View(approvedModel);
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Admins()
        {
            var data = await bl.GetApprovalItems();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Admin Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Params()
        {
            var data = await bl.GetParamItems();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Admin Approval List", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageParams(int id = 0)
        {
            var item = await bl.GetApprovalParams(id);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("Params");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage Admins Approval Item " + item.idxno + " Created On " + item.Date + " Details " + item.Descr, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageItem(int code = 0)
        {
            var item = await bl.GetApprovalItem(code);
            if (item == null)
            {
                Danger("Unable to retrieve the record!");
                return RedirectToAction("Admins");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage Admins Approval Item " + item.ItemCode+ " Created On "+item.CreateDate+" Details "+item.Details, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SuperviseParams(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("ManageParams", new { id = model.Code });
                }

                var result = await bl.ParamsApprovalAction(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);
                if (result.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Approve Item  " + model.Code + " Action " + model.MyAction + " Reason " + model.Reason, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success("Action completed succesfully.");
                    return RedirectToAction("Params");
                }
                else
                {
                    if (result.RespStatus == 1)
                    {
                        Danger(result.RespMessage);
                        Audit.AuditAction(_appSett, GetUserBrowser(), result.RespMessage + " Approve Item " + model.Code, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    }
                    else
                    {
                        LogUtil.Error(logFile, "Supervisor.SuperviseItem", result.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Supervisor.SuperviseItem", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("ManageParams", new { id = model.Code });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SuperviseItem(SuperviseModel model)
        {
            try
            {
                if (model.MyAction == 0 && string.IsNullOrEmpty(model.Reason))
                {
                    Danger("You have not entered the reason for rejecting this item!");
                    return RedirectToAction("ManageItem", new { id = model.Code });
                }

                var result = await bl.ItemApprovalAction(model.Code, model.MyAction, model.Reason, SessionUserData.UserCode);
                if (result.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Approve Item  " + model.Code + " Action " + model.MyAction + " Reason " + model.Reason, 4, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success("Action completed succesfully.");
                    return RedirectToAction("Admins");
                }
                else
                {
                    if (result.RespStatus == 1)
                    {
                        Danger(result.RespMessage);
                        Audit.AuditAction(_appSett, GetUserBrowser(), result.RespMessage+ " Approve Item " + model.Code, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    }
                    else
                    {
                        LogUtil.Error(logFile, "Supervisor.SuperviseItem", result.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Supervisor.SuperviseItem", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("ManageItem", new { id = model.Code });
        }

        [HttpGet]
        public async Task<IActionResult> FailedTransactions(string dateRange = "")
        {
            var data = await bl.GetFailedTransactions(SessionUserData.UserCode, dateRange);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Failed Tax Payments " + dateRange, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "checker")]
        public async Task<IActionResult> PushFailed(int code = 0)
        {
            try 
            { 

                 var data = await bl.UpdateFailedTransactions(SessionUserData.UserCode, code);
         
                if (data.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Get Push Failed Tax Payments " + code, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success("Action completed succesfully.");
                    return RedirectToAction("FailedTransactions");
                }
                else
                {
                    if (data.RespStatus == 1)
                    {
                        Danger(data.RespMessage);
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Push Failed Tax Payments " + code, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    }
                    else
                    {
                        LogUtil.Error(logFile, "Supervisor.PushFailedAsync", data.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Supervisor.SuperviseItem", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("FailedTransactions");

        }

        private void LoadTaxTypes(int type = 0)
        {
            //---- To be loaded from db
            var types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "ALL", Value = "0", Selected = type.ToString() == "0" });
            types.Add(new SelectListItem { Text = "OBR CUSTOM", Value = "10", Selected = type.ToString() == "10" });
            types.Add(new SelectListItem { Text = "OBR DOMESTIC", Value = "11", Selected = type.ToString() == "11" });
            types.Add(new SelectListItem { Text = "MIARIE", Value = "12", Selected = type.ToString() == "12" });

            ViewData["TaxTypes"] = types;
        }
    }
}