using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class installation_installation : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "John Mendes CMS - Installation";
        logo.InnerHtml = "<img src=\"../images/1.gif\" alt=\"\" title=\"\" />";
    }
}
