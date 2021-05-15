using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.Text;

/*using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;*/
using System.Text.RegularExpressions;

public partial class Default : System.Web.UI.Page
{
    public string CurrentIDLanguage = "1";
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //General g = Session["app"] as General;
        //g.FUrl.retrieveOriginalURL
        //g.FUrl.LastFURL
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;
        //General g = new General();

        if (g.DB!=null) g.getConfigWebSite();

        if (!g.EnableWebSite)
        {
            this.Title = g.sTitle;
            g.LogoImage = "1.gif";
            Response.Write(g.maintenance());
            Response.End();
        }

        string action="", idA="", idL="", idS="", keyword="";
        action = g.DB.cleanSQLInjection(Request.QueryString["action"] ?? "0"); //0 = front page
        idA = g.DB.cleanSQLInjection(Request.QueryString["idA"] ?? "0");//ID_Article
        idS = g.DB.cleanSQLInjection(Request.QueryString["idS"] ?? "0");//ID_Subject
        CurrentIDLanguage = idL = g.DB.cleanSQLInjection(Request.QueryString["idL"] ?? g.idLanguage);//ID_Language
        keyword = g.DB.cleanSQLInjection(Request.QueryString["keyword"] ?? txtSearch.Text);
        
        mainMediaHTML.InnerHtml = "";

        switch (action)
        {
            case "0": releaseContent(sender, e, true); homePage(sender, e, (g.idSubject.Length == 0 ? "0" : g.idSubject), idL); break; //Front Page   homePage(sender, e, (g.idSubject.Length==0 ? "0" : g.idSubject), idL);
            case "1": releaseContent(sender, e, false); idS = selectArticle(sender, e, idS, idL, idA, /*type,*/ mainContentHTML, true, false); break;//by idMenu
            case "2": releaseContent(sender, e, false); idS = selectArticle(sender, e, idS, idL, idA, /*type,*/ mainContentHTML, true, false); break;//by idArticle
            case "3": releaseContent(sender, e, false); buildLatestNews(sender, e, "all", idL); break;
            case "4": releaseContent(sender, e, false); buildSearch(sender, e, idL, keyword); break;
            //default: if (action != null) msg.InnerHtml = "We are sorry, we are experiencing technical problems ..."; break;
        }
        buildMainMenu(sender, e, idA, idL, idS);
        buildLatestNews(sender, e, "limited", idL);

        btnSearch.PostBackUrl = g.ReturnProtocolAndPort() + "/Default.aspx?action=4&idL=" + idL;

