/* CLASS NAME: Admin/ControlPanel.aspx.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is manage The Control Panel.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_ControlPanel : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        if (g.Auth.Status == false)
        {
            Session.Abandon();
            Response.Redirect("Default.aspx");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        g.currentPage = "Panel";

        ((ContentPlaceHolder)Master.FindControl("RSMainMenu")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R1Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R2Enabled")).Visible = true;
        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;

        if (!g.Auth.CheckPermission(g.Auth.idUser, 1, "SelectData")) linkUsers.Visible = false; //Users
        if (!g.Auth.CheckPermission(g.Auth.idUser, 2, "SelectData")) linkMenu.Visible = false; //Menus
        if (!g.Auth.CheckPermission(g.Auth.idUser, 5, "SelectData")) linkTags.Visible = false; //Tags
        if (!g.Auth.CheckPermission(g.Auth.idUser, 7, "SelectData")) linkGroupMedia.Visible = false; //Group of Media
        if (!g.Auth.CheckPermission(g.Auth.idUser, 8, "SelectData")) linkLanguages.Visible = false; //Languages
        if (!g.Auth.CheckPermission(g.Auth.idUser, 4, "SelectData")) linkArticles.Visible = false; //Articles
        if (!g.Auth.CheckPermission(g.Auth.idUser, 3, "SelectData")) linkMedia.Visible = false; //Media
        if (!g.Auth.CheckPermission(g.Auth.idUser, 6, "SelectData")) linkSettings.Visible = false; //Settings
        if (!g.Auth.CheckPermission(g.Auth.idUser, 9, "SelectData")) linkGroupMenu.Visible = false; //Group of Menus
        if (!g.Auth.CheckPermission(g.Auth.idUser, 10, "SelectData")) linkSubject.Visible = false; //Subject
    }
}
