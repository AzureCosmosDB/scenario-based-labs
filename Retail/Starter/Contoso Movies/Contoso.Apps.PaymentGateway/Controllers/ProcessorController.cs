using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Cors;
using Contoso.Apps.Common;
using System.Text;

namespace Contoso.Apps.PaymentGateway.Controllers
{
    public class ProcessorController : ApiController
    {
        [Route("api/nvp")]
        [HttpPost]
        public HttpResponseMessage PerformAction([FromBody] string data)
        {
            string response = string.Empty;
            HttpResponseMessage resp = new HttpResponseMessage();
            var encoder = new NVPCodec();

            try
            {
                // Deserialize the string data into a key value pair collection.
                var decoder = new NVPCodec();
                decoder.Decode(data);

                // Perform mock validation:
                var isAuthorized = ProcessorMethods.ValidateRequestCredentials(decoder);

                if (isAuthorized)
                {
                    // Determine which method is being called:
                    string method = decoder[NVPProperties.Properties.METHOD];
                    if (!string.IsNullOrWhiteSpace(method))
                    {
                        if (method.ToLower().Trim() == NVPProperties.Methods.AuthorizePayment.ToLower())
                        {
                            // Conduct the payment authorization (first step).
                            string token = string.Empty;
                            var status = ProcessorMethods.AuthorizePayment(decoder, ref token);

                            if (status)
                            {
                                // Success!
                                encoder[NVPProperties.Properties.ACK] = "success";
                                encoder[NVPProperties.Properties.TOKEN] = token;
                            }
                        }
                        else if (method.ToLower().Trim() == NVPProperties.Methods.ProcessPayment.ToLower())
                        {
                            // Conduct a final processing of the payment.
                            string transactionId = string.Empty;
                            var status = ProcessorMethods.ProcessPayment(decoder, ref transactionId);

                            if (status)
                            {
                                // Success!
                                encoder[NVPProperties.Properties.ACK] = "success";
                                encoder[NVPProperties.Properties.TRANSACTIONID] = transactionId;
                            }
                        }
                        else
                        {
                            // Invalid method requested. Throw implementation error.
                            throw new InvalidOperationException("The specified transaction method was invalid.");
                        }
                    }
                }
                else
                {
                    // Throw unauthorized response.
                    resp.StatusCode = HttpStatusCode.Unauthorized;
                    resp.Content = new StringContent("You are unauthorized to use this API. Check your credentials and try again.", Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                // TODO: Log exception; catch other exception types first
                encoder[NVPProperties.Properties.ERRORCODE] = ex.Source;
                encoder[NVPProperties.Properties.LONGMESSAGE] = ex.Message;
                encoder[NVPProperties.Properties.ACK] = "failure";
            }
            finally
            {
                // Serialize our response.
                response = encoder.Encode();
            }

            resp.StatusCode = HttpStatusCode.OK;
            resp.Content = new StringContent(response, Encoding.UTF8, "text/plain");
            return resp;
        }

        /// <summary>
        /// Simple method just to ensure the API is reachable.
        /// </summary>
        /// <returns></returns>
        [Route("api/ping")]
        [HttpGet]
        public string Ping()
        {
            return new Randomizer().GetRandomString(12);
        }
    }
}
