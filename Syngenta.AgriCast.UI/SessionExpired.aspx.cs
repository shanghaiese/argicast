using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.DataAccess;

public partial class SessionExpired : System.Web.UI.Page
{
    public const string INVALID_KEY = "Security Key Expired.Please create a new key.";
    public const string INVALID_TOKEN = "Invalid Token.";
    public const string INVALID_PUB = "Invalid publication name.";

    /* IM01335823- New Agricast - Session timed out - Return URL and Iframe - Jerrey - Start */
    public const string MOSS_SESSION_TIMEOUT_KEY = "mossSessionTimeout";
    TranslateData trans = new TranslateData();
    /* IM01335823- New Agricast - Session timed out - Return URL and Iframe - Jerrey - End */

    protected void Page_Load(object sender, EventArgs e)
    {
        /* Agricast CR - 3.5	R5 - External user management changes. - Begin */
        if (!IsPostBack)
        {
            /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - BEGIN*/
            // if the session expires in moss site , redirect to the url specified in the config "mossReturnUrl"
            ServiceInfo objServiceInfo = (ServiceInfo)Session["serviceInfo"];
            if (objServiceInfo != null)
            {
                /* IM01335823- New Agricast - Session timed out - Return URL and Iframe - Jerrey - Begin */
                ////if moss=true and if this url is not emprty, redirect the moss to agricast site
                //if (objServiceInfo.Moss.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                //    && !string.IsNullOrEmpty(objServiceInfo.MossRedirectionUrl))
                //{
                //    //set the redirection url 
                //    hdefault.HRef = objServiceInfo.MossRedirectionUrl;
                //}
                if (objServiceInfo.Moss.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    message.InnerHtml = trans.getTranslatedText(MOSS_SESSION_TIMEOUT_KEY, objServiceInfo.MossCultureCode);
                }
                /* IM01335823- New Agricast - Session timed out - Return URL and Iframe - Jerrey - End */
            }

            /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - END*/
            if (Request.QueryString["ExtUsrAD"] != null)
            {
                switch (Request.QueryString["ExtUsrAD"])
                {
                    case "1":
                        message.InnerHtml = "Access Denied - " + INVALID_KEY;
                        break;
                    case "2":
                        message.InnerHtml = "Access Denied - " + INVALID_TOKEN;
                        break;
                    case "3":
                        message.InnerHtml = "Access Denied - " + INVALID_PUB;
                        break;
                }
            }
        }
        /* Agricast CR - 3.5	R5 - External user management changes. - End */
    }
}