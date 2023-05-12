using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OBRClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OBRClient
{
    public class TaxClient
    {
        public string LogFile { get; set; }
        private HttpWebRequest httpRequest;

        public async Task<RequestResponseModel> OperationRequest(GatewayRequestData requestData)
        {
            return await Task.Run(() =>
            {
                RequestResponseModel resp = new RequestResponseModel
                {
                    Status = 1,
                    Message = "Unknown response!"
                };
                try
                {
                    switch (requestData.Function)
                    {
                        case 0:
                            //---- Query declaration data
                            DeclarationQueryRequestData qData = JsonConvert.DeserializeObject<DeclarationQueryRequestData>(new JObject(requestData.Data).ToString());
                            return QueryTaxDeclaration(requestData.Url, requestData.UserName, requestData.Password, qData);
                        case 1:
                            //----- Make custom tex payment
                            MakePaymentRequestModel payData = JsonConvert.DeserializeObject<MakePaymentRequestModel>(new JObject(requestData.Data).ToString());
                            return MakePayment(requestData.Url, requestData.UserName, requestData.Password, payData);
                        case 2:
                            //---- Look Up data
                            LookUpRequestData lData=JsonConvert.DeserializeObject<LookUpRequestData>(new JObject(requestData.Data).ToString());
                            return LookUpPayment(requestData.Url, requestData.UserName, requestData.Password, lData);
                        case 3:
                            //---- Add Transaction 
                            AddTranRequestData rData = JsonConvert.DeserializeObject<AddTranRequestData>(new JObject(requestData.Data).ToString());
                            return AddTransaction(requestData.Url, requestData.UserName, requestData.Password, rData);
                        case 4:
                            //---- Tin Validation
                            ValidateTinRequestData tinData = JsonConvert.DeserializeObject<ValidateTinRequestData>(new JObject(requestData.Data).ToString());
                            return ValidateTin(requestData.Url, requestData.UserName, requestData.Password, tinData);
                        case 5:
                            //---- Query credit slip payment
                            CreditQueryRequestData cData = JsonConvert.DeserializeObject<CreditQueryRequestData>(new JObject(requestData.Data).ToString());
                            return QueryCreditDeclaration(requestData.Url, requestData.UserName, requestData.Password, cData);
                        case 6:
                            //---- Fetch tax list
                            return FetchTaxList(requestData.Url, requestData.UserName, requestData.Password);
                        case 7:
                            //----- Make custom tex payment
                            MakePaymentRequestModel bulkData = JsonConvert.DeserializeObject<MakePaymentRequestModel>(new JObject(requestData.Data).ToString());
                            return MakeBulkPayment(requestData.Url, requestData.UserName, requestData.Password, bulkData);
                        case 9:
                            //---- NIF Validation
                            ValidateNIFRequestData NifData = JsonConvert.DeserializeObject<ValidateNIFRequestData>(new JObject(requestData.Data).ToString());
                            return ValidateNIF(requestData.Url, requestData.UserName, requestData.Password, NifData);
                        case 10:
                            //---- Declarant Validation
                            ValidateDeclarantRequestData declarantData = JsonConvert.DeserializeObject<ValidateDeclarantRequestData>(new JObject(requestData.Data).ToString());
                            return ValidateDeclarant(requestData.Url, requestData.UserName, requestData.Password, declarantData);
                    }

                }
                catch (WebException e)
                {
                    WebExceptionStatus status = e.Status;
                    if (status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)e.Response;
                    }
                    resp.Message = "Request failed due to a web exception!";
                    resp.Status = 3;
                    resp.Content = e;
                }
                catch (Exception ex)
                {
                    resp.Message = "Request failed due to an exception!";
                    resp.Status = 2;
                    resp.Content = ex;
                }
                return resp;
            });
        }

        private RequestResponseModel FetchTaxList(string url, string userName, string pass)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateRequestHeader(writer);

                    //---- Write get getDeclarationDetails
                    writer.WriteStartElement("asy:fetchTaxList");
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();

                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());
                    string xml = xmlString.Replace("<asy:fetchTaxList></asy:fetchTaxList>", "<asy:fetchTaxList/>");
                    string xml2 = xml.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");

                    if (!string.IsNullOrEmpty(xml2))
                    {
                        FetchTaxList credit = new FetchTaxList();

                        var postResp = DoPost(url, userName, pass, xml2);
                       
                        postResp = postResp.Replace("</taxCode></taxCodes>", "</taxCode><info></info></taxCodes>");

                        XDocument Xml = XDocument.Parse(postResp);
                        var myData = Xml.Descendants().Where(x => x.Name.LocalName == "TaxList").FirstOrDefault();
                        if (myData != null)
                        {
                            int errorCode = (int)myData.Element("errorCode");
                            string errorMsg = (string)myData.Element("errorDescription");
                            var mySteps = (from s in myData.Descendants("taxCodes")
                                           select new TaxList
                                           {
                                               Code = s.Element("requiresDeclaration").Value+'|'+s.Element("taxCode").Value,
                                               Label = s.Element("taxCode").Value+'('+s.Element("taxDescription").Value+')'
                                           }).ToList();

                            credit.TaxLists = mySteps;
                            resp.Status = errorCode;
                            resp.Message = errorMsg;
                            resp.Content = new FetchTaxList
                            {
                                TaxLists = mySteps
                            };
                            return resp;
                        }
                        else
                        {
                            resp.Status = 1;
                            resp.Message = "Generating response data fail!";
                        }
                    }
                    else
                    {
                        resp.Status = 1;
                        resp.Message = "Generating request data failed!";
                    }
                }
            }
            return resp;
        }

        private RequestResponseModel AddTransaction(string url, string userName, string pass, AddTranRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateDomesticRequestHeader(writer);

                    //---- Write get getDeclarationDetails
                    writer.WriteStartElement("urn:AddTransaction");
                    writer.WriteAttributeString("soapenv", "encodingStyle", null, "http://schemas.xmlsoap.org/soap/encoding/");

                    writer.WriteStartElement("username");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(userName);
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("password");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(pass);
                    writer.WriteFullEndElement();
                    //< transaction_code > BIPOR </ transaction_code >
                    writer.WriteStartElement("transaction_code");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.TransactionCode);
                    writer.WriteFullEndElement();
                    //<transaction_type > 2018 </ transaction_type >
                    writer.WriteStartElement("transaction_type");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.TransactionType);
                    writer.WriteFullEndElement();
                    //<date_created ></ date_created >
                    writer.WriteStartElement("date_created");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:date");
                    writer.WriteString(model.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.WriteFullEndElement();
                    //<customer_name ></ customer_name >
                    writer.WriteStartElement("customer_name");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.CustomerName);
                    writer.WriteFullEndElement();
                    //<customer_name ></ customer_name >
                    writer.WriteStartElement("payer_name");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.PayerName);
                    writer.WriteFullEndElement();
                    //<amount > A </ amount >
                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:int");
                    writer.WriteString(model.Amount.ToString());
                    writer.WriteFullEndElement();
                    //<other_fields > 5263 </ other_fields >
                    writer.WriteStartElement("other_fields");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.OtherFields);
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();

                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    string xml = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");
                   
                    if (!string.IsNullOrEmpty(xml))
                    {
                        var postResp = DomesticPost(url, "", "", xml);
                        Payment res = new Payment();
                        XDocument resXML = XDocument.Parse(postResp);
                        var myData = resXML.Descendants().Where(x => x.Name.LocalName == "AddTransactionResponse").FirstOrDefault();
                        if (myData != null)
                        {
                            string content = (string)myData.Element("return");
                            string[] data = content.Split('|');
                            if (data[0] == "0")
                            {
                                resp.Status = 0;
                                resp.Message = "Request was successful";
                                res.ResponseCode = data[1];
                                resp.Content = new Payment
                                {
                                    ResponseCode = res.ResponseCode
                                };
                            }
                            else
                            {
                                resp.Status = Convert.ToInt32(data[0]);
                                resp.Message = data[1];
                            }
                        }
                        else
                        {
                            resp.Status = 1;
                            resp.Message = (string)myData.Element("return");
                        }

                    }
                    else
                    {
                        resp.Status = 1;
                        resp.Message = "Generating request data failed!";
                    }
                }
            }
            return resp;
        }

        private RequestResponseModel ValidateTin(string url, string userName, string pass, ValidateTinRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateDomesticRequestHeader(writer);
                    writer.WriteStartElement("urn:CheckTin");
                    writer.WriteAttributeString("soapenv", "encodingStyle", null, "http://schemas.xmlsoap.org/soap/encoding/");

                    writer.WriteStartElement("username");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(userName);
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("password");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(pass);
                    writer.WriteFullEndElement();
                    //< tin > 400202020 </ tin >
                    writer.WriteStartElement("tin");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.tin);
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();


                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    string xml = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");
                   
                    if (!string.IsNullOrEmpty(xml))
                    {
                        var postResp = DomesticPost(url, "", "", xml);
                        Tin tin = new Tin();
                        XDocument Xml = XDocument.Parse(postResp);
                        var myData = Xml.Descendants().Where(x => x.Name.LocalName == "CheckTinResponse").FirstOrDefault();
                        if (myData != null)
                        {
                            string content = (string)myData.Element("return");
                            string[] data = content.Split('|');
                            if (data[0] == "0")
                            {
                                resp.Status = 0;
                                resp.Message = "Request was successful";
                                string tins = (string)myData.Element("return");
                                string[] CustomerName = tins.Split('|');
                                tin.CustomerName = CustomerName[1];
                                resp.Content = new Tin
                                { 
                                   CustomerName = tin.CustomerName
                                };
                            }
                            else
                            {
                                resp.Status = 1;
                                resp.Message = data[1];
                            }

                        }

                    }
                    else
                    {
                        resp.Status = 1;
                        resp.Message = "Generating request data failed!";
                    }
                }
            }
            return resp;
        }
        private RequestResponseModel ValidateNIF(string url, string userName, string pass, ValidateNIFRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateValidationRequestHeader(writer);
                    writer.WriteStartElement("asy:CheckCompanyNif");
                    //   writer.WriteAttributeString("soapenv", "encodingStyle", null, "http://schemas.xmlsoap.org/soap/encoding/");

                    writer.WriteStartElement("CompanySearchInfo");
                    writer.WriteStartElement("companyNif");
                    writer.WriteString(model.nif);
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();


                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    string xml = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");

                    if (!string.IsNullOrEmpty(xml))
                    {
                        var postResp = DoPost(url, userName, pass, xml);
                        Nif nif = new Nif();
                                  XDocument Xml = XDocument.Parse(postResp);
                        var myData = Xml.Descendants().Where(x => x.Name.LocalName == "CheckCompanyNifResponse").FirstOrDefault();
                        if (myData != null)
                        {
                            if (myData.Elements().Elements().ElementAt(1).Value == "0")
                            {
                                resp.Status = 0;
                                resp.Message = myData.Elements().Elements().ElementAt(3).Value;
                            }
                            else
                            {
                                resp.Status = 1;
                                resp.Message = myData.Elements().Elements().ElementAt(2).Value;
                            }
                        }

                        else
                        {
                            resp.Status = 1;
                            resp.Message = "Request Failed";
                        }

                    }
                }
            }
            return resp;
        }

        private RequestResponseModel ValidateDeclarant(string url, string userName, string pass, ValidateDeclarantRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateValidationRequestHeader(writer);
                    writer.WriteStartElement("asy:CheckDeclarantCode");
                    writer.WriteStartElement("DeclarantSearchInfo");
                    writer.WriteStartElement("declarantCode");
                   writer.WriteString(model.declarant);
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                   writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                   writer.WriteFullEndElement();
                    writer.WriteEndDocument();


                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    string xml = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");

                    if (!string.IsNullOrEmpty(xml))
                    {
                        var postResp = DoPost(url,  userName, pass, xml);
                        Declarant declarant = new Declarant();
                        XDocument Xml = XDocument.Parse(postResp);
                        var myData = Xml.Descendants().Where(x => x.Name.LocalName == "CheckDeclarantCodeResponse").FirstOrDefault();
                        if (myData != null)
                        {
                            if (myData != null)
                            {
                                if (myData.Elements().Elements().ElementAt(1).Value == "0")
                                {
                                    resp.Status = 0;
                                    resp.Message = myData.Elements().Elements().ElementAt(3).Value;
                                }
                                else
                                {
                                    resp.Status = 1;
                                    resp.Message = myData.Elements().Elements().ElementAt(2).Value;
                                }
                            }
                        }
                        else
                        {
                            resp.Status = 1;
                            resp.Message = "Generating request data failed!";
                        }
                        }
                }
            }
            return resp;
        }
        private RequestResponseModel LookUpPayment(string url, string userName, string pass, LookUpRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateDomesticRequestHeader(writer);

                    writer.WriteStartElement("urn:Lookup");
                    writer.WriteAttributeString("soapenv", "encodingStyle", null, "http://schemas.xmlsoap.org/soap/encoding/");
                    writer.WriteStartElement("username");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(userName);
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("password");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(pass);
                    writer.WriteFullEndElement();
                    //< transaction_code > BIPOR </ transaction_code >
                    writer.WriteStartElement("transaction_code");
                    writer.WriteAttributeString("xsi", "type", null, "xsd:string");
                    writer.WriteString(model.TransactionCode);
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();

                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    string xml = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");
                   
                    if (!string.IsNullOrEmpty(xml))
                    {
                        var postResp = DomesticPost(url, "", "", xml);
                      
                        TranLookUp tin = new TranLookUp();
                        XDocument Xml = XDocument.Parse(postResp);
                        var myData = Xml.Descendants().Where(x => x.Name.LocalName == "LookupResponse").FirstOrDefault();
                        if (myData != null)
                        {
                            string content = (string)myData.Element("return");
                            string[] data = content.Split('|');

                            string ErrorCode = data[0];
                            string ErrorMsg = data[1];

                            if(ErrorCode== "111")
                            {
                                resp.Status = Convert.ToInt32(ErrorCode);
                                resp.Message = ErrorMsg;

                                return resp;
                            }


                            string TranCode = data[0];
                            string[] tranCode = TranCode.Split(':');
                            tin.TransactionCode = tranCode[1];

                            string CusName = data[1];
                            string[] cusName = CusName.Split(':');
                            tin.CustomerName = cusName[1];

                            string Date = data[2];
                            string[] date = Date.Split(':');
                            tin.TransactionDate = date[1];

                            string TranStat = data[3];
                            string[] tranStat = TranStat.Split(':');
                            tin.TransactionStatus = tranStat[1];

                            string RespStat = data[4];
                            string[] respStat = RespStat.Split(':');
                            int Status =Convert.ToInt32(respStat[1]);

                            string RespMsg = data[5];
                            string[] respMsg = RespMsg.Split(':');
                            string Message = respMsg[1];

                            resp.Status = Status;
                            resp.Message = Message;
                            resp.Content = new TranLookUp
                            {
                                TransactionCode = tin.TransactionCode,
                                TransactionDate=tin.TransactionDate,
                                CustomerName=tin.CustomerName,
                                TransactionStatus=tin.TransactionStatus
                            };

                        }
                    }
                    else
                    {
                        resp.Status = 1;
                        resp.Message = "Generating request data failed!";
                    }
                }
            }
            return resp;
        }

        private RequestResponseModel QueryTaxDeclaration(string url, string userName, string pass, DeclarationQueryRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateRequestHeader(writer);

                    //---- Write get getDeclarationDetails
                    writer.WriteStartElement("asy:getDeclarationDetails");
                    //--- Write find infor
                    writer.WriteStartElement("asy:FindInfo");

                    //--- Add
                    //< officeCode > BIPOR </ officeCode >
                    writer.WriteStartElement("officeCode");
                    writer.WriteString(model.OfficeCode);
                    writer.WriteFullEndElement();
                    //<registrationYear > 2018 </ registrationYear >
                    writer.WriteStartElement("registrationYear");
                    writer.WriteString(model.RegistrationYear);
                    writer.WriteFullEndElement();
                    //<registrationSerial ></ registrationSerial >
                    writer.WriteStartElement("registrationSerial");
                    writer.WriteString(model.RegistrationSerial);
                    writer.WriteFullEndElement();
                    //<registrationNumber ></ registrationNumber >
                    writer.WriteStartElement("registrationNumber");
                    writer.WriteString(model.RegistrationNumber);
                    writer.WriteFullEndElement();
                    //<assessmentSerial > A </ assessmentSerial >
                    writer.WriteStartElement("assessmentSerial");
                    writer.WriteString(model.AssessmentSerial);
                    writer.WriteFullEndElement();
                    //<assessmentNumber > 5263 </ assessmentNumber >
                    writer.WriteStartElement("assessmentNumber");
                    writer.WriteString(model.AssessmentNumber);
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();

                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    if (!string.IsNullOrEmpty(xmlString))
                    {
                        var postResp = DoPost(url, userName, pass, xmlString);

                        resp.Status = 0;
                        resp.Message = "";
                        resp.Content = postResp;
                    }
                    else
                    {
                        resp.Status = 1;
                        resp.Message = "Generating request data failed!";
                    }
                }
            }
            return resp;
        }

        private RequestResponseModel QueryCreditDeclaration(string url, string userName, string pass, CreditQueryRequestData model)
        {
            RequestResponseModel resp = new RequestResponseModel();

            using (StringWriter str = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(str))
                {
                    //---- Initiate writer
                    CreateRequestHeader(writer);

                    //---- Write get getDeclarationDetails
                    writer.WriteStartElement("asy:checkCreditStatement");
                    //--- Write find infor
                    writer.WriteStartElement("asy:CreditStatementFinder");

                    //--- Add
                    //< accountReference > BIPOR </ accountReference >
                    writer.WriteStartElement("accountReference");
                    writer.WriteString(model.AccountReference);
                    writer.WriteFullEndElement();
                    //<referenceYear > 2018 </ referenceYear >
                    writer.WriteStartElement("referenceYear");
                    writer.WriteString(model.ReferenceYear);
                    writer.WriteFullEndElement();
                    //<referenceNumber ></ referenceNumber >
                    writer.WriteStartElement("referenceNumber");
                    writer.WriteString(model.ReferenceNumber);
                    writer.WriteFullEndElement();
                    //<officeCode ></ officeCode >
                    writer.WriteStartElement("officeCode");
                    writer.WriteString(model.OfficeCode);
                    writer.WriteFullEndElement();
                  
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteEndDocument();

                    //----Result is a formated string.
                    string xmlString = FormatXml(str.ToString());

                    if (!string.IsNullOrEmpty(xmlString))
                    {
                        CreditQueryResponse credit = new CreditQueryResponse();

                        var postResp = DoPost(url, userName, pass, xmlString);
                       // var postResp= "<?xml version='1.0' encoding='UTF-8'?><S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\"><S:Body><ns2:checkCreditStatementResponse xmlns:ns2=\"http://www.asycuda.org\"><ns2:CreditStatementDetails><result>OK</result><errorCode>2</errorCode><errorDescription>Payment data OK</errorDescription><totalAmount>1.8453209E7</totalAmount><accountHolder>4000001463</accountHolder><companyName>null</companyName><officeCode>BIPOR</officeCode><officeName>BUJUMBURA-PORT</officeName></ns2:CreditStatementDetails></ns2:checkCreditStatementResponse></S:Body></S:Envelope>";
                        XDocument Xml = XDocument.Parse(postResp);
                        var myData = Xml.Descendants().Where(x => x.Name.LocalName == "CreditStatementDetails").FirstOrDefault();
                        if (myData != null)
                        {
                            int errorCode = (int)myData.Element("errorCode");
                            string errorMsg = (string)myData.Element("errorDescription");
                            string accHolder = (string)myData.Element("accountHolder");
                            string cmpyName = (string)myData.Element("companyName");
                            string offCode = (string)myData.Element("officeCode");
                            string offName = (string)myData.Element("officeName");
                            string ttAmt = (string)myData.Element("totalAmount");
                            string recNo = (string)myData.Element("receiptNumber") ?? "";
                            string recSerial = (string)myData.Element("receiptSerial") ?? "";
                            string date = (string)myData.Element("receiptDate") ?? "";


                            if (errorCode == 2)
                            {
                                resp.Status = 0;
                            }
                            else
                            {
                                resp.Status = errorCode;
                            }
                            resp.Message = errorMsg;
                            resp.Content = new CreditQueryResponse
                            {
                                AccountHolder = accHolder,
                                CompanyName = cmpyName,
                                OfficeCode = offCode,
                                OfficeName = offName,
                                TotalAmount = ttAmt,
                                ReceiptNo = recNo,
                                ReceiptSerial = recSerial,
                                Date = date
                            };
                            return resp;
                        }
                        else
                        {
                            resp.Status = 1;
                            resp.Message = "Generating response data fail!";
                        }
                    }
                    else
                    {
                        resp.Status = 1;
                        resp.Message = "Generating request data failed!";
                    }
                }
            }
            return resp;
        }


        private RequestResponseModel MakePayment(string url, string userName, string pass, MakePaymentRequestModel model)
        {
            RequestResponseModel resp = new RequestResponseModel
            {
                Status = 1,
                Message = "Unknown response!"
            };
            try
            {
                using (StringWriter str = new StringWriter())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(str))
                    {
                        //---- Initiate writer
                        CreateRequestHeader(writer);

                        //---- Write get getDeclarationDetails
                        if (model.Pay == "1")
                        {
                            writer.WriteStartElement("asy:declarationPayment");
                            //--- Write find infor
                            writer.WriteStartElement("asy:paymentInfo");
                        }
                        else if(model.Pay == "2"){
                            writer.WriteStartElement("asy:creditPayment");
                            //--- Write find infor
                            writer.WriteStartElement("asy:paymentInfo");
                        }
                        else if (model.Pay=="3"){
                            writer.WriteStartElement("asy:payCreditStatement");
                            //--- Write find infor
                            writer.WriteStartElement("asy:CreditStatementInfo");
                        }
                        else
                        {
                            writer.WriteStartElement("asy:payOtherPayment");
                            //--- Write find infor
                            writer.WriteStartElement("asy:otherPaymentInfo");
                           
                        }

                        if(model.Pay == "4")
                        {
                            writer.WriteStartElement("transactionId");
                            writer.WriteString(model.TranID);
                            writer.WriteFullEndElement();
                        }

                        //--- Add elements
                        if (model.Pay != "3")
                        {
                            //---officeCode 
                            writer.WriteStartElement("officeCode");
                            writer.WriteString(model.OfficeCode);
                            writer.WriteFullEndElement();
                        }
                        
                        //---declarantCode or accountReference
                        //0 is for cash payment ,1 is for credit payment
                        if (model.Pay == "1" || model.Pay=="4")
                        {
                            writer.WriteStartElement("declarantCode");
                            writer.WriteString(model.DeclarantCode);
                            writer.WriteFullEndElement();
                        }
                        else if(model.Pay=="2"){
                            writer.WriteStartElement("accountReference");
                            writer.WriteString(model.DeclarantCode);
                            writer.WriteFullEndElement();
                        }else if (model.Pay == "3")
                        {
                            writer.WriteStartElement("accountReference");
                            writer.WriteString(model.AccountReference);
                            writer.WriteFullEndElement();
                        }

                        string amount = Math.Round(Double.Parse(model.AmountToBePaid), 0) + "";
                        if (model.Pay == "1" || model.Pay =="2")
                        {
                            //---companyCode
                            writer.WriteStartElement("companyCode");
                            writer.WriteString(model.CompanyCode);
                            writer.WriteFullEndElement();

                            //<<<<< START: declarationsToBePaid >>>>>>>
                            writer.WriteStartElement("declarationsToBePaid");
                            //---amountToBePaid
                            writer.WriteStartElement("amountToBePaid");
                            writer.WriteString(amount);
                            writer.WriteFullEndElement();
                            //---registrationYear
                            writer.WriteStartElement("registrationYear");
                            writer.WriteString(model.RegistrationYear);
                            writer.WriteFullEndElement();
                            //---registrationSerial
                            writer.WriteStartElement("registrationSerial");
                            writer.WriteString(model.RegistrationSerial);
                            writer.WriteFullEndElement();
                            //---registrationNumber
                            writer.WriteStartElement("registrationNumber");
                            writer.WriteString(model.RegistrationNumber);
                            writer.WriteFullEndElement();
                            //---assessmentSerial
                            writer.WriteStartElement("assessmentSerial");
                            writer.WriteString(model.AssessmentSerial);
                            writer.WriteFullEndElement();
                            //---assessmentNumber
                            writer.WriteStartElement("assessmentNumber");
                            writer.WriteString(model.AssessmentNumber);
                            writer.WriteFullEndElement();

                            writer.WriteFullEndElement();
                            //<<<<< END: declarationsToBePaid >>>>>>>
                        }
                        else if(model.Pay=="3")
                        {
                            //---accountHolder 
                            writer.WriteStartElement("accountHolder");
                            writer.WriteString(model.AccountHolder);
                            writer.WriteFullEndElement();
                            //---referrenceYear 
                            writer.WriteStartElement("referenceYear");
                            writer.WriteString(model.ReferenceYear);
                            writer.WriteFullEndElement();
                            //---referenceNumber 
                            writer.WriteStartElement("referenceNumber");
                            writer.WriteString(model.ReferenceNumber);
                            writer.WriteFullEndElement();
                            //---officeCode 
                            writer.WriteStartElement("officeCode");
                            writer.WriteString(model.OfficeCode);
                            writer.WriteFullEndElement();
                            //---amountToBePaid
                            writer.WriteStartElement("amountToBePaid");
                            writer.WriteString(amount);
                            writer.WriteFullEndElement();
                        }else
                        {
                            //---companyCode
                            writer.WriteStartElement("companyCode");
                            writer.WriteString(model.CompanyCode);
                            writer.WriteFullEndElement();

                            //---taxPayerName
                            writer.WriteStartElement("taxPayerName");
                            writer.WriteString(model.TaxPayerName);
                            writer.WriteFullEndElement();

                            //<<<<< START: transactionsToBePaid >>>>>>>
                            writer.WriteStartElement("transactionsToBePaid");
                            //---transactionsCode
                            writer.WriteStartElement("transactionCode");
                            writer.WriteString(model.TransactionCode);
                            writer.WriteFullEndElement();
                            //---referenceYear
                            writer.WriteStartElement("referenceYear");
                            writer.WriteString(model.RegistrationYear);
                            writer.WriteFullEndElement();
                            //---referenceOffice
                            writer.WriteStartElement("referenceOffice");
                            writer.WriteString(model.OfficeCode);
                            writer.WriteFullEndElement();
                            //---referenceSerial
                            writer.WriteStartElement("referenceSerial");
                            writer.WriteString(model.AssessmentSerial);
                            writer.WriteFullEndElement();
                            //---referenceNumber
                            writer.WriteStartElement("referenceNumber");
                            writer.WriteString(model.AssessmentNumber);
                            writer.WriteFullEndElement();
                            //---transactionReference
                            writer.WriteStartElement("transactionReference");
                            writer.WriteString(model.TransactionReference);
                            writer.WriteFullEndElement();
                            //---transactionAmount
                            writer.WriteStartElement("transactionAmount");
                            writer.WriteString(amount);
                            writer.WriteFullEndElement();
                            //---currencyCode
                            writer.WriteStartElement("currencyCode");
                            writer.WriteString(model.Currency);
                            writer.WriteFullEndElement();

                            writer.WriteFullEndElement();
                            //<<<<< END: transactionsToBePaid >>>>>>>
                        }

                        //<<<<< START: meansOfPayment >>>>>>>
                        writer.WriteStartElement("meansOfPayment");
                        //---meanOfPayment
                        writer.WriteStartElement("meanOfPayment");
                        writer.WriteString(model.MeanOfPayment);
                        writer.WriteFullEndElement();
                        //---bankCode
                        writer.WriteStartElement("bankCode");
                        writer.WriteString(model.BankCode);
                        writer.WriteFullEndElement();
                        //---checkReference
                        writer.WriteStartElement("checkReference");
                        writer.WriteString(model.CheckReference);
                        writer.WriteFullEndElement();
                        //---amount
                        writer.WriteStartElement("amount");
                        writer.WriteString(amount);
                        writer.WriteFullEndElement();

                        if(model.Pay=="4")
                        {
                            //---currency
                            writer.WriteStartElement("currency");
                            writer.WriteString(model.Currency);
                            writer.WriteFullEndElement();
                        }

                        writer.WriteFullEndElement();
                        //<<<<< END: meansOfPayment >>>>>>>

                        if(model.Pay != "3")
                        {
                            //----paymentDate
                            writer.WriteStartElement("paymentDate");
                            writer.WriteString(model.PaymentDate);
                            writer.WriteFullEndElement();
                        }
                        
                        //----refund
                        writer.WriteStartElement("refund");
                        writer.WriteString(model.Refund);
                        writer.WriteFullEndElement();

                        //----- End elements
                        writer.WriteFullEndElement();
                        writer.WriteFullEndElement();
                        writer.WriteFullEndElement();
                        writer.WriteFullEndElement();
                        writer.WriteEndDocument();

                        //----Result is a formated string.
                        string xmlString = FormatXml(str.ToString());
                       
                        if (model.Pay=="2" || model.Pay== "4")
                        {
                            string xml = xmlString.Replace("<assessmentSerial></assessmentSerial>", "<assessmentSerial/>");
                            string xml2 = xml.Replace("<assessmentNumber></assessmentNumber>", "<assessmentNumber/>");
                            string xml3 = xml2.Replace("<referenceYear>0</referenceYear>", "<referenceYear/>");
                            string xml4 = xml3.Replace("<referenceOffice></referenceOffice>", "<referenceOffice/>");
                            string xml5 = xml4.Replace("<referenceSerial></referenceSerial>", "<referenceSerial/>");
                            string xml6 = xml5.Replace("<referenceNumber></referenceNumber>", "<referenceNumber/>");
                            string xml7 = xml6.Replace("<taxPayerName></taxPayerName>", "<taxPayerName/>");
                            string xml9 = "";
                            string xml8 = xml7.Replace("<declarantCode></declarantCode>", "<declarantCode/>");
                            xml9 = xml8.Replace("<companyCode></companyCode>", "<companyCode/>");

                            xmlString = xml9;
                        }

                        string xml10 = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");
                        xmlString = xml10;

                        if (!string.IsNullOrEmpty(xmlString))
                        {
                            var postResp = DoPost(url, userName, pass, xmlString);

                            resp.Status = 0;
                            resp.Message = "";
                            resp.Content = postResp;
                        }
                        else
                        {
                            resp.Message = "Generating request data failed!";
                        }
                    }
                }
            }
            catch (WebException e)
            {
                WebExceptionStatus status = e.Status;
                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)e.Response;
                }
                resp.Status = 3;
                resp.Content = e;
            }
            catch (Exception ex)
            {
                resp.Status = 2;
                resp.Content = ex;
            }
            return resp;
        }

        private RequestResponseModel MakeBulkPayment(string url, string userName, string pass, MakePaymentRequestModel model)
        {
            RequestResponseModel resp = new RequestResponseModel
            {
                Status = 1,
                Message = "Unknown response!"
            };
            try
            {
                using (StringWriter str = new StringWriter())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(str))
                    {
                        //---- Initiate writer
                        CreateRequestHeader(writer);

                        //---- Write get getDeclarationDetails
                        if (model.Pay == "6")
                        {
                            writer.WriteStartElement("asy:declarationPayment");
                            //--- Write find infor
                            writer.WriteStartElement("asy:paymentInfo");
                        }
                        else if (model.Pay == "7")
                        {
                            writer.WriteStartElement("asy:creditPayment");
                            //--- Write find infor
                            writer.WriteStartElement("asy:paymentInfo");
                        }
                        else if (model.Pay == "9")
                        {
                            writer.WriteStartElement("asy:payOtherPayment");
                            //--- Write find infor
                            writer.WriteStartElement("asy:otherPaymentInfo");

                        }

                        if (model.Pay == "9")
                        {
                            writer.WriteStartElement("transactionId");
                            writer.WriteString(model.TranID);
                            writer.WriteFullEndElement();
                        }
                        //--- Add elements
                        //---officeCode 
                        writer.WriteStartElement("officeCode");
                        writer.WriteString(model.OfficeCode);
                        writer.WriteFullEndElement();

                        if (model.Pay == "6" || model.Pay == "9")
                        {
                            //---declarantcode
                            writer.WriteStartElement("declarantCode");
                            writer.WriteString(model.DeclarantCode);
                            writer.WriteFullEndElement();
                        }
                        else if (model.Pay == "7")
                        {
                            //-----Account Reference
                            writer.WriteStartElement("accountReference");
                            writer.WriteString(model.DeclarantCode);
                            writer.WriteFullEndElement();
                        }
                       

                        //---companyCode
                        writer.WriteStartElement("companyCode");
                        writer.WriteString(model.CompanyCode);
                        writer.WriteFullEndElement();

                        string amount = Math.Round(Double.Parse(model.AmountToBePaid), 0) + "";

                        if  (model.Pay == "6"|| model.Pay == "7")
                        {
                            foreach (var cust in model.DeclarationList)
                            {
                                //<<<<< START: declarationsToBePaid >>>>>>>
                                writer.WriteStartElement("declarationsToBePaid");
                                string amt = Math.Round(Double.Parse(cust.AmountToBePaid), 0) + "";
                                writer.WriteElementString("amountToBePaid", amt);
                                writer.WriteElementString("registrationYear", cust.RegistrationYear);
                                writer.WriteElementString("registrationSerial", cust.RegistrationSerial);
                                writer.WriteElementString("registrationNumber", cust.RegistrationNumber);
                                writer.WriteElementString("assessmentSerial", cust.AssessmentSerial);
                                writer.WriteElementString("assessmentNumber", cust.AssessmentNumber);
                                //<<<<< END: declarationsToBePaid >>>>>>>
                                writer.WriteEndElement();
                            }
                        }else if (model.Pay == "9")
                        {
                            //---taxPayerName
                            writer.WriteStartElement("taxPayerName");
                            writer.WriteString(model.TaxPayerName);
                            writer.WriteFullEndElement();

                            foreach (var cust in model.DeclarationList)
                            {
                                //<<<<< START: declarationsToBePaid >>>>>>>
                                writer.WriteStartElement("transactionsToBePaid");
                                string amt = Math.Round(Double.Parse(cust.AmountToBePaid), 0) + "";
                                writer.WriteElementString("transactionCode", cust.TransactionCode);
                                writer.WriteElementString("referenceYear", cust.RegistrationYear);
                                writer.WriteElementString("referenceOffice", cust.OfficeCode);
                                writer.WriteElementString("referenceSerial", cust.AssessmentSerial);
                                writer.WriteElementString("referenceNumber", cust.AssessmentNumber);
                                writer.WriteElementString("transactionReference", cust.TransactionReference);
                                writer.WriteElementString("transactionAmount", amt);
                                writer.WriteElementString("currencyCode", cust.Currency);
                                //<<<<< END: declarationsToBePaid >>>>>>>
                                writer.WriteEndElement();
                            }
                        }
                        
                        

                        //<<<<< START: meansOfPayment >>>>>>>
                        writer.WriteStartElement("meansOfPayment");
                        //---meanOfPayment
                        writer.WriteStartElement("meanOfPayment");
                        writer.WriteString(model.MeanOfPayment);
                        writer.WriteFullEndElement();
                        //---bankCode
                        writer.WriteStartElement("bankCode");
                        writer.WriteString(model.BankCode);
                        writer.WriteFullEndElement();
                        //---checkReference
                        writer.WriteStartElement("checkReference");
                        writer.WriteString(model.CheckReference);
                        writer.WriteFullEndElement();
                        //---amount
                        writer.WriteStartElement("amount");
                        writer.WriteString(amount);
                        writer.WriteFullEndElement();
                        if(model.Pay == "9")
                        {
                            //---currencyCode
                            writer.WriteStartElement("currency");
                            writer.WriteString(model.Currency);
                            writer.WriteFullEndElement();
                        }
                        

                        writer.WriteFullEndElement();
                        //<<<<< END: meansOfPayment >>>>>>>

                        //----paymentDate
                        writer.WriteStartElement("paymentDate");
                        writer.WriteString(model.PaymentDate);
                        writer.WriteFullEndElement();

                        //----refund
                        writer.WriteStartElement("refund");
                        writer.WriteString(model.Refund);
                        writer.WriteFullEndElement();

                        //----- End elements
                        writer.WriteFullEndElement();
                        writer.WriteFullEndElement();
                        writer.WriteEndDocument();

                        //----Result is a formated string.
                        string xmlString = FormatXml(str.ToString());


                        if (model.Pay == "7"||model.Pay=="9")
                        {
                            string xml = xmlString.Replace("<assessmentSerial></assessmentSerial>", "<assessmentSerial/>");
                            string xml2 = xml.Replace("<assessmentNumber></assessmentNumber>", "<assessmentNumber/>");
                            string xml3 = xml2.Replace("<referenceYear></referenceYear>", "<referenceYear/>");
                            string xml4 = xml3.Replace("<referenceOffice></referenceOffice>", "<referenceOffice/>");
                            string xml5 = xml4.Replace("<referenceSerial></referenceSerial>", "<referenceSerial/>");
                            string xml6 = xml5.Replace("<referenceNumber></referenceNumber>", "<referenceNumber/>");
                            string xml7 = xml6.Replace("<taxPayerName></taxPayerName>", "<taxPayerName/>");
                            string xml9 = "";
                           
                                string xml8 = xml7.Replace("<declarantCode></declarantCode>", "<declarantCode/>");
                                xml9 = xml8.Replace("<companyCode></companyCode>", "<companyCode/>");
                            

                            xmlString = xml9;
                        }

                        string xml10 = xmlString.Replace("<soapenv:Header></soapenv:Header>", "<soapenv:Header/>");
                        xmlString = xml10;

                        if (!string.IsNullOrEmpty(xmlString))
                        {
                            var postResp = DoPost(url, userName, pass, xmlString);

                            resp.Status = 0;
                            resp.Message = "";
                            resp.Content = postResp;
                        }
                        else
                        {
                            resp.Message = "Generating request data failed!";
                        }
                    }
                }
            }
            catch (WebException e)
            {
                WebExceptionStatus status = e.Status;
                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)e.Response;
                }
                resp.Status = 3;
                resp.Content = e;
            }
            catch (Exception ex)
            {
                resp.Status = 2;
                resp.Content = ex;
            }
            return resp;
        }

        private RequestResponseModel GetDeclarationByCode()
        {
            RequestResponseModel resp = new RequestResponseModel
            {
                Status = 1,
                Message = "Unknown response!"
            };

            return resp;
        }

        private RequestResponseModel CheckPayment()
        {
            RequestResponseModel resp = new RequestResponseModel
            {
                Status = 1,
                Message = "Unknown response!"
            };

            return resp;
        }

        private string DoPost(string url, string userName, string password, string postData)
        {
            Util.LogError(LogFile, "OBRClient.DoPost", new Exception(postData), false);

            httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml;charset=UTF-8";
            httpRequest.Headers.Add("SOAPAction", url);
            httpRequest.KeepAlive = false;
            httpRequest.ProtocolVersion = HttpVersion.Version10;

            NetworkCredential credential = new NetworkCredential();
            credential.UserName = userName;
            credential.Password = password;
            httpRequest.Credentials = credential;

            byte[] reqBytes = new UTF8Encoding().GetBytes(postData);
            httpRequest.ContentLength = reqBytes.Length;

            using (Stream reqStream = httpRequest.GetRequestStream())
            {
                reqStream.Write(reqBytes, 0, reqBytes.Length);
            }

            //---- Get response
            string xmlResponse = null;
            HttpWebResponse resp = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                xmlResponse = sr.ReadToEnd();

                //========== Log response ==========
                Util.LogError(LogFile,"TaxClient.DoPost:Respose", new Exception(xmlResponse));
                //==================================

                return xmlResponse;
            }
        }

        private string DomesticPost(string url, string userName, string password, string postData)
        {
            Util.LogError(LogFile, "OBRClient.DomesticPost", new Exception(postData), false);

            httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml";
            httpRequest.Headers.Add("SOAPAction", url);
            httpRequest.KeepAlive = false;
            httpRequest.ProtocolVersion = HttpVersion.Version10;

            ////NetworkCredential credential = new NetworkCredential();
            ////credential.UserName = userName;
            ////credential.Password = password;
            ////httpRequest.Credentials = credential;

            byte[] reqBytes = new UTF8Encoding().GetBytes(postData);
            httpRequest.ContentLength = reqBytes.Length;

            using (Stream reqStream = httpRequest.GetRequestStream())
            {
                reqStream.Write(reqBytes, 0, reqBytes.Length);
            }

            //---- Get response
            string xmlResponse = null;
            HttpWebResponse resp = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                xmlResponse = sr.ReadToEnd();

                //========== Log response ==========
                Util.LogError(LogFile, "TaxClient.DomesticTaxPost:Respose", new Exception(xmlResponse));
                //==================================

                return xmlResponse;
            }
        }

        private void CreateRequestHeader(XmlTextWriter writer)
        {
            //---- Write root
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv:Envelope");
            writer.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope/");
            writer.WriteAttributeString("xmlns", "asy", null, "http://www.asycuda.org");
            //---- Write header
            writer.WriteStartElement("soapenv:Header");
            writer.WriteFullEndElement();
            //---- Write body
            writer.WriteStartElement("soapenv:Body");
        }

        private void CreateDomesticRequestHeader(XmlTextWriter writer)
        {
            //---- Write root
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv:Envelope");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            writer.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope/");
            writer.WriteAttributeString("xmlns", "urn", null, "urn:server");
            //---- Write header
            writer.WriteStartElement("soapenv:Header");
            writer.WriteFullEndElement();
            //---- Write body
            writer.WriteStartElement("soapenv:Body");
        }
        private void CreateValidationRequestHeader(XmlTextWriter writer)
        {
            //---- Write root
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv:Envelope");
            writer.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope/");
            writer.WriteAttributeString("xmlns", "asy", null, "http://www.asycuda.org");
                     //---- Write header
            writer.WriteStartElement("soapenv:Header");
            writer.WriteFullEndElement();
            //---- Write body
            writer.WriteStartElement("soapenv:Body");
        }
        private string FormatXml(string xml)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                var element = XDocument.Parse(xml);

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Indent = true;
                writerSettings.NewLineHandling = NewLineHandling.None;

                using (var xmlWriter = XmlWriter.Create(stringBuilder, writerSettings))
                    element.Save(xmlWriter);

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Util.LogError(LogFile, "Format xml", new Exception(ex.Message + "||"+xml));
                return xml;
            }
        }
    }
}
