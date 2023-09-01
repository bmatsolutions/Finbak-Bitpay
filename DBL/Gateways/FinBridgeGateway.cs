using BITPay.DBL.Models;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static BITPay.DBL.HttpClient;

namespace BITPay.DBL.Gateways
{
    public class FinBridgeGateway
    {
        private string _baseUrl;
        private string _logFile;
        public FinBridgeGateway(string baseUrl, string logFile)
        {
            _baseUrl = baseUrl;
            _logFile = logFile;
        }

        public async Task<FinBridgeAuthResult> GetAuthTokenAsync(string appId, string appKey, string endpoint = "auth")
        {
            var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

            //--- Generate signature
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string signature = DateTime.UtcNow.ToString("yyMMddHHmmss") + appKey;
            signature = Convert.ToBase64String(encoding.GetBytes(signature));

            //--- Creare auth header
            var authHeader = string.Format("{0}{1}{2}", appId.Length.ToString().PadLeft(3, '0'), appId, signature);
            var head = "Basic " + Convert.ToBase64String(encoding.GetBytes(authHeader));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", head);

            HttpClient httpClient = new HttpClient(url, RequestType.Post, headers);
            var postResp = await httpClient.SendRequestAsync("");
            if (postResp.Success)
            {
                return JsonConvert.DeserializeObject<FinBridgeAuthResult>(postResp.Data);
            }
            else
            {
                LogUtil.Error(_logFile, "Bl.GetAuthTokenAsync", postResp.Exception);

                return new FinBridgeAuthResult { ErrorCode = 1, ErrorMsg = postResp.Exception.Message };
            }            
        }

        public async Task<FinBridgeAuthResult> GetFinBridgeAuthAsync(string appId, string appKey, string endpoint = "auth")
        {
            var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;
            //---- Create header
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var header = DateTime.UtcNow.ToString("yyMMddHHmmss") + appKey;
            var signature = Convert.ToBase64String(encoding.GetBytes(header));
            var idLen = appId.Length.ToString().PadLeft(3, '0');
            header = idLen + appId + signature;
            header = Convert.ToBase64String(encoding.GetBytes(header));
            var authHeader = "Basic " + header;

            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", authHeader);

            //---- Make http call
            HttpClient httpClient = new HttpClient(url, RequestType.Post, headers);
            var postResp = await httpClient.SendRequestAsync("");
            if (postResp.Success)
            {
                return JsonConvert.DeserializeObject<FinBridgeAuthResult>(postResp.Data);
            }
            else
            {
                LogUtil.Error(_logFile, "Bl.GetAuthTokenAsync", postResp.Exception);

                return new FinBridgeAuthResult { ErrorCode = 1, ErrorMsg = postResp.Exception.Message };
            }
        }

        public MairieService CreateMairieService(string authToken)
        {
            return new MairieService(_baseUrl, _logFile, authToken);
        }
        public RegidesoService CreateRegidesoService(string authToken)
        {
            return new RegidesoService(_baseUrl, _logFile, authToken);
        }

        public class MairieService
        {
            private string _baseUrl;
            private string _logFile;
            private string _authToken;
            public MairieService(string baseUrl, string logFile, string authToken)
            {
                _baseUrl = baseUrl;
                _logFile = logFile;
                _authToken = authToken;
            }

            public async Task<MairieQueryTaxTypeDataResult> GetTaxTypesAsync(int groupCode, string endpoint = "service/call/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { code = groupCode };
                var result = await MakeRequestAsync(url, 511, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = new MairieQueryTaxTypeDataResult { Success = true };
                    var types = JsonConvert.DeserializeObject<MairieListItemModel[]>(new JArray(result.Data).ToString());
                    queryResp.Types = types;
                    return queryResp;
                }
                else
                {
                    return new MairieQueryTaxTypeDataResult { RespMessage = result.Message };
                }
            }

