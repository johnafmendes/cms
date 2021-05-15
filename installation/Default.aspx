<%@ Page Title="" Language="C#" MasterPageFile="~/installation/installation.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="installation_Default" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainMenu" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Installation - Part I of IV
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tPartI" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="center" colspan="2">
            Wellcome to Basic John Mendes CMS.<br/><br/>
            To install this software, follow nexts steps ...<br/><br/>
            To start, click in Begin installation.
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
            </td>
        </tr>
    </table>
    <br />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:Button ID="btnBegin" Text="Begin installation" PostBackUrl="PartII.aspx" runat="server" />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R2Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R3Enabled" Runat="Server">
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="R4Enabled" Runat="Server">
</asp:Content>
