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
    public class PaywayGatewayController : BaseController
    {
        private Bl bl;
        private string logFile;
        private AppConfig _appSett;
        public PaywayGatewayController(IOptions<AppConfig> appSett)
        {
            _appSett = appSett.Value;
            bl = new Bl(_appSett);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public IActionResult RetailerTopUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PowerPayment()
        {
            return PartialView("_PowerPayment");
        }

        [HttpGet]
        public IActionResult RetailerTopUpPayment()
        {
            return PartialView("_RetailerTopUpPayment");
        }

        [HttpGet]
        public IActionResult Payment()
        {
            return PartialView("_Payment");
        }

        [HttpGet]
        public IActionResult RPayment()
        {
            return PartialView("_RPayment");
        }

        [HttpPost]
        public async Task<IActionResult> PowerPayment(BuyToken model)
        {
            ErrorModel errorModel = new ErrorModel();

            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await bl.GetDetails(DBL.Enums.PaywayType.BuyToken);

            if (res != null)
            {
                model.AppID = res.Data3.ToString();
                model.AppToken = res.Data4.ToString();
                model.Password = res.Data2.ToString();
                model.Username = res.Data1.ToString();
                model.ProviderId = res.Data5.ToString();
                model.TxId = res.Data8.ToString();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var req = new GetProviders
                    {
                        AppID = model.AppID,
                        AppToken = model.AppToken,
                        Password = model.Password,
                        Username = model.Username,
                        ProviderId = model.ProviderId
                    };
                    var vr = await bl.GetProvider(req);


                    ServiceProviderDetails serviceProvider = new ServiceProviderDetails();
                    if (vr != null)
                    {
                        serviceProvider.Availability = vr.ElementAt(0);
                    }


                    if (serviceProvider.Availability == "Available")
                    {

                        var req2 = new GetCustomers
                        {
                            AppID = model.AppID,
                            AppToken = model.AppToken,
                            Password = model.Password,
                            Username = model.Username,
                            ProviderId = model.ProviderId,
                            ContactInfo = model.ContactInfo,
                            CustomerNumber = model.CustomerNumber,
                            Amount = model.Amount
                        };

                        var vr2 = await bl.GetCustomer(req2);

                        CustomerDetails customerDetails = new CustomerDetails();

                        if (vr2 != null)
                        {
                            customerDetails.Valid = Convert.ToBoolean(vr2.ElementAt(0));
                            customerDetails.Status = Convert.ToInt32(vr2.ElementAt(1));
                            customerDetails.Notes = vr2.ElementAt(2);
                        }


                        if (customerDetails.Valid == true)
                        {
                            string transactionid = model.TxId;
                            PowerPaymentModel powerPayment = new PowerPaymentModel
                            {
                                TransactionId = transactionid,
                                UserCode = SessionUserData.UserCode,
                                Amount = Convert.ToDecimal(model.Amount),
                                ContactInfo = model.ContactInfo,
                                CustomerNumber = model.CustomerNumber,
                                PhoneNo = model.ContactInfo,
                                CustomerNo = model.CustomerNumber,
                                PaywayAccountName = customerDetails.Notes
                            };

                            var lists = bl.GetListModel(DBL.Enums.ListModelType.Payway).Result;
                            var list = lists.Select(x => new SelectListItem
                            {
                                Text = x.Text,
                                Value = x.Value,
                            }).ToList();

                            powerPayment.PaymentModes = list;
                            Audit.AuditAction(_appSett, GetUserBrowser(), "Validate Customer Details " + model.CustomerNumber + " " + model.ContactInfo + " " + model.Amount, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());

                            return PartialView("_Payment", powerPayment);
                        }
                        else
                        {
                            errorModel = new ErrorModel
                            {
                                ErrorMessage = customerDetails.Notes
                            };
                            return PartialView("_QueryError", errorModel);
                        }

                    }
                    else
                    {
                        errorModel = new ErrorModel
                        {
                            ErrorMessage = serviceProvider.msg
                        };
                        return PartialView("_QueryError", errorModel);
                    }

                }
                catch (Exception ex)
                {
                    errorModel.ErrorMessage = "Operation failed due to an error!";
                    LogUtil.Error(logFile, "PaywayGateway.PowerPayment", ex);
                }
            }
            else
            {
                errorModel.ErrorMessage = "Some or all the data entered is invalid!";
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> RetailerPayment(TopUp model)
        {
            ErrorModel errorModel = new ErrorModel();

            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await bl.GetDetails(DBL.Enums.PaywayType.RetailerTopUp);
            if (res != null)
            {
                model.AppID = res.Data3.ToString();
                model.AppToken = res.Data4.ToString();
                model.Password = res.Data2.ToString();
                model.Username = res.Data1.ToString();
                model.ProviderId = res.Data5.ToString();
                model.TxId = res.Data8.ToString();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var req = new ValidateRetailers
                    {
                        AppID = model.AppID,
                        AppToken = model.AppToken,
                        Password = model.Password,
                        Username = model.Username,
                        ProviderId = model.ProviderId,
                        CustomerNo = model.CustomerNumber
                    };
                    var vr = await bl.GetRetailer(req);

                    RetailerValid serviceProvider = new RetailerValid();
                    if (vr != null)
                    {
                        serviceProvider.Valid = Convert.ToBoolean(vr.ElementAt(0));
                        serviceProvider.Notes = vr.ElementAt(1);
                    }

                    if (serviceProvider.Valid == true)
                    {

                        string transactionid = model.TxId;

                        RetailerTopUpModel powerPayment = new RetailerTopUpModel
                        {
                            TransactionId = transactionid,
                            UserCode = SessionUserData.UserCode,
                            Amount = Convert.ToDecimal(model.Amount),
                            ContactInfo = model.ContactInfo,
                            PhoneNo = model.ContactInfo,
                            CustomerNumber = model.CustomerNumber,
                            CustomerNo = model.CustomerNumber,
                            PaywayAccountName = serviceProvider.Notes
                        };

                        var lists = bl.GetListModel(DBL.Enums.ListModelType.Payway).Result;
                        var list = lists.Select(x => new SelectListItem
                        {
                            Text = x.Text,
                            Value = x.Value,
                        }).ToList();
                        Audit.AuditAction(_appSett, GetUserBrowser(), "Validate Retailer Details " + model.CustomerNumber + " " + model.ContactInfo + " " + model.Amount, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                        powerPayment.PaymentModes = list;

                        return PartialView("_rpayment", powerPayment);

                    }
                    else
                    {
                        errorModel = new ErrorModel
                        {
                            ErrorMessage = serviceProvider.Notes
                        };
                        return PartialView("_QueryError", errorModel);
                    }

                }
                catch (Exception ex)
                {
                    errorModel.ErrorMessage = "Operation failed due to an error!";
                    LogUtil.Error(logFile, "PaywayGateway.PowerPayment", ex);
                }
            }
            else
            {
                errorModel.ErrorMessage = "Some or all the data entered is invalid!";
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> TokenPayments(string assesNo = "", string dateRange = "")
        {
            var data = await bl.GetPowerPayments(SessionUserData.UserCode, assesNo, dateRange);
            Audit.AuditAction(_appSett, GetUserBrowser(), "View All CashPower Payments " + assesNo + " " + dateRange, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> EvaluePayments(string assesNo = "", string dateRange = "")
        {
            var data = await bl.GetTopUpPayments(SessionUserData.UserCode, assesNo, dateRange);
            Audit.AuditAction(_appSett, GetUserBrowser(), "View All Evalue Payments " + assesNo + " " + dateRange, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
            return View(data);
        }



        [HttpPost]
        public async Task<IActionResult> MakePayment(PowerPaymentModel model)
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
                var validResult = await bl.validatePayment(model);
                if (validResult.RespStatus == 0)
                {
                    model.PayModeName = validResult.Data1;
                    model.ContactInfo = validResult.Data2;
                    model.CustomerNumber = validResult.Data3;
                    model.AccountName = validResult.Data3;
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Validate CashPower Payment " + model.CustomerNumber + " " + model.ContactInfo + " " + model.Amount + " " + model.ReceivedFrom, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
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
                        LogUtil.Error(logFile, "PaywayGateway.MakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "PaywayGateway.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> RMakePayment(RetailerTopUpModel model)
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
                var validResult = await bl.validateRetailerPayment(model);
                if (validResult.RespStatus == 0)
                {
                    model.PayModeName = validResult.Data1;
                    model.ContactInfo = validResult.Data2;
                    model.CustomerNumber = validResult.Data3;
                    model.AccountName = validResult.Data3;
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Validate EValue Payments  " + model.CustomerNumber + " " + model.ContactInfo + " " + model.Amount + " " + model.ReceivedFrom, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return PartialView("_RConfirm", model);
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
                        LogUtil.Error(logFile, "PaywayGateway.RetailerMakePayment", validResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "PaywayGateway.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(PowerPaymentModel powerPaymentModel)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                //  model.brachCode = SessionUserData.brachCode;
                var browser = GetUserBrowser();
                var payResult = await bl.MakePowerPayment(powerPaymentModel, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    FinishModel finishModel = new FinishModel
                    {
                        Code = Convert.ToInt32(payResult.Data1),
                        PaymentCode = Convert.ToInt32(payResult.Data4),
                        AutoPrint = Convert.ToInt32(payResult.Data2),
                        NeedApproval = payResult.Data3.Equals("1"),
                        Message = String.IsNullOrEmpty(payResult.RespMessage) ? "PowerPayment completed successfully." :
                        payResult.RespMessage,
                        OBRMsg = ""
                    };
                    Audit.AuditAction(_appSett, GetUserBrowser(), "Cash Power Payment " + powerPaymentModel.TransactionId + " " + powerPaymentModel.ReceivedFrom + " " + powerPaymentModel.Amount, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());

                    return PartialView("_Finish", finishModel);
                }
                else
                {
                    if (payResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = payResult.RespMessage;
                        LogUtil.Error(logFile, "PowerPayment.MakePayment", payResult.RespMessage);
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "PowerPayment.MakePayment", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "PowerPayment.MakePayment", ex);
            }
            return PartialView("_QueryError", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> RConfirmPayment(RetailerTopUpModel retailerTopUpModel)
        {
            ErrorModel errorModel = new ErrorModel();
            try
            {
                var browser = GetUserBrowser();
                var payResult = await bl.RetailerTopUp(retailerTopUpModel, SessionUserData.UserCode);
                if (payResult.RespStatus == 0)
                {
                    FinishModel finishModel = new FinishModel
                    {
                        Code = Convert.ToInt32(payResult.Data1),
                        PaymentCode = Convert.ToInt32(payResult.Data4),
                        AutoPrint = Convert.ToInt32(payResult.Data2),
                        NeedApproval = payResult.Data3.Equals("1"),
                        Message = String.IsNullOrEmpty(payResult.RespMessage) ? "Retailer Top UpPayment completed successfully." :
                        payResult.RespMessage,
                        OBRMsg = ""
                    };
                    Audit.AuditAction(_appSett, GetUserBrowser(), "EValue Payment " + retailerTopUpModel.TransactionId + " " + retailerTopUpModel.ReceivedFrom + " " + retailerTopUpModel.Amount, 1, this.ControllerContext.RouteData.Values["controller"].ToString(), SessionUserData.UserCode, GetIP());
                    return PartialView("_RFinish", finishModel);
                }
                else
                {
                    if (payResult.RespStatus == 1)
                    {
                        errorModel.ErrorMessage = payResult.RespMessage;
                        LogUtil.Error(logFile, "RetailerTopUp.MakePayment", payResult.RespMessage);
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Operation failed due to a database error!";
                        LogUtil.Error(logFile, "RetailerTopUp.MakePayment", payResult.RespMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Operation failed due to an error!";
                LogUtil.Error(logFile, "PowerPayment.MakePayment", ex);
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
        public async Task<IActionResult> GetPayMode(int payMode = 0)
        {
            var data = await bl.GetPayMod(payMode);
            return Json(data.Data1);
        }
    }
}