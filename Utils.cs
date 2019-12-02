using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Migration
{
    public static class Utils
    {
        //Lofg4Net declare log variable
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //////////////////////////////////////////
        public static string WebRequestCall(string data, string apiUrl, string apiMethod, string apiContentType, string apiAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string result_Final_NotOK)
        {
            StatusCodeFinal = string.Empty;
            StatusDescriptionFinal = string.Empty;
            result_Final_NotOK = string.Empty;
            string result = string.Empty;
            resultFinal = string.Empty;
            try
            {
                log.Info("Start getting result ");
                /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.Timeout = 18000000; //18000sec 5hours
                httpWebRequest.ContentType = apiContentType;
                httpWebRequest.Method = apiMethod;

                /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
                httpWebRequest.ProtocolVersion = HttpVersion.Version10;
                httpWebRequest.PreAuthenticate = true;

                if (apiAuth != string.Empty)
                {
                    httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(apiAuth));
                }

                if (data != string.Empty)
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }
                }

                try
                {
                    log.Info("Before getting response. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    log.Info("After getting response. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                    GetStatusAndDescriptionCode(httpResponse, out string StatusCode, out string StatusDescription);
                    StatusCodeFinal = StatusCode;
                    StatusDescriptionFinal = StatusDescription;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var res = streamReader.ReadToEnd();
                        resultFinal = res;
                        log.Info("Result is " + res);
                    }
                }
                catch (WebException ex)
                {
                    log.Info("Web exception happened: " + ex.Message);
                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse1 = (HttpWebResponse)response;
                        GetStatusAndDescriptionCode(httpResponse1, out string StatusCode, out string StatusDescription);
                        StatusCodeFinal = StatusCode;
                        StatusDescriptionFinal = StatusDescription;
                        using (var stream = ex.Response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                            result_Final_NotOK = result;
                            log.Info("Web exception message: " + result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error getting result: " + ex.Message + " ||| " + ex.InnerException + " ||| " + ex.StackTrace);
                    //throw new Exception("Error getting result: " + ex.Message + " ||| " + ex.InnerException + " ||| " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in function WebRequestCall. " + ex.Message);
                //throw new Exception("Error in function WebRequestCall. " + ex.Message);
            }
            return result;
        }


        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void GetStatusAndDescriptionCode(HttpWebResponse httpResponse, out string StatusCodeFinal, out string StatusDescriptionFinal)
        {
            StatusCodeFinal = string.Empty;
            int StatusCode = Convert.ToInt32(httpResponse.StatusCode);
            StatusCodeFinal = StatusCode.ToString();

            StatusDescriptionFinal = string.Empty;
            string StatusDecription = httpResponse.StatusDescription;
            StatusDescriptionFinal = StatusDecription;
            log.Info("Status code is " + StatusCode);
            log.Info("Status desctiption is " + StatusDecription);
        }


        public static string ParseJsonOneValue(string jsonResponse, string requestText)
        {
            string res = string.Empty;

            // Parse your Result to an Array
            var x = JObject.Parse(jsonResponse);
            var res1 = x[requestText];
            res = res1.ToString();

            return res;
        }

        public static string ParseJsonTwoValues(string jsonResponse, string requestFirstText, string requestSecondText)
        {
            string res = string.Empty;

            // Parse your Result to an Array
            var x = JObject.Parse(jsonResponse);
            var res1 = x[requestFirstText][requestSecondText];
            res = res1.ToString();

            return res;
        }


        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
