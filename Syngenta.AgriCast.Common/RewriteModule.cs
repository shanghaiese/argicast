using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Syngenta.AgriCast.Common.DTO;
using System.Web.SessionState;

namespace Syngenta.AgriCast.Common
{

    class RewriteModule : System.Web.UI.Page, IHttpModule
    {
        ServiceInfo objSvcInfo;
        string strFinalUrl = string.Empty;
        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            // 
            context.BeginRequest += new EventHandler(RewriteModule_BeginRequest);
            context.PreRequestHandlerExecute += new EventHandler(RewriteModule_PreRequestHandlerExecute);
        }

        void RewriteModule_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if ((app.Context.CurrentHandler is Page) && app.Context.CurrentHandler != null)
            {
                Page pg = (Page)app.Context.CurrentHandler;
                pg.PreInit += new EventHandler(Page_PreInit);
            }

        }

        void Page_PreInit(object sender, EventArgs e)
        {
            // restore internal path to original
            // this is required to handle postbacks
            if (HttpContext.Current.Items.Contains("OriginalUrl"))
            {
                string path = (string)HttpContext.Current.Items["OriginalUrl"];

                // save query string parameters to context
                RewriteContext con = new RewriteContext(HttpContext.Current.Request.QueryString, path);

                HttpContext.Current.Items["RewriteContextInfo"] = con;

                if (path.IndexOf("?") == -1)
                    path += "?";
                HttpContext.Current.RewritePath(path);
                //HttpContext.Current.RewritePath(strFinalUrl);
            }
           
        }

        /// <summary>
        /// To check if request is made from mobile device.
        /// </summary>
        /// <returns>Bool</returns>
        public static bool isMobileBrowser()
        {
            HttpContext context = HttpContext.Current;

            //CHECK .Net built in IsMobileDevice property
            if (context.Request.Browser.IsMobileDevice)
            {
               // return true;
                return false;
            }

            //CHECK THE HTTP_USER_AGENT if not found in IsMobileDevice
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //agents for major browsers in mobile devices based on priority
                //1st - Major mobile OS, and commonly used devices
                //2nd - Optional browsers
                //3rd - Others native browsers by mobile carrier providers
                string[] agents =
                    new[]
                {
                    "android", "iphone", "ipad", "ipod", "blackberry", "symbian",
                    "webos", "opera mini", "iemobile", "meego", "Dolphin",
                    "SonyEricsson", "Samsung", "nokia", "Vodafone",
                    "midp", "j2me", "avant", "palmos", "palmsource", "HTC"
                };

                //check for mobile text in user agent string
                foreach (string ag in agents)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                        ToLower().Contains(ag.ToLower()))
                    {
                        // return true;
                        return false;
                    }
                }
            }
            //return not a mobile device request
            return false;
        }

        void RewriteModule_BeginRequest(object sender, EventArgs e)
        {

            RewriteModuleSectionHandler cfg = (RewriteModuleSectionHandler)ConfigurationManager.GetSection("modulesSection/RewriteModule");
            objSvcInfo = new ServiceInfo();

            // module is turned off in web.config
            if (!cfg.RewriteOn) return;

            string path = HttpContext.Current.Request.Path;
           
            // there is nothing to process
            if (path.Length == 0) return;

            string lastsegment = path.Substring(path.LastIndexOf('/') + 1);
            if (!lastsegment.Contains('.') && !path.ToLower().Contains("webservice"))
            {
                path += "/default.aspx";
                //HttpContext.Current.Response.Redirect(path);
            }

            //For aspx page requests that do not have the pubname,add it in the correct format
            if (path.Contains(".aspx"))
            {

                if (!((path.Contains("pub/")) || (path.Contains("Pub/"))))
                {
                    int i = path.LastIndexOf("/");
                    string defaultPubName = ConfigurationManager.AppSettings.Get("defaultPub").ToString();
                    string newpath = cfg.RewriteBase + "Pub/" + defaultPubName + path.Substring(i);
                    path = newpath;
                    // save rewritten path with pubname as original to HttpContext for further use
                    HttpContext.Current.Items.Add("OriginalUrl", path);
                    //Redirect to new path
                    if (HttpContext.Current.Request.QueryString.Count > 0)
                        HttpContext.Current.Response.Redirect(path + "?" + HttpContext.Current.Request.QueryString.ToString());
                    else
                        HttpContext.Current.Response.Redirect(path);
                }

                else
                {
                    if (!lastsegment.Contains('.') && !path.ToLower().Contains("webservice"))
                    {
                        if (HttpContext.Current.Request.QueryString.Count == 0)
                            HttpContext.Current.Response.Redirect(path);
                    }

                    // save original path to HttpContext for further use
                    HttpContext.Current.Items.Add(
                        "OriginalUrl",HttpContext.Current.Request.RawUrl);
                }

            }
            // load rewriting rules from web.config
            // and loop through rules collection until first match
            XmlNode rules = cfg.XmlSection.SelectSingleNode("rewriteRules");
            foreach (XmlNode xml in rules.SelectNodes("rule"))
            {
                try
                {
                    Regex re = new Regex(cfg.RewriteBase + xml.Attributes["source"].InnerText, RegexOptions.IgnoreCase);
                    Match match = re.Match(path);
                    if (match.Success)
                    { 
                        //SOCB 3-Feb-2012 for mobile pages redirection - Abhijit
                        if (!path.Contains("Mobile"))
                        {
                            bool isMobileRequest = isMobileBrowser();//HttpContext.Current.Request.Browser.IsMobileDevice;
                            string fullSite = "";
                            if (HttpContext.Current.Request.Cookies["fullsite"] != null)
                            {
                                fullSite = (HttpContext.Current.Request.Cookies["fullsite"].Value ?? "").ToString();
                            }

                            if ((!isMobileRequest && string.IsNullOrWhiteSpace(fullSite)) || (isMobileRequest && fullSite == "1"))
                            {
                                path = re.Replace(path, xml.Attributes["destination"].InnerText);
                            }
                            else
                            {
                                path = re.Replace(path, xml.Attributes["mobdest"].InnerText);
                            }
                        }
                        else
                        {
                            path = re.Replace(path, xml.Attributes["mobdest"].InnerText);
                        }
                        //EOCB 3-Feb-2012 for mobile pages redirection - Abhijit

                        if (path.Length != 0)
                        {
                            // check for QueryString parameters
                            if (HttpContext.Current.Request.QueryString.Count != 0)
                            {
                                // if there are Query String papameters
                                // then append them to current path
                                string sign = (path.IndexOf('?') == -1) ? "?" : "&";

                                {
                                    path = path + sign + HttpContext.Current.Request.QueryString.ToString();


                                }
                            }
                            // new path to rewrite to
                            string rew = cfg.RewriteBase + path;

                            // rewrite and save for future use
                            HttpContext.Current.Items.Add("RedirectedUrl", rew);
                            HttpContext.Current.RewritePath(rew);
                        }
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw (new Exception("Incorrect rule.", ex));
                }
            }
           
            return;
        }

    }

}