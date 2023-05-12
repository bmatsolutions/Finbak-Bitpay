using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.DBL;
using BITPay.DBL.Consts;
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
    public class DomesticController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public DomesticController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public IActionResult Pay()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SearchFile()
        {
            LoadDomesticSelections();
            return PartialView("_SearchFile");
        }


        [HttpGet]
        public IActionResult Payment()
        {
            return PartialView("_Payment");
        }

        [HttpPost]
        public async Task<IActionResult> Query(IncomeTaxDecl model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                GenericModel resp = new GenericModel();
                resp = await bl.ValidateDomesticDeclaration(model, SessionUserData.UserCode);
                if (resp.RespStatus == 1)
                {
                    Danger(resp.RespMessage, true);
                    errorModel.ErrorMessage = resp.RespMessage;
                    return PartialView("_QueryError", errorModel);
                }
                else
                {
                    var payername = "";

                    if (!string.IsNullOrEmpty(model.tin))
                    {
                        var validatetin = await bl.ValidateTin(model.tin);

                        if (validatetin.status == 1)
                        {
                            Danger(validatetin.message, true);
                            errorModel.ErrorMessage = validatetin.message;
                            return PartialView("_QueryError", errorModel);
                        }
                        payername = validatetin.content.pName;
                    }
                    else
                    {
                        payername = model.CustomName;
                    }

                    var payResult = await bl.ConfirmDomesticPayTax(model, payername, SessionUserData.UserCode);

                    string Licence = "";
                    if (model.DomesticTaxName == "721320|1")
                    {
                        Licence = model.LicenceType;
                    }
                    else if (model.DomesticTaxName == "721320|2")
                    {
                        Licence = model.LicencesType;
                    }

                    if (!string.IsNullOrEmpty(model.CarDetails))
                    {
                        model.CarChassis = model.CarDetails;
                    }

                    DomesticTaxPayment domesticTaxPayment = new DomesticTaxPayment
                    {
                        adjustment = model.AdjustMentType,
                        TaxAmount = Convert.ToDecimal(model.AmountPayable),
                        Chasis = model.CarChassis,
                        Imma = model.CarImma,
                        CarOnwer = model.CarOwner,
                        commune = model.commune,
                        Contracavation = model.Contraviction,
                        CustomerName = payername,
                        DeclNo = model.DeclNo,
                        Delay = model.DelayMajoration,
                        Document = model.Document,
                        DomesticTaxType = model.DomesticTaxType,
                        DomesticTaxName = model.DomesticTaxName,
                        DriverName = model.DriverName,
                        Education = model.EducationDoc,
                        Infraction = model.Infraction,
                        LicenceType = Licence,
                        Period = model.Period,
                        Service = model.Service,
                        TaxAdjustmentCat = model.TaxAdjustmentCat,
                        tin = model.tin,
                        Copies = model.TotCopies,
                        Vehicle = model.VehicleType,
                        Word = model.Wording,
                        ChargeAmount = Convert.ToDecimal(payResult.Data2),
                        FileCode = Convert.ToInt32(payResult.Data1),
                        PostToCBS = Convert.ToInt32(payResult.Data3),
                        TaxName = payResult.Data4,
                        PrintReceipt = true,
                        NoCharge = true,
                        Product = model.Product,
                        CNI = model.CNI,
                        CNIIssue = model.CNIIssue,
                        ManufactureYear = model.ManufactureYear,
                        BrandType = model.BrandType
                    };

                    var lists = await bl.GetListModel(DBL.Enums.ListModelType.PaymentModes);
                    var list = lists.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value,
                    }).ToList();

                    domesticTaxPayment.PaymentModes = list;
                    if (domesticTaxPayment.PostToCBS == 0)
                    {
                        return PartialView("_DomesticSearch", domesticTaxPayment);//Manual Posting
                    }

                    return PartialView("_Payment", domesticTaxPayment);
                }

            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Domestic.Query", ex);
            }
            return PartialView("_QueryError", errorModel);
        }


        [HttpPost]
        public async Task<IActionResult> MakeDomesticPayment(DomesticTaxPayment model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                if (string.IsNullOrEmpty(model.ReceivedFrom))
                {
                    errorModel.ErrorMessage = "Received from cannot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                model.ReceivedFrom = model.ReceivedFrom.ToUpper();
                if (model.NoCharge)
                {
                    model.ChargeAmount = 0.00M;
                }
                var validResult = await bl.ValidateDomesticTaxPayment(model);
                if (validResult.RespStatus == 0)
                {
                    model.PayModeName = validResult.Data1;
                    model.CrAccountName = validResult.Data2;
                    model.CustAccountName = validResult.Data3;
                    model.TaxName = validResult.Data7;
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
                        LogUtil.Error(logFile, "Domestic.MakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Domestic.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> DomesticSearch(DomesticTaxPayment model)
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
                var validResult = await bl.ValidateDomesticTaxPayment(model);
                if (validResult.RespStatus == 0)
                {
                    model.PayModeName = validResult.Data1;
                    model.CrAccountName = validResult.Data2;
                    model.CustAccountName = validResult.Data3;
                    model.TaxName = validResult.Data7;
                    model.Amount = model.TaxAmount;
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
                        LogUtil.Error(logFile, "Domestic.MakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Domestic.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(TaxPaymentModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                model.TaxType = 11;
                var payResult = await bl.MakeTaxPayment(model, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    FinishModel finishModel = new FinishModel
                    {
                        Code = Convert.ToInt32(payResult.Data1),
                        PaymentCode = Convert.ToInt32(payResult.Data4),
                        AutoPrint = Convert.ToInt32(payResult.Data2),
                        NeedApproval = payResult.Data3.Equals("1"),
                        Message = string.IsNullOrEmpty(payResult.RespMessage) ? "Domestic Tax payment completed successfully." : payResult.RespMessage,
                        OBRMsg = ""
                    };
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Confirm Domestic Tax Payment" + model.FileCode, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return PartialView("_Finish", finishModel);
                }
                else
                {
                    if (payResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = payResult.RespMessage;
                        LogUtil.Error(logFile, "Domestic.MakePayment", payResult.RespMessage);
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "Domestic.MakePayment", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Domestic.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomeTax()
        {
            var incometax = await bl.GetListModel(DBL.Enums.ListModelType.IncomeTax);
            return Json(incometax);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxCatergory(int mode)
        {
            var incometax = await bl.GetTaxCatergory(mode);
            return Json(incometax);
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



        [HttpGet]
        public async Task<IActionResult> Payments(string assesNo = "", string dateRange = "")
        {
            var data = await bl.GetDomesticTaxPayments(SessionUserData.UserCode, assesNo, dateRange);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Domestic Tax Payments " + assesNo + dateRange, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public IActionResult QueryPayment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QueryPayment(QueryPayment model)
        {
            var data = await bl.QueryPayment(model);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Query Domestic Tax Payments " + model.RefNo, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            if (data.status != 0)
            {
                ErrorModel errorModel = new ErrorModel();
                errorModel.ErrorMessage = data.message;
                return PartialView("_QueryError", errorModel);
            }
            return PartialView("_PaymentStatus", data.content);
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedPayments(int usercode = 0, string assesNo = "")
        {
            var data = await bl.ApprovedDomesticPayments(SessionUserData.UserCode, assesNo);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Approved Domestic Tax Payments " + assesNo, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Files(string dateRange = "", int fileStatus = -1, string assesNo = "")
        {
            var data = await bl.GetDomesticFiles(dateRange, fileStatus, assesNo);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Domestic Tax File " + dateRange + assesNo, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            LoadFilesList(fileStatus);
            return View(data);
        }

        private void LoadFilesList(int status = -1)
        {
            var lists = bl.GetListModel(DBL.Enums.ListModelType.FileStatus).Result;
            var list = lists.Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value,
                Selected = x.Value == status.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "ALL",
                Value = "-1"
            });

            ViewData["fileStatus"] = list;
        }

        private void LoadDomesticSelections()
        {
            var list = ConstData.GetVehicleTypes().Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            ViewData["Vehicles"] = list;

            list = ConstData.GetAnnualFees().Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            ViewData["AnnualFees"] = list;

            list = ConstData.GetTaxeValorem().Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            ViewData["Valorem"] = list;

            list = ConstData.GetCivilStat().Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            ViewData["CivilStatus"] = list;
        }
    }
}