            public async Task<MairieQueryTaxTypeDataResult> GetTaxTypeFieldsAsync(int typeCode, string endpoint = "service/call/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { code = typeCode };
                var result = await MakeRequestAsync(url, 512, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = new MairieQueryTaxTypeDataResult { Success = true };
                    var fields = JsonConvert.DeserializeObject<MarieDataFieldModel[]>(new JArray(result.Data).ToString());
                    queryResp.Fields = fields;
                    return queryResp;
                }
                else
                {
                    return new MairieQueryTaxTypeDataResult { RespMessage = result.Message };
                }
            }

            public async Task<MairieTaxNoteDataModel> QueryTaxItemAsync(int taxType, string referenceNo, string endpoint = "service/call/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { ref_no = referenceNo };
                int action = taxType == 1 ? 501 : 502;
                var result = await MakeRequestAsync(url, action, queryData);
                if (result.RespStatus == 0)
                {
                    var taxData = JsonConvert.DeserializeObject<MairieTaxNoteDataModel>(new JObject(result.Data).ToString());
                    taxData.Success = true;
                    taxData.TaxType = taxType;
                    return taxData;
                }
                else
                {
                    return new MairieTaxNoteDataModel { RespMessage = result.Message };
                }
            }

            public async Task<MairieTaxNoteDataModel> QueryTaxItemAsync(int taxType, QueryMarie queryData, string endpoint = "service/call/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                 int action = taxType == 1 ? 508 : 509;
                var result = await MakeRequestAsync(url, action, queryData);
                if (result.RespStatus == 0)
                {
                    var taxData = JsonConvert.DeserializeObject<MairieTaxNoteDataModel>(new JObject(result.Data).ToString());
                    taxData.Success = true;
                    taxData.TaxType = taxType;
                    return taxData;
                }
                else
                {
                    return new MairieTaxNoteDataModel { RespMessage = result.Message };
                }
            }

            public async Task<MairieTaxPayResultModel> MakeTaxPaymentAsync(MairieTaxNotePaymentModel payment, string endpoint = "service/call/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                 int action = payment.TaxType == 1 ? 503 : 504;
                var result = await MakeRequestAsync(url, action, payment);
                if (result.RespStatus == 0)
                {
                    var taxData = JsonConvert.DeserializeObject<MairieTaxPayResultModel>(new JObject(result.Data).ToString());
                    taxData.Success = true;
                    return taxData;
                }
                else
                {
                    return new MairieTaxPayResultModel { RespMessage = result.Message };
                }
            }

            public async Task<MairieTaxpayerModel> QueryTaxpayerAsync(string taxpayerNo, string endpoint = "service/call/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { tid = taxpayerNo };
                var result = await MakeRequestAsync(url, 500, queryData);
                if (result.RespStatus == 0)
                {
                    var taxData = JsonConvert.DeserializeObject<MairieTaxpayerModel>(new JObject(result.Data).ToString());
                    taxData.Success = true;
                    return taxData;
                }
                else
                {
                    return new MairieTaxpayerModel { RespMessage = result.Message };
                }
            }
           
            private async Task<FinBridgeApiResponseModel> MakeRequestAsync(string url, int action, dynamic data, RequestType requestType = RequestType.Post)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", "Basic " + _authToken);

                string timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
                var requestData = new FinBridgeApiRequest
                {
                    Timestamp = timestamp,
                    version = 1,
                    action = action,
                    Data = data
                };

                var postData = JsonConvert.SerializeObject(requestData);
                HttpClient httpClient = new HttpClient(url, requestType, headers);
                var result = await httpClient.SendRequestAsync(postData);
                if (result.Success)
                {
                    return JsonConvert.DeserializeObject<FinBridgeApiResponseModel>(result.Data);
                }
                else
                {
                    LogUtil.Error(_logFile, "FinBridgeGateway.MakeRequestAsync()", result.Exception);
                    return new FinBridgeApiResponseModel
                    {
                        RespStatus = 1,
                        Message = result.Exception.Message
                    };
                }
            }
        }

        public class RegidesoService
        {
            private string _baseUrl;
            private string _logFile;
            private string _authToken;
            public RegidesoService(string baseUrl, string logFile, string authToken)
            {
                _baseUrl = baseUrl;
                _logFile = logFile;
                _authToken = authToken;
            }

