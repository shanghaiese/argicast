using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Net.Mail;
using System.Text;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common;

public partial class ForgotPassword : BasePage
{
    string strUserName = string.Empty;
    string strPassword = string.Empty;
    string strEmailId = string.Empty;
    MembershipUser memUser = null;
    ServiceInfo objServ = null;
    CommonUtil objCommonUtil =  new CommonUtil();
    protected void Page_Load(object sender, EventArgs e)
    {
    

        //Get the Service Object from sEssion
        objServ = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

        if (objServ != null)
        {
           
            //Set the Control names based on the Culture Code
            lblUserName.InnerText = objCommonUtil.getTransText(lblUserName.Attributes["name"]);
            btnSendToMail.Text = objCommonUtil.getTransText(btnSendToMail.Attributes["name"]);
        }

    }
    protected void btnSendToMail_Click(object sender, EventArgs e)
    {
        strUserName = txtUserName.Text.Trim();
        if (!string.IsNullOrEmpty(strUserName))
        {
            memUser = Membership.GetUser(strUserName);

            if (memUser != null)
            {
                strPassword = memUser.GetPassword();
                strEmailId = memUser.Email;
            }

            //Send the E Mail
            if (!string.IsNullOrEmpty(strEmailId))
            {
                SendEmail();
            }
            else
            {
                lblMessage.Text = objCommonUtil.getTransText(Constants.GEN_EMAILIDNOTFOUND);
                lblMessage.CssClass = "Error";
            }
        }
    }


    public void SendEmail()
    {


        //Form the Strcuture of EMail
        SmtpClient smtpClientObj = null;
        string strFrom = string.Empty;
        string strTo = string.Empty;
        string strSubject = string.Empty;
        StringBuilder strMailBody = new StringBuilder();

        MailMessage objMail = null;
        string smtpServer = System.Configuration.ConfigurationManager.AppSettings["MailConfig"];
        //******************************
        // Method responsible for writing out the e-mail to be sent
        //******************************
        try
        {

            strFrom = "Agricast-Team@agricast.com";
            strTo = strEmailId;
            strSubject = "Agricast - Password";

            objMail = new MailMessage(strFrom, strTo);

            objMail.Subject = strSubject;
           
            //Form the Mail Body
            strMailBody.Append("<html><head></head><title></title><body>");
            strMailBody.Append(" <table><tr><td>Dear "+strUserName+"");
            strMailBody.Append("</td></tr><tr><td><br /></td></tr>");
            strMailBody.Append("<tr><td>Your Agricast Login Password is "+strPassword+"</td></tr>");
            strMailBody.Append("<tr><td><br /></td></tr><tr><td>Regards,</td></tr>");
            strMailBody.Append("<tr><td style=\"font-weight: bold\">Team Agricast</td></tr>");
            strMailBody.Append("</table></body></html>");
            objMail.Body = strMailBody.ToString();
            objMail.IsBodyHtml = true;
            smtpClientObj = new SmtpClient(smtpServer);
            //smtpClientObj.Host = "localhost";
            smtpClientObj.Port = 25;
            smtpClientObj.Send(objMail);

        }
        catch (SmtpException smtpex)
        {
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + " : " + smtpex.Message.ToString();
            lblMessage.Text = HttpContext.Current.Session["ErrorMessage"].ToString();
        }
        catch (Exception ex)
        {
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GENERIC_ERRORONPAGE) +" : " + ex.Message.ToString();
            lblMessage.Text = HttpContext.Current.Session["ErrorMessage"].ToString();
        }
    }
}