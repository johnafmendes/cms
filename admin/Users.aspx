<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="admin_Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Users Management
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tUser" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidUser" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td align="right">
            First Name:
            </td>
            <td>
            <asp:TextBox ID="txtFirstName" Columns="30" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Surname:
            </td>
            <td>
            <asp:TextBox ID="txtSurname" Columns="30" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Username:
            </td>
            <td>
            <asp:TextBox ID="txtUsername" Columns="30" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Password:
            </td>
            <td>
            <asp:TextBox ID="txtPassword" TextMode="Password" Columns="20" runat="server" />
            <asp:RegularExpressionValidator 
                ID="vldPasswordContent" 
                ControlToValidate = "txtPassword" 
                ValidationExpression = "\w{8,20}" 
                Text = "*" 
                EnableClientScript = "false" 
                runat = "server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Confirm Password:
            </td>
            <td>
            <asp:TextBox ID="txtPassword2" TextMode="Password" Columns="20" runat="server" />
            <asp:CompareValidator 
                ID = "vldPassword" 
                ControlToValidate = "txtPassword" 
                ControlToCompare = "txtPassword2" 
                EnableClientScript = "false" 
                Text = "*"
                Operator = "Equal" 
                Type = "String" 
                runat = "server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            e-Mail:
            </td>
            <td>
            <asp:TextBox ID="txtEmail" Columns="30" runat="server" />
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
        <tr>
            <td align="right">
            Type Authentication:
            </td>
            <td>
                <asp:RadioButtonList ID="rbTypeAuthe" runat="server" RepeatColumns="2">
                    <asp:ListItem Text="LDAP" Value="LDAP" />
                    <asp:ListItem Text="Local" Value="Local" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td align="right">
            Type of User:
            </td>
            <td>
                <asp:RadioButton Text="Administrator" GroupName="rbType" ID="rbAdmin" runat="server" Checked="true"/>
                <asp:RadioButton Text="Standard" GroupName="rbType" ID="rbStandard" runat="server"/>
            </td>
        </tr>
    </table>
    <div id="mainTitlePermissions" class="title4">Permissions</div>
    <asp:GridView ID="gvPermissions" runat="server" Width="100%" 
        AutoGenerateColumns="false" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idModule,idUser"
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource2" RowStyle-Wrap="True" 
        onRowUpdating="gvPermissions_RowUpdating" 
        OnRowCommand="gvPermissions_RowCommand" 
        onrowediting="gvPermissions_RowEditing" 
        onselectedindexchanged="gvPermissions_SelectedIndexChanged" >
            <Columns>
                <asp:BoundField DataField="idModule" InsertVisible="false" ReadOnly="true" 
                    HeaderText="ID" SortExpression="idModule" />                    
                <asp:BoundField DataField="Title" InsertVisible="true" ReadOnly="true" 
                    HeaderText="Section" SortExpression="title" />
                <asp:TemplateField HeaderText="Read/Select" SortExpression="SelectData" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>
                        <asp:CheckBox ID="SelectData" runat="server" Checked='<%# ((Int16)Eval("SelectData"))==1?true:false %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Write/Insert" SortExpression="InsertData" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>
                        <asp:CheckBox ID="InsertData" runat="server" Checked='<%# ((Int16)Eval("InsertData"))==1?true:false %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Update/Amend" SortExpression="UpdateData" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>
                        <asp:CheckBox ID="UpdateData" runat="server" Checked='<%# ((Int16)Eval("UpdateData"))==1?true:false %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Delete/Exclude" SortExpression="DeleteData" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>
                        <asp:CheckBox ID="DeleteData" runat="server" Checked='<%# ((Int16)Eval("DeleteData"))==1?true:false %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" HeaderText="Action" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server"
        ConnectionString=""
        ProviderName=""
        SelectCommand="SELECT mt.idmodule, mut.selectdata, mut.insertdata, mut.updatedata, mut.deletedata, mt.title, mut.iduser FROM moduleusertab AS mut INNER JOIN modulestab AS mt ON mut.idmodule = mt.idmodule WHERE mut.iduser = ? GROUP BY mt.title, mut.idmodule, mut.iduser, mut.selectdata, mut.insertdata, mut.updatedata, mut.deletedata"
        UpdateCommand="UPDATE moduleusertab SET selectdata=@SelectData, insertdata=@InsertData, updatedata=@UpdateData, deletedata=@DeleteData WHERE idmodule=@idModule AND iduser=@idUser">
        <UpdateParameters>
            <asp:Parameter Name="SelectData" Type="Boolean" />
            <asp:Parameter Name="InsertData" Type="Boolean" />
            <asp:Parameter Name="DeleteData" Type="Boolean" />
            <asp:Parameter Name="UpdateData" Type="Boolean" />
        </UpdateParameters>
        <SelectParameters>
            <asp:Parameter Name="iduser" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleListUsers" class="title4">User's List</div>
    <asp:GridView ID="gvUser" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="iduser"  
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" onrowdatabound="gvUser_RowDataBound"
        OnRowCommand="gvUser_RowCommand">
            <Columns>
                <asp:BoundField DataField="idUser" HeaderText="ID" InsertVisible="False" SortExpression="iduser" />
                <asp:BoundField DataField="Surname" HeaderText="Surname" SortExpression="surname" />
                <asp:BoundField DataField="Name" HeaderText="First Name" SortExpression="name" />
                <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="username" />
                <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="type" />
                <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" SortExpression="Enabled" ItemStyle-HorizontalAlign="Center" />
                <asp:ButtonField CommandName="deleteUser" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        DeleteCommand="DELETE FROM usertab WHERE iduser = ?" 
        ProviderName="" 
        SelectCommand="SELECT idUser, Surname, Name, UserName, if(Enabled, 'true', 'false') as Enabled, Type FROM usertab">
        <DeleteParameters>
            <asp:Parameter Name="iduser" Type="Int32" />
        </DeleteParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Users.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newUser" ToolTip="New User" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Users.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveUser" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="R3Content" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="R4Content" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Users Management</b>
    <br />
    Users section is where you can add, amend, exclude, enable or disable users that is going to access Control Panel - Menu.
    <br />
    <br />
    <b>Password</b> - Must have at least 8 character.<br/>
    - To amend details without change the password, you must leave the Password in blank.
    <br />
    <br />
    <b>Enabled</b> - Must be set as true if you want that this user get access to Control Panel.
    <br />
    <br />
    <b>Type of User</b> - Administrator has access on any part of Control Panel and can read, add, amend, remove 
    and set any information. Standard users depends on rights seted at Access Rights area.
</asp:Content>
