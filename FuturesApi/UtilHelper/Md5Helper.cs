namespace FuturesApi.UtilHelper
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

	public static class Md5Helper
    {
        private static readonly char[] HexDigits = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
                                                    'A', 'B', 'C', 'D', 'E', 'F'};

        public static string BuildMySignV1(Dictionary<string, string> sArray, string secretKey)
        {
            var link = CreateLink(sArray);
            link = link + "&secret_key=" + secretKey;
            var mySign = ConvertUrlToMd5String(link);

            return mySign;
        }

        private static string CreateLink(Dictionary<string, string> paras)
        {
            var keys = new List<string>(paras.Keys);
	        var paraSort = paras.OrderBy(x => x.Key);
            var link = "";
            var i = 0;

            foreach (var kvp in paraSort)
            {
	            link = i == keys.Count - 1
		            ? link + kvp.Key + "=" + kvp.Value
		            : link + kvp.Key + "=" + kvp.Value + "&";

	            i++;
                if (i == keys.Count) break;
            }

            return link;
        }

        private static string ConvertUrlToMd5String(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return "";
            }

            var bytes = Encoding.Default.GetBytes(link);
            var md = new MD5CryptoServiceProvider();
            var sb = new StringBuilder();

            bytes = md.ComputeHash(bytes);

            foreach (var t in bytes)
            {
                sb.Append(HexDigits[(t & 0xf0) >> 4] + "" + HexDigits[t & 0xf]);
            }

            return sb.ToString();
        }

        public static void CreateUrl(ref string url, Dictionary<string, string> paras)
        {
            // TODO: do we need that stuff?
            //AddSign(ref paras);

            url = paras.Keys.Aggregate(url, (current, key) => current + (key == paras.Keys.First()
													            ? $"?{key}={paras[key]}"
													            : $"&{key}={paras[key]}"));
        }

        public static void AddSign(ref Dictionary<string, string> paras)
        {
            var sign = BuildMySignV1(paras, ConfigurationManager.AppSettings["apiPrivateKey"]);
            paras.Add("sign", sign);
        }
    }
}