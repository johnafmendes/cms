/* CLASS NAME: App_Code/General.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is be the base class for access others from this one.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for General
/// </summary>
public partial class General : System.Web.UI.Page
{
    /*[Serializable()] */
    public Database DB;
    public Authentication Auth;
    public Styles Styles;
    public Users Users;
    public FriendlyURL FUrl;
    public Captcha Cpcha;

    public bool EnableWebSite = false;
    public static string _ErrorMSG = "", _idSubject, _idArticle, _idSubjectHeader, _idSubjectFooter, _idLogo, 
        _sTitle, _NumberNews, _LogoImage, _ErrorLevel = "Fully" /*Basic or Fully*/, _currentPage, _MenuStyle, 
        _Description, _Keywords, _idLanguage, _FolderInstalled, _MediaType, _idMediaGroup, _LDAPurl, _LDAPContext;

    public string ErrorMSG { set { _ErrorMSG = value; } get { return _ErrorMSG; } }
    public string idSubject { set { _idSubject = value; } get { return _idSubject; } }
    public string idArticle { set { _idArticle = value; } get { return _idArticle; } }
    public string idSubjectHeader { set { _idSubjectHeader = value; } get { return _idSubjectHeader; } }
    public string idSubjectFooter { set { _idSubjectFooter = value; } get { return _idSubjectFooter; } }
    public string idLogo { set { _idLogo = value; } get { return _idLogo; } }
    public string sTitle { set { _sTitle = value; } get { return _sTitle; } }
    public string NumberNews { set { _NumberNews = value; } get { return _NumberNews; } }
    public string LogoImage { set { _LogoImage = value; } get { return _LogoImage; } }
    public string ErrorLevel { set { _ErrorLevel = value; } get { return _ErrorLevel; } }
    public string currentPage { set { _currentPage = value; } get { return _currentPage; } }
    public string MenuStyle { set { _MenuStyle = value; } get { return _MenuStyle; } }
    public string Description { set { _Description = value; } get { return _Description; } }
    public string Keywords { set { _Keywords = value; } get { return _Keywords; } }
    public string idLanguage { set { _idLanguage = value; } get { return _idLanguage; } }
    public string FolderInstalled { set { _FolderInstalled = value; } get { return _FolderInstalled; } }
    public string MediaType { set { _MediaType = value; } get { return _MediaType; } }
    public string idMediaGroup { set { _idMediaGroup = value; } get { return _idMediaGroup; } }
    public string LDAPurl { set { _LDAPurl = value; } get { return _LDAPurl; } }
    public string LDAPContext { set { _LDAPContext = value; } get { return _LDAPContext; } }

    //constructor function
    public General()
    {
        string[] Config = retrieveConfig();
        if (Config[0] != "false")
        {            
            DB = new Database(Config[0], Config[2], Config[3], Config[4], Config[1], Config[5]);
            Auth = new Authentication();
            Styles = new Styles();
            Users = new Users();
            FUrl = new FriendlyURL();
            Cpcha = new Captcha();
            getConfigWebSite();
        }
        else { 
            EnableWebSite = false;
            sTitle = "John Mendes";
            LogoImage = "1.gif";
            MediaType = "Image";
            FolderInstalled = "";
        }
        //General g = Session["app"] as General;
    }

    //display the Maintenance message when a user set it on Settings.
    public string maintenance(){
        //General g = new General();// Session["app"] as General;

        string msg = "<center>";
        if (MediaType == "Image") msg += "<img src=\"" + ReturnProtocolAndPort() + "/images/" + LogoImage + "\" alt=\"\" title=\"\" />";
        if (MediaType == "Flash") msg += "<object width=\"100%\" height=\"100%\">"
            + "<param name=\"movie\" value=\"" + ReturnProtocolAndPort()+ LogoImage + "\">"
            + "<embed src=\"" + ReturnProtocolAndPort() + LogoImage + "\" width=\"100%\" height=\"100%\">"
            + "</embed>"
            + "</object>";
        msg += "<br/><br/><b>Sorry, we are under maintenance!! Come back later, thanks.</b></center>";
        //Response.Write(msg);
        //Response.End();
        return msg;
    }

    //return the javascript to time out.
    public string returnJavaScriptTimeOut()
    {
        return "<script type=\"text/javascript\">"
            + "DisplaySessionTimeout(" + Session.Timeout + ")"
            + "</script>";
    }

    //Retrieve configuration in config.txt on root folder of website.
    //to have information to access databases. This file must be protected agains public read.
    public string[] retrieveConfig() {
        int lines = 6;
        string[] Config = new string[lines];

        //string currentDirectory = System.IO.Directory.GetDirectoryRoot(".");// GetCurrentDirectory();
        string pathConfigFile = Server.MapPath("~") + "\\config.txt";

        if (File.Exists(pathConfigFile))
        {
            TextReader ConfigFile = new StreamReader(pathConfigFile);
            for (int i = 0; i < lines; i++)
            {
                Config[i] = ConfigFile.ReadLine();
            }
            ConfigFile.Close();
        }
        else {
            Config[0] = "false";
        }
        return Config;
    }

    //Do authentication based in domain, username and password
    public bool authAdmin(string domain, string username, string password)
    {
        /*try
        {
            if(Session["app"] == null) General g = new General();
            else General g = Session["app"] as General;*/

        username = DB.cleanSQLInjection(username);
        password = DB.cleanSQLInjection(password);
        domain = DB.cleanSQLInjection(domain);

        if(domain.Length > 0)
            if (Auth.ValidateUserByLDAP(domain, username, password))
                return true;
            else return false;
        else
            if (Auth.ValidateUser(username, password))
                return true;
            else return false;
    }

    //convert string in MD5 hash string - 32 characteres
    public string getMd5Hash(string input)
    {
        // Create a new instance of the MD5CryptoServiceProvider object.
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    // Verify a hash against a string.
    public bool verifyMd5Hash(string input, string hash)
    {
        // Hash the input.
        string hashOfInput = getMd5Hash(input);

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        if (0 == comparer.Compare(hashOfInput, hash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Access the table configtab and read all columns to populate all variables that are used in whole website
    public void getConfigWebSite()
    {
        //General g = Session["app"] as General;
        //General g = new General();
        string SQL = "SELECT ct.*, mt.type "
             + "FROM configtab as ct, mediatab as mt "
             + "WHERE ct.idlogo=mt.idmedia";

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
        //OdbcCommand cmd;
        //OdbcDataReader result;
        OdbcCommand cmd = DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();
            /*using (OdbcConnection connDB = new OdbcConnection(g.DB.connectionString))
            {                
                cmd = new OdbcCommand(SQL, connDB);
                connDB.Open();
                result = cmd.ExecuteReader();
            */
                if (result.HasRows)
                {
                    if (result["enabled"].ToString() == "1") EnableWebSite = true;
                    else EnableWebSite = false;
                    idSubject = result["idsubject"].ToString();
                    Description = result["description"].ToString();
                    Keywords = result["keywords"].ToString();
                    idSubjectHeader = result["idsubjectheader"].ToString();
                    idSubjectFooter = result["idsubjectfooter"].ToString();
                    idLogo = result["idlogo"].ToString();
                    idLanguage = result["idlanguage"].ToString();
                    NumberNews = result["numbernews"].ToString();
                    sTitle = result["title"].ToString();
                    FolderInstalled = result["folderinstalled"].ToString();
                    MenuStyle = result["menustyle"].ToString();
                    MediaType = result["type"].ToString();
                    LDAPurl = result["ldaphosturl"].ToString();
                    LDAPContext = result["ldapcontexts"].ToString();
                    if (idLogo.Length != 0)
                    {
                        SQL = "SELECT filename FROM mediatab WHERE idmedia=" + idLogo;

                        OdbcCommand cmd2 = DB.PrepareSQL(SQL);
                        //OdbcCommand cmd2 = new OdbcCommand(SQL, g.DB.connDB);

                        OdbcDataReader result2 = cmd2.ExecuteReader();

                        if (result2.HasRows) LogoImage = result2["filename"].ToString();

                        result2.Close();
                        result2.Dispose();
                        cmd2.Dispose();
                    }
                }
                result.Close();
                result.Dispose();
            //}
        }
        catch (OdbcException o)
        {
            ErrorMSG = DB.catchODBCException(o, ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
        }
    }

    //based in datetime format, create a datetime based in MySQL format
    public string MakeMySQLDateTime(string datetime)
    {
        string MySQL="";
        if (datetime == null) 
            MySQL = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
        else if (datetime.Length == 0)
            MySQL = "0000-00-00 00:00:00";
        else if (isDate(datetime))
        {
            Console.WriteLine(datetime.ToString());
            MySQL = datetime.Substring(6, 4) + "/";
            MySQL += datetime.Substring(3, 2) + "/";
            MySQL += datetime.Substring(0, 2) + " ";
            MySQL += datetime.Substring(11, 8);
            /*DateTime date1 = new DateTime(datetime);
            DateTimeOffset dateOffset = new DateTimeOffset(date1,TimeZoneInfo.Local.GetUtcOffset(date1));
            Console.WriteLine(date1.ToString("o"));
            // Displays 2008-04-10T06:30:00.0000000                        
            Console.WriteLine(dateOffset.ToString("o"));
            // Displays 2008-04-10T06:30:00.0000000-07:00  */
        }
        else 
        {
            return "false";
        }
        return MySQL;
    }

    //check if string is date or not
    public bool isDate(string strDate)
    {
        /*string strRegex = @"((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1- 9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))";

        Regex re = new Regex(strRegex);
        if (re.IsMatch(strDate))
            return true;
        else
            return false;*/

        string strRegexShort = @"^\d{1,2}\/\d{1,2}\/\d{2,4}$";
        string strRegexLong = @"^\d{1,2}\/\d{1,2}\/\d{2,4}\ \d{1,2}\:\d{1,2}\:\d{1,2}$";
        Regex reS = new Regex(strRegexShort);
        Regex reL = new Regex(strRegexLong);
        if (reS.IsMatch(strDate) || reL.IsMatch(strDate))
            return true;
        else
            return false;
    }

    //Fuction used to produce a exception based in Level
    public string catchException(Exception o, string Level)
    {
        string tmsg = "";

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

    /*public HtmlHead createMetaTags(string type, string content)
    {
        HtmlMeta hm = new HtmlMeta();

        HtmlHead head = (HtmlHead)Page.Header;
        hm.Name = type;
        hm.Content += content;
        head.Controls.Add(hm);
    }*/

    //check if the string is email or not (returning boolean as a answer)
    public bool isEmail(string inputEmail)
    {
        if( inputEmail == null || inputEmail.Length == 0 )
        {
            //throw new ArgumentNullException( "inputEmail" );
            return false;
        }

        const string expression = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        Regex regex = new Regex(expression);
        return regex.IsMatch(inputEmail);
    }

    //return the Protocol and Port to help fix FriendlyURL
    public string ReturnProtocolAndPort()
    {
        string Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"].ToString();
        if (Port == null || Port == "80" || Port == "443") Port = "";
        else Port = ":" + Port;

        string Protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"].ToString();
        if (Protocol == null || Protocol == "0") Protocol = "http://";
        else Protocol = "https://";

        Protocol = Protocol + HttpContext.Current.Request.ServerVariables["http_host"].ToString() + HttpContext.Current.Request.ApplicationPath.ToString();
        if (Protocol.EndsWith("/"))
            Protocol = Protocol.Remove(Protocol.Length - 1, 1);
        return Protocol;
    }

    
}
