<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="GroupMedia.aspx.cs" Inherits="GroupMedia" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Group Media Management
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tGroupMedia" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidGroupMedia" runat="server" Text="" />
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
            Description:
            </td>
            <td>
            <asp:TextBox ID="txtDescription" Columns="30" Rows="5" TextMode="MultiLine" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Enabled:
            </td>
            <td>
            <asp:CheckBox ID="cbEnabled" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitlePermissions" class="title4">Media Group List</div>
    <asp:GridView ID="gvMediaGroup" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idmediagroup"
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        onrowdatabound="gvMediaGroup_RowDataBound"
        OnRowCommand="gvMediaGroup_RowCommand">
            <Columns>
                <asp:BoundField DataField="idMediaGroup" HeaderText="ID" InsertVisible="False" SortExpression="idmediagroup" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="title" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="description" />
                <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" SortExpression="Enabled" ItemStyle-HorizontalAlign="Center" />
                <asp:ButtonField CommandName="deleteMediaGroup" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        ProviderName="" 
        SelectCommand="SELECT idmediaGroup, title, description, if(enabled, 'true', 'false') as enabled FROM mediagrouptab">
    </asp:SqlDataSource>
    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <table id="adf" border="0" width="100%">
        <tr>
            <td colspan="3">
                Media Group List: <asp:DropDownList ID="ddlMediaGroup" runat="server"/> <asp:Button ID="btnFilterMediaGroup" runat="server" OnCommand="FilterMediaGroup" Text="Filter" PostBackUrl="GroupMedia.aspx?action=0" />
            </td>
        </tr>
        <tr>
            <td style="width:45%;" valign="top">
                <div id="Div1" class="title4">Media List</div>
                    <asp:GridView ID="gvMediaList" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
                        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
                        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
                        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
                        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
                        HeaderStyle-CssClass="HeadTableAdm" 
                        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idmedia"  
                        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
                        DataSourceID="SqlDataSource2" 
                        onrowdatabound="gvMedia_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="idMedia" HeaderText="ID" InsertVisible="False" SortExpression="idmedia" />
                                <asp:ImageField ControlStyle-Width="150px" DataImageUrlField="FileName" DataImageUrlFormatString="~\images\{0}" NullDisplayText="No Media" HeaderText="Media" SortExpression="filename" />
                                <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="AddSelector" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                        ConnectionString=""
                        ProviderName="" 
                        SelectCommand="SELECT * FROM mediatab WHERE idmedia NOT IN (SELECT idmedia FROM medialisttab WHERE idmediagroup = ?)">
                        <SelectParameters>
                            <asp:Parameter Name="idMedia" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
            </td>
            <td  style="width:10%;" align="center"  valign="middle">
                <asp:Button ID="btnAdd" runat="server" Text="->>" ToolTip="Add Media to Group" OnCommand="AddMedia" /><br /><br />
                <asp:Button ID="btnRemove" runat="server" Text="<<-" ToolTip="Remove Media of Group" OnCommand="RemoveMedia" />
            </td>
            <td  style="width:45%;"  valign="top">
                <div id="Div2" class="title4">Media on Group</div>
                <asp:GridView ID="gvMediaInGroup" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
                    AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
                    GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
                    EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
                    PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
                    HeaderStyle-CssClass="HeadTableAdm" 
                    EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idmediagroup"
                    AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
                    DataSourceID="SqlDataSource3" 
                    OnRowDataBound="gvMedia_RowDataBound">
                        <Columns>
                                <asp:BoundField DataField="idMedia" HeaderText="ID" InsertVisible="False" SortExpression="idmedia" />
                                <asp:ImageField ControlStyle-Width="150px" DataImageUrlField="FileName" DataImageUrlFormatString="~\images\{0}" NullDisplayText="No Media" HeaderText="Media" SortExpression="filename" />
                                <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="RemoveSelector" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                        </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource3" runat="server" 
                    ConnectionString=""
                    ProviderName="" 
                    SelectCommand="SELECT ml.*, m.* FROM medialisttab ml, mediatab m WHERE ml.idmedia=m.idmedia AND idmediagroup = ?">
                    <SelectParameters>
                        <asp:Parameter Name="idMediaGroup" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="GroupMedia.aspx?action=0" Width="30" ImageAlign="AbsBottom" OnCommand="newMediaGroup" ID="btnNew"  ToolTip="New Group" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="GroupMedia.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveMediaGroup" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Group Media Management</b>
    .
    <br />
    <br />
    <b>Title</b> - .
    <br />
    <br />
    <b>Description</b> - .
    <br />
    <br />
    <b>Enabled</b> - .
</asp:Content>