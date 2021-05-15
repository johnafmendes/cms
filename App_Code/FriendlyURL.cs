/* CLASS NAME: FriendlyURL.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 14/09/2009
 * The objective of this class is manage (create, convert, store and delete) FriendlyURL
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

/// <summary>
/// Summary description for FriendlyURL
/// </summary>
public class FriendlyURL : System.Web.UI.Page
{
    //Constructor - do nothing
	public FriendlyURL() { }

    //Retrieve the Friendly URL by idArticle
    public string retrieveFriendlyURL(string idArticle)
    {
        General g;
        if (Session["app"] == null) g = new General();
        else g = Session["app"] as General;

        string url = "";

        string SQL = "SELECT friendlyurl "
            + "FROM friendlyurltab "
            + "WHERE idarticle=" + idArticle;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                url = result["friendlyurl"].ToString();
            }
        }
        catch (OdbcException o)
        {
            g.ErrorMSG = g.DB.catchODBCException(o, g.ErrorLevel);
            //url = "/No-Page-To-Display";
        }
        return url;
    }

    //Retrieve the Original URL passing FriendlyURL and idAction
    public string retrieveOriginalURL(string FriendlyURL, string idAction)
    {

        if (FriendlyURL.CompareTo("/") == 0)
        {
            return "Default.aspx";
        }

        General g = new General();

        string url = "";
        if(g.FolderInstalled.Length > 0)
            FriendlyURL = FriendlyURL.Replace("/" + g.FolderInstalled.ToLower(), "");
        //Response.Write(FriendlyURL);
        /*foreach (string[,] fURL in ReservedFURL)
        {
            if (fURL[0,0].ToString() == FriendlyURL) return fURL[0,1].ToString();
        }*/

        if (g.DB != null)
        {
            string SQL = "SELECT * "
             + "FROM friendlyurltab "
             + "WHERE friendlyurl='" + FriendlyURL + "'";

            OdbcCommand cmd = g.DB.PrepareSQL(SQL);

            try
            {
                OdbcDataReader result = cmd.ExecuteReader();
                if (result.HasRows)
                {
                    SQL = "SELECT a.* "
                        + "FROM articletab a "
                        + "WHERE a.idarticle=" + result["idarticle"].ToString();

                    //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
                    OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);
                    OdbcDataReader result2 = cmd2.ExecuteReader();

                    if (result2.HasRows)
                        if (g.FolderInstalled.Length > 0)
                            url = "/" + g.FolderInstalled;
                    url += "/Default.aspx?action=" + idAction + "&idA=" + result["idarticle"].ToString()
                        + "&idS=" + result2["idsubject"].ToString() + "&idL=" + result2["idlanguage"].ToString();

                    result2.Close();
                    result2.Dispose();
                    cmd2.Dispose();
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
        }
        return url;
    }

    //Update Friendly URL on database (I need fix it to update and not delete and insert)
    public void UpdateFriendlyURL(string idArticle, string idSubject, string TitleArticle)
    {
        General g = Session["app"] as General;
        try
        {
            /*string SQLFURLDelete = "DELETE FROM FriendlyURLTab "
                + "WHERE idArticle = " + idArticle;

            OdbcCommand cmdFURL = g.DB.PrepareSQL(SQLFURLDelete);

            cmdFURL.ExecuteNonQuery();

            string fURL = CreateFriendURL(idSubject, idArticle, TitleArticle);

            string SQLFURL = "INSERT INTO FriendlyURLTab VALUES (NULL, '" + idArticle + "', '" + fURL + "')";
            cmdFURL = g.DB.PrepareSQL(SQLFURL);
            cmdFURL.ExecuteNonQuery();
            cmdFURL.Dispose();*/

            string fURL = CreateFriendlyURL(idSubject, idArticle, TitleArticle);

            string SQL = "UPDATE friendlyurltab SET "
                + "friendlyurl='" + fURL
                + "' WHERE idarticle = " + idArticle;

            OdbcCommand cmd = g.DB.PrepareSQL(SQL);
            cmd = g.DB.PrepareSQL(SQL);
            int RowAffected = cmd.ExecuteNonQuery();
            if (RowAffected == 0)
            {
                SQL = "INSERT INTO friendlyurltab VALUES (NULL, '" + idArticle + "', '" + fURL + "')";
                cmd = g.DB.PrepareSQL(SQL);
                cmd.ExecuteNonQuery();            
            }
            cmd.Dispose();
        }
        catch (OdbcException o){}
        finally {}
    }

    //Update All FriendURL - it is necessary when there are some amend into Menu area.
    public void UpdateALLFriendlyURL()
    {
        General g = Session["app"] as General;
        try
        {
            string SQL = "SELECT * FROM articletab";

            OdbcCommand cmd = g.DB.PrepareSQL(SQL);
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    /*string SQLFURLDelete = "DELETE FROM FriendlyURLTab "
                        + "WHERE idArticle = " + result["idArticle"].ToString();
                    
                    OdbcCommand cmdFURL = g.DB.PrepareSQL(SQLFURLDelete);

                    cmdFURL.ExecuteNonQuery();
                    
                    string fURL = CreateFriendURL(result["idSubject"].ToString(), result["idArticle"].ToString(), 
                        result["Title"].ToString());
                    

                    string SQLFURL = "INSERT INTO FriendlyURLTab VALUES (NULL, '" + result["idArticle"].ToString() 
                        + "', '" + fURL + "')";

                    cmdFURL = g.DB.PrepareSQL(SQLFURL);
                    cmdFURL.ExecuteNonQuery();
                    cmdFURL.Dispose();

                    */

                    UpdateFriendlyURL(result["idarticle"].ToString(), result["idsubject"].ToString(),
                        result["title"].ToString());
                }
            }
            result.Close();
            result.Dispose();
            cmd.Dispose();
        }
        catch (OdbcException o) { }
        finally { }
    }

    //Create the Friendly URL given idSubject, idArticle ant Article Title.
    public string CreateFriendlyURL(string idSubject, string idArticle, string TitleArticle)
    {
        General g = Session["app"] as General;

        //+, /, ?, %, #, &, 

        string NewText="";
        do
        {
            NewText = TitleArticle;
            TitleArticle = TitleArticle.Replace("  ", " ");
        } while (TitleArticle != NewText);

        TitleArticle = TitleArticle.Replace("/", "");

        string fURL = (RetrieveMenuPath(idSubject, "")
            + "/" + TitleArticle + idArticle).Replace(" ", "-").Replace(".", "").Replace("?", "").Replace("%", "").Replace("#", "").Replace("&", "");
        if (!fURL.StartsWith("/")) fURL = "/" + fURL;

        NewText = "";
        do
        {
            NewText = fURL;
            fURL = fURL.Replace("--", "-");
        } while (fURL != NewText);

        return fURL;
    }


    /*public string CreateFriendlyURL(string idArticle)
    {
        General g = Session["app"] as General;

        string FURL = "", SQL = "SELECT * "
            + "FROM ArticleTab "
            + "WHERE idArticle=" + idArticle;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                string MenuPath = RetrieveMenuPath(result["idSubject"].ToString(), "");
            }
        }
        catch (Exception o)
        {
            g.ErrorMSG = g.catchException(o, g.ErrorLevel);
        }
        finally
        {
            //if (tmsg.Length != 0) msg.InnerHtml = tmsg;
        }

        return FURL;
    }*/

    //Retrieve the Path of Menu (menu and submenu)
    public string RetrieveMenuPath(string idSubject, string idMenu)
    {
        General g = Session["app"] as General;
        string MenuPath = "";
        string SQL = "SELECT * "
            + "FROM menutab ";
        if (idMenu.Length == 0) SQL += "WHERE idsubject=" + idSubject;
        else SQL += "WHERE idmenu=" + idMenu;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                MenuPath = result["title"].ToString();
                if (result["idmenuparent"].ToString().Length > 0)
                {
                    MenuPath = RetrieveMenuPath(result["idsubject"].ToString(), result["idmenuparent"].ToString()) + "/" + MenuPath;
                }
            }
        }
        catch (Exception o)
        {
            g.ErrorMSG = g.catchException(o, g.ErrorLevel);
        }
        finally
        {
            //if (tmsg.Length != 0) msg.InnerHtml = tmsg;
        }

        return MenuPath;
    }

    //
    public string RetrieveFullPathFile(Match m)
    {
        General g = Session["app"] as General;

        string relPath = g.ReturnProtocolAndPort();
        string s = m.Groups[1].Value.Replace("../", "/");
        return relPath + s;
    }

    public string RelativePathToAbsolutePath(string htmltext)
    {
        foreach (Match m in Regex.Matches(htmltext, "(<img.*?src=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
        {
            //Regex r = new Regex("src=\"(?<1>.*)\" ", RegexOptions.Multiline);
            Regex r = new Regex("<img .*?src=[\"']?([^'\">]+)[\"']?.*?>", RegexOptions.Multiline);
            string url = r.Replace(m.Value, new MatchEvaluator(RetrieveFullPathFile));

            htmltext = htmltext.Replace(m.Value, String.Format("{0}{1}{2}", m.Groups[1].Value, url, m.Groups[3].Value));
        }
        foreach (Match m in Regex.Matches(htmltext, "(<a.*?href=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
        {
            //Regex r = new Regex("src=\"(?<1>.*)\" ", RegexOptions.Multiline);
            Regex r = new Regex("<a .*?href=[\"']?([^'\">]+)[\"']?.*?>", RegexOptions.Multiline);
            string url = r.Replace(m.Value, new MatchEvaluator(RetrieveFullPathFile));

            htmltext = htmltext.Replace(m.Value, String.Format("{0}{1}{2}", m.Groups[1].Value, url, m.Groups[3].Value));
        }
        return htmltext;
    }

}
