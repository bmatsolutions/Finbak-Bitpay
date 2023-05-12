using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Models;
using BITPay.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AccountController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public AccountController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var userModel = await bl.UserLogin(model.UserName, model.Password);
                if (userModel.RespStatus == 0)
                {
                    bool changePass = userModel.UserStatus == DBL.Enums.UserLoginStatus.ChangePassword;
                    SetUserLoggedIn(userModel, model.RememberMe, changePass);
                    //---- Check if change password
                    if (changePass)
                    {
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Change Password Requested " + model.UserName, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), userModel.UserCode, GetIP());
                        return RedirectToAction("ChangePass", new { returnUrl = returnUrl });
                    }
                    else
                    {
                        
                        if (userModel.UserRole == 3)
                        {
                            Audit.AuditAction(_appSett, GetUserBrowser(), "Log in successfully " + model.UserName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), userModel.UserCode, GetIP());
                            return RedirectToAction("Dashboard", "Home", new { Area = "ReportViewer" });
                        }
                        if (userModel.UserRole == 4)
                        {
                            Audit.AuditAction(_appSett, GetUserBrowser(), "Log in successfully " + model.UserName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), userModel.UserCode, GetIP());
                            return RedirectToAction("Dashboard", "Home", new { Area = "obr" });
                        }
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Log in successfully " + model.UserName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), userModel.UserCode, GetIP());
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                    ModelState.AddModelError(string.Empty, userModel.RespMessage);
            }

            return View(model);
        }

        [HttpGet]
        //[AllowAnonymous]
        [Authorize(AuthenticationSchemes = AppData.ChangePaxScheme)]
        public IActionResult ChangePass(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ChangeUserPassModel passModel = new ChangeUserPassModel { UserCode = 0 };
            return View(passModel);
        }

        [HttpPost]
        //[AllowAnonymous]
        [Authorize(AuthenticationSchemes = AppData.ChangePaxScheme)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePass(ChangeUserPassModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                model.UserCode = SessionUserData.UserCode;
                var userModel = await bl.ChangeUserPassword(model);
                if (userModel.RespStatus == 0)
                {
                    await HttpContext.SignOutAsync(AppData.ChangePaxScheme);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Changed Password successfully " + model.UserCode, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), model.UserCode, GetIP());
                    Success("Password changed successfully.");
                }
                else
                    Danger(userModel.RespMessage);
            }
            else
                Danger("Data model is NOT valid!");
            return RedirectToAction("Login", new { returnUrl = returnUrl });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async void SetUserLoggedIn(UserModel user, bool rememberMe, bool changePass)
        {
            UserDataModel serializeModel = new UserDataModel
            {
                UserId = user.Id,
                FullNames = user.FullNames,
                UserName = user.UserName,
                UserCode = user.UserCode,
                UserRole = user.UserRole,
                Title = user.Extra1,
                BankName = user.Extra2,
                Post = Convert.ToInt32(user.Extra3)
                // brachCode = user.brachCode

            };

            string userData = JsonConvert.SerializeObject(serializeModel);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.UserRoleName),
                new Claim("userData", userData),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity[] { claimsIdentity });
            string scheme = changePass ? AppData.ChangePaxScheme : CookieAuthenticationDefaults.AuthenticationScheme;
            await HttpContext.SignInAsync(scheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = new DateTimeOffset?(DateTime.UtcNow.AddMinutes(30))
            });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Dashboard), "Home", new { area = "" });
            }
        }

        #endregion
    }
}