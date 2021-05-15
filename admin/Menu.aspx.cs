using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_Menu : System.Web.UI.Page
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
            if (!g.Auth.CheckPermission(g.Auth.idUser, 2, "SelectData")) Response.Redirect("ControlPanel.aspx");
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
            case "1": selectMenu(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newMenu(sender, e); break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    public int fillMenuDrop(object sender, EventArgs e, string idMenuSelected, string subMenu, int Item, int nLevel, string idGroupMenu)
    {
        General g = Session["app"] as General;

        //int item=-1;
        string tmsg="", sIdent="";
        string SQL = "SELECT * "
            + "FROM menutab ";
            if (subMenu.Length == 0) SQL += "WHERE idmenuparent is NULL AND idgroupmenu=" + idGroupMenu;
            else SQL += "WHERE idmenuparent =" + subMenu + " AND idgroupmenu=" + idGroupMenu;
                

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();
        if (subMenu.Length == 0)
        {
            ddlMenu.Items.Clear();
            ddlMenu.Items.Add(new ListItem("Main menu", "0"));
            Item++;
            nLevel = 0;
        }
        else nLevel++;

        if (result.HasRows)
        {
            try
            {                
                while (result.Read())
                {                    
                    Item++;
                    if (subMenu.Length > 0)
                    {
                        sIdent = "|__";
                        string desloc = "";
                        for (int i = 0; i < (nLevel*2); i++) desloc += ". ";
                        sIdent = desloc + sIdent;
                    }
                    else sIdent = "";
                    ddlMenu.Items.Add(new ListItem(sIdent + result["title"].ToString(), result["idmenu"].ToString()));
                    if (idMenuSelected == result["idmenu"].ToString()) ddlMenu.SelectedIndex = Item;
                    Item = fillMenuDrop(sender, e, idMenuSelected, result["idmenu"].ToString(), Item, nLevel, idGroupMenu);
                }
            }
            catch (Exception o)
            {
                tmsg = g.catchException(o, g.ErrorLevel);
            }
            finally
            {
                msg.InnerHtml = tmsg;
            }
        }
        else
        {
            msg.InnerHtml = "No menu to show ...";
        }
        result.Close();
        result.Dispose();
        cmd.Dispose();
        return Item;
    }

    private void populateGroupMenu(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM menugrouptab ";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                ddlGroupMenu.Items.Clear();
                while (result.Read())
                {
                    item++;
                    ddlGroupMenu.Items.Add(new ListItem(result["title"].ToString(), result["idgroupmenu"].ToString()));
                    if (idSelected == result["idgroupmenu"].ToString()) ddlGroupMenu.SelectedIndex = item;
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

    private void populateSubject(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM subjecttab ";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                ddlSubject.Items.Clear();
                while (result.Read())
                {
                    item++;
                    ddlSubject.Items.Add(new ListItem(result["title"].ToString(), result["idsubject"].ToString()));
                    if (idSelected == result["idsubject"].ToString()) ddlSubject.SelectedIndex = item;
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

    public void saveMenu(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            msg.InnerHtml = "";
            return;
        }

        
        General g = Session["app"] as General;

        string tmsg = "", SQL = "", MainMenu="NULL", Enabled="0", ID="", Visible="0";

        if (ddlMenu.SelectedValue.ToString() != "0") MainMenu = ddlMenu.SelectedValue.ToString();

        if (MainMenu == lblidMenu.Text)
        {
            msg.InnerHtml = "Impossible Update. The Int Menu and Menu are the same. <br/><br/>The ID of dependent menu is the same of main ID!";
            return;
        }

        if (chbEnabled.Checked) Enabled = "1";
        if (chbVisible.Checked) Visible = "1";

        if (lblidMenu.Text == "")
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 2, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            SQL = "INSERT INTO menutab "
                + "VALUES (NULL, " + ddlGroupMenu.SelectedValue.ToString() + ", " + ddlSubject.SelectedValue.ToString()
                + ", '" + txtMenuName.Text + "', " + txtPosition.Text + ", " + MainMenu + ", "
                + Visible + ", " + Enabled + ")";
        }
        else
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 2, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            SQL = "UPDATE menutab SET "
                + "title='" + txtMenuName.Text + "', idmenuparent=" + MainMenu + ", enabled=" + Enabled 
                + ", position=" + txtPosition.Text + ", visible=" + Visible + ", idgroupmenu=" 
                + ddlGroupMenu.SelectedValue.ToString() + ", idsubject=" + ddlSubject.SelectedValue.ToString()
                + " WHERE idmenu=" + lblidMenu.Text;            
        }

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();
            g.FUrl.UpdateALLFriendlyURL();

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
            selectMenu(sender, e, ID);
            listMenu(sender, e);
            if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;
        }
    }

    public void newMenu(object sender, EventArgs e)
    {
        populateGroupMenu(sender, e, "1");
        fillMenuDrop(sender, e, "0", "", -1, 0, "1");
        populateSubject(sender, e, "");
        lblidMenu.Text = "";
        txtMenuName.Text = "";
        txtPosition.Text = "";
        chbEnabled.Checked = false;
        chbVisible.Checked = false;
        statusEdit(sender, e, true);
    }

    public void SelectGroupMenu(object sender, EventArgs e)
    {
        fillMenuDrop(sender, e, "0", "", -1, 0, ddlGroupMenu.SelectedValue.ToString());
    }

    private void statusEdit(object sender, EventArgs e, bool status)
    {
        ddlMenu.Enabled = status;
        txtMenuName.Enabled = status;
        txtPosition.Enabled = status;
        chbEnabled.Enabled = status;
        chbVisible.Enabled = status;
        ddlGroupMenu.Enabled = status;
        ddlSubject.Enabled = status;
    }

    public void deleteMenu(object sender, EventArgs e, string idMenu)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 2, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM menutab "
            + "WHERE idmenu=" + idMenu;

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
        }
        if (tmsg.Length == 0) msg.InnerHtml = "Excluded successfully!";
        else
        {
            msg.InnerHtml = tmsg;
            return;
        }
        listMenu(sender, e);
        newMenu(sender, e);
    }

    public void listMenu(object sender, EventArgs e)
    {
        gvMenu.DataBind();
    }

    public void selectMenu(object sender, EventArgs e, string idMenu)
    {
        General g = Session["app"] as General;

        string SQL = "SELECT * "
            + "FROM menutab "
            + "WHERE idmenu=" + idMenu;
        string tmsg="";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidMenu.Text = idMenu;
                populateGroupMenu(sender, e, result["idgroupmenu"].ToString());
                populateSubject(sender, e, result["idsubject"].ToString());
                fillMenuDrop(sender, e, result["idmenuparent"].ToString(), "", -1, 0, result["idgroupmenu"].ToString());                
                txtMenuName.Text = result["title"].ToString();
                txtPosition.Text = result["position"].ToString();
                if (result["enabled"].ToString() == "0") chbEnabled.Checked = false;
                else chbEnabled.Checked = true; 
                if (result["visible"].ToString() == "0") chbVisible.Checked = false;
                else chbVisible.Checked = true;
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

    protected void gvMenu_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;

            String state = rowView["idmenu"].ToString();

            //if (e.Row.Cells[5].Text == "1") e.Row.Cells[5].Text = "True"; else e.Row.Cells[5].Text = "False";
            //if (e.Row.Cells[6].Text == "1") e.Row.Cells[6].Text = "True"; else e.Row.Cells[6].Text = "False";

            if (rowView["submenu"].ToString().Length == 0) e.Row.Cells[2].Text = "Main menu";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            e.Row.Attributes.Add("onclick", "javascript:window.location='Menu.aspx?action=1&id=" + state + "';");
        }
    }

    protected void gvMenu_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteMenu")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvMenu.Rows[index];

            deleteMenu(sender, e, row.Cells[0].Text);
        }
    }
}
