<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="GroupMenu.aspx.cs" Inherits="admin_GroupMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
Group Menu Management
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tUser" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidGroupMenu" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Title:
            </td>
            <td>
            <asp:TextBox ID="txtTitle" Columns="30" runat="server" />
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
            Enabled:
            </td>
            <td>
            <asp:CheckBox ID="chbEnabled" Checked="true" runat="server" />
            </td>
        </tr>
    </table>
    
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleListMenu" class="title4">Group Menu's List</div> 
    <asp:GridView ID="gvGroupMenu" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idgroupmenu"  
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        onrowdatabound="gvGroupMenu_RowDataBound"
        OnRowCommand="gvGroupMenu_RowCommand">
            <Columns>
                <asp:BoundField DataField="idGroupMenu" HeaderText="ID" InsertVisible="False" SortExpression="idgroupmenu" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="title" />
                <asp:BoundField DataField="Language" HeaderText="Language" SortExpression="language" />                
                <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" SortExpression="Enabled" ItemStyle-HorizontalAlign="Center" />
                <asp:ButtonField CommandName="deleteGroupMenu" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        ProviderName="" 
        SelectCommand="SELECT gm.idgroupmenu, gm.title, if(gm.enabled, 'true', 'false') as enabled, l.title as language FROM menugrouptab gm, languagetab l WHERE gm.idlanguage = l.idlanguage">
    </asp:SqlDataSource>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="GroupMenu.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newGroupMenu" ToolTip="New Menu" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="GroupMenu.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveGroupMenu" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Group Menu Management</b>
    <br />
    .
    <br />
    <br />
    <b>Title</b> - .
    <br />
    <br />
    <b>Language</b> - .
    <br />
    <br />
    <b>Enabled</b> - .
    <br />
    <br />
</asp:Content>

