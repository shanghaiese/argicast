using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.AppendHeader("Refresh", (this.Session.Timeout * 60).ToString());

        if (!IsPostBack)
        {
            dynamicCss.Attributes.Add("href", HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath + ("/Styles/Common.css"));
            siteCss.Attributes.Add("href", HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath + ("/Styles/Site.css"));
            // dynamicCss.Attributes.Add("href", HttpContext.Current.Request.MapPath("~/Styles/Common.css"));
            //  siteCss.Attributes.Add("href", HttpContext.Current.Request.MapPath("~/Styles/Site.css"));
        }
        
        //get the pub name
        string root = "";
        
        /* Agricast CR - 3.5	R5 - External user management changes. - Begin */
        var url = HttpContext.Current.Request.Url.ToString().ToLower();
        url = url.Remove(url.LastIndexOf(".aspx"));
        root = url.Remove(url.LastIndexOf('/'))
            .Substring(url.IndexOf("pub/") + 4);
        //root = HttpContext.Current.Request.Url.ToString() 
        //  .Remove(HttpContext.Current.Request.Url.ToString().LastIndexOf('/'))
        //  .Substring(HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("pub/") + 4);
        /* Agricast CR - 3.5	R5 - External user management changes. - End */

        System.Web.UI.Page CurrentPage = (System.Web.UI.Page)HttpContext.Current.CurrentHandler;
        //get a list of all the css files in the pub folder
        //string[] CssPaths = Directory.GetFiles(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath + "/pub/" + root, "*.css");
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
        //get a list of all the js files in the pub folder
        string[] jsPaths = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "/pub/" + root, "*.js");
        foreach (string file in jsPaths)
        {
            //foreach js file add a script tag to the head of the current page
            HtmlGenericControl ScriptControl = new HtmlGenericControl("script");
            string httpFile = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath + file.Substring(file.IndexOf("/pub/"));
            ScriptControl.Attributes.Add("type", "text/javascript");
            ScriptControl.Attributes.Add("src", httpFile.Replace("\\", "/"));
            CurrentPage.Header.Controls.Add(ScriptControl);
        }
    }

}

