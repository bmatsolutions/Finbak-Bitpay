using BITPayService.Models;
using ClosedXML.Excel;
using Newtonsoft.Json;
using OBRClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BITPayService
{
    public partial class BITPayService : ServiceBase
    {
        private Timer timer;
        private int INTERVAL = 10;

        private string apiUrl;
        private string obrUrl;
        private string obrUserName;
        private string obrPassword;
        private Db db;

        private DailyTimer dailyTimer;
        private DateTime? timeReached = null;

        private int dayHour;
        private int dayMinute;

        public BITPayService()
        {
            InitializeComponent();

            db = new Db(Util.GetDbConnString());
        }

        protected override void OnStart(string[] args)
        {
            //----- Load initial settings
            var setting = db.GetSettings(1);
            if (setting != null)
            {
                apiUrl = setting.Data4;
                obrUrl = setting.Data1;
                obrUserName = setting.Data2;
                obrPassword = setting.Data3;
           }

            //----- Start timer
            StartTheTimer();
            StartTheBulkTimer();
            StartDFSTimer();
            StartDomesticTimer();
        }

        protected override void OnStop()
        {
            this.timer.Dispose();
        }

        private void StartTheTimer()
        {
            try
            {
                timer = new Timer(new TimerCallback(TimerCallback));
                DateTime runTime = DateTime.Now.AddSeconds(INTERVAL);
                if (DateTime.Now > runTime)
                {
                    //If Scheduled Time is passed set Schedule for the next Interval.
                    runTime = runTime.AddSeconds(INTERVAL);
                }

                TimeSpan timeSpan = runTime.Subtract(DateTime.Now);
                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                timer.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Util.LogError("StartTheTimer", ex);
            }
        }
        private void StartDomesticTimer()
        {
            try
            {
                timer = new Timer(new TimerCallback(DomesticTimerCallback));
                DateTime runTime = DateTime.Now.AddSeconds(INTERVAL);
                if (DateTime.Now > runTime)
                {
                    //If Scheduled Time is passed set Schedule for the next Interval.
                    runTime = runTime.AddSeconds(INTERVAL);
                }

                TimeSpan timeSpan = runTime.Subtract(DateTime.Now);
                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                timer.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Util.LogError("StartdomesticTimer", ex);
            }
        }
        private void StartTheBulkTimer()
        {
            try
            {
                timer = new Timer(new TimerCallback(BulkTimerCallback));
                DateTime runTime = DateTime.Now.AddSeconds(INTERVAL);
                if (DateTime.Now > runTime)
                {
                    //If Scheduled Time is passed set Schedule for the next Interval.
                    runTime = runTime.AddSeconds(INTERVAL);
                }

                TimeSpan timeSpan = runTime.Subtract(DateTime.Now);
                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                timer.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Util.LogError("StartTheTimer", ex);
            }
        }

        private void StartDFSTimer()
        {
            try
            {
                //----- Get settings
                string dHour = System.Configuration.ConfigurationManager.AppSettings["DFS_RUN_HOUR"];
                string dMin = System.Configuration.ConfigurationManager.AppSettings["DFS_RUN_MIN"];               
                dayHour = Convert.ToInt32(dHour);
                dayMinute = Convert.ToInt32(dMin);

                //---- Create and initialize timer
                dailyTimer = new DailyTimer(dayHour, dayMinute);
                dailyTimer.TimeReached += DailyTimer_TimeReached;
                dailyTimer.ScheduleRun();
            }
            catch (Exception ex)
            {
                Util.LogError("BITPayService.StartDFSTimer", ex);
            }
        }       

        private void TimerCallback(object e)
        {
            
            try
            {
                //1. Get OBR notifications
                List<MakePaymentRequestModel> payments = db.GetPayments();

                //---- Start a thread for each payment
                var results = Parallel.ForEach(payments, payment =>
                {

                      int paytype = Convert.ToInt32(payment.Pay);
                      var Url = obrUrl + (obrUrl.EndsWith("/") ? "" : "/");
                      if (paytype == 1)
                      {
                          Url = Url + "WSDeclarationPayment?wsdl";
                      }
                      else if(paytype == 2)
                      {
                          Url = Url + "WSCreditPayment?wsdl";
                      }
                      else if (paytype == 3)
                      {
                          Url = Url + "WSCreditStatement?wsdl";
                      }
                      else if (paytype == 4)
                      {
                          Url = Url + "WSOtherPayment?wsdl";
                      }
                      else
                      {
                          Url = Url;
                      }
                      
                      GatewayRequestData requestData = new GatewayRequestData
                      {
                          Url = Url,
                          UserName = obrUserName,
                          Password = obrPassword,
                          Function = 1,
                          Data = payment
                      };
                      
                      string jsonData = JsonConvert.SerializeObject(requestData);

                      //----- Create create client
                      MyHttpClient httpClient = new MyHttpClient(apiUrl, MyHttpClient.RequestType.Post);
                      Exception ex;
                      var postResults = httpClient.SendRequest(jsonData, out ex);
                     
                      if (string.IsNullOrEmpty(postResults))
                      {
                          Util.LogError("TimerCallback",ex);
                      }
                      else
                      {
                          Util.LogError("TimerCallback: XML-RESP", new Exception(postResults), false);

                          RequestResponseModel respData = JsonConvert.DeserializeObject<RequestResponseModel>(postResults);
                          if (respData != null)
                          {
                              if (respData.Status == 0)
                              {
                                  //---- Process results
                                  string responseXml = (string)respData.Content;
                                  XDocument xml = XDocument.Parse(responseXml);
                                  string localName = "";
                                  if (paytype == 1)
                                  {
                                      localName = "declarationPaymentResult";
                                  }
                                  else if (paytype == 2)
                                  {
                                      localName = "creditPaymentResult";
                                  }
                                  else if (paytype == 3)
                                  {
                                      localName = "CreditStatementResult";
                                  }
                                  else if (paytype == 4)
                                  {
                                      localName = "otherPaymentResult";
                                  }
                                  else
                                  {
                                      localName = "";
                                  }
                                  
                                  var myData = xml.Descendants().Where(x => x.Name.LocalName == localName).FirstOrDefault();
                                  if (myData != null)
                                  {
                                      int stat = 0;
                                      string statMessage = "";
                                      string receiptNo = "";
                                      string payDate = ""; 
                                      string cmpName = "";
                                      string dclntName = "";
                                    var result = (string)myData.Element("result");
                                      var errorCode = (int)myData.Element("errorCode");
                                      if(paytype == 3 && errorCode == 0)
                                      {
                                          var datas = (from data in myData.Descendants("paidDeclaration")
                                                       select new CreditStateData
                                                       {
                                                           OfficeName = data.Element("office").Value,
                                                           AssYear = data.Element("assessmentYear").Value,
                                                           AssSerial = data.Element("assessmentSerial").Value,
                                                           AssNo = data.Element("assessmentNumber").Value,
                                                           Amount = (decimal) Math.Round(Util.ParseNumber(data.Element("paidAmount").Value),0)
                                                       }).ToList();

                                          var fileData = new DataTable();

                                          fileData.Columns.Add("Office");
                                          fileData.Columns.Add("AssessmentYear");
                                          fileData.Columns.Add("AssessmentSerial");
                                          fileData.Columns.Add("AssessmentNumber");
                                          fileData.Columns.Add("Amount", typeof(decimal));
                                          foreach (var item in datas)
                                          {
                                              fileData.Rows.Add(item.OfficeName, item.AssYear, item.AssSerial,item.AssNo,item.Amount);
                                          }
                                          db.InsertCreditPayment(fileData,payment.FileCode);
                                      }
                                      if (errorCode != 0)
                                      {
                                          stat = 1;
                                          statMessage = errorCode + " - " + (string)myData.Element("errorDescription") + " - " +(string)myData.Element("functionalError");
                                      }
                                      else
                                      {
                                          receiptNo = (string)myData.Element("receiptSerial") + (string)myData.Element("receiptNumber"); 
                                          payDate = (string)myData.Element("receiptDate");
                                            if (paytype == 9 || paytype == 4)
                                            {
                                                cmpName = (string)myData.Element("companyName");
                                                dclntName = (string)myData.Element("declarantName");
                                            }
                                    }

                                    db.UpdateFileStatus(payment.FileCode, stat, statMessage, receiptNo, payDate, cmpName, dclntName);
                                }
                              }
                              else
                              {
                                  Util.LogError("TimerCallback.Failed", new Exception(respData.Content));
                              }
                          }                                                
                      }
                  });

            
            }
            catch (Exception ex)
            {
                Util.LogError("TimerCallback", ex);
            }

            this.StartTheTimer();
        }

        public void DomesticTimerCallback(object e)
        {

            try
            {
                //1. Get OBR notifications
                List<DomesticTaxPayment> payments = db.GetDomesticPayments();

                //---- Start a thread for each payment
                var results = Parallel.ForEach(payments, payment =>
                {
                    var queryResult = AddDomesticTransaction(payment);
                    if (queryResult.status != 0)
                    {
                        if (queryResult.content == null)
                        {
                            queryResult.content = new PaymentResponseDetails { resId = "" };
                        }
                        db.UpdateDomesticPaymentStatusAsync(1, payment.PaymentCode, 4, queryResult.message, queryResult.content.resId);

                    }

                });
                
            }
            catch (Exception ex)
            {
                Util.LogError("DomesticTimerCallback", ex);
            }

            this.StartTheTimer();
        }
        public  DomesticPaymentResponse AddDomesticTransaction(DomesticTaxPayment queryData)
        {
            string responseXml = "";
            var queryResp = new DomesticPaymentResponse
            {
                status = -1,
                message = "Request was not executed!",
            };

            //----- Get query settings
            var setting = db.GetSettings(4);
            if (setting.RespStatus == 0)
            {
                if (string.IsNullOrEmpty(setting.Data1))
                {
                    queryResp.message = "Some of parameters needed to query data are not set!";
                    return queryResp;
                }
                else
                {

                    string[] data = queryData.DomesticTaxName.Split('|');
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


                    //  System.IO.File.AppendAllText("c:\\bmat\\others.txt", others);
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
                    MyHttpClient httpClient = new MyHttpClient(setting.Data4, MyHttpClient.RequestType.Post);
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
        public void BulkTimerCallback(object e)
        {

            try
            {
                //1. Get OBR notifications
                List<MakePaymentRequestModel> payments = db.Getmultipayments();


                //---- Start a thread for each payment
                var results = Parallel.ForEach(payments, payment =>
                {
                    payment.DeclarationList = db.GetBulkDataInfo(payment.FileCode);
                    int paytype = Convert.ToInt32(payment.Pay);
                    var Url = obrUrl + (obrUrl.EndsWith("/") ? "" : "/");
                    if (paytype == 6)
                    {
                        Url = Url + "WSDeclarationPayment?wsdl";
                    }
                    else if (paytype == 7)
                    {
                        Url = Url + "WSCreditPayment?wsdl";
                    }
                    else if (paytype == 9)
                    {
                        Url = Url + "WSOtherPayment?wsdl";
                    }
                    else
                    {
                        Url = Url;
                    }

                    GatewayRequestData requestData = new GatewayRequestData
                    {
                        Url = Url,
                        UserName = obrUserName,
                        Password = obrPassword,
                        Function = 7,
                        Data = payment
                    };

                    string jsonData = JsonConvert.SerializeObject(requestData);
                   //----- Create create client
                    MyHttpClient httpClient = new MyHttpClient(apiUrl, MyHttpClient.RequestType.Post);
                    Exception ex;
                    var postResults = httpClient.SendRequest(jsonData, out ex);

                    if (string.IsNullOrEmpty(postResults))
                    {
                        Util.LogError("BulkTimerCallback", ex);
                    }
                    else
                    {
                        Util.LogError("BulkTimerCallback: XML-RESP", new Exception(postResults), false);
                       RequestResponseModel respData = JsonConvert.DeserializeObject<RequestResponseModel>(postResults);
                        if (respData != null)
                        {
                            if (respData.Status == 0)
                            {
                                //---- Process results
                                string responseXml = (string)respData.Content;
                                XDocument xml = XDocument.Parse(responseXml);
                                string localName = "";
                                if (paytype == 6)
                                {
                                    localName = "declarationPaymentResult";
                                }
                                else if (paytype == 7)
                                {
                                    localName = "creditPaymentResult";
                                }
                                else if (paytype == 9)
                                {
                                    localName = "otherPaymentResult";
                                }
                                else
                                {
                                    localName = "";
                                }

                                var myData = xml.Descendants().Where(x => x.Name.LocalName == localName).FirstOrDefault();
                                if (myData != null)
                                {
                                    int stat = 0;
                                    string statMessage = "";
                                    string receiptNo = "";
                                    string payDate = "";
                                    string cmpName = "";
                                    string dclntName = "";
                                    var result = (string)myData.Element("result");
                                    var errorCode = (int)myData.Element("errorCode");
                                    
                                    if (errorCode != 0)
                                    {
                                        stat = 1;
                                        statMessage = errorCode + " - " + (string)myData.Element("errorDescription") + " - " + (string)myData.Element("functionalError");
                                    }
                                    else
                                    {
                                        receiptNo = (string)myData.Element("receiptSerial") + (string)myData.Element("receiptNumber");
                                        payDate = (string)myData.Element("receiptDate");
                                        if (paytype == 9)
                                        {
                                            cmpName = (string)myData.Element("companyName");
                                            dclntName = (string)myData.Element("declarantName");
                                        }
                                    }
                                   
                                    db.UpdateFileStatus(payment.FileCode, stat, statMessage, receiptNo, payDate,cmpName,dclntName);
                                    
                                }
                            }
                            else
                            {
                                Util.LogError("BulkTimerCallback.Failed", new Exception(respData.Content));
                            }
                        }
                    }
                });


            }
            catch (Exception ex)
            {
                Util.LogError("BulkTimerCallback", ex);
            }

            this.StartTheBulkTimer();
        }

        private void DailyTimer_TimeReached(DateTime time)
        {
            if (timeReached != null)
            {
                var timeDiff = time.Subtract((DateTime)timeReached);
                if (timeDiff.TotalHours < 1)
                {
                    string msg = string.Format("Time too short! Time now:{0:dd/MM/yyyy hh:mm tt}, Last run:{1:dd/MM/yyyy hh:mm tt}", time, timeReached);
                    Util.LogError("DailyTimer_TimeReached", new Exception(msg), false);
                    //dailyTimer.Stop();
                    return;
                }
            }
            timeReached = time;
            Util.LogError("DailyTimer_TimeReached", new Exception("Reached at: " + time.ToString("dd/MM/yyyy HH:mm tt")), false);

            GenerateMairieReconFile();

            //---- Delay for a minute
            Thread.Sleep(60000);

            dailyTimer.ScheduleRun();
        }

        public void GenerateMairieReconFile()
        {
            try
            {
                //---- Get seetings
                Task.Factory.StartNew(async () =>
                {
                    //---- Get data
                    var reportData = await db.GetMairieReportDataAsync();
                    if (reportData.Count == 0)
                        return;

                    //---- Generate file
                    var fileDir = System.Configuration.ConfigurationManager.AppSettings["DFS_FILE_DIR"];
                    string wDir = Path.Combine(fileDir, DateTime.Now.ToString("MMyy"));
                    if (!Directory.Exists(wDir))
                        Directory.CreateDirectory(wDir);

                    string fileName = Path.Combine(wDir, "Mairie_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
                    string reportName = "FinBank_" + DateTime.Now.ToString("yyyyMMdd");
                    string reportTitle = "Relever des transactions du " + DateTime.Now.ToString("dd/MM/yyyy");
                    var createResult = CreateMairieReportFile(fileName, reportName, reportTitle, reportData);
                    if (!createResult)
                        return;

                    //---- Get gateway settings
                    var setts = db.GetSettings(6);

                    //--- Get auth token
                    var finBridge = new FinBridgeGateway(setts.Data1);
                    var authData = await finBridge.GetAuthTokenAsync(setts.Data2, setts.Data3);
                    if (authData.ErrorCode != 0)
                    {
                        Util.LogError("BITPayService.GenerateMairieReconFile", new Exception(authData.ErrorMsg));
                        return;
                    }

                    //--- Create Mairie gateway
                    string agent = System.Configuration.ConfigurationManager.AppSettings["MAIRIE_AGENT_CODE"];
                    if (string.IsNullOrEmpty(agent))
                        agent = "finbank";

                    var mairie = finBridge.CreateMairieService(authData.Token);
                    var fileResult = await mairie.UploadFileAsync(agent, 506, fileName);

                    if (fileResult.Success)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine();
                        sb.AppendLine(" ============== UPLOAD SUCCESSFUL ===================");
                        sb.AppendLine(" Sent Name:" + fileName);
                        sb.AppendLine(" Result File:" + fileResult.Data.file);
                        sb.AppendLine(" Result Agent:" + fileResult.Data.agt);
                        sb.AppendLine(" =====================================================");
                        Util.LogError("BITPayService.GenerateMairieReconFile", new Exception(sb.ToString()), false);
                    }
                    else
                        Util.LogError("BITPayService.GenerateMairieReconFile", new Exception("Uploading file [" + fileName + "] failed! Reason:" + fileResult.Message));
                });
            }
            catch (Exception ex)
            {
                Util.LogError("BITPayService.GenerateMairieReconFile", ex);
            }
        }

        private bool CreateMairieReportFile(string fileName, string reportName, string reportTitle, List<MairieReportDataItem> records)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(reportName);
                    int rowIdx = 1;
                    //---- Write headers
                    worksheet.Cell("A" + rowIdx).Value = reportTitle;
                    worksheet.Range("A" + rowIdx + ":D" + rowIdx).Row(1).Merge();
                    rowIdx += 1;

                    worksheet.Cell("A" + rowIdx).Value = "reference banque";
                    worksheet.Cell("B" + rowIdx).Value = "montant paie";
                    worksheet.Cell("C" + rowIdx).Value = "reference";
                    worksheet.Cell("D" + rowIdx).Value = "type";
                    rowIdx += 1;

                    foreach (var rec in records)
                    {
                        worksheet.Cell("A" + rowIdx).Value = rec.BankRef;
                        worksheet.Cell("B" + rowIdx).Value = rec.Amount;
                        worksheet.Cell("C" + rowIdx).Value = rec.RefNo;
                        worksheet.Cell("D" + rowIdx).Value = rec.ItemType;

                        rowIdx += 1;
                    }

                    workbook.SaveAs(fileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                Util.LogError("BITPayService.CreateMairieReportFile", ex);
            }
            return false;
        }
    }
}
