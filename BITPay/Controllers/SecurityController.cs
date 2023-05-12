using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Consts;
using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
    public class SecurityController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public SecurityController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await bl.GetUsers();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Users", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> SystemSettings()
        {
            var data = await bl.GetSysSettings();
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get System Parameters", 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Audits(string assesNo = "", string dateRange = "")
        {
            var data = await bl.GetAudit(SessionUserData.UserCode, assesNo, dateRange);
            return View(data);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            LoadCreateUserList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bl.CreateUser(model, SessionUserData.UserCode);
                    if (result.RespStatus == 0)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Create User " + model.FullNames, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        Success("User created successfully.");
                        return RedirectToAction("users");
                    }
                    else
                    {
                        if (result.RespStatus == 1)
                        {
                            Danger(result.RespMessage);
                        }
                        else
                        {
                            LogUtil.Error(logFile, "Security.CreateUser", result.RespMessage);
                            Danger("Request failed due to a database error!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(logFile, "Security.CreateUser", ex);
                    Danger("Request failed due to an error!");
                }
            }

            LoadCreateUserList(model.UserRole, model.BranchCode);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUser(int code = 0)
        {
            var user = await bl.GetUser(code);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage User " + code, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ManageSettings(int code = 0)
        {
            var user = await bl.GetSystemSett(code);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Manage System Parameters " + code, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPass(int code = 0)
        {
            try
            {
                var result = await bl.ResetUserPassword(code);
                if (result.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Reset User Password " + code, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success("Password reset was successfully. New password is: <strong>" + result.RespMessage + "</strong>");
                }
                else
                {
                    if (result.RespStatus == 1)
                    {
                        Danger(result.RespMessage);
                    }
                    else
                    {
                        LogUtil.Error(logFile, "Security.ResetPass", result.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Security.ResetPass", ex);
                Danger("Request failed due to an error!");
            }

            return RedirectToAction("manageuser", new { code = code });
        }

        [HttpGet]
        public async Task<IActionResult> changeDetails(int code = 0)
        {
            try
            {
                var result = await bl.Get_User(code);
                LoadCreateUserList();
                return View(result);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Security.ResetPass", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("manageuser", new { code = code });
        }

        [HttpGet]
        public async Task<IActionResult> changeSettingDetails(int code = 0)
        {
            try
            {
                var result = await bl.Get_SystemSett(code);
                //LoadCreateUserList();
                return View(result);
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Security.ResetPass", ex);
                Danger("Request failed due to an error!");
            }
            return RedirectToAction("manageuser", new { code = code });
        }

        [HttpPost]
        public async Task<IActionResult> changeSettingDetails(vwSystemSett model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bl.UpdateSystemSett(model, SessionUserData.UserCode);
                    if (result.RespStatus == 0)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Change System Params  " + model.ItemName, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        Success(result.RespMessage);
                        return RedirectToAction("systemsettings");
                    }
                    else
                    {
                        if (result.RespStatus == 1)
                        {
                            Danger(result.RespMessage);
                        }
                        else
                        {
                            LogUtil.Error(logFile, "Security.CreateUser", result.RespMessage);
                            Danger("Request failed due to a database error!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(logFile, "Security.UpdateUser", ex);
                    Danger("Request failed due to an error!");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> changeDetails(vwUser model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bl.UpdateUser(model, SessionUserData.UserCode);
                    if (result.RespStatus == 0)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Change User Details  " + model.UserCode + " " + model.FullNames, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        Success("User Updated successfully.");
                        return RedirectToAction("users");
                    }
                    else
                    {
                        if (result.RespStatus == 1)
                        {
                            Danger(result.RespMessage);
                        }
                        else
                        {
                            LogUtil.Error(logFile, "Security.CreateUser", result.RespMessage);
                            Danger("Request failed due to a database error!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(logFile, "Security.UpdateUser", ex);
                    Danger("Request failed due to an error!");
                }
            }

            LoadCreateUserList(model.UserRole, model.BranchCode);
            return View(model);
        }

        public async Task<IActionResult> BlockUser(int code = 0)
        {
            try
            {
                int mode = 0;
                var result = await bl.BlockUser(code, SessionUserData.UserCode, mode);
                if (result.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Block User " + code, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success(result.RespMessage);
                }
                else
                {
                    if (result.RespStatus == 1)
                    {
                        Danger(result.RespMessage);
                    }
                    else
                    {
                        LogUtil.Error(logFile, "Security.BlockUser", result.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Security.BlockUser", ex);
                Danger("Request failed due to an error!");
            }

            return RedirectToAction("manageuser", new { code = code });
        }

        public async Task<IActionResult> UnBlockUser(int code = 0)
        {
            try
            {
                int mode = 1;
                var result = await bl.BlockUser(code, SessionUserData.UserCode, mode);
                if (result.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Unblock User " + code, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success(result.RespMessage);
                }
                else
                {
                    if (result.RespStatus == 1)
                    {
                        Danger(result.RespMessage);
                    }
                    else
                    {
                        LogUtil.Error(logFile, "Security.BlockUser", result.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Security.BlockUser", ex);
                Danger("Request failed due to an error!");
            }

            return RedirectToAction("manageuser", new { code = code });
        }

        [HttpPost]
        public async Task<IActionResult> SetCashAccount(string cash_Account, int userCode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bl.SetUserCashAccount(userCode, cash_Account);
                    if (result.RespStatus == 0)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Set User Cash Account " + userCode, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        Success("Account updated successfully.");
                    }
                    else
                    {
                        if (result.RespStatus == 1)
                        {
                            Danger(result.RespMessage);
                        }
                        else
                        {
                            LogUtil.Error(logFile, "Security.SetCashAccount", result.RespMessage);
                            Danger("Request failed due to a database error!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(logFile, "Security.SetCashAccount", ex);
                    Danger("Request failed due to an error!");
                }
            }

            return RedirectToAction("manageuser", new { code = userCode });
        }

        private void LoadCreateUserList(int role = 0, int branch = 0)
        {
            var list = ConstData.GetUserRoles().Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value,
                Selected = x.Value == role.ToString()
            }).ToList();
            ViewData["Roles"] = list;

            list = bl.GetListModel(DBL.Enums.ListModelType.Branch).Result.Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value,
                Selected = x.Value == branch.ToString()
            }).ToList();
            ViewData["Branches"] = list;
        }
    }
}