            public async Task<RegidesoModel> GetPaymentBillsAsync(string accnt_No, string endpoint = "service/call/regideso")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { accnt_No = accnt_No };
                var result = await MakeRequestAsync(url, 900, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = JsonConvert.DeserializeObject<RegidesoModel>(new JObject(result.Data).ToString());
                    queryResp.Success = true;
                    return queryResp;
                }
                else
                {
                    return new RegidesoModel { RespMessage = result.Message };
                }
            }
            public async Task<Bills> PayBillsAsync(string Invoice_no,  string txnId, string PhoneNo, string TransanctionId,string endpoint = "service/call/regideso")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { invoice_no = Invoice_no,txnid = txnId, phoneno= PhoneNo, TRANS_ID = TransanctionId, operation_id = txnId};
                var result = await MakeRequestAsync(url, 901, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = JsonConvert.DeserializeObject<Bills>(new JObject(result.Data).ToString()); 
                    queryResp.Success = true;
                    return queryResp;
                }
                else
                {
                    return new Bills { Msg = result.Message };
                }
            }
            public async Task<PrePaidModel> RequestPrePayDetailsAsync(PrePaidModel model, string endpoint = "service/call/regideso")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;
                
                var queryData = new {  meter_no = model.Meter_No };
                var result = await MakeRequestAsync(url, 903, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = JsonConvert.DeserializeObject<PrePaidModel>(new JObject(result.Data).ToString());
                    queryResp.Success = true;

                    return queryResp;
                }
                else
                {
                    return new PrePaidModel { Msg = result.Message };
                }
            }
            public async Task<QueryDetails> RequestAmountAsync(int units,string Meter_No, string endpoint = "service/call/regideso")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { meter_no = Meter_No, units = units };
                var result = await MakeRequestAsync(url, 904, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = JsonConvert.DeserializeObject<QueryDetails>(new JObject(result.Data).ToString());
                    queryResp.Success = true;

                    return queryResp;
                }
                else
                {
                    return new QueryDetails { Msg = result.Message };
                }
            }
            public async Task<QueryDetails> RequestTokenAsync(decimal amount,string meterno, string endpoint = "service/call/regideso")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new { meter_no = meterno, amnt = amount };
                var result = await MakeRequestAsync(url, 905, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = JsonConvert.DeserializeObject<QueryDetails>(new JObject(result.Data).ToString());
                    queryResp.Success = true;

                    return queryResp;
                }
                else
                {
                    return new QueryDetails { Msg = result.Message };
                }
            }
            public async Task<PrePaidModel> RequestPrePayAsync(PrePaidModel model, string endpoint = "service/call/regideso")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;

                var queryData = new {meter_no = model.Meter_No,amnt = model.Amount, TRANS_ID = model.CBSRef, telephon=model.PhoneNo, operation_id = model.BillCode.ToString() };
                var result = await MakeRequestAsync(url, 902, queryData);
                if (result.RespStatus == 0)
                {
                    var queryResp = JsonConvert.DeserializeObject<PrePaidModel>(new JObject(result.Data).ToString());
                    queryResp.Success = true;
                    return queryResp;
                }
                else
                {
                    return new PrePaidModel { Msg = result.Message };
                }
            }
            private async Task<FinBridgeApiResponseModel> MakeRequestAsync(string url, int action, dynamic data, RequestType requestType = RequestType.Post)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", "Basic " + _authToken);

                string timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
                var requestData = new FinBridgeApiRequest
                {
                    Timestamp = timestamp,
                    version = 1,
                    action = action,
                    Data = data
                };
                var postData = JsonConvert.SerializeObject(requestData);
                HttpClient httpClient = new HttpClient(url, requestType, headers);
                var result = await httpClient.SendRequestAsync(postData);
                if (result.Success)
                {
                    return JsonConvert.DeserializeObject<FinBridgeApiResponseModel>(result.Data);
                }
                else
                {
                    LogUtil.Error(_logFile, "FinBridgeGateway.MakeRequestAsync()", result.Exception);
                    return new FinBridgeApiResponseModel
                    {
                        RespStatus = 1,
                        Message = result.Exception.Message
                    };
                }
            }

        }
    }
}
