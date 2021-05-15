<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="ControlPanel.aspx.cs" Inherits="admin_ControlPanel" %>

<asp:Content ID="Content8" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Control Panel - Menu
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
        <table border="0" cellpadding="5" cellspacing="5" id="tMenu" width="100%">
            <tr align="center" valign="middle">
                <td align="center">
                    <a href="Users.aspx?action=0" runat="server" id="linkUsers"><img src="../images/Fermer-SZ-48x48.png" alt=""/><br/>Users</a>
                </td>
                <td align="center">
                    <a href="Menu.aspx?action=0" runat="server" id="linkMenu"><img src="../images/4directions-SZ-48x48.png" alt=""/><br/>Menus</a>
                </td>
                <td align="center">
                    <a href="Tags.aspx?action=0" runat="server" id="linkTags"><img src="../images/Isis-Draw-SZ-48x48.png" alt=""/><br/>Tags</a>
                </td>
            </tr>
            <tr align="center" valign="middle">
                <td align="center">
                    <a href="GroupMenu.aspx?action=0" runat="server" id="linkGroupMenu"><img src="../images/4directions-SZ-48x48.png" alt=""/><br/>Group of Menus</a>
                </td>
                <td align="center">
                    <a href="GroupMedia.aspx?action=0" runat="server" id="linkGroupMedia"><img src="../images/Divers-SZ-48x48.png" alt=""/><br/>Group of Media</a>
                </td>
                <td align="center">
                    <a href="Languages.aspx?action=0" runat="server" id="linkLanguages"><img src="../images/config-48x48.png" alt=""/><br/>Languages</a>
                </td>
            </tr>
            <tr align="center" valign="middle">
                <td align="center">
                    <a href="Article.aspx?action=0" runat="server" id="linkArticles"><img src="../images/Brush-SZ-48x48.png" alt=""/><br/>Articles</a>
                </td>
                <td align="center">
                    <a href="Media.aspx?action=0" runat="server" id="linkMedia"><img src="../images/Divers-SZ-48x48.png" alt=""/><br/>Media</a>
                </td>
                <td align="center">
                    <a href="Config.aspx?action=0" runat="server" id="linkSettings"><img src="../images/config-48x48.png" alt=""/><br/>Settings</a>
                </td>
            </tr>
            <tr align="center" valign="middle">
                <td align="center">
                    <a href="Subject.aspx?action=0" runat="server" id="linkSubject"><img src="../images/Brush-SZ-48x48.png" alt=""/><br/>Subjects</a>
                </td>
                <td align="center">
                    
                </td>
                <td align="center">
                    
                </td>
            </tr>
        </table>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="R1Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R2Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel</b>
    <br />
    You can click on any icon/link to access specific part of this CMS.
    <br />
    <br />
    <b>Users</b> - Users section is where you can add, amend, exclude, enable or disable users that is going
    to access Control Panel - Menu.
    <br />
    <br />
    <b>Menus</b> - Menus section is where you can add, amend, exclude, enable or disable the menus on front end
    side. Remember, the menu must be added firstly and just after type the articles, because, each article at least
    one relation or connection with one menu.
    <br />
    <br />
    <b>Tags</b> - Tags section is where you can add, amend, exclude, enable or disable the tags wich are added inside article. 
    This tags works like a keyworks, then, it is very important to help articles to be founded using search on
    front end side.
    <br />
    <br />
    <b>Articles</b> - Articles section is where you can add, amend, exclude, enable or disable the articles or pages 
    that are going to be accessed by users at front end side. For you add a new article or page, first you must 
    add some menus on menu section and just after add a new content.
    <br />
    <br />
    <b>Media</b> - Media section is where you can add, amend or exclude a select range of kind of media that can be added
    inside of articles, set as logo of this CMS or simplicity give a way to upload some files like .zip or .doc to be 
    linked inside of content on articles to be downloaded after.
    <br />
    <br />
    <b>Settings</b> - Settings section is where you can amend relevant information that allow this CMS works properly.
    <br />
    <br />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainMenu" Runat="Server">
    <br />
</asp:Content>