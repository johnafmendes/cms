<%@ Page Title="" Language="C#" MasterPageFile="~/installation/installation.master" AutoEventWireup="true" CodeFile="PartII.aspx.cs" Inherits="installation_PartII" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainMenu" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Installation - Part II of IV
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tPartII" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="left" colspan="2">
            Now you need create a database for this system.<br/><br/>
            This system accept just MySQL database, then, you need have installed MySQL database before that come here. 
            If you do have not one installed, do download clicking <asp:HyperLink ID="HyperLink1" NavigateUrl="http://dev.mysql.com/downloads/" Target="_blank" runat="server">
            here</asp:HyperLink><br/><br/>
            Also, you need to certificate that your Web Server has a Driver for access to MySQL database. If you do have not one, do download 
            clicking <asp:HyperLink ID="hlDriverMySQL" NavigateUrl="http://dev.mysql.com/downloads/connector/odbc/5.1.html" Target="_blank" runat="server">
            here</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="right">
                Host of Database:
            </td>
            <td align="left">
                <asp:TextBox ID="txtHost" Text="127.0.0.1" runat="server" />
            </td>            
        </tr>
        <tr>
            <td align="right">
                Port:
            </td>
            <td align="left">
                <asp:TextBox ID="txtPort" Text="3306" runat="server" />
            </td>            
        </tr>
        <tr>
            <td align="right">
                User Name for MySQL:
            </td>
            <td align="left">
                <asp:TextBox ID="txtUserName" Text="root" runat="server" />
            </td>            
        </tr>
        <tr>
            <td align="right">
                Password:
            </td>
            <td align="left">
                <asp:TextBox ID="txtPassword" Text="" runat="server" />
            </td>            
        </tr>
        <tr>
            <td align="right">
                Database name:
            </td>
            <td align="left">
                <asp:TextBox ID="txtDBName" Text="JohnMendesCMS" runat="server" />
            </td>            
        </tr>        
        <tr>
            <td align="right">
                MySQL Driver Version:
            </td>
            <td align="left">
                <asp:TextBox ID="txtMySQLDriverVersion" Text="5.1" runat="server" />
            </td>            
        </tr>
    </table>
    <br />    
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:Button ID="btnCreateDB" Text="Create Database" OnCommand="createDB" PostBackUrl="PartII.aspx" runat="server" />
    <asp:Button ID="btnNext" Text="Next Step" PostBackUrl="PartIII.aspx" Enabled="false" runat="server" />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R3Enabled" Runat="Server">
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="R4Enabled" Runat="Server">
</asp:Content>