        if (g.idSubjectHeader.Length > 0)
        {
            string SQL = "SELECT * "
                + "FROM articletab "
                + "WHERE idsubject = " + g.idSubjectHeader.ToString()
                + " AND idlanguage = " + g.idLanguage.ToString();
                //+ " AND Enabled=1 ";

            OdbcCommand cmdHeader = g.DB.PrepareSQL(SQL);

            OdbcDataReader resultHeader = cmdHeader.ExecuteReader();

            if (resultHeader.HasRows)
                mainHeaderHTML.InnerHtml = g.Styles.title(g.FUrl.RelativePathToAbsolutePath(g.DB.HTMLDecode(resultHeader["Description"].ToString())), "5");

            resultHeader.Close();
            resultHeader.Dispose();
            cmdHeader.Dispose();
        }
        //btnSearch.ImageUrl = g.ReturnProtocolAndPort() + "/images/Search-16x16.png";
        languagesHTML.InnerHtml = getLanguages("", "0");
    }

    public void buildSearch(object sender, EventArgs e, string idL, string keyword)
    {
        releaseContent(sender, e, false);
        General g = Session["app"] as General;

        string SQL = "SELECT a.*, t.tag "
            + "FROM articletab a, tagtab t, articletagtab att "
            + "WHERE (a.title LIKE '%" + txtSearch.Text + "%' "
            + "OR a.description LIKE '%" + txtSearch.Text + "%' "
            + "OR t.tag LIKE '%" + keyword + "%') "
            + "AND a.enabled=1 "
            + "AND a.idlanguage=" + idL
            + " AND (a.expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
            + "OR a.expires = '0000/00/00 00:00:00') "
            + "AND a.idarticle = att.idarticle and t.idtag=att.idtag";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();

        if (result.HasRows)
        {
            mainContentHTML.InnerHtml = g.Styles.title("Results for: " + keyword + "<br/>" + g.Styles.dotLine+"<br/>","3");
            while (result.Read())
            {
                /*mainContentHTML.InnerHtml += "<div id=\"Div10\" class=\"square2\"></div><b><a href=\"Default.aspx?action=1&idS=" 
                    + result["idSubject"].ToString() + "&idL=" + idL + "&idA=" + result["idArticle"].ToString() 
                    + "\" alt=\"\">" + result["Title"].ToString() + "</a></b>"
                    + g.DB.HTMLDecode((result["Description"].ToString().Replace("../", "")).Substring(0, result["Description"].ToString().Length > 300 ? 300 : result["Description"].ToString().Length))
                    + "<br/>";*/
                mainContentHTML.InnerHtml += "<div id=\"Div10\" class=\"square2\"></div><b><a href=\""
                    + g.ReturnProtocolAndPort() + g.FUrl.retrieveFriendlyURL(result["idarticle"].ToString())
                    + "\" alt=\"\">" + result["title"].ToString() + "</a></b>"
                    + g.DB.HTMLDecode((result["description"].ToString().Replace("../", "")).Substring(0, result["Description"].ToString().Length > 300 ? 300 : result["description"].ToString().Length))
                    + "<br/>";
            }
        }
        else
        {
            mainContentHTML.InnerHtml = g.Styles.title("Results for: " + keyword + "<br/>" + g.Styles.dotLine + "<br/>", "3");
            mainContentHTML.InnerHtml += "We are sorry, we have not article to display!";
        }
        result.Close();
        result.Dispose();
        cmd.Dispose();
    }

    public void buildSearch2(object sender, EventArgs e)
    {
        releaseContent(sender, e, false);
        General g = Session["app"] as General;

        string SQL = "SELECT a.*, t.tag "
            + "FROM articletab a, tagtab t, articletagtab att "
            + "WHERE (a.title LIKE '%" + txtSearch.Text + "%' "
            + "OR a.description LIKE '%" + txtSearch.Text + "%' "
            + "OR t.tag LIKE '%" + txtSearch.Text + "%') "
            + "AND a.enabled=1 "
            + "AND a.idlanguage=" + CurrentIDLanguage
            + " AND (a.expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
            + "OR a.expires = '0000/00/00 00:00:00') "
            + "AND a.idarticle = att.idarticle and t.idtag=att.idtag";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();

        if (result.HasRows)
        {
            mainContentHTML.InnerHtml = g.Styles.title("Results for: " + txtSearch.Text + "<br/>" + g.Styles.dotLine + "<br/>", "3");
            while (result.Read())
            {
                /*mainContentHTML.InnerHtml += "<div id=\"Div10\" class=\"square2\"></div><b><a href=\"Default.aspx?action=1&idS=" 
                    + result["idSubject"].ToString() + "&idL=" + idL + "&idA=" + result["idArticle"].ToString() 
                    + "\" alt=\"\">" + result["Title"].ToString() + "</a></b>"
                    + g.DB.HTMLDecode((result["Description"].ToString().Replace("../", "")).Substring(0, result["Description"].ToString().Length > 300 ? 300 : result["Description"].ToString().Length))
                    + "<br/>";*/
                mainContentHTML.InnerHtml += "<div id=\"Div10\" class=\"square2\"></div><b><a href=\""
                    + g.ReturnProtocolAndPort() + g.FUrl.retrieveFriendlyURL(result["idarticle"].ToString())
                    + "\" alt=\"\">" + result["title"].ToString() + "</a></b>"
                    + g.DB.HTMLDecode((result["description"].ToString().Replace("../", "")).Substring(0, result["description"].ToString().Length > 300 ? 300 : result["Description"].ToString().Length))
                    + "<br/>";
            }
        }
        else
        {
            mainContentHTML.InnerHtml = g.Styles.title("Results for: " + txtSearch.Text + "<br/>" + g.Styles.dotLine + "<br/>", "3");
            mainContentHTML.InnerHtml += "We are sorry, we have not article to display!";
        }
        result.Close();
        result.Dispose();
        cmd.Dispose();
    }

    public string[] BuildMenus(object sender, EventArgs e, string RelSubMenu, string idMenuSelected, 
        string subMenu, int Item, int nLevel, string[] Results, string idL)
    {
        General g = Session["app"] as General;

        //int item=-1;
        string tmsg = "", sBeginIdent = "", sEndIdent = "", boldb = "", bolde = "", ClassMenuSelected="";
        
        int PreviousLevel = 0;

        /*string SQL = "SELECT * "
            + "FROM MenuTab ";
        if (subMenu.Length == 0) SQL += "WHERE idMenuParent is NULL ";
        else SQL += "WHERE idMenuParent =" + subMenu;
        SQL += " AND Enabled=1 "
             + "AND Visible=1 "
             + " ORDER BY Position";
        */

        string SQL = "SELECT mt.* "
            + "FROM menutab mt, menugrouptab mgt ";
            if (subMenu.Length == 0) SQL += "WHERE idmenuparent is NULL ";
            else SQL += "WHERE idmenuparent =" + subMenu;
            SQL += " AND mt.enabled=1 "
            + "AND mt.visible=1 ";
            SQL += " AND mgt.idlanguage=" + idL
            + " AND mgt.idgroupmenu=mt.idgroupmenu "
            + "ORDER BY mt.position";
        
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();
        if ((subMenu.Length == 0 && Item==-1) || RelSubMenu.Length > 0)
        {
            if (RelSubMenu.Length == 0)
            {
                Results[0] = "<ul id=\"TreeMenu\" class=\"treeview\">";

                if (idMenuSelected == "0") { boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected"; }
                else { ClassMenuSelected = "menuNonSelected"; }
                Results[0] += "<li><a href=\""+g.ReturnProtocolAndPort() + "/Default.aspx?action=0\" class=\"" 
                    + ClassMenuSelected + "\">" + boldb + "Home" + bolde + "</a></li>"; 
                    //<img src=\"images/1-Normal-Home-24x24.png\" title=\"Home\" style=\"margin-bottom:-7px;\" />
                boldb = ""; bolde = ""; ClassMenuSelected = "";
            }
            else Results[0] = "<ul id=\"" + RelSubMenu + "\" class=\"ddsubmenustyle\">";

            Item++;
            nLevel = 0;
        }
        else nLevel++;

        if (result.HasRows)
        {
            int recordCount = 0;
            string idArticle = "";
            try
            {
                while (result.Read())
                {
                    if (idMenuSelected == result["idmenu"].ToString() && RelSubMenu.Length == 0)
                    {
                        boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected";
                    }
                    else
                    {
                        ClassMenuSelected = "menuNonSelected";
                        boldb = "<b>"; 
                        bolde = "</b>";
                    }

                    recordCount++;
                    Item++;

                    if (subMenu.Length > 0 && (nLevel != PreviousLevel))
                    {
                        sBeginIdent = "<ul class=\"menu\">";
                        sEndIdent = "</ul>";
                        PreviousLevel = nLevel;
                        idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);
                        if(idArticle.Length > 0)
                            Results[0] += sBeginIdent + "<li><a href=\"" + g.ReturnProtocolAndPort()
                                + g.FUrl.retrieveFriendlyURL(idArticle)
                                + "\">" + boldb + result["title"].ToString() + bolde + "</a>";
                        else
                            Results[0] += sBeginIdent + "<li><a href=\"" + g.ReturnProtocolAndPort() 
                                + "/Default.aspx?action=2&idS=" + result["idsubject"].ToString() + "&idL=" + idL 
                                + "&idA=" + result["idmenu"].ToString() + "\">" + boldb + result["title"].ToString() 
                                + bolde + "</a>";

                        boldb = bolde = "";
                    }
                    else {
                        if (sBeginIdent.Length == 0)
                        {
                            sBeginIdent = ""; sEndIdent = "";
                        }
                        idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);
                        if (idArticle.Length > 0)
                            Results[0] += "<li><a href=\"" + g.ReturnProtocolAndPort()
                                + g.FUrl.retrieveFriendlyURL(idArticle)
                                + "\">" + boldb + result["title"].ToString() + bolde + "</a>";
                        else
                            Results[0] += "<li><a href=\"" + g.ReturnProtocolAndPort() 
                                + "/Default.aspx?action=2&idS=" + result["idsubject"].ToString() + "&idL=" + idL 
                                + "&idA=" + result["idmenu"].ToString() + "\">" + boldb + result["title"].ToString() 
                                + bolde + "</a>";
                        boldb = bolde = "";
                    }
                    if (nLevel != PreviousLevel)
                    {
                        idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);
                        if (idArticle.Length > 0)
                            Results[0] += "<li><a href=\"" + g.ReturnProtocolAndPort()
                                + g.FUrl.retrieveFriendlyURL(idArticle)
                                + "\">" + boldb + result["title"].ToString() + bolde + "</a>";
                        else
                            Results[0] += "<li><a href=\"" + g.ReturnProtocolAndPort() 
                                + "/Default.aspx?action=2&idS=" + result["idsubject"].ToString() + "&idL=" + idL 
                                + "&idA=" + result["idmenu"].ToString() + "\">" + boldb + result["title"].ToString() 
                                + bolde + "</a>";
                        boldb = bolde = "";
                    }
                    //if (idMenuSelected == result["idMenu"].ToString()) ddlMenu.SelectedIndex = Item;
                    Results[0] = BuildMenus(sender, e, "", idMenuSelected, result["idmenu"].ToString(), Item, nLevel, Results, idL)[0];
                    if (result.RecordsAffected == recordCount) Results[0] += sEndIdent;
                }
            }
            catch (Exception o)
            {
                tmsg = g.catchException(o, g.ErrorLevel);
            }
            finally
            {
                //msg.InnerHtml = tmsg;
                if (nLevel > 0 && (result.RecordsAffected == recordCount)) Results[0] += "</li>";
            }
        }
        else
        {
            //msg.InnerHtml = "No menu to show ...";
            Results[0] += "</li>";
        }
        result.Close();
        result.Dispose();
        cmd.Dispose();
        Results[1] = nLevel.ToString();
        return Results;
    }

    protected void buildMenuOriginal2(object sender, EventArgs e, string idA, string idL, string idS)
    {
        General g = Session["app"] as General;

        string SQL = "SELECT mt.* "
            + "FROM menutab mt, menugrouptab mgt "
            + "WHERE mt.idmenuparent is NULL "
            + "AND mt.enabled=1 "
            + "AND mt.visible=1 "        
            + " AND mgt.idlanguage=" + idL
            + " AND mgt.idgroupmenu=mt.idgroupmenu "
            + "ORDER BY mt.position";

        string boldb = "", bolde = "", ClassMenuSelected = "";
        string tMenu = "", tSubMenu = "", idSelected = idA;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);
        OdbcDataReader result = cmd.ExecuteReader();

        if (idA == "0") { boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected"; }
        else ClassMenuSelected = "menuNonSelected";
        tMenu = "<a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=0&idL=" + idL + "\" class=\"" 
            + ClassMenuSelected + "\">" + boldb + "Home" + bolde + "</a>"; 
        //<img src=\"images/1-Normal-Home-24x24.png\" title=\"Home\" style=\"margin-bottom:-7px;\" />
        boldb = ""; bolde = ""; ClassMenuSelected = "";

        if (result.HasRows)
        {
            while (result.Read())
            {
                SQL = "SELECT mt.* "
                    + "FROM menutab mt "
                    + "WHERE mt.idmenuparent = " + result["idmenu"].ToString()
                    + " AND mt.enabled=1 "
                    + "AND mt.visible=1 "
                    + "ORDER BY mt.position";

                OdbcCommand cmdSubMenu = g.DB.PrepareSQL(SQL);
                OdbcDataReader resultSubMenu = cmdSubMenu.ExecuteReader();

                string idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);

                List<string> ListMenu = retrieveIDListMenu(idS, idL, "0", new List<string>());

                //if (ListMenu.Count > 0)
                //{
                    foreach (string idM in ListMenu)
                    {
                        if (idM == result["idmenu"].ToString())
                        {
                            boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected";
                            break;
                        }
                    }
                //}//else boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected";
                

                if (idArticle.Length > 0)
                {                    
                    tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;" + boldb
                        + "<a href=\"" + g.ReturnProtocolAndPort()
                        + g.FUrl.retrieveFriendlyURL(idArticle)
                        + "\">" + result["title"].ToString() + "</a>" + bolde;
                }
                else
                {
                    tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;" + boldb
                    + "<a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=2&idL=" + idL + "&idS="
                    + result["idsubject"].ToString() + "&idA=" + result["idmenu"].ToString() + "\">"
                    + result["title"].ToString() + "</a>" + bolde;
                }

                if (resultSubMenu.HasRows)
                {
                    string[] menu = { "", "0" };
                    tSubMenu += BuildMenus(sender, e, "", idSelected, result["idmenu"].ToString(), -1, 0, menu, idL)[0] + "</ul>";
                }
            }
        }
        MainMenuContentHTML.InnerHtml = tMenu;
        R1TitleHTML.InnerHtml = "Sub Menus";
        R1ContentHTML.InnerHtml = tSubMenu;
    }

    protected List<string> retrieveIDListMenu(string idS, string idL, string idMenuParent, List<string> idMenusL )
    {
        General g = Session["app"] as General;
        //List<string> idMenusL = new List<string>();

        string SQL = "SELECT mt.* "
             + "FROM menutab mt, menugrouptab mgt ";
        if (idMenuParent != "0")
            SQL += "WHERE mt.idmenu = " + idMenuParent;
        else SQL += "WHERE mt.idsubject = " + idS;
            SQL += " AND mgt.idgroupmenu=mt.idgroupmenu"
            + " AND mgt.idlanguage = " + idL;

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();

        if(result.HasRows)
            if (result["idmenuparent"].ToString().Length > 0)
            {
                idMenusL.Add(result["idmenu"].ToString());
                idMenusL = retrieveIDListMenu(idS, idL, result["idmenuparent"].ToString(), idMenusL);
            }
            else idMenusL.Add(result["idmenu"].ToString());
        return idMenusL;
    }

    protected void buildMenuOriginal(object sender, EventArgs e, string id, string idL, string idS)
    {
        General g = Session["app"] as General;

        string tMenu = "", tSubMenu = "", idSelected = id, MidS="";
        string SQL = "SELECT mt.* "
             + "FROM menutab mt, menugrouptab mgt "
             + "WHERE mt.idmenuparent is NULL "
             + "AND mt.enabled=1 "
             + "AND mt.visible=1 ";
             //if (idS != "0") SQL += "AND mt.idSubject=" + idS;
             SQL += " AND mgt.idlanguage=" + idL
             + " AND mgt.idgroupmenu=mt.idgroupmenu "
             + "ORDER BY mt.position";

        string boldb = "", bolde = "", ClassMenuSelected="";
        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();

        if (id == "0" || idS == g.idSubject) { boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected"; }
        else ClassMenuSelected = "menuNonSelected";
        tMenu = "<a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=0&idL=" + idL + "\" class=\"" + ClassMenuSelected + "\">" + boldb + "Home" + bolde + "</a>"; //<img src=\"images/1-Normal-Home-24x24.png\" title=\"Home\" style=\"margin-bottom:-7px;\" />
        boldb = ""; bolde = ""; ClassMenuSelected = "";

        if (result.HasRows)
        {
            try
            {
                if (id != "0")
                {
                    string MainSubject = "0";
                    while (MainSubject.Length > 0)
                    {
                        /*SQL = "SELECT idMenuParent AS MainMenu "
                        + "FROM MenuTab mt, "
                        + "WHERE idMenu=" + id + " AND idMenuParent IS NOT NULL "
                        //+ "WHERE idSubject=" + idS
                        + " ORDER BY Position";
                         * */

                        SQL = "SELECT mt.idsubject as mainsubject "
                            + "FROM menutab mt, menugrouptab mgt "
                            + "WHERE mt.enabled=1 "
                            + "AND mt.visible=1 "
                            + "AND mt.idsubject=" + idS
                            + " AND mgt.idlanguage=" + idL
                        + " AND mgt.idgroupmenu=mt.idgroupmenu "
                        + "ORDER BY mt.position";

                        OdbcCommand cmd4 = g.DB.PrepareSQL(SQL);

                        OdbcDataReader result4 = cmd4.ExecuteReader();
                        if (result4.HasRows)
                        {
                            if (MidS != idS)
                            {
                                MidS = idS; idS = result4["mainsubject"].ToString(); MainSubject = MidS;
                            }
                            else MainSubject = "";
                        }
                        else MainSubject = "";
                        result4.Close();
                        result4.Dispose();
                        cmd4.Dispose();
                    }
                }

                while (result.Read())
                {
                    if (idS == result["idsubject"].ToString())
                    //if (id == result["idMenu"].ToString() || idS == MidS)
                    {
                        string idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);
                        boldb = "<b>"; bolde = "</b>"; ClassMenuSelected = "menuSelected";

                        if (idArticle.Length > 0)
                            tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;<a href=\""
                                + g.ReturnProtocolAndPort() + g.FUrl.retrieveFriendlyURL(idArticle)
                                + "\" class=\"" + ClassMenuSelected + "\">" + boldb + result["Title"].ToString() + bolde
                                + "</a>";
                        else
                            //tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;" + boldb + result["Title"].ToString() + bolde;
                            tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;<a href=\""
                                + g.ReturnProtocolAndPort() + "/Default.aspx?action=2&idL="
                                + idL + "&idS=" + result["idsubject"] + "&idA=" + result["idmenu"].ToString()
                                + "\" class=\"" + ClassMenuSelected + "\">" + boldb + result["Title"].ToString() + bolde
                                + "</a>";

                        boldb = bolde = ""; ClassMenuSelected = "menuNonSelected";

                        SQL = "SELECT * "
                        + "FROM menutab "
                        + "WHERE idmenuparent =" + result["idmenu"].ToString()
                        + " AND enabled=1 "
                        + "AND visible=1 "
                        + "ORDER BY position";

                        OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);
                        OdbcDataReader result2 = cmd2.ExecuteReader();
                        if (result2.HasRows)
                        {
                            string[] menu = { "", "0" };
                            tSubMenu += BuildMenus(sender, e, "", idSelected, result["idmenu"].ToString(), -1, 0, menu, idL)[0] + "</ul>";
                        }
                        else
                        {
                            ContentPlaceHolder c = (ContentPlaceHolder)Master.FindControl("R1Enabled");
                            c.Visible = false;
                        }
                        result2.Close();
                        result2.Dispose();
                        cmd2.Dispose();
                    }
                    else
                    {
                        string idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);
                        if (idArticle.Length > 0)
                            tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;" + boldb
                                + "<a href=\"" + g.ReturnProtocolAndPort()
                                + g.FUrl.retrieveFriendlyURL(idArticle)
                                + "\">" + result["title"].ToString() + "</a>" + bolde;
                        else
                            tMenu += "&nbsp;&nbsp;<span class=\"MenuDivisor\">&nbsp;</span>&nbsp;&nbsp;" + boldb
                            + "<a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=2&idL=" + idL + "&idS=" 
                            + result["idsubject"].ToString() + "&idA=" + result["idmenu"].ToString() + "\">" 
                            + result["title"].ToString() + "</a>" + bolde;
                    }
                }
            }
            catch (Exception o)
            {
                tSubMenu = "We are sorry, we are experiencing technical problems ...";
                tSubMenu += g.Styles.dotLine;
                tSubMenu += "<br/>Message: " + o.Message;
                tSubMenu += "<br/>Source: " + o.Source;
                tSubMenu += "<br/>Stack Trace: " + o.StackTrace;
                tSubMenu += "<br/>Target Site: " + o.TargetSite.Name;
                tSubMenu += "<br/>Others: " + o.InnerException;
                tSubMenu += g.Styles.dotLine;
                tSubMenu += "<br/>Try again! If the problem persist, contact techinical suport";
            }
            finally
            {
                MainMenuContentHTML.InnerHtml = tMenu;
                R1TitleHTML.InnerHtml = "Sub Menus";
                R1ContentHTML.InnerHtml = tSubMenu;
            }
        }
        else
        {
            MainMenuContentHTML.InnerHtml = "No menu to show! Try later ...";
        }
        result.Close();
        result.Dispose();
        cmd.Dispose();
    }

    protected void buildMainMenu(object sender, EventArgs e, string idA, string idL, string idS)
    {
        General g = Session["app"] as General;
        string[] menu = { "", "0" };

        switch (g.MenuStyle) 
        {
            case "Original": 
                buildMenuOriginal2(sender, e, idA, idL, idS); 
                //MainMenuContentHTML.InnerHtml = BuildTopORSideMenuBar(sender, e, id, "Original");
                break;
            case "Tree":
                JavaScriptAtEndPage.InnerHtml = "<script type=\"text/javascript\">"
                    + "ddtreemenu.createTree(\"TreeMenu\", true) "
                    + "</script>";

                MainMenuContentHTML.InnerHtml += BuildMenus(sender, e, "", idA, "", -1, 0, menu, idL)[0] + "</ul>";
                ((ContentPlaceHolder)Master.FindControl("R1Enabled")).Visible = false;
                break;
            
            case "Top Bar": 
                JavaScriptAtEndPage.InnerHtml = "<script type=\"text/javascript\">"
                + "ddlevelsmenu.setup(\"MenuBar\", \"topbar\")"
                + "</script>";
                //ddlevelsmenu.setup("mainmenuid", "topbar|sidebar")                
                MainMenuContentHTML.InnerHtml = BuildTopORSideMenuBar(sender, e, idA, "topbar", idL);
                break;

            case "Side Bar":
                JavaScriptAtEndPage.InnerHtml = "<script type=\"text/javascript\">"
                + "ddlevelsmenu.setup(\"MenuBar\", \"sidebar\")"
                + "</script>";
                //ddlevelsmenu.setup("mainmenuid", "topbar|sidebar")
                MainMenuContentHTML.InnerHtml = BuildTopORSideMenuBar(sender, e, idA, "sidebar", idL);
                break;
        }
    }

    protected string BuildTopORSideMenuBar(object sender, EventArgs e, string id, string typeMenu, string idL)
    {
        General g = Session["app"] as General;
        string[] menu = { "", "0" };
        string SQL = "", boldb = "", bolde = "", sMenu = "", sSubMenu = "";

        /*SQL = "SELECT * "
            + "FROM menutab "
            + "WHERE sidmenu is NULL "
            + "AND Enabled=1 "
            + "AND Visible=1 "
            + "ORDER BY Position";
        */

        SQL = "SELECT mt.* "
            + "FROM menutab mt, menugrouptab mgt "
            + "WHERE mt.idmenuparent is NULL "
            + "AND mt.enabled=1 "
            + "AND mt.visible=1 ";
        SQL += " AND mgt.idlanguage=" + idL
            + " AND mgt.idgroupmenu=mt.idgroupmenu "
            + "ORDER BY mt.position";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();

        if (result.HasRows)
        {
            if (typeMenu == "topbar" || typeMenu == "Original")
                sMenu = "<div id=\"MenuBar\" class=\"mattblackmenu\"><ul>";
            else sMenu = "<div id=\"MenuBar\" class=\"markermenu\"><ul>";

            sMenu += "<li><a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=0&idL=" + idL + "\">Home</a></li>";
            int item = 0;
            while (result.Read())
            {
                string RelSubMenu = "";
                item++;

                SQL = "SELECT * "
                + "FROM menutab "
                + "WHERE idmenuparent = " + result["idmenu"].ToString()
                + " AND enabled=1 "
                + " AND visible=1";

                OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);
                OdbcDataReader result2 = cmd2.ExecuteReader();
                if (result2.HasRows)
                {
                    RelSubMenu = "SubMenuBar" + item.ToString();
                    sSubMenu += BuildMenus(sender, e, RelSubMenu, id, result["idmenu"].ToString(), -1, 0, menu, idL)[0] + "</ul>";
                }

                string idArticle = RetrieveIDArticle(result["idsubject"].ToString(), idL);
                if (idArticle.Length > 0)
                    sMenu += "<li><a href=\"" + g.ReturnProtocolAndPort()
                        + g.FUrl.retrieveFriendlyURL(idArticle)
                        + "\" rel=\"" + RelSubMenu + "\">" + boldb + result["title"].ToString() + bolde
                        + "</a></li>";
                else
                    sMenu += "<li><a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=2&idA=" + result["idmenu"].ToString() + "&idL=" + idL + "&idS=" + result["idsubject"].ToString()
                        + "\" rel=\"" + RelSubMenu + "\">" + boldb + result["title"].ToString() + bolde
                        + "</a></li>";

                result2.Close();
                result2.Dispose();
                cmd2.Dispose();
            }
            sMenu += "</ul></div>";
        }

        result.Close();
        result.Dispose();
        cmd.Dispose();
        ((ContentPlaceHolder)Master.FindControl("R1Enabled")).Visible = false;
        return sMenu + sSubMenu;
    }

    protected void homePage(object sender, EventArgs e, string idS, string idL)
    {
        //buildLatestNews(sender, e, "limited", idL);
        General g = Session["app"] as General;
        //selectArticle(sender, e, idS, idL, "0", "3", mainContentHTML, false, false);
        selectArticle(sender, e, idS, idL, "0", mainContentHTML, false, false);
    }

    protected void buildLatestNews(object sender, EventArgs e, string type, string idL)
    {
        General g = Session["app"] as General;
        if (type == "limited")
        {
            R2TitleHTML.InnerHtml = "Latest News";
            string SQL = "SELECT * "
                + "FROM articletab "
                + "WHERE type='News' "
                + "AND enabled=1 "
                + " AND idlanguage=" + idL
                + " AND (expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
                + "OR expires = '0000/00/00 00:00:00') "
                + "LIMIT " + g.NumberNews;

            OdbcCommand cmd = g.DB.PrepareSQL(SQL);
            OdbcDataReader result = cmd.ExecuteReader();
            
            R2ContentHTML.InnerHtml = "";
            if (result.HasRows)
            {
                //while (result.Read()) R2ContentHTML.InnerHtml += "<div id=\"Div10\" class=\"square2\"></div><a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=1&idA=" + result["idArticle"].ToString() + "\" alt=\"\">" + result["Title"].ToString() + "</a><br/>";
                while (result.Read()) R2ContentHTML.InnerHtml += "<div id=\"Div10\" class=\"square2\"></div><a href=\"" + g.ReturnProtocolAndPort() + g.FUrl.retrieveFriendlyURL(result["idarticle"].ToString()) + "\" alt=\"\">" + result["title"].ToString() + "</a><br/>";
                R2ContentHTML.InnerHtml += "<a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=3\" alt=\"\">More news ...</a><br/>";
            }
            else R2ContentHTML.InnerHtml = "We are sorry, we have not News to display!";

            result.Close();
            result.Dispose();
            cmd.Dispose();
        }
        if (type == "all")
        {
            string SQL = "SELECT * "
                + "FROM articletab "
                + "WHERE type='News' "
                + "AND idlanguage=" + idL
                + " AND (expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
                + "OR expires = '0000/00/00 00:00:00') "
                + "AND enabled=1 ";

            OdbcCommand cmd = g.DB.PrepareSQL(SQL);
            OdbcDataReader result = cmd.ExecuteReader();
            
            if (result.HasRows)
            {
                mainContentHTML.InnerHtml = "<h2><b>News</b></h2>";
                /*while (result.Read()) mainContentHTML.InnerHtml += "<ul><li><b><a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=1&idL="
                    +idL+"&idS="+result["idSubject"].ToString()+"&idA=" + result["idArticle"].ToString() 
                    + "\" alt=\"\">" + result["Title"].ToString() + "</a></b></li></ul>" 
                    + g.DB.HTMLDecode((result["Description"].ToString().Replace("../", "")).Substring(0, result["Description"].ToString().Length > 300 ? 300 : result["Description"].ToString().Length)) 
                    + "...<br/>";*/
                while (result.Read()) mainContentHTML.InnerHtml += "<ul><li><b><a href=\""
                    + g.ReturnProtocolAndPort() + g.FUrl.retrieveFriendlyURL(result["idarticle"].ToString())
                    + "\" alt=\"\">" + result["title"].ToString() + "</a></b></li></ul>"
                    + g.DB.HTMLDecode((result["description"].ToString().Replace("../", "")).Substring(0, result["description"].ToString().Length > 300 ? 300 : result["description"].ToString().Length))
                    + "...<br/>";
                mainContentHTML.InnerHtml += "<br/><br/>";
            }
            else mainContentHTML.InnerHtml = "We are sorry, we have not News to display!";

            result.Close();
            result.Dispose();
            cmd.Dispose();
        }
    }

    private void releaseContent(object sender, EventArgs e, bool FrontPage)
    {
        ((ContentPlaceHolder)Master.FindControl("MainContent")).Visible = true;
        if (FrontPage)
        {
            ((ContentPlaceHolder)Master.FindControl("R1Enabled")).Visible = false;
            ((ContentPlaceHolder)Master.FindControl("R2Enabled")).Visible = true;
            ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = true;
            ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = false;
        }
        else
        {
            ((ContentPlaceHolder)Master.FindControl("R1Enabled")).Visible = true;
            ((ContentPlaceHolder)Master.FindControl("R2Enabled")).Visible = true;
            ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = true;
            ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = false;
        }
    }

    public string selectArticle(object sender, EventArgs e, string idS, string idL, string idA, 
        System.Web.UI.HtmlControls.HtmlGenericControl div, bool RelatedArticles, bool Recursive)
    {
        General g = Session["app"] as General;

        //string t = "";
        //if (type == "1") t = "idArticle"; //by Article
        //if (type == "2") t = "idMenu"; //by Menu
        //if (type == "3") t = "idSubject"; //by Subject

        //considering caming by idSubject and idLanguage
        string SQL = "SELECT a.*, u.surname, u.name, t.tag "
            + "FROM articletab a, usertab u, tagtab t, articletagtab att "
            + "WHERE a.idsubject=" + idS;
        if (Int32.Parse(idA) > 0) SQL += " AND a.idarticle=" + idA;

            SQL += " AND (a.expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
            + "OR a.expires = '0000/00/00 00:00:00') "
            + "AND a.idarticle = att.idarticle "
            + "AND t.idtag=att.idtag "
            + "AND a.iduser=u.iduser "
            + "AND a.enabled=1 "
            + "AND a.idlanguage="+idL;

        string idSubject = "", sTags="", idMainMedia="", idMediaGroup="";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        OdbcDataReader result = cmd.ExecuteReader();

        if (result.HasRows)
        {
            //add part of body/description from current article to general description
            HtmlHelper h = new HtmlHelper();

            string space = "";
            if (g.Description.Length > 0) space = ". ";

            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            g.Description += reg.Replace(result["Title"].ToString() + ". "
                + result["description"].ToString().Replace("../", ""), "");
            g.Description = h.StripHTML(g.Description);
            g.Description = h.HtmlStripTags(g.Description, true, true);
            
            string NewText;
            do
            {
                NewText = g.Description;
                g.Description = g.Description.Replace("  ", " ");
            } while (g.Description != NewText);

            g.Description = (g.Description.Substring(0, g.Description.Length > 300 ? 300 : g.Description.Length)).Trim();

            idMainMedia = result["idmainmedia"].ToString();
            idMediaGroup = result["idmediagroup"].ToString();
            g.idMediaGroup = idMediaGroup;
            if (idMediaGroup.Length > 0) ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = true;
            idSubject = result["idsubject"].ToString();
            String idArticle = result["idarticle"].ToString();

            SQL = "SELECT * "
            + "FROM mediatab WHERE idmedia = " + idMainMedia;

            OdbcCommand cmdMedia = g.DB.PrepareSQL(SQL);
            try
            {
                OdbcDataReader rMedia = cmdMedia.ExecuteReader();

                if (rMedia.HasRows) mainMediaHTML.InnerHtml = "<img src=\""+g.ReturnProtocolAndPort()+"/images/"
                    +rMedia["filename"].ToString()+"\" alt=\"\" title=\"\">";
                rMedia.Close();
                rMedia.Dispose();
            }
            catch (OdbcException oe) { }
            cmdMedia.Dispose();

            if (result["enablenews"].ToString() == "1") buildLatestNews(sender, e, "limited", idL);
            else
            {
                ContentPlaceHolder c = (ContentPlaceHolder)Master.FindControl("R2Enabled");
                c.Visible = false;
            }

            if (result["enabledetails"].ToString() == "1" && RelatedArticles)
            {
                div.InnerHtml = g.Styles.title(g.DB.HTMLDecode(result["title"].ToString()), "1");
                div.InnerHtml += g.Styles.title("By: " + result["surname"].ToString() + ", " + result["name"].ToString(), "6");
                div.InnerHtml += g.Styles.title("Posted: " + result["published"].ToString(), "6");
                div.InnerHtml += g.Styles.title("Last update: " + result["lastupdate"].ToString(), "6");
                div.InnerHtml += g.Styles.title("Languages: " + getLanguages(idS, "1"), "6");
                div.InnerHtml += g.Styles.title(g.Styles.dotLine, "6");
            }
            else if (result["enabledetails"].ToString() != "0") div.InnerHtml = g.Styles.title(g.DB.HTMLDecode(result["title"].ToString()), "1");

            div.InnerHtml += g.Styles.title(g.FUrl.RelativePathToAbsolutePath(g.DB.HTMLDecode(result["description"].ToString())) + "<br/>", "5");
            //div.InnerHtml += g.Styles.title(g.DB.HTMLDecode(result["Description"].ToString().Replace("../", "")) + "<br/>", "5");

            if (RelatedArticles && idS!=g.idSubject)
            {
                while (result.Read())
                    if (idArticle == result["idarticle"].ToString()) sTags += "<a href=\"" + g.ReturnProtocolAndPort() + "/Default.aspx?action=4&idS=" + result["idsubject"].ToString() + "&idL=" + idL + "&idA=" + result["idarticle"].ToString() + "&keyword=" + result["tag"].ToString() + " \" alt=\"\">" + result["tag"].ToString() + "</a>, ";

                if (sTags.Trim().EndsWith(",")) sTags = sTags.Remove(sTags.Length - 2, 2);
                //add tags from current article to general keywords
                space = "";
                if (g.Keywords.Length > 0) space = ", ";
                g.Keywords += space + sTags;

                div.InnerHtml += g.Styles.title("<b>Tags:</b> " + sTags + "<br/><br/><br/>", "6");

                SQL = "SELECT * "
                + "FROM articletab "
                + "WHERE idsubject=" + idSubject
                + " AND (expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString())
                + "' OR expires = '0000/00/00 00:00:00') "
                + "AND enabled=1 "
                + "AND idlanguage=" + idL;

                OdbcCommand cmd2 = g.DB.PrepareSQL(SQL);
                OdbcDataReader result2 = cmd2.ExecuteReader();

                if (result2.HasRows)
                {
                    div.InnerHtml += g.Styles.title("<b>Related Articles</b>", "5");
                    div.InnerHtml += g.Styles.dotLine;

                    while (result2.Read())
                    {
                        div.InnerHtml += "<br/><div id=\"Div10\" class=\"square2\"></div><b>"
                        + "<a href=\"" + g.ReturnProtocolAndPort()
                        + g.FUrl.retrieveFriendlyURL(result2["idarticle"].ToString())+ "\" alt=\"\">"
                        + result2["Title"].ToString() + "</a></b><br/>";

                        string Description = result2["description"].ToString();
                        
                        Regex reg2 = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
                        Description = reg2.Replace(Description, "");

                        Description = h.StripHTML(Description);
                        Description = h.HtmlStripTags(Description, true, true);
                        
                        string NewText2="";
                        do
                        {
                            NewText2 = Description;
                            Description = Description.Replace("  ", " ");
                        } while (Description != NewText2);

                        Description = (Description.Substring(0, Description.Length > 300 ? 300 : Description.Length)).Trim();
                        div.InnerHtml += g.DB.HTMLDecode(Description);
                    }
                }
                result2.Close();
                result2.Dispose();
                cmd2.Dispose();
            }
            else languagesHTML.InnerHtml = getLanguages(idS, "0");

            if (g.idSubjectFooter.Length > 0)
            {
                SQL = "SELECT * "
                    + "FROM articletab "
                    + "WHERE idsubject = " + g.idSubjectFooter.ToString()
                    + " AND idlanguage = " + g.idLanguage.ToString()
                    + " AND enabled=1 ";

                OdbcCommand cmdFooter = g.DB.PrepareSQL(SQL);

                OdbcDataReader resultFooter = cmdFooter.ExecuteReader();

                if (resultFooter.HasRows)
                    mainFooterHTML.InnerHtml = g.Styles.title(g.FUrl.RelativePathToAbsolutePath(g.DB.HTMLDecode(resultFooter["Description"].ToString())), "5");

                resultFooter.Close();
                resultFooter.Dispose();
                cmdFooter.Dispose();
            }
        }
        else
        {
            //need write a message = We have the same article in others languages.
            //e listar todas as linguas possiveis.
            div.InnerHtml = "We are sorry, we have not article to display, or, we have not article in your language to "
            + "display.<br/>Select other language to read it if there is. <br/><br/>";

            if(!Recursive) selectArticle(sender, e, idS, g.idLanguage, idA, /*type,*/ mainContentHTML, true, true);

            idSubject = idS;
        }

        result.Close();
        result.Dispose();
        cmd.Dispose();
        return idSubject;
    }

    private string getLanguages(string idS, string action)
    {
        General g = Session["app"] as General;
        string Languages = "", SQL = "";

        if (idS.Length == 0)
        {
            SQL = "SELECT * FROM languagetab as lt, mediatab as mt "
                + "WHERE lt.idmedia=mt.idmedia";
        }
        else
        {
            SQL = "SELECT at.idarticle, at.idsubject, lt.idlanguage, lt.abreviation, "
                + "lt.title, mt.filename "
                + "FROM articletab as at, languagetab as lt, mediatab as mt "
                + "WHERE at.idsubject=" + idS
                + " AND (at.expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
                + "OR at.expires = '0000/00/00 00:00:00') "
                + "AND at.enabled=1 "
                + "AND at.idlanguage=lt.idlanguage "
                + "AND lt.idmedia=mt.idmedia";
        }

        OdbcCommand cmdLanguages = g.DB.PrepareSQL(SQL);

        OdbcDataReader resultLanguages = cmdLanguages.ExecuteReader();

        if (resultLanguages.HasRows)
        {
            while (resultLanguages.Read())
            {
                string url = "";

                if (action == "0") url = g.ReturnProtocolAndPort() + "/Default.aspx?action=" + action + "&idL=" 
                    + resultLanguages["idlanguage"].ToString();
                else /*url = g.ReturnProtocolAndPort() + "/Default.aspx?action=" + action + "&idA=" 
                    + resultLanguages["idArticle"].ToString() + "&idS=" + resultLanguages["idSubject"].ToString() 
                    + "&idL=" + resultLanguages["idLanguage"].ToString();*/
                    url = g.ReturnProtocolAndPort() + g.FUrl.retrieveFriendlyURL(resultLanguages["idarticle"].ToString());

                Languages += "<a href=\"" + url + "\" title=\"\" class=\"\"><img src=\""+g.ReturnProtocolAndPort()
                    + "/images/" + resultLanguages["filename"].ToString() + "\" alt=\"" 
                    + resultLanguages["abreviation"].ToString() + " - " + resultLanguages["title"].ToString() 
                    + "\" title=\"" + resultLanguages["abreviation"].ToString() + " - " 
                    + resultLanguages["title"].ToString() + "\" class=\"\" /></a>";
            }
        }
        return Languages;
    }

    public string RetrieveIDArticle(string IDSubject, string idLanguage)
    { 
        General g = Session["app"] as General;

        string idArticle="";
        string SQL = "SELECT a.*, u.surname, u.name, t.tag "
            + "FROM articletab a, usertab u, tagtab t, articletagtab att "
            + "WHERE a.idsubject=" + IDSubject
            + " AND (a.expires>='" + g.MakeMySQLDateTime(System.DateTime.Now.ToString()) + "' "
            + "OR a.expires = '0000/00/00 00:00:00') "
            + "AND a.idarticle = att.idarticle "
            + "AND t.idtag=att.idtag "
            + "AND a.iduser=u.iduser "
            + "AND a.enabled=1 "
            + "AND a.idlanguage="+idLanguage;

        OdbcCommand cmdArticle = g.DB.PrepareSQL(SQL);

        OdbcDataReader resultArticle = cmdArticle.ExecuteReader();

        if (resultArticle.HasRows)
        {
            idArticle = resultArticle["idarticle"].ToString();
        }
        resultArticle.Close();
        resultArticle.Dispose();
        cmdArticle.Dispose();

        return idArticle;
    }
}