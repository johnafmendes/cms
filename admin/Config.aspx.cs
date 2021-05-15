/* CLASS NAME: Admin/Config.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage Settings of CMS.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_Config : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        if (g.Auth.Status == false)
        {
            Session.Abandon();
            Response.Redirect("Default.aspx");
        }
        else
        {
            /*
             1 - users
             * 2 - menus
             * 3 - media
             * 4 - articles
             * 5 - tags
             * 6 - settings
             * 7 - group of media
             * 8 - languages
             * 9 - group of menus
             * 10 - subject
             * Actions Allowed
             *  - SelectData
             *  - InsertData
             *  - UpdateData
             *  - DeleteData
             */
            if (!g.Auth.CheckPermission(g.Auth.idUser, 6, "SelectData")) Response.Redirect("ControlPanel.aspx");
        }

    }

    //every time when this page is loaded, this function call loadConfig function to retrieve the 
    //general settings of cms.
    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        
        string action;
        action = g.DB.cleanSQLInjection(Request.QueryString["action"]);

        switch (action)
        {
            case "0": loadConfig(sender, e); break; //consult
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    //this function do load of setting and populated the fields
    public void loadConfig(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        g.getConfigWebSite();
        txtTitle.Text = g.sTitle;
        txtFolderInstalled.Text = g.FolderInstalled;
        txtDescription.Text = g.Description;
        txtKeywords.Text = g.Keywords;
        if (g.EnableWebSite) chbEnabled.Checked = true;
        else chbEnabled.Checked = false;
        txtNumberNews.Text = g.NumberNews;
        txtLDAPUrl.Text = g.LDAPurl;
        txtLDAPContext.Text = g.LDAPContext;

        populateMenuStyle(sender, e, g.MenuStyle);
        populateMedia(sender, e, g.idLogo);
        populateLanguage(sender, e, g.idLanguage);
        populateSubject(sender, e, ddlSubject, g.idSubject);
        populateSubject(sender, e, ddlSubjectHeader, g.idSubjectHeader);
        populateSubject(sender, e, ddlSubjectFooter, g.idSubjectFooter);
    }

    //retrieve list of media and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private void populateMedia(object sender, EventArgs e, string idSelected)
    {
        string tmsg="", SQL = "SELECT * "
            + "FROM mediatab "
            + "WHERE type='Image' OR type='Flash'";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                ddlLogo.Items.Clear();
                while (result.Read())
                {
                    item++;
                    ddlLogo.Items.Add(new ListItem(result["filename"].ToString() + " - " + result["title"].ToString(), result["idmedia"].ToString()));
                    if (idSelected == result["idmedia"].ToString()) ddlLogo.SelectedIndex = item;
                }
            }
            result.Close();
            result.Dispose();
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
            cmd.Dispose();
        }
    }

    //retrieve list of Kind of Menu and populate into Drop Down List.
    private void populateMenuStyle(object sender, EventArgs e, string MenuSelected)
    {
        General g = Session["app"] as General;

        ddlMenuStyle.Items.Clear();
        ddlMenuStyle.Items.Add(new ListItem("Original", "0"));
        ddlMenuStyle.Items.Add(new ListItem("Tree", "1"));
        ddlMenuStyle.Items.Add(new ListItem("Top Bar", "2"));
        ddlMenuStyle.Items.Add(new ListItem("Side Bar", "3"));

        if (MenuSelected == "Original") ddlMenuStyle.SelectedIndex = 0;
        if (MenuSelected == "Tree") ddlMenuStyle.SelectedIndex = 1;
        if (MenuSelected == "Top Bar") ddlMenuStyle.SelectedIndex = 2;
        if (MenuSelected == "Side Bar") ddlMenuStyle.SelectedIndex = 3;
    }

    //retrieve list of Subject and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private void populateSubject(object sender, EventArgs e, DropDownList field, string idSelected)
    {
        int item = -1;
        string SQL="", tmsg="";

        General g = Session["app"] as General;

        SQL = "SELECT idsubject, title "
            + "FROM subjecttab ";
            //+ "WHERE enabled = 1";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                field.Items.Clear();
                field.Items.Add(new ListItem("Nothing", "NULL"));
                item++;
                while (result.Read())
                {
                    item++;
                    field.Items.Add(new ListItem(g.DB.HTMLDecode(result["title"].ToString()), result["idsubject"].ToString()));
                    if (idSelected == result["idsubject"].ToString()) field.SelectedIndex = item;
                }
            }
            result.Close();
            result.Dispose();
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
            cmd.Dispose();
        }
    }

    //retrieve list of Languages and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private void populateLanguage(object sender, EventArgs e, string idSelected)
    {
        int item = -1;
        string SQL = "", tmsg = "";

        General g = Session["app"] as General;

        SQL = "SELECT * "
            + "FROM languagetab ";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                ddlLanguage.Items.Clear();
                while (result.Read())
                {
                    item++;
                    ddlLanguage.Items.Add(new ListItem(g.DB.HTMLDecode(result["title"].ToString()), result["idlanguage"].ToString()));
                    if (idSelected == result["idlanguage"].ToString()) ddlLanguage.SelectedIndex = item;
                }
            }
            result.Close();
            result.Dispose();
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
            cmd.Dispose();
        }
    }

    //Save the settings.
    public void saveConfig(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 6, "UpdateData"))
        {
            msg.InnerHtml = "Update data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "", Enabled = "0", idSubject="NULL", idSubjectHeader="NULL", idSubjectFooter="NULL", NumberNews="0";

        if (chbEnabled.Checked) Enabled = "1";
        if (ddlSubject.SelectedValue.Length > 0) idSubject = ddlSubject.SelectedValue.ToString();
        if (ddlSubjectHeader.SelectedValue.Length > 0) idSubjectHeader = ddlSubjectHeader.SelectedValue.ToString();
        if (ddlSubjectFooter.SelectedValue.Length > 0) idSubjectFooter = ddlSubjectFooter.SelectedValue.ToString();
        if (txtNumberNews.Text != "") NumberNews = txtNumberNews.Text;

        SQL = "UPDATE configtab SET "
            + "title='" + txtTitle.Text + "', idlogo=" + ddlLogo.SelectedValue.ToString() + ", enabled=" + Enabled
            + ", numbernews=" + NumberNews + ", idsubject=" + idSubject
            + ", idsubjectheader="+idSubjectHeader+", idsubjectfooter=" + idSubjectFooter + ", folderinstalled='" 
            + txtFolderInstalled.Text + "', menustyle='" + ddlMenuStyle.SelectedItem.ToString() 
            + "', description='" + txtDescription.Text + "', keywords='" + txtKeywords.Text 
            + "', idlanguage=" + ddlLanguage.SelectedValue.ToString() + ", ldaphosturl='" + txtLDAPUrl.Text
            + "', ldapcontexts = '" + txtLDAPContext.Text + "'";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;
        }
    }
}
