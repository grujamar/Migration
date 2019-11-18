using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ConstantsProject
{
    //public const int REGISTER_USER_METHOD = 2;
    public const string REGISTER_USER_NAME = "RegisterUser";
    /// <summary>
    /// /
    /// </summary>
    public const string DELETE_METHOD = "DELETE";
    public const string PUT_METHOD = "PUT";
    /// <summary>
    /// FROM DATABASE METHOD ID
    /// </summary>
    public const int REGISTER_USER_ID = 2;
    public const int VALIDATE_CODE_METHOD_ID = 4;
    public const int VALIDATE_USERNAME_METHOD_ID = 5;
    public const int VALIDATE_UMCN_METHOD_ID = 6;
    public const int EXPORT_USER_INFO_BY_USERNAME = 7;
    public const int SEARCH_USER_ID_BY_USERNAME = 8;
    public const int SEARCH_USER_ID_BY_UMCN = 9;
    public const int EXPORT_AUTH_INFO_BY_USERNAME = 10;
    /// <summary>
    /// ////////
    /// </summary>

    public const int REGISTER_USER_ОК = 201;
    public const int REGISTER_USER_SCIM_ОК = 200;

    ////////////////////////////////////
    public const int VALIDATE_CODE_ОК = 202;
    public const int VALIDATE_USERNAME_ОК = 200;

    public const int VALIDATE_UMCN_ОК = 60000;

    public const int EXPORT_USER_INFO_BY_USERNAME_ОК = 200;
    public const int EXPORT_AUTH_INFO_BY_USERNAME_ОК = 200;
}