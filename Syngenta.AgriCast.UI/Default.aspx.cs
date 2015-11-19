using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Default : System.Web.UI.Page
{
    protected string switchUrl = "";//Add for IM01865638:New Agricast - default.aspx not stylable

    protected void Page_Load(object sender, EventArgs e)
    {

        //IM01865638:New Agricast - default.aspx not stylable - start

        if (Request.RawUrl.Split('?').Length > 1)
            Response.AddHeader("REFRESH", "1;URL=Default2.aspx?" + Request.RawUrl.Split('?')[1].ToString());

        else
            Response.AddHeader("REFRESH", "1;URL=Default2.aspx");


        //if (Request.RawUrl.Split('?').Length > 1)
        //{
        //    switchUrl = "Default2.aspx?" + Request.RawUrl.Split('?')[1].ToString();
        //}
        //else
        //{
        //    switchUrl = "Default2.aspx";
        //}

        //IM01865638:New Agricast - default.aspx not stylable - end

        /* IM01769909 - New Agricast - "Loading" sign not stylable - Shery - 20140410 - Start */

        if (!IsPostBack)
        {
            string root = "";
            var url = HttpContext.Current.Request.Url.ToString().ToLower();
            url = url.Remove(url.LastIndexOf(".aspx"));
            root = url.Remove(url.LastIndexOf('/')).Substring(url.IndexOf("pub/") + 4);

            System.Web.UI.Page CurrentPage = (System.Web.UI.Page)HttpContext.Current.CurrentHandler;
            string[] CssPaths = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "/pub/" + root, "*.css");
            foreach (string file in CssPaths)
            {
                //foreach css file add a link tag to the head of the current page
                HtmlGenericControl ScriptControl = new HtmlGenericControl("link");
                ScriptControl.Attributes.Add("type", "text/css");
                string httpFile = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath + file.Substring(file.IndexOf("/pub/"));
                ScriptControl.Attributes.Add("href", httpFile.Replace("\\", "/"));
                ScriptControl.Attributes.Add("rel", "stylesheet");
                CurrentPage.Header.Controls.Add(ScriptControl);
            }
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
            string[] JSPaths = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "/pub/" + root, "*.js");
            foreach (string file in JSPaths)
            {
                //foreach css file add a link tag to the head of the current page
                HtmlGenericControl ScriptControl = new HtmlGenericControl("script");
                ScriptControl.Attributes.Add("type", "text/javascript");
                string httpFile = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath + file.Substring(file.IndexOf("/pub/"));
                ScriptControl.Attributes.Add("src", httpFile.Replace("\\", "/"));
                CurrentPage.Header.Controls.Add(ScriptControl);
            }
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
        }

        /* IM01769909 - New Agricast - "Loading" sign not stylable - Shery - 20140410 - End */
    }
}