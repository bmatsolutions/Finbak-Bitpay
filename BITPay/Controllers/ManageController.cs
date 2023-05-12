using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "maker,checker")]
    public class ManageController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public ManageController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await bl.GetUserProfile(SessionUserData.UserCode);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get user profile " + user.UserCode +" "+user.FullNames, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(UserProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var userModel = await bl.ChangeUserPassword(model);
                if (userModel.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Change Password successfully " + model.UserCode+" "+model.FullNames, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    Success("Password changed successfully.");
                    return RedirectToAction("Profile");
                }
                else
                    Danger(userModel.RespMessage);
            }
            return View(model);
        }
    }
}