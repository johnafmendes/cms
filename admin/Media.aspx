<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Media.aspx.cs" Inherits="admin_Media" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Media Management
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tUser" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID Media:
            </td>
            <td>
            <asp:Label ID="lblidMedia" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Title:
            </td>
            <td>
            <asp:TextBox ID="txtTitle" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Upload a File:
            </td>
            <td>
            <asp:FileUpload ID="File" Columns="30" runat="server" />
            </td>
        </tr>        
    </table>
    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleListUsers" class="title4">Media's List</div>
    <asp:GridView ID="gvMedia" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idmedia"  
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        OnRowDataBound="gvMedia_RowDataBound"
        OnRowCommand="gvMedia_RowCommand">
            <Columns>
                <asp:BoundField DataField="idMedia" HeaderText="ID" InsertVisible="False" SortExpression="idmedia" />
                <asp:ImageField ControlStyle-Width="150px" DataImageUrlField="FileName" DataImageUrlFormatString="~\images\{0}" NullDisplayText="No Media" HeaderText="Media" SortExpression="filename" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="title" />
                <asp:BoundField DataField="FileName" HeaderText="File Name" SortExpression="filename" />
                <asp:ButtonField CommandName="deleteMedia" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        DeleteCommand="DELETE FROM mediatab WHERE idmedia = ?" 
        ProviderName="" 
        SelectCommand="SELECT idmedia, title, originalname, filename, type, width, height, enabled FROM mediatab">
        <DeleteParameters>
            <asp:Parameter Name="idmedia" Type="Int32" />
        </DeleteParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Media.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newMedia" ToolTip="New Media" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Media.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveMedia" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Media Management</b>
    <br />
    Media section is where you can add, amend or exclude a select range of kind of media that can be added inside 
    of articles, set as logo of this CMS or simplicity give a way to upload some files like .zip or .doc to be linked 
    inside of content on articles to be downloaded after.
    <br />
    <br />
    <b>Title</b> - Title is just a indetification of media that you can do upload.
    <br />
    <br />
    <b>Choose File</b> - Clicking in this button, you can select a media to do upload into the system and after you can 
    link this media inside of some article.
    

</asp:Content>
