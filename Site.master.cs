using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Site : System.Web.UI.MasterPage
{
    /*public Site()
    {
        //fazer um loop e criar o numero de elementos para a quantidade de niveis de menu existentes.
        base.ContentPlaceHolders.Add("contentplaceholder100");
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        ContentPlaceHolder cph = new ContentPlaceHolder();
        cph.ID = "ContentPlaceHolder100";
        PlaceHolder1.Controls.Add(cph);
        ((ITemplate)base.ContentTemplates["ContentPlaceHolder100"]).InstantiateIn(cph);
    }*/

    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        /*
         * This is responsible for retrieve the url and fix the path of friendly url to files such as CSS,
         * and others.
         */
        StylePage.Href = g.ReturnProtocolAndPort() + "/" + StylePage.Href;
        StylePageMenuBase.Href = g.ReturnProtocolAndPort() + "/" + StylePageMenuBase.Href;
        StylePageMenuSide.Href = g.ReturnProtocolAndPort() + "/" + StylePageMenuSide.Href;
        StylePageMenuTop.Href = g.ReturnProtocolAndPort() + "/" + StylePageMenuTop.Href;
        AdminLink.HRef = g.ReturnProtocolAndPort() + AdminLink.HRef;

        /*StringBuilder cstext2 = new StringBuilder();
        cstext2.Append("<script type=\"text/javascript\"> function DoClick() {");
        cstext2.Append("Form1.Message.value='Text from client script.'} </");
        cstext2.Append("script>");
        cs.RegisterClientScriptBlock(cstype, csname2, cstext2.ToString(), false);*/

        /*
         * Prepare the javascript to be written in frontend page with absolute path fixed.
         */
        string JS = "<script type=\"text/javascript\" src=\"" + g.ReturnProtocolAndPort() + "/js/bannerRotator.js\" >"
            + "/***********************************************"
            + "* Created by: Scragar"
            + "* Posted: December 05, 2008"
            + "* http://javascript.internet.com/image-effects/banner-rotater.html"
            + "***********************************************/"
            + "</script>"

            + "<script type=\"text/javascript\" src=\"" + g.ReturnProtocolAndPort() + "/ddlevelsfiles/ddlevelsmenu.js\" >"
            + "/***********************************************"
            + "* All Levels Navigational Menu- (c) Dynamic Drive DHTML code library (http://www.dynamicdrive.com)"
            + "* This notice MUST stay intact for legal use"
            + "* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code"
            + "***********************************************/"
            + "</script>"

            + "<script type=\"text/javascript\" src=\"" + g.ReturnProtocolAndPort() + "/js/simpletreemenu.js\" >"
            + "/***********************************************"
            + "* Simple Tree Menu- © Dynamic Drive DHTML code library (www.dynamicdrive.com)"
            + "* This notice MUST stay intact for legal use"
            + "* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code"
            + "***********************************************/"
            + "</script>";

        //Title of page
        Page.Title = g.sTitle;

        //detect if the logo is Image or Flash and fit the html to display the correct tag;
        if (g.MediaType == "Image") 
            logo.InnerHtml = "<img src=\"" + g.ReturnProtocolAndPort() + "/images/" 
                + g.LogoImage + "\" alt=\"\" title=\"\" />";
        if (g.MediaType == "Flash") logo.InnerHtml = "<object width=\"100%\" height=\"100%\">"
            + "<param name=\"movie\" value=\"" + g.ReturnProtocolAndPort() + g.FolderInstalled + "/" + g.LogoImage 
            + "\"><embed src=\"" + g.ReturnProtocolAndPort() + g.FolderInstalled + "/" + g.LogoImage 
            + "\" width=\"100%\" height=\"100%\"></embed></object>";

        //if the web site is under maintenance, access it and display the message
        if (!g.EnableWebSite)
        {
            Response.Write(g.maintenance());
            Response.End();
        }

        //Create KeyWords and Description for each page that is displayed.
        HtmlMeta KeyWords = new HtmlMeta();
        HtmlHead head = (HtmlHead)Page.Header;

        KeyWords.Name = "Keywords";
        KeyWords.Content += g.Keywords;

        HtmlMeta Description = new HtmlMeta();
        Description.Name = "Description";
        Description.Content += g.Description;

        head.Controls.Add(Description);
        head.Controls.Add(KeyWords);

        //Prepare script to be written in the frontend page. This function is necessary for Banner Rotator
        string ScriptBannerRotator = @"function addLoadEvent(func) {"
            + "     var oldonload = window.onload;"
            + "     if (typeof window.onload != 'function') {"
            + "         window.onload = func;"
            + "     } else {"
            + "         window.onload = function() {"
            + "             if (oldonload) {"
            + "                 oldonload();"
            + "             }"
            + "             func();"
            + "         }"
            + "     }"
            + "}"
            + "var myBanner1 = new BannerRotator();";

        string SQL = "SELECT ml.*, m.* "
                + "FROM medialisttab ml, mediatab m "
                + "WHERE ml.idmedia=m.idmedia AND idmediagroup = " + g.idMediaGroup;//ID_Article

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        //Take the images and populate the script to do banner rotator.
        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while(result.Read())
                {
                    if (result["type"].ToString() == "Image")
                        ScriptBannerRotator += "myBanner1.add('', '" + g.ReturnProtocolAndPort() 
                            + g.FolderInstalled + "/images/" + result["filename"].ToString() + "');";
                }

                ScriptBannerRotator += "myBanner1.timer = 8;"
                + "addLoadEvent(function(){"
                + "     myBanner1.bind('banner1');"
                + "     myBanner1.startTimer();"
                + "});";

                //write the script ScriptBannerRotator in the frontend page.
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ScriptBannerRotator", ScriptBannerRotator, true);
            }
            else
            {
                //msg.InnerHtml = "We are sorry, we are experiencing technical problems ...";
            }

            result.Close();
            result.Dispose();
        }
        catch (OdbcException o)
        {
            //tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
        }
        //write the script JS in the page
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "JSMenus", JS, false);
    }
}
