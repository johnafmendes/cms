<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Config.aspx.cs" Inherits="admin_Config" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Web site Settings
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tConfig" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            Title:
            </td>
            <td>
            <asp:TextBox ID="txtTitle" Columns="30" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Logo:
            </td>
            <td>
            <asp:DropDownList ID="ddlLogo" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
                News at Front Page:
            </td>
            <td>
                <asp:TextBox ID="txtNumberNews" Columns="5" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Main Subject in Front Page:
            </td>
            <td>
            <asp:DropDownList ID="ddlSubject" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Header Subject:
            </td>
            <td>
            <asp:DropDownList ID="ddlSubjectHeader" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Footer Subject:
            </td>
            <td>
            <asp:DropDownList ID="ddlSubjectFooter" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Language:
            </td>
            <td>
            <asp:DropDownList ID="ddlLanguage" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Folder Installed: http://<% Response.Write(Request.ServerVariables["http_host"].ToString());%>/
            </td>
            <td>
            <asp:TextBox ID="txtFolderInstalled" Columns="30" runat="server"/>/
            </td>
        </tr>
        <tr>
            <td align="right">
            LDAP Url:
            </td>
            <td>
            <asp:TextBox ID="txtLDAPUrl" Columns="30" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            LDAP Context
            </td>
            <td>
            <asp:TextBox ID="txtLDAPContext" Columns="30" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
                Menu Style:
            </td>
            <td>
                <asp:DropDownList ID="ddlMenuStyle" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right" valign="top">
            Description:
            </td>
            <td>
            <asp:TextBox ID="txtDescription" TextMode="MultiLine" Rows="5" Columns="30" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right" valign="top">
            Keywords:
            </td>
            <td>
            <asp:TextBox ID="txtKeywords" TextMode="MultiLine" Rows="5" Columns="30" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Enabled Web Site:
            </td>
            <td>
            <asp:CheckBox ID="chbEnabled" Checked="true" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Upgrade CMS System: <asp:Label ID="lblVersion" Text="" />
            </td>
            <td>
            <asp:Button ID="btnUpgrade" runat="server" PostBackUrl="Upgrade.aspx" Text="Upgrade Now" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Config.aspx" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveConfig" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Settings</b>
    <br />
    Settings section is where you can amend relevant information that allow this CMS works properly.
    <br />
    <br />
    <b>Title:</b> - Title or name of your company. This is going to be displayed at title bar on browsers.
    <br />
    <br />
    <b>Logo:</b> - Logotype or simble that represent your company. It is going to be displayed in your Web Site.
    <br />
    <br />
    <b>News at Front Page:</b> - Number of links allowed to be displayed for user at front page.
    <br />
    <br />
    <b>Main Article in Front Page:</b> - Your front page can has just a article at front page. You can choose it selecting 
    in this option.
    <br />
    <br />
    <b>Header Article:</b> - This option might you set some article to be displayed as header of your web site.
    <br />
    <br />
    <b>Footer Article:</b> - This option might you set some article to be displayed as footer of your web site.
    <br />
    <br />
    <b>Folder Installed:</b> This option may the web site knows wich folder your web site is installed and do correctly 
    actions.
    <br />
    <br />
    <b>Menu Style:</b> - You can choose one in five type of menu to represent your menus.
    <br />
    <br />
    <b>Description:</b> - Writing a description of your web site is going to help your web site to be found by search engine
     as Google, Yahoo and others. This information will be displayed to users that typed specific keywords to find it.
    <br />
    <br />
    <b>Keywords:</b> - Writing keywords will improve your web site to be found on the Internet. You must think strategicly 
    and type all keywords necessary as you can that has corelation with your content or website.
    <br />
    <br />
    <b>Enabled Web Site:</b> You can enable or disable the access to your web site. Some times could you need some designer 
    layout and it could take some time, than, the best way could be take off your page.
</asp:Content>
