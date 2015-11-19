/* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
/* 2.1	If mobile site is enabled or restricted service then login page should appear. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common.Service;
using Syngenta.Agricast.UserManagement;
using Syngenta.AgriCast.Common.DTO;
using System.Web.Security;
using Syngenta.AgriCast.Common;
using System.Collections;
using Syngenta.AgriCast.ExceptionLogger;
using System.Web.UI.HtmlControls;

public partial class Mobile_Login : System.Web.UI.Page
{
    CommonUtil objCommonUtil = new CommonUtil();
    const string LOGIN_MODULE = "Login";
    string strFinalPubName = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        string strSessionPubName = string.Empty;
        string strNewPubName = string.Empty;

        //Read the Pub Name from the URL
        string RedirectedUrl = HttpContext.Current.Request.Url.ToString();
        string subStrURL = RedirectedUrl.Substring(0, RedirectedUrl.LastIndexOf('/'));
        //Split and Get the pub name
        if (!string.IsNullOrEmpty(RedirectedUrl))
        {
            subStrURL = RedirectedUrl.Substring(0, RedirectedUrl.LastIndexOf('/'));
            int lastindex = RedirectedUrl.LastIndexOf("Pub/", StringComparison.InvariantCultureIgnoreCase);
            strNewPubName = subStrURL.Substring(lastindex + 4);
        }

        //Get the Service Object from sEssion
        ServiceInfo objServ = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

        //One post back faiulure - issue fix
        //redirect the authenticated user to default page when login .aspx is givn in the url after login
        UserInfo objUser = (UserInfo)HttpContext.Current.Session["objUserInfo"];

        if (objServ == null || (objUser != null && objUser.IsAuthenticated))
        {
            Response.Redirect(subStrURL + "/default.aspx", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        //Set the Control names based on the Culture Code
        lblUserName.Text = objCommonUtil.getTransText(lblUserName.Attributes["name"]);
        lblPassword.Text = objCommonUtil.getTransText(lblPassword.Attributes["name"]);
        btnLogin.Text = objCommonUtil.getTransText(btnLogin.Attributes["name"]);
        h1Welcome.InnerText = objCommonUtil.getTransText(h1Welcome.Attributes["name"]);

        if (objServ != null)
        {
            //Set the Pub Name
            strSessionPubName = objServ.ServiceName;
        }
        else
        {
            strSessionPubName = System.Configuration.ConfigurationManager.AppSettings["defaultPub"].ToString();
        }

        // check if the Pub Name in Session and URL are same
        if (strSessionPubName.Equals(strNewPubName))
        {
            //Assign the Session Value
            strFinalPubName = strSessionPubName;
        }
        else
        {
            //Assign the New URL Value
            strFinalPubName = strNewPubName;
        }

        //Add this new value to session
        if (objServ == null)
            objServ = ServiceInfo.ServiceConfig;
        objServ.ServiceName = strFinalPubName;
        Session["serviceInfo"] = objServ;

        HtmlAnchor btnBack = (HtmlAnchor)this.Master.FindControl("btnBack");
        btnBack.Visible = false;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        UserInfo objUserInfo = new UserInfo();
        string strUserName = txtUsername.Text.Trim();
        string strPassword = txtPassword.Text.Trim();
        lblLoginMessage.Text = string.Empty;

        try
        {
            if (!string.IsNullOrEmpty(strUserName) && !string.IsNullOrEmpty(strPassword))
            {
                bool blValResult = Membership.ValidateUser(strUserName, strPassword);
                if (blValResult)
                {
                    ProfileCommon objProf = Profile.GetProfile(strUserName);
                    CustomProfile cus = objProf.MyCustomProfile;
                    //Add tgheAuthenticated user detiails
                    objUserInfo = objUserInfo.getUserInfoObject;
                    objUserInfo.UserId = 0;
                    objUserInfo.UserName = strUserName;
                    objUserInfo.IsAuthenticated = true;

                    //Add the User Object to Session
                    Session.Add("objUserInfo", objUserInfo);

                    //Add the UserFavouites to Session
                    Session["UserFavorites"] = objProf.MyCustomProfile.FavoriteList;

                    //Redirect to Default Page
                    if (Session["RedirectURL"] != null)
                    {
                        string str = Session["RedirectURL"].ToString();
                        str = str.Substring(0, str.LastIndexOf('/'));
                        str = str + "/" + strFinalPubName;
                        Response.Redirect(str + "/default.aspx", false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                else
                {
                    lblLoginMessage.Text = objCommonUtil.getTransText(Constants.LOGIN_INVALID_CREDENTIALS);
                }
            }
        }
        catch (Exception ex)
        {
            lblLoginMessage.Text = objCommonUtil.getTransText(Constants.LOGIN_GENERIC_ERROR);

            ArrayList al = new ArrayList();
            al.Add(strFinalPubName);
            al.Add(LOGIN_MODULE);
            al.Add(objUserInfo.UserName);
            AgriCastException currEx = new AgriCastException(al, ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
        }

    }
}

/* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */
