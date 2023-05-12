using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BITPay.Controllers
{  
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "maker,checker,admin")]
    public class RefController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public RefController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public async Task< IActionResult> PaymentModes()
        {
            var data = await bl.GetPaymentModes();
            Audit.AuditAction(_appSett, GetUserBrowser(), "View Payment Modes", 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Branches()
        {
            var data = await bl.GetBranches();
            Audit.AuditAction(_appSett, GetUserBrowser(), "View Bank Branches", 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public IActionResult Branch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Branch(Branch model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bl.CreateBranch(model, SessionUserData.UserCode);
                    if (result.RespStatus == 0)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Create Branch " +model.BranchCode+" "+model.BranchName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        Success("Branch created successfully.");
                        return RedirectToAction("branches");
                    }
                    else
                    {
                        if (result.RespStatus == 1)
                        {
                            Danger(result.RespMessage);
                        }
                        else
                        {
                            LogUtil.Error(logFile, "Ref.Branch", result.RespMessage);
                            Danger("Request failed due to a database error!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(logFile, "Ref.Branch", ex);
                    Danger("Request failed due to an error!");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Banks()
        {
            var data = await bl.GetBanks();
            Audit.AuditAction(_appSett, GetUserBrowser(), "View Banks", 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Bank(int code=0)
        {
            Bank b = new Bank();          
            if (code != 0)
            {
                b =await bl.GetBank(code);
                Audit.AuditAction(_appSett, GetUserBrowser(), "View Bank Details "+b.BankCode+" "+b.BankName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                b.mode = 1;
            }
            return View(b);
        }

        [HttpPost]
        public async Task<IActionResult> Bank(Bank model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bl.CreateBank(model, SessionUserData.UserCode);
                    if (result.RespStatus == 0)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Add New Bank "+model.BankCode+" "+model.BankName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        Success(result.RespMessage);
                        return RedirectToAction("banks");
                    }
                    else
                    {
                        if (result.RespStatus == 1)
                        {
                            Danger(result.RespMessage);
                        }
                        else
                        {
                            LogUtil.Error(logFile, "Ref.Bank", result.RespMessage);
                            Danger("Request failed due to a database error!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(logFile, "Ref.Banks", ex);
                    Danger("Request failed due to an error!");
                }
            }

            return View(model);
        }
    }
}