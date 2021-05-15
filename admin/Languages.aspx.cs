/* CLASS NAME: Admin/Languages.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage Languages area that allow create for each subject/topic a page with 
 * different language.
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

public partial class admin_Languages : System.Web.UI.Page
{
    //check if the system is ready to be accessed.
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
            //do verification if user can access or not this area
            if (!g.Auth.CheckPermission(g.Auth.idUser, 8, "SelectData")) Response.Redirect("ControlPanel.aspx");
        }
    }

    //if the user has passed by Page_PreInit, it mean that can reach these area/page_load
    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string action, id;
        //clean sqlInjection from action and id
        action = g.DB.cleanSQLInjection(Request.QueryString["action"]);
        id = g.DB.cleanSQLInjection(Request.QueryString["id"]);

        //pass string connection to datagridview
        SqlDataSource1.ConnectionString = g.DB.connectionString;
        //pass the provider to datagridview
        SqlDataSource1.ProviderName = g.DB.providerName;

        switch (action)
        {
            case "0": break; //do nothing
            case "1": selectLanguage(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newLanguage(sender, e); break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    //save language (update or insert a new)
    public void saveLanguage(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "", ID = "", Enabled = "0";

        if (chbEnabled.Checked) Enabled = "1";

        if (lblidLanguage.Text == "")
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 8, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            SQL = "INSERT INTO languagetab "
                + "VALUES (NULL, '" + txtTitle.Text + "', '" + txtAbreviation.Text + "', " 
                + ddlFlag.SelectedValue.ToString() + ", "+Enabled+")";
        }
        else
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 8, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            SQL = "UPDATE languagetab SET "
                + "title='" + txtTitle.Text + "', abreviation='" + txtAbreviation.Text 
                + "', idmedia=" + ddlFlag.SelectedValue.ToString() + ", enabled=" + Enabled
                + " WHERE idlanguage=" + lblidLanguage.Text;
            ID = lblidLanguage.Text;
        }

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();

            if (ID.Length == 0)
            {
                SQL = "SELECT LAST_INSERT_ID() AS ID";
                cmd.CommandText = SQL;
                OdbcDataReader result = cmd.ExecuteReader();

                if (result.HasRows) ID = result["ID"].ToString();
            }
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0)
            {
                selectLanguage(sender, e, ID);
                msg.InnerHtml = "Saved successfully!";
            }
            else msg.InnerHtml = tmsg;
        }
    }

    //release and clean the fields to prepare for new data 
    public void newLanguage(object sender, EventArgs e)
    {
        lblidLanguage.Text = "";
        txtTitle.Text = "";
        txtAbreviation.Text = "";
        populateMedia(sender, e, "");
    }

    //change the status of fields allowing or not editing
    private void statusEdit(object sender, EventArgs e, bool status)
    {
        txtTitle.Enabled = status;
        txtAbreviation.Enabled = status;
        ddlFlag.Enabled = status;
        chbEnabled.Enabled = status;
    }

    //Delete languages
    public void deleteLanguage(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 8, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM languagetab "
            + " WHERE idlanguage=" + id;

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
            if (tmsg.Length == 0) msg.InnerHtml = "Excluded successfully!";
            else msg.InnerHtml = tmsg;
        }
        listLanguages(sender, e);
        newLanguage(sender, e);
    }

    public void listLanguages(object sender, EventArgs e)
    {
        gvLanguage.DataBind();
    }

    public void selectLanguage(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "SELECT * "
            + "FROM languagetab "
            + "WHERE idlanguage=" + id;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidLanguage.Text = result["idlanguage"].ToString();
                txtTitle.Text = result["title"].ToString();
                txtAbreviation.Text = result["abreviation"].ToString();
                populateMedia(sender, e, result["idmedia"].ToString());
                if (result["enabled"].ToString() == "0") chbEnabled.Checked = false;
                else chbEnabled.Checked = true; 
            }
            else
            {
                msg.InnerHtml = "We are sorry, we are experiencing technical problems ...";
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
            cmd.Dispose();
            if (tmsg.Length == 0) msg.InnerHtml = "Selected successfully!";
            else msg.InnerHtml = tmsg;
        }

        statusEdit(sender, e, true);
        listLanguages(sender, e);
    }

    private void populateMedia(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM mediatab "
            + "WHERE type='Image'";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                ddlFlag.Items.Clear();
                while (result.Read())
                {
                    item++;
                    ddlFlag.Items.Add(new ListItem(result["filename"].ToString() + " - " + result["title"].ToString(), result["idmedia"].ToString()));
                    if (idSelected == result["idmedia"].ToString()) ddlFlag.SelectedIndex = item;
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

    protected void gvLanguage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;

            string state = rowView["idlanguage"].ToString();
            string type = Path.GetExtension(rowView["flag"].ToString());
            e.Row.Cells[3].Text = "<img src=\"..\\images\\" + rowView["flag"].ToString() + "\" alt=\"\" title=\"\">";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            e.Row.Attributes.Add("onclick", "javascript:window.location='Languages.aspx?action=1&id=" + state + "';");
        }
    }

    protected void gvLanguage_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteLanguage")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvLanguage.Rows[index];

            deleteLanguage(sender, e, row.Cells[0].Text);
        }
    }

}
