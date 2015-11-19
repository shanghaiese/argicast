using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using System.Web.Caching;
using System.Web;
using System.Web.UI;
using Syngenta.AgriCast.Common.Service;

namespace Syngenta.AgriCast.Common.DTO
{
    public class ServiceInfo
    {
        private string strCulture = "";
        private string strCountry = "";
        private string strUnit = "";
        private string strWUnit = "";
        private string strModule = "";
        private string strService = "";
        private string strMoss = "";
        private string strAllowedRoles = "";
        private ModuleInfo objModuleInfo;
        private static ServiceInfo objServiceInfo;
        //SOCB on 20-Mar-2012 for reading only mobile template
        private bool bolIsMobile;
        //EOCB on 20-Mar-2012 for reading only mobile template
        // private string code;
        // private string text;
        // private string cultureCodeSession;
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        string strMossCultureCode = string.Empty;
        string strMossRedirectionUrl = string.Empty;
       
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        public string Culture
        {
            get
            {
                return strCulture;
            }
            set
            {
                strCulture = value;
            }
        }
        public string Module
        {
            get
            {
                return strModule;
            }
            set
            {
                strModule = value;
            }
        }
        public string Country
        {
            get
            {
                return strCountry;
            }
            set
            {
                strCountry = value;
            }
        }

        public string Unit
        {
            get
            {
                return strUnit;
            }
            set
            {
                strUnit = value;
            }
        }
        public string WUnit
        {
            get
            {
                return strWUnit;
            }
            set
            {
                strWUnit = value;
            }
        }

        public string ServiceName
        {
            get
            {
                return strService;
            }
            set
            {
                strService = value;
            }
        }
        public string AllowedRoles
        {
            get
            {
                return strAllowedRoles;
            }
            set
            {
                strAllowedRoles = value;
            }
        }
        //SOCB on 20-Mar-2012 for reading only mobile template
        public Boolean IsMobile
        {
            get
            {
                return bolIsMobile;
            }
            set
            {
                bolIsMobile = value;
            }
        }
        public string Moss
        {
            get
            {
                return strMoss;
            }
            set
            {
                strMoss = value;
            }
        }

        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        public string MossCultureCode
        {
            set
            {
                strMossCultureCode = value;
            }
            get
            {
                return strMossCultureCode;
            }
        }

        public string MossRedirectionUrl
        {
            set
            {
                strMossRedirectionUrl = value;
            }
            get
            {
                return strMossRedirectionUrl;
            }
        }

       

        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/


        //EOCB on 20-Mar-2012 for reading only mobile template
        /*public ModuleInfo ModuleInfo
        {
            get
            {
               return ModuleInfo.getModuleInfoObject;
            }             
        }*/

        /*  public string Item
          {
              get {
                  return code;
              }
              set {
                  code = value;
              }
          }

          public string Text
          {
              get
              {
                  return text;
              }
              set
              {
                  text = value;
              }
          }*/

        /*  public string CultureCodeSession
          {
              get
              {
                  return cultureCodeSession;
              }


              set
              {                
                  cultureCodeSession = value;
              }
          }*/

        public static ServiceInfo ServiceConfig
        {
            get
            {
                if (CommonUtil.IsSessionAvailable())
                {

                    HttpSessionState session = HttpContext.Current.Session;

                    if (session == null || session["serviceInfo"] == null)
                    {
                        objServiceInfo = new ServiceInfo();
                        objServiceInfo.Culture = "";
                        session.Add("serviceInfo", objServiceInfo);
                        session["serviceInfo"] = objServiceInfo;
                    }
                    return (ServiceInfo)session["serviceInfo"];
                }
                else
                {
                    if (objServiceInfo == null) objServiceInfo = new ServiceInfo();
                    return objServiceInfo;
                }
            }
            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    Session["serviceInfo"] = value;
                }
                else
                {
                    objServiceInfo = value;
                }
            }
        }
    }
}
