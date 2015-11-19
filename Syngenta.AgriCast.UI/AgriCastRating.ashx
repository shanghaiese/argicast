<%@ WebHandler Language="C#" Class="AgriCastRating" %>

using System;
using System.Web;
using System.Collections.Generic;
using Syngenta.AgriCast.Common.DTO;
using System.Web.SessionState;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.Service;

public class AgriCastRating : IHttpHandler, System.Web.SessionState.IRequiresSessionState 
{
    ServiceInfo objSvcInfo;
    ServicePresenter objSvcPre = new ServicePresenter();
    ServiceHandler sh;
    public const string  DB_LANG = "en-GB";
    public void ProcessRequest (HttpContext context) {

        string Rating="";
        string Module="";
        string Service="";
        if (context.Request.QueryString.Count > 0)
        {
            Rating = context.Request.QueryString["rate"].ToString();
             Module = context.Request.QueryString["module"].ToString();
             Service = context.Request.QueryString["service"].ToString();
        }
        else
        {
             Rating = context.Request.Form["rate"]; 
        }
        bool requiresSession = false;

        if (context.Handler is IRequiresSessionState)
            requiresSession = true;

        if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
        {

            objSvcInfo = new ServiceInfo();
        }
        else
        {
            objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
        }
               
        //for embed js rating saving
        if (!string.IsNullOrEmpty(Service) && (Rating != null))
        {
            objSvcInfo.ServiceName = Service;
            if (Module == "")
            {
                sh = new ServiceHandler();
                ServiceInfo.ServiceConfig = objSvcInfo;
                service svc = objSvcPre.readConfig();
                objSvcPre.createServiceSession();
                Module = sh.getDefaultPage();
            }

            Rating = Rating.Substring(7) + '%' + Module;
            List<string[]> lstRatings = new List<string[]>();
            string[] al;
            al = Rating.Split('%');

            if (al.Length > 0)
            {
                al[0] = TranslatedText(al[0]);
            }
            lstRatings.Add(al);
            objSvcPre.SaveRatings(lstRatings);            
            
            System.Text.StringBuilder sbJson = new System.Text.StringBuilder();
            sbJson.Append(@"{""msg"":""");
            sbJson.Append("saved");
            sbJson.Append(@""" }");
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(string.Format("{0}({1});", context.Request["callback"], sbJson.ToString()));
        }
        else if (Rating != null)
        {           

            Rating = Rating.Substring(7) + '#' + objSvcInfo.Module;
            List<string[]> lstRatings;// = new List<string[]>();
            string[] al;
            al = Rating.Split('#');

            if (al.Length > 0)
            {
                al[0] = TranslatedText(al[0]);
            }
            
            if (HttpContext.Current.Session["Rating"] != null)
            {
                lstRatings = (List<string[]>)HttpContext.Current.Session["Rating"];
                lstRatings.Add(al);
            }
            else
            {
                lstRatings = new List<string[]>();
                lstRatings.Add(al);
                HttpContext.Current.Session["Rating"] = lstRatings;
            }
        }       
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
    private string TranslatedText(string label)
    {
        if (objSvcInfo == null)
            objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

        return objSvcPre.getTranslatedText(label, DB_LANG);
         
    }
}