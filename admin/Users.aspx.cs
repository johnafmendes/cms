using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;


//using System.Data.SqlClient;
//using System.Configuration;
//using System.Collections;
//using System.Web.Security;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;


public partial class admin_Users : System.Web.UI.Page
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
            if (!g.Auth.CheckPermission(g.Auth.idUser, 1, "SelectData")) Response.Redirect("ControlPanel.aspx");
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
        SqlDataSource2.ConnectionString = g.DB.connectionString;
        SqlDataSource2.ProviderName = g.DB.providerName;

        switch (action)
        {
            case "0": break; //do nothing
            case "1": selectUser(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newUser(sender, e); break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    public void saveUser(object sender, EventArgs e)
    {
        if (lblidUser.Text.Length == 0 || txtPassword.Text.Length > 0)
            if (!Page.IsValid)
            {
                msg.InnerHtml = "";
                if (!vldPassword.IsValid) msg.InnerHtml += "- Password not is match!";
                if (!vldPasswordContent.IsValid) msg.InnerHtml += "<br/>- Password must contain at least 8 character!";
                return;
            }

        General g = Session["app"] as General;

        bool result = false;
        string Type="0";
        string ID="";

        if (rbAdmin.Checked) Type = "1";
        if (rbStandard.Checked) Type = "0";

        if (lblidUser.Text.Length == 0)
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 1, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            if (!g.isEmail(txtEmail.Text))
            {
                msg.InnerHtml = "e-Mail is not valid!";
                return;
            }
            else
                if (g.Users.eMailExist(txtEmail.Text))
                {
                    msg.InnerHtml = "e-Mail already exist! Have you already registred before?";
                    return;
                }
                else
                    if (!g.Users.UserNameExist(txtUsername.Text))
                    {
                        msg.InnerHtml = "Username already exist! Have you already registred before?";
                        return;
                    }
            
            if (g.Users.InsertUser(txtFirstName.Text, txtSurname.Text, txtUsername.Text, txtEmail.Text, 
                g.getMd5Hash(txtPassword.Text), Type, rbTypeAuthe.SelectedValue.ToString(), 
                g.MakeMySQLDateTime(null), cbEnabled.Checked.ToString()))
                result = true;
            else result = false;
        }
        else {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 1, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            if (g.Users.UpdateUser(lblidUser.Text, txtFirstName.Text, txtSurname.Text, txtUsername.Text,
                    txtEmail.Text, txtPassword.Text, Type, cbEnabled.Checked.ToString()))
                result = true;
            else result = false;

            ID = lblidUser.Text;
        }

        if (result) msg.InnerHtml = "Saved successfully!";
        else msg.InnerHtml = "Cannot Save!";

        selectUser(sender, e, ID);
    }

    public void newUser(object sender, EventArgs e)
    {
        lblidUser.Text = "";
        txtFirstName.Text = "";
        txtSurname.Text = "";
        txtUsername.Text = "";
        txtPassword.Text = "";
        txtPassword2.Text = "";
        txtEmail.Text = "";
        rbTypeAuthe.ClearSelection();
        statusEdit(sender, e, true);
        SqlDataSource2.SelectParameters["iduser"].DefaultValue = "0";
    }

    private void statusEdit(object sender, EventArgs e, bool status)
    {
        txtFirstName.Enabled = status;
        txtSurname.Enabled = status;
        txtUsername.Enabled = status;
        txtEmail.Enabled = status;
        txtPassword.Enabled = status;
        txtPassword2.Enabled = status;
        cbEnabled.Enabled = status;
        rbAdmin.Enabled = status;
        rbStandard.Enabled = status;
        rbTypeAuthe.Enabled = status;
    }

    public void deleteUser(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 1, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM moduleusertab "
            + " WHERE iduser=" + id;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();

            SQL = "DELETE FROM usertab "
                + " WHERE iduser=" + id;

            cmd = g.DB.PrepareSQL(SQL);
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
        listUser(sender, e);
        newUser(sender, e);
    }

    public void listUser(object sender, EventArgs e)
    {
        gvUser.DataBind();
    }

    public void selectUser(object sender, EventArgs e, string idUser)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "SELECT *, if (enabled, 'true', 'false') as enabled "
            + "FROM usertab "
            + "WHERE iduser=" + idUser.ToString();

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidUser.Text = result["iduser"].ToString();
                txtFirstName.Text = result["name"].ToString();
                txtSurname.Text = result["surname"].ToString();
                txtUsername.Text = result["username"].ToString();
                txtEmail.Text = result["email"].ToString();
                if(result["typeauthentication"].ToString() == "LDAP") rbTypeAuthe.Items[0].Selected = true;
                else rbTypeAuthe.Items[1].Selected = true;
                txtPassword.Text = "";
                txtPassword2.Text = "";                
                cbEnabled.Checked = Convert.ToBoolean(result["enabled"]);
                //if (result["Enabled"].ToString() == "1") rbActive.Checked = true;
                //if (result["Enabled"].ToString() == "0") rbInactive.Checked = true;
                if (result["type"].ToString() == "Admin") rbAdmin.Checked = true;
                if (result["type"].ToString() == "Standard") rbStandard.Checked = true;
                SqlDataSource2.SelectParameters["iduser"].DefaultValue = idUser.ToString();
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
        
        statusEdit(sender, e, true);
        txtEmail.Enabled = false;
        txtUsername.Enabled = false;
        listUser(sender, e);
    }

    protected void gvUser_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            //if (e.Row.Cells[5].Text == "1") e.Row.Cells[5].Text = "True"; else e.Row.Cells[5].Text = "False";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            DataRowView rowView = (DataRowView)e.Row.DataItem;
 
            String state = rowView["iduser"].ToString();

            e.Row.Attributes.Add("onclick", "javascript:window.location='Users.aspx?action=1&id=" + state + "';");
        }
    }

    protected void gvUser_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteUser")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvUser.Rows[index];

            deleteUser(sender, e, row.Cells[0].Text);
        }
    }

    protected void gvPermissions_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        /*int index = Convert.ToInt32(e.CommandArgument);
        GridViewRow row = gvPermissions.Rows[index];

        if (e.CommandName == "Edit")
        {
            ((CheckBox)gvPermissions.Rows[index].Cells[3].FindControl("SelectData")).Checked = true;
            ((CheckBox)gvPermissions.Rows[index].FindControl("SelectData")).Enabled = false;

            ((CheckBox)gvPermissions.Rows[index].FindControl("SelectData")).Enabled = false;
            ((CheckBox)gvPermissions.Rows[index].FindControl("UpdateData")).Enabled = true;
            ((CheckBox)gvPermissions.Rows[index].FindControl("InsertData")).Enabled = true;
            ((CheckBox)gvPermissions.Rows[index].FindControl("DeleteData")).Enabled = true;
        }

        if (e.CommandName == "Update")
        {
            ((CheckBox)gvPermissions.Rows[index].FindControl("SelectData")).Enabled = false;
            ((CheckBox)gvPermissions.Rows[index].FindControl("UpdateData")).Enabled = false;
            ((CheckBox)gvPermissions.Rows[index].FindControl("InsertData")).Enabled = false;
            ((CheckBox)gvPermissions.Rows[index].FindControl("DeleteData")).Enabled = false;
        }*/
    }

    protected void gvPermissions_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        General g = Session["app"] as General;

        CheckBox cbSelect = (CheckBox)gvPermissions.Rows[e.RowIndex].FindControl("SelectData");
        CheckBox cbUpdate = (CheckBox)gvPermissions.Rows[e.RowIndex].FindControl("UpdateData");
        CheckBox cbInsert = (CheckBox)gvPermissions.Rows[e.RowIndex].FindControl("InsertData");
        CheckBox cbDelete = (CheckBox)gvPermissions.Rows[e.RowIndex].FindControl("DeleteData");

        /*string abc = gvPermissions.Rows[e.RowIndex].Cells[1].Text;*/

        string tmsg = "", SQL = "UPDATE moduleusertab "
            + "SET selectdata=" + cbSelect.Checked + ", insertdata=" + cbInsert.Checked + ", updatedata="
            + cbUpdate.Checked + ", deletedata=" + cbDelete.Checked
            + " WHERE idmodule=" + gvPermissions.Rows[e.RowIndex].Cells[0].Text + " AND iduser=" + lblidUser.Text;

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
            if (tmsg.Length == 0) msg.InnerHtml = "Updated successfully!";
            else msg.InnerHtml = tmsg;
        }
        gvPermissions.EditIndex = -1;
        gvPermissions.DataBind();
    }
    protected void gvPermissions_RowEditing(object sender, GridViewEditEventArgs e)
    {
        /*int index = Convert.ToInt32(e.NewEditIndex);

        ((CheckBox)gvPermissions.Rows[index].Cells[3].FindControl("SelectData")).Checked = true;
        ((CheckBox)gvPermissions.Rows[index].FindControl("SelectData")).Enabled = false;
        ((CheckBox)gvPermissions.Rows[index].FindControl("UpdateData")).Enabled = true;
        ((CheckBox)gvPermissions.Rows[index].FindControl("InsertData")).Enabled = true;
        ((CheckBox)gvPermissions.Rows[index].FindControl("DeleteData")).Enabled = true;*/
    }
    protected void gvPermissions_SelectedIndexChanged(object sender, EventArgs e)
    {
        //((CheckBox)gvPermissions.Rows[gvPermissions.SelectedIndex].Cells[3].FindControl("SelectData")).Checked = true;
        //((CheckBox)gvPermissions.Rows[gvPermissions.SelectedIndex].FindControl("SelectData")).Enabled = false;
    }
}