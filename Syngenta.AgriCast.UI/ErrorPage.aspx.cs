using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ErrorPage : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Cookies.Clear();
        Response.Redirect("Default2.aspx");
    }
}