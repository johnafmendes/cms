<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="ContentMainMenu" ContentPlaceHolderID="MainMenu" Runat="Server">
    <div id="menu" class="color1">
        <div class="bl">
            <div class="br">
                <div class="tl">
                    <div class="tr">
                        <div id="MainMenuContentHTML" runat="server"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="menuUnderline" class="underline"></div>
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function reloadCaptcha() {
            document.getElementById("imageCaptcha").src = document.getElementById("imageCaptcha").src;
        }

        var CMSAjax = {};
        CMSAjax.DIV_RESULT = 'divResult';

        CMSAjax.ReproduceSound = function() {
            var params = '';
            var filePath = 'Default.aspx?action=2';
            CMSAjax.sendRequest(filePath, params, CMSAjax.DIV_RESULT);
        };

        CMSAjax.sendRequest = function(filePath, params, resultDivName) {
            if (window.XMLHttpRequest) {
                var xmlhr = new XMLHttpRequest();
            } else {
                var xmlhr = new ActiveXObject('MSXML2.XMLHTTP.3.0');
            }

            xmlhr.open('GET', filePath, true);
            xmlhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

            xmlhr.onreadystatechange = function() {
                var resultDiv = document.getElementById(resultDivName);
                if (xmlhr.readyState == 1) {
                    resultDiv.innerHTML = '<b>Loading...</b>';
                } else if (xmlhr.readyState == 4 && xmlhr.status == 200) {
                    if (xmlhr.responseText) {
                        resultDiv.innerHTML = xmlhr.responseText;
                    }
                } else if (xmlhr.readyState == 4) {
                    alert('Invalid response received - Status: ' + xmlhr.status);
                }
            }
            xmlhr.send(params);
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Login Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="mainContent">
        <div id="divForm">
            <asp:Panel ID="Panel1" DefaultButton="btnAuth" runat="server">
                Domain\Username:<br /><asp:TextBox ID="txtUserName" runat="server"/><br/>
                Password:<br /><asp:TextBox ID="txtPassWord" TextMode="Password" runat="server" /><br/><br/>
                <iframe id="imageCaptcha" style="border: 0px;" width="110" height="40" frameborder="0" marginheight="0" marginwidth="0" scrolling="no" src="Default.aspx?action=0"></iframe>
                <asp:ImageButton ID="imgbtnListen" ImageUrl="../images/speaker-32x32.png" ToolTip="Play Audio." OnClientClick="javascript: CMSAjax.ReproduceSound(); return false;" runat="server" />
                <asp:ImageButton ID="imgbtnReloadImage" ImageUrl="../images/Button-Reload-32x32.png" ToolTip="See a new set of characters." OnClientClick="javascript: reloadCaptcha(); return false;" runat="server" /><br/>            
                Characteres (Case-sensitive):<br /><asp:TextBox ID="txtCode" runat="server" /><br/>
                <asp:Label ID="lblMustType" runat="server" /><br/>
                <div id="divResult"></div><br/>
                <asp:Button ID="btnAuth" Text="Authentication" OnClick="authAdmin" runat="server"/>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="R1Content" Runat="Server">
    <div id="R1ContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R2Content" Runat="Server">
    <div id="R2ContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R3Content" Runat="Server">
    <div id="R3ContentHTML" runat="server"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R4Content" Runat="Server">
    <div id="R4ContentHTML" runat="server">
        This section allow you get into the System Administration.
        <br/><br/>
        <b>Username:</b> - This field must be typed considering upper or lower case-sensitive with limit of 255 characteres.
        <br/><br/>
        <b>Password:</b> - This field must be typed considering upper or lower case-sensitive with limit of 32 characteres.
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="Footer1" Runat="Server">
</asp:Content>