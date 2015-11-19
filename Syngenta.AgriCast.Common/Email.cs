using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Syngenta.AgriCast.Common
{
    public class Email
    {
        
        

        SmtpClient smtpClientObj;
        string smtpServer = System.Configuration.ConfigurationManager.AppSettings["MailConfig"];
        public void SendEmail(string strUserId, string strUserPwd, string strUserName, string strSubject, string strMailBody, string strFrom, string strTo, string strAttachmentpath)
        {
            //******************************
            // Method responsible for writing out the e-mail to be sent
            //******************************
            try
            {

                string server = System.Configuration.ConfigurationManager.AppSettings["MailConfig"];



                MailMessage mail = new MailMessage(strFrom.ToString(), strTo.ToString());
                if (strAttachmentpath != "" && strAttachmentpath != null)
                {
                    Attachment attachFile = new Attachment(strAttachmentpath);
                    mail.Attachments.Add(attachFile);
                }
                mail.Subject = strSubject;
                mail.Body = strMailBody;
                mail.IsBodyHtml = true;
                smtpClientObj = new SmtpClient(smtpServer);
                //smtpClientObj.Host = "localhost";
                smtpClientObj.Port = 25;
                smtpClientObj.Send(mail);

            }
            catch (SmtpException smtpex)
            {
                HttpContext.Current.Session["ErrorMessage"] = "The following error has occured on the page : " + smtpex.Message.ToString();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = "The following error has occured on the page : " + ex.Message.ToString();
            }

        }
    }
}