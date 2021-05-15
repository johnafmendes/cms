/* CLASS NAME: App_Code/Styles.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is add some html div into page to create division.
 */
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for Styles
/// </summary>
public partial class Styles : System.Web.UI.Page
{
    public string dotLine;
    public string underlineTitle; 

    //constructor
	public Styles()
	{
        dotLine = "<div id=\"divisorLineArticle\"></div>";
        underlineTitle = "<div id=\"menuUnderline\" class=\"underline\" />";
	}

    //The rewrite the string inside a div and put a level in the class attribute
    public string title(string text, string level)
    {
        return "<div id=\"title01\" class=\"title" + level + "\">" + text + "</div>";
    }
}
