/* CLASS NAME: Authentication.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 12/11/2008
 * The objective this class is control session, user and any other information about
 */

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Data;
using System.Data.Odbc;
using System.DirectoryServices;
//using System.Data.OleDb;
/*using System.Data.Sql;
using System.Data.SqlClient;*/

using System.Xml;
using System.Xml.XPath;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Summary description for Authentication
/// </summary>
public class Authentication : System.Web.UI.Page
{
    private static bool _Status = false; //Status of User (true - can access, false - cannot access backend)
    private static string _idUser = "0"; //ID of current user on backend side.

    public bool Status { set { _Status = value; } get { return _Status; } }
    public string idUser { set { _idUser = value; } get { return _idUser; } }

    //constructor - do nothing
    public Authentication() { }

    /*Given Domain, Username and Password, this method does authentication/validation of datas
    by LDAP - Active Directory.
     * */
    public bool ValidateUserByLDAP(string domain, string username, string password)
    {
        General g = Session["app"] as General;
        bool result = false;
        DirectoryEntry entry = new DirectoryEntry(g.LDAPurl + "/" + g.LDAPContext, domain + @"\" + username, password);

        try
        {
            // Bind to the native AdsObject to force authentication.

            Object obj = entry.NativeObject;
            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = "(SAMAccountName=" + username + ")";
            search.PropertiesToLoad.Add("cn");
            SearchResult resultUser = search.FindOne();

            if (null == resultUser) return false;//"Sorry, unable to verify your information";

            string SQL = "SELECT iduser, username, type "
                + "FROM usertab "
                + "WHERE username='" + username + "' "
                + "AND enabled = 1";

            OdbcCommand cmd = g.DB.PrepareSQL(SQL);

            try
            {
                OdbcDataReader resultUserDB = cmd.ExecuteReader();

                if (resultUserDB.HasRows)
                {
                    _idUser = resultUserDB["iduser"].ToString();
                    cmd.Dispose();
                    _Status = true;
                    result = true;
                }
                else
                {

                    DirectorySearcher dirSearcher = new DirectorySearcher(
                        new DirectoryEntry(g.LDAPurl + "/" + g.LDAPContext), "(&(objectClass=user)(anr=" + username + "))",
                        new string[] { "sAMAccountName", "displayname", "sn", "givenName", "mail" });

                    string FirstName = "First Name", LastName = "Last Name", eMail = "";

                    foreach (SearchResult s in dirSearcher.FindAll())
                    {
                        System.DirectoryServices.PropertyCollection p = s.GetDirectoryEntry().Properties;

                        if (p["givenName"].Value != null) FirstName = (string)p["givenName"].Value;
                        if (p["sn"].Value != null) LastName = (string)p["sn"].Value;
                        if (p["mail"].Value != null) eMail = (string)p["mail"].Value;
                    }

                    if (g.Users.eMailExist(eMail))
                    {
                        g.ErrorMSG = "e-Mail already exist! ";
                        return false;
                    }
                    else
                    {
                        int i=1;
                        while (g.Users.UserNameExist(username)) i++;
                        if (i > 1) username += i.ToString(); 
                    }

                    if (g.Users.InsertUser(FirstName, LastName, username, eMail, g.getMd5Hash(password),
                        "Standard", "LDAP", g.MakeMySQLDateTime(null), "1"))
                    {
                        result = true;
                        _Status = true;
                    }
                    else result = false;
                }

                resultUserDB.Close();
                resultUserDB.Dispose();
            }
            catch (OdbcException o)
            {
                result = false;
                g.ErrorMSG = g.DB.catchODBCException(o, g.ErrorLevel);
            }
            finally
            {
                cmd.Dispose();
            }
        }
        catch (Exception ex)
        {
            result = false;//"Sorry, unable to verify your information";
        }
        return result;
    }


    /* Given Username and Password, this method does authentication/validation of datas
       locally.
     */
    public bool ValidateUser(string userName, string passWord)
    {
        General g = Session["app"] as General;
        
        string pwdMD5 = g.getMd5Hash(passWord);
        string SQL = "SELECT iduser, username, password, type "
            + "FROM usertab "
            + "WHERE username='" + userName + "' "
            //+"WHERE username=@userName "
            + "AND password = '" + pwdMD5 + "' "
            + "AND enabled = 1 "
            + "AND typeauthentication='Local'";

        /*OleDbCommand cmd = new OleDbCommand(SQL, conDB);
        OleDbDataReader Dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);*/
        /*SqlCommand cmd = new SqlCommand(SQL, conDB);
        SqlDataReader Dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);*/
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        /*cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@userName", OdbcType.VarChar, 20);
        cmd.Parameters["@userName"].Value = userName;*/

        /*prm = new OdbcParameter("@username", OdbcType.VarChar, 150);
        prm.Direction = ParameterDirection.Input;
        prm.Value = userName;
        cmd.Parameters.Add(prm);*/
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                if (g.verifyMd5Hash(passWord, pwdMD5))
                {
                    _idUser = result["iduser"].ToString();
                    result.Close();
                    result.Dispose();
                    cmd.Dispose();
                    _Status = true;
                    return true;
                }
            }

            result.Close();
            result.Dispose();
        }
        catch (OdbcException o)
        {
            g.ErrorMSG = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
        }
        return false;
    }

    /*
     * Check Permission wether user can Read/Access, Update/Amend, Insert/Add or Delete/Exclude
     * data from specifc module.
     */
    public bool CheckPermission(string idUser, int idModule, string Action)
    { 
        bool result=false;

        General g = Session["app"] as General;

        string SQL = "SELECT * "
            + "FROM moduleusertab "
            + "WHERE iduser=" + idUser
            + " AND idmodule = " + idModule
            + " AND " + Action + " = 1";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader resultData = cmd.ExecuteReader();

            if (resultData.HasRows)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            resultData.Close();
            resultData.Dispose();
        }
        catch (OdbcException o)
        {
            g.ErrorMSG = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
        }

        return result;
    }
}
