namespace FuturesApi.UtilHelper
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public static class HttpHelper
    {
        public static string CreateUrl(string currency, string contractType)
        {
            var url = ConfigurationManager.AppSettings["OkexUrl"] + ConfigurationManager.AppSettings["FutureDepthUrl"];
            var paras = new Dictionary<string, string>
            {
                { "symbol", currency },
                { "contract_type", contractType },
                { "size", Convert.ToInt32(ConfigurationManager.AppSettings["MarketDepth"]).ToString() }
            };

            Md5Helper.CreateUrl(ref url, paras);

            return url;
        }

        public static async Task<string> TryGetData(string url, string requestMethod)
        {
            var data = string.Empty;

            try
            {
                var request = CreateRequest(url, requestMethod);
                var response = await GetResponseAsync(request);

                data = ReadResponse(response);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            
            return data;
        }

        private static async Task<WebResponse> GetResponseAsync(WebRequest request)
        {
            var response = (HttpWebResponse)await request.GetResponseAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: redirect to error page.
                throw new ArgumentException(@"Did not get a response from server.", nameof(request));
            }

            return response;
        }

        private static string ReadResponse(WebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                return reader.ReadToEnd();
            }
        }

        private static WebRequest CreateRequest(string url, string requestMethod)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(url));
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = requestMethod;

            return request;
        }
    }
}