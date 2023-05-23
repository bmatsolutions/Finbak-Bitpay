using BITPay.DBL.Consts;
using BITPay.DBL.Entities;
using BITPay.DBL.Enums;
using BITPay.DBL.Models;
using BITPay.DBL.UOW;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bmat.Tools;
using BITPay.DBL.Gateways;
using FastReport;
using FastReport.Export.PdfSimple;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using FastReport.Utils;
using NPOI.SS.Formula.Functions;
using System.Reflection;

namespace BITPay.DBL
{
    public class Bl
    {
        private UnitOfWork db;
        private string _connString;
        private string _logFile;

        public Bl(AppConfig appConfig)
        {
            _connString = appConfig.ConnectionString;
            _logFile = appConfig.LogFile;
            db = new UnitOfWork(_connString);
        }

        #region User
        public async Task<UserModel> UserLogin(string userName, string password)
        {
            return await Task.Run(() =>
            {
                UserModel userModel = new UserModel { };

                //------ Get User
                var user = db.SecurityRepository.VerifyUser(userName);
                if (user == null)
                {
                    userModel.RespStatus = 1;
                    userModel.RespMessage = "Invalid Username and/or Password!";
                    return userModel;
                }
                //----- Check user status
                if (user.RespStatus == 0)
                {
                    //----- Validate User Password
                    BTSecurity sec = new BTSecurity();
                    if (sec.ValidatePassword(password, user.Data2, user.Data3))
                    {
                        var respData = db.SecurityRepository.UserLogin(Convert.ToInt32(user.Data1), 0);
                        db.Reset();

                        if (respData.RespStatus == 0)
                        {
                            bool changePass = Convert.ToBoolean(respData.Data3);
                            int role = Convert.ToInt32(respData.Data4);
                            string roleName = "invalid";
                            var uRole = ConstData.GetUserRoles().Where(x => x.Value == respData.Data4).FirstOrDefault();
                            if (uRole != null)
                                roleName = uRole.Text.ToLower();

                            userModel = new UserModel
                            {
                                FullNames = respData.Data2,
                                Id = Convert.ToInt32(respData.Data1),
                                UserName = userName,
                                UserStatus = UserLoginStatus.Ok,
                                UserCode = Convert.ToInt32(user.Data1),
                                UserRole = role,
                                UserRoleName = roleName,
                                Extra1 = respData.Data5,
                                Extra2 = respData.Data6,
                                Extra3 = respData.Data7
                                // brachCode = respData.Data6
                            };

                            if (changePass)
                                userModel.UserStatus = UserLoginStatus.ChangePassword;

                            return userModel;
                        }
                        else
                        {
                            userModel.RespStatus = 1;
                            userModel.RespMessage = respData.RespMessage;
                        }
                    }
                    else
                    {
                        var respData = db.SecurityRepository.UserLogin(Convert.ToInt32(user.Data1), 1);
                        db.Reset();

                        userModel.RespStatus = 1;
                        userModel.RespMessage = "Incorrect Username and/or Password!";
                    }
                }
                else
                {
                    userModel.RespStatus = 1;
                    userModel.RespMessage = user.RespMessage;
                }
                return userModel;
            });
        }


        public async Task<IEnumerable<ListModel>> GetTaxCatergory(int mode)
        {
            return await Task.Run(() =>
            {
                return db.GeneralRepository.GetPayMode(mode);
            });
        }

        public async Task<RetailerTopUpModel> GetRetailerTopUpPaymentApprovalQueueItem(int code)
        {
            {
                return db.PaywayGatewayRepository.GetRetailerApprovalQueueItem(code);
            };
        }

