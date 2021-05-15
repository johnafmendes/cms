<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="UpgradePartII.aspx.cs" Inherits="admin_UpgradePartII" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainMenu" Runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Upgrade - Step II of IV
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tUpgradeII" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            Current Version Installed:
            </td>
            <td>
            <asp:Label ID="lblCVInstalled" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Current Version's Number Installed:
            </td>
            <td>
            <asp:Label ID="lblCVNInstalled" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td align="right">
            This Version (Upgrade Version):
            </td>
            <td>
            <asp:Label ID="lblDateInstalled" runat="server" Text="" />
            </td>
        </tr>
    </table>
    <div id="divisorLineArticle"></div>
    <br />
    <table id="Table1" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            This Version (Upgrade Version):
            </td>
            <td>
            <asp:Label ID="lblUpgradeVersion" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Upgrade Version's Number:
            </td>
            <td>
            <asp:Label ID="lblUpgradeNumber" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Installation's Date:
            </td>
            <td>
            <asp:Label ID="lblDateInstallationUpgrade" runat="server" Text="" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:Button Enabled="false" PostBackUrl="UpgradePartII.aspx?action=1" ID="btnUpgrade" OnCommand="doUpgrade" ToolTip="Go and do not back more! Do you have sure?" runat="server" Text="Commit"/>
    <asp:Button PostBackUrl="Config.aspx?action=0" ID="btnCancel" ToolTip="Cancel" runat="server" Text="Cancel"/>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="R4Content" Runat="Server">
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="footer1" Runat="Server">
</asp:Content>

