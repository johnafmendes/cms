using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_Subject : System.Web.UI.Page
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
             * 9 - group of Menus
             * 10 - Subject
             * Actions Allowed
             *  - SelectData
             *  - InsertData
             *  - UpdateData
             *  - DeleteData
             */
            if (!g.Auth.CheckPermission(g.Auth.idUser, 10, "SelectData")) Response.Redirect("ControlPanel.aspx");
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
            case "0": break; //do nothing
            case "1": selectSubject(sender, e, id); break;
            //case "2": saveArticle(sender, e); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newSubject(sender, e); break;
        }
        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    public void newSubject(object sender, EventArgs e)
    {
        lblidSubject.Text = "";
        txtTitle.Text = "";
        txtDescription.Text = "";
        msg.InnerHtml = "";
        statusEdit(sender, e, true);
    }

    public void selectSubject(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "SELECT * "
            + "FROM subjecttab "
            + "WHERE idsubject=" + id.ToString();

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            //Create three columns with string as their type

            if (result.HasRows)
            {
                lblidSubject.Text = result["idsubject"].ToString();

                txtTitle.Text = g.DB.HTMLDecode(result["title"].ToString());

                txtDescription.Text = g.DB.HTMLDecode(result["description"].ToString());
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
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
        }
        listSubject(sender, e);
        statusEdit(sender, e, true);
    }

    public void saveSubject(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "", id = "";

        if (txtTitle.Text.Length == 0 || txtDescription.Text.Length == 0)
        {
            msg.InnerHtml = "You must type some information on Title and Description.";
            return;
        }

        if (lblidSubject.Text == "")
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 10, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            SQL = "INSERT INTO subjecttab "
                + "VALUES (NULL, '" + txtTitle.Text + "', '" + txtDescription.Text + "')";
        }
        else
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 10, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            SQL = "UPDATE subjecttab SET "
                + "title='" + txtTitle.Text + "', description='" + txtDescription.Text 
                + "' WHERE idsubject=" + lblidSubject.Text;
        }

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();
            if (lblidSubject.Text == "")
            {
                SQL = "SELECT LAST_INSERT_ID() AS ID";
                cmd.CommandText = SQL;
                OdbcDataReader result = cmd.ExecuteReader();

                if (result.HasRows) id = result["ID"].ToString();
            }
            else id = lblidSubject.Text;
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
                selectSubject(sender, e, id);
                msg.InnerHtml = "Saved successfully!";
            }
            else msg.InnerHtml = tmsg;
        }
    }

    public void listSubject(object sender, EventArgs e)
    {
        gvSubject.DataBind();
    }

    private void statusEdit(object sender, EventArgs e, bool status)
    {
        txtTitle.Enabled = status;
        txtDescription.Enabled = status;
    }

    public void deleteSubject(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 10, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM subjecttab "
            + " WHERE idsubject=" + id;

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
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
        listSubject(sender, e);
    }

    protected void gvSubject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        General g = Session["app"] as General;

        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            e.Row.Cells[1].Text = e.Row.Cells[1].Text;

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            DataRowView rowView = (DataRowView)e.Row.DataItem;

            String state = rowView["idsubject"].ToString();

            e.Row.Attributes.Add("onclick", "javascript:window.location='Subject.aspx?action=1&id=" + state + "';");
        }
    }

    protected void gvSubject_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteSubject")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvSubject.Rows[index];

            deleteSubject(sender, e, row.Cells[0].Text);
        }
    }
}
