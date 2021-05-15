using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Data.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;


public partial class admin_Media : System.Web.UI.Page
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
            if (!g.Auth.CheckPermission(g.Auth.idUser, 3, "SelectData")) Response.Redirect("ControlPanel.aspx");
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
            case "1": selectMedia(sender, e, id); break;
            default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; else newMedia(sender, e); break;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
    }

    public void newMedia(object sender, EventArgs e)
    {
        lblidMedia.Text = "";
        txtTitle.Text = "";
        statusEdit(sender, e, true);
        gvMedia.DataBind();
    }

    private void statusEdit(object sender, EventArgs e, bool status)
    {
        File.Enabled = status;
        txtTitle.Enabled = status;
    }

    private string[] checkFileType(string sFile)
    {
        string ext = Path.GetExtension(sFile);
        string[] result = {false.ToString(), "File"};

        switch (ext.ToLower())
        {
            case "": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".gif": { result[0] = true.ToString(); result[1] = "Image"; return result; }
            case ".png": { result[0] = true.ToString(); result[1] = "Image"; return result; }
            case ".jpg": { result[0] = true.ToString(); result[1] = "Image"; return result; }
            case ".bmp": { result[0] = true.ToString(); result[1] = "Image"; return result; }
            case ".jpeg": { result[0] = true.ToString(); result[1] = "Image"; return result; }
            case ".swf": { result[0] = true.ToString(); result[1] = "Flash"; return result; }
            case ".doc": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".xls": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".rtf": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".pdf": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".avi": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".rm": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".ra": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".ram": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".mov": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".qt": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".wma": { result[0] = true.ToString(); result[1] = "Audio"; return result; }
            case ".wmv": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".wav": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".mpeg": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".mp3": { result[0] = true.ToString(); result[1] = "Audio"; return result; }
            case ".mp4": { result[0] = true.ToString(); result[1] = "Audio"; return result; }
            case ".mpg": { result[0] = true.ToString(); result[1] = "Video"; return result; }
            case ".mid": { result[0] = true.ToString(); result[1] = "Audio"; return result; }
            case ".midi": { result[0] = true.ToString(); result[1] = "Audio"; return result; }
            case ".txt": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".rar": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".zip": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".7z": { result[0] = true.ToString(); result[1] = "File"; return result; }
            case ".ppt": { result[0] = true.ToString(); result[1] = "File"; return result; }
            default: { result[0] = false.ToString(); result[1] = "Unknown"; return result; }
        }
    }

    public void saveMedia(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            msg.InnerHtml = "";
            return;
        }

        General g = Session["app"] as General;

        string tmsg = "", SQL = "";//, Enabled = "0";
        ArrayList f = new ArrayList();

        OdbcTransaction myTransaction = null;
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            if (myTransaction!=null) myTransaction = g.DB.connDB.BeginTransaction();
            
            cmd.Transaction = myTransaction;
            //bool typeAllowed = bool.Parse(checkFileType(File.FileName)[0]);

            if (bool.Parse(checkFileType(File.FileName)[0]) && (File.PostedFile.ContentLength <= 11048576)/* || lblidMedia.Text.Length > 0*/)//1024k
            {
                int i = 0, Width=0, Height=0;
                string[] keys;
                string filename, file_ext, fullfilename, filepath;//, pat = @"\\(?:.+)\\(.+)\.(.+)";
                HttpFileCollection Files;

                Files = Request.Files;
                keys = Files.AllKeys;

                for (i = 0; i <= keys.GetUpperBound(0); i++)
                {
                    filepath = Files[i].FileName;
                    if (checkFileType(File.FileName)[1].ToString() == "Image")
                    {
                        //Create an image object from the uploaded file
                        System.Drawing.Image UploadedImage = System.Drawing.Image.FromStream(File.PostedFile.InputStream);

                        //Determine width and height of uploaded image
                        Width = int.Parse(UploadedImage.PhysicalDimension.Width.ToString());
                        Height = int.Parse(UploadedImage.PhysicalDimension.Height.ToString());

                        //Check that image does not exceed maximum dimension settings
                        //if (int.Parse(Width) > 600 || int.Parse(Height) > 400)
                        //Response.Write("This image is too big - please resize it!");

                    }
                    //if (filepath.Length > 0)
                    //{                        
                   // }
                    filename = Path.GetFileNameWithoutExtension(filepath);
                    file_ext = Path.GetExtension(filepath);
                    fullfilename = Path.GetFileName(filepath);

                    if (lblidMedia.Text.Length == 0) //new file
                    {                        
                        if (!g.Auth.CheckPermission(g.Auth.idUser, 3, "InsertData"))
                        {
                            msg.InnerHtml = "Insert new data is not allowed.";
                            return;
                        }

                        SQL = "INSERT INTO mediatab "
                            + "VALUES (NULL, '" + txtTitle.Text + "', '" + fullfilename + "', '0', '" 
                            + checkFileType(File.FileName)[1].ToString() + "', "+Width+", "+Height+", 1)";

                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();

                        SQL = "SELECT LAST_INSERT_ID() AS ID"; // SELECT SCOPE_IDENTITY()                    
                        cmd.CommandText = SQL;
                        OdbcDataReader result = cmd.ExecuteReader();

                        if (result.HasRows)
                        {
                            fullfilename = result["ID"].ToString() + file_ext;
                            SQL = "UPDATE mediatab SET "
                                + "filename='" + fullfilename
                                + "' WHERE idmedia=" + result["ID"].ToString();
                            cmd.CommandText = SQL;
                            result.Close();
                            result.Dispose();
                            cmd.ExecuteNonQuery();

                            f.Add(fullfilename);
                        }
                        else
                        {
                            tmsg += "No files uploaded, or wrong content type!";
                            if (myTransaction != null) myTransaction.Rollback();
                            return;
                        }

                        try
                        {
                            Files[i].SaveAs(Server.MapPath("..\\images\\") + fullfilename);
                            tmsg += "- File: " + fullfilename;
                            tmsg += "<br/>- Saved successfully!";
                        }
                        catch (Exception err)
                        {
                            if (myTransaction != null) myTransaction.Rollback();
                            tmsg += "No files uploaded, or wrong content type! <br/>" + err.Message;
                            throw;
                        }
                    }
                    else //update record
                    {
                        string idMedia = lblidMedia.Text;

                        if (!g.Auth.CheckPermission(g.Auth.idUser, 3, "UpdateData"))
                        {
                            msg.InnerHtml = "Update data is not allowed.";
                            return;
                        }

                        SQL = "UPDATE mediatab SET "
                            + "title='" + txtTitle.Text + "'";
                        if (filename.Length > 0)
                        {
                            deleteMedia(sender, e, idMedia);
                            SQL += ", originalname='" + filename + file_ext + "', filename='"
                                + idMedia + file_ext + "', type ='"
                                + checkFileType(filename + file_ext)[1].ToString() + "' ";

                            if (checkFileType(filename + file_ext)[1].ToString() == "Image")
                                SQL += ", width=" + Width + ", height=" + Height;
                        }
                        SQL += " WHERE idmedia=" + idMedia;

                        cmd.CommandText = SQL;

                        try
                        {
                            cmd.ExecuteNonQuery();

                            if (fullfilename.Length > 0)
                            {
                                try
                                {
                                    Files[i].SaveAs(Server.MapPath("..\\images\\") + idMedia + file_ext);
                                    tmsg += "- File: " + fullfilename;
                                    tmsg += "<br/>- Saved successfully!";
                                }
                                catch (Exception err)
                                {
                                    if (myTransaction != null) myTransaction.Rollback();
                                    tmsg += "No files uploaded, or wrong content type! <br/>" + err.Message;
                                    throw;
                                }
                            }
                        }
                        catch(OdbcException odb)
                        {
                            tmsg = g.DB.catchODBCException(odb, g.ErrorLevel);
                        }
                    }
                }
            }
            else
            {
                if (myTransaction != null) myTransaction.Rollback();
                tmsg += "No files uploaded, or wrong content type!";
                return;
            }

            if (myTransaction != null) myTransaction.Commit();
        }
        catch (OdbcException o)
        {
            if (myTransaction != null) myTransaction.Rollback();

            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0) msg.InnerHtml = "Saved successfully!";
            else msg.InnerHtml = tmsg;
            listMedia(sender,e);
        }
    }

    public void selectMedia(object sender, EventArgs e, string id)
    {
        General g = Session["app"] as General;

        string tmsg="", SQL = "SELECT * "
            + "FROM mediatab "
            + "WHERE idmedia=" + id.ToString();

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblidMedia.Text = result["idmedia"].ToString();
                txtTitle.Text = result["title"].ToString();
                //if (result["Enabled"].ToString() == "1") cbEnabled.Checked = true; else cbEnabled.Checked = false;
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
    }


    public void listMedia(object sender, EventArgs e)
    {
        gvMedia.DataBind();
    }

    public void deleteMedia(object sender, EventArgs e, string idMedia)
    {
        General g = Session["app"] as General;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 3, "DeleteData"))
        {
            msg.InnerHtml = "Delete data is not allowed.";
            return;
        }

        string tmsg = "", SQL = "";

        SQL = "DELETE FROM mediatab "
            + " WHERE idmedia=" + idMedia;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();
            foreach(string sFile in System.IO.Directory.GetFiles(@Server.MapPath("..\\images\\"), idMedia+".*"))
            {
                System.IO.File.Delete(sFile);
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
        listMedia(sender, e);
        newMedia(sender, e);
    }

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
                            + "<embed src=\"../images/" + rowView["filename"].ToString() + "\" width=\"150\" height=\"100\">"
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
                            + "\" alt=\"\" title=\"\" width=\""+width+"\">";
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
                /*
                case ".gif": e.Row.Cells[1].Text = "<img src=\"..\\images\\Imagen-GIF-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".png": e.Row.Cells[1].Text = "<img src=\"..\\images\\Imagen-PNG-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".jpg": e.Row.Cells[1].Text = "<img src=\"..\\images\\Imagen-JPG-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".bmp": e.Row.Cells[1].Text = "<img src=\"..\\images\\Imagen-BMP-48x48.png\" alt=\"\" title=\"\">"; break;
                case ".jpeg": e.Row.Cells[1].Text = "<img src=\"..\\images\\Imagen-JPG-48x48.png\" alt=\"\" title=\"\">"; break;

                case ".m3u": e.Row.Cells[1].Text = "<img src=\"..\\images\\icoDOC.jpg\" alt=\"\" title=\"\">"; break;
                case ".mid": e.Row.Cells[1].Text = "<img src=\"..\\images\\icoDOC.jpg\" alt=\"\" title=\"\">"; break;
                case ".midi": e.Row.Cells[1].Text = "<img src=\"..\\images\\icoDOC.jpg\" alt=\"\" title=\"\">"; break;
                case ".rmi": e.Row.Cells[1].Text = "<img src=\"..\\images\\icoDOC.jpg\" alt=\"\" title=\"\">"; break;*/

            }

            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.style.color='Silver'; this.title='Original Name: " + rowView["OriginalName"].ToString() + "';");
            e.Row.Attributes.Add("onmouseout", "this.style.cursor='pointer';this.style.color='Black';");

            e.Row.Attributes.Add("onclick", "javascript:window.location='Media.aspx?action=1&id=" + state + "';");
        }
    }

    protected void gvMedia_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteMedia")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gvMedia.Rows[index];

            deleteMedia(sender, e, row.Cells[0].Text);
        }
    }

}
