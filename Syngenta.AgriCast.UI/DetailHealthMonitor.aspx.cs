using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Syngenta.AgriCast.Icon.View;
using Syngenta.AgriCast.Icon.Presenter;
using System.Text;
using Syngenta.AgriCast.Tables;
using Syngenta.AgriCast.Tables.Presenter;
using Syngenta.AgriCast.Tables.View;
using System.Collections;
using System.Globalization;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.Service;
using System.IO;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Icon.Service;
using Syngenta.AgriCast.Icon.DTO;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.Tables.Service;
using System.Web.Security;
using Syngenta.AgriCast.LocationSearch.Presenter;

public partial class DetailHealthMonitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            var memUser = Membership.GetUser(@"rogeri");
            test.Text = memUser != null ? "Hurray...Agricast is up" : "I am down.. please fix it ASAP";
        }
        catch (Exception ex)
        {
            test.Text = ex.Message;
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            var memUser = Membership.GetUser(@"rogeri");
            TextBox1.Text = memUser != null ? "AgriCast DB has been successfully Tested" : "No data found. kindly check the AgriCast DB once again.";
        }
        catch (Exception ex)
        {
            TextBox1.Text = ex.Message;
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        try
        {
            TableService objTblSvc = new TableService();
            DataTable dtseries = new DataTable();
            dtseries.Columns.Add("name");
            DataRow dr = dtseries.NewRow();
            dr[0] = "";
            dtseries.Rows.Add(dr);
            dr = dtseries.NewRow();
            dr[0] = "";
            dtseries.Rows.Add(dr);
            dr = dtseries.NewRow();
            dr[0] = "";
            dtseries.Rows.Add(dr);
            dr = dtseries.NewRow();
            dr[0] = "";
            dtseries.Rows.Add(dr);
            dr = dtseries.NewRow();
            dr[0] = "";
            dtseries.Rows.Add(dr);
            var result = objTblSvc.GetHealthMointorWeather();
            TextBox2.Text = result.Rows.Count > 0 ? "Weather Data has been successfully Tested" : "No data found. kindly check the Weather Data  once again.";
        }
        catch (Exception ex)
        {
            TextBox2.Text = ex.Message;
        }
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        try
        {
            locSearchPresenter objLocPre = new locSearchPresenter();
            var itemList = objLocPre.getAutoSuggestLocation("FR", "Paris");
            TextBox3.Text = itemList.Count > 0 ? "LocationSearch has been successfully Tested" : "No data found. kindly check the LocationSearch service once again.";
        }
        catch (Exception ex)
        {
            TextBox3.Text = ex.Message;
        }
    }
}