<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" EnableViewState="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="ContentMainMenu" ContentPlaceHolderID="MainMenu" Runat="Server">
    <div id="MainMenuContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="Languages" Runat="Server">
    <div id="languagesHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentArticles" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="mainContent">
        <div id="mainContentHTML" runat="server"></div>
    </div>
</asp:Content>

<asp:Content ID="ContentMainMedia" ContentPlaceHolderID="MainMedia" Runat="Server">
    <div id="mainMedia">
       <div id="mainMediaHTML" runat="server"></div>
    </div>
</asp:Content>

<asp:Content ID="ContentMainHeader" ContentPlaceHolderID="MainHeader" Runat="Server">
    <div id="mainHeaderHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="R1TitleContent" Runat="Server">
    <div id="R1TitleHTML" class="TitleColumns" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentR1" ContentPlaceHolderID="R1Content" Runat="Server">
    <div id="R1ContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="R2TitleContent" Runat="Server">
    <div id="R2TitleHTML" class="TitleColumns" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentR2" ContentPlaceHolderID="R2Content" Runat="Server">
    <div id="R2ContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="R3TitleContent" Runat="Server">
    <div id="R3TitleHTML" class="TitleColumns" runat="server">Search</div>
</asp:Content>

<asp:Content ID="ContentR3" ContentPlaceHolderID="R3Content" Runat="Server">
    <asp:Panel DefaultButton="btnSearch" runat="server">
        <asp:Label Text="Key word:" ID="lblKeyWord" Font-Bold="true" runat="server"/><br /><br />
        <asp:TextBox Text="" TextMode="SingleLine" ID="txtSearch" Columns="35" runat="server"/>
        <asp:Button UseSubmitBehavior="true" Text="Search" ID="btnSearch" ToolTip="Search" OnCommand="buildSearch2" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R4TitleContent" Runat="Server">
    <div id="R4TitleHTML" class="TitleColumns" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentR4" ContentPlaceHolderID="R4Content" Runat="Server">
    <div id="R4ContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentMainFooter" ContentPlaceHolderID="MainFooter" Runat="Server">
    <div id="mainFooterHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentJavaScriptAtEndPage" ContentPlaceHolderID="JavaScriptAtEndPage" Runat="Server">
    <div id="JavaScriptAtEndPage" runat="server"></div>
</asp:Content>

