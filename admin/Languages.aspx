<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Languages.aspx.cs" Inherits="admin_Languages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Language Management
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tUser" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidLanguage" runat="server" Text="" />
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
            Abreviation:
            </td>
            <td>
            <asp:TextBox ID="txtAbreviation" Columns="30" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Flag:
            </td>
            <td>
            <asp:DropDownList ID="ddlFlag" runat="server"/>
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
    <div id="mainTitlePermissions" class="title4">Language's List</div>
    <asp:GridView ID="gvLanguage" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idlanguage"
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        OnRowDataBound="gvLanguage_RowDataBound"
        OnRowCommand="gvLanguage_RowCommand">
            <Columns>
                <asp:BoundField DataField="idLanguage" HeaderText="ID" InsertVisible="False" SortExpression="idlanguage" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="title" />
                <asp:BoundField DataField="Abreviation" HeaderText="Abreviation" SortExpression="abreviation" />
                <asp:BoundField DataField="Flag" HeaderText="Flag" ItemStyle-HorizontalAlign="Center" />
                <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" SortExpression="Enabled" ItemStyle-HorizontalAlign="Center" />
                <asp:ButtonField CommandName="deleteLanguage" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        ProviderName="" 
        SelectCommand="SELECT l.idlanguage, l.title, l.abreviation, l.idmedia, if(l.enabled, 'true', 'false') as enabled, m.filename as flag FROM languagetab as l, mediatab as m WHERE l.idmedia=m.idmedia">
    </asp:SqlDataSource>
    <br />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Languages.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newLanguage" ToolTip="New Language" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Languages.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveLanguage" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R4Content" Runat="Server">
<b>Control Panel/Language Management</b>
    <br />
    .
    <br />
    <br />
    <b>Title</b> - .
    <br />
    <br />
    <b>Abreviation</b> - .
    <br />
    <br />
    <b>Flag</b> - .
    <br />
    <br />
</asp:Content>
