/* Agricast CR - 3.5	R5 - External user management changes. - Begin */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using Syngenta.AgriCast.WebService.View;
using Syngenta.AgriCast.WebService.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Web.Security;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common;
using System.Configuration;
using Syngenta.Agriweb.SynCryptography;

namespace Syngenta.AgriCast.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ExternalService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ExternalService : IExternalService
    {
        HttpContext _context;
        private string _encryptKey;

        public ExternalService()
        {
            if (_context == null)
            {
                if (HttpContext.Current != null)
                {
                    _context = HttpContext.Current;
                }
                else
                {
                    _context = new HttpApplication().Context;
                }
            }
            _encryptKey = ConfigurationManager.AppSettings["EncryptKey"];
        }

        #region // html encode & decode
        private string EncodeString(string sourceString)
        {
            return HttpUtility.HtmlEncode(sourceString);
        }
        private string DecodeString(string sourceString)
        {
            return HttpUtility.HtmlDecode(sourceString);
        }
        #endregion

        string IExternalService.GetSecurityKey(string token, string userID)
        {
            // encoding Token & userID
            var encodedToken = EncodeString(token);
            var encodedUserID = EncodeString(userID);
            // get current date time as string
            var dateNow = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
            var encString = String.Format("{0}^{1}^{2}", encodedToken, encodedUserID, dateNow);
            // encrypt & encode these information
            var encryptedString = SynCryptographyConsumer.Encrypt(encString, _encryptKey,"New Agricast","Direct");
            return EncodeString(encryptedString.Replace("+", "~"));
            //var strEncryptedKey=EncodeString(encryptedNode.LastChild.InnerText.Replace("+", "~"));

            //StringBuilder sb=new StringBuilder();
            //sb.Append("<ExternalService>");
            //sb.Append("<SecurityKey>");
            //sb.Append(strEncryptedKey);
            //sb.Append("</SecurityKey>");
            //sb.Append("</ExternalService>");

            //return sb.ToString();
        }

        private ArrayList GetServiceDetails()
        {
            ArrayList alError = new ArrayList();
            alError.Add("ExternalService");
            alError.Add("ExternalService");
            alError.Add(string.Empty);
            return alError;
        }

        string IExternalService.Version()
        {
            try
            {
                return "$Id: ExternalService 266 2012-09-06 13:00:19Z Infosys $\n";
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                return "";
            }
        }


        string IExternalService.DecryptSecurityKey(string encryptedKey)
        {
            return SynCryptographyConsumer.Decrypt(DecodeString(encryptedKey).Replace("~", "+"), _encryptKey, "New Agricast", "Direct");
        }
    }
}

/* Agricast CR - 3.5	R5 - External user management changes. - End */
