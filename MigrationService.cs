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
using System.Threading;
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
        public static string CreateUsersInBulk_MaxSizeStart { get; set; }
        public static string SCIMcheckData_Url_Out { get; set; }
        public static string SCIMcheckData_ContentType_Out { get; set; }
        public static string SCIMcheckData_Method_Out { get; set; }
        public static string SCIMcheckData_Username_Out { get; set; }
        public static string SCIMcheckData_Password_Out { get; set; }
        public static string SCIMcheckData_BasicAuth { get; set; }
        public static string SCIMcheckData_MaxSizeStart { get; set; }
        public static string ConnectionString { get; set; }
        public static string SCIMcheckData_ConnectionString { get; set; }
        private Object workInProgressLock = new Object();
        private List<StartBulkTime> timesForBulkStarting = new List<StartBulkTime>();
        private Timer getStartBulkTimer;

        public MigrationService()
        {
            //log.Info("Service Initialized..");
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
                bool ConnectionActive = utility.IsAvailableConnection(ConnectionString);
                if (!ConnectionActive)
                {
                    log.Error("Connection problem to database. ");
                }
                else
                {
                    //1.
                    // tajmer za ocitavanje vremena za Bulk Starting --------
                    List<string> stringTimesForBulkStarting = new List<string>();

                    try
                    {
                        XmlSettings.XmlSettings.getArraySettings(SettingsFile, "startBulkTime", ref stringTimesForBulkStarting);
                    }
                    catch (Exception)
                    { }

                    if (stringTimesForBulkStarting.Count > 0) // ako nisu upisana vremena za ocitavanje loga, onda nemoj da pravis tajmer za to uopste
                    {
                        foreach (string t in stringTimesForBulkStarting)
                        {
                            int index = t.IndexOf(":");
                            int h = Convert.ToInt32((index > 0 ? t.Substring(0, index) : ""));
                            int m = Convert.ToInt32((index > 0 ? t.Substring(index + 1) : ""));

                            if ((h < 0) || (h > 23))
                            {
                                throw new Exception("Wrong startBulkTime in settings file: " + t);
                            }

                            if ((m < 0) || (m > 59))
                            {
                                throw new Exception("Wrong startBulkTime in settings file: " + t);
                            }

                            timesForBulkStarting.Add(new StartBulkTime(h, m));
                        }

                        TimerCallback bulkStartingTimerDelegate = new TimerCallback(doWork);
                        getStartBulkTimer = new Timer(bulkStartingTimerDelegate, null, 30000, 60000);  //svakog minuta se poziva tajmer
                    }
                    //------------------------------------------------------
                }
            }
            catch (Exception ex)
            {
                log.Error("Error. " + ex.Message);
            }

            log.Info(@"Service Started. ");
        }


        //--------------------------Do Work funkcija--------------------------------
        protected void doWork(object state)
        {
            try
            {
                DateTime now = DateTime.Now;

                foreach (StartBulkTime time in timesForBulkStarting)
                {
                    DateTime whenShouldRead = new DateTime(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0);

                    if ((now > whenShouldRead) && (now < whenShouldRead.AddMinutes(5)))
                    {
                        lock (workInProgressLock)
                        {
                            if (!time.isDone)
                            {
                                log.Info("BulkInsert START. ");
                                BulkInsert();
                                time.isDone = true;
                                log.Info("BulkInsert END. ");
                            }
                        }
                    }
                    else
                    {
                        if (time.isDone)
                        {
                            time.isDone = false;
                        }
                    }
                }   
            }
            catch (Exception ex)
            {
                log.Error("Error in doWork method. " + ex.Message);
            }
        }
        //-------------------------------------------------------------------------------------

        protected void BulkInsert()
        {
            List<string> responseList = new List<string>();
            ProjectUtility utility = new ProjectUtility();
            string Response = string.Empty;
            string ResponseStatus = string.Empty;
            string ResponseExternal = string.Empty;
            string ResponseStatusExternal = string.Empty;
            string jsonData_SearchUserIDByUsername = string.Empty;
            string ResponseSearch = string.Empty;
            string ResponseStatusSearch = string.Empty;
            int CompareData = 0;
            ////////////////////
            int MaxSize = Convert.ToInt32(CreateUsersInBulk_MaxSizeStart);
            int BulkSetId = 0;
            int MaxSizeNext = 0;
            string RequestData = string.Empty;
            int RequestDataSize = 0;
            int Result = 0;
            string jsonDataSCIM_BULK_Replace = string.Empty;
            int NumberOfExecutedRecords = 0;

            try
            {
                lock (workInProgressLock)
                {
                    while (MaxSize > 0)
                    {
                        utility.spCreateNewBulkSet(MaxSize, ConnectionString, out BulkSetId, out MaxSizeNext, out RequestData, out RequestDataSize, out Result);
                        log.Info("spCreateNewBulkSet: " + " MaxSize - " + MaxSize + " " + ". BulkSetId - " + BulkSetId + " " + ". MaxSizeNext - " + MaxSizeNext + " " + ". RequestData - " + RequestData + " " + ". RequestDataSize - " + RequestDataSize + " " + ". Result - " + Result);

                        if (Result != 0)
                        {
                            log.Error("Result from database is diferent from 0. Result is: " + Result);
                        }
                        else
                        {
                            //log.Info("RequestData is: " + RequestData);
                            jsonDataSCIM_BULK_Replace = RequestData.Replace(@"""""", @"""");
                            //log.Info("After replacing. RequestData is: " + jsonDataSCIM_BULK_Replace);

                            //field that are not required
                            string data1 = Utils.getBetween(jsonDataSCIM_BULK_Replace, "\"password\"", ",");
                            string data2 = Utils.getBetween(jsonDataSCIM_BULK_Replace, "\"city\"", ",");
                            string data3 = Utils.getBetween(jsonDataSCIM_BULK_Replace, "\"postalcode\"", ",");
                            string data4 = Utils.getBetween(jsonDataSCIM_BULK_Replace, "\"country\"", ",");
                            string data5 = Utils.getBetween(jsonDataSCIM_BULK_Replace, "\"streetaddress\"", ",");
                            log.Info("Data's is " + data1 + " " + data2 + " " + data3 + " " + data4 + " " + data5);

                            //if there is a data with :" it will be change with :"" 
                            if (data1 == ":\"" || data2 == ":\"" || data3 == ":\"" || data4 == ":\"" || data5 == ":\"")
                            {
                                jsonDataSCIM_BULK_Replace = jsonDataSCIM_BULK_Replace.Replace(@":"",", @":"""",");
                                //log.Info(jsonDataSCIM_BULK_Replace);
                            }

                            log.Info("Create users in BULK ID " + BulkSetId + " start. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                            string RegisterUser_Response = CreateUsersInBulk_WebRequestCall(jsonDataSCIM_BULK_Replace, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
                            log.Info("Create users in BULK ID " + BulkSetId + " end. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));

                            ResponseStatus = statusCode + " " + statusDescription;
                            log.Info("Response status for BULK ID " + BulkSetId + " is: " + ResponseStatus + " .");

                            if (Convert.ToInt32(statusCode) == ConstantsProject.CREATE_USERS_IN_BULK_ОК)
                            {
                                Response = resultResponse;
                                ////////////Number of enrolled users in this bulk////////////
                                responseList = ParseResponseForSCIMUsers(resultResponse);
                                log.Info("Number of enrolled users in this bulk: " + responseList.Count);
                                if (responseList.Count == RequestDataSize)
                                {
                                    CompareData = 1;
                                }
                                else
                                {
                                    CompareData = 0;
                                    SCIM_DeleteUsersById(responseList, BulkSetId);
                                }
                                NumberOfExecutedRecords = responseList.Count;
                            }
                            else
                            {
                                CompareData = 0;
                                Response = resulNotOK;
                                NumberOfExecutedRecords = 0;
                                log.Info("Result Not OK + " + Response);
                            }
                            log.Debug("spBulkSetExecutionResult: " + " BulkSetId - " + BulkSetId + " CompareData - " + CompareData + " NumberOfExecutedRecords - " + NumberOfExecutedRecords);
                            utility.spBulkSetExecutionResult(BulkSetId, CompareData, NumberOfExecutedRecords, ConnectionString, out int ProcedureResult);

                            if (ProcedureResult != 0)
                            {
                                log.Error("Result from database is diferent from 0. Result is: " + ProcedureResult);
                            }
                            log.Info("Register user in BULK end1. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                        }

                        MaxSize = MaxSizeNext;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in function BulkInsert. " + ex.Message);
            }
        }


        protected void SCIM_DeleteUsersById(List<string> responseList, int bulkId)
        {
            string jsonDataSCIM = string.Empty;
            string Response = string.Empty;
            string ResponseStatus = string.Empty;
            string ResponseExternal = string.Empty;
            string ResponseStatusExternal = string.Empty;
            bool FinalOutcome = false;
            int counter = 0;

            try
            {
                log.Info("Start deleting users from bulk: " + bulkId + " in SCIM web service. ");

                foreach (var id in responseList)
                {
                    string resultDeleteFinal = string.Empty;
                    string SCIM_DeleteUser_Response = SCIMcheckData_WebRequestCall(jsonDataSCIM, id, ConstantsProject.DELETE_METHOD, out resultDeleteFinal, out string statusCode, out string statusDescription, out string resultNotOK);
                    ResponseExternal = resultDeleteFinal;
                    ResponseStatusExternal = statusCode + " " + statusDescription;
                    if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_SCIM_ОК)
                    {
                        FinalOutcome = true;
                        log.Info("Is user with id: " + id + " deleted: " + FinalOutcome);
                        counter++;
                    }
                }

                log.Info("End deleting users from bulk: " + bulkId + " in SCIM web service. Number of deleting users: " + counter);
            }
            catch (Exception ex)
            {
                log.Error("Error in function SCIM_DeleteUsersById. " + ex.Message);
                throw new Exception("Error in function SCIM_DeleteUsersById. " + ex.Message);
            }
        }

        public static string SCIMcheckData_WebRequestCall(string data, string UserID, string Method, out string result_Final_SCIMcheckData, out string StatusCode_Final_SCIMcheckData, out string StatusDescription_Final_SCIMcheckData, out string result_Final_NotOK)
        {
            StatusCode_Final_SCIMcheckData = string.Empty;
            StatusDescription_Final_SCIMcheckData = string.Empty;
            result_Final_SCIMcheckData = string.Empty;
            result_Final_NotOK = string.Empty;
            string WebCall = string.Empty;
            /*******************************/
            if (Method == ConstantsProject.DELETE_METHOD)
            {
                WebCall = Utils.WebRequestCall(data, (SCIMcheckData_Url_Out + UserID), Method, SCIMcheckData_ContentType_Out, SCIMcheckData_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
                StatusCode_Final_SCIMcheckData = StatusCodeFinal;
                StatusDescription_Final_SCIMcheckData = StatusDescriptionFinal;
                result_Final_SCIMcheckData = resultFinal;
                result_Final_NotOK = resultFinalBad;
            }

            return WebCall;
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

            try
            {
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
            }
            catch (Exception ex)
            {
                log.Error("Error in function ParseResponseForSCIMUsers. " + ex.Message);
            }
            return resultList;
        }

        protected override void OnStop()
        {
            lock (workInProgressLock)
            {
                log.Error("The service cannot be shut down while the transaction is in progress.");
            }

            log.Info(@"Service stopped.");
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
                    navigator.MoveToFirstChild();//<CreateUsersInBulk>
                    do
                    {
                        if (navigator.Name == "CreateUsersInBulk")
                        {
                            LoopingThrowNavigatorChild(navigator, out string CreateUsersInBulk_Url_Out_Final, out string CreateUsersInBulk_ContentType_Out_Final, out string CreateUsersInBulk_Method_Out_Final, out string CreateUsersInBulk_Username_Out_Final, out string CreateUsersInBulk_Password_Out_Final, out string CreateUsersInBulk_MaxSizeStart_Out_Final, out string ConnectionString_Out_Final);
                            CreateUsersInBulk_Url_Out = CreateUsersInBulk_Url_Out_Final;
                            CreateUsersInBulk_ContentType_Out = CreateUsersInBulk_ContentType_Out_Final;
                            CreateUsersInBulk_Method_Out = CreateUsersInBulk_Method_Out_Final;
                            CreateUsersInBulk_Username_Out = CreateUsersInBulk_Username_Out_Final;
                            CreateUsersInBulk_Password_Out = CreateUsersInBulk_Password_Out_Final;
                            CreateUsersInBulk_BasicAuth = CreateUsersInBulk_Username_Out + ":" + CreateUsersInBulk_Password_Out;
                            CreateUsersInBulk_MaxSizeStart = CreateUsersInBulk_MaxSizeStart_Out_Final;
                            ConnectionString = ConnectionString_Out_Final;
                            navigator.MoveToFollowing(XPathNodeType.Element);
                            navigator.MoveToNext();
                        }
                        if (navigator.Name == "SCIMcheckData")
                        {
                            LoopingThrowNavigatorChild(navigator, out string SCIMcheckData_Url_Out_Final, out string SCIMcheckData_ContentType_Out_Final, out string SCIMcheckData_Method_Out_Final, out string SCIMcheckData_Username_Out_Final, out string SCIMcheckData_Password_Out_Final, out string SCIMcheckData_MaxSizeStart_Out_Final, out string SCIMcheckData_ConnectionString_Out_Final);
                            SCIMcheckData_Url_Out = SCIMcheckData_Url_Out_Final;
                            SCIMcheckData_ContentType_Out = SCIMcheckData_ContentType_Out_Final;
                            SCIMcheckData_Method_Out = SCIMcheckData_Method_Out_Final;
                            SCIMcheckData_Username_Out = SCIMcheckData_Username_Out_Final;
                            SCIMcheckData_Password_Out = SCIMcheckData_Password_Out_Final;
                            SCIMcheckData_BasicAuth = SCIMcheckData_Username_Out + ":" + SCIMcheckData_Password_Out;
                            SCIMcheckData_MaxSizeStart = SCIMcheckData_MaxSizeStart_Out_Final;
                            SCIMcheckData_ConnectionString = SCIMcheckData_ConnectionString_Out_Final;
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

        public static void LoopingThrowNavigatorChild(XPathNavigator navigator, out string Url_Out, out string ContentType_Out, out string Method_Out, out string Username_Out, out string Password_Out, out string MaxSizeStart_Out, out string ConnectionString_Out)
        {
            Url_Out = string.Empty;
            ContentType_Out = string.Empty;
            Method_Out = string.Empty;
            Username_Out = string.Empty;
            Password_Out = string.Empty;
            MaxSizeStart_Out = string.Empty;
            ConnectionString_Out = string.Empty;

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
                navigator.MoveToFollowing(XPathNodeType.Element);
                if (navigator.Name == "maxSizeStart")
                {
                    MaxSizeStart_Out = navigator.Value;
                }
                navigator.MoveToFollowing(XPathNodeType.Element);
                if (navigator.Name == "connectionString")
                {
                    ConnectionString_Out = navigator.Value;
                }
                log.Info("Get parameters from settings file : URL - " + Url_Out + " . Content Type - " + ContentType_Out + " . Method - " + Method_Out + " . Username - " + Username_Out + " . Password - " + Password_Out + " . MaxSizeStart - " + MaxSizeStart_Out);
                navigator.MoveToFollowing(XPathNodeType.Element);

                navigator.MoveToParent();

            } while (navigator.MoveToNext());
        }
    }

}

