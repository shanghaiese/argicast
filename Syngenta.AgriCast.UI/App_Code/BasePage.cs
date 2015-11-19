/* Agricast CR - R4 - Dynamic master template rendering - Begin */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{ 
    protected override void OnPreInit(EventArgs e)
    {
        string pubMasterFileName = ConfigurationManager.AppSettings["pubMasterFileName"];
        if (string.IsNullOrEmpty(pubMasterFileName))
            pubMasterFileName = "site.master";
        // for mobile site
        //string mobileMasterFileName = "mobile.master";

        // string url = HttpContext.Current.Request.Url.ToString();
        // Get the Pub Name
        string pubName = HttpContext.Current.Request.QueryString["pub"];
        string virturlPubFolder = string.Format("~/Pub/{0}/", pubName);
        string physicalPubFolder = Server.MapPath(virturlPubFolder);

        string masterFile = Path.Combine(physicalPubFolder, pubMasterFileName);
        if (File.Exists(masterFile))
        {
            this.MasterPageFile = virturlPubFolder + pubMasterFileName;
        }

        base.OnPreInit(e);
    }

}

/* Agricast CR - R4 - Dynamic master template rendering - End */
