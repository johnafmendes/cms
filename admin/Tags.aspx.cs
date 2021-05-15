using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_Tags : System.Web.UI.Page
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
             * Actions Allowed
             *  - SelectData
             *  - InsertData
             *  - UpdateData
             *  - DeleteData
             */
            if (!g.Auth.CheckPermission(g.Auth.idUser, 5, "SelectData")) Response.Redirect("ControlPanel.aspx");
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
            case "1": selectTag(sender, e, id); break;
            //case "2": deleteTag(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newTag(sender, e); break;
        }
        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    public void newTag(object sender, EventArgs e)
    {
        lblidTag.Text = "";
        txtTag.Text = "";
        cbEnabled.Checked = false;
        statusEdit(sender, e, true);
    }

    private void statusEdit(object sender, EventArgs e, bool status)
    {
        txtTag.Enabled = status;
        cbEnabled.Enabled = status;
    }

    public void saveTag(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            msg.InnerHtml = "";
            return;
        }

        General g = Session["app"] as General;

        string tmsg = "", SQL = "", Enabled = "0";

        if (cbEnabled.Checked) Enabled = "1";

        //OdbcTransaction myTransaction = null;
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            //myTransaction = g.DB.connDB.BeginTransaction();

            //cmd.Transaction = myTransaction;           

            if (lblidTag.Text == "")
            {
                if (!g.Auth.CheckPermission(g.Auth.idUser, 5, "InsertData"))
                {
                    msg.InnerHtml = "Insert new data is not allowed.";
                    return;
                }

                SQL = "INSERT INTO tagtab "
                    + "VALUES (NULL, '" + txtTag.Text + "', '" + Enabled + "')";
            }
            else
            {
                if (!g.Auth.CheckPermission(g.Auth.idUser, 5, "UpdateData"))
                {
                    msg.InnerHtml = "Update data is not allowed.";
                    return;
                }

                SQL = "UPDATE tagtab SET "
                    + "tag='" + txtTag.Text + "', enabled=" + Enabled
                    + " WHERE idtag=" + lblidTag.Text;
            }
            cmd.CommandText = SQL;
            cmd.ExecuteNonQuery();
            //myTransaction.Commit();
        }
        catch (OdbcException o)
        {
            //myTransaction.Rollback();
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;
            listTags(sender, e);
        }
    }

    public void deleteTag(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 5, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM tagtab "
            + " WHERE idtag=" + id;

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
        listTags(sender, e);
        newTag(sender, e);
    }

    public void selectTag(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string tmsg="", SQL = "SELECT * "
            + "FROM tagtab "
            + "WHERE idtag=" + id.ToString();

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidTag.Text = result["idtag"].ToString();
                txtTag.Enabled = true;
                txtTag.Text = result["tag"].ToString();
                cbEnabled.Enabled = true;
                if (result["enabled"].ToString() == "1") cbEnabled.Checked = true; else cbEnabled.Checked = false;
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
    }

    public void listTags(object sender, EventArgs e)
    {
        gvTag.DataBind();
    }

    protected void gvTag_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            if (e.Row.Cells[2].Text == "1") e.Row.Cells[2].Text = "True"; else e.Row.Cells[2].Text = "False";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            DataRowView rowView = (DataRowView)e.Row.DataItem;

            String state = rowView["idtag"].ToString();

            e.Row.Attributes.Add("onclick", "javascript:window.location='Tags.aspx?action=1&id=" + state + "';");
        }
    }

    protected void gvTag_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteTag")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvTag.Rows[index];

            deleteTag(sender, e, row.Cells[0].Text);
        }
    }
}
