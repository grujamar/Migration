using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

/// <summary>
/// Summary description for Utility
/// </summary>

public class ProjectUtility
{
    public string EGovMigrationConnectionString = ConfigurationManager.ConnectionStrings["EGovMigrationConnectionString"].ToString();
    //public string scnsconnectionstring = ConfigurationManager.ConnectionStrings["SCNSPISConnectionString"].ToString();
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public ProjectUtility()
    {

    }

    public bool IsAvailableConnection(string connString)
    {
        using (SqlConnection objConn = new SqlConnection(connString))
        {
            try
            {
                objConn.Open();
                objConn.Close();
            }
            catch (SqlException ex)
            {
                log.Error("Error while connecting to Database. " + ex.Message);
                return false;
            }
            return true;
        }
    }


    public void testSessionStart(int TestSessionId, out int result)
    {
        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand("spTestSessionStart", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("@TestSessionId", System.Data.SqlDbType.Int).Value = TestSessionId;

                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    //Retrieve the value of the output parameter
                    result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function testSessionStart. " + ex.Message);
                    throw new Exception("Error in function testSessionStart. " + ex.Message);
                }
            }
        }
    }


    public void testSessionFinish(int TestSessionId, out int result)
    {
        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand("spTestSessionFinish", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("@TestSessionId", System.Data.SqlDbType.Int).Value = TestSessionId;

                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    //Retrieve the value of the output parameter
                    result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function testSessionFinish. " + ex.Message);
                    throw new Exception("Error in function testSessionFinish. " + ex.Message);
                }
            }
        }
    }

    public void testCombinationStart(int TestCombinationId, out int result)
    {
        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand("spTestCombinationStart", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("@TestCombinationId", System.Data.SqlDbType.Int).Value = TestCombinationId;

                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    //Retrieve the value of the output parameter
                    result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function testCombinationStart. " + ex.Message);
                    throw new Exception("Error in function testCombinationStart. " + ex.Message);
                }
            }
        }
    }


    public void testCombinationFinish(int TestCombinationId, string Response, string ResponseStatus, string ResponseExternal, string ResponseStatusExternal, bool FinalOutcome, out int result)
    {
        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand("spTestCombinationFinish", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("@TestCombinationId", System.Data.SqlDbType.Int).Value = TestCombinationId;
                    objCmd.Parameters.AddWithValue("@Response", Response);
                    objCmd.Parameters.AddWithValue("@ResponseStatus", ResponseStatus);
                    objCmd.Parameters.AddWithValue("@ResponseExternal", ResponseExternal);
                    objCmd.Parameters.AddWithValue("@ResponseStatusExternal", ResponseStatusExternal);
                    objCmd.Parameters.Add("@FinalOutcome", SqlDbType.Bit).Value = FinalOutcome;

                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    //Retrieve the value of the output parameter
                    result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function testCombinationFinish. " + ex.Message);
                    throw new Exception("Error in function testCombinationFinish. " + ex.Message);
                }
            }
        }
    }

    public int getMethodID(string MethodName)
    {
        int MethodID = 0;

        string upit = @"SELECT        MethodId
                        FROM            dbo.Method
                        WHERE        (MethodName = @methodname)";

        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@methodname", MethodName);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        MethodID = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error in fuction getMethodID. " + ex.Message);
                    throw new Exception("Error in fuction getMethodID. " + ex.Message);
                }
            }
        }

        return MethodID;
    }


    public string getMethodName(int MethodId)
    {
        string MethodName = string.Empty;

        string query = @"SELECT        MethodName
                        FROM            dbo.Method
                        WHERE        (MethodId = @methodid)";

        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand(query, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@methodid", System.Data.SqlDbType.Int).Value = MethodId;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        MethodName = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error in fuction getMethodName. " + ex.Message);
                    throw new Exception("Error in fuction getMethodName. " + ex.Message);
                }
            }
        }
        return MethodName;
    }


    public string spBulkSet()
    {
        string test = string.Empty;

        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand("spBulkInsert", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        test = reader.GetSqlString(0).ToString();
                    }

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function spBulkSet. " + ex.Message);
                    throw new Exception("Error in function spBulkSet. " + ex.Message);
                }
            }
        }

        return test;
    }


    public string getDataString(int ID)
    {
        string dataString = string.Empty;

        string upit = @"SELECT        DataString
                        FROM            dbo.DataStringTest
                        WHERE        (ID = @id)";

        using (SqlConnection objConn = new SqlConnection(EGovMigrationConnectionString))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = ID;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataString = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error in function getGetDataString. " + ex.Message);
                    throw new Exception("Error in function getGetDataString. " + ex.Message);
                }
            }
        }

        return dataString;
    }


    public void spCreateNewBulkSet(int MaxSize, string connString, out int BulkSetId, out int MaxSizeNext, out string RequestData, out int RequestDataSize, out int result)
    {
        using (SqlConnection objConn = new SqlConnection(connString))
        {
            using (SqlCommand objCmd = new SqlCommand("spCreateNewBulkSet", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.CommandTimeout = 600; //set to 10 minutes (for 10.000 data, SP needs 4.5 minutes)
                                                 //The time in seconds to wait for the command to execute. The default is 30 seconds.
                    objCmd.Parameters.Add("@MaxSize", System.Data.SqlDbType.Int).Value = MaxSize;

                    objCmd.Parameters.Add("@BulkSetId", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@BulkSetId"].Direction = System.Data.ParameterDirection.Output;

                    objCmd.Parameters.Add("@MaxSizeNext", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@MaxSizeNext"].Direction = System.Data.ParameterDirection.Output;

                    objCmd.Parameters.Add("@RequestData", System.Data.SqlDbType.NVarChar, -1);
                    objCmd.Parameters["@RequestData"].Direction = System.Data.ParameterDirection.Output;

                    objCmd.Parameters.Add("@RequestDataSize", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@RequestDataSize"].Direction = System.Data.ParameterDirection.Output;

                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    //Retrieve the values of the output parameters
                    BulkSetId = Convert.ToInt32(objCmd.Parameters["@BulkSetId"].Value);
                    MaxSizeNext = Convert.ToInt32(objCmd.Parameters["@MaxSizeNext"].Value);
                    RequestData = objCmd.Parameters["@RequestData"].Value.ToString();
                    RequestDataSize = Convert.ToInt32(objCmd.Parameters["@RequestDataSize"].Value);
                    result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function spCreateNewBulkSet. " + ex.Message);
                    throw new Exception("Error in function spCreateNewBulkSet. " + ex.Message);
                }
            }
        }
    }


    public void spBulkSetExecutionResult(int BulkSetId, int ExecutionResult, int NumberOfExecutedRecords, string connString, out int result)
    {
        using (SqlConnection objConn = new SqlConnection(connString))
        {
            using (SqlCommand objCmd = new SqlCommand("spBulkSetExecutionResult", objConn))
            {
                try
                {
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("@BulkSetId", System.Data.SqlDbType.Int).Value = BulkSetId;
                    objCmd.Parameters.Add("@ExecutionResult", System.Data.SqlDbType.Int).Value = ExecutionResult;
                    objCmd.Parameters.Add("@NumberOfExecutedRecords", System.Data.SqlDbType.Int).Value = NumberOfExecutedRecords;


                    objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                    objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                    objConn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error in function spBulkSetExecutionResult. " + ex.Message);
                    throw new Exception("Error in function spBulkSetExecutionResult. " + ex.Message);
                }
            }
        }
    }

}
/*
public void loginAdmin(string Username, string Password, out int IDLogAdmin, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spLoginAdmin", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.AddWithValue("@username", Username);
                objCmd.Parameters.AddWithValue("@password", Password);

                objCmd.Parameters.Add("@IDLogAdmin", System.Data.SqlDbType.Int);
                objCmd.Parameters["@IDLogAdmin"].Direction = System.Data.ParameterDirection.Output;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the values of the output parameters
                IDLogAdmin = Convert.ToInt32(objCmd.Parameters["@IDLogAdmin"].Value);
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function loginAdmin. " + ex.Message);
                throw new Exception("Error in function loginAdmin. " + ex.Message);
            }
        }
    }
}

public void logoutAdmin(int IDLogAdmin, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spLogoutAdmin", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@IDLogAdmin", System.Data.SqlDbType.Int).Value = IDLogAdmin;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function logoutAdmin. " + ex.Message);
                throw new Exception("Error in function logoutAdmin. " + ex.Message);
            }
        }
    }
}

public void changePassword(string Username, string OldPassword, string NewPassword, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spPromenaLozinkeAdmin", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.AddWithValue("@username", Username);
                objCmd.Parameters.AddWithValue("@oldPassword", OldPassword);
                objCmd.Parameters.AddWithValue("@newPassword", NewPassword);


                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function logoutAdmin. " + ex.Message);
                throw new Exception("Error in function logoutAdmin. " + ex.Message);
            }
        }
    }
}


public int getIDLokacijeAdmin(int idTerminPredavanja)
{
    int IDLokacije = 0;

    string upit = @"SELECT        IDLokacija
                    FROM            dbo.TerminPredavanja
                    WHERE        (IDTerminPredavanja = @idterminpredavanja)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = idTerminPredavanja;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    IDLokacije = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in fuction getIDLokacije. " + ex.Message);
                throw new Exception("Error in fuction getIDLokacije. " + ex.Message);
            }
        }
    }

    return IDLokacije;
}


public List<int> getCheckedIDPredmet(int idTerminPredavanja)
{
    List<int> responses = new List<int>();

    string upit = @"SELECT        dbo.Predavanje.IDPredmet
                    FROM            dbo.TerminPredavanja INNER JOIN
                     dbo.PredavanjeUTerminu ON dbo.TerminPredavanja.IDTerminPredavanja = dbo.PredavanjeUTerminu.IDTerminPredavanja INNER JOIN
                     dbo.Predavanje ON dbo.PredavanjeUTerminu.IDPredavanje = dbo.Predavanje.IDPredavanje
                    WHERE        (dbo.TerminPredavanja.IDTerminPredavanja = @idterminpredavanja)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = idTerminPredavanja;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in fuction getCheckedIDPredmet. " + ex.Message);
                throw new Exception("Error in fuction getCheckedIDPredmet. " + ex.Message);
            }
        }
    }

    return responses;
}


public int getIDTipPredavanja(int idTerminPredavanja)
{
    int IDTipPredavanja = 0;

    string upit = @"SELECT        dbo.TipPredavanja.IDTipPredavanja
FROM            dbo.TerminPredavanja INNER JOIN
                     dbo.PredavanjeUTerminu ON dbo.TerminPredavanja.IDTerminPredavanja = dbo.PredavanjeUTerminu.IDTerminPredavanja INNER JOIN
                     dbo.Predavanje ON dbo.PredavanjeUTerminu.IDPredavanje = dbo.Predavanje.IDPredavanje INNER JOIN
                     dbo.Predmet ON dbo.Predavanje.IDPredmet = dbo.Predmet.IDPredmet INNER JOIN
                     dbo.TipPredavanja ON dbo.Predavanje.IDTipPredavanja = dbo.TipPredavanja.IDTipPredavanja
WHERE        (dbo.TerminPredavanja.IDTerminPredavanja = @idterminpredavanja)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = idTerminPredavanja;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    IDTipPredavanja = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in fuction getIDTipPredavanja. " + ex.Message);
                throw new Exception("Error in fuction getIDTipPredavanja. " + ex.Message);
            }
        }
    }

    return IDTipPredavanja;
}






    






public void loginPredavanja(string Username, int IDLokacija, out int idLogPredavanja, out int idOsoba, out string Ime, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spLoginPredavanja", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.AddWithValue("@username", Username);
                objCmd.Parameters.Add("@IDLokacija", System.Data.SqlDbType.Int).Value = IDLokacija;

                objCmd.Parameters.Add("@IDLogPredavanja", System.Data.SqlDbType.Int);
                objCmd.Parameters["@IDLogPredavanja"].Direction = System.Data.ParameterDirection.Output;

                objCmd.Parameters.Add("@IDOsoba", System.Data.SqlDbType.Int);
                objCmd.Parameters["@IDOsoba"].Direction = System.Data.ParameterDirection.Output;

                objCmd.Parameters.Add("@PunoIme", System.Data.SqlDbType.NVarChar, -1);
                objCmd.Parameters["@PunoIme"].Direction = System.Data.ParameterDirection.Output;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the values of the output parameters
                idLogPredavanja = Convert.ToInt32(objCmd.Parameters["@IDLogPredavanja"].Value);
                idOsoba = Convert.ToInt32(objCmd.Parameters["@IDOsoba"].Value);
                Ime = objCmd.Parameters["@PunoIme"].Value.ToString();
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function loginPredavanja. " + ex.Message);
                throw new Exception("Error in function loginPredavanja. " + ex.Message);
            }
        }
    }
}

public void logoutPredavanja(int idLogPredavanja, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spLogoutPredavanja", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@idLogPredavanja", System.Data.SqlDbType.Int).Value = idLogPredavanja;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function logoutPredavanja. " + ex.Message);
                throw new Exception("Error in function logoutPredavanja. " + ex.Message);
            }
        }
    }
}

public void zapocinjanjeTermina(int IDLokacija, DateTime Datum, TimeSpan Pocetak, int idOsoba, int idLogPredavanja, out int idTerminPredavanja, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spZapocinjanjeTermina", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@IDLokacija", System.Data.SqlDbType.Int).Value = IDLokacija;
                objCmd.Parameters.AddWithValue("@Datum", Datum);
                objCmd.Parameters.AddWithValue("@pocetak", Pocetak);
                objCmd.Parameters.Add("@idOsoba", System.Data.SqlDbType.Int).Value = idOsoba;
                objCmd.Parameters.Add("@idLogPredavanja", System.Data.SqlDbType.Int).Value = idLogPredavanja;

                //Add the output parameter to the command object
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@idTerminPredavanja";
                outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                objCmd.Parameters.Add(outPutParameter);

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the value of the output parameter
                idTerminPredavanja = Convert.ToInt32(outPutParameter.Value);
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function zapocinjanjeTermina. " + ex.Message);
                throw new Exception("Error in function zapocinjanjeTermina. " + ex.Message);
            }
        }
    }
}

public void zapocinjanjePredavanja(int IDTerminPredavanja, int idPredmet, int idTipPredavanja, out int idPredavanje, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spZapocinjanjePredavanja", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@idTerminPredavanja", System.Data.SqlDbType.Int).Value = IDTerminPredavanja;
                objCmd.Parameters.Add("@idPredmet", System.Data.SqlDbType.Int).Value = idPredmet;
                objCmd.Parameters.Add("@idTipPredavanja", System.Data.SqlDbType.Int).Value = idTipPredavanja;

                //Add the output parameter to the command object
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@idPredavanje";
                outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                objCmd.Parameters.Add(outPutParameter);

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the value of the output parameter
                idPredavanje = Convert.ToInt32(outPutParameter.Value);
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function zapocinjanjePredavanja. " + ex.Message);
                throw new Exception("Error in function zapocinjanjePredavanja. " + ex.Message);
            }
        }
    }
}

public void zavrsavanjePredavanja(int idPredavanje, TimeSpan Kraj, decimal procenatZaPriznavanje, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spZavrsavanjePredavanja", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@idPredavanje", System.Data.SqlDbType.Int).Value = idPredavanje;
                objCmd.Parameters.AddWithValue("@kraj", Kraj);
                objCmd.Parameters.Add("@procenatZaPriznavanje", System.Data.SqlDbType.Decimal).Value = procenatZaPriznavanje;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the value of the output parameter
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function zavrsavanjePredavanja. " + ex.Message);
                throw new Exception("Error in function zavrsavanjePredavanja. " + ex.Message);
            }
        }
    }
}


public void zavrsavanjeTermina(int idTerminPredavanja, TimeSpan Kraj, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spZavrsavanjeTermina", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@idTerminPredavanja", System.Data.SqlDbType.Int).Value = idTerminPredavanja;
                objCmd.Parameters.AddWithValue("@kraj", Kraj);

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the value of the output parameter
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function zavrsavanjeTermina. " + ex.Message);
                throw new Exception("Error in function zavrsavanjeTermina. " + ex.Message);
            }
        }
    }
}

public void ponistavanjePrisustva(int IDDnevniStatusOsobeNaLokaciji, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spPonistavanjePrisustva", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@IDDnevniStatusOsobeNaLokaciji", System.Data.SqlDbType.Int).Value = IDDnevniStatusOsobeNaLokaciji;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the value of the output parameter
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function ponistavanjePrisustva. " + ex.Message);
                throw new Exception("Error in function ponistavanjePrisustva. " + ex.Message);
            }
        }
    }
}


public List<TimeSpan> proveriKrajUTabeliTerminPredavanja(int idosoba)
{
    try
    {
        List<TimeSpan> responses = new List<TimeSpan>();

        string upit = @"SELECT        Kraj
                        FROM            dbo.TerminPredavanja
                        WHERE        (IDOsobaPredavac = @idosoba)";

        using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idosoba", System.Data.SqlDbType.Int).Value = idosoba;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(reader.GetTimeSpan(0));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error: " + ex.Message);
                    throw new Exception("Error: " + ex.Message);
                }
            }
        }

        return responses;
    }
    catch (Exception ex)
    {
        log.Error("Error in function proveriKrajUTabeliTerminPredavanja: " + ex.Message);
        throw new Exception("Error in function proveriKrajUTabeliTerminPredavanja: " + ex.Message);
    }
}


public void getTerminPredavanjaKraj(int idosoba, int IDLokacija, out int IDTerminPredavanja, out int IDLogPredavanja, out List<int> predmetiList, out TimeSpan d1)
{
    IDTerminPredavanja=0;
    IDLogPredavanja = 0;
    predmetiList = new List<int>();
    d1= new TimeSpan();

    string upit = @"SELECT        TOP (100) PERCENT dbo.TerminPredavanja.IDTerminPredavanja, dbo.TerminPredavanja.IDLogPredavanja, dbo.Predmet.IDPredmet, dbo.TerminPredavanja.Pocetak
FROM            dbo.TerminPredavanja INNER JOIN
                     dbo.PredavanjeUTerminu ON dbo.TerminPredavanja.IDTerminPredavanja = dbo.PredavanjeUTerminu.IDTerminPredavanja INNER JOIN
                     dbo.Predavanje ON dbo.PredavanjeUTerminu.IDPredavanje = dbo.Predavanje.IDPredavanje INNER JOIN
                     dbo.Predmet ON dbo.Predavanje.IDPredmet = dbo.Predmet.IDPredmet
WHERE        (dbo.TerminPredavanja.IDOsobaPredavac = @idosoba) AND (dbo.TerminPredavanja.Kraj IS NULL) AND (dbo.TerminPredavanja.IDLokacija = @idlokacija)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idosoba", System.Data.SqlDbType.Int).Value = idosoba;
                objCmd.Parameters.Add("@idlokacija", System.Data.SqlDbType.Int).Value = IDLokacija;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IDTerminPredavanja = reader.GetInt32(0);
                        IDLogPredavanja = reader.GetInt32(1);
                        predmetiList.Add(reader.GetInt32(2));
                        d1 = reader.GetTimeSpan(3);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting TerminPredavanjaKraj. " + ex.Message);
                throw new Exception("Error while getting TerminPredavanjaKraj. " + ex.Message);
            }
        }
    }
}


public string getPredmetNaziv(int idOsoba, int idPredmet)
{
    string NazivPredmeta = string.Empty;

    string upit = @"SELECT NazivPredmeta FROM dbo.vPredavanjaNastavnika WHERE (IDOsoba = @idosoba) AND (IDPredmet = @idpredmet)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idosoba", System.Data.SqlDbType.Int).Value = idOsoba;
                objCmd.Parameters.Add("@idpredmet", System.Data.SqlDbType.Int).Value = idPredmet;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    NazivPredmeta = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in fuction getPredmetNaziv. " + ex.Message);
                throw new Exception("Error in fuction getPredmetNaziv. " + ex.Message);
            }
        }
    }

    return NazivPredmeta;
}


public string getTipPredavanja(int idosoba, int IDTerminPredavanja, int IDLokacija, int IDLogPredavanja)
{
    string tipPredavanja = string.Empty;

    string upit = @"SELECT        dbo.TipPredavanja.TipPredavanja
FROM            dbo.TerminPredavanja INNER JOIN
                     dbo.PredavanjeUTerminu ON dbo.TerminPredavanja.IDTerminPredavanja = dbo.PredavanjeUTerminu.IDTerminPredavanja INNER JOIN
                     dbo.Predavanje ON dbo.PredavanjeUTerminu.IDPredavanje = dbo.Predavanje.IDPredavanje INNER JOIN
                     dbo.TipPredavanja ON dbo.Predavanje.IDTipPredavanja = dbo.TipPredavanja.IDTipPredavanja
WHERE        (dbo.TerminPredavanja.IDOsobaPredavac = @idosoba) AND (dbo.TerminPredavanja.IDTerminPredavanja = @idterminpredavanja) AND (dbo.TerminPredavanja.IDLokacija = @idlokacija)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idosoba", System.Data.SqlDbType.Int).Value = idosoba;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = IDTerminPredavanja;
                objCmd.Parameters.Add("@idlokacija", System.Data.SqlDbType.Int).Value = IDLokacija;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    tipPredavanja = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting tipPredavanja. " + ex.Message);
                throw new Exception("Error while getting tipPredavanja. " + ex.Message);
            }
        }
    }

    return tipPredavanja;
}



public void getIDTerminePredavanja(int IDTerminPredavanja, out List<int> idTerminiPredavanja)
{
    idTerminiPredavanja = new List<int>();

    string upit = @"SELECT        dbo.PredavanjeUTerminu.IDPredavanje
FROM            dbo.TerminPredavanja INNER JOIN
                     dbo.PredavanjeUTerminu ON dbo.TerminPredavanja.IDTerminPredavanja = dbo.PredavanjeUTerminu.IDTerminPredavanja
WHERE        (dbo.TerminPredavanja.IDTerminPredavanja = @idterminpredavanja)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = IDTerminPredavanja;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        idTerminiPredavanja.Add(reader.GetInt32(0));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in function getIDTerminePredavanja. " + ex.Message);
                throw new Exception("Error in function getIDTerminePredavanja. " + ex.Message);
            }
        }
    }
}


public string getImeLokacije(int IDLokacije)
{
    string ImeLokacije = string.Empty;

    string upit = @"SELECT        NazivLokacije
FROM            dbo.Lokacija
WHERE        (IDLokacija = @idlokacije)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idlokacije", System.Data.SqlDbType.Int).Value = IDLokacije;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    ImeLokacije = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting ImeLokacije. " + ex.Message);
                throw new Exception("Error while getting ImeLokacije. " + ex.Message);
            }
        }
    }

    return ImeLokacije;
}


public int getBrojAkreditacije(string nazivPredmeta)
{
    int BrojAkreditacije = 0;

    string upit = @"SELECT        TOP (1) PERCENT BrojAkreditacije
                    FROM            dbo.vPredavanjaNastavnika
                    WHERE        (NazivPredmeta = @nazivpredmeta)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@nazivpredmeta", nazivPredmeta);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    BrojAkreditacije = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting BrojAkreditacije. " + ex.Message);
                throw new Exception("Error while getting BrojAkreditacije. " + ex.Message);
            }
        }
    }

    return BrojAkreditacije;
}



public void upisivanjePrisustvaRucno(string brojIndeksa, int idLokacija, DateTime datum, TimeSpan Vreme, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spUpisivanjePrisustvaRucno", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.AddWithValue("@brojIndeksa", brojIndeksa);
                objCmd.Parameters.Add("@idLokacija", System.Data.SqlDbType.Int).Value = idLokacija;
                objCmd.Parameters.AddWithValue("@datum", datum);
                objCmd.Parameters.AddWithValue("@vreme", Vreme);

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the value of the output parameter
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function upisivanjePrisustvaRucno. " + ex.Message);
                throw new Exception("Error in function upisivanjePrisustvaRucno. " + ex.Message);
            }
        }
    }
}


public void brisanjePredavanjaIzTermina(int IDTerminPredavanja, out int result)
{
    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand("spBrisanjePredavanjaIzTermina", objConn))
        {
            try
            {
                objCmd.CommandType = CommandType.StoredProcedure;

                objCmd.Parameters.Add("@idTerminPredavanja", System.Data.SqlDbType.Int).Value = IDTerminPredavanja;

                objCmd.Parameters.Add("@err", System.Data.SqlDbType.Int);
                objCmd.Parameters["@err"].Direction = ParameterDirection.ReturnValue;

                objConn.Open();
                objCmd.ExecuteNonQuery();

                //Retrieve the values of the output parameters
                result = Convert.ToInt32(objCmd.Parameters["@err"].Value);

                objConn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in function brisanjePredavanjaIzTermina. " + ex.Message);
                throw new Exception("Error in function brisanjePredavanjaIzTermina. " + ex.Message);
            }
        }
    }
}



public TimeSpan getPocetakTermina(int IdTerminPredavanja)
{
    TimeSpan Pocetak = DateTime.Now.TimeOfDay;

    string upit = @"SELECT        Pocetak AS Expr1
                    FROM            dbo.TerminPredavanja
                    WHERE        (IDTerminPredavanja = @idterminpredavanja)";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = IdTerminPredavanja;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    Pocetak = reader.GetTimeSpan(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting Pocetak in function getPocetakTermina. " + ex.Message);
                throw new Exception("Error while getting Pocetak in function getPocetakTermina. " + ex.Message);
            }
        }
    }

    return Pocetak;
}


public void upisiNazivFajla(int IdTerminPredavanja, string nazivFajla)
{

    string upit = @"update TerminPredavanja
                    set Izvestaj = @nazivFajla
                    where IDTerminPredavanja = @idterminpredavanja";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idterminpredavanja", System.Data.SqlDbType.Int).Value = IdTerminPredavanja;
                objCmd.Parameters.AddWithValue("@nazivFajla", nazivFajla);
                objConn.Open();
                objCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                log.Error("Error in function upisiNazivFajla. " + ex.Message);
                throw new Exception("Error in function upisiNazivFajla. " + ex.Message);
            }
        }
    }
}
/*
public List<vIzvestaji> pronadjiPromenljiveIzvestaj(int idOsoba)
{
    List<vIzvestaji> responses = new List<vIzvestaji>();

    string upit = @"SELECT        TOP (100) PERCENT OpisTermina, Izvestaj
                    FROM            dbo.vIzvestaji
                    WHERE        (IDOsoba = @idOsoba)
                    ORDER BY Datum, Pocetak";

    using (SqlConnection objConn = new SqlConnection(bioconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idOsoba", System.Data.SqlDbType.Int).Value = idOsoba;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new vIzvestaji(reader.GetSqlString(0).ToString(), reader.GetSqlString(1).ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in function pronadjiPromenljiveIzvestaj. " + ex.Message);
                throw new Exception("Error in function pronadjiPromenljiveIzvestaj. " + ex.Message);
            }
        }
    }

    return responses;
}





public void upisiOrganizaciju(string organizacija)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"insert into blOrganizacija (NazivOrganizacije) values (@organizacija)", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.AddWithValue("@organizacija", organizacija);

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while inserting Organization value. " + ex.Message);
        throw new Exception("Error while inserting Organization value. " + ex.Message);
    }
}

public void upisiBlagajnicu(string blagajnica)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"insert into blVMoguciBlagajniciZaZaduzivanje (PunoIme) values (@blagajnica)", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.AddWithValue("@blagajnica", blagajnica);

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while inserting PunoIme value. " + ex.Message);
        throw new Exception("Error while inserting PunoIme value. " + ex.Message);
    }
}

public List<string> proveriOrganizaciju()
{
    try
    {
        List<string> responses = new List<string>();

        string upit = @"SELECT  NazivOrganizacije FROM dbo.blOrganizacija";

        using (SqlConnection objConn = new SqlConnection(scnsconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(reader.GetSqlString(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error: " + ex.Message);
                    throw new Exception("Error: " + ex.Message);
                }
            }
        }

        return responses;
    }
    catch (Exception ex)
    {
        log.Error("Error: " + ex.Message);
        throw new Exception("Error: " + ex.Message);
    }
}

public List<string> proveriBlagajnicu()
{
    try
    {
        List<string> responses = new List<string>();

        string upit = @"SELECT  PunoIme FROM dbo.blVMoguciBlagajniciZaZaduzivanje";

        using (SqlConnection objConn = new SqlConnection(scnsconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(reader.GetSqlString(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error: " + ex.Message);
                    throw new Exception("Error: " + ex.Message);
                }
            }
        }

        return responses;
    }
    catch (Exception ex)
    {
        log.Error("Error: " + ex.Message);
        throw new Exception("Error: " + ex.Message);
    }
}

public void upisiEksternoPlacanje(int TypeOfPaymentSelectedValue, int OrganizationSelectedValue, string FactureNumber, decimal Price, string Date, string Description, int Operater)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"insert into blEksternoPlacanje (IDTipPlacanja, IDOrganizacija, DatumPlacanja, BrojPlacanja, Opis, Iznos, Operater) values (@TypeOfPaymentSelectedValue, @OrganizationSelectedValue, @Date, @FactureNumber, @Description, @Price, @Operater)", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.Add("@TypeOfPaymentSelectedValue", System.Data.SqlDbType.Int).Value = TypeOfPaymentSelectedValue;
        objCmd.Parameters.Add("@OrganizationSelectedValue", System.Data.SqlDbType.Int).Value = OrganizationSelectedValue;
        objCmd.Parameters.AddWithValue("@Date", Date);
        objCmd.Parameters.AddWithValue("@FactureNumber", FactureNumber);
        objCmd.Parameters.AddWithValue("@Description", Description);
        objCmd.Parameters.Add("@Price", System.Data.SqlDbType.Decimal).Value = Price;
        objCmd.Parameters.Add("@Operater", System.Data.SqlDbType.Int).Value = Operater;

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while inserting All values. " + ex.Message);
        throw new Exception("Error while inserting All values. " + ex.Message);
    }
}

public void upisiZaduzenje(int TypeOfServiceSelectedValue, int CashierSelectedValue, decimal Price, string Date, int Operater)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand("blSpZaduzivanjeBlagajnika", objConn);
        objCmd.CommandType = CommandType.StoredProcedure;

        objCmd.Parameters.Add("@IDTipStavkeBlagajnickogIzvestaja", System.Data.SqlDbType.Int).Value = TypeOfServiceSelectedValue;
        objCmd.Parameters.AddWithValue("@Datum", Date);
        objCmd.Parameters.Add("@Iznos", System.Data.SqlDbType.Decimal).Value = Price;
        objCmd.Parameters.Add("@Operater", System.Data.SqlDbType.Int).Value = CashierSelectedValue;

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while inserting All values. " + ex.Message);
        throw new Exception("Error while inserting All values. " + ex.Message);
    }
}

public void BindGridView(GridView GridView1, out DataTable dt)
{
    using (SqlConnection con = new SqlConnection(scnsconnectionstring))
    {
        using (SqlCommand cmd = new SqlCommand("SELECT TOP (100) PERCENT dbo.blEksternoPlacanje.IDEksternoPlacanje, dbo.blEksternoPlacanje.DatumPlacanja, dbo.blEksternoPlacanje.BrojPlacanja, dbo.blEksternoPlacanje.Opis, dbo.blEksternoPlacanje.Iznos,  dbo.blEksternoPlacanje.Operater, dbo.blOrganizacija.NazivOrganizacije, dbo.pisTipPlacanja.TipPlacanja, dbo.blEksternoPlacanje.Ponisteno FROM dbo.blEksternoPlacanje INNER JOIN dbo.blOrganizacija ON dbo.blEksternoPlacanje.IDOrganizacija = dbo.blOrganizacija.IDOrganizacija INNER JOIN dbo.pisTipPlacanja ON dbo.blEksternoPlacanje.IDTipPlacanja = dbo.pisTipPlacanja.IDTipPlacanja ORDER BY dbo.blEksternoPlacanje.IDEksternoPlacanje DESC"))
        {
            SqlDataAdapter sda = new SqlDataAdapter();
            try
            {
                cmd.Connection = con;
                con.Open();
                sda.SelectCommand = cmd;

                dt = new DataTable();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                log.Error("Error while BindGridView. " + ex.Message);
                throw new Exception("Error while BindGridView. " + ex.Message);
            }
        }
    }
}


public void BindGridViewZaduzenja(GridView GridView1, out DataTable dt)
{
    using (SqlConnection con = new SqlConnection(scnsconnectionstring))
    {
        using (SqlCommand cmd = new SqlCommand("SELECT TOP (100) PERCENT IDStavkaBlagajnickogIzvestaja, PunoIme, TipStavkeBlagajnickogIzvestaja, Datum, Iznos, KadaJeUpisano, Storno FROM dbo.blVPregledUpisanihZaduzenja ORDER BY KadaJeUpisano DESC"))
        {
            SqlDataAdapter sda = new SqlDataAdapter();
            try
            {
                cmd.Connection = con;
                con.Open();
                sda.SelectCommand = cmd;

                dt = new DataTable();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                log.Error("Error while BindGridView. " + ex.Message);
                throw new Exception("Error while BindGridView. " + ex.Message);
            }
        }
    }
}

public void obrisiRed(int IDRow)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"update blEksternoPlacanje set Ponisteno=getDate() where IDEksternoPlacanje=@idrow", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.Add("@idrow", System.Data.SqlDbType.Int).Value = IDRow;

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while setting Ponisteno=1 on Row with ID: " + IDRow + ". " + ex.Message);
        throw new Exception("Error while setting Ponisteno=1 on Row with ID: " + IDRow + ". " + ex.Message);
    }
}

public void stornirajRed(int IDRow)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand("blSpPonistavanjeZaduzenjaBlagajnika", objConn);
        objCmd.CommandType = CommandType.StoredProcedure;

        objCmd.Parameters.Add("@IDStavkaBlagajnickogIzvestaja", System.Data.SqlDbType.Int).Value = IDRow;

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while call procedure blSpPonistavanjeZaduzenjaBlagajnika on Row with ID: " + IDRow + ". " + ex.Message);
        throw new Exception("Error while call procedure blSpPonistavanjeZaduzenjaBlagajnika on Row with ID: " + IDRow + ". " + ex.Message);
    }
}

public void editujRed(string txtBrojPlacanja, string txtIznos, string txtDatumPlacanja, string txtOpis, int IDRow)
{
    try
    {
        SqlConnection objConn = new SqlConnection(scnsconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"update blEksternoPlacanje set BrojPlacanja=@txtBrojPlacanja, Iznos = @txtIznos, DatumPlacanja=@txtDatumPlacanja, Opis=@txtOpis where IDEksternoPlacanje=@idrow", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.AddWithValue("@txtBrojPlacanja", txtBrojPlacanja);
        objCmd.Parameters.AddWithValue("@txtIznos", txtIznos);
        objCmd.Parameters.AddWithValue("@txtDatumPlacanja", txtDatumPlacanja);
        objCmd.Parameters.AddWithValue("@txtOpis", txtOpis);
        objCmd.Parameters.Add("@idrow", System.Data.SqlDbType.Int).Value = IDRow;

        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while editing Row with ID: " + IDRow + ". " + ex.Message);
        throw new Exception("Error while editing Row with ID: " + IDRow + ". " + ex.Message);
    }
}


public void BindSearchingGridView(GridView GridView1, string BrojFakture, out DataTable dt)
{
    using (SqlConnection con = new SqlConnection(scnsconnectionstring))
    {   
        using (SqlCommand cmd = new SqlCommand("SELECT TOP (100) PERCENT dbo.blEksternoPlacanje.IDEksternoPlacanje, dbo.blEksternoPlacanje.DatumPlacanja,dbo.blEksternoPlacanje.BrojPlacanja, dbo.blEksternoPlacanje.Opis, dbo.blEksternoPlacanje.Iznos, dbo.blEksternoPlacanje.Operater, dbo.blOrganizacija.NazivOrganizacije, dbo.pisTipPlacanja.TipPlacanja, dbo.blEksternoPlacanje.Ponisteno FROM  dbo.blEksternoPlacanje INNER JOIN dbo.blOrganizacija ON dbo.blEksternoPlacanje.IDOrganizacija = dbo.blOrganizacija.IDOrganizacija INNER JOIN dbo.pisTipPlacanja ON dbo.blEksternoPlacanje.IDTipPlacanja = dbo.pisTipPlacanja.IDTipPlacanja WHERE(dbo.blEksternoPlacanje.BrojPlacanja like ('%" + BrojFakture + "%')) ORDER BY dbo.blEksternoPlacanje.IDEksternoPlacanje DESC"))
        {
            cmd.Parameters.AddWithValue("@BrojFakture", BrojFakture);
            SqlDataAdapter sda = new SqlDataAdapter();
            try
            {
                cmd.Connection = con;
                con.Open();
                sda.SelectCommand = cmd;

               dt = new DataTable();
               sda.Fill(dt);
            }
            catch (Exception ex)
            {
                log.Error("Error while BindSearchingGridView. " + ex.Message);
                throw new Exception("Error while BindSearchingGridView. " + ex.Message);
            }
        }
    }
}


public void BindSearchingGridViewUpdate(GridView GridView1, string BrojFakture, string Iznos, string DatumPlacanja, string Opis, out DataTable dt)
{
    using (SqlConnection con = new SqlConnection(scnsconnectionstring))
    {
        using (SqlCommand cmd = new SqlCommand("SELECT TOP (100) PERCENT dbo.blEksternoPlacanje.IDEksternoPlacanje, dbo.blEksternoPlacanje.DatumPlacanja, dbo.blEksternoPlacanje.BrojPlacanja, dbo.blEksternoPlacanje.Opis, dbo.blEksternoPlacanje.Iznos, dbo.blEksternoPlacanje.Operater, dbo.blOrganizacija.NazivOrganizacije, dbo.pisTipPlacanja.TipPlacanja, dbo.blEksternoPlacanje.Ponisteno FROM  dbo.blEksternoPlacanje INNER JOIN dbo.blOrganizacija ON dbo.blEksternoPlacanje.IDOrganizacija = dbo.blOrganizacija.IDOrganizacija INNER JOIN dbo.pisTipPlacanja ON dbo.blEksternoPlacanje.IDTipPlacanja = dbo.pisTipPlacanja.IDTipPlacanja WHERE(dbo.blEksternoPlacanje.BrojPlacanja = @BrojFakture) AND(dbo.blEksternoPlacanje.Iznos = @Iznos) AND(dbo.blEksternoPlacanje.DatumPlacanja = @DatumPlacanja) AND(dbo.blEksternoPlacanje.Opis = @Opis) ORDER BY dbo.blEksternoPlacanje.IDEksternoPlacanje DESC"))
        {
            cmd.Parameters.AddWithValue("@BrojFakture", BrojFakture);
            cmd.Parameters.AddWithValue("@Iznos", Iznos);
            cmd.Parameters.AddWithValue("@DatumPlacanja", DatumPlacanja);
            cmd.Parameters.AddWithValue("@Opis", Opis);
            SqlDataAdapter sda = new SqlDataAdapter();
            try
            {
                cmd.Connection = con;
                con.Open();
                sda.SelectCommand = cmd;

                dt = new DataTable();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                log.Error("Error while BindSearchingGridViewUpdate. " + ex.Message);
                throw new Exception("Error while BindSearchingGridViewUpdate. " + ex.Message);
            }
        }
    }
}


public void BindSearchingGridViewZaduzenja(GridView GridView1, string SelectedValue, out DataTable dt)
{
    using (SqlConnection con = new SqlConnection(scnsconnectionstring))
    {
        using (SqlCommand cmd = new SqlCommand("SELECT TOP (100) PERCENT IDStavkaBlagajnickogIzvestaja, PunoIme, TipStavkeBlagajnickogIzvestaja, Datum, Iznos, KadaJeUpisano, Storno FROM dbo.blVPregledUpisanihZaduzenja WHERE (PunoIme =  @punoime) ORDER BY KadaJeUpisano DESC"))
        {
            cmd.Parameters.AddWithValue("@punoime", SelectedValue);
            SqlDataAdapter sda = new SqlDataAdapter();
            try
            {
                cmd.Connection = con;
                con.Open();
                sda.SelectCommand = cmd;

                dt = new DataTable();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                log.Error("Error while BindSearchingGridViewZaduzenja. " + ex.Message);
                throw new Exception("Error while BindSearchingGridViewZaduzenja. " + ex.Message);
            }
        }
    }
}
/*
public List<WebControl> pronadjiKontrole(string page)
{
    try
    {
        List<WebControl> responses = new List<WebControl>();
        SqlConnection objConn = new SqlConnection(postaconnectionstring);
        SqlCommand objCmd = new SqlCommand("SELECT WebControl.ControlId, WebControl.ValidationActive,WebControl.IsRequired FROM WebControl INNER JOIN WebPage ON WebControl.IDWebPage = WebPage.IDWebPage WHERE WebPage.FileName ='" + page + "'", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;
        objConn.Open();
        SqlDataReader reader = objCmd.ExecuteReader();
        while (reader.Read())
        {
            responses.Add(new WebControl((reader.GetSqlString(0)).ToString(), reader.GetBoolean(1), reader.GetBoolean(2)));
        }
        objConn.Close();

        return responses;
    }
    catch (Exception ex)
    {
        log.Error("Error while sellecting pronadjiKontrole. " + ex.Message);
        throw new Exception("Error while sellecting pronadjiKontrole. " + ex.Message);
    }
}

public List<WebControl> pronadjiKontrolePoTipu(string page, string controltype)
{
    try
    {
        List<WebControl> responses = new List<WebControl>();
        SqlConnection objConn = new SqlConnection(postaconnectionstring);
        SqlCommand objCmd = new SqlCommand("SELECT WebControl.ControlId, WebControl.ValidationActive,WebControl.IsRequired FROM WebControl INNER JOIN WebPage ON WebControl.IDWebPage = WebPage.IDWebPage WHERE WebPage.FileName ='" + page + "' AND WebControl.ControlType = '" + controltype + "'", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;
        objConn.Open();
        SqlDataReader reader = objCmd.ExecuteReader();
        while (reader.Read())
        {
            responses.Add(new WebControl((reader.GetSqlString(0)).ToString(), reader.GetBoolean(1), reader.GetBoolean(2)));
        }
        objConn.Close();

        return responses;
    }
    catch (Exception ex)
    {
        log.Error("Error while sellecting pronadjiKontrolePoTipu. " + ex.Message);
        throw new Exception("Error while sellecting pronadjiKontrolePoTipu. " + ex.Message);
    }
}

public List<PTTVariable> pronadjiPromenljivePTT(string filename, string controlid)
{
    List<PTTVariable> responses = new List<PTTVariable>();

    string upit = @"SELECT  Item.IDItem, ISNULL(LegalEntity.City, N'') AS City, ISNULL(LegalEntity.Street, N'') AS Street, ISNULL(LegalEntity.HouseNumber, N'') AS HouseNumber, ISNULL(LegalEntity.ZipCode, N'') AS ZipCode, 
                     ISNULL(LegalEntity.PAK, N'') AS PAK, ISNULL(LegalEntity.InHouse, 0) AS InHouse, ControlItem.IsAllowed, CASE WHEN Item.Itemtextenglish = 'LEGAL_ENTITY_ADDRESS' THEN 1 ELSE 0 END AS IsLegalEntity

                     FROM  LegalEntity INNER JOIN
                     ItemEntity ON LegalEntity.IDLegalEntity = ItemEntity.IDLegalEntity RIGHT OUTER JOIN
                     WebPage INNER JOIN
                     WebControl ON WebPage.IDWebPage = WebControl.IDWebPage INNER JOIN
                     ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                     Item ON ControlItem.IDItem = Item.IDItem ON ItemEntity.IDItem = Item.IDItem
                     WHERE  (WebControl.ControlId = @controlid) AND (WebPage.FileName = @filename)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new PTTVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetSqlString(3).ToString(), reader.GetSqlString(4).ToString(), reader.GetSqlString(5).ToString(), reader.GetBoolean(6), reader.GetBoolean(7), reader.GetInt32(8)));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting PTTVariable. " + ex.Message);
                throw new Exception("Error while getting PTTVariable. " + ex.Message);
            }
        }
    }

    return responses;
}

public string getEnglishText(int idItem)
{
    string itemTextEnglish = string.Empty;

    string upit = @"SELECT        TOP (1) ItemTextEnglish
                   FROM            dbo.Item
                   WHERE        (IDItem = @idItem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemTextEnglish = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemTextEnglish. " + ex.Message);
                throw new Exception("Error while getting itemTextEnglish. " + ex.Message);
            }
        }
    }

    return itemTextEnglish;
}

public string getEnglishTextInputString(string itemText)
{
    string itemTextEnglish = string.Empty;

    string upit = @"SELECT        TOP (1) ItemTextEnglish
                    FROM            dbo.Item
                    WHERE        (ItemText = @itemtext)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@itemtext", itemText);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemTextEnglish = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemTextEnglish while input string. " + ex.Message);
                throw new Exception("Error while getting itemTextEnglish while input string. " + ex.Message);
            }
        }
    }

    return itemTextEnglish;
}

public string getEnglishTextItemText(string itemtext)
{
    string itemTextEnglish = string.Empty;

    string upit = @"SELECT        TOP (1) ItemTextEnglish
                    FROM            dbo.Item
                    WHERE        (ItemText = @itemtext)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@itemtext", itemtext);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemTextEnglish = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemTextEnglish. " + ex.Message);
                throw new Exception("Error while getting itemTextEnglish. " + ex.Message);
            }
        }
    }

    return itemTextEnglish;
}

public List<ItemVariable> getIdItemDefault(string filename, string controlid)
{
    List<ItemVariable> responses = new List<ItemVariable>();

    string upit = @"SELECT  TOP (100) PERCENT Item.IDItem, Item.ItemText, ControlItem.IsDefault
                    FROM    WebControl INNER JOIN
                    WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                    ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                    Item ON ControlItem.IDItem = Item.IDItem
                    WHERE        (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid)
                    ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    responses.Add(new ItemVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetBoolean(2)));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting IdItemDefault. " + ex.Message);
                throw new Exception("Error while getting IdItemDefault. " + ex.Message);
            }
        }
    }

    return responses;
}

public string getItemText(int idItem)
{
    string itemText = string.Empty;

    string upit = @"SELECT        TOP (1) ItemText
                   FROM            dbo.Item
                   WHERE        (IDItem = @idItem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemText = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemText. " + ex.Message);
                throw new Exception("Error while getting itemText. " + ex.Message);
            }
        }
    }

    return itemText;
}

public int getItemValueAddedTax(int idItem)
{
    int itemValue = 0;

    string upit = @"SELECT        TOP (1) ItemValue
                   FROM            dbo.Item
                   WHERE        (IDItem = @idItem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemValue = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemValue. " + ex.Message);
                throw new Exception("Error while getting itemValue. " + ex.Message);
            }
        }
    }

    return itemValue;
}

public List<LegalEntityVariable> pronadjiPromenljiveLegalEntity(string IdentificationNumber)
{
    List<LegalEntityVariable> responses = new List<LegalEntityVariable>();

    string upit = @"SELECT        IDLegalEntity, FullName, PIB, PDVpayer, BysinessTypeCode, Street, HouseNumber, ZipCode, PAK, City, PhoneNumber, EmailAddress
                    FROM          LegalEntity
                    WHERE        (IdentificationNumber = @IdentificationNumber)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@IdentificationNumber", IdentificationNumber);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new LegalEntityVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetBoolean(3), reader.GetSqlString(4).ToString(), reader.GetSqlString(5).ToString(), reader.GetSqlString(6).ToString(), reader.GetSqlString(7).ToString(), reader.GetSqlString(8).ToString(), reader.GetSqlString(9).ToString(), reader.GetSqlString(10).ToString(), reader.GetSqlString(11).ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting LegalEntityVariable. " + ex.Message);
                throw new Exception("Error while getting LegalEntityVariable. " + ex.Message);
            }
        }
    }

    return responses;
}

public int getIDItem(int itemvalue, string filename, string controlid)
{
    int IDItem = 0;

    string upit = @"SELECT   TOP (100) PERCENT Item.IDItem
                    FROM     WebControl INNER JOIN
                    WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                    ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                    Item ON ControlItem.IDItem = Item.IDItem
                    WHERE  (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid) AND (Item.ItemValue = @itemvalue)
                    ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@itemvalue", System.Data.SqlDbType.Int).Value = itemvalue;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    IDItem = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting IDItem. " + ex.Message);
                throw new Exception("Error while getting IDItem. " + ex.Message);
            }
        }
    }

    return IDItem;
}

public int getItemValue(int selectedvalue, string filename, string controlid)
{
    int ItemValue = 0;

    string upit = @"SELECT  TOP (100) PERCENT Item.ItemValue
                    FROM   WebControl INNER JOIN
                    WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                    ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                    Item ON ControlItem.IDItem = Item.IDItem
                    WHERE  (WebControl.ControlId = @controlid) AND (WebPage.FileName = @filename) AND (Item.IDItem = @selectedvalue)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@selectedvalue", System.Data.SqlDbType.Int).Value = selectedvalue;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    ItemValue = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting ItemValue. " + ex.Message);
                throw new Exception("Error while getting ItemValue. " + ex.Message);
            }
        }
    }

    return ItemValue;
}

public List<StatusChangeVariable> pronadjiPromenljiveStatusChange(string filename, string controlid)
{
    List<StatusChangeVariable> responses = new List<StatusChangeVariable>();

    string upit = @"SELECT  TOP (100) PERCENT Item.IDItem, ControlItem.IsAllowed, ControlItem.IsDefault, Item.ItemValue
                    FROM   WebPage INNER JOIN
                    WebControl ON WebPage.IDWebPage = WebControl.IDWebPage INNER JOIN
                    ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                    Item ON ControlItem.IDItem = Item.IDItem
                    WHERE   (WebControl.ControlId = @controlid) AND (WebPage.FileName = @filename) AND (dbo.ControlItem.Active = 1)
                    ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new StatusChangeVariable(reader.GetInt32(0), reader.GetBoolean(1), reader.GetBoolean(2), reader.GetInt32(3)));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting StatusChangeVariable. " + ex.Message);
                throw new Exception("Error while getting StatusChangeVariable. " + ex.Message);
            }
        }
    }

    return responses;
}

public List<CertificateStatus> pronadjiPromenljiveStatusSertifikata(int IDTypeOfItem)
{
    List<CertificateStatus> responses = new List<CertificateStatus>();

    string upit = @"SELECT Item.ItemText, Item.ItemTextEnglish, ISNULL(ItemCertificateStatus.Notification, N'') AS Notification, Item.ItemValue
                    FROM   Item INNER JOIN
                    TypeOfItem ON Item.IDTypeOfItem = TypeOfItem.IDTypeOfItem LEFT OUTER JOIN
                    ItemCertificateStatus ON Item.IDItem = ItemCertificateStatus.IDItem
                    WHERE  (TypeOfItem.IDTypeOfItem = @idtypeofitem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new CertificateStatus(reader.GetSqlString(0).ToString(), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetInt32(3)));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting StatusVariable. " + ex.Message);
                throw new Exception("Error while getting StatusVariable. " + ex.Message);
            }
        }
    }

    return responses;
}

public void upisiPravnoLice(string maticnibroj, string nazivpravnoglica, string pib, bool obveznikpdv, string sifradel, string ulica, string broj, string postanskibroj, string pak, string mesto, string telefon, string adresaeposte, bool inhouse)
{
    try
    {
        SqlConnection objConn = new SqlConnection(postaconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"insert into LegalEntity (IdentificationNumber, FullName, PIB, PDVPayer, BysinessTypeCode, Street, HouseNumber, ZipCode, PAK, City, PhoneNumber, EmailAddress, InHouse) values (@maticnibroj, @nazivpravnoglica, @pib, @obveznikpdv, @sifradel, @ulica, @broj, @postanskibroj, @pak, @mesto, @telefon, @adresaeposte, @inhouse)", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.AddWithValue("@maticnibroj", maticnibroj);
        objCmd.Parameters.AddWithValue("@nazivpravnoglica", nazivpravnoglica);
        objCmd.Parameters.AddWithValue("@pib", pib);
        objCmd.Parameters.AddWithValue("@obveznikpdv", obveznikpdv);
        objCmd.Parameters.AddWithValue("@sifradel", sifradel);
        objCmd.Parameters.AddWithValue("@ulica", ulica);
        objCmd.Parameters.AddWithValue("@broj", broj);
        objCmd.Parameters.AddWithValue("@postanskibroj", postanskibroj);
        objCmd.Parameters.AddWithValue("@pak", pak);
        objCmd.Parameters.AddWithValue("@mesto", mesto);
        objCmd.Parameters.AddWithValue("@telefon", telefon);
        objCmd.Parameters.AddWithValue("@adresaeposte", adresaeposte);
        objCmd.Parameters.AddWithValue("@inhouse", inhouse);
        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while inserting LegalEntity values. " + ex.Message);
        throw new Exception("Error while inserting LegalEntity values. " + ex.Message);
    }
}

public void editujPravnoLice(string maticnibroj, string nazivpravnoglica, string pib, bool obveznikpdv, string sifradel, string ulica, string broj, string postanskibroj, string pak, string mesto, string telefon, string adresaeposte, bool inhouse)
{
    try
    {
        SqlConnection objConn = new SqlConnection(postaconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"update LegalEntity set IdentificationNumber = @maticnibroj, PIB = @pib, PDVPayer = @obveznikpdv, BysinessTypeCode = @sifradel, Street = @ulica, HouseNumber = @broj, ZipCode = @postanskibroj, PAK = @pak, City=@mesto, PhoneNumber = @telefon, EmailAddress = @adresaeposte, InHouse = @inhouse where FullName = @nazivpravnoglica", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.AddWithValue("@maticnibroj", maticnibroj);
        objCmd.Parameters.AddWithValue("@nazivpravnoglica", nazivpravnoglica);
        objCmd.Parameters.AddWithValue("@pib", pib);
        objCmd.Parameters.AddWithValue("@obveznikpdv", obveznikpdv);
        objCmd.Parameters.AddWithValue("@sifradel", sifradel);
        objCmd.Parameters.AddWithValue("@ulica", ulica);
        objCmd.Parameters.AddWithValue("@broj", broj);
        objCmd.Parameters.AddWithValue("@postanskibroj", postanskibroj);
        objCmd.Parameters.AddWithValue("@pak", pak);
        objCmd.Parameters.AddWithValue("@mesto", mesto);
        objCmd.Parameters.AddWithValue("@telefon", telefon);
        objCmd.Parameters.AddWithValue("@adresaeposte", adresaeposte);
        objCmd.Parameters.AddWithValue("@inhouse", inhouse);
        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while editting LegalEntity values. " + ex.Message);
        throw new Exception("Error while editting LegalEntity values. " + ex.Message);
    }
}

public List<FulNameLegalEntity> pronadjiNazivPravnogLica()
{
    List<FulNameLegalEntity> responses = new List<FulNameLegalEntity>();

    string upit = @"SELECT        IDLegalEntity, FullName
                    FROM          LegalEntity";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new FulNameLegalEntity(reader.GetInt32(0), reader.GetSqlString(1).ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting FulNameLegalEntity. " + ex.Message);
                throw new Exception("Error while getting FulNameLegalEntity. " + ex.Message);
            }
        }
    }

    return responses;
}

public string getItemTextSatus(string itemTextEnglish)
{
    string itemText = string.Empty;

    string upit = @"SELECT        TOP (1) ItemText
                    FROM          Item
                    WHERE        (ItemTextEnglish = @itemtextenglish)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@itemtextenglish", itemTextEnglish);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemText = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemText. " + ex.Message);
                throw new Exception("Error while getting itemText. " + ex.Message);
            }
        }
    }

    return itemText;
}

public string getItemTextEnglishSatus(string itemText)
{
    string itemTextEng = string.Empty;

    string upit = @"SELECT        TOP (1) ItemTextEnglish
                    FROM          Item
                    WHERE        (ItemText = @itemtext)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@itemtext", itemText);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemTextEng = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemTextEng. " + ex.Message);
                throw new Exception("Error while getting itemTextEng. " + ex.Message);
            }
        }
    }

    return itemTextEng;
}

public string getCertificateRoot(string SettingName)
{
    string CertificateRoot = string.Empty;

    string upit = @"SELECT        SettingValue
                    FROM            dbo.GlobalSetting
                    WHERE        (SettingName = @settingname)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@settingname", SettingName);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    CertificateRoot = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting CertificateRoot. " + ex.Message);
                throw new Exception("Error while getting CertificateRoot. " + ex.Message);
            }
        }
    }

    return CertificateRoot;
}

public string getCertificateName(int idItem)
{
    string CertificateName = string.Empty;

    string upit = @"SELECT        TOP (1) CertificateName
                    FROM        dbo.ItemRevocationCheckMethod
                    WHERE        (IDItem = @idItem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    CertificateName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting CertificateName. " + ex.Message);
                throw new Exception("Error while getting CertificateName. " + ex.Message);
            }
        }
    }

    return CertificateName;
}

public int getOrderNumber(int IDOrderNumber)
{    
    int LastUsed = 0;
    int OrderNumber = 0;

    string upit = @"SELECT TOP (1) LastUsed
                    FROM  OrderNumberRange
                    WHERE (IDOrderNumberRange = @idordernumber)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idordernumber", System.Data.SqlDbType.Int).Value = IDOrderNumber;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    LastUsed = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting LastUsed. " + ex.Message);
                throw new Exception("Error while getting LastUsed. " + ex.Message);
            }
        }
    }
    OrderNumber = LastUsed + IDOrderNumber;
    log.Error("Last used is: " + LastUsed + ". OrderNumber is: " + OrderNumber);
    return OrderNumber;
}

public void editujPoslednjeKoriscenOrderNumber(int OrderNumber, int IDOrderNumber)
{
    try
    {
        SqlConnection objConn = new SqlConnection(postaconnectionstring);
        SqlCommand objCmd = new SqlCommand(@"update OrderNumberRange set LastUsed = @ordernumber where IDOrderNumberRange = @IDOrderNumberRange", objConn);
        objCmd.CommandType = System.Data.CommandType.Text;

        objCmd.Parameters.Add("@ordernumber", System.Data.SqlDbType.Int).Value = OrderNumber;
        objCmd.Parameters.Add("@IDOrderNumberRange", System.Data.SqlDbType.Int).Value = IDOrderNumber;
        objConn.Open();
        objCmd.ExecuteNonQuery();
        objConn.Close();
    }
    catch (Exception ex)
    {
        log.Error("Error while editting OrderNumber value. " + ex.Message);
        throw new Exception("Error while editting OrderNumber value. " + ex.Message);
    }
}

public string getURLocsp(string filename, string controlid, int revocationchecktype, int selectedvalue)
{
    string url = string.Empty;

    string upit = @"SELECT        TOP (1) PERCENT ItemRevocationCheckMethod.URL
                    FROM          WebControl INNER JOIN
                     WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                     ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                     Item ON ControlItem.IDItem = Item.IDItem LEFT OUTER JOIN
                     ItemRevocationCheckMethod ON Item.IDItem = ItemRevocationCheckMethod.IDItem
                     WHERE   (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid) AND (ControlItem.Active = 1) AND (ItemRevocationCheckMethod.RevocationCheckType = @revocationchecktype) AND (ItemRevocationCheckMethod.IDItem = @selectedvalue)
                     ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objCmd.Parameters.Add("@revocationchecktype", System.Data.SqlDbType.Int).Value = revocationchecktype;
                objCmd.Parameters.Add("@selectedvalue", System.Data.SqlDbType.Int).Value = selectedvalue;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    url = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting revocation URL. " + ex.Message);
                throw new Exception("Error while getting revocation URL. " + ex.Message);
            }
        }
    }

    return url;
}

public string getURLcrl(string filename, string controlid, int revocationchecktype)
{
    string url = string.Empty;

    string upit = @"SELECT        TOP (1) PERCENT ItemRevocationCheckMethod.URL
                    FROM          WebControl INNER JOIN
                     WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                     ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                     Item ON ControlItem.IDItem = Item.IDItem LEFT OUTER JOIN
                     ItemRevocationCheckMethod ON Item.IDItem = ItemRevocationCheckMethod.IDItem
                     WHERE   (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid) AND (ControlItem.Active = 1) AND (ItemRevocationCheckMethod.RevocationCheckType = @revocationchecktype)
                     ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objCmd.Parameters.Add("@revocationchecktype", System.Data.SqlDbType.Int).Value = revocationchecktype;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    url = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting revocation URL. " + ex.Message);
                throw new Exception("Error while getting revocation URL. " + ex.Message);
            }
        }
    }

    return url;
}


public List<SerialNoVariable> pronadjiPromenljiveSerialNo(string filename, string controlid)
{
    List<SerialNoVariable> responses = new List<SerialNoVariable>();

    string upit = @"SELECT        TOP (100) PERCENT ISNULL(dbo.Property.PropertyName, N'') AS PropertyName, ISNULL(CAST(dbo.WebControlProperties.PropertyValue AS int), 0) AS PropertyValue
                    FROM            dbo.WebControl INNER JOIN
                                    dbo.WebPage ON dbo.WebControl.IDWebPage = dbo.WebPage.IDWebPage LEFT OUTER JOIN
                                    dbo.Property INNER JOIN
                                    dbo.WebControlProperties ON dbo.Property.IDProperty = dbo.WebControlProperties.IDProperty ON dbo.WebControl.IDWebControl = dbo.WebControlProperties.IDWebControl
                    WHERE        (dbo.WebPage.FileName = @filename) AND (dbo.WebControl.ControlId = @controlid)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new SerialNoVariable(reader.GetSqlString(0).ToString(), reader.GetInt32(1)));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting SerialNoVariable. " + ex.Message);
                throw new Exception("Error while getting SerialNoVariable. " + ex.Message);
            }
        }
    }

    return responses;
}

public List<PhonePrefixVariable> pronadjiPrefikseMobilnihTelefona(int IDTypeOfItem)
{
    List<PhonePrefixVariable> responses = new List<PhonePrefixVariable>();

    string upit = @"SELECT        TOP (100) PERCENT IDItem, ItemText, Active
                    FROM          Item
                    WHERE        (IDTypeOfItem = @idtypeofitem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new PhonePrefixVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetBoolean(2)));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting PhonePrefixVariable. " + ex.Message);
                throw new Exception("Error while getting PhonePrefixVariable. " + ex.Message);
            }
        }
    }

    return responses;
}

public List<WebControlLanguage> pronadjiSvaPoljaNaStranici(string filename)
{
    List<WebControlLanguage> responses = new List<WebControlLanguage>();

    string upit = @"SELECT        TOP (100) PERCENT WebControlLanguage.ControlId, WebPageLanguage.PageTitle, WebControlLanguage.ControlTitle, WebControlLanguage.ValidationActive, WebControlLanguage.IsVisible, 
                     WebControlLanguage.IsEnabled, WebControlLanguage.IsRequired, WebControlLanguage.ControlType
                    FROM            WebControlLanguage INNER JOIN
                     WebPageLanguage ON WebControlLanguage.IDWebPage = WebPageLanguage.IDWebPage
                    WHERE        (WebPageLanguage.FileName = @filename)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new WebControlLanguage(reader.GetSqlString(0).ToString(), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetBoolean(3), reader.GetBoolean(4), reader.GetBoolean(5), reader.GetBoolean(6), reader.GetSqlString(7).ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting WebControlLanguage. " + ex.Message);
                throw new Exception("Error while getting WebControlLanguage. " + ex.Message);
            }
        }
    }

    return responses;
}

public string pronadjiNaziveGresaka(int idtypeofitem, int IDItem)
{
    string itemText = string.Empty;

    string upit = @"SELECT        TOP (1) ItemText
                   FROM           dbo.Item
                   WHERE        (IDItem = @iditem) AND (IDTypeOfItem = @idtypeofitem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@iditem", System.Data.SqlDbType.Int).Value = IDItem;
                objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = idtypeofitem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemText = reader.GetString(0);
                }                   
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemText for Error message. " + ex.Message);
                throw new Exception("Error while getting itemText for Error message. " + ex.Message);
            }
        }
    }

    return itemText;
}

public string getSettingsValueGlobalSettings(string Validation)
{
    string SettingsValue = string.Empty;

    string upit = @"SELECT        SettingValue
                    FROM            dbo.GlobalSetting
                    WHERE        (SettingName = @validation)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@validation", Validation);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    SettingsValue = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting SettingsValue. " + ex.Message);
                throw new Exception("Error while getting SettingsValue. " + ex.Message);
            }
        }
    }

    return SettingsValue;
}

public bool pronadjiDaLiJeStranicaAktivna(string filename)
{
    bool Active = true;

    string upit = @"SELECT        Active
                FROM        dbo.WebPage
                WHERE        (FileName = @filename)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    Active = reader.GetBoolean(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting Active Value. " + ex.Message);
                throw new Exception("Error while getting Active Value. " + ex.Message);
            }
        }
    }

    return Active;
}

public bool pronadjiDaLiJeStranicaUputstvoAktivna(string filename)
{
    bool ActiveAgreeme = true;

    string upit = @"SELECT        ShowAgreement
                    FROM            dbo.WebPage
                    WHERE        (FileName = @filename)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    ActiveAgreeme = reader.GetBoolean(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting ActiveAgreeme Value. " + ex.Message);
                throw new Exception("Error while getting ActiveAgreeme Value. " + ex.Message);
            }
        }
    }

    return ActiveAgreeme;
}

public string getPropertyValue(string controlid, string filename)
{
    string PropertyValue = string.Empty;

    string upit = @"SELECT        TOP (1) PERCENT dbo.WebControlProperties.PropertyValue
                    FROM            dbo.WebControl INNER JOIN
                     dbo.WebPage ON dbo.WebControl.IDWebPage = dbo.WebPage.IDWebPage LEFT OUTER JOIN
                     dbo.Property INNER JOIN
                     dbo.WebControlProperties ON dbo.Property.IDProperty = dbo.WebControlProperties.IDProperty ON dbo.WebControl.IDWebControl = dbo.WebControlProperties.IDWebControl
                    WHERE        (dbo.WebControl.ControlId = @controlid) AND (dbo.WebPage.FileName = @filename)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@filename", filename);
                objCmd.Parameters.AddWithValue("@controlid", controlid);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    PropertyValue = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting PropertyValue. " + ex.Message);
                throw new Exception("Error while getting PropertyValue. " + ex.Message);
            }
        }
    }

    return PropertyValue;
}

public string getCountryCode(int idItem)
{
    string CountryCode = string.Empty;

    string upit = @"SELECT        TOP (1) dbo.ItemCountry.CountryCode
                    FROM            dbo.Item INNER JOIN
                     dbo.ItemCountry ON dbo.Item.IDItem = dbo.ItemCountry.IDItem
                    WHERE        (dbo.Item.IDItem = @idItem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    CountryCode = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting CountryCode. " + ex.Message);
                throw new Exception("Error while getting CountryCode. " + ex.Message);
            }
        }
    }

    return CountryCode;
}

public string getCountryCodeInputString(string itemText)
{
    string CountryCode = string.Empty;

    string upit = @"SELECT        TOP (1) dbo.ItemCountry.CountryCode
                    FROM            dbo.Item INNER JOIN
                     dbo.ItemCountry ON dbo.Item.IDItem = dbo.ItemCountry.IDItem
                    WHERE        (dbo.Item.ItemText = @itemtext)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@itemText", itemText);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    CountryCode = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting CountryCode while input string. " + ex.Message);
                throw new Exception("Error while CountryCode while input string. " + ex.Message);
            }
        }
    }

    return CountryCode;
}

public string getItemTextIDTypeOfItem(int IDTypeOfItem)
{
    string itemText = string.Empty;

    string upit = @"SELECT        TOP (1) ItemText
                    FROM            dbo.Item
                    WHERE        (IDTypeOfItem = @idtypeofitem)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    itemText = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting itemText in function getItemTextIDTypeOfItem. " + ex.Message);
                throw new Exception("Error while getting itemText in function getItemTextIDTypeOfItem. " + ex.Message);
            }
        }
    }

    return itemText;
}


public string getRevocationMethod(string revocationmethodenglish)
{
    string revocationmethod = string.Empty;

    string upit = @"SELECT        ItemText
                    FROM            dbo.Item
                    WHERE        (ItemTextEnglish = @itemtextenglish)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@itemtextenglish", revocationmethodenglish);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    revocationmethod = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting revocationmethod in function getRevocationMethod. " + ex.Message);
                throw new Exception("Error while getting revocationmethod in function getRevocationMethod. " + ex.Message);
            }
        }
    }

    return revocationmethod;
}

public string getMinAndMaxSerialLength(string SettingName)
{
    string SerialLength = string.Empty;

    string upit = @"SELECT        SettingValue
                    FROM            dbo.GlobalSetting
                    WHERE        (SettingName = @settingname)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@settingname", SettingName);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    SerialLength = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting SerialLength. " + ex.Message);
                throw new Exception("Error while getting SerialLength. " + ex.Message);
            }
        }
    }

    return SerialLength;
}

public string getpkcs12timeout(string SettingName)
{
    string pkcs12timeout = string.Empty;

    string upit = @"SELECT        SettingValue
                    FROM            dbo.GlobalSetting
                    WHERE        (SettingName = @settingname)";

    using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
    {
        using (SqlCommand objCmd = new SqlCommand(upit, objConn))
        {
            try
            {
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.Parameters.AddWithValue("@settingname", SettingName);
                objConn.Open();
                SqlDataReader reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    pkcs12timeout = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting pkcs12timeout. " + ex.Message);
                throw new Exception("Error while getting pkcs12timeout. " + ex.Message);
            }
        }
    }

    return pkcs12timeout;
}

*/
