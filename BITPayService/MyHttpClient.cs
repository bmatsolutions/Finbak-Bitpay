using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BITPayService
{
    public class MyHttpClient
    {
        private HttpWebRequest webRequest;
        private string url;
        private RequestType requestType;

        public MyHttpClient(string url, RequestType requestType)
        {
            this.url = url;
            this.requestType = requestType;
            webRequest = (HttpWebRequest)WebRequest.Create(this.url);
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            webRequest.ContentType = "application/json";
        }

        public MyHttpClient(string url, RequestType requestType, Dictionary<string, string> headers)
        {
            this.url = url;
            this.requestType = requestType;
            webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "application/json";

            //---- Add headers
            if (headers != null)
                foreach (var h in headers)
                {
                    webRequest.Headers.Add(h.Key, h.Value);
                }
        }

        public string SendRequest(string requestData, out Exception error)
        {
            error = null;
            webRequest.Method = requestType == RequestType.Post ? "POST" : "GET";
            try
            {
                byte[] bytes = null;
                if (!string.IsNullOrEmpty(requestData))
                {
                    bytes = System.Text.Encoding.ASCII.GetBytes(requestData);
                    webRequest.ContentLength = bytes.Length;
                    using (Stream os = webRequest.GetRequestStream())
                    {
                        os.Write(bytes, 0, bytes.Length);
                    }
                }

                HttpWebResponse response;
                response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    return responseStr;
                }
                else
                {
                    error = new Exception(response.StatusDescription);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                error = ex;
                webRequest.Abort();
                return string.Empty;
            }
            finally
            {

            }
        }

        public async Task<HttpResult> SendRequestAsync(string requestData)
        {
            webRequest.Method = requestType == RequestType.Post ? "POST" : "GET";
            try
            {
                byte[] bytes = null;
                if (!string.IsNullOrEmpty(requestData))
                {
                    bytes = Encoding.ASCII.GetBytes(requestData);
                    webRequest.ContentLength = bytes.Length;
                    using (Stream os = await webRequest.GetRequestStreamAsync())
                    {
                        await os.WriteAsync(bytes, 0, bytes.Length);
                    }
                }

                HttpWebResponse response;
                response = (HttpWebResponse)await webRequest.GetResponseAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = await new StreamReader(responseStream).ReadToEndAsync();
                    return new HttpResult { Success = true, Data = responseStr };
                }
                else
                {
                    return new HttpResult { Exception = new Exception(response.StatusDescription), StatusCode = (int)response.StatusCode };
                }
            }
            catch (Exception ex)
            {
                webRequest.Abort();
                return new HttpResult { Exception = ex };
            }
        }

        public enum RequestType { Get = 0, Post = 1 }

        public class HttpResult
        {
            public bool Success { get; set; }
            public string Data { get; set; }
            public Exception Exception { get; set; }
            public int StatusCode { get; set; }
        }
    }
}
