<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Upgrade.aspx.cs" Inherits="admin_upgrade" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainMenu" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Upgrade - Step I of IV
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tPartI" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="center">
            Wellcome to John Mendes CMS <b>UPGRADE</b>!<br/><br/>
            To install this software, follow nexts steps ...<br/><br/>
            To start, click in Begin UPGRADE.
            <br/><br/><br/>
            <b>Remember, if you have not sure if you must or not do this
            upgrade, ask for John Mendes about it.</b>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:Button ID="btnBegin" Text="Begin UPGRADE" PostBackUrl="UpgradePartII.aspx" runat="server" />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R4Content" Runat="Server">
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="footer1" Runat="Server">
</asp:Content>

