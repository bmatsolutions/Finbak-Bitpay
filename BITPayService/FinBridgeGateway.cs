using BITPayService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BITPayService
{
    public class FinBridgeGateway
    {
        private string _baseUrl;
        public FinBridgeGateway(string baseUrl)
        {
            _baseUrl = baseUrl;
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

            MyHttpClient httpClient = new MyHttpClient(url, MyHttpClient.RequestType.Get, headers);
            var postResp = await httpClient.SendRequestAsync("");
            if (postResp.Success)
            {
                return JsonConvert.DeserializeObject<FinBridgeAuthResult>(postResp.Data);
            }
            else
            {
                Util.LogError("FinBridgeGateway.GetAuthTokenAsync", postResp.Exception);

                return new FinBridgeAuthResult { ErrorCode = 1, ErrorMsg = postResp.Exception.Message };
            }
        }

        public MairieService CreateMairieService(string authToken)
        {
            return new MairieService(_baseUrl,authToken);
        }

        public class MairieService
        {
            private string _baseUrl;
            private string _authToken;
            public MairieService(string baseUrl, string authToken)
            {
                _baseUrl = baseUrl;
                _authToken = authToken;
            }

            public async Task<ApiResultBaseModel> UploadFileAsync(string agent, int fileAction, string fileName, string endpoint = "file/upload/mairie")
            {
                var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + endpoint;     

                using (var form = new MultipartFormDataContent())
                {
                    //---- Add file
                    var fi = new FileInfo(fileName);
                    var data = File.ReadAllBytes(fileName);
                    HttpContent fileContent = new ByteArrayContent(data);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    form.Add(fileContent, "uploadFile", fi.Name);

                    //---- Add form data
                    string timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
                    var content = JsonConvert.SerializeObject(new { agt = agent });
                    form.Add(new StringContent(timestamp), "tsp");
                    form.Add(new StringContent("1"), "ver");
                    form.Add(new StringContent(fileAction.ToString()), "act");
                    form.Add(new StringContent(content), "content");

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", _authToken));
                        var response = await client.PostAsync(url, form);
                        var result = await response.Content.ReadAsStringAsync();

                        var respose = JsonConvert.DeserializeObject<ApiResponseModel>(result);
                        return new ApiResultBaseModel
                        {
                            Success = respose.RespStatus == 0,
                            Message = respose.Message,
                            Data = respose.Data
                        };
                    }
                }
            }
        }
    }
}
