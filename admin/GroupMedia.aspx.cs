/* CLASS NAME: Admin/GroupMedia.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage Group of Media that can be used to create a rotation images on front page.
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

public partial class GroupMedia : System.Web.UI.Page
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
            if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "SelectData")) Response.Redirect("ControlPanel.aspx");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string action, id;
        action = g.DB.cleanSQLInjection(Request.QueryString["action"]);
        id = g.DB.cleanSQLInjection(Request.QueryString["id"]);

        //set string to connect prepare dataGridView to display data
        SqlDataSource1.ConnectionString = g.DB.connectionString;
        SqlDataSource1.ProviderName = g.DB.providerName;
        SqlDataSource2.ConnectionString = g.DB.connectionString;
        SqlDataSource2.ProviderName = g.DB.providerName;
        SqlDataSource3.ConnectionString = g.DB.connectionString;
        SqlDataSource3.ProviderName = g.DB.providerName;

        switch (action)
        {
            case "0": break; //do nothing
            case "1": selectMediaGroup(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newMediaGroup(sender, e); break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    //Save data
    public void saveMediaGroup(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string Type = "0";
        string tmsg = "", SQL = "", ID = "";

        if (lblidGroupMedia.Text == "")
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            SQL = "INSERT INTO mediagrouptab "
                + "VALUES (NULL, '" + txtTitle.Text + "', '" + txtDescription.Text + "', " 
                + cbEnabled.Checked.ToString() + ")";
        }
        else
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            SQL = "UPDATE mediagrouptab SET "
                + "title='" + txtTitle.Text + "', description='" + txtDescription.Text 
                + "', enabled=" + cbEnabled.Checked.ToString() 
                + " WHERE idmediagroup=" + lblidGroupMedia.Text;
            ID = lblidGroupMedia.Text;
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
            if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;
        }
        selectMediaGroup(sender, e, ID);
        populateMediaGroup(sender, e);
    }

    //clean the fields and change the status to might new data and populate the Media Group
    public void newMediaGroup(object sender, EventArgs e)
    {
        lblidGroupMedia.Text = "";
        txtTitle.Text = "";
        txtDescription.Text = "";
        statusEdit(sender, e, true);
        populateMediaGroup(sender, e);
    }

    //change the status of fields to allow or denny insert/edit data
    private void statusEdit(object sender, EventArgs e, bool status)
    {
        txtTitle.Enabled = status;
        txtDescription.Enabled = status;
        cbEnabled.Enabled = status;
    }

    //Delete media Group by id
    public void deleteMediaGroup(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM mediagrouptab "
            + " WHERE idmediagroup=" + id;

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
        listMediaGroup(sender, e);
        newMediaGroup(sender, e);
        populateMediaGroup(sender, e);
    }

    //List/Refresh the dataGridView 
    public void listMediaGroup(object sender, EventArgs e)
    {
        gvMediaGroup.DataBind();
    }

    //This function update the ID of Media and ID of Group of Media to recreate dataGridView
    public void FilterMediaGroup(object sender, EventArgs e)
    {
        SqlDataSource2.SelectParameters["idmedia"].DefaultValue = ddlMediaGroup.SelectedValue.ToString();
        gvMediaList.DataBind();
        SqlDataSource3.SelectParameters["idmediagroup"].DefaultValue = ddlMediaGroup.SelectedValue.ToString();
        gvMediaInGroup.DataBind();
    }

    //retrieve list of media group and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private void populateMediaGroup(object sender, EventArgs e)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM mediagrouptab ";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                ddlMediaGroup.Items.Clear();
                while (result.Read())
                {
                    item++;
                    ddlMediaGroup.Items.Add(new ListItem(result["title"].ToString(), result["idmediagroup"].ToString()));
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
        FilterMediaGroup(sender, e);
    }

    //Select Media Group to display in a form
    public void selectMediaGroup(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "SELECT *, if (enabled, 'true', 'false') as enabled "
            + "FROM mediagrouptab "
            + "WHERE idmediagroup=" + id;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidGroupMedia.Text = result["idmediagroup"].ToString();
                txtTitle.Text = result["title"].ToString();
                txtDescription.Text = result["description"].ToString();
                cbEnabled.Checked = Boolean.Parse(result["enabled"].ToString());
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
        listMediaGroup(sender, e);
        populateMediaGroup(sender, e);
    }

    //everytime when a user click in a button --> , it will call this function to add media for a group
    protected void AddMedia(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvMediaList.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("AddSelector");
            if (cb != null && cb.Checked)
            {
                //int idMedia = Convert.ToInt32(gvMediaList.DataKeys[row.RowIndex].Value);
                string idMedia = row.Cells[0].Text;

                General g = Session["app"] as General;

                string tmsg = "", SQL = "";

                if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "InsertData"))
                {
                    msg.InnerHtml = "Insert new data is not allowed.";
                    return;
                }

                SQL = "INSERT INTO medialisttab "
                    + "VALUES (" + idMedia + ", " + ddlMediaGroup.SelectedValue.ToString() + ") ";

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
        FilterMediaGroup(sender, e);       
    }

    //everytime when a user click in the button <--, will call this function to remove the selected media from
    //the group
    protected void RemoveMedia(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvMediaInGroup.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("RemoveSelector");
            if (cb != null && cb.Checked)
            {
                //int idMedia = Convert.ToInt32(gvMediaInGroup.DataKeys[row.RowIndex].Value);
                string idMedia = row.Cells[0].Text;

                General g = Session["app"] as General;

                string tmsg = "", SQL = "";

                if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "DeleteData"))
                {
                    msg.InnerHtml = "Delete data is not allowed.";
                    return;
                }

                SQL = "DELETE FROM medialisttab "
                    + "WHERE idmedia=" + idMedia + " AND idmediagroup=" + ddlMediaGroup.SelectedValue.ToString();

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
                    if (tmsg.Length == 0) msg.InnerHtml = "Removed successfully!";
                    else msg.InnerHtml = tmsg;
                }
            }
        }
        FilterMediaGroup(sender, e);
    }

    //for each row created in datagridview it will add the attributes onMouseOver, onMouseOut
    // and onclick that allow user click on the row to open the Group of Media of that row.
    protected void gvMediaGroup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            //if (e.Row.Cells[5].Text == "1") e.Row.Cells[5].Text = "True"; else e.Row.Cells[5].Text = "False";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            DataRowView rowView = (DataRowView)e.Row.DataItem;

            String state = rowView["idmediagroup"].ToString();

            e.Row.Attributes.Add("onclick", "javascript:window.location='GroupMedia.aspx?action=1&id=" + state + "';");
        }
    }

    //Each click in dataGridView if checked if was clicked in column called Delete. If yes, send the user to 
    //deleteMediaGroup function.
    protected void gvMediaGroup_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteMediaGroup")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvMediaGroup.Rows[index];

            deleteMediaGroup(sender, e, row.Cells[0].Text);
        }
    }

    //for each row created in datagridview it will add the attributes onMouseOver, onMouseOut
    // and onclick that allow user click on the row to open the Media of that row. Also it is changing
    // the images path of each cell.
    protected void gvMedia_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;

            string state = rowView["idmedia"].ToString(), width = null;
            string type = Path.GetExtension(rowView["filename"].ToString());

            switch (type.ToLower())
            {
                case ".pdf": e.Row.Cells[1].Text = "<img src=\"..\\images\\Oficina-PDF-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".doc": e.Row.Cells[1].Text = "<img src=\"..\\images\\Oficina-DOC-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".swf":
                    {
                        //e.Row.Cells[1].Text = "<img src=\"..\\images\\Flash-8-48x48.png\" alt=\"\" title=\"\">"; 
                        e.Row.Cells[1].Text = "<object width=\"150\" height=\"100\">"
                            + "<param name=\"movie\" value=\"../images/" + rowView["filename"].ToString() + "\">"
                            + "<embed src=\"../images/"+rowView["filename"].ToString()+"\" width=\"150\" height=\"100\">"
                            + "</embed></object>";
                        break;
                    }
                case ".xls": e.Row.Cells[1].Text = "<img src=\"..\\images\\Oficina-XLS-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".avi": e.Row.Cells[1].Text = "<img src=\"..\\images\\Video-AVI-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".mov": e.Row.Cells[1].Text = "<img src=\"..\\images\\Video-MOVIE-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".wma": e.Row.Cells[1].Text = "<img src=\"..\\images\\Audio-WMA-2-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".wmv": e.Row.Cells[1].Text = "<img src=\"..\\images\\Video-WMV-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".wav": e.Row.Cells[1].Text = "<img src=\"..\\images\\Audio-WAV-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".mpeg": e.Row.Cells[1].Text = "<img src=\"..\\images\\Video-MPEG-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".mp3": e.Row.Cells[1].Text = "<img src=\"..\\images\\Audio-MP3-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".mp4": e.Row.Cells[1].Text = "<img src=\"..\\images\\Filetype-MP-4-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".mpg": e.Row.Cells[1].Text = "<img src=\"..\\images\\Filetype-MPG-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".rar": e.Row.Cells[1].Text = "<img src=\"..\\images\\Comprimidos-RAR-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".zip": e.Row.Cells[1].Text = "<img src=\"..\\images\\Comprimidos-ZIP-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".ogg": e.Row.Cells[1].Text = "<img src=\"..\\images\\Audio-OGG-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".ppt": e.Row.Cells[1].Text = "<img src=\"..\\images\\Oficina-PPT-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".txt": e.Row.Cells[1].Text = "<img src=\"..\\images\\Oficina-TXT-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".7zip": e.Row.Cells[1].Text = "<img src=\"..\\images\\7zip-SZ-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".gif":
                    {
                        if (int.Parse(rowView["width"].ToString()) > 150) width = "150";
                        else width = rowView["width"].ToString();
                        e.Row.Cells[1].Text = "<img src=\"..\\images\\" + rowView["filename"].ToString()
                            + "\" alt=\"\" title=\"\" width=\"" + width + "\">";
                        break;
                    }
                case ".png":
                    {
                        if (int.Parse(rowView["width"].ToString()) > 150) width = "150";
                        else width = rowView["width"].ToString();
                        e.Row.Cells[1].Text = "<img src=\"..\\images\\" + rowView["filename"].ToString()
                            + "\" alt=\"\" title=\"\" width=\"" + width + "\">";
                        break;
                    }
                case ".jpg":
                    {
                        if (int.Parse(rowView["width"].ToString()) > 150) width = "150";
                        else width = rowView["width"].ToString();
                        e.Row.Cells[1].Text = "<img src=\"..\\images\\" + rowView["filename"].ToString()
                            + "\" alt=\"\" title=\"\" width=\"" + width + "\">";
                        break;
                    }
                case ".bmp":
                    {
                        if (int.Parse(rowView["width"].ToString()) > 150) width = "150";
                        else width = rowView["width"].ToString();
                        e.Row.Cells[1].Text = "<img src=\"..\\images\\" + rowView["filename"].ToString()
                            + "\" alt=\"\" title=\"\" width=\"" + width + "\">";
                        break;
                    }
                case ".jpeg":
                    {
                        if (int.Parse(rowView["width"].ToString()) > 150) width = "150";
                        else width = rowView["width"].ToString();
                        e.Row.Cells[1].Text = "<img src=\"..\\images\\" + rowView["filename"].ToString()
                            + "\" alt=\"\" title=\"\" width=\"" + width + "\">";
                        break;
                    }
                default: e.Row.Cells[1].Text = "<img src=\"..\\images\\empty-48x48.png\" alt=\"\" title=\"\">"; break;
            }

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver'; this.title='Title|Original Name: [" + rowView["Title"].ToString() + " | " + rowView["OriginalName"].ToString() + "]';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            //e.Row.Attributes.Add("onclick", "javascript:window.location='Media.aspx?action=1&id=" + state + "';");
        }
    }

}
