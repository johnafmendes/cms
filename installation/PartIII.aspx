<%@ Page Title="" Language="C#" MasterPageFile="~/installation/installation.master" AutoEventWireup="true" CodeFile="PartIII.aspx.cs" Inherits="installation_PartIII" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainMenu" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Installation - Part III of IV
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tPartIII" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="left" colspan="2">
            This Part III, you can choose a name for your Web Site. Also you must type the name of folder where 
            your copied this system.
            </td>
        </tr>
        <tr>
            <td align="right">
            Title:
            </td>
            <td>
            <asp:TextBox ID="txtTitle" Columns="30" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Folder Installed: http://<% Response.Write(Request.ServerVariables["http_host"].ToString());%>/
            </td>
            <td>
            <asp:TextBox ID="txtFolderInstalled" Columns="30" runat="server"/>/
            </td>
        </tr>
    </table>
    <br />    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:Button ID="btnSave" Text="Save" OnCommand="saveConfig" PostBackUrl="PartIII.aspx" runat="server" />
    <asp:Button ID="btnNext" Text="Next Step" PostBackUrl="PartIV.aspx" Enabled="false" runat="server" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R3Enabled" Runat="Server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Enabled" Runat="Server">
</asp:Content>