        public async Task<IEnumerable<RetailerTopUpModel>> GetTopUpPaymentsApprovalQueue()
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetTopUpApprovalQueue();
            });
        }

        public async Task<BaseEntity> ChangeUserPassword(ChangeUserPassModel data)
        {
            return await Task.Run(() =>
            {
                //---- Get user
                var passDets = db.SecurityRepository.GetUserPassword(data.UserCode);
                if (passDets.RespStatus != 0)
                {
                    return new BaseEntity
                    {
                        RespStatus = passDets.RespStatus,
                        RespMessage = passDets.RespMessage
                    };
                }

                //---- Validate old password
                BTSecurity sec = new BTSecurity();
                if (!sec.ValidatePassword(data.OldPassword, passDets.Data1, passDets.Data2))
                {
                    return new BaseEntity
                    {
                        RespStatus = 1,
                        RespMessage = "Invalid old password!"
                    };
                }

                //----- Create and update password
                string salt = sec.GenerateSalt(30);
                string password = sec.HashPassword(data.NewPassword, salt);

                var result = db.SecurityRepository.ChangeUserPassword(data.UserCode, password, salt);
                db.Reset();

                return result;
            });
        }

        public async Task<GenericModel> BuyTokenSupervisePaymentItem(int paymentCode, int action, string reason, int userCode)
        {
            return await Task.Run(async () =>
            {
                GenericModel makePaymentResponse = new GenericModel
                {
                    RespStatus = 1,
                    RespMessage = "Unknown response!",
                    Data1 = "0",
                    Data2 = "0"
                };

                var result = db.PaywayGatewayRepository.SupervisePayment(paymentCode, action, reason, userCode);


                if (result.RespStatus == 0)
                {
                    //-----Post To payway here
                    PowerPaymentModel powerPaymentModel = new PowerPaymentModel
                    {
                        CustomerNumber = result.CustomerNo,
                        Amount = result.MainAmount,
                        ContactInfo = result.ContactInfo,
                        TransactionId = result.TransactionId.ToString()
                    };
                    var postRPayment = await PostPowersPayment(powerPaymentModel, paymentCode);

                    if (postRPayment.stat != 0)
                    {
                        makePaymentResponse.RespStatus = 3;
                        makePaymentResponse.RespMessage = postRPayment.msg;
                        db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postRPayment.msg);
                        makePaymentResponse.RespStatus = 1;
                        return makePaymentResponse;
                    }

                    var result2 = new TaxToPostModel
                    {
                        PostUrl = result.PostUrl,
                        MainCrAccount = result.MainCrAccount,
                        MainDrAccount = result.MainDrAccount,
                        CurrencyCode = result.CurrencyCode,
                        MainFlag = result.MainFlag,
                        MainNarration = result.MainNarration,
                        CBSOfficer = result.CBSOfficer,
                        MainRefNo = result.MainRefNo,
                        TransactorName = result.TransactorName,
                        MainTxnCode = result.MainTxnCode,
                        MainTxnType = result.MainTxnType,
                        ChequeNo = result.ChequeNo,
                        BrachCode = result.BrachCode,
                        ChargeCrAccount = result.ChargeCrAccount,
                        ChargeDrAccount = result.ChargeDrAccount,
                        ChargeFlag = result.ChargeFlag,
                        ChargeNarration = result.ChargeNarration,
                        ChargeRefNo = result.ChargeRefNo,
                        ChargeTxnCode = result.ChargeTxnCode,
                        ChargeTxnType = result.ChargeTxnType,
                        MainAmount = postRPayment.MainAmount,
                        ChargeAmount = postRPayment.ChargeAmount
                    };
                    //------ Do the posting here
                    var postResp = PostPowerFlex(result2);
                    if (!postResp.Successful)
                    {
                        makePaymentResponse.Data2 = "1";
                        makePaymentResponse.RespStatus = 1;
                        if (postResp.Message.StartsWith("ERROR!"))
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! >>" + postResp.Message;
                        }
                        else
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;

                        }//---- Update payment status
                        //txn failed update status to failure
                        makePaymentResponse.RespStatus = 4;
                        db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message);
                        makePaymentResponse.RespStatus = 1;
                    }
                    else
                    {

                        //---- Update payment status
                        var update = db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message);

                        if (update.RespStatus == 0)
                        {

                            // var updatetoken = db.PaywayGatewayRepository.SaveTokenData(postResp.CBSRefNo, postRPayment.content);
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                        }

                    }
                }
                else
                {
                    makePaymentResponse.RespStatus = 2; //status 2 is a rejected txn by supervisor
                    makePaymentResponse.RespMessage = result.RespMessage;
                }
                //---- Update payment status for multiple temp payment records,
                //db.PaywayGatewayRepository.ArchTempDecl(result.Filecode);

                return makePaymentResponse;
            });
        }


        public async Task<GenericModel> RetailerTopUpSupervisePaymentItem(int paymentCode, int action, string reason, int userCode)
        {
            return await Task.Run(async () =>
            {
                GenericModel makePaymentResponse = new GenericModel
                {
                    RespStatus = 1,
                    RespMessage = "Unknown response!",
                    Data1 = "0",
                    Data2 = "0"
                };

                var result = db.PaywayGatewayRepository.SuperviseTopUpPayment(paymentCode, action, reason, userCode);
                if (result.RespStatus == 0)
                {
                    RetailerTopUpModel retailerTopUpModel = new RetailerTopUpModel
                    {
                        CustomerNumber = result.CustomerNo,
                        Amount = result.ChargeAmount + result.MainAmount,
                        ContactInfo = result.ContactInfo,
                        TransactionId = result.TransactionId.ToString()
                    };
                    var postRPayment = await PostRetailerPayment(retailerTopUpModel);
                    string TopUpStatus = "";
                    string TopUpNarration = "";
                    string TopUpTxid = "";
                    if (postRPayment.stat == 0)
                    {
                        TopUpStatus = postRPayment.content.status;
                        TopUpNarration = postRPayment.content.Name;
                        TopUpTxid = postRPayment.content.outTxId;
                    }
                    else
                    {
                        //TopUpStatus = postRPayment.content.status;
                        //TopUpNarration = postRPayment.msg;
                        //TopUpTxid = postRPayment.content.outTxId;
                        makePaymentResponse.RespStatus = 3;
                        makePaymentResponse.RespMessage = postRPayment.msg;
                        db.PaywayGatewayRepository.UpdateTopUpStatus(result.PaymentCode, makePaymentResponse.RespStatus, postRPayment.msg, TopUpStatus, TopUpNarration, TopUpTxid);
                        makePaymentResponse.RespStatus = 1;
                    }
                    //------ Do the posting here
                    var postResp = PostToFlex(result);
                    if (!postResp.Successful)
                    {
                        makePaymentResponse.Data2 = "1";
                        makePaymentResponse.RespStatus = 1;
                        if (postResp.Message.StartsWith("ERROR!"))
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! >>" + postResp.Message;
                        }
                        else
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;

                        }//---- Update payment status
                        //txn failed update status to failure
                        makePaymentResponse.RespStatus = 4;
                        db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message);
                        makePaymentResponse.RespStatus = 1;
                    }
                    else
                    {

                        makePaymentResponse.RespStatus = 0;
                        makePaymentResponse.RespMessage = "Transaction posting was successful.";

                        //---- Update payment status


                        db.PaywayGatewayRepository.UpdateTopUpStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message, TopUpStatus, TopUpNarration, TopUpTxid);

                    }
                }
                else
                {
                    makePaymentResponse.RespStatus = 2; //status 2 is a rejected txn by supervisor
                    makePaymentResponse.RespMessage = result.RespMessage;
                }
                //---- Update payment status for multiple temp payment records,
                //db.PaywayGatewayRepository.ArchTempDecl(result.Filecode);

                return makePaymentResponse;
            });
        }

        public async Task<UserProfileModel> GetUserProfile(int userCode)
        {
            return await Task.Run(() =>
            {
                //------ Get User
                var user = db.SecurityRepository.GetUserProfile(userCode);
                return user;
            });
        }

        public async Task<IEnumerable<SysUserModel>> GetUsers()
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.GetUsers();
                return user;
            });
        }

        public async Task<IEnumerable<Syssetting>> GetSysSettings()
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.GetSysSettings();
                return user;
            });
        }



        public async Task<SysUserModel> GetUser(int code)
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.GetUser(code);
                return user;
            });
        }

        public async Task<Syssetting> GetSystemSett(int code)
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.GetSyssetting(code);
                return user;
            });
        }
        public async Task<vwUser> Get_User(int code)
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.Get_User(code);
                return user;
            });
        }

        public async Task<vwSystemSett> Get_SystemSett(int code)
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.Get_Systemsett(code);
                return user;
            });
        }

        public async Task<BaseEntity> CreateUser(User user, int makerUser)
        {
            return await Task.Run(() =>
            {
                BTSecurity security = new BTSecurity();
                string salt = security.GenerateSalt(25);
                string password = security.HashPassword(user.Pwd, salt);

                user.Salt = salt;
                user.Pwd = password;

                var result = db.SecurityRepository.CreateUser(user, makerUser);
                db.Reset();

                return new BaseEntity
                {
                    RespMessage = result.RespMessage,
                    RespStatus = result.RespStatus
                };
            });
        }

        public async Task<BaseEntity> UpdateSystemSett(vwSystemSett user, int makerUser)
        {
            return await Task.Run(() =>
            {
                var result = db.SecurityRepository.UpdateParams(user, makerUser);
                db.Reset();

                return new BaseEntity
                {
                    RespMessage = result.RespMessage,
                    RespStatus = result.RespStatus
                };
            });
        }

        public async Task<BaseEntity> UpdateUser(vwUser user, int makerUser)
        {
            return await Task.Run(() =>
            {
                var result = db.SecurityRepository.UpdateUser(user, makerUser);
                db.Reset();

                return new BaseEntity
                {
                    RespMessage = result.RespMessage,
                    RespStatus = result.RespStatus
                };
            });
        }

        public async Task<GenericModel> BlockUser(int code, int Maker, int mode)
        {
            return await Task.Run(() =>
            {
                var user = db.SecurityRepository.BlockUser(code, Maker, mode);
                return user;
            });
        }

        public async Task<BaseEntity> SetUserCashAccount(int userCode, string account)
        {
            return await Task.Run(() =>
            {

                var result = db.SecurityRepository.UpdateUserCashAccount(userCode, account);
                db.Reset();

                return result;
            });
        }



        public async Task<BaseEntity> ResetUserPassword(int userCode)
        {
            return await Task.Run(() =>
            {
                string rawPassword = new Random().Next(10000000, 99999999).ToString();
                BTSecurity security = new BTSecurity();
                string salt = security.GenerateSalt(25);
                string password = security.HashPassword(rawPassword, salt);

                var result = db.SecurityRepository.ResetUserPassword(userCode, salt, password);
                db.Reset();

                return new BaseEntity
                {
                    RespMessage = result.RespStatus == 0 ? rawPassword : result.RespMessage,
                    RespStatus = result.RespStatus
                };
            });
        }
        #endregion

        #region Tax
        public async Task<DeclarationQueryResponse> QueryTax(DeclarationQueryData queryData)
        {
            string responseXml = "";
            var queryResp = new DeclarationQueryResponse
            {
                ErrorCode = -1,
                ErrorDescription = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryTax);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.ErrorDescription = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                if (setting.Data6 == "0")
                {
                    ////===================================
                    if (!File.Exists(setting.Data7))
                    {
                        return new DeclarationQueryResponse { ErrorCode = -1, ErrorDescription = "Test response file does not exists!!" };
                    }
                    responseXml = File.ReadAllText(setting.Data7);
                    ////=====================================
                }
                else
                {
                    var Url = setting.Data1 + (setting.Data1.EndsWith("/") ? "" : "/") + (queryData.TaxType == "1" ? "WSDeclarationPayment?wsdl" : "WSCreditPayment?wsdl");
                    //---- Prepare query data
                    DeclarationQueryRequestData qData = new DeclarationQueryRequestData
                    {
                        AssessmentNumber = queryData.AssessmentNumber,
                        AssessmentSerial = queryData.AssessmentSerial,
                        OfficeCode = queryData.OfficeCode,
                        RegistrationNumber = queryData.RegistrationNumber,
                        RegistrationSerial = queryData.RegistrationSerial,
                        RegistrationYear = queryData.RegistrationYear
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = Url,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.QueryTax,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    RequestResponseModel respData = JsonConvert.DeserializeObject<RequestResponseModel>(postResults);

                    if (respData.Status != 0)
                    {
                        //---- Request failed
                        switch (respData.Status)
                        {
                            case 1:
                                queryResp.ErrorDescription = respData.Message;
                                break;
                            case 2:
                                queryResp.ErrorDescription = "Request failed due to an error!";
                                break;
                            case 3:
                                queryResp.ErrorDescription = "Request failed due to http error!";
                                break;
                        }

                        return queryResp;
                    }
                    responseXml = (string)respData.Content;
                }


                XDocument xml = XDocument.Parse(responseXml);
                var myData = xml.Descendants().Where(x => x.Name.LocalName == "getDeclarationDetailsResult").FirstOrDefault();
                if (myData != null)
                {
                    var errorCode = (int)myData.Element("errorCode");
                    queryResp.ErrorCode = errorCode;
                    queryResp.ErrorDescription = (string)myData.Element("errorDescription");
                    queryResp.Result = (string)myData.Element("result");
                    switch (errorCode)
                    {
                        case 38:
                            queryResp.OfficeCode = (string)myData.Element("officeCode");
                            queryResp.OfficeName = (string)myData.Element("officeName");
                            queryResp.DeclarantCode = (string)myData.Element("declarantCode");
                            queryResp.DeclarantName = (string)myData.Element("declarantName");
                            queryResp.CompanyCode = (string)myData.Element("companyCode");
                            queryResp.CompanyName = (string)myData.Element("companyName");
                            queryResp.AssessmentYear = (string)myData.Element("assessmentYear");
                            queryResp.AssessmentSerial = (string)myData.Element("assessmentSerial");
                            queryResp.AssessmentNumber = (string)myData.Element("assessmentNumber");
                            queryResp.RegistrationNumber = queryData.AccountReference + queryData.ReferenceNumber + queryData.RegistrationNumber;
                            queryResp.RegistrationSerial = queryData.RegistrationSerial;
                            string amountToBePaid = (string)myData.Element("amountToBePaid");

                            //----- Some amounts are in scientific format, handle them here
                            var amount = Util.ParseNumber(amountToBePaid);
                            if (amount != 0)
                            {
                                queryResp.AmountToBePaid = (decimal)Math.Round(amount, 0);
                            }
                            break;
                        case 34:
                            queryResp.ReceiptSerial = (string)myData.Element("receiptSerial");
                            queryResp.ReceiptNumber = (string)myData.Element("receiptNumber");
                            queryResp.ReceiptDate = (string)myData.Element("receiptDate");
                            break;
                    }
                }
                return queryResp;
            }
            else
            {
                queryResp.ErrorDescription = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<DeclarationQueryResponse> QueryCreditTax(DeclarationQueryData queryData)
        {
            string responseXml = "";
            var queryResp = new DeclarationQueryResponse
            {
                ErrorCode = -1,
                ErrorDescription = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryTax);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.ErrorDescription = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                if (setting.Data6 == "0")
                {
                    ////===================================
                    responseXml = File.ReadAllText(setting.Data7);
                    ////=====================================
                }
                else
                {
                    var Url = setting.Data1 + (setting.Data1.EndsWith("/") ? "" : "/") + (queryData.TaxType == "1" ? "WSDeclarationPayment?wsdl" : "WSCreditPayment?wsdl");
                    //---- Prepare query data
                    DeclarationQueryRequestData qData = new DeclarationQueryRequestData
                    {
                        AssessmentNumber = queryData.AssessmentNumber,
                        AssessmentSerial = queryData.AssessmentSerial,
                        OfficeCode = queryData.OfficeCode,
                        RegistrationNumber = queryData.RegistrationNumber,
                        RegistrationSerial = queryData.RegistrationSerial,
                        RegistrationYear = queryData.RegistrationYear
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = Url,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.QueryTax,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    RequestResponseModel respData = JsonConvert.DeserializeObject<RequestResponseModel>(postResults);

                    if (respData.Status != 0)
                    {
                        //---- Request failed
                        switch (respData.Status)
                        {
                            case 1:
                                queryResp.ErrorDescription = respData.Message;
                                break;
                            case 2:
                                queryResp.ErrorDescription = "Request failed due to an error!";
                                break;
                            case 3:
                                queryResp.ErrorDescription = "Request failed due to http error!";
                                break;
                        }

                        return queryResp;
                    }
                    responseXml = (string)respData.Content;
                }


                XDocument xml = XDocument.Parse(responseXml);
                var myData = xml.Descendants().Where(x => x.Name.LocalName == "getDeclarationDetailsResult").FirstOrDefault();
                if (myData != null)
                {
                    var errorCode = (int)myData.Element("errorCode");
                    queryResp.ErrorCode = errorCode;
                    queryResp.ErrorDescription = (string)myData.Element("errorDescription");
                    queryResp.Result = (string)myData.Element("result");
                    switch (errorCode)
                    {
                        case 37:
                            queryResp.OfficeCode = (string)myData.Element("officeCode");
                            queryResp.OfficeName = (string)myData.Element("officeName");
                            queryResp.DeclarantCode = (string)myData.Element("creditAccountRef");
                            queryResp.DeclarantName = (string)myData.Element("declarantName");
                            queryResp.CompanyCode = (string)myData.Element("companyCode");
                            queryResp.CompanyName = (string)myData.Element("companyName");
                            queryResp.AssessmentYear = (string)myData.Element("assessmentYear");
                            queryResp.AssessmentSerial = (string)myData.Element("assessmentSerial");
                            queryResp.AssessmentNumber = (string)myData.Element("assessmentNumber");
                            queryResp.RegistrationNumber = queryData.RegistrationNumber;
                            queryResp.RegistrationSerial = queryData.RegistrationSerial;
                            string amountToBePaid = (string)myData.Element("amountToBePaid");

                            //----- Some amounts are in scientific format, handle them here
                            var amount = Util.ParseNumber(amountToBePaid);
                            if (amount != 0)
                            {
                                queryResp.AmountToBePaid = (decimal)Math.Round(amount, 0);
                            }
                            break;
                        case 3:
                            queryResp.ReceiptSerial = (string)myData.Element("receiptSerial");
                            queryResp.ReceiptNumber = (string)myData.Element("receiptNumber");
                            queryResp.ReceiptDate = (string)myData.Element("receiptDate");
                            break;
                    }
                }
                return queryResp;
            }
            else
            {
                queryResp.ErrorDescription = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<DeclarationQueryResponse> QueryCredit(DeclarationQueryData queryData)
        {
            string responseXml = "";
            var queryResp = new DeclarationQueryResponse
            {
                ErrorCode = -1,
                ErrorDescription = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryTax);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.ErrorDescription = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {
                    var Url = setting.Data1 + (setting.Data1.EndsWith("/") ? "" : "/") + (queryData.TaxType == "3" ? "WSCreditStatement?wsdl" : "");
                    //---- Prepare query data
                    CreditQueryRequestData qData = new CreditQueryRequestData
                    {
                        OfficeCode = queryData.OfficeCode,
                        ReferenceNumber = queryData.ReferenceNumber,
                        ReferenceYear = queryData.ReferenceYear,
                        AccountReference = queryData.AccountReference
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = Url,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.QueryCredit,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    CreditResponseModel respData = JsonConvert.DeserializeObject<CreditResponseModel>(postResults);

                    if (respData.Content != null)
                    {

                        queryResp = new DeclarationQueryResponse
                        {
                            ErrorCode = respData.Status,
                            ErrorDescription = respData.Message,
                            CompanyName = respData.Content.cmpyName,
                            AccountHolder = respData.Content.accHolder,
                            OfficeCode = respData.Content.offCode,
                            OfficeName = respData.Content.offName,
                            ReferenceNumber = queryData.ReferenceNumber,
                            AssessmentYear = queryData.ReferenceYear,
                            AccountReference = queryData.AccountReference,
                            AmountToBePaid = (decimal)Math.Round(Util.ParseNumber(respData.Content.ttAmt), 0),
                            DeclarantCode = respData.Content.accHolder,
                            ReceiptDate = respData.Content.date,
                            ReceiptNumber = respData.Content.recNo,
                            ReceiptSerial = respData.Content.recSerial
                        };
                    }
                }
                return queryResp;
            }
            else
            {
                queryResp.ErrorDescription = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<FetchTaxList> GetTranRef()
        {
            string responseXml = "";
            var queryResp = new FetchTaxList
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryTax);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {
                    var Url = setting.Data1 + (setting.Data1.EndsWith("/") ? "" : "/") + "WSOtherPayment?wsdl";
                    //---- Prepare query data

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = Url,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.QueryTranRef
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        //throw ex;
                        return new FetchTaxList();
                    }

                    FetchTaxList respData = JsonConvert.DeserializeObject<FetchTaxList>(postResults);

                    queryResp = respData;
                }
                return queryResp;
            }
            else
            {
                queryResp.message = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<GenericModel> ConfirmPayTax(DeclarationQueryResponse queryData, int userCode)
        {
            return await Task.Run(() =>
            {
                int assYear = Convert.ToInt32(queryData.AssessmentYear);
                decimal taxAmount = Convert.ToDecimal(queryData.AmountToBePaid);

                TaxFile taxFile = new TaxFile
                {
                    AsmtNumber = queryData.AssessmentNumber,
                    AsmtSerial = queryData.AssessmentSerial,
                    RegYear = assYear,
                    CompanyCode = queryData.CompanyCode,
                    CompanyName = queryData.CompanyName,
                    DclntCode = queryData.DeclarantCode,
                    DclntName = queryData.DeclarantName,
                    OfficeCode = queryData.OfficeCode,
                    OfficeName = queryData.OfficeName,
                    TaxAmount = taxAmount,
                    UserCode = userCode,
                    RegNumber = queryData.RegistrationNumber,
                    RegSerial = queryData.RegistrationSerial,
                    PayType = queryData.PayType,
                    AccountHolder = queryData.AccountHolder,
                    TaxPayerName = queryData.TaxPayerName,
                    TransactionCode = queryData.TransactionCode,
                    TransactionRef = queryData.TransactionRef,
                    Currency = queryData.Currency
                };

                var result = db.TaxRepository.CreateFile(taxFile);
                db.Reset();

                return result;
            });
        }

        public async Task<TaxPaymentModel> CustSearch(string Ref, int userCode)
        {
            return await Task.Run(() =>
            {
                return db.TaxRepository.CustSearch(Ref);
            });
        }

        public async Task<GenericModel> ValidateTaxPayment(TaxPaymentModel taxPayment)
        {
            var result = await db.TaxRepository.ValidateTaxPaymentAsync(taxPayment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                if (taxPayment.PostToCBS == 1)//post to cbs
                {
                    //---- Validate accounts
                    if (taxPayment.PayMode == 1)
                    {
                        var jsonAccount = JsonConvert.SerializeObject(new { AccountNo = taxPayment.AccountNo });
                        //=================================
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccount);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get customer account balance!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            if (balData != null)
                            {
                                if (balData.Successful)
                                {
                                    decimal totalAmount = Convert.ToDecimal(result.Data5);
                                    decimal balance = balData.Balance + balData.Overdraft;

                                    if (totalAmount > balance)
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = "Insufficient customer account balance!";
                                    }
                                    else
                                    {
                                        result.Data3 = balData.AccountName;
                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = balData.Response;
                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get customer account balance return invalid data!";
                            }
                        }
                        //==================================                       
                    }
                    else if (taxPayment.PayMode == 2)
                    {
                        //----- Validate inhouse cheque
                        var jsonCheque = JsonConvert.SerializeObject(new { accountno = taxPayment.AccountNo, chqno = taxPayment.ChequeNo });
                        var cbsResp = MakeCBSRequest(result.Data6, jsonCheque);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get cheque details!";
                            return result;
                        }
                        else
                        {
                            var chqData = JsonConvert.DeserializeObject<ChequeQueryResposeModel>(cbsResp);
                            if (chqData != null)
                            {
                                if (!chqData.Valid)
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = chqData.Response;

                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get cheque details returned invalid data!";
                            }
                        }
                    }
                }
                else
                {
                    var cbs = CheckCBS(taxPayment.CBSRef, taxPayment.ExpectedAmount, taxPayment.DclntName, result.Data7, result.Data8);
                    result.RespStatus = cbs.Result.RespStatus;
                    result.RespMessage = cbs.Result.RespMessage;
                }
            }
            return result;
        }

        public async Task<GenericModel> CheckCBS(string cbsRef, decimal expectedAmount, string dclntName, string accredit, string currency)
        {
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryCBS);
            db.Reset();
            if (setting.RespStatus == 0)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                //DayOfWeek wk = DateTime.Today.DayOfWeek;
                //if (wk == DayOfWeek.Saturday)
                //{
                //    date = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
                //}
                //else if (wk == DayOfWeek.Sunday)
                //{
                //    date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                //}
                var result = db.GeneralRepository.ValidateCBSRef(setting.Data1, cbsRef, expectedAmount, dclntName, accredit, setting.Data7, date, currency);
                if (result.Rows.Count < 1)
                {
                    return new GenericModel
                    {
                        RespStatus = 1,
                        RespMessage = "The reference " + cbsRef + " does not exists in CBS!"
                    };
                }
                else
                {
                    return new GenericModel
                    {
                        RespStatus = 0,
                        RespMessage = ""
                    };
                }
            }
            return setting;
        }

        public async Task<GenericModel> MakeTaxPayment(TaxPaymentModel taxPayment, int userCode)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0",
                Data3 = "0"
            };

            Payment payment = new Payment
            {
                Amount = taxPayment.Amount,
                ModeCode = taxPayment.PayMode,
                Remarks = taxPayment.Remarks,
                UserCode = userCode,
                FileCode = taxPayment.FileCode,
                Dr_Account = taxPayment.AccountNo,
                Extra1 = taxPayment.ChequeNo,
                Extra2 = taxPayment.SortCode,
                Extra3 = taxPayment.CBSRef,
                ApplyCharge = !taxPayment.NoCharge,
                Extra4 = taxPayment.ReceivedFrom,
                TaxType = taxPayment.TaxType


            };

            var result = await db.TaxRepository.MakePaymentAsync(payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                if (result.ApprovalNeeded)
                {
                    makePaymentResponse.RespStatus = 0;
                    makePaymentResponse.RespMessage = "Payment created awaiting approval.";
                    makePaymentResponse.Data1 = taxPayment.FileCode.ToString();
                    makePaymentResponse.Data3 = "1";
                    makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                }
                else
                {
                    makePaymentResponse.Data1 = result.PaymentCode.ToString();
                    makePaymentResponse.Data2 = "1";

                    string statMessage = taxPayment.CBSRef;
                    string postRespMessage = "";
                    makePaymentResponse.RespMessage = "Transaction processed successfully.";
                    makePaymentResponse.RespStatus = 0;

                    if (taxPayment.PostToCBS == 1)
                    {
                        //---- Upload to core
                        var postResp = PostToFlex(result);
                        if (!postResp.Successful)
                        {
                            postRespMessage = postResp.Message;
                            makePaymentResponse.Data2 = "1";
                            makePaymentResponse.RespStatus = 1;
                            if (postResp.Message.StartsWith("ERROR!"))
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            else
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }

                            //---- Update payment status (4 - Failed)
                            await db.TaxRepository.UpdatePaymentStatusAsync(result.PaymentCode, 4, postResp.Message);

                            return makePaymentResponse;
                        }
                        else
                        {
                            //--- Posting successfully
                            postRespMessage = postResp.Message;
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                            statMessage = postResp.Message;
                        }
                    }

                    //---- Update payment status
                    await db.TaxRepository.UpdatePaymentStatusAsync(result.PaymentCode, 0, statMessage);

                    //---- Post to gateways
                    if (payment.TaxType == 11)
                    {
                        DomesticTaxPayment obrPayment = await db.DomesticRepository.GetDomesticTaxDataAsync(payment.FileCode);
                        var queryResult = await AddDomesticTransaction(obrPayment);
                        if (queryResult.status != 0)
                        {
                            if (queryResult.content == null)
                            {
                                queryResult.content = new PaymentResponseDetails { resId = "" };
                            }

                            await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 4, queryResult.message, queryResult.content.resId);

                            makePaymentResponse.RespStatus = 1;
                            makePaymentResponse.RespMessage = "Transaction posting to OBR  failed! " + queryResult.message;
                            return makePaymentResponse;
                        }
                        makePaymentResponse.RespStatus = 0;
                        makePaymentResponse.RespMessage = "Transaction posting was successful.";

                        //---- Update payment status
                        await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 0, queryResult.message, queryResult.content.resId);
                        await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(0, result.PaymentCode, 0, postRespMessage, queryResult.content.resId);
                    }
                    else if (payment.TaxType == 12)
                    {
                        int fileStat = 0;
                        string fileStatMessage = "";
                        string extra1 = "";
                        string extra2 = "";
                        string extra3 = "";
                        makePaymentResponse.Data1 = taxPayment.FileCode.ToString();
                        var miarieResult = await MakeMiariePaymentAsync(taxPayment, result);
                        if (!miarieResult.Success)
                        {
                            fileStat = 1;
                            fileStatMessage = miarieResult.RespMessage;
                            makePaymentResponse.RespStatus = 1;
                            makePaymentResponse.RespMessage = "Transaction posting to Miarie failed! " + miarieResult.RespMessage;
                            extra1 = miarieResult.RespMessage;
                        }
                        else
                        {
                            extra1 = miarieResult.Description;
                            //extra2= miarieResult.P
                            makePaymentResponse.RespMessage = "Transaction has been processed successfully.";
                        }

                        //---- Update status
                        await db.MiarieRepository.UpdateStatusAsync(taxPayment.FileCode, fileStat, fileStatMessage, extra1, extra2, extra3);
                    }

                    //---- Update payment status for multiple temp payment records,
                    if (payment.TaxType == 10)
                        db.TaxRepository.ArchTempDecl(taxPayment.FileCode);
                }
            }
            else
                makePaymentResponse.RespMessage = result.RespMessage;

            return makePaymentResponse;
        }

        public async Task<IEnumerable<TaxFileMiniModel>> GetTaxFiles(string dateRange, int fileStatus, string assesNo)
        {
            return await Task.Run(() =>
            {
                return db.TaxRepository.GetFiles(dateRange, fileStatus, assesNo);
            });
        }

        public async Task<IEnumerable<ReceiptReportModels>> GetTaxPayments(int usercode, string assesNo, string dateRange)
        {
            string dateFrom = "";
            string dateTo = "";
            //---- Get dates
            if (!string.IsNullOrEmpty(dateRange))
            {
                string[] dates = dateRange.Split('-');
                dateFrom = dates[0].Trim();
                dateTo = dates[1].Trim();
            }
            return await db.TaxRepository.GetTaxPaymentsAsync(usercode, assesNo, dateFrom, dateTo);
        }

        public async Task<IEnumerable<FailedTransactions>> GetFailedTransactions(int usercode, string dateRange)
        {
            return await Task.Run(() =>
            {
                string dateFrom = "";
                string dateTo = "";
                //---- Get dates
                if (!string.IsNullOrEmpty(dateRange))
                {
                    string[] dates = dateRange.Split('-');
                    dateFrom = dates[0].Trim();
                    dateTo = dates[1].Trim();
                }
                return db.SecurityRepository.GetFailedPayments(usercode, dateFrom, dateTo);
            });
        }

        public async Task<GenericModel> UpdateFailedTransactions(int usercode, int code)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.UpdateFailedTransactions(usercode, code);
            });
        }

        public async Task<IEnumerable<TokenReportModels>> GetPowerPayments(int usercode, string assesNo, string dateRange)
        {
            return await Task.Run(() =>
            {
                string dateFrom = "";
                string dateTo = "";
                //---- Get dates
                if (!string.IsNullOrEmpty(dateRange))
                {
                    string[] dates = dateRange.Split('-');
                    dateFrom = dates[0].Trim();
                    dateTo = dates[1].Trim();
                }
                return db.PaywayGatewayRepository.GetPowerPayments(usercode, assesNo, dateFrom, dateTo);
            });
        }

        public async Task<IEnumerable<AuditModel>> GetAudit(int usercode, string assesNo, string dateRange)
        {
            return await Task.Run(() =>
            {
                string dateFrom = "";
                string dateTo = "";
                //---- Get dates
                if (!string.IsNullOrEmpty(dateRange))
                {
                    string[] dates = dateRange.Split('-');
                    dateFrom = dates[0].Trim();
                    dateTo = dates[1].Trim();
                }
                return db.SecurityRepository.GetAudit(usercode, assesNo, dateFrom, dateTo);
            });
        }

        public async Task<IEnumerable<TopUpReportModels>> GetTopUpPayments(int usercode, string assesNo, string dateRange)
        {
            return await Task.Run(() =>
            {
                string dateFrom = "";
                string dateTo = "";
                //---- Get dates
                if (!string.IsNullOrEmpty(dateRange))
                {
                    string[] dates = dateRange.Split('-');
                    dateFrom = dates[0].Trim();
                    dateTo = dates[1].Trim();
                }
                return db.PaywayGatewayRepository.GetTopUpPayments(usercode, assesNo, dateFrom, dateTo);
            });
        }

        public async Task<IEnumerable<ApprovedPayments>> ApprovedPayments(int usercode = 0, string assesNo = "")
        {
            return await Task.Run(() =>
            {
                return db.TaxRepository.ApprovedPayments(usercode, assesNo);
            });
        }

        public async Task<ReceiptReportModels> GetTaxPaymentReceipt(int paymentCode)
        {
            return await Task.Run(() =>
            {
                ReceiptReportModels dataModel = new ReceiptReportModels();

                dataModel = db.TaxRepository.GetPaymentReceipt(paymentCode);
                if (dataModel.PayType == 3)
                {
                    dataModel.CreditData = db.TaxRepository.GetCreditSlipData(paymentCode);
                }
                else if (dataModel.PayType == 6 || dataModel.PayType == 7)
                {
                    dataModel.CreditData = db.TaxRepository.GetBulkData(paymentCode);
                } else if (dataModel.PayType == 9)
                {
                    dataModel.CreditData = db.TaxRepository.GetBulkOther(paymentCode);
                }

                return dataModel;
            });
        }

        public async Task<IEnumerable<TaxPaymentModel>> GetPaymentsApprovalQueue(int taxType)
        {
            return await db.TaxRepository.GetApprovalQueueAsync(taxType);
        }

        public async Task<TaxPaymentModel> GetPaymentApprovalQueueItem(int code)
        {
            return await db.TaxRepository.GetApprovalQueueItemAsync(code);
        }

        public async Task<GenericModel> SupervisePaymentItem(int paymentCode, int action, string reason, int userCode)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0"
            };

            var result = await db.TaxRepository.SupervisePaymentAsync(paymentCode, action, reason, userCode);
            if (result.RespStatus == 0)
            {
                if (action == 1)
                {
                    //------ Do the posting here

                    ////Johnson, chck if this txn is already posted to avoid duplicated posting which will update the same record as failed
                    //var dup =await CheckDuplicate(result.FileCode);
                    //if (dup.RespStatus == 0)
                    //{
                    //    makePaymentResponse.RespStatus = 0;
                    //    makePaymentResponse.RespMessage = "Transaction already posted and was successful.";
                    //    //makePaymentResponse.Data1 = result.Data4;
                    //    //makePaymentResponse.Data2 = result.Data5;
                    //    return makePaymentResponse;
                    //}

                    var postResp = PostToFlex(result);
                    if (!postResp.Successful)
                    {
                        makePaymentResponse.Data2 = "1";
                        makePaymentResponse.RespStatus = 1;
                        if (postResp.Message.StartsWith("ERROR!"))
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! >>" + postResp.Message;
                        }
                        else
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }

                        //---- Update payment status
                        await db.TaxRepository.UpdatePaymentStatusAsync(result.PaymentCode, 4, postResp.Message);
                        makePaymentResponse.RespStatus = 1;
                    }
                    else
                    {
                        //---- Update payment status
                        await db.TaxRepository.UpdatePaymentStatusAsync(result.PaymentCode, 0, postResp.Message);

                        makePaymentResponse.RespStatus = 0;
                        makePaymentResponse.RespMessage = "Transaction posting was successful.";
                        makePaymentResponse.Data1 = result.Data4;
                        makePaymentResponse.Data2 = result.Data5;

                        if (result.Data4 == "12")
                        {
                            TaxPaymentModel taxPayment = new TaxPaymentModel
                            {
                                FileCode = Convert.ToInt32(result.Data5),
                                TaxRef = result.Data6,
                                PaymentCode = paymentCode,
                                Amount = result.MainAmount,
                                ItemType = Convert.ToInt32(result.Data8)
                            };

                            int fileStat = 0;
                            string fileStatMessage = "";
                            string extra1 = "";
                            string extra2 = "";
                            string extra3 = "";

                            var miarieResult = await MakeMiariePaymentAsync(taxPayment, result);
                            if (!miarieResult.Success)
                            {
                                fileStat = 1;
                                fileStatMessage = miarieResult.RespMessage;
                                makePaymentResponse.RespStatus = 1;
                                makePaymentResponse.RespMessage = "Transaction posting to Miarie failed! " + miarieResult.RespMessage;
                                extra1 = miarieResult.RespMessage;
                            }
                            else
                            {
                                extra1 = "";// miarieResult.Description;
                                makePaymentResponse.RespMessage = "Transaction has been processed successfully.";
                            }

                            //---- Update status
                            await db.MiarieRepository.UpdateStatusAsync(taxPayment.FileCode, fileStat, fileStatMessage, extra1, extra2, extra3);
                        }
                    }
                }
                else
                {
                    makePaymentResponse.RespStatus = 3; //status 3 is a rejected txn by supervisor
                    makePaymentResponse.RespMessage = result.RespMessage;
                }
            }
            else
            {
                makePaymentResponse.RespStatus = result.RespStatus;
                makePaymentResponse.RespMessage = result.RespMessage;
            }

            //---- Update payment status for multiple temp payment records,
            db.TaxRepository.ArchTempDecl(result.FileCode);

            return makePaymentResponse;
        }

        public async Task<GenericModel> GetPayMode(int payMode)
        {
            return await db.TaxRepository.GetPayModeAsyc(payMode);
        }

        public async Task<GenericModel> GetPaymentStatusAsync(int code)
        {
            return await db.TaxRepository.GetPaymentStatusAsync(code);
        }

        public async Task<GenericModel> GetOBRStatus(int code)
        {
            return await db.TaxRepository.GetOBRStatusAsync(code);
        }

        public async Task<GenericModel> TempTaxDecl(DeclarationQueryResponse data, int mode = 0, int userCode = 0)
        {
            return await db.TaxRepository.TempTaxDeclAsync(data, mode, userCode);
        }

        public DeclarationQueryResponse GetTempTaxDets(int mode, int Code)
        {
            return db.TaxRepository.GetTempTaxDets(mode, Code);
        }

        public DeclarationQueryResponse GetTempTaxDets_uCode(int mode, int Code)
        {
            return db.TaxRepository.GetTempTaxDets_uCode(mode, Code);
        }

        public IEnumerable<DeclarationQueryResponse> Get_TempTaxDets(int mode = 0, int ucode = 0)
        {
            return db.TaxRepository.Get_TempTaxDets(mode, ucode).ToList();
        }

        public DeclarationQueryResponse GetAllTempTaxDets(int mode, int usercode)
        {
            return db.TaxRepository.GetAllTempTaxDets(mode, usercode);
        }

        public async Task<BaseEntity> UpdateFileCodeTempDecl(int Filecode, int usercode)
        {
            return await db.TaxRepository.UpdateFileCodeTempDeclAsync(Filecode, usercode);
        }

        public async Task<GenericModel> Validate_Declaration(DeclarationQueryData model, int userCode)
        {
            return await Task.Run(() =>
            {
                return db.TaxRepository.Val_Declaration(model, userCode);
            });
        }
        //public async Task<BaseEntity> ValidateDecl(DeclarationQueryData model, int userCode)
        //{
        //    return await Task.Run(() =>
        //    {
        //        return db.TaxRepository.Validate_Declaration(model,userCode);
        //    });
        //}

        #endregion

        #region PayWayGateway

        public async Task<TokenReportModels> GetPowerPaymentReceipt(int paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetPaymentReceipt(paymentCode);
            });
        }

        public async Task<TopUpReportModels> GetTopUpPaymentReceipt(int paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetTopUpPaymentReceipt(paymentCode);
            });
        }

        public async Task<IEnumerable<PowerPaymentModel>> GetPowerPaymentsApprovalQueue()
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetApprovalQueue();
            });
        }

        public async Task<PowerPaymentModel> GetPowerPaymentApprovalQueueItem(int code)
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetApprovalQueueItem(code);
            });
        }

        public async Task<GenericModel> GetDetails(PaywayType paywayType)
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetDetails(paywayType);
            });
        }

        //public async Task<GenericModel> SavePayment(PowerPaymentModel powerPayment)
        //{
        //    return await Task.Run(() =>
        //    {
        //        return db.PaywayGatewayRepository.SavePayment(powerPayment);
        //    });
        //}

        public async Task<GenericModel> GetPayMod(int payMode)
        {
            return await Task.Run(() =>
            {
                return db.PaywayGatewayRepository.GetPayMod(payMode);
            });
        }

        public async Task<GenericModel> validatePayment(PowerPaymentModel model)
        {
            return await Task.Run(() =>
            {
                var result = db.PaywayGatewayRepository.ValidatePayment(model);
                db.Reset();

                if (result.RespStatus == 0)
                {
                    //---- Validate accounts
                    int[] modes = new int[] { 1, 2 };
                    if (modes.Contains(model.PayMode))
                    {
                        var jsonAccount = JsonConvert.SerializeObject(new { AccountNo = model.AccountNo });
                        //=================================
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccount);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get customer account balance!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            if (balData != null)
                            {
                                if (balData.Successful)
                                {
                                    decimal totalAmount = Convert.ToDecimal(result.Data5);
                                    decimal balance = balData.Balance + balData.Overdraft;

                                    if (totalAmount > balance)
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = "Insufficient customer account balance!";
                                    }
                                    else
                                    {
                                        result.Data3 = balData.AccountName;
                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = balData.Response;
                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get customer account balance return invalid data!";
                            }
                        }
                        //==================================                       
                    }
                    else if (model.PayMode == 2)
                    {
                        //----- Validate inhouse cheque
                        var jsonCheque = JsonConvert.SerializeObject(new { accountno = model.AccountNo, chqno = model.ChequeNo });
                        var cbsResp = MakeCBSRequest(result.Data6, jsonCheque);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get cheque details!";
                            return result;
                        }
                        else
                        {
                            var chqData = JsonConvert.DeserializeObject<ChequeQueryResposeModel>(cbsResp);
                            if (chqData != null)
                            {
                                if (!chqData.Valid)
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = chqData.Response;

                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get cheque details returned invalid data!";
                            }
                        }
                    }
                    else if (model.PayMode == 3)
                    {
                        //----- Validate inhouse cheque
                        var jsonAccont = JsonConvert.SerializeObject(new { accountno = result.Data3 });
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccont);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get account!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            result.Data3 = balData.AccountName;

                        }
                    }
                }
                return result;
            });
        }

        public async Task<GenericModel> validateRetailerPayment(RetailerTopUpModel model)
        {
            return await Task.Run(() =>
            {
                var result = db.PaywayGatewayRepository.ValidateRetailerPayment(model);
                db.Reset();

                if (result.RespStatus == 0)
                {
                    //---- Validate accounts
                    int[] modes = new int[] { 1, 2 };
                    if (modes.Contains(model.PayMode))
                    {
                        var jsonAccount = JsonConvert.SerializeObject(new { AccountNo = model.AccountNo });
                        //=================================
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccount);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get customer account balance!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            if (balData != null)
                            {
                                if (balData.Successful)
                                {
                                    decimal totalAmount = Convert.ToDecimal(result.Data5);
                                    decimal balance = balData.Balance + balData.Overdraft;

                                    if (totalAmount > balance)
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = "Insufficient customer account balance!";
                                    }
                                    else
                                    {
                                        result.Data3 = balData.AccountName;
                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = balData.Response;
                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get customer account balance return invalid data!";
                            }
                        }
                        //==================================                       
                    }
                    else if (model.PayMode == 2)
                    {
                        //----- Validate inhouse cheque
                        var jsonCheque = JsonConvert.SerializeObject(new { accountno = model.AccountNo, chqno = model.ChequeNo });
                        var cbsResp = MakeCBSRequest(result.Data6, jsonCheque);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get cheque details!";
                            return result;
                        }
                        else
                        {
                            var chqData = JsonConvert.DeserializeObject<ChequeQueryResposeModel>(cbsResp);
                            if (chqData != null)
                            {
                                if (!chqData.Valid)
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = chqData.Response;

                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get cheque details returned invalid data!";
                            }
                        }
                    }
                    else if (model.PayMode == 3)
                    {
                        //----- Validate inhouse cheque
                        var jsonAccont = JsonConvert.SerializeObject(new { accountno = result.Data3 });
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccont);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get account!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            result.Data3 = balData.AccountName;

                        }
                    }

                }
                return result;
            });
        }




        public async Task<GenericModel> MakePowerPayment(PowerPaymentModel powerPaymentModel, int userCode)
        {
            return await Task.Run(async () =>
            {
                GenericModel makePaymentResponse = new GenericModel
                {
                    RespStatus = 1,
                    RespMessage = "Unknown response!",
                    Data1 = "0",
                    Data2 = "0",
                    Data3 = "0"
                };

                PowerPayments payment = new PowerPayments
                {
                    Amount = powerPaymentModel.Amount,
                    ModeCode = powerPaymentModel.PayMode,
                    Remarks = powerPaymentModel.Remarks,
                    UserCode = userCode,
                    FileCode = powerPaymentModel.TransactionId.ToString(),
                    Dr_Account = powerPaymentModel.AccountNo,
                    Extra1 = powerPaymentModel.ChequeNo,
                    Extra2 = powerPaymentModel.SortCode,
                    ApplyCharge = !powerPaymentModel.NoCharge,
                    Extra4 = powerPaymentModel.ReceivedFrom,
                    CustomerNo = powerPaymentModel.CustomerNo,
                    PhoneNo = powerPaymentModel.PhoneNo,
                    AccountName = powerPaymentModel.AccountName
                };

                var result = db.PaywayGatewayRepository.MakePayment(payment);
                //result.brachCode = taxPayment.brachCode;
                db.Reset();

                if (result.RespStatus == 0)
                {
                    if (result.ApprovalNeeded)
                    {
                        makePaymentResponse.RespStatus = 0;
                        makePaymentResponse.RespMessage = "Payment created awaiting approval.";
                        makePaymentResponse.Data3 = "1";
                        makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                    }
                    else
                    {
                        makePaymentResponse.Data1 = result.PaymentCode.ToString();
                        makePaymentResponse.Data2 = "1";

                        //----Post To Payway
                        var postRPayment = await PostPowersPayment(powerPaymentModel, Convert.ToInt32(result.PaymentCode));
                        if (postRPayment.stat != 0)
                        {
                            makePaymentResponse.RespStatus = 3;
                            makePaymentResponse.RespMessage = postRPayment.msg;
                            db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postRPayment.msg);
                            makePaymentResponse.RespStatus = 1;
                            return makePaymentResponse;
                        }

                        var result2 = new TaxToPostModel
                        {
                            PostUrl = result.PostUrl,
                            MainCrAccount = result.MainCrAccount,
                            MainDrAccount = result.MainDrAccount,
                            CurrencyCode = result.CurrencyCode,
                            MainFlag = result.MainFlag,
                            MainNarration = result.MainNarration,
                            CBSOfficer = result.CBSOfficer,
                            MainRefNo = result.MainRefNo,
                            TransactorName = result.TransactorName,
                            MainTxnCode = result.MainTxnCode,
                            MainTxnType = result.MainTxnType,
                            ChequeNo = result.ChequeNo,
                            BrachCode = result.BrachCode,
                            ChargeCrAccount = result.ChargeCrAccount,
                            ChargeDrAccount = result.ChargeDrAccount,
                            ChargeFlag = result.ChargeFlag,
                            ChargeNarration = result.ChargeNarration,
                            ChargeRefNo = result.ChargeRefNo,
                            ChargeTxnCode = result.ChargeTxnCode,
                            ChargeTxnType = result.ChargeTxnType,
                            MainAmount = postRPayment.MainAmount,
                            ChargeAmount = postRPayment.ChargeAmount
                        };
                        //---- Upload to core
                        var postResp = PostPowerFlex(result2);
                        if (!postResp.Successful)
                        {
                            makePaymentResponse.Data2 = "1";
                            makePaymentResponse.RespStatus = 1;
                            if (postResp.Message.StartsWith("ERROR!"))
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            else
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            //---- Update payment status
                            //txn failed update status to failure 
                            makePaymentResponse.RespStatus = 4;
                            db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message);
                            makePaymentResponse.RespStatus = 1;
                        }
                        else
                        {
                            PowerPaymentModel serviceProvider = new PowerPaymentModel();
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                            //---- Update payment status
                            var update = db.PaywayGatewayRepository.UpdatePaymentStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message);



                        }
                        //---- Update payment status for multiple temp payment records,
                        // db.TaxRepository.ArchTempDecl(taxPayment.FileCode);

                    }
                }
                else
                    makePaymentResponse.RespMessage = result.RespMessage;

                return makePaymentResponse;
            });
        }

        private async Task<PowerTokenData> PostPowersPayment(PowerPaymentModel powerPaymentModel, int paymentCode)
        {
            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await GetDetails(PaywayType.BuyToken);



            var validateretalier = await Task.Run((Func<PowerTokenData>)(() =>
            {
                try
                {
                    PowerTokenData resp = new PowerTokenData();
                    string result = "";
                    RetailerTopUp js = new RetailerTopUp
                    {
                        AppID = res.Data3.ToString(),
                        AppToken = res.Data4.ToString(),
                        Password = res.Data2.ToString(),
                        Username = res.Data1.ToString(),
                        ProviderId = res.Data5.ToString(),
                        CustomerNumber = powerPaymentModel.CustomerNumber.ToString(),
                        Amount = powerPaymentModel.Amount.ToString("0"),
                        ContactInfo = powerPaymentModel.ContactInfo.ToString(),
                        TxId = powerPaymentModel.TransactionId.ToString()
                    };

                    string json = JsonConvert.SerializeObject(js);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(res.Data7.ToString() + "BuyToken");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string jso = json;

                        streamWriter.Write(jso);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var reply = streamReader.ReadToEnd();
                        byte[] response = Encoding.UTF8.GetBytes(reply);
                        result = Encoding.UTF8.GetString(response);
                        string result1 = result.Replace(@"\", string.Empty);
                        string final = result1.Trim().Substring(1, (result1.Length) - 2);

                        PowerTokenData powerTokenData = JsonConvert.DeserializeObject<PowerTokenData>(result);

                        PowerToken rep = powerTokenData.content;

                        if (powerTokenData.stat == 0)
                        {
                            string Totalvalue = rep.TotalValue;
                            string acceptedamount = Totalvalue.Substring(0, Totalvalue.Length - 3);

                            if (rep.Vat == null)
                            {
                                rep.Vat = "";
                            }

                            var powerToken = new PowerToPost
                            {
                                ReceiptNo = rep.TxnId.ToString(),
                                ExtInfoId = rep.ExtInfo.ToString(),
                                TokenNo = rep.TokenNumber.ToString(),
                                ContactInfoNo = js.ContactInfo.ToString(),
                                CustomerNo = js.CustomerNumber.ToString(),
                                Amount = Convert.ToDecimal(acceptedamount),
                                MeterNo = rep.MeterNo.ToString(),
                                ConsumerName = rep.ConsumerName.ToString(),
                                TotalUnits = rep.TotalUnits.ToString(),
                                TokenValue = rep.TokeValue.ToString(),
                                Status = rep.TokenStatus.ToString(),
                                KwhValue = rep.KwhValue.ToString(),
                                Vat = rep.Vat.ToString()
                            };
                            var user = db.PaywayGatewayRepository.SavePowerPayment(powerToken, paymentCode);

                            PowerToCBSPost powerToCBSPost = new PowerToCBSPost
                            {
                                ChargeAmount = user.Data18,
                                MainAmount = user.Data19
                            };

                            resp = new PowerTokenData
                            {
                                stat = user.RespStatus,
                                msg = user.RespMessage,
                                MainAmount = user.Data19,
                                ChargeAmount = user.Data18
                            };
                            return resp;
                        }
                        else
                        {
                            resp = new PowerTokenData
                            {
                                stat = powerTokenData.stat,
                                msg = powerTokenData.msg
                            };
                            return resp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new PowerTokenData
                    {
                        stat = 1,
                        msg = ex.ToString(),
                    };
                }
            }));

            PowerTokenData arr = new PowerTokenData
            {
                stat = validateretalier.stat,
                msg = validateretalier.msg,
                MainAmount = validateretalier.MainAmount,
                ChargeAmount = validateretalier.ChargeAmount
            };
            return arr;
        }

        public async Task<GenericModel> RetailerTopUp(RetailerTopUpModel retailerTopUpModel, int userCode)
        {
            return await Task.Run(async () =>
            {
                GenericModel makePaymentResponse = new GenericModel
                {
                    RespStatus = 1,
                    RespMessage = "Unknown response!",
                    Data1 = "0",
                    Data2 = "0",
                    Data3 = "0"
                };

                RetailerPayments payment = new RetailerPayments
                {
                    Amount = retailerTopUpModel.Amount,
                    ModeCode = retailerTopUpModel.PayMode,
                    Remarks = retailerTopUpModel.Remarks,
                    UserCode = userCode,
                    FileCode = retailerTopUpModel.TransactionId.ToString(),
                    Dr_Account = retailerTopUpModel.AccountNo,
                    Extra1 = retailerTopUpModel.ChequeNo,
                    Extra2 = retailerTopUpModel.SortCode,
                    ApplyCharge = !retailerTopUpModel.NoCharge,
                    Extra4 = retailerTopUpModel.ReceivedFrom,
                    CustomerNo = retailerTopUpModel.CustomerNo,
                    PhoneNo = retailerTopUpModel.PhoneNo,
                    AccountName = retailerTopUpModel.AccountName
                };

                var result = db.PaywayGatewayRepository.RMakePayment(payment);
                //result.brachCode = taxPayment.brachCode;
                db.Reset();

                if (result.RespStatus == 0)
                {
                    if (result.ApprovalNeeded)
                    {
                        makePaymentResponse.RespStatus = 0;
                        makePaymentResponse.RespMessage = "Payment created awaiting approval.";
                        makePaymentResponse.Data3 = "1";
                        makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                    }
                    else
                    {
                        makePaymentResponse.Data1 = result.PaymentCode.ToString();
                        makePaymentResponse.Data2 = "1";

                        var postRPayment = await PostRetailerPayment(retailerTopUpModel);
                        string TopUpStatus = "";
                        string TopUpNarration = "";
                        string TopUpTxid = "";
                        if (postRPayment.stat == 0)
                        {
                            TopUpStatus = postRPayment.content.status;
                            TopUpNarration = postRPayment.content.Name;
                            TopUpTxid = postRPayment.content.outTxId;
                        }
                        else
                        {
                            //TopUpStatus = postRPayment.content.status;
                            // TopUpNarration = postRPayment.msg;
                            // TopUpTxid = postRPayment.content.outTxId;
                            makePaymentResponse.RespStatus = 3;
                            makePaymentResponse.RespMessage = postRPayment.msg;
                            db.PaywayGatewayRepository.UpdateTopUpStatus(result.PaymentCode, makePaymentResponse.RespStatus, postRPayment.msg, TopUpStatus, TopUpNarration, TopUpTxid);
                            makePaymentResponse.RespStatus = 1;
                            return makePaymentResponse;
                        }
                        //---- Upload to core
                        var postResp = PostToFlex(result);
                        if (!postResp.Successful)
                        {
                            makePaymentResponse.Data2 = "1";
                            makePaymentResponse.RespStatus = 1;
                            if (postResp.Message.StartsWith("ERROR!"))
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            else
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            //---- Update payment status
                            //txn failed update status to failure 

                            db.PaywayGatewayRepository.UpdateTopUpStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message, TopUpStatus, TopUpNarration, TopUpTxid);
                            makePaymentResponse.RespStatus = 1;
                        }
                        else
                        {

                            //---- Update payment status
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";// + //postRPayment.msg;


                            //var topup = PostRetailerPayment(new RetailerTopUpModel
                            //{
                            //    TransactionId=result.TransactionId,
                            //    ContactInfo=result.ContactInfo,
                            //    Amount=result.ChargeAmount+result.MainAmount,
                            //    CustomerNo=result.CustomerNo
                            //});

                            //  serviceProvider.Valid = Convert.ToBoolean(postRPayment.ElementAt(0));



                            db.PaywayGatewayRepository.UpdateTopUpStatus(result.PaymentCode, makePaymentResponse.RespStatus, postResp.Message, TopUpStatus, TopUpNarration, TopUpTxid);
                        }
                        //---- Update payment status for multiple temp payment records,
                        // db.TaxRepository.ArchTempDecl(taxPayment.FileCode);

                    }
                }
                else
                    makePaymentResponse.RespMessage = result.RespMessage;

                return makePaymentResponse;
            });
        }

        private async Task<RetailerTopUPData> PostRetailerPayment(RetailerTopUpModel retailerTopUpModel)
        {
            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await GetDetails(PaywayType.RetailerTopUp);



            var validateretalier = await Task.Run((Func<RetailerTopUPData>)(() =>
            {
                try
                {
                    RetailerTopUPData resp = new RetailerTopUPData();
                    string result = "";
                    RetailerTopUp js = new RetailerTopUp
                    {
                        AppID = res.Data3.ToString(),
                        AppToken = res.Data4.ToString(),
                        Password = res.Data2.ToString(),
                        Username = res.Data1.ToString(),
                        ProviderId = res.Data5.ToString(),
                        CustomerNumber = retailerTopUpModel.CustomerNumber.ToString(),
                        Amount = retailerTopUpModel.Amount.ToString("0"),
                        ContactInfo = retailerTopUpModel.ContactInfo.ToString(),
                        TxId = retailerTopUpModel.TransactionId.ToString()
                    };

                    string json = JsonConvert.SerializeObject(js);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(res.Data7.ToString() + "TopUp");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string jso = json;

                        streamWriter.Write(jso);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var reply = streamReader.ReadToEnd();
                        byte[] response = Encoding.UTF8.GetBytes(reply);
                        result = Encoding.UTF8.GetString(response);
                        string result1 = result.Replace(@"\", string.Empty);
                        string final = result1.Trim().Substring(1, (result1.Length) - 2);

                        RetailerTopUPData powerTokenData = JsonConvert.DeserializeObject<RetailerTopUPData>(result);

                        RetailerTopUP rep = powerTokenData.content;

                        if (powerTokenData.stat == 0)
                        {
                            var powerToken = new RetailerTopUP
                            {
                                Id = rep.Id.ToString(),
                                Name = rep.Name.ToString(),
                                outTxId = rep.outTxId.ToString(),
                                status = rep.status.ToString()
                            };


                            resp = new RetailerTopUPData
                            {
                                stat = powerTokenData.stat,
                                content = powerToken
                            };
                            return resp;
                        }
                        else
                        {
                            resp = new RetailerTopUPData
                            {
                                stat = powerTokenData.stat,
                                msg = powerTokenData.msg
                            };
                            return resp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new RetailerTopUPData
                    {
                        stat = 1,
                        msg = ex.ToString(),
                    };
                }
            }));

            RetailerTopUPData arr = new RetailerTopUPData
            {
                stat = validateretalier.stat,
                msg = validateretalier.msg,
                content = validateretalier.content
            };
            return arr;
        }

        public async Task<List<string>> GetProvider(GetProviders req)
        {
            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await GetDetails(PaywayType.BuyToken);

            var getprovider = await Task.Run((Func<ServiceProviderDetails>)(() =>
            {
                ServiceProviderDetails resp = new ServiceProviderDetails();
                string result = "";

                try
                {

                    string json = JsonConvert.SerializeObject(req);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(res.Data7.ToString() + "GetProvider");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string jso = json;

                        streamWriter.Write(jso);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var reply = streamReader.ReadToEnd();
                        byte[] response = Encoding.UTF8.GetBytes(reply);
                        result = Encoding.UTF8.GetString(response);
                        string result1 = result.Replace(@"\", string.Empty);
                        string final = result1.Trim().Substring(1, (result1.Length) - 2);

                        ServiceProviderDetailsData serviceProviderDetailsData = JsonConvert.DeserializeObject<ServiceProviderDetailsData>(result);

                        ServiceProviderDetails rep = serviceProviderDetailsData.content;
                        if (rep.Availability == "Available")
                        {
                            resp = new ServiceProviderDetails
                            {
                                Availability = rep.Availability,
                                MinPay = rep.MinPay,
                                MaxPay = rep.MaxPay
                            };
                            return resp;
                        }
                        else
                        {
                            resp = new ServiceProviderDetails
                            {
                                Availability = serviceProviderDetailsData.msg
                            };
                            return resp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(_logFile, "Bl.GetProvider ", ex);
                    resp = new ServiceProviderDetails
                    {
                        Availability = "false",
                        MaxPay = 0,
                        MinPay = 0,
                    };
                }
                return resp;
            }));

            //ArrayList arr = new ArrayList();
            List<String> arr = new List<String>();
            arr.Add(getprovider.Availability);
            arr.Add(getprovider.MaxPay.ToString());
            arr.Add(getprovider.MinPay.ToString());
            return arr;
        }

        public async Task<List<string>> GetCustomer(GetCustomers req2)
        {
            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await GetDetails(PaywayType.BuyToken);

            var getcustomer = await Task.Run((Func<CustomerDetails>)(() =>
            {
                try
                {
                    CustomerDetails resp = new CustomerDetails();
                    string result = "";
                    GetCustomers js = new GetCustomers
                    {
                        AppID = req2.AppID.ToString(),
                        AppToken = req2.AppToken.ToString(),
                        Password = req2.Password.ToString(),
                        Username = req2.Username.ToString(),
                        ProviderId = req2.ProviderId.ToString(),
                        CustomerNumber = req2.CustomerNumber.ToString(),
                        ContactInfo = req2.ContactInfo.ToString(),
                        Amount = req2.Amount.ToString()
                    };

                    string json = JsonConvert.SerializeObject(js);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(res.Data7.ToString() + "GetCustomer");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string jso = json;

                        streamWriter.Write(jso);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var reply = streamReader.ReadToEnd();
                        byte[] response = Encoding.UTF8.GetBytes(reply);
                        result = Encoding.UTF8.GetString(response);
                        string result1 = result.Replace(@"\", string.Empty);
                        string final = result1.Trim().Substring(1, (result1.Length) - 2);

                        CustomerDetailsData customerDetailsData = JsonConvert.DeserializeObject<CustomerDetailsData>(result);

                        CustomerDetails rep = customerDetailsData.content;

                        if (rep.Valid == true)
                        {
                            resp = new CustomerDetails
                            {
                                Valid = rep.Valid,
                                Status = rep.Status,
                                Notes = rep.Notes
                            };
                            return resp;
                        }
                        else
                        {
                            resp = new CustomerDetails
                            {
                                Valid = rep.Valid,
                                Status = rep.Status,
                                Notes = rep.message
                            };
                            return resp;
                        }
                    }
                }
                catch (Exception ex)
                {

                    LogUtil.Error(_logFile, "Bl.GetCustomer ", ex);
                    return new CustomerDetails
                    {
                        Valid = false,
                        Notes = StatusEnum.SERVER_ERROR.ToString(),
                    };
                }
            }));

            List<String> arr = new List<String>();
            arr.Add(getcustomer.Valid.ToString());
            arr.Add(getcustomer.Status.ToString());
            arr.Add(getcustomer.Notes.ToString());
            return arr;
        }

        public async Task<List<string>> GetRetailer(ValidateRetailers req)
        {
            GenericModel res = new GenericModel();
            //validate if payment for the declaration has already been made for even proceeding to query
            res = await GetDetails(PaywayType.RetailerTopUp);

            var validateretalier = await Task.Run((Func<RetailerValid>)(() =>
            {
                try
                {
                    RetailerValid resp = new RetailerValid();
                    string result = "";
                    ValidateRetailers js = new ValidateRetailers
                    {
                        AppID = req.AppID.ToString(),
                        AppToken = req.AppToken.ToString(),
                        Password = req.Password.ToString(),
                        Username = req.Username.ToString(),
                        ProviderId = req.ProviderId.ToString(),
                        CustomerNo = req.CustomerNo.ToString()
                    };

                    string json = JsonConvert.SerializeObject(js);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(res.Data7.ToString() + "ValidateRetailer");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string jso = json;

                        streamWriter.Write(jso);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var reply = streamReader.ReadToEnd();
                        byte[] response = Encoding.UTF8.GetBytes(reply);
                        result = Encoding.UTF8.GetString(response);
                        string result1 = result.Replace(@"\", string.Empty);
                        string final = result1.Trim().Substring(1, (result1.Length) - 2);

                        RetailerValidData retailerValidData = JsonConvert.DeserializeObject<RetailerValidData>(result);

                        RetailerValid rep = retailerValidData.content;

                        if (rep.Valid == true)
                        {
                            resp = new RetailerValid
                            {
                                Valid = rep.Valid,
                                Notes = rep.Notes

                            };
                            return resp;
                        }
                        else
                        {
                            resp = new RetailerValid
                            {
                                Valid = retailerValidData.content.Valid,
                                Notes = retailerValidData.msg
                            };
                            return resp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(_logFile, "Bl.GetRetailer ", ex);
                    return new RetailerValid
                    {
                        Valid = false,
                        Notes = StatusEnum.SERVER_ERROR.ToString(),
                    };
                }
            }));

            List<string> arr = new List<string>();
            arr.Add(validateretalier.Valid.ToString());
            arr.Add(validateretalier.Notes.ToString());
            return arr;
        }


        #endregion

        #region Domestic Tax
        public async Task<GenericModel> ValidateDomesticDeclaration(IncomeTaxDecl model, int userCode)
        {
            return await db.DomesticRepository.ValidateDeclarationAsync(model, userCode);
        }

        public async Task<GenericModel> ConfirmDomesticPayTax(IncomeTaxDecl queryData, string Customer, int userCode)
        {
            //LookUp(queryData);
            decimal taxAmount = Convert.ToDecimal(queryData.AmountPayable);

            //DomesticTaxFiles taxFile = new DomesticTaxFiles
            //{
            //    TaxAdjustment = queryData.TaxAdjustmentCat,
            //    Service = queryData.Service,
            //    TransactionCode = queryData.DomesticTaxType,
            //    TransactionType = queryData.DomesticTaxName,
            //    UserCode = userCode,
            //    Amount = Convert.ToDecimal(queryData.AmountPayable),
            //    Tin = queryData.tin,
            //    TaxPeriod = queryData.Period,
            //    DeclNo = queryData.DeclNo,
            //    CustomerName = Customer,
            //    CommuneName = queryData.commune,
            //    Delay = queryData.DelayMajoration,
            //    Adjustment = queryData.AdjustMentType,
            //    Chasis = queryData.CarChassis,
            //    Imma = queryData.CarImma,
            //    CarOnwer = queryData.CarOwner,
            //    Contracavation = queryData.Contraviction,
            //    Document = queryData.Document,
            //    DriverName = queryData.DriverName,
            //    Education = queryData.EducationDoc,
            //    Infraction = queryData.Infraction,
            //    LicenceType = queryData.LicenceType,
            //    Copies = queryData.TotCopies,
            //    Vehicle = queryData.VehicleType,
            //    Word = queryData.Wording,
            //    Product = queryData.Product
            //};

            DomesticTaxFiles taxFile = new DomesticTaxFiles
            {
                TaxAdjustment = queryData.TaxAdjustmentCat,
                Service = queryData.Service,
                TransactionCode = queryData.DomesticTaxType,
                TransactionType = queryData.DomesticTaxName,
                UserCode = userCode,
                Amount = Convert.ToDecimal(queryData.AmountPayable),
                Tin = queryData.tin,
                TaxPeriod = queryData.Period,
                DeclNo = queryData.DeclNo,
                CustomerName = Customer,
                CommuneName = queryData.commune,
                Delay = queryData.DelayMajoration,
                Adjustment = queryData.AdjustMentType,
                Chasis = queryData.CarDetails,// queryData.CarChassis,
                Imma = queryData.CarImma,
                CarOnwer = queryData.CarOwner,
                Contracavation = queryData.Contraviction,
                Document = queryData.Document,
                DriverName = queryData.DriverName,
                Education = queryData.EducationDoc,
                Infraction = queryData.Infraction,
                LicenceType = queryData.LicenceType,
                Copies = queryData.TotCopies,
                Vehicle = queryData.VehicleType,
                Word = queryData.Wording,
                Product = queryData.Product
            };

            var result = await db.DomesticRepository.CreateFileAsync(taxFile);
            db.Reset();

            return result;
        }

        public async Task<DomesticResponse> ValidateTin(string tin)
        {
            string responseXml = "";
            var queryResp = new DomesticResponse
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.DomesticPayment);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {
                    //---- Prepare query data
                    ValidateTinRequestData qData = new ValidateTinRequestData
                    {
                        tin = tin
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = setting.Data1,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.ValidateTin,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    DomesticResponse respData = JsonConvert.DeserializeObject<DomesticResponse>(postResults);

                    return respData;
                }
            }
            else
            {
                queryResp.message = setting.RespMessage;
                return queryResp;
            }
        }
        public async Task<NIFResponse> ValidateNIF(string NIF)
        {
            string responseXml = "";
            var queryResp = new NIFResponse
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryTax);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data4))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {
                    //---- Prepare query data
                    ValidateNIFRequestData qData = new ValidateNIFRequestData
                    {
                        nif = NIF
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = setting.Data8,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.QueryNIF,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    NIFResponse respData = JsonConvert.DeserializeObject<NIFResponse>(postResults);

                    return respData;
                }
            }
            else
            {
                queryResp.message = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<DeclarantResponse> ValidateDelarant(string decCode)
        {
            string responseXml = "";
            var queryResp = new DeclarantResponse
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.QueryTax);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data4))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {
                    //---- Prepare query data
                    ValidateDecRequestData qData = new ValidateDecRequestData
                    {
                        declarant = decCode
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = setting.Data8,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.QueryDeclarant,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    DeclarantResponse respData = JsonConvert.DeserializeObject<DeclarantResponse>(postResults);

                    return respData;
                }
            }
            else
            {
                queryResp.message = setting.RespMessage;
                return queryResp;
            }
        }
        public async Task<BaseEntity> CheckObrOffice(string Office)
        {
            BaseEntity Resp = new BaseEntity
            {
                RespMessage = "Office Not Found",
                RespStatus = 1
            };
            var Res = await db.DomesticRepository.ValidateObrOffice(Office);
            return Res;
        }
        public async Task<BaseEntity> CheckDuplicate(int filecode)
        {
            BaseEntity Resp = new BaseEntity
            {
                RespMessage = "Transaction not posted",
                RespStatus = 1
            };
            var Res = await db.GeneralRepository.CheckDuplicate(filecode);
            return Res;
        }
        public async Task<DomesticQueryResponse> QueryPayment(QueryPayment model)
        {
            string responseXml = "";
            var queryResp = new DomesticQueryResponse
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.DomesticPayment);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {
                    //---- Prepare query data
                    QueryDomesticPayment qData = new QueryDomesticPayment
                    {
                        TransactionCode = model.RefNo
                    };

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = setting.Data1,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.LookUp,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);

                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    DomesticQueryResponse respData = JsonConvert.DeserializeObject<DomesticQueryResponse>(postResults);

                    return respData;
                }
            }
            else
            {
                queryResp.message = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<DomesticPaymentResponse> AddDomesticTransaction(DomesticTaxPayment queryData)
        {
            string responseXml = "";
            var queryResp = new DomesticPaymentResponse
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = await db.GeneralRepository.GetSystemSettingAsync(SettingType.DomesticPayment);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {

                    string[] data = queryData.DomesticTaxName.Split("|");
                    queryData.DomesticTaxName = data[0];
                    string others = "";
                    others = "{";

                    others = others + "\"immatriculation_number\":\"" + queryData.Imma + "\",";

                    others = others + "\"chassis_number\":\"" + queryData.Chasis + "\",";

                    others = others + "\"car_owner\":\"" + queryData.CarOnwer + "\",";

                    if (queryData.DomesticTaxName == "723340")
                    {
                        others = others + "\"diploma_type\":\"" + queryData.Education + "\",";
                    }
                    else
                    {
                        others = others + "\"diploma_type\":\"\",";
                    }

                    others = others + "\"copies\":\"" + queryData.Copies + "\",";

                    if (queryData.DomesticTaxName == "723340")
                    {
                        others = others + "\"attestation_type\":\"" + queryData.Document + "\",";
                    }
                    else if (queryData.DomesticTaxName == "724210")
                    {
                        others = others + "\"attestation_type\":\"" + queryData.Infraction + "\",";
                    }
                    else
                    {
                        others = others + "\"attestation_type\":\"\",";
                    }

                    others = others + "\"nif\":\"" + queryData.tin + "\",";

                    others = others + "\"Tax_period\":\"" + queryData.Period + "\",";

                    others = others + "\"declaration_number\":\"" + queryData.DeclNo + "\",";

                    if (!string.IsNullOrEmpty(queryData.CommuneName))
                    {
                        others = others + "\"commune_name\":\"" + queryData.CommuneName + "\",";
                    }
                    else
                    {
                        others = others + "\"commune_name\":\"" + queryData.commune + "\",";
                    }

                    others = others + "\"contravation_number\":\"" + queryData.Contracavation + "\",";

                    others = others + "\"service\":\"" + queryData.Service + "\",";

                    others = others + "\"product\":\"" + queryData.Product + "\",";

                    if (queryData.adjustment == "711120")
                    {
                        others = others + "\"tax\":\"IPR\",";
                    }
                    else if (queryData.adjustment == "714120")
                    {
                        others = others + "\"tax\":\"TVA\",";
                    }
                    else
                    {
                        others = others + "\"tax\":\"\",";
                    }
                    if (queryData.adjustment == "711120" || queryData.adjustment == "714120")
                    {
                        if (queryData.TaxAmount > 0)
                        {
                            others = others + "\"principalamount\":\"" + queryData.TaxAmount + "\",";
                        }
                    }
                    else
                    {
                        others = others + "\"principalamount\":\"\",";
                    }

                    others = others + "\"delay_Majoration\":\"" + queryData.Delay + "\",";

                    if (!string.IsNullOrEmpty(queryData.CustomerName))
                    {
                        others = others + "\"full_name\":\"" + queryData.CustomerName + "\",";
                    }
                    else if (!string.IsNullOrEmpty(queryData.DriverName))
                    {
                        others = others + "\"full_name\":\"" + queryData.DriverName + "\",";
                    }
                    else
                    {
                        others = others + "\"full_name\":\"" + queryData.ReceivedFrom + "\",";
                    }

                    others = others + "\"cardtype\":\"\"}";



                    AddTranRequestData qData = new AddTranRequestData();
                    if (!string.IsNullOrEmpty(queryData.tin))
                    {
                        qData = new AddTranRequestData
                        {
                            TransactionCode = setting.Data5 + queryData.FileCode.ToString(),
                            TransactionType = queryData.DomesticTaxName,
                            CreatedDate = DateTime.Now,
                            CustomerName = queryData.CustomerName,
                            PayerName = queryData.ReceivedFrom,
                            Amount = Convert.ToInt32(queryData.TaxAmount),
                            Tin = queryData.tin,
                            OtherFields = others
                        };
                    }
                    else
                    {
                        string cust = "";
                        if (!string.IsNullOrEmpty(queryData.DriverName))
                        {
                            cust = queryData.DriverName;
                        }
                        else
                        {
                            cust = queryData.CustomerName;
                        }
                        qData = new AddTranRequestData
                        {
                            TransactionCode = setting.Data5 + queryData.FileCode.ToString(),
                            TransactionType = queryData.DomesticTaxName,
                            CreatedDate = DateTime.Now,
                            CustomerName = cust,
                            PayerName = queryData.ReceivedFrom,
                            Amount = Convert.ToInt32(queryData.TaxAmount),
                            OtherFields = others
                        };
                    }
                    //---- Prepare query data


                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = setting.Data1,
                        UserName = setting.Data2,
                        Password = setting.Data3,
                        Function = (int)OBRFunctionType.AddTran,
                        Data = qData
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);
                    //----- Create create client
                    HttpClient httpClient = new HttpClient(setting.Data4, HttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);
                    if (string.IsNullOrEmpty(postResults))
                    {
                        throw ex;
                    }

                    DomesticPaymentResponse respData = JsonConvert.DeserializeObject<DomesticPaymentResponse>(postResults);

                    return respData;
                }
            }
            else
            {
                queryResp.message = setting.RespMessage;
                return queryResp;
            }
        }

        public async Task<GenericModel> ValidateDomesticTaxPayment(DomesticTaxPayment domesticTaxPayment)
        {
            return await Task.Run(() =>
            {
                var result = db.DomesticRepository.ValidateDomesticTaxPayment(domesticTaxPayment);
                db.Reset();

                if (domesticTaxPayment.PostToCBS == 1)//Automatic Posting
                {
                    if (result.RespStatus == 0)
                    {
                        //---- Validate accounts
                        int[] modes = new int[] { 1, 2 };
                        if (modes.Contains(domesticTaxPayment.PayMode))
                        {
                            var jsonAccount = JsonConvert.SerializeObject(new { AccountNo = domesticTaxPayment.AccountNo });
                            //=================================
                            var cbsResp = MakeCBSRequest(result.Data4, jsonAccount);
                            if (cbsResp.StartsWith("ERROR"))
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Failed to get customer account balance!";
                                return result;
                            }
                            else
                            {
                                var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                                if (balData != null)
                                {
                                    if (balData.Successful)
                                    {
                                        decimal totalAmount = Convert.ToDecimal(result.Data5);

                                        decimal balance = balData.Balance + balData.Overdraft;

                                        if (totalAmount > balance)
                                        {
                                            result.RespStatus = 1;
                                            result.RespMessage = "Insufficient customer account balance!";
                                        }
                                        else
                                        {
                                            result.Data3 = balData.AccountName;
                                        }
                                    }
                                    else
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = balData.Response;
                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = "Call to get customer account balance return invalid data!";
                                }
                            }
                            //==================================                       
                        }
                        else if (domesticTaxPayment.PayMode == 2)
                        {
                            //----- Validate inhouse cheque
                            var jsonCheque = JsonConvert.SerializeObject(new { accountno = domesticTaxPayment.AccountNo, chqno = domesticTaxPayment.ChequeNo });
                            var cbsResp = MakeCBSRequest(result.Data6, jsonCheque);
                            if (cbsResp.StartsWith("ERROR"))
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Failed to get cheque details!";
                                return result;
                            }
                            else
                            {
                                var chqData = JsonConvert.DeserializeObject<ChequeQueryResposeModel>(cbsResp);
                                if (chqData != null)
                                {
                                    if (!chqData.Valid)
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = chqData.Response;

                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = "Call to get cheque details returned invalid data!";
                                }
                            }
                        }

                    }
                }
                else
                {
                    if (result.RespStatus == 0)
                    {
                        var cbs = CheckCBS(domesticTaxPayment.CBSref, domesticTaxPayment.TaxAmount, domesticTaxPayment.DomesticTaxName, result.Data8, result.Data9);
                        result.RespStatus = cbs.Result.RespStatus;
                        result.RespMessage = cbs.Result.RespMessage;
                    }

                }

                return result;
            });
        }

        public async Task<GenericModel> MakeDomesticTaxPayment(DomesticTaxPayment taxPayment, int userCode)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0",
                Data3 = "0"
            };

            DomesticPayments payment = new DomesticPayments
            {
                Amount = taxPayment.Amount,
                ModeCode = taxPayment.PayMode,
                Remarks = taxPayment.Remarks,
                UserCode = userCode,
                FileCode = taxPayment.FileCode.ToString(),
                Dr_Account = taxPayment.AccountNo,
                Extra1 = taxPayment.ChequeNo,
                Extra2 = taxPayment.SortCode,
                ApplyCharge = !taxPayment.NoCharge,
                Extra4 = taxPayment.ReceivedFrom
            };

            var result = db.DomesticRepository.MakePayment(payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                if (taxPayment.PostToCBS == 1)//post to cbs
                {
                    if (result.ApprovalNeeded)
                    {
                        makePaymentResponse.RespStatus = 0;
                        makePaymentResponse.RespMessage = "Payment created awaiting approval.";
                        makePaymentResponse.Data3 = "1";
                        makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                    }
                    else
                    {
                        makePaymentResponse.Data1 = result.PaymentCode.ToString();
                        makePaymentResponse.Data2 = "1";
                        //---- Upload to core
                        var postResp = PostDomesticTaxToFlex(result);
                        if (!postResp.Successful)
                        {
                            makePaymentResponse.Data2 = "1";
                            makePaymentResponse.RespStatus = 1;
                            if (postResp.Message.StartsWith("ERROR!"))
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            else
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            //---- Update payment status
                            await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(0, result.PaymentCode, 4, postResp.Message, "");
                            makePaymentResponse.RespStatus = 1;
                        }
                        else
                        {
                            var queryResult = await AddDomesticTransaction(taxPayment);
                            if (queryResult.status != 0)
                            {
                                if (queryResult.content == null)
                                {
                                    queryResult.content = new PaymentResponseDetails { resId = "" };
                                }

                                await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 4, queryResult.message, queryResult.content.resId);

                                makePaymentResponse.RespStatus = 1;
                                makePaymentResponse.RespMessage = "Transaction posting to OBR  failed! " + queryResult.message;
                                return makePaymentResponse;
                            }
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";

                            //---- Update payment status
                            await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 0, queryResult.message, queryResult.content.resId);
                            await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(0, result.PaymentCode, 0, postResp.Message, queryResult.content.resId);
                        }
                    }
                }
                else if (taxPayment.PostToCBS == 0)//manual posting
                {
                    var queryResult = await AddDomesticTransaction(taxPayment);
                    if (queryResult.status != 0)
                    {
                        if (queryResult.content == null)
                        {
                            queryResult.content = new PaymentResponseDetails { resId = "" };
                        }

                        await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 4, taxPayment.CBSref, queryResult.content.resId);

                        makePaymentResponse.RespStatus = 1;
                        makePaymentResponse.RespMessage = "Transaction posting to OBR  failed! " + queryResult.message;
                        return makePaymentResponse;
                    }
                    makePaymentResponse.RespStatus = 0;
                    makePaymentResponse.RespMessage = "Transaction posting was successful.";
                    //---- Update payment status

                    await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 0, taxPayment.CBSref, queryResult.content.resId);
                    await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(0, result.PaymentCode, 0, taxPayment.CBSref, queryResult.content.resId);
                }
                //---- Update payment status for multiple temp payment records,
                //  db.DomesticRepository.ArchTempDecl(taxPayment.FileCode);

            }

            //makePaymentResponse.RespMessage = result.RespMessage;

            return makePaymentResponse;
        }

        public async Task<DomesticReportModels> GetDomesticPaymentReceipt(int paymentCode)
        {
            return await db.DomesticRepository.GetDomesticPaymentReceiptAsync(paymentCode);
        }

        public async Task<IEnumerable<DomesticTaxPayment>> GetDomesticTaxFiles()
        {
            return await Task.Run(() =>
            {
                return db.DomesticRepository.GetDomesticTaxFiles();
            });
        }

        public async Task<GenericModel> SuperviseDomesticTax(int FileCode, int action, string reason, int userCode)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0"
            };

            var result = await db.DomesticRepository.SuperviseDomesticTaxAsync(FileCode, action, reason, userCode);
            if (result.RespStatus == 0)
            {
                var taxpayment = await db.DomesticRepository.GetDomesticTaxDataAsync(FileCode);

                if (taxpayment.RespStat != 0)
                {
                    makePaymentResponse.RespStatus = taxpayment.RespStat;
                    makePaymentResponse.RespMessage = taxpayment.RespMessage;
                    return makePaymentResponse;
                }

                //------ Do the posting here
                ////Johnson, chck if this txn is already posted to avoid duplicated posting which will update the same record as failed
                //var dup = await CheckDuplicate(result.FileCode);
                //if (dup.RespStatus == 0)
                //{
                //    makePaymentResponse.RespStatus = 0;
                //    makePaymentResponse.RespMessage = "Transaction already posted and was successful.";
                //    //makePaymentResponse.Data1 = result.Data4;
                //    //makePaymentResponse.Data2 = result.Data5;
                //    return makePaymentResponse;
                //}
                var postResp = PostDomesticTaxToFlex(result);
                if (!postResp.Successful)
                {
                    makePaymentResponse.Data2 = "1";
                    makePaymentResponse.RespStatus = 1;
                    if (postResp.Message.StartsWith("ERROR!"))
                    {
                        makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! >>" + postResp.Message;
                    }
                    else
                    {
                        makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;

                    }

                    //---- Update payment status
                    await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(0, result.PaymentCode, 4, postResp.Message, "");
                    makePaymentResponse.RespStatus = 1;
                }
                else
                {
                    var queryResult = await AddDomesticTransaction(taxpayment);
                    if (queryResult.status != 0)
                    {
                        if (queryResult.content == null)
                        {
                            queryResult.content = new PaymentResponseDetails { resId = "" };
                        }

                        await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 4, queryResult.message, "");

                        makePaymentResponse.RespStatus = 1;
                        makePaymentResponse.RespMessage = "Transaction posting to OBR  failed! " + queryResult.message;
                        return makePaymentResponse;
                    }
                    else
                    {
                        await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(1, result.PaymentCode, 0, queryResult.message, queryResult.content.resId);
                    }
                    makePaymentResponse.RespStatus = 0;
                    makePaymentResponse.RespMessage = "Transaction posting was successful.";
                    makePaymentResponse.Data1 = result.PaymentCode.ToString();

                    //---- Update payment status
                    await db.DomesticRepository.UpdateDomesticPaymentStatusAsync(0, result.PaymentCode, 0, postResp.Message, queryResult.content.resId);
                }
            }
            else
            {
                makePaymentResponse.RespStatus = 2; //status 2 is a rejected txn by supervisor
                makePaymentResponse.RespMessage = result.RespMessage;
            }
            //---- Update payment status for multiple temp payment records,
            //db.TaxRepository.ArchTempDecl(result.Filecode);

            return makePaymentResponse;
        }

        #endregion

        #region Approvals
        public async Task<IEnumerable<ApprovalItemModel>> GetApprovalItems(DateTime? from = null, DateTime? to = null, int typ = 0)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.GetApprovalItems(from, to, typ);
            });
        }

        public async Task<IEnumerable<ApprovalParamsModel>> GetParamItems(DateTime? from = null, DateTime? to = null, int typ = 0)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.GetParamItems(from, to, typ);
            });
        }

        public async Task<ApprovalItemModel> GetApprovalItem(int id)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.GetApprovalItem(id);
            });
        }

        public async Task<ApprovalParamsModel> GetApprovalParams(int id)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.GetApprovalParams(id);
            });
        }

        public async Task<BaseEntity> ItemApprovalAction(int id, int action, string reason, int userCode)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.ItemApprovalAction(id, action, reason, userCode);
            });
        }

        public async Task<BaseEntity> ParamsApprovalAction(int id, int action, string reason, int userCode)
        {
            return await Task.Run(() =>
            {
                return db.SecurityRepository.ParamsApprovalAction(id, action, reason, userCode);
            });
        }
        #endregion

        #region General
        public async Task<IEnumerable<ListModel>> GetListModel(ListModelType listType)
        {
            return await Task.Run(() =>
            {
                return db.GeneralRepository.GetListModel(listType);
            });
        }

        public async Task<DBoardModel> GetDBoardData(int userCode)
        {
            return await Task.Run(() =>
            {
                return db.GeneralRepository.GetDBoardData(userCode);
            });
        }

        public async Task<GenericModel> GetCharge(int payMode)
        {
            return await Task.Run(() =>
            {
                return db.DomesticRepository.GetCharge(payMode);
            });
        }

        public async Task<IEnumerable<DomesticReportModels>> GetDomesticTaxPayments(int usercode, string assesNo, string dateRange)
        {
            string dateFrom = "";
            string dateTo = "";
            //---- Get dates
            if (!string.IsNullOrEmpty(dateRange))
            {
                string[] dates = dateRange.Split('-');
                dateFrom = dates[0].Trim();
                dateTo = dates[1].Trim();
            }
            return await db.DomesticRepository.GetDomesticTaxPaymentsAsync(usercode, assesNo, dateFrom, dateTo);
        }

        public async Task<DomesticTaxPayment> GetDomesticTaxFile(int code)
        {
            return await db.DomesticRepository.GetDomesticTaxFileAsync(code);
        }

        public async Task<IEnumerable<DomesticReportModels>> ApprovedDomesticPayments(int usercode = 0, string assesNo = "")
        {
            return await Task.Run(() =>
            {
                return db.DomesticRepository.ApprovedDomesticPayments(usercode, assesNo);
            });
        }

        public async Task<IEnumerable<DomesticTaxFileMiniModel>> GetDomesticFiles(string dateRange, int fileStatus, string assesNo)
        {
            return await Task.Run(() =>
            {
                return db.DomesticRepository.GetDomesticFiles(dateRange, fileStatus, assesNo);
            });
        }
        #endregion

        #region Banks
        public async Task<Bank> GetBank(int BankCode = 0)
        {
            return await Task.Run(() =>
            {
                var bank = db.RefRepository.GetBank(BankCode);
                return bank;
            });
        }
        public async Task<IEnumerable<Bank>> GetBanks()
        {
            return await Task.Run(() =>
            {
                var bank = db.RefRepository.GetBanks();
                return bank;
            });
        }

        public async Task<BaseEntity> CreateBank(Bank bk, int makerUser)
        {
            return await Task.Run(() =>
            {

                var result = db.RefRepository.CreateBank(bk, makerUser);
                db.Reset();

                return result;
            });
        }
        #endregion

        #region Branches
        public async Task<IEnumerable<Branch>> GetBranches()
        {
            return await Task.Run(() =>
            {
                var user = db.RefRepository.GetBranches();
                return user;
            });
        }

        public async Task<BaseEntity> CreateBranch(Branch branch, int makerUser)
        {
            return await Task.Run(() =>
            {

                var result = db.RefRepository.CreateBranch(branch, makerUser);
                db.Reset();

                return result;
            });
        }
        #endregion

        #region Payment modes
        public async Task<IEnumerable<PaymentMode>> GetPaymentModes()
        {
            return await Task.Run(() =>
            {
                var user = db.RefRepository.GetPaymentModes();
                return user;
            });
        }
        #endregion

        #region Reports
        public async Task<IEnumerable<ReportModel>> GetReports()
        {
            return await Task.Run(() =>
            {
                return db.ReportRepository.GetReports();
            });
        }
        public ReportDataModel Process_ReportData(ReportGenModel model)
        {
            ReportDataModel dataModel = new ReportDataModel();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;

            //---- Get dates
            //string[] dates = model.dateRange.Split('-');
            dateFrom = Convert.ToDateTime(model.datefrom);
            dateTo = Convert.ToDateTime(model.dateto);

            //---- Get report header
            var header = db.ReportRepository.GetReportHeader(model.ReportCode);

            dataModel.ReportName = header.ReportName;
            dataModel.Title = header.Title;
            dataModel.DateRange = string.Format("{0:dd/MM/yyyy} to {1:dd/MM/yyyy}", dateFrom, dateTo);

            var data = db.ReportRepository.GetReportData(model.ReportCode, dateFrom, dateTo, model.branch, model.user);
            dataModel.DataList = data;

            return dataModel;
        }

        public async Task<ReportDataModel> ProcessReportData(ReportGenModel model)
        {
            return await Task.Run(() =>
            {
                ReportDataModel dataModel = new ReportDataModel();
                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;

                //---- Get dates
                //string[] dates = model.dateRange.Split('-');
                dateFrom = Convert.ToDateTime(model.datefrom);
                dateTo = Convert.ToDateTime(model.dateto);

                //---- Get report header
                var header = db.ReportRepository.GetReportHeader(model.ReportCode);

                dataModel.ReportName = header.ReportName;
                dataModel.Title = header.Title;
                dataModel.DateRange = string.Format("{0:dd/MM/yyyy} to {1:dd/MM/yyyy}", dateFrom, dateTo);

                var data = db.ReportRepository.GetReportData(model.ReportCode, dateFrom, dateTo, model.branch, model.user);
                dataModel.DataList = data;

                return dataModel;
            });
        }


        public async Task<ReportDataModel> ProcessObrReportData(ReportGenModel model)
        {
            return await Task.Run(() =>
            {
                ReportDataModel dataModel = new ReportDataModel();
                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;

                //---- Get dates
                //string[] dates = model.dateRange.Split('-');
                dateFrom = Convert.ToDateTime(model.datefrom);
                dateTo = Convert.ToDateTime(model.dateto);

                //---- Get report header
                var header = db.ReportRepository.GetReportHeader(model.ReportCode);

                dataModel.ReportName = header.ReportName;
                dataModel.Title = header.Title;
                dataModel.DateRange = string.Format("{0:dd/MM/yyyy} to {1:dd/MM/yyyy}", dateFrom, dateTo);

                var data = db.ReportRepository.GetObrReportData(model.ReportCode, dateFrom, dateTo, model.office);
                dataModel.DataList = data;

                return dataModel;
            });
        }

        #endregion

        #region Regideso postpay
        public async Task<RegidesoModel> QueryBillsAsync(RegidesoModel model)
        {
            RegidesoModel resp = new RegidesoModel();
            

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
            if (data.RespStatus != 0)
            {
                resp.RespMessage = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetFinBridgeAuthAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.RespMessage = authData.ErrorMsg;
                return resp;
            }

           // ---Create Mairie gateway
            var regideso = finBridge.CreateRegidesoService(authData.Token);
            return await regideso.GetPaymentBillsAsync(model.Accnt_no);
        }
        public async Task<Bills> ConfrimBillsAsync(Bills model)
        {
            Bills resp = new Bills();
            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
            if (data.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }

            var data1 = await db.RegidesoRepository.CreatePostPaymentAsync(model);
            if (data1.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }
            var data2 = await ValidateRegidesoPostBillPayment(model);
            if (data2.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }
            model.txnId = data1.Data1;

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.Msg = authData.ErrorMsg;
                return resp;
            }
            var payResult = await PostPayAsync(model);
            if (payResult.RespStatus != 0)
            {
                resp.Msg = payResult.RespMessage;
                return resp;
            }

            //---Create Mairie gateway
            var regideso = finBridge.CreateRegidesoService(authData.Token);
            return await regideso.PayBillsAsync(model.Invoice_no,model.txnId,model.PhoneNo);
        }

        public async Task<GenericModel> ValidateRegidesoPostBillPayment(Bills Payment)
        {
            var result = await db.RegidesoRepository.ValidatePostBillPaymentAsync(Payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
               //post to cbs
                {
                    //---- Validate accounts
                    if (Payment.PayMode == 1)
                    {
                        var jsonAccount = JsonConvert.SerializeObject(new { AccountNo = Payment.Accnt_no });
                        //=================================
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccount);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get customer account balance!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            if (balData != null)
                            {
                                if (balData.Successful)
                                {
                                    decimal totalAmount = Convert.ToDecimal(result.Data5);
                                    decimal balance = balData.Balance + balData.Overdraft;

                                    if (totalAmount > balance)
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = "Insufficient customer account balance!";
                                    }
                                    else
                                    {
                                        result.Data3 = balData.AccountName;
                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = balData.Response;
                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get customer account balance return invalid data!";
                            }
                        }
                        //==================================                       
                    }
                    else if (Payment.PayMode == 2)
                    {
                        //----- Validate inhouse cheque
                        var jsonCheque = JsonConvert.SerializeObject(new { accountno = Payment.Accnt_no, chqno = Payment.ChequeNo });
                        var cbsResp = MakeCBSRequest(result.Data6, jsonCheque);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get cheque details!";
                            return result;
                        }
                        else
                        {
                            var chqData = JsonConvert.DeserializeObject<ChequeQueryResposeModel>(cbsResp);
                            if (chqData != null)
                            {
                                if (!chqData.Valid)
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = chqData.Response;

                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get cheque details returned invalid data!";
                            }
                        }
                    }
                }
               
            }
            return result;
        }

        public async Task<GenericModel> PostPayAsync(Bills Payment)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0",
                Data3 = "0"
            };
            var data1 = await db.RegidesoRepository.CreatePostPaymentAsync(Payment);
            if (data1.RespStatus != 0)
            {
                makePaymentResponse.RespMessage = data1.RespMessage;
                return makePaymentResponse;
            }
            Payment.BillCode = data1.Data20;

            var result = await db.RegidesoRepository.MakePostPaymentAsync(Payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                if (result.ApprovalNeeded)
                {
                    await db.RegidesoRepository.UpdatePostPaymentStatusAsync(0, result.PaymentCode, 0,"","","", "");
                    makePaymentResponse.RespStatus = 0;
                    makePaymentResponse.RespMessage = "Payment created awaiting approval.";
                    makePaymentResponse.Data1 = Payment.Cust_name;
                    makePaymentResponse.Data3 = "1";
                    makePaymentResponse.Data2 = Payment.Invoice_no;
                    makePaymentResponse.Data18 = Payment.Amnt;
                    makePaymentResponse.Data19 = Payment.Amnt;
                    makePaymentResponse.Data21 = result.ApprovalNeeded;
                    makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                }
                else
                {
                    makePaymentResponse.Data1 = result.PaymentCode.ToString();
                    makePaymentResponse.Data2 = "1";
                    makePaymentResponse.Data21 = result.ApprovalNeeded;
                    string statMessage = Payment.CBSRef;
                    string postRespMessage = "";
                    makePaymentResponse.RespMessage = "Transaction processed successfully.";
                    makePaymentResponse.RespStatus = 0;

                    //---- Upload to core
                    var postResp = RGPostToFlex(result);
                    if (!postResp.Successful)
                    {
                        postRespMessage = postResp.Message;
                        makePaymentResponse.Data2 = "1";
                        makePaymentResponse.RespStatus = 1;
                        makePaymentResponse.Data21 = result.ApprovalNeeded;
                        if (postResp.Message.StartsWith("ERROR!"))
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }
                        else
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }

                        //---- Update payment status (4 - Failed)
                        await db.RegidesoRepository.UpdatePostPaymentStatusAsync(1, result.PaymentCode, 4,"1", postResp.CBSRefNo, postResp.Message, "");

                        return makePaymentResponse;
                    }
                    else
                    {
                        //---- Update payment status (4 - Failed)
                        var r = await db.RegidesoRepository.UpdatePostPaymentStatusAsync(1, result.PaymentCode,3, "2", postResp.CBSRefNo, postResp.Message, "");
                        var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
                        if (data.RespStatus != 0)
                        {
                            makePaymentResponse.RespMessage = data.RespMessage;
                            return makePaymentResponse;
                        }

                        //--- Get auth token
                        var finBridge = new FinBridgeGateway(data.Data1, _logFile);
                        var authData = await finBridge.GetFinBridgeAuthAsync(data.Data2, data.Data3);
                        if (authData.ErrorCode != 0)
                        {
                            makePaymentResponse.RespMessage = authData.ErrorMsg;
                            return makePaymentResponse;
                        }
                        var regideso = finBridge.CreateRegidesoService(authData.Token);
                        var resp = await regideso.PayBillsAsync(Payment.Invoice_no, data1.Data20.ToString(), Payment.PhoneNo);
                        if(resp.Success)
                        {
                            //--- Posting successfully
                            await db.RegidesoRepository.UpdatePostPaymentStatusAsync(2, result.PaymentCode, 1, "", "","",resp.Msg);
                            postRespMessage = postResp.Message;
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                            statMessage = postResp.Message;
                            makePaymentResponse.Data21 = result.ApprovalNeeded;
                            makePaymentResponse.Data1 = resp.ClientName;
                            makePaymentResponse.Data18 = resp.invoice_amnt;
                            makePaymentResponse.Data19 = resp.deduction_amnt;
                            makePaymentResponse.Data2 = Payment.Invoice_no;
                            makePaymentResponse.Data20 = Payment.BillCode;

                        }
                        else
                        {
                            //--- Posting failed
                            await db.RegidesoRepository.UpdatePostPaymentStatusAsync(2, result.PaymentCode, 4, "", "", "", resp.Msg);

                            postRespMessage = resp.Msg;
                            makePaymentResponse.RespStatus = 1;
                            makePaymentResponse.RespMessage = "Transaction posting Failed .";
                            statMessage = postResp.Message;
                            makePaymentResponse.Data21 = result.ApprovalNeeded;
                            makePaymentResponse.Data1 = resp.ClientName;
                            makePaymentResponse.Data18 = resp.invoice_amnt;
                            makePaymentResponse.Data19 = resp.deduction_amnt;
                            makePaymentResponse.Data2 = Payment.Invoice_no;
                            makePaymentResponse.Data20 = Payment.BillCode;

                        }
                       

                    }

                }
            }
            else
                makePaymentResponse.RespMessage = result.RespMessage;

            return makePaymentResponse;
        }

        public async Task<GenericModel> ApprovePostPayAsync(int FileCode, int action, string reason, int userCode)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0",
                Data3 = "0"
            };
            var payment =  db.RegidesoRepository.GetPostPay(FileCode);
            if (payment == null)
            {
                makePaymentResponse.RespStatus = 1;
                makePaymentResponse.RespMessage = "An error occured while fetching transaction";
                return makePaymentResponse;
            }
            payment.Maker = userCode;
            payment.txnId = payment.BillCode.ToString();
            var result = await db.RegidesoRepository.MakeApprovalPostPaymentAsync(payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                if (action == 0)
                {
                    var res1 = await db.RegidesoRepository.UpdatePostPaymentStatusAsync(0, result.PaymentCode, 2, "", "", "", "");
                    makePaymentResponse.RespStatus = res1.RespStatus;
                    makePaymentResponse.RespMessage = "Payment Rejected Successifully.";
                    makePaymentResponse.Data1 = payment.BillCode.ToString();
                    makePaymentResponse.Data3 = "1";
                    makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                }
                else
                {
                    makePaymentResponse.Data1 = result.PaymentCode.ToString();
                    makePaymentResponse.Data2 = "1";

                    string statMessage = payment.CBSRef;
                    string postRespMessage = "";
                    makePaymentResponse.RespMessage = "Transaction processed successfully.";
                    makePaymentResponse.RespStatus = 0;

                    //---- Upload to core
                    var postResp = RGPostToFlex(result);
                    if (!postResp.Successful)
                    {
                        postRespMessage = postResp.Message;
                        makePaymentResponse.Data2 = "1";
                        makePaymentResponse.RespStatus = 1;
                        if (postResp.Message.StartsWith("ERROR!"))
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }
                        else
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }

                        //---- Update payment status (4 - Failed)
                        await db.RegidesoRepository.UpdatePostPaymentStatusAsync(1, result.PaymentCode, 4, "1", "", postResp.Message, "");

                        return makePaymentResponse;
                    }
                    else
                    {
                        await db.RegidesoRepository.UpdatePostPaymentStatusAsync(1, result.PaymentCode, 1, "2", postResp.CBSRefNo, postResp.Message, "");
                        var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
                        if (data.RespStatus != 0)
                        {
                            makePaymentResponse.RespMessage = data.RespMessage;
                            return makePaymentResponse;
                        }

                        //--- Get auth token
                        var finBridge = new FinBridgeGateway(data.Data1, _logFile);
                        var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
                        if (authData.ErrorCode != 0)
                        {
                            makePaymentResponse.RespMessage = authData.ErrorMsg;
                            return makePaymentResponse;
                        }
                        var regideso = finBridge.CreateRegidesoService(authData.Token);
                        var resp = await regideso.PayBillsAsync(payment.InvoiceNo, payment.txnId, payment.PhoneNo);
                        if (resp.Success)
                        {
                            //--- Posting successfully
                            await db.RegidesoRepository.UpdatePostPaymentStatusAsync(2, result.PaymentCode, 1, "", "", "", resp.Msg);
                            //--- Posting successfully
                            postRespMessage = postResp.Message;
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                            statMessage = postResp.Message;

                        }
                        else
                        {
                            //--- Posting failed
                            await db.RegidesoRepository.UpdatePostPaymentStatusAsync(2, result.PaymentCode, 4,"", "", "",resp.Msg);
                            //--- Posting failed
                            postRespMessage = postResp.Message;
                            makePaymentResponse.RespStatus = 1;
                            makePaymentResponse.RespMessage = "Transaction posting failed due to an error.";
                            statMessage = postResp.Message;

                        }

                    }
                }
            }
            else
                makePaymentResponse.RespMessage = result.RespMessage;

            return makePaymentResponse;
        }

        #endregion
        #region Regideso prepay

        public async Task<GenericModel> CreatePrePaymentAsync(PrePaidModel model)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.CreatePrePaymentAsync(model);
            });
        }

        public async Task<QueryDetails> QueryTokenAsync(decimal amount,string meterno)
        {
            QueryDetails resp = new QueryDetails();

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
            if (data.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.Msg = authData.ErrorMsg;
                return resp;
            }
            //--- Create  gateway
            var regideso = finBridge.CreateRegidesoService(authData.Token);
            var res = await regideso.RequestTokenAsync(amount, meterno);
            return res;
        }
        public async Task<QueryDetails> QueryAmountAsync(int Token, string meterno)
        {
            QueryDetails resp = new QueryDetails();

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
            if (data.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.Msg = authData.ErrorMsg;
                return resp;
            }
            //--- Create  gateway
            var regideso = finBridge.CreateRegidesoService(authData.Token);
            var res = await regideso.RequestAmountAsync(Token, meterno);
            return res;
        }
        public async Task<PrePaidModel> QueryCustDetAsync(PrePaidModel model)
        {
            PrePaidModel resp = new PrePaidModel();

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
            if (data.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.Msg = authData.ErrorMsg;
                return resp;
            }
            //--- Create  gateway
            var regideso = finBridge.CreateRegidesoService(authData.Token);
            var res = await regideso.RequestPrePayDetailsAsync(model);
            return res;
        }
        public async Task<PrePaidModel> QueryPrePayAsync(PrePaidModel model)
        {
            PrePaidModel resp = new PrePaidModel();

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
            if (data.RespStatus != 0)
            {
                resp.Msg = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.Msg = authData.ErrorMsg;
                return resp;
            }
            //make a cbs request
            var payResult = await BuyTokenAsync(model);
            if (payResult.RespStatus != 0)
            {
                resp.Msg = payResult.RespMessage;
                return resp;
            }
                //--- Create  gateway
            var regideso = finBridge.CreateRegidesoService(authData.Token);
            var res = await regideso.RequestPrePayAsync(model);
            return res;
        }
        public async Task<IEnumerable<PostPayReportModels>> GetPayBillListPayments(int stat)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPayBillListPayments(stat);
            });
        }
        public async Task<IEnumerable<BuyTokenReportModels>> GetPrePayListPayments(int stat)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPrePayListPayments(stat);
            });
        }
        public async Task<IEnumerable<PostPayReportModels>> GetPayBillList(int stat)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPayBillListPayments(stat);
            });
        }
        public async Task<IEnumerable<PostPayReportModels>> GetPayBillApprovalList(int stat)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPostPayApprovalList(stat);
            });
        }
        public async Task<IEnumerable<BuyTokenReportModels>> GetPrePayApprovalList(int stat)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPrePayApprovalList(stat);
            });
        }
        public async Task<IEnumerable<BuyTokenReportModels>> GetPrePayList(int stat)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPrePayListPayments(stat);
            });
        }

        
        public async Task<BuyTokenReportModels> GetPrePayReceipt(int paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPrePayReceipt(paymentCode);
            });
        }
        public async Task<PostPayReportModels> GetPostPayReceipt(string paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPostPayReceipt(paymentCode);
            });
        }
        public async Task<PrePaidModel> GetPrePay(int paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPrePay(paymentCode);
            });
        }
        public async Task<PostPayReportModels> GetPostPay(int paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.RegidesoRepository.GetPostPay(paymentCode);
            });
        }
        public async Task<GenericModel> ValidateRegidesoPreBillPayment(PrePaidModel Payment)
        {
            var result = await db.RegidesoRepository.ValidateBillPaymentAsync(Payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                //post to cbs
                {
                    //---- Validate accounts
                    if (Payment.PayMode == 1)
                    {
                        var jsonAccount = JsonConvert.SerializeObject(new { AccountNo = Payment.Accnt_no });
                        //=================================
                        var cbsResp = MakeCBSRequest(result.Data4, jsonAccount);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get customer account balance!";
                            return result;
                        }
                        else
                        {
                            var balData = JsonConvert.DeserializeObject<BalanceQueryResposeModel>(cbsResp);
                            if (balData != null)
                            {
                                if (balData.Successful)
                                {
                                    decimal totalAmount = Convert.ToDecimal(result.Data5);
                                    decimal balance = balData.Balance + balData.Overdraft;

                                    if (totalAmount > balance)
                                    {
                                        result.RespStatus = 1;
                                        result.RespMessage = "Insufficient customer account balance!";
                                    }
                                    else
                                    {
                                        result.Data3 = balData.AccountName;
                                    }
                                }
                                else
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = balData.Response;
                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get customer account balance return invalid data!";
                            }
                        }
                        //==================================                       
                    }
                    else if (Payment.PayMode == 2)
                    {
                        //----- Validate inhouse cheque
                        var jsonCheque = JsonConvert.SerializeObject(new { accountno = Payment.Accnt_no, chqno = Payment.ChequeNo });
                        var cbsResp = MakeCBSRequest(result.Data6, jsonCheque);
                        if (cbsResp.StartsWith("ERROR"))
                        {
                            result.RespStatus = 1;
                            result.RespMessage = "Failed to get cheque details!";
                            return result;
                        }
                        else
                        {
                            var chqData = JsonConvert.DeserializeObject<ChequeQueryResposeModel>(cbsResp);
                            if (chqData != null)
                            {
                                if (!chqData.Valid)
                                {
                                    result.RespStatus = 1;
                                    result.RespMessage = chqData.Response;

                                }
                            }
                            else
                            {
                                result.RespStatus = 1;
                                result.RespMessage = "Call to get cheque details returned invalid data!";
                            }
                        }
                    }
                }

            }
            return result;
        }

        public async Task<GenericModel> BuyTokenAsync(PrePaidModel Payment)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0",
                Data3 = "0"
            };

            var prepaid =  db.RegidesoRepository.GetPrePay(Payment.BillCode);
            db.Reset();
            var result = await db.RegidesoRepository.MakePrePaymentAsync(Payment);
            db.Reset();
            Payment.Amnt = result.MainAmount;

            if (result.RespStatus == 0)
            {
                if (result.ApprovalNeeded)
                {
                    await db.RegidesoRepository.UpdatePrePaymentStatusAsync(0,result.PaymentCode, 0, "", "Payment created awaiting approval.","","");
                    makePaymentResponse.RespStatus = 0;
                    makePaymentResponse.RespMessage = "Payment created awaiting approval.";
                    makePaymentResponse.Data1 = Payment.BillCode.ToString();
                    makePaymentResponse.Data3 = "1";
                    makePaymentResponse.Data2 = prepaid.Meter_No;
                    makePaymentResponse.Data21 = result.ApprovalNeeded;
                    makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                }
                else
                {
                    makePaymentResponse.Data1 = result.PaymentCode.ToString();
                    makePaymentResponse.Data2 = "1";
                    makePaymentResponse.Data21 = result.ApprovalNeeded;
                    string statMessage = Payment.CBSRef;
                    string postRespMessage = "";
                    makePaymentResponse.RespMessage = "Transaction processed successfully.";
                    makePaymentResponse.RespStatus = 0;

                        //---- Upload to core
                        var postResp = RGPostToFlex(result);
                        if (!postResp.Successful)
                        {
                            postRespMessage = postResp.Message;
                            makePaymentResponse.Data2 = "1";
                            makePaymentResponse.RespStatus = 1;
                        makePaymentResponse.Data21 = result.ApprovalNeeded;
                        if (postResp.Message.StartsWith("ERROR!"))
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }
                            else
                            {
                                makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                            }

                            //---- Update payment status (4 - Failed)
                            await db.RegidesoRepository.UpdatePrePaymentStatusAsync(1,result.PaymentCode, 4, "1", "",postResp.Message,"");

                            return makePaymentResponse;
                        }
                        else
                        {
                        //---- Update payment status (cbsref and message)
                        await db.RegidesoRepository.UpdatePrePaymentStatusAsync(1, result.PaymentCode, 4, "2", postResp.CBSRefNo, postResp.Message, "");
                        var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
                                if (data.RespStatus != 0)
                                {
                                    makePaymentResponse.RespMessage = data.RespMessage;
                                    return makePaymentResponse;
                                }

                                //--- Get auth token
                                var finBridge = new FinBridgeGateway(data.Data1, _logFile);
                                var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
                                if (authData.ErrorCode != 0)
                                {
                                    makePaymentResponse.RespMessage = authData.ErrorMsg;
                                    return makePaymentResponse;
                                }
                                var regideso = finBridge.CreateRegidesoService(authData.Token);
                                var res = await regideso.RequestPrePayAsync(prepaid);
                                    res.Maker = prepaid.Maker;
                                    res.BillCode = result.PaymentCode;
                                    res.Stat = 1;
                                    res.Meter_No = prepaid.Meter_No;
                            if(res.Success)
                            {
                                var post = await db.RegidesoRepository.UpdatePrePayment(res);
                                postRespMessage = postResp.Message;
                                makePaymentResponse.RespStatus = 0;
                                makePaymentResponse.Data21 = result.ApprovalNeeded;
                                makePaymentResponse.RespMessage = "Transaction posting was successful.";
                                statMessage = postResp.Message;
                                makePaymentResponse.Data1 = res.Token3;
                                makePaymentResponse.Data19 = res.Consumption;
                                makePaymentResponse.Data20 = Payment.BillCode;
                            }
                            else
                            {
                            await db.RegidesoRepository.UpdatePrePaymentStatusAsync(2, result.PaymentCode, 4, "", postResp.CBSRefNo, postResp.Message, res.Msg);
                                postRespMessage = postResp.Message;
                                makePaymentResponse.RespStatus = 1;
                                makePaymentResponse.Data21 = result.ApprovalNeeded;
                                makePaymentResponse.RespMessage = "Transaction posting Failed.";
                                statMessage = postResp.Message;
                            makePaymentResponse.Data1 = res.Token3;
                            makePaymentResponse.Data19 = res.Consumption;
                            makePaymentResponse.Data20 = Payment.BillCode;
                        }
                    }
                    //---- Update payment status
                }
            }
            else
                makePaymentResponse.RespStatus = 1;
                makePaymentResponse.RespMessage = result.RespMessage;

            return makePaymentResponse;
        }
        public async Task<GenericModel> ApproveBuyTokenAsync(int FileCode, int action, string reason, int userCode)
        {
            GenericModel makePaymentResponse = new GenericModel
            {
                RespStatus = 1,
                RespMessage = "Unknown response!",
                Data1 = "0",
                Data2 = "0",
                Data3 = "0"
            };
            var payment =  db.RegidesoRepository.GetPrePay(FileCode);
            if (payment == null)
            {
                makePaymentResponse.RespStatus = 1;
                makePaymentResponse.RespMessage = "An error occured while fetching transaction";
                return makePaymentResponse;
            }

            var result = await db.RegidesoRepository.MakePrePaymentAsync(payment);
            db.Reset();

            if (result.RespStatus == 0)
            {
                if (action == 0)
                {
                    var res1 = await db.RegidesoRepository.UpdatePrePaymentStatusAsync(0, result.PaymentCode, 2, "", "","", "");
                    makePaymentResponse.RespStatus = res1.RespStatus;
                    makePaymentResponse.RespMessage = "Payment Rejected Successifully.";
                    makePaymentResponse.Data1 = payment.BillCode.ToString();
                    makePaymentResponse.Data3 = "1";
                    makePaymentResponse.Data4 = Convert.ToString(result.PaymentCode);
                }
                else
                {
                    makePaymentResponse.Data1 = result.PaymentCode.ToString();
                    makePaymentResponse.Data2 = "1";

                    string statMessage = payment.CBSRef;
                    string postRespMessage = "";
                    makePaymentResponse.RespMessage = "Transaction processed successfully.";
                    makePaymentResponse.RespStatus = 0;

                    //---- Upload to core
                    var postResp = RGPostToFlex(result);
                    if (!postResp.Successful)
                    {
                        postRespMessage = postResp.Message;
                        makePaymentResponse.Data2 = "1";
                        makePaymentResponse.RespStatus = 1;
                        if (postResp.Message.StartsWith("ERROR!"))
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }
                        else
                        {
                            makePaymentResponse.RespMessage = "Transaction posting to the core banking failed! " + postResp.Message;
                        }

                        //---- Update payment status (4 - Failed)
                        await db.RegidesoRepository.UpdatePrePaymentStatusAsync(1, result.PaymentCode, 4, "1", "", postResp.Message, "");

                        return makePaymentResponse;
                    }
                    else
                    {
                        // ----update for successful
                        await db.RegidesoRepository.UpdatePrePaymentStatusAsync(1, result.PaymentCode, 4, "2", postResp.CBSRefNo, postResp.Message, "");
                        var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.RegidesoPayBill);
                        if (data.RespStatus != 0)
                        {
                            makePaymentResponse.RespMessage = data.RespMessage;
                            return makePaymentResponse;
                        }

                        //--- Get auth token
                        var finBridge = new FinBridgeGateway(data.Data1, _logFile);
                        var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
                        if (authData.ErrorCode != 0)
                        {
                            makePaymentResponse.RespMessage = authData.ErrorMsg;
                            return makePaymentResponse;
                        }
                        var regideso = finBridge.CreateRegidesoService(authData.Token);
                        var res = await regideso.RequestPrePayAsync(payment);
                        res.Maker = payment.Maker;
                        res.BillCode = result.PaymentCode;
                        res.Stat = 1;
                        res.Meter_No = payment.Meter_No;
                        if (res.Success)
                        {
                            //--- Posting successfully
                            var post = await db.RegidesoRepository.UpdatePrePayment(res);
                            postRespMessage = postResp.Message;
                            makePaymentResponse.RespStatus = 0;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                            statMessage = postResp.Message;
                            makePaymentResponse.Data1 = res.Token3;
                            makePaymentResponse.Data19 = res.Consumption;
                            makePaymentResponse.Data20 = payment.BillCode;
                        }
                        else
                        {
                            await db.RegidesoRepository.UpdatePrePaymentStatusAsync(2, result.PaymentCode, 4, "", "", "",res.Msg);
                            makePaymentResponse.RespStatus = 1;
                            makePaymentResponse.RespMessage = "Transaction posting was successful.";
                            makePaymentResponse.Data1 = res.Token3;
                            makePaymentResponse.Data20 = payment.BillCode;

                        }
                       
                    }

                    //---- Update payment status

                }
            }
            else
                makePaymentResponse.RespMessage = result.RespMessage;

            return makePaymentResponse;
        }
        #endregion

        #region Miarie
        public async Task<MairieTaxNoteDataModel> QueryMiarieTaxAsync(QueryMarie model)
        {
            MairieTaxNoteDataModel resp = new MairieTaxNoteDataModel();

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.Miarie);
            if (data.RespStatus != 0)
            {
                resp.RespMessage = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.RespMessage = authData.ErrorMsg;
                return resp;
            }

            //--- Create Mairie gateway
            var mairie = finBridge.CreateMairieService(authData.Token);

            if (model.TypeCode == 0)
            {
                return await mairie.QueryTaxItemAsync(model.TaxType, model.RefNo);
            }
            else
            {
                //---- Validate
                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                var jsonData = JsonConvert.SerializeObject(model, settings);
                if (jsonData == "{}")
                {
                    resp.RespMessage = "Invalid tax query data!";
                    return resp;
                }

                return await mairie.QueryTaxItemAsync(model.TaxType, model);
            }
        }

        public async Task<MairieTaxpayerModel> QueryMiarieTaxpayerAsync(MairieTaxpayerQueryModel model)
        {
            MairieTaxpayerModel resp = new MairieTaxpayerModel();

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.Miarie);
            if (data.RespStatus != 0)
            {
                resp.RespMessage = data.RespMessage;
                return resp;
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.RespMessage = authData.ErrorMsg;
                return resp;
            }

            //--- Create Mairie gateway
            var mairie = finBridge.CreateMairieService(authData.Token);
            return await mairie.QueryTaxpayerAsync(model.TaxpayerId);
        }

        public async Task<GenericModel> ConfirmMiarieTaxAsync(MairieTaxNoteModel taxData, int userCode)
        {
            MiarieTaxFile taxFile = new MiarieTaxFile
            {
                NoteType = taxData.TaxNoteType,
                NoteNo = taxData.RefNo,
                RefNo = taxData.Nic,
                PayerName = taxData.TaxPayerName,
                Descr = taxData.TaxNoteDescr,
                Title = taxData.Title,
                TaxAmount = Convert.ToDecimal(taxData.Amount),
                Period = taxData.Period,
                UserCode = userCode,
                TypeName = taxData.NoteTypeName
            };

            var result = await db.MiarieRepository.CreateTaxAsync(taxFile);
            db.Reset();

            return result;
        }
        
        public async Task<MiarieReportModels> GetMiariePaymentReceipt(int paymentCode)
        {
            return await Task.Run(() =>
            {
                return db.MiarieRepository.GetMiariePaymentReceipt(paymentCode);
            });
        }

        

        private async Task<MairieTaxPayResultModel> MakeMiariePaymentAsync(TaxPaymentModel miariePay, TaxToPostModel result)
        {
            MairieTaxPayResultModel resp = new MairieTaxPayResultModel();

            //--- Get auth token
            var finBridge = new FinBridgeGateway(result.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(result.Data2, result.Data3);
            if (authData.ErrorCode != 0)
            {
                resp.RespMessage = authData.ErrorMsg;
                return resp;
            }

            //--- Create Mairie gateway
            var mairie = finBridge.CreateMairieService(authData.Token);

            var payment = new MairieTaxNotePaymentModel
            {
                RefNo = miariePay.TaxRef,
                AgentRef = result.MainRefNo,
                Amount = miariePay.Amount,
                Desciption = "Miarie Payment",
                TaxType = miariePay.ItemType
            };

            return await mairie.MakeTaxPaymentAsync(payment);            
        }       

        public async Task<IEnumerable<MiariePaymentModel>> GetMiariePaymentsAsync(int status, string dateRange)
        {
            string dateFrom = DateTime.Now.AddDays(-7).ToString("dd MMM yyyy");
            string dateTo = DateTime.Now.ToString("dd MMM yyyy");
            if (!string.IsNullOrEmpty(dateRange))
            {
                var dates = dateRange.Split('-');
                dateFrom = dates[0].Trim();
                dateTo = dates[1].Trim();
            }
            return await db.MiarieRepository.GetPaymentsAsync(status, dateFrom, dateTo);
        }

        public async Task<IEnumerable<MairieListItemModel>> GetMiarieTaxTypesAsync(int type)
        {

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.Miarie);
            if (data.RespStatus != 0)
            {
                LogUtil.Error(_logFile, "Bl.GetMiarieTaxTypesAsync", data.RespMessage);
                return new List<MairieListItemModel>();
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                LogUtil.Error(_logFile, "Bl.GetMiarieTaxTypesAsync", authData.ErrorMsg);
                return new List<MairieListItemModel>();
            }

            //--- Create Mairie gateway
            var mairie = finBridge.CreateMairieService(authData.Token);
            var queryResult = await mairie.GetTaxTypesAsync(type);
            if (queryResult.Success)
            {
                return queryResult.Types.ToList();
            }
            else
            {
                LogUtil.Error(_logFile, "Bl.GetMiarieTaxTypesAsync", queryResult.RespMessage);
                return new List<MairieListItemModel>();
            }
        }

        public async Task<IEnumerable<MarieDataFieldModel>> GetMiarieTaxTypeFieldsAsync(int type)
        {

            var data = await db.GeneralRepository.GetSystemSettingAsync(SettingType.Miarie);
            if (data.RespStatus != 0)
            {
                LogUtil.Error(_logFile, "Bl.GetMiarieTaxTypeFieldsAsync", data.RespMessage);
                return new List<MarieDataFieldModel>();
            }

            //--- Get auth token
            var finBridge = new FinBridgeGateway(data.Data1, _logFile);
            var authData = await finBridge.GetAuthTokenAsync(data.Data2, data.Data3);
            if (authData.ErrorCode != 0)
            {
                LogUtil.Error(_logFile, "Bl.GetMiarieTaxTypeFieldsAsync", authData.ErrorMsg);
                return new List<MarieDataFieldModel>();
            }

            //--- Create Mairie gateway
            var mairie = finBridge.CreateMairieService(authData.Token);
            var queryResult = await mairie.GetTaxTypeFieldsAsync(type);
            if (queryResult.Success)
            {
                return queryResult.Fields.ToList();
            }
            else
            {
                LogUtil.Error(_logFile, "Bl.GetMiarieTaxTypeFieldsAsync", queryResult.RespMessage);
                return new List<MarieDataFieldModel>();
            }
        }

        public async Task<BaseEntity> RepostMiarieTaxAsync(int fileCode, int userCode)
        {
            var response = new BaseEntity { RespStatus = 1 };
            int fileStat = 0;
            string fileStatMessage = "";
            string extra1 = "";
            string extra2 = "";
            string extra3 = "";

            var taxData = await db.MiarieRepository.GetRepostTaxAsync(fileCode);
            if (taxData.RespStatus != 0)
                return new BaseEntity { RespStatus = taxData.RespStatus, RespMessage = taxData.RespMessage };

            TaxPaymentModel taxPayment = new TaxPaymentModel
            {
                TaxRef = taxData.Data1,
                Amount = Convert.ToDecimal(taxData.Data2),
                ItemType = Convert.ToInt32(taxData.Data3)
            };

            TaxToPostModel gatewaySetts = new TaxToPostModel
            {
                MainRefNo = taxData.Data4,
                Data1 = taxData.Data5,
                Data2 = taxData.Data6,
                Data3 = taxData.Data7
            };

            var miarieResult = await MakeMiariePaymentAsync(taxPayment, gatewaySetts);

            if (!miarieResult.Success)
            {
                fileStat = 1;
                fileStatMessage = miarieResult.RespMessage;
                response.RespStatus = 1;
                response.RespMessage = "Transaction posting to Miarie failed! " + miarieResult.RespMessage;
                extra1 = miarieResult.RespMessage;
            }
            else
            {
                extra1 = miarieResult.Description;
                response.RespStatus = 0;
                response.RespMessage = "Transaction has been processed successfully.";
            }

            //---- Update status
            await db.MiarieRepository.UpdateStatusAsync(fileCode, fileStat, fileStatMessage, extra1, extra2, extra3);

            return response;
        }
        #endregion

        #region Reports
        public async Task<IEnumerable<ReportModel>> GetReportsAsync()
        {
            return await db.ReportRepository.GetListAsync();
        }

        public async Task<IEnumerable<ReportFilterItemModel>> GetReportFiltersAsync(int reportCode)
        {
            return await db.ReportRepository.GetFiltersAsync(reportCode);
        }


        public async Task<CreateFileResultModel> GenerateReportAsync(ReportFilterModel filterData, int type)
        {
            var result = new CreateFileResultModel();
            var report = await db.ReportRepository.GetSettAsync(filterData.ReportCode);
            if (report.RespStatus == 0)
            {
                if (!string.IsNullOrEmpty(filterData.DateFrom))
                {
                    string[] dates = filterData.DateFrom.Split('-');
                    if (dates.Length == 2)
                    {
                        filterData.DateFrom = dates[0].Trim();
                        filterData.DateTo = dates[1].Trim();
                    }
                }

                var myDataSets = report.DataSets.Split('|');
                var dataList = new List<ReportData>();
                foreach (var dSet in myDataSets)
                {
                    var setData = dSet.Split(',');
                    if (setData.Length == 3)
                    {
                        var table = await db.ReportRepository.GetDataAsync(filterData, Convert.ToInt32(setData[0]));
                        dataList.Add(new ReportData
                        {
                            Data = table,
                            BindName = setData[2],
                            SourceName = setData[1]
                        });
                    }
                }

                //---- Get parameters
                var repParams = await db.ReportRepository.GetParamsAsync(filterData);

                report.DataList = dataList;
                report.Parameters = repParams.ToList();

                if (type == 0)
                    return CreatePdfReport(report);
                else
                    return CreateExcelReport(report);
            }
            else
            {
                result.Message = report.RespMessage;
            }

            return result;
        }
        #endregion

        #region Private methods
        private PostResultModel PostToFlex(TaxToPostModel postData)
        {
            PostResultModel postResult = new PostResultModel
            {
                Successful = false,
                Message = "ERROR! Unknown results!"
            };
            try
            {
                HttpClient httpClient;
                Exception ex;
                string TxnTyp = postData.MainTxnType;
                if (string.IsNullOrEmpty(TxnTyp))
                {
                    if (postData.PaymentMode.ToString() == "0") //cash
                    {
                        TxnTyp = "601";
                    }
                    else if (postData.PaymentMode.ToString() == "1") //funds transfer
                    {
                        TxnTyp = "602";
                    }
                    else if (postData.PaymentMode.ToString() == "2") // inhse chq
                    {
                        TxnTyp = "603";
                    }
                    else if (postData.PaymentMode.ToString() == "3") //certified chq
                    {
                        TxnTyp = "604";
                    }
                }

                //----- Post main transaction
                CBSPostModel postModel = new CBSPostModel
                {
                    Amount = postData.MainAmount,
                    CrAccount = postData.MainCrAccount, //Johnson interchanged that due to flex configuration
                    DrAccount = postData.MainDrAccount,
                    CurrencyCode = postData.CurrencyCode,
                    Flag = postData.MainFlag,
                    Narration = postData.MainNarration,
                    Officer = postData.CBSOfficer,
                    RefNo = postData.MainRefNo,
                    TransactorName = postData.TransactorName,
                    TxnCode = postData.MainTxnCode,
                    TxnType = TxnTyp,// postData.MainTxnType,
                    Txndata=postData.ChargeAmount.ToString(),
                    ChequeNo = postData.ChequeNo ?? "",
                    brachCode = postData.BrachCode,
                    Appid = 200
                };

                var jsonData = JsonConvert.SerializeObject(postModel);
                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostToFlex:Data ", jsonData);
                //===============================================

                httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                var postResp = httpClient.SendRequest(jsonData, out ex);

                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostToFlex >> ", postResp);
                //===============================================

                if (string.IsNullOrEmpty(postResp))
                {
                    if (ex != null)
                        postResult.Message = ex.Message;

                    return postResult;
                }
                else
                {
                    var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                    if (respData != null)
                    {
                        if (respData.Successful)
                        {
                            if (Convert.ToString(postData.ChargeAmount) != "0.00") //post only when charge amount is not zero
                            {
                                //---- Post charge asynchronous
                               // PostTaxCharge(postData);
                            }


                            postResult.Successful = true;
                            postResult.Message = respData.BankRef;
                            return postResult;
                        }
                        else
                            postResult.Message = respData.Response;
                    }
                }
            }
            catch (Exception ex)
            {
                postResult.Message = "ERROR! " + ex.Message;
                LogUtil.Error(_logFile, "Bl.PostToFlex", ex);
            }
            return postResult;
        }

        private PostResultModel RGPostToFlex(BuyTokenPostModel postData)
        {
            PostResultModel postResult = new PostResultModel
            {
                Successful = false,
                Message = "ERROR! Unknown results!"
            };
            try
            {
                HttpClient httpClient;
                Exception ex;
                string TxnTyp = "";
                    if (postData.PaymentMode.ToString() == "0") //cash
                    {
                        TxnTyp = "501";
                    }
                    else if (postData.PaymentMode.ToString() == "1") //funds transfer
                    {
                        TxnTyp = "502";
                    }
                    else if (postData.PaymentMode.ToString() == "2") // inhse chq
                    {
                        TxnTyp = "503";
                    }
                    else if (postData.PaymentMode.ToString() == "3") //certified chq
                    {
                        TxnTyp = "504";
                    }
                

                //----- Post main transaction
                CBSPostModel postModel = new CBSPostModel
                {
                    Amount = postData.MainAmount,
                    CrAccount = postData.MainCrAccount, //Johnson interchanged that due to flex configuration
                    DrAccount = postData.MainDrAccount,
                    CurrencyCode = postData.CurrencyCode,
                    Flag = postData.MainFlag,
                    Narration = postData.MainNarration,
                    Officer = postData.CBSOfficer,
                    RefNo = postData.MainRefNo,
                    TransactorName = postData.TransactorName,
                    TxnCode = TxnTyp,
                    TxnType = TxnTyp,// postData.MainTxnType,
                    Txndata = postData.ChargeAmount.ToString(),
                    ChequeNo = postData.ChequeNo ?? "",
                    brachCode = postData.BrachCode,
                    Appid = 200
                };

                var jsonData = JsonConvert.SerializeObject(postModel);
                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostToFlex:Data ", jsonData);
                //===============================================

                httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                var postResp = httpClient.SendRequest(jsonData, out ex);

                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostToFlex >> ", postResp);
                //===============================================

                if (string.IsNullOrEmpty(postResp))
                {
                    if (ex != null)
                        postResult.Message = ex.Message;

                    return postResult;
                }
                else
                {
                    var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                    if (respData != null)
                    {
                        if (respData.Successful)
                        {
                            if (Convert.ToString(postData.ChargeAmount) != "0.00") //post only when charge amount is not zero
                            {
                                //---- Post charge asynchronous
                                // PostTaxCharge(postData);
                            }


                            postResult.Successful = true;
                            postResult.Message = respData.Response;
                            postResult.CBSRefNo = respData.BankRef;
                            return postResult;
                        }
                        else
                            postResult.Message = respData.Response;
                    }
                }
            }
            catch (Exception ex)
            {
                postResult.Message = "ERROR! " + ex.Message;
                LogUtil.Error(_logFile, "Bl.PostToFlex", ex);
            }
            return postResult;
        }

        private void PostTaxCharge(TaxToPostModel postData)
        {
            Task.Run(() =>
            {
                try
                {
                    HttpClient httpClient;
                    Exception ex;
                    int postStat = 1;
                    string postMsg = "Unknown error!";

                    //----- Post charge transaction
                    CBSPostModel postModel = new CBSPostModel
                    {
                        Amount = postData.ChargeAmount,
                        CrAccount = postData.ChargeCrAccount,
                        DrAccount = postData.ChargeDrAccount,
                        CurrencyCode = postData.CurrencyCode,
                        Flag = postData.ChargeFlag,
                        Narration = postData.ChargeNarration,
                        Officer = postData.CBSOfficer,
                        RefNo = postData.ChargeRefNo,
                        TransactorName = postData.TransactorName,
                        TxnCode = postData.ChargeTxnCode,
                        TxnType = postData.ChargeTxnType
                    };

                    var jsonData = JsonConvert.SerializeObject(postModel);
                    httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                    var postResp = httpClient.SendRequest(jsonData, out ex);

                    if (string.IsNullOrEmpty(postResp))
                    {
                        if (ex != null)
                            postMsg = ex.Message;
                    }
                    else
                    {
                        var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                        if (respData != null)
                        {
                            if (respData.Successful)
                            {
                                postStat = 0;
                                postMsg = respData.Response;
                            }
                            else
                                postMsg = respData.Response;
                        }
                    }

                    //---- Update charge transaction
                    db.TaxRepository.UpdateTaxChargeStatus(postData.ChargeCode, postStat, postMsg);

                }
                catch (Exception ex)
                {
                    //postResult.Message = "ERROR! " + ex.Message;
                }
            });
        }

        private PostResultModel PostPowerFlex(TaxToPostModel postData)
        {
            PostResultModel postResult = new PostResultModel
            {
                Successful = false,
                Message = "ERROR! Unknown results!"
            };
            try
            {
                HttpClient httpClient;
                Exception ex;

                //----- Post main transaction
                CBSPostModel postModel = new CBSPostModel
                {
                    Amount = postData.MainAmount,
                    CrAccount = postData.MainCrAccount,
                    DrAccount = postData.MainDrAccount,
                    CurrencyCode = postData.CurrencyCode,
                    Flag = postData.MainFlag,
                    Narration = postData.MainNarration,
                    Officer = postData.CBSOfficer,
                    RefNo = postData.MainRefNo,
                    TransactorName = postData.TransactorName,
                    TxnCode = postData.MainTxnCode,
                    TxnType = postData.MainTxnType,
                    ChequeNo = postData.ChequeNo ?? "",
                    brachCode = postData.BrachCode,
                    Appid = 200
                };

                var jsonData = JsonConvert.SerializeObject(postModel);
                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostToFlex:Data ", jsonData);
                //===============================================

                httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                var postResp = httpClient.SendRequest(jsonData, out ex);

                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostToFlex >> ", postResp);
                //===============================================

                if (string.IsNullOrEmpty(postResp))
                {
                    if (ex != null)
                        postResult.Message = ex.Message;

                    return postResult;
                }
                else
                {
                    var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                    if (respData != null)
                    {
                        if (respData.Successful)
                        {
                            //if (Convert.ToString(postData.ChargeAmount) != "0.00") //post only when charge amount is not zero
                            //{
                            //    //---- Post charge asynchronous
                            //    PostPowerCharge(postData);
                            //}

                            //if (Convert.ToString(postData.Balance) != "0.00") //post only when payway has reimbursement
                            //{
                            //    //---- Post charge asynchronous
                            //    PostReimbursement(postData);
                            //}

                            postResult.Successful = true;
                            postResult.Message = respData.BankRef;
                            return postResult;
                        }
                        else
                            postResult.Message = respData.Response;
                    }
                }
            }
            catch (Exception ex)
            {
                postResult.Message = "ERROR! " + ex.Message;
                LogUtil.Error(_logFile, "Bl.PostToFlex", ex);
            }
            return postResult;
        }

        private void PostPowerCharge(TaxToPostModel postData)
        {
            Task.Run(() =>
            {
                try
                {
                    HttpClient httpClient;
                    Exception ex;
                    int postStat = 1;
                    string postMsg = "Unknown error!";

                    //----- Post charge transaction
                    CBSPostModel postModel = new CBSPostModel
                    {
                        Amount = postData.ChargeAmount,
                        CrAccount = postData.ChargeCrAccount,
                        DrAccount = postData.ChargeDrAccount,
                        CurrencyCode = postData.CurrencyCode,
                        Flag = postData.ChargeFlag,
                        Narration = postData.ChargeNarration,
                        Officer = postData.CBSOfficer,
                        RefNo = postData.ChargeRefNo,
                        TransactorName = postData.TransactorName,
                        TxnCode = postData.ChargeTxnCode,
                        TxnType = postData.ChargeTxnType
                    };

                    var jsonData = JsonConvert.SerializeObject(postModel);
                    httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                    var postResp = httpClient.SendRequest(jsonData, out ex);

                    if (string.IsNullOrEmpty(postResp))
                    {
                        if (ex != null)
                            postMsg = ex.Message;
                    }
                    else
                    {
                        var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                        if (respData != null)
                        {
                            if (respData.Successful)
                            {
                                postStat = 0;
                                postMsg = respData.Response;
                            }
                            else
                                postMsg = respData.Response;
                        }
                    }

                    //---- Update charge transaction
                    db.PaywayGatewayRepository.UpdatePowerChargeStatus(postData.ChargeCode, postStat, postMsg);

                }
                catch (Exception ex)
                {
                    //postResult.Message = "ERROR! " + ex.Message;
                }
            });
        }

        private void PostReimbursement(TaxToPostModel postData)
        {
            Task.Run(() =>
            {
                try
                {
                    HttpClient httpClient;
                    Exception ex;
                    int postStat = 1;
                    string postMsg = "Unknown error!";

                    //----- Post charge transaction
                    CBSPostModel postModel = new CBSPostModel
                    {
                        Amount = postData.Balance,
                        CrAccount = postData.MainDrAccount,
                        DrAccount = postData.MainCrAccount,
                        CurrencyCode = postData.CurrencyCode,
                        Flag = postData.ChargeFlag,
                        Narration = postData.ReimbursementNarration,
                        Officer = postData.CBSOfficer,
                        RefNo = postData.ReimbursementRefNo,
                        TransactorName = postData.TransactorName,
                        TxnCode = postData.ChargeTxnCode,
                        TxnType = postData.ChargeTxnType
                    };

                    var jsonData = JsonConvert.SerializeObject(postModel);
                    httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                    var postResp = httpClient.SendRequest(jsonData, out ex);

                    if (string.IsNullOrEmpty(postResp))
                    {
                        if (ex != null)
                            postMsg = ex.Message;
                    }
                    else
                    {
                        var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                        if (respData != null)
                        {
                            if (respData.Successful)
                            {
                                postStat = 0;
                                postMsg = respData.Response;
                            }
                            else
                                postMsg = respData.Response;
                        }
                    }

                    //---- Update charge transaction
                    // db.TaxRepository.UpdateTaxChargeStatus(postData.ChargeCode, postStat, postMsg);

                }
                catch (Exception ex)
                {
                    //postResult.Message = "ERROR! " + ex.Message;
                }
            });
        }

        private PostResultModel PostDomesticTaxToFlex(TaxToPostModel postData)
        {
            PostResultModel postResult = new PostResultModel
            {
                Successful = false,
                Message = "ERROR! Unknown results!"
            };
            try
            {
                HttpClient httpClient;
                Exception ex;
                string TxnTyp = "";
                if (postData.PaymentMode.ToString() == "0") //cash
                {
                    TxnTyp = "701";
                }
                else if (postData.PaymentMode.ToString() == "1") //funds transfer
                {
                    TxnTyp = "702";
                }
                else if (postData.PaymentMode.ToString() == "2") // inhse chq
                {
                    TxnTyp = "603";
                }
                else if (postData.PaymentMode.ToString() == "3") //certified chq
                {
                    TxnTyp = "704";
                }
                //----- Post main transaction
                CBSPostModel postModel = new CBSPostModel
                {
                    Amount = postData.MainAmount,
                    CrAccount = postData.MainCrAccount,
                    DrAccount = postData.MainDrAccount,
                    //CrAccount = postData.MainDrAccount,
                    //DrAccount = postData.MainCrAccount,
                    CurrencyCode = postData.CurrencyCode,
                    Flag = postData.MainFlag,
                    Narration = postData.MainNarration,
                    Officer = postData.CBSOfficer,
                    RefNo = postData.MainRefNo,
                    Txndata=postData.ChargeAmount.ToString(),
                    TransactorName = postData.TransactorName,
                    TxnCode = postData.MainTxnCode,
                    TxnType = TxnTyp,//postData.MainTxnType,
                    ChequeNo = postData.ChequeNo ?? "",
                    brachCode = postData.BrachCode,
                    Appid = 200
                };

                var jsonData = JsonConvert.SerializeObject(postModel);
                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostDomesticTaxToFlex:Data ",jsonData);
                //===============================================

                httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                var postResp = httpClient.SendRequest(jsonData, out ex);

                //=============================================
                LogUtil.Infor(_logFile, "Bl.PostDomesticTaxToFlex >> ", postResp);
                //===============================================

                if (string.IsNullOrEmpty(postResp))
                {
                    if (ex != null)
                        postResult.Message = ex.Message;

                    return postResult;
                }
                else
                {
                    var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                    if (respData != null)
                    {
                        if (respData.Successful)
                        {
                            if (Convert.ToString(postData.ChargeAmount) != "0.00") //post only when charge amount is not zero
                            {
                                //---- Post charge asynchronous
                               // PostDomesticTaxCharge(postData);
                            }


                            postResult.Successful = true;
                            postResult.Message = respData.BankRef;
                            return postResult;
                        }
                        else
                            postResult.Message = respData.Response;
                    }
                }
            }
            catch (Exception ex)
            {
                postResult.Message = "ERROR! " + ex.Message;
                LogUtil.Error(_logFile, "Bl.PostToFlex", ex);
            }
            return postResult;
        }

        private void PostDomesticTaxCharge(TaxToPostModel postData)
        {
            Task.Run(() =>
            {
                try
                {
                    HttpClient httpClient;
                    Exception ex;
                    int postStat = 1;
                    string postMsg = "Unknown error!";

                    //----- Post charge transaction
                    CBSPostModel postModel = new CBSPostModel
                    {
                        Amount = postData.ChargeAmount,
                        CrAccount = postData.ChargeCrAccount,
                        DrAccount = postData.ChargeDrAccount,
                        CurrencyCode = postData.CurrencyCode,
                        Flag = postData.ChargeFlag,
                        Narration = postData.ChargeNarration,
                        Officer = postData.CBSOfficer,
                        RefNo = postData.ChargeRefNo,
                        TransactorName = postData.TransactorName,
                        TxnCode = postData.ChargeTxnCode,
                        TxnType = postData.ChargeTxnType
                    };

                    var jsonData = JsonConvert.SerializeObject(postModel);
                    httpClient = new HttpClient(postData.PostUrl, HttpClient.RequestType.Post);
                    var postResp = httpClient.SendRequest(jsonData, out ex);

                    if (string.IsNullOrEmpty(postResp))
                    {
                        if (ex != null)
                            postMsg = ex.Message;
                    }
                    else
                    {
                        var respData = JsonConvert.DeserializeObject<CBSPostResposeModel>(postResp);
                        if (respData != null)
                        {
                            if (respData.Successful)
                            {
                                postStat = 0;
                                postMsg = respData.Response;
                            }
                            else
                                postMsg = respData.Response;
                        }
                    }

                    //---- Update charge transaction
                    db.DomesticRepository.UpdateDomesticTaxChargeStatus(postData.ChargeCode, postStat, postMsg);

                }
                catch (Exception ex)
                {
                    //postResult.Message = "ERROR! " + ex.Message;
                }
            });
        }
       
        private string MakeCBSRequest(string url, string reqData)
        {
            var httpClient = new HttpClient(url, HttpClient.RequestType.Post);
            Exception ex;
            var balResp = httpClient.SendRequest(reqData, out ex);
            if (string.IsNullOrEmpty(balResp))
            {
                if (ex != null)
                    return "ERROR:" + ex.Message;
                return "ERROR:Request failed due to an error!";
            }
            else
            {
                return balResp;
            }
        }

        private CreateFileResultModel CreatePdfReport(PrintReportModel reportModel)
        {
            try
            {
                //----- Create report
                Report report = new Report();

                //--- Load report
                report.Load($@"{reportModel.ReportsDir}\{reportModel.FileName}.frx");

                //---- Manage data
                foreach (var data in reportModel.DataList)
                {
                    //----- Register data
                    report.RegisterData(data.Data, data.SourceName);
                    report.GetDataSource(data.SourceName).Enabled = true;

                    var dataBand = report.FindObject(data.BindName) as DataBand;
                    if (dataBand != null)
                    {
                        var ds = report.GetDataSource(data.SourceName);
                        dataBand.DataSource = ds;
                    }
                }

                //----- Manage Parameters
                foreach (var p in reportModel.Parameters)
                {
                    var rptParam = report.Parameters.FindByName(p.ParamName);
                    if (rptParam != null)
                        report.SetParameterValue(p.ParamName, p.ParamValue);
                }

                //----- Prepare the report
                report.Prepare();

                //---- Create PDF
                MemoryStream ms = new MemoryStream();
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                pdfExport.Export(report, ms);

                return new CreateFileResultModel
                {
                    Successful = true,
                    FileData = ms.ToArray(),
                    ContentType = "application/pdf",
                    DownloadName = reportModel.Title.Replace(" ", "_") + ".pdf"
                };
            }
            catch (Exception ex)
            {
                LogUtil.Error(_logFile, "Bl.CreatePdfReport()", ex);
                return new CreateFileResultModel { Message = "Report generation failed due to an error!" };
            }
        }

        private CreateFileResultModel CreateExcelReport(PrintReportModel reportModel)
        {
            try
            {
                var myData = reportModel.DataList.FirstOrDefault();

                //----- Create excel
                IWorkbook workbook = new XSSFWorkbook();
                ISheet mySheet = workbook.CreateSheet("Report Data");

                //---- Create row
                int rowIndex = 0;
                IRow row = mySheet.CreateRow(rowIndex);
                var range = new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, 0, myData.Data.Columns.Count);
                rowIndex += 1;
                var cell = row.CreateCell(0);

                cell.SetCellValue("BITPay System");

                row = mySheet.CreateRow(rowIndex);
                var range1 = new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, 0, myData.Data.Columns.Count);
                rowIndex += 1;
                cell = row.CreateCell(0);
                cell.SetCellValue(reportModel.Title);

                row = mySheet.CreateRow(rowIndex);
                var range2 = new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, 0, myData.Data.Columns.Count);
                rowIndex += 1;
                cell = row.CreateCell(0);
                cell.SetCellValue(reportModel.SubTitle);

                mySheet.AddMergedRegion(range);
                mySheet.AddMergedRegion(range1);
                mySheet.AddMergedRegion(range2);

                row = mySheet.CreateRow(rowIndex);
                rowIndex += 1;
                var font = workbook.GetFontAt(0);
                font.FontHeightInPoints = 12;
                font.Boldweight = (short)FontBoldWeight.Bold;

                var hStyle = workbook.CreateCellStyle();

                hStyle.BorderLeft = BorderStyle.Thin;
                hStyle.BorderTop = BorderStyle.Thin;
                hStyle.BorderRight = BorderStyle.Thin;
                hStyle.BorderBottom = BorderStyle.Thin;

                hStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                hStyle.FillPattern = FillPattern.SolidForeground;
                hStyle.SetFont(font);

                for (int k = 0; k < myData.Data.Columns.Count; k++)
                {
                    cell = row.CreateCell(k);
                    cell.CellStyle = hStyle;
                    cell.SetCellValue(myData.Data.Columns[k].ColumnName);
                }

                for (int x = 0; x < myData.Data.Rows.Count; x++)
                {
                    row = mySheet.CreateRow(rowIndex);
                    for (int k = 0; k < myData.Data.Columns.Count; k++)
                    {
                        cell = row.CreateCell(k);
                        cell.SetCellValue(myData.Data.Rows[x].ItemArray[k].ToString());
                    }
                    rowIndex += 1;
                }

                for (int i = 0; i < 5; i++) mySheet.AutoSizeColumn(i);

                var ms = new MemoryStream();
                workbook.Write(ms);

                return new CreateFileResultModel
                {
                    Successful = true,
                    FileData = ms.ToArray(),
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    DownloadName = reportModel.Title.Replace(" ", "_") + ".xlsx"
                };
            }
            catch (Exception ex)
            {
                LogUtil.Error(_logFile, "Bl.CreateExcelReport()", ex);
                return new CreateFileResultModel { Message = "Report generation failed due to an error!" };
            }
        }

        #endregion

    }
}
