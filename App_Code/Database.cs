/* CLASS NAME: Database.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 12/11/2008
 * The objective this class is control database connection and any other information about
 */

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Data;
using System.Data.Odbc;
using System.Text;

//using System.Data.OleDb;
/*using System.Data.Sql;
using System.Data.SqlClient;*/
//using System.Data.ProviderBase;

/// <summary>
/// Summary description for Database
/// </summary>
public class Database : System.Web.UI.Page
{
    /*private const string DB = "Driver={MySQL ODBC 3.51 Driver};" +
        "Server=localhost;Database=test;uid=root;pwd=123;option=3";*/

    public OdbcConnection connDB; //Database Connection via ODBC
    public string connectionString, //String to connect to database
        databaseName; //Name of Database
    public string providerName = "System.Data.Odbc"; //Name of Database provider.
    public bool tryPersistentConnection = true; //to identify if must try keep the connection persistent or not.

    //public OleDbConnection connDB;
    //public SqlConnection connDB;

    /*
     Constructor. Receiving ServerIP, DataBaseUser, Password, DatabaseName, Port and MySQLVersion to 
     * construct the ConnectionString and connect to database.
     */
    public Database(string server, string uid, string pwd, string databasename, string port, string MySQLVersion)
    {
        databaseName = databasename;
        /*Acrescentar o ANSI para versoes novas do ODBC, para o UOLHOST remover o ANSI e utilizar odbc 3.51*/
        //connectionString = "Driver={MySql ODBC 5.1 ANSI Driver}; Server=127.0.0.1; uid=root; pwd=123; database=johnmendescms; option=3; port=3306;"; /*wait_timeout=120;*/
        connectionString = "Driver={MySql ODBC " + MySQLVersion + " Driver}; Server=" + server + "; uid=" + uid + "; pwd=" + pwd + "; database=" + databasename + "; option=3; port=" + port + ";";
        connectDB();

        /*string NameConnectionString = "ConnectionString";

        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        // Remove the existing connectionstring.
        config.ConnectionStrings.ConnectionStrings.Remove(NameConnectionString);
        // Add the connectionstring
        ConnectionStringsSection csSection = config.ConnectionStrings;
        csSection.ConnectionStrings.Add(new ConnectionStringSettings(NameConnectionString, connectionString, "System.Data.Odbc"));

        // Save the configuration file
        config.Save(ConfigurationSaveMode.Full);*/
    }

    /*
     Connect to DB
     */
    public void connectDB()
    {
        /*
           //The connection keep opened only during the USING method
           //using (this.connDB = new OdbcConnection(connectionString))
        {
            this.connDB.Open();
              
           // The connection is automatically closed at 
           // the end of the Using block.
        }*/

        //the next two lines allow and keep connection opened during 
        //the life of this class.
        //this.connDB = new OleDbConnection(connectionString);
        //this.connDB = new SqlConnection(connectionString);

        //General g = Session["app"] as General;
        
        try
        {
            this.connDB = new OdbcConnection(connectionString);
            this.connDB.Open();
        }
        catch (OdbcException o)
        {
            General._ErrorMSG = catchODBCException(o, "Basic");
        }
    }

    //disconnect from DB
    public void disconnectDB()
    {
        this.connDB.Close();
        this.connDB.Dispose();
    }

    //retrieve the DB status
    public bool statusDB()
    {
        if (this.connDB.State.ToString() == "Open") return true;
        else return false;
    }

    //clean the SQL Injection (clean just single quote/apostrophes)
    public string cleanSQLInjection(string str)
    {
        str = str ?? "";
        return str.Replace("'", "''");
    }

    //Convert special chars to HTML tags
    public string HTMLEncode(string strValue)
    {
        strValue = cleanSQLInjection(strValue);
        strValue = strValue.Replace("'", "\x27"); //' JScript encode apostrophes
        strValue = strValue.Replace("\"\"", "\x22"); //' JScript encode double-quotes
        strValue = System.Web.HttpUtility.HtmlEncode(strValue); //' encode chars special to HTML

        /*char[] chars = HttpUtility.HtmlEncode(strValue).ToCharArray();
        StringBuilder result = new StringBuilder(strValue.Length + (int)(strValue.Length * 0.1));

        foreach (char c in chars)
        {
            int value = Convert.ToInt32(c);
            if (value > 127)
                result.AppendFormat("&#{0};", value);
            else
                result.Append(c);
        }

        return result.ToString();*/

        return strValue;
    }

    //Convert special chars to HTML tags
    public string HTMLDecode(string strValue)
    {
        strValue = strValue.Replace("\x27", "'"); //' JScript decode apostrophes
        strValue = strValue.Replace("\x22", "\"\""); //' JScript decode double-quotes
        strValue = System.Web.HttpUtility.HtmlDecode(strValue); //' decode chars special to HTML
        return strValue;
    }

    //Using SQL, prepare the command to be trigged.
    public OdbcCommand PrepareSQL(string SQL)
    {
        OdbcCommand cmd;
        if (!tryPersistentConnection)
        {
            connectDB();
            cmd = new OdbcCommand(SQL, connDB);
        }
        else cmd = new OdbcCommand(SQL, connDB);

        return cmd;
    }

    //Generic ODBC Exception used in all try/exception where needs show the error messages.
    public string catchODBCException(OdbcException o, string Level)
    {
        string tmsg="";

        switch (Level)
        {
            case "Basic": 
            {
                tmsg = "We are sorry, we are experiencing technical problems ...<br/><br/>";
                tmsg += "<hr/>";
                tmsg += "<br/>Message: " + o.Message;
                tmsg += "<br/>Others: " + o.InnerException;
                tmsg += "<hr/>";
                tmsg += "<br/><br/>Try again! If the problem persist, contact techinical suport";
                break;
            }
            case "Fully":
            {
                tmsg = "We are sorry, we are experiencing technical problems ...";
                tmsg += "<hr/>";
                tmsg += "<br/>Message: " + o.Message;
                tmsg += "<br/>Source: " + o.Source;
                tmsg += "<br/>Stack Trace: " + o.StackTrace;
                tmsg += "<br/>Target Site: " + o.TargetSite.Name;
                tmsg += "<br/>Others: " + o.InnerException;
                tmsg += "<hr/>";
                tmsg += "<br/>Try again! If the problem persist, contact techinical suport";
                break;
            }
            default: break;
        }

        return tmsg;
    }
}
