/* CLASS NAME: Admin/Config.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage Default page in Admin area that ask for authentication.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //say to browser to do not do cache of data- it is important to avoid mistakes on Captcha
        HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
        HttpContext.Current.Response.Expires = 0;
        HttpContext.Current.Response.Cache.SetNoStore();
        HttpContext.Current.Response.AddHeader("Pragma", "no-cache");

        //switch on or switch off blocks/areas on page.
        ((ContentPlaceHolder)Master.FindControl("RSMainMenu")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R1Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;

        General g = Session["app"] as General;

        if (Request.QueryString["action"] == "0") g.Cpcha.returnImage(HttpContext.Current);
        if (Request.QueryString["action"] == "2")
        {
            g.Cpcha.reproduceSound(sender, e);
            Response.Write(" ");
            Response.End();
        }
        lblMustType.Text = g.Cpcha.ReturnRequirement();
    }

    //Do authentication and redirect to the Control Panel or not.
    public void authAdmin(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        char[] delimiterChars = {'\\'};

        string[] words = txtUserName.Text.Split(delimiterChars);
        string Domain = "";
        string UserName = "";

        if (words.Length > 1)
        {
            Domain = words[0];
            UserName = words[1];
        }
        else UserName = words[0];

        if (!g.authAdmin(Domain, UserName, txtPassWord.Text)/* || !g.Cpcha.validateCode(txtCode.Text)*/)
            R2ContentHTML.InnerHtml = "Sorry, you cannot access!";
        else Response.Redirect("ControlPanel.aspx");
    }
}
