using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class installation_PartIII : System.Web.UI.Page
{
    //public OdbcConnection connDB;

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    public void saveConfig(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "";
        
        SQL = "UPDATE configtab SET "
            + "Title='" + txtTitle.Text + "', folderinstalled='" + txtFolderInstalled.Text + "'";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (OdbcException o)
        {
            tmsg = "We are sorry, we are experiencing technical problems ...";
            tmsg += "<hr/>";
            tmsg += "<br/>Message: " + o.Message;
            tmsg += "<br/>Source: " + o.Source;
            tmsg += "<br/>Stack Trace: " + o.StackTrace;
            tmsg += "<br/>Target Site: " + o.TargetSite.Name;
            tmsg += "<br/>Others: " + o.InnerException;
            tmsg += "<hr/>";
            tmsg += "<br/>Try again! If the problem persist, contact techinical suport";
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0)
            {
                msg.InnerHtml = "Saved successfully!";
                btnNext.Enabled = true;
            }
            else msg.InnerHtml = tmsg;
        }
    }
}
