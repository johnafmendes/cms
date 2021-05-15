/* CLASS NAME: Admin/Article.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage Article area.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_Article : System.Web.UI.Page
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
             * 9 - group of Menus
             * 10 - Subject
             * Actions Allowed
             *  - SelectData
             *  - InsertData
             *  - UpdateData
             *  - DeleteData
             */
            //do verification if user can access or not this area
            if (!g.Auth.CheckPermission(g.Auth.idUser, 4, "SelectData")) Response.Redirect("ControlPanel.aspx");
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
            case "1": selectArticle(sender, e, id); break;
            //case "2": saveArticle(sender, e); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newArticle(sender, e); break;
        }
        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    //retrieve list of media and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private void populateMedia(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM mediatab ";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            ddlMainMedia.Items.Clear();

            item++;
            ddlMainMedia.Items.Add(new ListItem("Nothing", "NULL"));

            if (result.HasRows)
            {
                while (result.Read())
                {
                    item++;
                    ddlMainMedia.Items.Add(new ListItem(result["filename"].ToString() + " - " + result["title"].ToString(), result["idmedia"].ToString()));
                    if (idSelected == result["idmedia"].ToString()) ddlMainMedia.SelectedIndex = item;
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

    //retrieve list of subjects/topics and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
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

            ddlSubject.Items.Clear();

            if (result.HasRows)
            {
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

    //retrieve list of group media and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private void populateMediaGroup(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM mediagrouptab "
            + "WHERE enabled=1";
        int item = -1;

        General g = Session["app"] as General;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            ddlMediaGroup.Items.Clear();

            item++;
            ddlMediaGroup.Items.Add(new ListItem("Nothing", "NULL"));

            if (result.HasRows)
            {
                while (result.Read())
                {
                    item++;
                    ddlMediaGroup.Items.Add(new ListItem(result["title"].ToString(), result["idmediagroup"].ToString()));
                    if (idSelected == result["idmediagroup"].ToString()) ddlMediaGroup.SelectedIndex = item;
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

    //delete an article by articleID
    public void deleteArticle(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 4, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM articletagtab "
            + " WHERE idarticle=" + id;

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();

            SQL = "DELETE FROM friendlyurltab "
                + " WHERE idarticle=" + id;

            cmd = g.DB.PrepareSQL(SQL);
            cmd.ExecuteNonQuery();

            SQL = "DELETE FROM articletab "
                + " WHERE idarticle=" + id;

            cmd = g.DB.PrepareSQL(SQL);
            //cmd = new OdbcCommand(SQL, g.DB.connDB);

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
                if (tmsg.Length != 0) msg.InnerHtml = tmsg;
            }
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
        listArticle(sender, e);
    }

    //release and clean the fields to prepare for new data 
    public void newArticle(object sender, EventArgs e)
    {
        lblidArticle.Text = "";
        txtTitle.Text = "";
        lblURL.Text = "";
        txtBody.Text = "";
        txtTag.Text = "";
        txtExpires.Text = "";
        msg.InnerHtml = "";
        //fillMenuDrop(sender, e, null, "", -1, 0, null);
        statusEdit(sender, e, true);
        populateMedia(sender, e, "");
        populateMediaGroup(sender, e, "");
        populateSubject(sender, e, "");
        //populateGroupMenu(sender, e, "1");
        populateLanguages(sender, e, "0");
    }

    //change the status of fields allowing or not editing
    private void statusEdit(object sender, EventArgs e, bool status)
    {
        txtTitle.Enabled = status;
        txtBody.Enabled = status;
        txtTag.Enabled = status;
        ddlSubject.Enabled = status;
        cbEnabled.Enabled = status;
        ddlType.Enabled = status;
        txtExpires.Enabled = status;
    }

    //
    public void SelectGroupMenu(object sender, EventArgs e)
    {
        //fillMenuDrop(sender, e, "0", "", -1, 0, ddlGroupMenu.SelectedValue.ToString());
    }

    //retrieve list of languages and populate into Drop Down List.
    //this method can be rewrited to be used in many areas of cms. At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
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

    /*private void populateGroupMenu(object sender, EventArgs e, string idSelected)
    {
        string tmsg = "", SQL = "SELECT * "
            + "FROM MenuGrouptab ";
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
                    ddlGroupMenu.Items.Add(new ListItem(result["Title"].ToString(), result["idGroupMenu"].ToString()));
                    if (idSelected == result["idGroupMenu"].ToString()) ddlGroupMenu.SelectedIndex = item;
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
    }*/

    /*public int fillMenuDrop(object sender, EventArgs e, string idMenuSelected, string subMenu, int Item, int nLevel, string idGroupMenu)
    {
        General g = Session["app"] as General;

        //int item=-1;
        string tmsg = "", sIdent = "";
        string SQL = "select * from menudetailtab";
        //string SQL = "SELECT * "
        //    + "FROM MenuDetailTab ";
        //if (subMenu.Length == 0) SQL += "WHERE idMenuParent is NULL AND idGroupMenu=" + idGroupMenu;
        //else SQL += "WHERE idMenuParent =" + subMenu + " AND idGroupMenu=" + idGroupMenu;



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
                        for (int i = 0; i < (nLevel * 2); i++) desloc += ". ";
                        sIdent = desloc + sIdent;
                    }
                    else sIdent = "";
                    ddlMenu.Items.Add(new ListItem(sIdent + result["Title"].ToString(), result["idMenuDetail"].ToString()));
                    if (idMenuSelected == result["idMenuDetail"].ToString()) ddlMenu.SelectedIndex = Item;
                    Item = fillMenuDrop(sender, e, idMenuSelected, result["idMenuDetail"].ToString(), Item, nLevel, idGroupMenu);
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
    }*/

    //retrieve an Article passing ID.
    public void selectArticle(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "SELECT a.* "
            + "FROM articletab a "
            + "WHERE a.idarticle=" + id.ToString();

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            //Create three columns with string as their type

            if (result.HasRows)
            {
                lblidArticle.Text = result["idarticle"].ToString();
                txtTitle.Text = g.DB.HTMLDecode(result["title"].ToString());
                txtBody.Text = g.DB.HTMLDecode(result["description"].ToString());
                populateMedia(sender, e, result["idmainmedia"].ToString());
                populateMediaGroup(sender, e, result["idmediagroup"].ToString());
                populateSubject(sender, e, result["idsubject"].ToString());
                populateLanguages(sender, e, result["idlanguage"].ToString());

                if (result["enabled"].ToString() == "0") cbEnabled.Checked = false;
                else cbEnabled.Checked = true;

                if (result["enabledetails"].ToString() == "0") cbEnableDetails.Checked = false;
                else cbEnableDetails.Checked = true;

                if (result["enablenews"].ToString() == "0") cbEnableNews.Checked = false;
                else cbEnableNews.Checked = true;

                ddlType.SelectedValue = result["type"].ToString();

                txtExpires.Text = result["expires"].ToString();

                //fillMenuDrop(sender, e, result["IDMenu"].ToString(), "", -1, 0, null);

                /*SQL = "SELECT idGroupMenu "
                    + "FROM MenuDetailTab "
                    + "WHERE idMenuDetail = " + result["IDMenu"].ToString();

                //OdbcCommand cmd2 = new OdbcCommand(SQL, g.DB.connDB);
                OdbcCommand cmdMenu = g.DB.PrepareSQL(SQL);

                OdbcDataReader resultMenu = cmdMenu.ExecuteReader();

                //if (resultMenu.HasRows) populateGroupMenu(sender, e, resultMenu["idGroupMenu"].ToString());

                resultMenu.Close(); 
                resultMenu.Dispose();
                cmdMenu.Dispose();*/

                SQL = "SELECT tag "
                    + "FROM tagtab "
                    + "WHERE idtag in (SELECT idtag FROM articletagtab WHERE idarticle=" + id + ")";

                //OdbcCommand cmd2 = new OdbcCommand(SQL, g.DB.connDB);
                OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);

                try
                {
                    OdbcDataReader result2 = cmd2.ExecuteReader();

                    txtTag.Text = "";
                    if (result2.HasRows) while (result2.Read()) txtTag.Text += g.DB.HTMLDecode(result2["tag"].ToString()) + ", ";
                    if (g.FolderInstalled.Length > 0)
                        lblURL.Text = "http://" + Request.ServerVariables["http_host"].ToString() + "/" + g.FolderInstalled + "/Default.aspx?action=1&idA=" + id + "&idS=" + result["idsubject"].ToString() + "&idL=" + result["idlanguage"].ToString();
                    else lblURL.Text = "http://" + Request.ServerVariables["http_host"].ToString() + "/Default.aspx?action=1&idA=" + id + "&idS=" + result["idsubject"].ToString() + "&idL=" + result["idlanguage"].ToString();
                    SQL = "SELECT friendlyurl "
                        + "FROM friendlyurltab "
                        + "WHERE idarticle=" + id;

                    //OdbcCommand cmd2 = new OdbcCommand(SQL, g.DB.connDB);
                    OdbcCommand cmd3 = g.DB.PrepareSQL(SQL);
                    OdbcDataReader result3 = cmd3.ExecuteReader();
                    if (result3.HasRows) 
                        lblFriendlyURL.Text = g.ReturnProtocolAndPort() 
                            + result3["friendlyurl"].ToString().Remove(0,1);
                }
                catch (OdbcException o)
                {
                    tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
                }
                finally
                {
                    if (tmsg.Length != 0) msg.InnerHtml = tmsg;
                }
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
        listArticle(sender, e);
        statusEdit(sender, e, true);
    }

    //Do inserting of Tags that has been written.
    //this method can be rewrited to be used in other area of cms (tags.cs). At the moment it is being used in more the one
    //place. It mean that from maitenance point of view it is not good
    private string InsertTags(string tags, string id)
    {
        General g = Session["app"] as General;
        string tmsg = "", SQL = "";
        string[] stringSeparators = new string[] { "," };
        string[] r;

        r = tags.Split(stringSeparators, StringSplitOptions.None);

        SQL = "DELETE FROM articletagtab "
        + "WHERE idarticle=" + id;

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
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
        }

        try
        {
            foreach (string s in r)
            {
                string sTag;
                sTag = s.Trim();
                if (sTag.Length != 0)
                {
                    SQL = "SELECT * FROM tagtab "
                    + "WHERE tag ='" + sTag + "'";

                    cmd = g.DB.PrepareSQL(SQL);

                    //cmd = new OdbcCommand(SQL, g.DB.connDB);
                    OdbcDataReader result = cmd.ExecuteReader();
                    if (result.HasRows)
                    {
                        try
                        {
                            SQL = "INSERT INTO articletagtab "
                                + "VALUES (" + id + ", " + result["idtag"].ToString() + ")";
                            //OdbcCommand cmd2 = new OdbcCommand(SQL, g.DB.connDB);
                            OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);

                            cmd2.ExecuteNonQuery();
                        }
                        catch (OdbcException o)
                        {
                            tmsg += g.DB.catchODBCException(o, g.ErrorLevel);
                        }
                        finally
                        {
                            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
                        }
                    }
                    else
                    {
                        SQL = "INSERT INTO tagtab "
                            + "VALUES (NULL, '" + sTag + "', 1)";
                        //OdbcCommand cmd2 = new OdbcCommand(SQL, g.DB.connDB);
                        OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);

                        try
                        {
                            cmd2.ExecuteNonQuery();
                        }
                        catch (OdbcException o)
                        {
                            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
                        }
                        finally
                        {
                            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
                        }

                        SQL = "SELECT LAST_INSERT_ID() AS ID";
                        cmd2.CommandText = SQL;
                        string idTag = "";

                        try
                        {
                            OdbcDataReader result2 = cmd2.ExecuteReader();

                            if (result2.HasRows) idTag = result2["ID"].ToString();
                        }
                        catch (OdbcException o)
                        {
                            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
                        }
                        finally
                        {
                            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
                        }

                        SQL = "INSERT INTO articletagtab "
                            + "VALUES (" + id + ", " + idTag.ToString() + ")";
                        //OdbcCommand cmd3 = new OdbcCommand(SQL, g.DB.connDB);
                        OdbcCommand cmd3 = g.DB.PrepareSQL(SQL);

                        try
                        {
                            cmd3.ExecuteNonQuery();
                        }
                        catch (OdbcException o)
                        {
                            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
                        }
                        finally
                        {
                            cmd3.Dispose();
                            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
                        }
                    }
                }
            }
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
        return tmsg;
    }

    //Save the article.
    //in general, the save function are being used as Insert and Update function. I decide what to do by
    //lblidArticle.Text
    public void saveArticle(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "", SQLFURL = "", Enabled = "0", id = "";

        if (txtTitle.Text.Length == 0 || txtTag.Text.Length == 0 || txtBody.Text.Length == 0)
        {
            tmsg = "You must type some information on Title, Description, and Tags.";
            return;
        }

        if (cbEnabled.Checked) Enabled = "1";

        if (g.MakeMySQLDateTime(txtExpires.Text) == "false")
        {
            msg.InnerHtml = "Expire Date is wrong! <br/><br/>You need type folowing this format: dd/mm/yyyy hh:mm:ss<br/>Your article has not been saved.";
            return;
        }

        if (lblidArticle.Text == "")
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 4, "InsertData"))
            {
                msg.InnerHtml = "Insert new data is not allowed.";
                return;
            }

            SQL = "INSERT INTO articletab (idarticle, title, description, idsubject, iduser, idlanguage, type, "
                + "published, "
                + "expires, lastupdate, idmainmedia, idmediagroup, enabledetails, enablenews, enabled) "
                + "VALUES (NULL, '" + g.DB.HTMLEncode(txtTitle.Text) + "', '" + g.DB.HTMLEncode(txtBody.Text)
                + "', " + ddlSubject.SelectedValue.ToString() + ", " + g.Auth.idUser + ", "
                + ddlLanguage.SelectedValue.ToString() + ", '" + ddlType.SelectedItem.Text.ToString()
                + "', '" + g.MakeMySQLDateTime(null)
                + "', '" + g.MakeMySQLDateTime(txtExpires.Text) + "', '" + g.MakeMySQLDateTime(null)
                + "', " + ddlMainMedia.SelectedValue.ToString() + ", " + ddlMediaGroup.SelectedValue.ToString()
                + ", " + cbEnableDetails.Checked + ", " + cbEnableNews.Checked + ", " + Enabled + ")";
        }
        else
        {
            if (!g.Auth.CheckPermission(g.Auth.idUser, 4, "UpdateData"))
            {
                msg.InnerHtml = "Update data is not allowed.";
                return;
            }

            SQL = "UPDATE articletab SET "
                + "title='" + g.DB.HTMLEncode(txtTitle.Text) + "', description='" + g.DB.HTMLEncode(txtBody.Text)
                + "', idsubject=" + ddlSubject.SelectedValue.ToString() + ", type='"
                + ddlType.SelectedItem.Text.ToString() + "', expires='" + g.MakeMySQLDateTime(txtExpires.Text)
                + "', lastupdate='" + g.MakeMySQLDateTime(null) + "', idmainmedia="
                + ddlMainMedia.SelectedValue.ToString() + ", idmediagroup=" + ddlMediaGroup.SelectedValue.ToString()
                + ", enabledetails=" + cbEnableDetails.Checked + ", enablenews=" + cbEnableNews.Checked
                + ", enabled=" + Enabled
                + " WHERE idarticle=" + lblidArticle.Text;
        }

        //OdbcCommand cmd = new OdbcCommand(SQL, g.DB.connDB);
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();

            if (lblidArticle.Text == "")
            {
                SQL = "SELECT LAST_INSERT_ID() AS ID";
                cmd.CommandText = SQL;
                OdbcDataReader result = cmd.ExecuteReader();

                if (result.HasRows) id = result["ID"].ToString();
            }else id = lblidArticle.Text;

            /*string SQLFURLDelete = "DELETE FROM FriendlyURLTab "
                + "WHERE idArticle = " + id;

            OdbcCommand cmdFURL = g.DB.PrepareSQL(SQLFURLDelete);
            
            cmdFURL.ExecuteNonQuery();

            string fURL = g.FUrl.CreateFriendURL(ddlSubject.SelectedValue.ToString(), txtTitle.Text, id.ToString());

            SQLFURL = "INSERT INTO FriendlyURLTab VALUES (NULL, '" + id + "', '"+ fURL +"')";
            cmdFURL = g.DB.PrepareSQL(SQLFURL);
            cmdFURL.ExecuteNonQuery();
            cmdFURL.Dispose();*/

            g.FUrl.UpdateFriendlyURL(id, ddlSubject.SelectedValue.ToString(), txtTitle.Text);

            tmsg = InsertTags(g.DB.HTMLEncode(txtTag.Text), id);
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
                selectArticle(sender, e, id);
                msg.InnerHtml = "Saved successfully!";
            }
            else msg.InnerHtml = tmsg;
        }
    }

    //retrieve a list of articles in datagridview
    public void listArticle(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        string tmsg = "";

        try { gvArticle.DataBind(); }
        catch (Exception o)
        {
            tmsg = g.catchException(o, g.ErrorLevel);
        }
        finally
        {
            if (tmsg.Length != 0) msg.InnerHtml = tmsg;
        }
    }

    //for each row created in datagridview is cleaned HTML and added the attributes onMouseOver, onMouseOut
    // and onclick that allow user click on the row to open the article of that row.
    protected void gvArticle_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        General g = Session["app"] as General;

        if (e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Pager/*GridView1.EditIndex == -1*/)
        {
            e.Row.Cells[1].Text = g.DB.HTMLDecode(e.Row.Cells[1].Text);
            //if (e.Row.Cells[3].Text == "1") e.Row.Cells[3].Text = "True"; else e.Row.Cells[3].Text = "False";

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            DataRowView rowView = (DataRowView)e.Row.DataItem;

            String state = rowView["idarticle"].ToString();

            e.Row.Attributes.Add("onclick", "javascript:window.location='Article.aspx?action=1&id=" + state + "';");
        }
    }

    //Each click in dataGridView if checked if was clicked in column called Delete. If yes, send the user to 
    //deleteArticle function.
    protected void gvArticle_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteArticle")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvArticle.Rows[index];

            deleteArticle(sender, e, row.Cells[0].Text);
        }
    }
}