<%@ Page Title="" Language="C#" MasterPageFile="~/installation/installation.master" AutoEventWireup="true" CodeFile="PartIV.aspx.cs" Inherits="installation_PartIV" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainMenu" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleMainContent" Runat="Server">
    Installation - Part IV of IV
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tPartIV" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="center" colspan="2">
            Thank you for choose John Mendes CMS for Dummies.<br/><br/>
            To start management and insert content, click on Administration button.<br/><br/>
            The User Name is <b>admin</b>. The Password is: <b>admin</b>.<br/><br/>
            To see your front page, click in Front page.
            <br/><br/>
        </tr>
        <tr>
            <td colspan="2">
            <b>REMEMBER:</b><br/>
            For this CMS use the Friendly URL properly, you must go to IIS and do the follow steps.<br/>
            In IIS 6.0:<br/><br/>

            1) Right click the website Foo ---> Select Properties<br/>
            2) In Properties window, select "Home Directory" tab<br/>
            3) Select "Configuration" button on the right side (near bottom)<br/>
            4) You can see "Wildcard application maps" in the new window now.<br/>
            5) Click "Insert" to add a wildcard mapping. <br/>
            6) Select the file: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll 
            If you want to use URL redirect, you need uncheck "Verify that file exists"<br/><br/>

            In IIS 7.0<br/><br/>
            In IIS7 things are a little different. First thing you need to do is unlock the config section 
            "handlers". You do this by bring up a DOS prompt and entering the command: <br/>
            C:\Windows\System32\inetsrv>appcmd.exe unlock config /section:system.webserver/handlers<br/>
            Then, you can set your wildcard handler in your web.config file as follows.<br/>
            Now, the default handlers will be executed in your web.config file, and of course, all the 
            defaults for that are in the chain of files that inherits from. For more details on that, 
            see this FAQ: Configuration Files FAQs (web.config, machine.config…).<br/>
            You may need to add the wildcard mapper handler to your web.config.  To do this, you would put the 
            following in your web.config<br/>
                <br />
                &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;<br />
&nbsp; &lt;configuration&gt;<br />
&nbsp;&nbsp;&nbsp; &lt;system.webServer&gt;<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;handlers&gt;<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;add name=&quot;WildCard&quot; path=&quot;*&quot; verb=&quot;*&quot; 
                type=&quot;sampletype&quot; resourceType=&quot;Unspecified&quot; /&gt;<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/handlers&gt;<br />
&nbsp;&nbsp;&nbsp; &lt;/system.webServer&gt;<br />
&nbsp; &lt;/configuration&gt;
                <br />
                <br/>

            Now, you should be set. See the next section for a good method for actually doing the remapping, 
            which is why we went down this path in the first place.<br/><br/>
            Reference: http://peterkellner.net/2008/08/24/urlrewrite-with-aspnet-urlrewriter-wildcard-mapping-iis6-iis7/
            </td>
        </tr>
    </table>
    <br />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:Button ID="btnAdminstration" Text="Administration" PostBackUrl="../admin/Default.aspx" runat="server" />
    <asp:Button ID="btnFrontPage" Text="Front Page" PostBackUrl="../Default.aspx" runat="server" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="R3Enabled" Runat="Server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Enabled" Runat="Server">
</asp:Content>
