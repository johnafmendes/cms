<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Tags.aspx.cs" Inherits="admin_Tags" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Tag Management
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tTags" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidTag" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Tag:
            </td>
            <td>
            <asp:TextBox ID="txtTag" Text="" Columns="78" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Visible:
            </td>
            <td>
            <asp:CheckBox ID="cbEnabled" Checked="false" runat="server" />
            </td>
        </tr>
    </table>

    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleTagList" class="title4">Tag's List</div>
    <asp:GridView ID="gvTag" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idtag"  
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" onrowdatabound="gvTag_RowDataBound"
        OnRowCommand="gvTag_RowCommand">
            <Columns>
                <asp:BoundField DataField="idTag" HeaderText="ID" InsertVisible="False" SortExpression="idtag" />
                <asp:BoundField DataField="Tag" HeaderText="Tag" SortExpression="tag" />
                <asp:BoundField DataField="enabled" HeaderText="Enabled" SortExpression="enabled" />
                <asp:BoundField DataField="Times" HeaderText="Used" SortExpression="Times" />
                <asp:ButtonField CommandName="deleteTag" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        DeleteCommand="DELETE FROM tagtab WHERE idtag = ?" 
        ProviderName="" 
        SelectCommand="SELECT t.idtag, t.tag, t.enabled, COUNT(a.idtag) as times FROM tagtab t, articletagtab a WHERE t.idtag = a.idtag GROUP BY t.tag">
        <DeleteParameters>
            <asp:Parameter Name="idtag" Type="Int32" />
        </DeleteParameters>
    </asp:SqlDataSource>
    
    <asp:Table BorderWidth="1" ID="Table2" runat="server" CellPadding="5" CellSpacing="5" Width="100%"></asp:Table>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Tags.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="newTag" ToolTip="New Tag" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Tags.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="saveTag" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Tag Management</b>
    <br />
    Tags are the keywords that can be used to create identification of articles.
    <br />
    <br />
    <b>Tag</b> - Name/Title of tag.
    <br />
    <br />
    <b>Visible</b> - You can set a tag as visible or not. If visible, it can be used to tag an article.
</asp:Content>
