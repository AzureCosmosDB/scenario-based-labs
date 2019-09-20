
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.Common
{
    public class HttpHelper
    {
        public string Accept { get; set; }
        public string ContentType { get; set; }

        public string DoGet(string url, string cookies)
        {
            String response = "";

            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "GET";

                SetupHttpRequest(req);

                if (!String.IsNullOrEmpty(cookies) && cookies != "Cookie: ")
                    req.Headers.Add(cookies);

                response = ProcessResponse(req);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return response;
        }

        public string DoPost(string url, string postData, string cookies)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            return DoPost(url, data, cookies);
        }

        public string DoPost(string url, byte[] data, string cookies)
        {
            return DoPostWork(url, data, "POST", cookies);
        }

        public string DoDelete(string url, string postData, string cookies)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            return DoDelete(url, data, cookies);
        }

        public string DoDelete(string url, byte[] data, string cookies)
        {
            return DoPostWork(url, data, "DELETE", cookies);
        }

        public string DoPostWork(string url, byte[] data, string type, string cookies)
        {
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(url);
            httpWReq.UseDefaultCredentials = true;

            httpWReq.Method = type;

            SetupHttpRequest(httpWReq);

            if (!String.IsNullOrEmpty(cookies))
                httpWReq.Headers.Add(cookies);

            httpWReq.ContentLength = data.Length;

            try
            {
                if (data.Length > 0)
                {
                    using (Stream stream = httpWReq.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            String responseString = ProcessResponse(httpWReq);

            return responseString;
        }

        public String ProcessResponse(HttpWebResponse res)
        {
            String response = "";

            try
            {
                Stream responseStream = res.GetResponseStream();

                if (res.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

                else if (res.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                response = reader.ReadToEnd();

                res.Close();
                responseStream.Close();

            }
            catch (WebException ex)
            {
                HttpWebResponse exRes = (HttpWebResponse)ex.Response;

                Stream responseStream = exRes.GetResponseStream();

                if (exRes.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

                else if (exRes.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                response = reader.ReadToEnd();                
            }
            catch (Exception) { }

            return response;
        }

        public String ProcessResponse(HttpWebRequest req)
        {
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                return ProcessResponse(res);
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                return ProcessResponse(res);
            }
            catch (Exception ex)
            {
                throw;
            }

            return "";
        }

        public void SetupHttpRequest(HttpWebRequest req)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.Expect100Continue = false;
            req.ServicePoint.Expect100Continue = false;
            req.UseDefaultCredentials = true;
            req.Timeout = 200000;
            req.ContentType = this.ContentType;
            req.Accept = this.Accept;
        }
    }

}
