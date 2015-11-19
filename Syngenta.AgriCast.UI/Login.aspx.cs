using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common.DTO;
using System.Web.Security;
using Syngenta.Agricast.UserManagement;
using System.Web.Profile;
using Syngenta.AgriCast.ExceptionLogger;
using System.Collections;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.Service;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.Common.Presenter;
public partial class Login : System.Web.UI.Page
{
    UserInfo objUserInfo = new UserInfo(); 
    MembershipUser memUser = null;
    ServiceInfo objServ = null;
     string strSessionPubName = string.Empty;
    string strUserName = string.Empty;
    string strPassword = string.Empty;
    ProfileCommon objProf = null;
    CustomProfile cus = null;
    const string LOGIN_MODULE = "Login";
    ArrayList al;
    string RedirectedUrl = string.Empty;
    string subStrURL = string.Empty;
    string strNewPubName = string.Empty;
    int lastindex = 0;
    bool Allow = false;
    CommonUtil objCommonUtil = new CommonUtil();
    string strFinalPubName = string.Empty;
    Controls common = new Controls();
    protected void Page_Load(object sender, EventArgs e)
     {
            
            //set the Control labels based on the culture code
       
            //Read the Pub Name from the URL
            RedirectedUrl = HttpContext.Current.Request.Url.ToString();
            subStrURL = RedirectedUrl.Substring(0, RedirectedUrl.LastIndexOf('/'));
            //Split and Get the pub name
            if (!string.IsNullOrEmpty(RedirectedUrl))
            {
                subStrURL = RedirectedUrl.Substring(0, RedirectedUrl.LastIndexOf('/'));
                lastindex = RedirectedUrl.LastIndexOf("Pub/", StringComparison.InvariantCultureIgnoreCase);
                strNewPubName = subStrURL.Substring(lastindex + 4);
            }

            //Get the Service Object from sEssion
            objServ = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];


            //One post back faiulure - issue fix
            //redirect the authenticated user to default page when login .aspx is givn in the url after login
            UserInfo objUser = (UserInfo)HttpContext.Current.Session["objUserInfo"];
            
            if (objServ == null || (objUser!=null && objUser.IsAuthenticated))
            {
                Response.Redirect(subStrURL + "/default2.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

           //Set the Control names based on the Culture Code
            lblUserName.Text = objCommonUtil.getTransText(lblUserName.Attributes["name"]);
            lblPassword.Text = objCommonUtil.getTransText(lblPassword.Attributes["name"]);
            btnLogin.Text = objCommonUtil.getTransText(btnLogin.Attributes["name"]);
            aForgotPassword.InnerText = objCommonUtil.getTransText(aForgotPassword.Attributes["name"]);
            lgndLoginInfo.InnerText = objCommonUtil.getTransText(lgndLoginInfo.Attributes["name"]);
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

            divErrorMessage.Visible = false;
            if (Session["LoginError"] != null && Session["LoginError"].ToString() != "")
            {
                lblLoginMessage.Text = objCommonUtil.getTransText(Session["LoginError"].ToString());
                divErrorMessage.Visible = true;
                loginDiv.Visible = false;
                divLoginLink.Visible = true;
                lblClick.Text = objCommonUtil.getTransText(Constants.LOGIN_CLICK);
                lnkLogin.InnerHtml = objCommonUtil.getTransText(Constants.LOGIN_HERE);
                lblLoginAgain.Text = objCommonUtil.getTransText(Constants.LOGIN_LOGIN_AGAIN);
                lnkLogin.HRef = HttpContext.Current.Request.Url.ToString();
                Session.Remove("LoginError");
            }
        
        //Set the footer in Login.aspx
            setFooter();
    }
    void setFooter()
    {
       
        //set the master page controls

        HtmlAnchor hplTerms = (HtmlAnchor)common.FindControlRecursive(this, "lnkTnC");
        hplTerms.InnerHtml = objCommonUtil.getTransText(Constants.TERMS_AND_CONDITIONS);

        HtmlAnchor hplPrivPolicy = (HtmlAnchor)common.FindControlRecursive(this, "lnkPrivPolicy");
        hplPrivPolicy.InnerText = objCommonUtil.getTransText(Constants.PRIVACY_POLICY);

        HtmlAnchor hplContacts = (HtmlAnchor)common.FindControlRecursive(this, "lnkContacts");
        hplContacts.InnerText = objCommonUtil.getTransText(Constants.CONTACTS);

        HtmlAnchor hplMobile = (HtmlAnchor)common.FindControlRecursive(this, "lnkMobile");
        hplMobile.InnerText = objCommonUtil.getTransText(Constants.MOBILE_SITE);

    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        strUserName = txtUsername.Text.Trim();
        
        strPassword = txtPassword.Text.Trim();

        divErrorMessage.Visible = false;
        lblLoginMessage.Text = string.Empty;

        try
        {
            if (!string.IsNullOrEmpty(strUserName) && !string.IsNullOrEmpty(strPassword))
            {

                bool blValResult = Membership.ValidateUser(strUserName, strPassword);
                if (blValResult)
                {
                //    memUser = Membership.GetUser(strUserName);
                ////Get all the roles for the signed in user
                // string[] roles =   Roles.GetRolesForUser(strUserName);
                //    //Get the roles allowed in the service
                // string[] AllowedRoles =   objServ.AllowedRoles.Split(',');
                // if (AllowedRoles[0] != string.Empty)
                // {
                //     foreach (string strAllowedRole in AllowedRoles)
                //     {

                //         var srchString = strAllowedRole;
                //         var strFound = Array.FindAll(roles, str => str.ToLower().Trim().Equals(srchString.ToLower().Trim()));
                //         if (strFound.ToList().Count > 0)
                //         {
                //             //if role exists in the roles for the particular user, set flag allow to true
                //             Allow = true;
                //             break;
                //         }
                //     }
                // }
                // else
                // {
                //     Response.Redirect("Default.aspx", false);
                //     //Server.Transfer("Default.aspx");
                // }

                    //if (Allow)
                    //{

                        objProf = Profile.GetProfile(strUserName);
                        cus = objProf.MyCustomProfile;
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
                            Response.Redirect(str + "/default2.aspx", false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();                       
                        }

                    //}
                    //else
                    //{
                    //    lblLoginMessage.Text =  objCommonUtil.getTransText(Constants.LOGIN_NOPUBACCESS);
                    //    divErrorMessage.Visible = true;
                    //}
                }
                else
                {
                    lblLoginMessage.Text = objCommonUtil.getTransText(Constants.LOGIN_INVALID_CREDENTIALS);
                    divErrorMessage.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            lblLoginMessage.Text = objCommonUtil.getTransText(Constants.LOGIN_GENERIC_ERROR);
            divErrorMessage.Visible = true;
            al = new ArrayList();
            al.Add(strFinalPubName);
            al.Add(LOGIN_MODULE);
            al.Add(objUserInfo.UserName);
            AgriCastException currEx = new AgriCastException(al, ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
        }

    }



}
