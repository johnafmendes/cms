<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Menu.aspx.cs" Inherits="admin_Menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Menu Management
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tUser" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidMenu" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Group Menu:
            </td>
            <td>
            <asp:DropDownList ID="ddlGroupMenu" runat="server" /> <asp:Button ID="btnFilterGroup" runat="server" OnCommand="SelectGroupMenu" Text="Filter" PostBackUrl="Menu.aspx?action=0" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Subject:
            </td>
            <td>
            <asp:DropDownList ID="ddlSubject" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Into Menu:
            </td>
            <td>
            <asp:DropDownList ID="ddlMenu" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Menu:
            </td>
            <td>
            <asp:TextBox ID="txtMenuName" Columns="30" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Position:
            </td>
            <td>
            <asp:TextBox ID="txtPosition" Columns="2" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Enabled:
            </td>
            <td>
            <asp:CheckBox ID="chbEnabled" Checked="true" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Visible:
            </td>
            <td>
            <asp:CheckBox ID="chbVisible" Checked="true" runat="server" />
            </td>
        </tr>
    </table>
    
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleListMenu" class="title4">Menu's List</div> 
    <asp:GridView ID="gvMenu" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idmenu"  
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        onrowdatabound="gvMenu_RowDataBound"
        OnRowCommand="gvMenu_RowCommand">
            <Columns>
                <asp:BoundField DataField="idMenu" HeaderText="ID" InsertVisible="False" SortExpression="idmenu" />
                <asp:BoundField DataField="Title" HeaderText="Menu" SortExpression="title" />
                <asp:BoundField DataField="SubMenu" HeaderText="Dependence" SortExpression="submenu" />
                <asp:BoundField DataField="GroupMenu" HeaderText="Group" SortExpression="GroupMenu" />
                <asp:BoundField DataField="Position" HeaderText="Position" SortExpression="Position" />
                <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" SortExpression="Enabled" ItemStyle-HorizontalAlign="Center" />
                <asp:CheckBoxField  DataField="Visible" HeaderText="Visible" SortExpression="Visible" ItemStyle-HorizontalAlign="Center" />
                <asp:ButtonField CommandName="deleteMenu" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        ProviderName="" 
        SelectCommand="SELECT m.idmenu, m.title, mg.title as groupmenu, ms.title as submenu, m.position, if(m.enabled, 'true', 'false') as enabled, if(m.visible, 'true', 'false') as visible FROM menutab AS ms RIGHT JOIN (menugrouptab as mg INNER JOIN menutab as m ON mg.idgroupmenu = m.idgroupmenu) ON ms.idmenu = m.idmenuparent">
    </asp:SqlDataSource>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Menu.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newMenu" ToolTip="New Menu" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Menu.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveMenu" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Menu Management</b>
    <br />
    Menus section is where you can add, amend, exclude, enable or disable the menus on front end side. Remember, 
    the menu must be added firstly and just after type the articles, because, each article at least one relation or 
    connection with one menu.
    <br />
    <br />
    <b>Into Menu</b> - Every menu that you create into menu "Main menu" is going to be inserted on first group of menu. 
    This menu sistem allow you to create a hierarchy menu with primary and secundary menus recursives.
    <br />
    <br />
    <b>Menu</b> - Name/Title of menu.
    <br />
    <br />
    <b>Position</b> - You can choose the position of each menu and set the sequence or order, e.g. if you want the 
    hipotetical menu called "About us" be the first, just type 0 and if you want the hipotetical menu called "Contact"
    be the last always, just type 999.
    <br />
    <br />
    <b>Enabled</b> - It allow you to enable or to disable the menu.
    <br />
    <br />
    <b>Visible</b> - A menu can be enable to allow articles be linked to this menu, but must be visible to be displayed
    at menu bars. Then, if you want create menus to be linked with articles but, do not want display this menu at menu bar, 
    you must set as unchecked this box. It is a good example if you want create a article to represent a footer or header
    of articles.    
</asp:Content>

