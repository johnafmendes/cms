/* CLASS NAME: App_Code/Users.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage User information in backend side.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Data.Odbc;

/// <summary>
/// Summary description for Users
/// </summary>
public class Users : System.Web.UI.Page
{
	//constructor, do nothing
    public Users()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    //Create a New user and based in ID generated, add modules that the user can access.
    public bool InsertUser(string firstname, string surname, string username, string email, string password, 
        string type, string typeAutentication, string datetime, string enabled)
    {
        General g = Session["app"] as General;
        bool result = false;

        string SQL = "INSERT INTO usertab "
            + "VALUES (NULL, '" + firstname + "', '" + surname + "', '" + username
            + "', '" + password + "', '"+ email +"', '" + type + "', '" + datetime + "', '"+typeAutentication
            +"', " + enabled + ")";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();

            SQL = "SELECT LAST_INSERT_ID() AS ID";
            cmd.CommandText = SQL;
            OdbcDataReader resultUser = cmd.ExecuteReader();

            if (resultUser.HasRows)
            {
                ID = resultUser["ID"].ToString();

                SQL = "INSERT INTO moduleusertab VALUES (1, " + ID + ", 1, 0, 0, 0)";
                OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (2, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (3, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (4, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (5, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (6, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (7, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (8, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();

                SQL = "INSERT INTO moduleusertab VALUES (9, " + ID + ", 1, 0, 0, 0)";
                cmd2 = g.DB.PrepareSQL(SQL);
                cmd2.ExecuteNonQuery();
            }

            result = true;
        }
        catch (OdbcException o)
        {
            //tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
            result = false;
        }
        finally
        {
            cmd.Dispose();
            /*if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;*/
        }

        return result;
    }

    //Update user data
    public bool UpdateUser(string ID, string firstname, string surname, string username, string email, 
        string password, string type, string enabled)
    {
        General g = Session["app"] as General;
        bool result = false;
        string SQL = "";

        if (password.Length > 0)
            SQL = "UPDATE usertab SET "
                + "name='" + firstname + "', surname='" + surname + "', username='" + username 
                + "', password='" + g.getMd5Hash(password) + "', enabled=" + enabled + ", type=" + type
                + ", email='" + email
                + "' WHERE iduser=" + ID;
        else
            SQL = "UPDATE usertab SET "
                + "name='" + firstname + "', surname='" + surname + "', username='" + username
                + "', enabled=" + enabled + ", type=" + type + ", email='" + email
                + "' WHERE iduser=" + ID;

                OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();
            result = true;
        }
        catch (OdbcException o)
        {
            result = false;
            //tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            /*if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;*/
        }

        return result;
    }

    //check if email exist in database before insert or update to avoid duplicate
    public bool eMailExist(string email)
    {
        bool result = false;
        General g = Session["app"] as General;

        string SQL = "SELECT email "
            + "FROM usertab "
            + "WHERE email='" + email + "'";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader resultEmail = cmd.ExecuteReader();

            if (resultEmail.HasRows) result = true;
            else result = false;
        }
        catch (OdbcException o)
        {
            result = false;
            //tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
        }

        return result;
    }

    //check if UserName exist before insert or update to avoid duplicate
    public bool UserNameExist(string username)
    {
        bool result = false;
        General g = Session["app"] as General;

        string SQL = "SELECT username "
            + "FROM usertab "
            + "WHERE username='" + username + "'";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader resultUsername = cmd.ExecuteReader();

            if (resultUsername.HasRows) result = true;
            else result = false;
        }
        catch (OdbcException o)
        {
            result = false;
        }
        finally
        {
            cmd.Dispose();
        }

        return result;
    }

}
