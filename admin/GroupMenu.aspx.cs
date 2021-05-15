/* CLASS NAME: Admin/GroupMenu.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage Group of Menus area.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_GroupMenu : System.Web.UI.Page
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
             * Actions Allowed
             *  - SelectData
             *  - InsertData
             *  - UpdateData
             *  - DeleteData
             */
            if (!g.Auth.CheckPermission(g.Auth.idUser, 9, "SelectData")) Response.Redirect("ControlPanel.aspx");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string action, id;

        action = g.DB.cleanSQLInjection(Request.QueryString["action"]);
        id = g.DB.cleanSQLInjection(Request.QueryString["id"]);

        SqlDataSource1.ConnectionString = g.DB.connectionString;
        SqlDataSource1.ProviderName = g.DB.providerName;

        switch (action)
        {
            case "0": break; //consult
            case "1": selectGroupMenu(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newGroupMenu(sender, e); break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    //save Group of Menus
    public void saveGroupMenu(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            msg.InnerHtml = "";
            return;
        }

        General g = Session["app"] as General;

        string tmsg = "", SQL = "", Enabled = "0", ID = "";

        if (chbEnabled.Checked) Enabled = "1";

        if (lblidGroupMenu.Text == "")
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 9, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            SQL = "INSERT INTO menugrouptab "
                + "VALUES (NULL, '" + txtTitle.Text + "', " + ddlLanguage.SelectedValue.ToString() 
                + ", " + Enabled + ")";
        }
        else
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 9, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            SQL = "UPDATE menugrouptab SET "
                + "title='" + txtTitle.Text + "', idlanguage=" + ddlLanguage.SelectedValue.ToString()
                + ", enabled=" + Enabled
                + " WHERE idgroupmenu=" + lblidGroupMenu.Text;
        }

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();

            SQL = "SELECT LAST_INSERT_ID() AS ID";
            cmd.CommandText = SQL;
            //cmd = g.DB.PrepareSQL(SQL);
            OdbcDataReader result = cmd.ExecuteReader();
            if (result.HasRows) ID = result["ID"].ToString();
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            selectGroupMenu(sender, e, ID);
            listGroupMenu(sender, e);
            if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;
        }
    }

    //release and clean the fields to prepare for new data 
    public void newGroupMenu(object sender, EventArgs e)
    {
        populateLanguages(sender, e, "0");
        lblidGroupMenu.Text = "";
        txtTitle.Text = "";
        chbEnabled.Checked = false;
        statusEdit(sender, e, true);
    }

    //change the status of fields allowing or not editing
    private void statusEdit(object sender, EventArgs e, bool status)
    {
        ddlLanguage.Enabled = status;
        txtTitle.Enabled = status;
        ddlLanguage.Enabled = status;
        chbEnabled.Enabled = status;
    }

    //delete Group of Menu by ID
    public void deleteGroupMenu(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 9, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM menugrouptab "
            + "WHERE idgroupmenu=" + id;

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
        listGroupMenu(sender, e);
        newGroupMenu(sender, e);
    }

    //retrieve/refresh a list Group of Menu in datagridview
    public void listGroupMenu(object sender, EventArgs e)
    {
        gvGroupMenu.DataBind();
    }

    //retrieve list of languages and populate into Drop Down List.
    // this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    // place. It mean that from maitenance point of view it is not good
    private void populateLanguages(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM languagetab ";
        int item = -1;

        General g = Session["app"] as General;

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
                    ddlLanguage.Items.Add(new ListItem(result["title"].ToString() + " - " + result["abreviation"].ToString(), result["idLanguage"].ToString()));
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

    //Select group of menu by id
    public void selectGroupMenu(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string SQL = "SELECT * "
            + "FROM menugrouptab "
            + "WHERE idgroupmenu=" + id;
        string tmsg = "";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidGroupMenu.Text = id;
                populateLanguages(sender, e, result["idlanguage"].ToString());
                txtTitle.Text = result["title"].ToString();
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
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
            else msg.InnerHtml = tmsg;
            cmd.Dispose();
        }
        statusEdit(sender, e, true);
    }

    //for each row created in datagridview is added the attributes onMouseOver, onMouseOut
    // and onclick that allow user click on the row to open the Group of Menu of that row.
    protected void gvGroupMenu_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;

            String state = rowView["idgroupmenu"].ToString();

            //if (e.Row.Cells[3].Text == "1") e.Row.Cells[3].Text = "True"; else e.Row.Cells[3].Text = "False";
            //if (e.Row.Cells[5].Text == "1") e.Row.Cells[5].Text = "True"; else e.Row.Cells[5].Text = "False";

            //if (rowView["SubMenu"].ToString().Length == 0) e.Row.Cells[2].Text = "Main menu";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            e.Row.Attributes.Add("onclick", "javascript:window.location='GroupMenu.aspx?action=1&id=" + state + "';");
        }
    }

    //Each click in dataGridView if checked if was clicked in column called Delete. If yes, send the user to 
    //deleteGroupMenu function.
    protected void gvGroupMenu_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteGroupMenu")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvGroupMenu.Rows[index];

            deleteGroupMenu(sender, e, row.Cells[0].Text);
        }
    }
}
