/* CLASS NAME: Admin/admin.master.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is be base for backend, giving or not authorization to access others modules
 * based to iduser.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

public partial class admin_admin : System.Web.UI.MasterPage
{
    public admin_admin()
    {
        /*General g = Session["app"] as General;

        if (g.Auth.GetStatus == false)
        {
            Session.Abandon();
            Response.Redirect("Default.aspx");
        }*/
    }

    //retrieve the config settings to and decide if can go ahead or send the user to installation area
    // and enable or disable modules that user has access to.
    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string pathConfigFile = Server.MapPath("~") + "\\config.txt";

        //JavaScriptAtEndPage.InnerHtml = g.returnJavaScriptTimeOut();

        if (File.Exists(pathConfigFile))
        {
            g.getConfigWebSite();
            Page.Title = g.sTitle;
            logo.InnerHtml = "<img src=\"../images/" + g.LogoImage + "\" alt=\"\" title=\"\" />";
        }
        else {
            Response.Redirect("../installation");
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), "onLoad", "DisplaySessionTimeout("+Session.Timeout+")", true);

        if (!g.Auth.CheckPermission(g.Auth.idUser, 1, "SelectData"))
        {
            try { linkUsers.Visible = false; }
            catch (SystemException se) { }
            finally { } //Users
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 2, "SelectData"))
        {
            try { linkMenu.Visible = false; }
            catch (SystemException se) { }
            finally { }//Menus
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 5, "SelectData"))
        {
            try { linkTags.Visible = false; }
            catch (SystemException se) { }
            finally { }//Tags
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "SelectData"))
        {
            try { linkGroupMedia.Visible = false; }
            catch (SystemException se) { }
            finally { }//Group of Media
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 8, "SelectData"))
        {
            try { linkLanguages.Visible = false; }
            catch (SystemException se) { }
            finally { }//Languages
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 4, "SelectData"))
        {
            try { linkArticles.Visible = false; }
            catch (SystemException se) { }
            finally { }//Articles
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 3, "SelectData"))
        {
            try { linkMedia.Visible = false; }
            catch (SystemException se) { }
            finally { }//Media
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 6, "SelectData"))
        {
            try { linkSettings.Visible = false; }
            catch (SystemException se) { }
            finally { }//Settings
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 9, "SelectData"))
        {
            try { linkGroupMenu.Visible = false; }
            catch (SystemException se) { }
            finally { }//Group of Menus
        }
        if (!g.Auth.CheckPermission(g.Auth.idUser, 10, "SelectData"))
        {
            try { linkSubject.Visible = false; }
            catch (SystemException se) { }
            finally { }//Subject
        }

    }

    //Do logout 
    public void LogoutAdmin(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        g.Auth.Status = false;
        Response.Redirect("Default.aspx");
    }

}
