<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Subject.aspx.cs" Inherits="admin_Subject" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Subject Management
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tSubject" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidSubject" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Title:
            </td>
            <td>
            <asp:TextBox ID="txtTitle" Text="" Columns="78" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right" valign="top">
            Description:
            </td>
            <td>            
            <asp:TextBox ID="txtDescription" Wrap="true" Columns="60" Rows="5" TextMode="MultiLine" Text="" runat="server"/>
            </td>
        </tr>
    </table>

    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleArticleList" class="title4">Subject's List</div>
    <asp:GridView ID="gvSubject" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idsubject"
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        onrowdatabound="gvSubject_RowDataBound"
        OnRowCommand="gvSubject_RowCommand">
            <Columns>
                <asp:BoundField DataField="idsubject" HeaderText="ID" InsertVisible="False" SortExpression="idarticledetail" />
                <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" />
                <asp:BoundField DataField="description" HeaderText="Description" SortExpression="description" />
                <asp:ButtonField CommandName="deleteSubject" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        ProviderName="" 
        SelectCommand="SELECT * FROM subjecttab">
    </asp:SqlDataSource>
    <asp:Table BorderWidth="1" ID="Table2" runat="server" CellPadding="5" CellSpacing="5" Width="100%"></asp:Table>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Subject.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newSubject" ToolTip="New Article" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Subject.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveSubject" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R4Content" Runat="Server">
</asp:Content>


