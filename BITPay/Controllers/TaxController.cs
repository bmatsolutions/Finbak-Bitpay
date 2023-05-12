using System;
using System.Collections.Generic;
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
    public class TaxController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public TaxController(IOptions<AppConfig> appSett)
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
            return PartialView("_SearchFile");
        }
     

        [HttpPost]
        public async Task<IActionResult> Query(DeclarationQueryData model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                GenericModel resp = new GenericModel();
                string Nif_Name = "";
                string Declarant_Name = "";
                //validate if payment for the declaration has already been made for even proceeding to query
                resp = await bl.Validate_Declaration(model, SessionUserData.UserCode);
                if (resp.RespStatus == 1)
                {
                    Danger(resp.RespMessage, true);
                    errorModel.ErrorMessage = resp.RespMessage;
                    return PartialView("_QueryError", errorModel);
                }
                else
                {
                    int Type = Convert.ToInt32(model.TaxType);
                    
                    if (!string.IsNullOrEmpty(model.RegistrationSerial))
                    {
                        model.RegistrationSerial = model.RegistrationSerial.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(model.AccountReference))
                    {
                        model.AccountReference = model.AccountReference.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(model.OfficeCode))
                    {
                        model.OfficeCode = model.OfficeCode.ToUpper();
                    }
                    var office = await bl.CheckObrOffice(model.OfficeCode);
                    if (office.RespStatus != 0)
                    {
                        Danger("Invalid Office code", true);
                        return RedirectToAction("MultiplePayments");
                    }
                    //validate NIF and Declarant if available
                    //Johnson .... validate NIF or declarant code whiever is available
                    if (!string.IsNullOrEmpty(model.CompCode))
                    {
                        var nif_Res = await bl.ValidateNIF(model.CompCode);
                        if (nif_Res.status != 0)//failed
                        {
                            errorModel.ErrorMessage = nif_Res.message;
                            return PartialView("_QueryError", errorModel);
                        }
                        Nif_Name = nif_Res.message;
                    }
                    if (!string.IsNullOrEmpty(model.DeclCode))
                    {
                        var dec_Res = await bl.ValidateDelarant(model.DeclCode);
                        if (dec_Res.status != 0)//failed
                        {
                            errorModel.ErrorMessage = dec_Res.message;
                            return PartialView("_QueryError", errorModel);
                        }
                        Declarant_Name = dec_Res.message;
                    }
                    switch (Type)
                    {
                        case 1:
                            var queryResult = await bl.QueryTax(model);
                            switch (Convert.ToInt32(queryResult.ErrorCode))
                            {
                                case 38:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Declaration Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryResult.PayType = Type;
                                    return PartialView("_TaxDets", queryResult);
                                case 34:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Declaration Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryResult.PayType = Type;
                                    return PartialView("_Paid", queryResult);
                                case 22:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Declaration Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryResult.PayType = Type;
                                    return PartialView("_NotFound", queryResult);
                                default:
                                    queryResult.PayType = Type;
                                    return PartialView("_NotKnown", queryResult);
                            }
                        case 2:
                            
                            var queryCreditResult = await bl.QueryCreditTax(model);
                            switch (Convert.ToInt32(queryCreditResult.ErrorCode))
                            {
                                case 37:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Credit Payment Tax Files " + queryCreditResult.OfficeCode + " " + queryCreditResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryCreditResult.PayType = Type;
                                    return PartialView("_TaxDets", queryCreditResult);
                                case 33:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Credit Payment Tax Files " + queryCreditResult.OfficeCode + " " + queryCreditResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryCreditResult.PayType = Type;
                                    return PartialView("_Paid", queryCreditResult);
                                case 24:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Credit Payment Tax Files " + queryCreditResult.OfficeCode + " " + queryCreditResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryCreditResult.PayType = Type;
                                    return PartialView("_NotFound", queryCreditResult);
                                default:
                                    queryCreditResult.PayType = Type;
                                    return PartialView("_NotKnown", queryCreditResult);
                            }
                        case 3:
                            var creditResult = await bl.QueryCredit(model);
                            switch (Convert.ToInt32(creditResult.ErrorCode))
                            {
                                case 0:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Credit Slip Tax Files " + creditResult.OfficeCode + " " + creditResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    creditResult.PayType = Type;
                                    return PartialView("_CreditDets", creditResult);
                                case 35:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query Credit Slip Tax Files " + creditResult.OfficeCode + " " + creditResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    creditResult.PayType = Type;
                                    return PartialView("_Paid", creditResult);
                                default:
                                    creditResult.PayType = Type;
                                    return PartialView("_NotKnown", creditResult);
                            }
                        case 4:
                            int Other = -1;
                            if (!string.IsNullOrEmpty(model.TranCode))
                            {
                                string[] Tranrefs = model.TranCode.Split("|");
                                Other = Convert.ToInt32(Tranrefs[0]);
                                model.TranCode = Tranrefs[1];
                            }
                         
                            switch (Other)// 0 doesn't require decl 1 requires decl
                            {
                                case 0:
                                    string Nif = model.CompCode;
                                    if (!string.IsNullOrEmpty(Nif))
                                    {
                                        DeclarationQueryResponse queryResp = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            CompanyCode = model.CompCode,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency=model.Currencies,
                                            PayType = Type,
                                            DeclarantName=Declarant_Name,
                                            CompanyName=Nif_Name,
                                            TaxPayerName=model.TaxPayer
                                        };
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Query Other Payment No decl with Nif Tax Files " + queryResp.OfficeCode + " " + queryResp.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return PartialView("_OtherDets", queryResp);
                                    }
                                    else
                                    {
                                        DeclarationQueryResponse qResp = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            TaxPayerName = model.TaxPayer,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency = model.Currencies,
                                            PayType = Type,
                                            DeclarantName = Declarant_Name,
                                            CompanyName = Nif_Name
                                        };
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Query Other Payment No decl No Nif Tax Files " + qResp.OfficeCode + " " + qResp.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return PartialView("_OtherDets", qResp);
                                    }
                                case 1:
                                    string nif = model.CompCode;
                                    if (!string.IsNullOrEmpty(nif))
                                    {
                                        DeclarationQueryResponse queryResp = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            CompanyCode = model.CompCode,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency = model.Currencies,
                                            PayType = Type,
                                            DeclarantName = Declarant_Name,
                                            CompanyName = Nif_Name
                                        };
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Query Other Payment No decl with Nif Tax Files " + queryResp.OfficeCode + " " + queryResp.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return PartialView("_OtherDets", queryResp);
                                    }
                                    else
                                    {
                                        DeclarationQueryResponse qResp = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            TaxPayerName = model.TaxPayer,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber=model.RegistrationNumber,
                                            AssessmentSerial=model.RegistrationSerial,
                                            Currency = model.Currencies,
                                            PayType = Type,
                                            DeclarantName = Declarant_Name,
                                            CompanyName = Nif_Name
                                        };
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Query Other Payment No decl No Nif Tax Files " + qResp.OfficeCode + " " + qResp.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return PartialView("_OtherDets", qResp);
                                    }
                                default:
                                    errorModel.ErrorMessage = "Unknown Option!";
                                    return PartialView("_QueryError", errorModel);
                            }
                        default:
                            errorModel.ErrorMessage="Option Not Yet Implemented!";
                            return PartialView("_QueryError", errorModel);
                    }

                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.Query", ex);
            }

            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDets(DeclarationQueryResponse model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                if (model.PayType == 3)
                {
                    model.RegistrationNumber = model.ReferenceNumber;
                    model.RegistrationSerial = model.AccountReference;
                }
                //model.AmountToBePaid = model.AmountToBePaid.Split('.')[0];
                var payResult = await bl.ConfirmPayTax(model, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    var fileCode = Convert.ToInt32(payResult.Data1);
                    TaxPaymentModel taxPayment = new TaxPaymentModel
                    {
                        FileCode = Convert.ToInt32(payResult.Data1),
                        ExpectedAmount = model.AmountToBePaid,
                        DclntName = model.DeclarantName,
                        CompanyName = model.OfficeName,
                        AccountHolder = model.AccountHolder,
                        TransactionCode = model.TransactionCode,
                        TransactionRef = model.TransactionRef,
                        TaxPayerName = model.TaxPayerName,
                        PayType = model.PayType,
                        PrintReceipt = true,
                        NoCharge = true,
                        ChargeAmount = Convert.ToDecimal(payResult.Data2),
                        PostToCBS = Convert.ToInt32(payResult.Data3)
                    };

                    Audit.AuditAction(_appSett, GetUserBrowser(),"Confirm Tax Dets Payment Declarant Name " + model.DeclarantName+" Company "+model.OfficeName+" Amount "+model.AmountToBePaid+ " File Code " +taxPayment.FileCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    var lists = bl.GetListModel(DBL.Enums.ListModelType.PaymentModes).Result;
                    var list = lists.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value,
                    }).ToList();

                    taxPayment.PaymentModes = list;

                    if (taxPayment.PostToCBS == 0)
                    {
                        return PartialView("_CustSearch", taxPayment);//Manual Posting
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
                        LogUtil.Error(logFile, "Tax.ConfirmPay", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.ConfirmPay", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> CustSearch(TaxPaymentModel model)
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
                model.NoCharge = true;
                if (model.NoCharge)
                {
                    model.ChargeAmount = 0.00M;
                }
                model.TaxType = 10;
                var validResult = await bl.ValidateTaxPayment(model);
                if (validResult.RespStatus == 0)
                {
                    model.PayModeName = validResult.Data1;
                    model.CrAccountName = validResult.Data2;
                    model.CustAccountName = validResult.Data3;
                    model.Amount = model.ExpectedAmount;
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Validate Tax Payment File Code " + model.FileCode + " Received From " + model.ReceivedFrom + " Amount " + model.Amount + " Cheque No " + model.ChequeNo, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
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
                        LogUtil.Error(logFile, "Tax.MakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(TaxPaymentModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                if(string.IsNullOrEmpty(model.ReceivedFrom))
                {
                    errorModel.ErrorMessage = "Received from cannot be blank!";
                    return PartialView("_QueryError", errorModel);
                }
                model.ReceivedFrom = model.ReceivedFrom.ToUpper();
                if (model.NoCharge)
                {
                    model.ChargeAmount = 0.00M;
                }
                model.TaxType = 10;
                var validResult = await bl.ValidateTaxPayment(model);
                if (validResult.RespStatus == 0)
                {
                    model.PayModeName = validResult.Data1;
                    model.CrAccountName = validResult.Data2;
                    model.CustAccountName = validResult.Data3;
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Validate Tax Payment File Code " + model.FileCode + " Received From " + model.ReceivedFrom + " Amount " + model.Amount + " Cheque No " + model.ChequeNo, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
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
                        LogUtil.Error(logFile, "Tax.MakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(TaxPaymentModel model)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                model.TaxType = 10;
                var payResult = await bl.MakeTaxPayment(model, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    FinishModel finishModel = new FinishModel
                    {
                        Code = Convert.ToInt32(payResult.Data1),
                        PaymentCode = Convert.ToInt32(payResult.Data4),
                        AutoPrint = Convert.ToInt32(payResult.Data2),
                        NeedApproval = payResult.Data3.Equals("1"),
                        Message = String.IsNullOrEmpty(payResult.RespMessage) ? "Tax payment completed successfully." :  payResult.RespMessage
                    };
                    Audit.AuditAction(_appSett, GetUserBrowser(),"Confirm Tax Payment" + model.FileCode, 5, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return PartialView("_Finish", finishModel);
                }
                else
                {
                    if (payResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = payResult.RespMessage;
                        LogUtil.Error(logFile, "Tax.ConfirmPayment", payResult.RespMessage);
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "Tax.ConfirmPayment", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> Files(string dateRange = "", int fileStatus = -1, string assesNo = "")
        {
            var data = await bl.GetTaxFiles(dateRange, fileStatus, assesNo);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Tax File " +dateRange+assesNo, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            LoadFilesList(fileStatus);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Payments(string assesNo = "", string dateRange= "")
        {
            var data = await bl.GetTaxPayments(SessionUserData.UserCode, assesNo, dateRange);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Tax Payments "+assesNo+dateRange, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }



        [HttpGet]
        public async Task<IActionResult> ApprovedPayments(string assesNo="")
        {
            var data = await bl.ApprovedPayments(SessionUserData.UserCode, assesNo);
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Approved Tax Payments "+assesNo, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetPayMode(int payMode = 0)
        {
            var data = await bl.GetPayMode(payMode);
            return Json(data.Data1);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomTax()
        {
            var customtax = await bl.GetListModel(DBL.Enums.ListModelType.CustomTax);
            return Json(customtax);
        }

        [HttpGet]
        public async Task<IActionResult> GetOtherTaxes()
        {
            var othertaxes = await bl.GetListModel(DBL.Enums.ListModelType.OtherTaxes);
            return Json(othertaxes);
        }

        [HttpGet]
        public async Task<IActionResult> GetTranRef()
        {
            try
            {
                var othertaxes = await bl.GetTranRef();
                return Json(othertaxes);
            }catch(Exception ex)
            {
                LogUtil.Error(logFile, "Tax.GetTranRef", ex);
                return Json(new FetchTaxList());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentStatus(int code = 0)
        {
            var data = await bl.GetPaymentStatusAsync(code);
            FinishModel finishModel = new FinishModel
            {
                Code =Convert.ToInt16(data.Data2),
                PaymentCode = 0,
                AutoPrint =Convert.ToInt16(data.Data3),
                NeedApproval =true,
                Message = "",
                ApprovalMessage = data.Data1,
                OBRMsg=""
            };
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Tax Payment Status  " + data.Data2, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return Json(data.Data1);
        }

        [HttpGet]
        public async Task<IActionResult> GetObrtatus(int code = 0)
        {
            var data = await bl.GetOBRStatus(code);
            FinishModel finishModel = new FinishModel
            {
                Code = Convert.ToInt16(data.Data2),
                PaymentCode = 0,
                AutoPrint = Convert.ToInt16(data.Data3),
                Status=Convert.ToInt16(data.Data4),
                OBRMsg = data.Data1,
                ApprovalMessage = "",
                Message=""
            };
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Tax OBR Status  " + data.Data2, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await bl.GetListModel(DBL.Enums.ListModelType.Bank);
            return Json(banks);
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

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await bl.GetListModel(DBL.Enums.ListModelType.Currency);
            return Json(currencies);
        }

        [HttpGet]
        public IActionResult MultiplePayments()
        {
            DeclarationQueryData d = new DeclarationQueryData();
            DeclarationQueryResponse queryResult = new DeclarationQueryResponse();
            List<DeclarationQueryResponse> queryResult2 = new List<DeclarationQueryResponse>();
            int mode = 0;
            queryResult2 = bl.Get_TempTaxDets(mode,SessionUserData.UserCode).ToList();
            if (queryResult == null)
            {
                queryResult = new DeclarationQueryResponse();
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Dets " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            ViewData["queryResult2"] = queryResult2;
            ViewData["queryResult"] = queryResult;
            if (queryResult.declCode == 0)
            {
                d.status2 = 0;
            }
            d.status = 1;
            return View(d);
        }

        [HttpPost]
        public async Task<IActionResult> MultiplePayments(DeclarationQueryData model)
        {
            DeclarationQueryData d = new DeclarationQueryData();
            DeclarationQueryResponse queryResult = new DeclarationQueryResponse();
            List<DeclarationQueryResponse> queryResult2 = new List<DeclarationQueryResponse>();
            ErrorModel errorModel = new ErrorModel();
            int mode = 0;
            string Nif_Name = "";
            string Declarant_Name = "";
            GenericModel respdata = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            respdata = await bl.Validate_Declaration(model, SessionUserData.UserCode);
            if (respdata.RespStatus == 1)
            {
                //Danger(respdata.RespMessage, true);
                errorModel.ErrorMessage = respdata.RespMessage;
                return PartialView("_QueryError", errorModel);
            }
            
                try
                {
                    if (model.OfficeCode == "")
                    {
                        ViewData["queryResult"] = queryResult;
                        return View();
                    }
                    else
                    {
                        model.mode = 0;

                        if (!string.IsNullOrEmpty(model.RegistrationSerial))
                        {
                            model.RegistrationSerial = model.RegistrationSerial.ToUpper();
                        }
                        if (!string.IsNullOrEmpty(model.AccountReference))
                        {
                            model.AccountReference = model.AccountReference.ToUpper();
                        }
                        if (!string.IsNullOrEmpty(model.OfficeCode))
                        {
                            model.OfficeCode = model.OfficeCode.ToUpper();
                        }
                    var office = await bl.CheckObrOffice(model.OfficeCode);
                    if(office.RespStatus!=0)
                    {
                        Danger("Invalid Office code", true);
                        return RedirectToAction("MultiplePayments");
                    }
                    //validate NIF and Declarant if available
                    //Johnson .... validate NIF or declarant code whiever is available
                    if (!string.IsNullOrEmpty(model.CompCode))
                    {
                        var nif_Res = await bl.ValidateNIF(model.CompCode);
                        if (nif_Res.status != 0)//failed
                        {
                            //errorModel.ErrorMessage = nif_Res.message;
                            //return PartialView("_QueryError", errorModel);
                            Danger(nif_Res.message, true);
                            return RedirectToAction("MultiplePayments");
                        }
                        Nif_Name = nif_Res.message;
                    }
                    if (!string.IsNullOrEmpty(model.DeclCode))
                    {
                        var dec_Res = await bl.ValidateDelarant(model.DeclCode);
                        if (dec_Res.status != 0)//failed
                        {
                            Danger(dec_Res.message, true);
                            return RedirectToAction("MultiplePayments");
                        }
                        Declarant_Name = dec_Res.message;
                    }
                    
                    int PayType = Convert.ToInt32(model.TaxType);
                        switch (PayType)
                        {
                            case 1:
                                 queryResult = await bl.QueryTax(model);
                                switch (Convert.ToInt32(queryResult.ErrorCode))
                                {
                                    case 38:
                                        queryResult.PayType = 5+PayType;
                                        var resp = await bl.TempTaxDecl(queryResult, model.mode, SessionUserData.UserCode);
                                        ViewData["queryResult"] = queryResult;
                                        mode = 0;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Decl " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return View(d);
                                    default:
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Query Declaration Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        queryResult.PayType = 5+PayType;
                                        ViewData["queryResult"] = queryResult;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        return View(d);
                                }
                            case 2:
                                queryResult = await bl.QueryCreditTax(model);
                                switch (Convert.ToInt32(queryResult.ErrorCode))
                                {
                                    case 37:
                                        queryResult.PayType = 5+PayType;
                                        var resp = await bl.TempTaxDecl(queryResult, model.mode, SessionUserData.UserCode);
                                        ViewData["queryResult"] = queryResult;
                                        mode = 0;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Decl " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return View(d);
                                    default:
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Query Credit Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        queryResult.PayType = 5+PayType;
                                        ViewData["queryResult"] = queryResult;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        return View(d);
                                }
                        case 4:
                            string[] Tranrefs = model.TranCode.Split("|");
                            int Other = Convert.ToInt32(Tranrefs[0]);
                            model.TranCode = Tranrefs[1];
                            switch (Other)// 0 doesn't require decl 1 requires decl
                            {
                                case 0:
                                    string Nif = model.CompCode;
                                    if (!string.IsNullOrEmpty(Nif))
                                    {
                                        queryResult = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            CompanyCode = model.CompCode,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency=model.Currencies,
                                            PayType = 5+PayType,
                                            DeclarantName=Declarant_Name,
                                            CompanyName=Nif_Name,
                                            TaxPayerName = model.TaxPayer
                                        };
                                        var resp = await bl.TempTaxDecl(queryResult, model.mode, SessionUserData.UserCode);
                                        ViewData["queryResult"] = queryResult;
                                        mode = 0;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Decl " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return View(d);
                                    }
                                    else
                                    {
                                        queryResult = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            TaxPayerName = model.TaxPayer,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency = model.Currencies,
                                            PayType = 5 + PayType,
                                            DeclarantName = Declarant_Name,
                                            CompanyName = Nif_Name
                                        };
                                        var resp = await bl.TempTaxDecl(queryResult, model.mode, SessionUserData.UserCode);
                                        ViewData["queryResult"] = queryResult;
                                        mode = 0;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Decl " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return View(d);
                                    }
                                case 1:
                                    string nif = model.CompCode;
                                    if (!string.IsNullOrEmpty(nif))
                                    {
                                        queryResult = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            CompanyCode = model.CompCode,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency = model.Currencies,
                                            PayType = 5 + PayType,
                                            DeclarantName = Declarant_Name,
                                            CompanyName = Nif_Name
                                        };
                                        var resp = await bl.TempTaxDecl(queryResult, model.mode, SessionUserData.UserCode);
                                        ViewData["queryResult"] = queryResult;
                                        mode = 0;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Decl " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return View(d);
                                    }
                                    else
                                    {
                                        queryResult = new DeclarationQueryResponse
                                        {
                                            OfficeCode = model.OfficeCode,
                                            DeclarantCode = model.DeclCode,
                                            TaxPayerName = model.TaxPayer,
                                            AmountToBePaid = model.Amount,
                                            TransactionCode = model.TranCode,
                                            TransactionRef = model.TranRef,
                                            AssessmentYear = model.RegistrationYear,
                                            AssessmentNumber = model.RegistrationNumber,
                                            AssessmentSerial = model.RegistrationSerial,
                                            Currency = model.Currencies,
                                            PayType = 5 + PayType,
                                            DeclarantName = Declarant_Name,
                                            CompanyName = Nif_Name
                                        };
                                        var resp = await bl.TempTaxDecl(queryResult, model.mode, SessionUserData.UserCode);
                                        ViewData["queryResult"] = queryResult;
                                        mode = 0;
                                        queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                        ViewData["queryResult2"] = queryResult2;
                                        d.status = 1;
                                        d.status2 = 1;
                                        Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Decl " + queryResult.declCode, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                        return View(d);
                                    }
                                default:
                                    Audit.AuditAction(_appSett, GetUserBrowser(), "Query  Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                    queryResult.PayType = 5 + PayType;
                                    ViewData["queryResult"] = queryResult;
                                    queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                    ViewData["queryResult2"] = queryResult2;
                                    d.status = 1;
                                    d.status2 = 1;
                                    return View(d);
                            }
                        default:
                                Audit.AuditAction(_appSett, GetUserBrowser(), "Query  Tax Files " + queryResult.OfficeCode + " " + queryResult.ErrorDescription, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                                queryResult.PayType = 5+PayType;
                                ViewData["queryResult"] = queryResult;
                                queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
                                ViewData["queryResult2"] = queryResult2;
                                d.status = 1;
                                d.status2 = 1;
                                return View(d);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.Query", ex);
                }
            queryResult.PayType = Convert.ToInt32(model.TaxType);
            ViewData["queryResult"] = queryResult;
            queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
            ViewData["queryResult2"] = queryResult2;
            d.status = 1;
            d.status2 = 1;
            return View(d);
        }
        

        [HttpGet]
        public IActionResult multiplePay()
        {
            int mode = 0;
            List<DeclarationQueryResponse> queryResult2 = new List<DeclarationQueryResponse>();
            queryResult2 = bl.Get_TempTaxDets(mode, SessionUserData.UserCode).ToList();
            if (queryResult2.Count==0)
            {
                Danger("Search for declaration details first to proceed!",true);
                return RedirectToAction("MultiplePayments");
            }
            Audit.AuditAction(_appSett, GetUserBrowser(), "Get Temp Tax Dets ", 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> _mPayment()
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var model = bl.GetAllTempTaxDets(2,SessionUserData.UserCode);
                if (model==null)
                {
                    return RedirectToAction("MultiplePayments");
                }
                if(model.ErrorCode != 0)
                {
                    errorModel.ErrorMessage = model.ErrorDescription;
                    return PartialView("_QueryError", errorModel);
                }
                var payResult = await bl.ConfirmPayTax(model, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    var fileCode = Convert.ToInt32(payResult.Data1);
                    TaxPaymentModel taxPayment = new TaxPaymentModel
                    {
                        FileCode = Convert.ToInt32(payResult.Data1),
                        ExpectedAmount = model.AmountToBePaid,
                        DclntName = model.DeclarantName,
                        CompanyName = model.OfficeName,
                        PrintReceipt = true,
                        ChargeAmount = Convert.ToDecimal(payResult.Data2),
                        Mode = 3, // this mode is used to identify multiple payment
                        PostToCBS = Convert.ToInt32(payResult.Data3)
                    };
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Confirm Multiple Tax Payments " + model.DeclarantName + " File Code" + taxPayment.FileCode + " Company Name " + model.OfficeName, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    //update the temporary file records for multiple pay with parent tax file code in tax file table
                    var resp = await bl.UpdateFileCodeTempDecl(taxPayment.FileCode,SessionUserData.UserCode);
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Update FileCode Temp Decl " +taxPayment.FileCode+ " Response " +resp.RespMessage, 2, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    var lists = bl.GetListModel(DBL.Enums.ListModelType.PaymentModes).Result;
                    var list = lists.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value,
                    }).ToList();

                    taxPayment.PaymentModes = list;
                    if (taxPayment.PostToCBS == 0)
                    {
                        return PartialView("_CustSearch", taxPayment);//Manual Posting
                    }
                    return View(taxPayment);
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
                        LogUtil.Error(logFile, "Tax.ConfirmPay", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "Tax.ConfirmPay", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        public async Task<IActionResult> RemoveTempDecl(int code=0)
        {
            try
            {
                int mode = 1;
                var data =  bl.GetTempTaxDets(mode,code);
                
                var result = await bl.TempTaxDecl(data, mode);
                if (result.RespStatus == 0)
                {
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Remove Temp Decl " + data.declCode, 3, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
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
                        LogUtil.Error(logFile, "Tax.RemoveTempDecl", result.RespMessage);
                        Danger("Request failed due to a database error!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(logFile, "Security.BlockUser", ex);
                Danger("Request failed due to an error!");
            }

            return RedirectToAction("MultiplePayments");
        }


       
    }
}