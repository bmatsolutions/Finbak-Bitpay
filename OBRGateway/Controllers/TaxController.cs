using OBRClient;
using OBRClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OBRGateway.Controllers
{
    public class TaxController : ApiController
    {
        private TaxClient taxClient;

        public TaxController()
        {
            taxClient = new TaxClient();
            taxClient.LogFile = Util.GetLogFile();
        }

        [HttpPost]
        public async Task<RequestResponseModel> Operation(GatewayRequestData requestData)
        {
            try
            {
                var result = await taxClient.OperationRequest(requestData);
                if (result.Status == 2 || result.Status == 3)
                {
                    Util.LogError("Tax.Operation", result.Content);
                    result.Content = null;
                    result.Status = 1;
                }
                return result;
            }
            catch (Exception ex)
            {
                return Util.HandleException("Tax.Operation", ex);
            }
        }

      
    }
}
