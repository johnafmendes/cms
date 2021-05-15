<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Article.aspx.cs" Inherits="admin_Article" ValidateRequest="false"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="server">
    <!-- TinyMCE -->
    <script type="text/javascript" src="../tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
    <script type="text/javascript">
        // O2k7 skin (silver)
        tinyMCE.init({
            // General options
            mode: "textareas", //exact
            elements: "txtBody",
            theme: "advanced",
            skin: "o2k7",
            skin_variant: "black",
            plugins: "safari,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,inlinepopups",

            // Theme options
            //formatselect,pasteword,blockquote,help,emotions,advhr,insertdate,inserttime,preview,|,ltr,rtl,|,styleselect,fullscreen
            theme_advanced_buttons1: "save,newdocument,print,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,fontselect,fontsizeselect",
            theme_advanced_buttons2: "cut,copy,paste,pastetext,|,search,replace,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,link,unlink,anchor,image,cleanup,code",
            theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,iespell,media",
            theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak,forecolor,backcolor",
            theme_advanced_toolbar_location: "top",
            theme_advanced_toolbar_align: "left",
            theme_advanced_statusbar_location: "bottom",
            theme_advanced_resizing: true,
            extended_valid_elements: "input[name|size|type|value|onclick]",
            extended_valid_elements: "a[class|name|href|target|title|onclick|rel],script[type|src],iframe[src|style|width|height|scrolling|marginwidth|marginheight|frameborder],img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name],$elements", //by John Mendes
            //extended_valid_elements: "script[charset|defer|language|src|type]", //by John Mendes

            // Example content CSS (should be your site CSS)
            content_css: "css/content.css",

            // Drop lists for link/image/media/template dialogs
            template_external_list_url: "lists/template_list.js",
            external_link_list_url: "lists/link_list.js",
            external_image_list_url: "lists/image_list.js",
            media_external_list_url: "lists/media_list.js",

            // Replace values for the template plugin
            template_replace_values: {
                username: "Some User",
                staffid: "991234"
            }
        });
    </script>
    <!-- /TinyMCE -->
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="TitleMainContent" Runat="server">
    Article Management
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table id="tArticle" border="0" cellpadding="5" cellspacing="5" width="100%">
        <tr>
            <td align="right">
            ID:
            </td>
            <td>
            <asp:Label ID="lblidArticle" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Title:
            </td>
            <td>
            <asp:TextBox ID="txtTitle" Text="" Columns="78" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right" valign="top">
            Body:
            </td>
            <td>            
            <asp:TextBox ID="txtBody" Wrap="true" Columns="60" Rows="25" TextMode="MultiLine" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Main Media:
            </td>
            <td>
                <asp:DropDownList ID="ddlMainMedia" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Media Group:
            </td>
            <td>
                <asp:DropDownList ID="ddlMediaGroup" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
                Tag:
            </td>
            <td>
                <asp:TextBox ID="txtTag" Text="" Columns="78" runat="server" />
                <asp:RequiredFieldValidator ID="vldTag" ControlToValidate="txtTag" ErrorMessage="<br/>You must write some tags!" runat="server" Display="Dynamic"/>
            </td>
        </tr>
        <tr>
            <td align="right">
            Subject:
            </td>
            <td>
            <asp:DropDownList ID="ddlSubject" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Language:
            </td>
            <td>
            <asp:DropDownList ID="ddlLanguage" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Enabled:
            </td>
            <td>
            <asp:CheckBox ID="cbEnabled" Checked="false" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Show Details:
            </td>
            <td>
            <asp:CheckBox ID="cbEnableDetails" Checked="false" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Show News:
            </td>
            <td>
            <asp:CheckBox ID="cbEnableNews" Checked="false" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Type:
            </td>
            <td>
            <asp:DropDownList ID="ddlType" runat="server">
                <asp:ListItem Value="Page" Text="Page" Selected="True"></asp:ListItem>
                <asp:ListItem Value="News" Text="News" Selected="False"></asp:ListItem>
            </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right">
            Expires:
            </td>
            <td>
                <asp:TextBox Text="" ID="txtExpires" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            URL:
            </td>
            <td>
                <asp:Label Text="" ID="lblURL" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
            Friendly URL:
            </td>
            <td>
                <asp:Label Text="" ID="lblFriendlyURL" runat="server" />
            </td>
        </tr>
    </table>

    <br />
    <div id="divisorLineArticle"></div>
    <br />
    <div id="mainTitleArticleList" class="title4">Article's List</div>
    <asp:GridView ID="gvArticle" runat="server" AllowPaging="true" Width="100%" AllowSorting="true" 
        AutoGenerateColumns="false" PageSize="20" CellPadding="4" 
        GridLines="Vertical" RowStyle-CssClass="RowAtTableAdm"
        EmptyDataRowStyle-CssClass="EmptyDataRowAtTableAdm" FooterStyle-CssClass="FooterTableAdm" 
        PagerStyle-CssClass="PageTableAdm" SelectedRowStyle-CssClass="SelectedRowAtTableAdm"
        HeaderStyle-CssClass="HeadTableAdm" 
        EditRowStyle-CssClass="EditRowAtTableAdm" DataKeyNames="idarticle"  
        AlternatingRowStyle-CssClass="AlternativeRowAtTableAdm" 
        DataSourceID="SqlDataSource1" 
        onrowdatabound="gvArticle_RowDataBound"
        OnRowCommand="gvArticle_RowCommand">
            <Columns>
                <asp:BoundField DataField="idarticle" HeaderText="ID" InsertVisible="False" SortExpression="idarticle" />
                <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" />                
                <asp:BoundField DataField="type" HeaderText="Type" SortExpression="type" />
                <asp:BoundField DataField="Language" HeaderText="Language" SortExpression="Language" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" SortExpression="Enabled" ItemStyle-HorizontalAlign="Center" />
                <asp:ButtonField CommandName="deleteArticle" HeaderText="Action" Text="Delete" />
            </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString=""
        ProviderName="" 
        SelectCommand="SELECT a.idarticle, a.title, if(a.enabled, 'true', 'false') as enabled, a.type, l.title as language, s.title as subject FROM articletab as a, languagetab as l, subjecttab as s WHERE l.idlanguage=a.idlanguage AND a.idsubject=s.idsubject">
    </asp:SqlDataSource>
    <asp:Table BorderWidth="1" ID="Table2" runat="server" CellPadding="5" CellSpacing="5" Width="100%"></asp:Table>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="R1Content" Runat="Server">
    <asp:ImageButton ImageUrl="../images/empty-48x48.png" PostBackUrl="Article.aspx?action=0" CausesValidation="false" Width="30" ImageAlign="AbsBottom" ID="btnNew" OnCommand="newArticle" ToolTip="New Article" runat="server"/>
    <asp:ImageButton ImageUrl="../images/3floppy-unmount-48x48.png" PostBackUrl="Article.aspx?action=0" Width="30" ImageAlign="AbsBottom" ID="btnSave" OnCommand="saveArticle" ToolTip="Save" runat="server"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="R2Content" Runat="Server">
    <span id="msg" runat="server" /><br />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="R3Content" Runat="Server">
    <br />
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="R4Content" Runat="Server">
    <b>Control Panel/Article Management</b>
    <br />
    Articles section is where you can add, amend, exclude, enable or disable the articles or pages that are going to 
    be accessed by users at front end side. For you add a new article or page, first you must add some menus on menu 
    section and just after add a new content.
    <br />
    <br />
    <b>Title</b> - Title of article, page or news.
    <br />
    <br />
    <b>Body</b> - Body is the place where you can write the content of your articles inserting images and creating styles 
    with tool bar fully of functionality similar to Microsoft Word.
    <br />
    <br />
    <b>Main Media</b> - This option allow you to choose a media as Image or Flash to be part of header of article.
    <br />
    <br />
    <b>Tag</b> - Tags are keywords that can help this article to be found by searcher.
    <br />
    <br />
    <b>Menu Link</b> - You must select a menu to link the article. If the menu is setted as enabled and visible, then, 
    the users at front end side can see the menu and click it.
    <br />
    <br />
    <b>Visible</b> - This option is usuful if you want write a large content, the, you can set visible off (unchecked) and 
    during this time the article cannot be accessible by front end side.
    <br />
    <br />
    <b>Type</b> - This option can be useful if you want create a category of your articles. You can set a article as Page or 
    News. If a article is setted as News, then, it is going to be displayed at expecific part of you front page.
    <br />
    <br />
    <b>Expires</b> - This option allow you to give a exact date and time to article be removed automactly without intervention 
    of Administrator.
    <br />
    <br />
    <b>URL</b> - This represent the full URL of this article. It can be used to create a new links inside of others article and
    link each other.
    
</asp:Content>