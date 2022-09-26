using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BWM.LIB
{
    static public class GetDataAPIHelper
    {
        static public string APICall(string url, out string html)
        {
            html = null;
            try
            {
                string strRes = "";

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    strRes = reader.ReadToEnd();

                    reader.Close();
                    dataStream.Close();
                }
                html = strRes;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public static string ApiCallPost(string url, string contentType, string authorization, object body, out string html)
        {
            html = null;
            try
            {
                var dataWsRequest = JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.Indented);
                var myWebClient = new WebClient();
                myWebClient.Headers.Add("Content-Type", contentType);
                myWebClient.Headers.Add("Authorization", authorization);
                var byteArray = Encoding.UTF8.GetBytes(dataWsRequest);
                var responseArray = myWebClient.UploadData(url, "POST", byteArray);

                var stream = new MemoryStream(responseArray);
                using (var reader = new StreamReader(stream))
                    html = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "Exception Call API: " + ex.StackTrace;
            }

            return "";
        }

        static public string Object2SQLCommand<T>(T[] os, out string cmd) where T : class
        {
            return Object2SQLCommand(os, new { }, out cmd);
        }
        static public string Object2SQLCommand<T, T2>(T[] os, T2 moreValues, out string cmd) where T : class
        {
            string msg = "";
            cmd = null;

            if (os == null) return "Error: os is null @Object2SQLCommand";

            try
            {
                StringBuilder msb = new StringBuilder();

                foreach (var i in os)
                {
                    msg = Object2SQLCommand(i, moreValues, out string cmd1);
                    if (msg.Length > 0) return msg;

                    msb.Append(cmd1);
                }
                msb.Remove(msb.Length - 2, 2);

                cmd = msb.ToString();
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        static public string Object2SQLCommand<T>(T o, out string cmd) where T : class
        {
            return Object2SQLCommand(o, new { }, out cmd);
        }
        static public string Object2SQLCommand<T, T2>(T o, T2 moreValues, out string cmd) where T : class
        {
            string msg = "";
            cmd = null;

            if (o == null) return "Error: o is null @Object2SQLCommand";

            try
            {
                StringBuilder sb = new StringBuilder("(");
                var ps = typeof(T).GetProperties();
                foreach (var p in ps)
                {
                    var propertyValue = p.GetValue(o, null);

                    sb.Append($"'{propertyValue}', ");
                }

                ps = typeof(T2).GetProperties();
                foreach (var p in ps)
                {
                    var propertyValue = p.GetValue(moreValues, null);

                    sb.Append($"'{propertyValue}', ");
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append("), ");

                cmd = sb.ToString();
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        /// <summary>
        /// Tự động sinh câu sql có các header của table là các tên của các property của object o. 
        /// Nếu o là 1 array thì sẽ lấy theo các property của item của o
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        static public string Object2ColumnHeader<T>(T o, out string cmd) where T : class
        {
            return Object2ColumnHeader(o, new { }, out cmd);
        }
        static public string Object2ColumnHeader<T, T2>(T o, T2 moreValues, out string cmd) where T : class
        {
            string msg = "";
            cmd = null;

            if (o == null) return "Error: o is null @Object2SQLCommand";

            try
            {
                StringBuilder sb = new StringBuilder();

                var t = typeof(T);
                if (o.GetType().IsArray) t = o.GetType().GetElementType();

                var ps = t.GetProperties();
                foreach (var p in ps)
                {
                    string headerName = p.Name;

                    // Nếu property có thuộc tính thay thế tên header thì sẽ thay 
                    object[] attrs = p.GetCustomAttributes(typeof(AlterHeaderTextAttribute), false);
                    foreach (var a in attrs)
                    {
                        AlterHeaderTextAttribute dca = (AlterHeaderTextAttribute)a;
                        if (dca.alterText != "") headerName = dca.alterText;
                        break;
                    }

                    sb.Append($"'{headerName}', ");
                }

                ps = typeof(T2).GetProperties();
                foreach (var p in ps) sb.Append($"'{p.Name}', ");

                sb.Remove(sb.Length - 2, 2);

                cmd = sb.ToString();
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }


        public static string GetCookieRequest(string url, string body, string token, out string html)
           => APICallCookie(url, "GET", body, token, out html);

        public static string PostCookieRequest(string url, string body, string token, out string html)
           => APICallCookie(url, "POST", body, token, out html);
        public static string APICallCookie(string url, string method, string body, string token, out string html)
        {
            html = null;
            try
            {
                string strRes = "";

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/json";

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers["Authorization"] = $"Bearer {token}";
                    request.Headers["Cookie"] = $"JSESSIONID={token}";
                }

                if (method.ToUpper().Equals("POST"))
                {
                    request.Method = "POST";
                    var data = Encoding.Default.GetBytes(body);
                    request.ContentLength = data.Length;
                    var newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    strRes = reader.ReadToEnd();

                    reader.Close();
                    dataStream.Close();
                }
                html = strRes;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AlterHeaderTextAttribute : System.Attribute
    {
        public string alterText { get; set; }

        public AlterHeaderTextAttribute(string alterText)
        {
            this.alterText = alterText;
        }
    }
}
