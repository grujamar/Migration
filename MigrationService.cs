using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Migration
{
    public partial class MigrationService : ServiceBase
    {
        //Lofg4Net declare log variable
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string SettingsFile { get; private set; }
        public static string CreateUsersInBulk_Url_Out { get; set; }
        public static string CreateUsersInBulk_ContentType_Out { get; set; }
        public static string CreateUsersInBulk_Method_Out { get; set; }
        public static string CreateUsersInBulk_Username_Out { get; set; }
        public static string CreateUsersInBulk_Password_Out { get; set; }
        public static string CreateUsersInBulk_BasicAuth { get; set; }

        public MigrationService()
        {
            log.Info("Service Initialized.");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log.Info("Service OnStart.");
                //procitaj interval 
                SettingsFile = AppDomain.CurrentDomain.BaseDirectory + "APIsettings.xml";
                initializeSetup();

                ProjectUtility utility = new ProjectUtility();
                bool ConnectionActive = utility.IsAvailableConnection();
                if (!ConnectionActive)
                {
                    log.Error("Connection problem to database. ");
                }
                else
                {
                    log.Info("BulkInsert START. ");
                    BulkInsert();
                    log.Info("BulkInsert END. ");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error. " + ex.Message);
            }

            log.Info(@"Service Started. ");
        }


        protected void BulkInsert()
        {
            try
            {
                List<string> responseList = new List<string>();
                string Response = string.Empty;
                string ResponseStatus = string.Empty;
                string ResponseExternal = string.Empty;
                string ResponseStatusExternal = string.Empty;
                bool FinalOutcome = false;
                bool FinalOutcomeFirstStep = false;
                bool FinalOutcomeSecondStep = false;
                string jsonData_SearchUserIDByUsername = string.Empty;
                string ResponseSearch = string.Empty;
                string ResponseStatusSearch = string.Empty;
                int dataStringId = 10006;

                ProjectUtility utility = new ProjectUtility();
                //string jsonDataResult = utility.spBulkSet();
                string jsonDataResult = utility.getDataString(dataStringId);

                //log.Info("request data is " + jsonDataResult);

                string jsonDataSCIM_BULK_Replace = jsonDataResult.Replace(@"""""", @"""");

                //log.Info("request data is " + jsonDataSCIM_BULK_Replace);

                log.Info("Register user in BULK start. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                string RegisterUser_Response = CreateUsersInBulk_WebRequestCall(jsonDataSCIM_BULK_Replace, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
                log.Info("Register user in BULK end1. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                responseList = ParseResponseForSCIMUsers(resultResponse);
                log.Info("Number of enrolled users in this bulk: " + responseList.Count);
                ResponseStatus = statusCode + " " + statusDescription;
                if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_ОК)
                {
                    FinalOutcomeFirstStep = true;
                    Response = resultResponse;
                }
                else
                {
                    Response = resulNotOK;
                }
                log.Info("Register user in BULK end2. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                //log.Info("Register user in BULK end. Response result is: " + Response + " " + ResponseStatus);
            }
            catch (Exception ex)
            {
                log.Error("Error in function BulkInsert. " + ex.Message);
            }
        }

        public static string CreateUsersInBulk_WebRequestCall(string data, out string result_Final_CreateUsersInBulk, out string StatusCode_Final_CreateUsersInBulk, out string StatusDescription_Final_CreateUsersInBulk, out string result_Final_NotOK)
        {
            StatusCode_Final_CreateUsersInBulk = string.Empty;
            StatusDescription_Final_CreateUsersInBulk = string.Empty;
            result_Final_CreateUsersInBulk = string.Empty;
            result_Final_NotOK = string.Empty;
            /*******************************/
            string WebCall = Utils.WebRequestCall(data, CreateUsersInBulk_Url_Out, CreateUsersInBulk_Method_Out, CreateUsersInBulk_ContentType_Out, CreateUsersInBulk_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
            /*******************************/
            StatusCode_Final_CreateUsersInBulk = StatusCodeFinal;
            StatusDescription_Final_CreateUsersInBulk = StatusDescriptionFinal;
            result_Final_CreateUsersInBulk = resultFinal;
            result_Final_NotOK = resultFinalBad;
            /*******************************/
            return WebCall;
        }

        protected List<string> ParseResponseForSCIMUsers(string jsonResponse)
        {
            List<string> resultList = new List<string>();

            // Parse your Result to an Array
            var x = JObject.Parse(jsonResponse);
            //log.Info("x is " + x.ToString());
            var y = x["Operations"];

            foreach (JObject o in y.Children<JObject>())
            {
                var resultPrepared = o["location"];
                string result = resultPrepared.ToString();
                var userID = result.Split('/').Last();
                resultList.Add(userID.ToString());
            }

            return resultList;
        }

        protected override void OnStop()
        {

        }


        //pravi se mogućnost za debagovanje
        public void onDebug()
        {
            OnStart(null);
        }

        /// <summary>
        /// initializeSetup
        /// </summary>

        public void initializeSetup()
        {
            getSettings();
        }

        public static void getSettings()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(SettingsFile);
                XPathNavigator navigator = xmlDoc.CreateNavigator();

                navigator.MoveToRoot();
                navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//parameters
                if (navigator.HasChildren)
                {
                    navigator.MoveToFirstChild();//<RegisterUser>
                    do
                    {
                        if (navigator.Name == "CreateUsersInBulk")
                        {
                            LoopingThrowNavigatorChild(navigator, out string CreateUsersInBulk_Url_Out_Final, out string CreateUsersInBulk_ContentType_Out_Final, out string CreateUsersInBulk_Method_Out_Final, out string CreateUsersInBulk_Username_Out_Final, out string CreateUsersInBulk_Password_Out_Final);
                            CreateUsersInBulk_Url_Out = CreateUsersInBulk_Url_Out_Final;
                            CreateUsersInBulk_ContentType_Out = CreateUsersInBulk_ContentType_Out_Final;
                            CreateUsersInBulk_Method_Out = CreateUsersInBulk_Method_Out_Final;
                            CreateUsersInBulk_Username_Out = CreateUsersInBulk_Username_Out_Final;
                            CreateUsersInBulk_Password_Out = CreateUsersInBulk_Password_Out_Final;
                            CreateUsersInBulk_BasicAuth = CreateUsersInBulk_Username_Out + ":" + CreateUsersInBulk_Password_Out;
                            navigator.MoveToFollowing(XPathNodeType.Element);
                        }
                    } while (navigator.MoveToNext());
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while reading configuration data. " + ex.Message);
            }
        }

        public static void LoopingThrowNavigatorChild(XPathNavigator navigator, out string Url_Out, out string ContentType_Out, out string Method_Out, out string Username_Out, out string Password_Out)
        {
            Url_Out = string.Empty;
            ContentType_Out = string.Empty;
            Method_Out = string.Empty;
            Username_Out = string.Empty;
            Password_Out = string.Empty;

            do
            {
                navigator.MoveToFirstChild();
                if (navigator.Name == "url")
                {
                    Url_Out = navigator.Value;
                }
                navigator.MoveToFollowing(XPathNodeType.Element);
                if (navigator.Name == "contentType")
                {
                    ContentType_Out = navigator.Value;
                }
                navigator.MoveToFollowing(XPathNodeType.Element);
                if (navigator.Name == "method")
                {
                    Method_Out = navigator.Value;
                }
                navigator.MoveToFollowing(XPathNodeType.Element);
                if (navigator.Name == "username")
                {
                    Username_Out = navigator.Value;
                }
                navigator.MoveToFollowing(XPathNodeType.Element);
                if (navigator.Name == "password")
                {
                    Password_Out = navigator.Value;
                }
                log.Info("Get parameters from settings file : URL - " + Url_Out + " . Content Type - " + ContentType_Out + " . Method - " + Method_Out + " . Username - " + Username_Out + " . Password - " + Password_Out);
                navigator.MoveToFollowing(XPathNodeType.Element);

                navigator.MoveToParent();

            } while (navigator.MoveToNext());
        }
    }

}

