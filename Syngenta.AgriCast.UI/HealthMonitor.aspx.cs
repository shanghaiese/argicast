using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class HealthMonitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var memUser = Membership.GetUser(@"rogeri");
        test.Text = memUser != null ? "Hurray...Agricast is up" : "I am down.. please fix it ASAP";
    }
}