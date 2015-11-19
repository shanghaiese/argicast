<%@ WebHandler Language="C#" Class="AutoComplete" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using Syngenta.AgriCast.LocationSearch.Presenter;
using Syngenta.AgriCast.Common;



public class AutoComplete : IHttpHandler, System.Web.SessionState.IRequiresSessionState 
{
    locSearchPresenter objLocPre = new locSearchPresenter();
    Syngenta.AgriCast.Common.DTO.LocationInfo objLocInfo;
    string provider;
     
    public void ProcessRequest(HttpContext context)
    {
     
       string strJSON = string.Empty;
       List<string> itemList = new List<string>();
        if (context.Request.QueryString["q"] != null)
        {
           // context.Response.Write("test");
            string strSearchString = HttpUtility.HtmlDecode(context.Request.QueryString["q"]);
            string limit = context.Request.QueryString["limit"];
            string strCountry = context.Request.QueryString["country"];
            if (strSearchString.Length >= 3)
            {
                //DataTable dtSuggestions = new DataTable();
                //dtSuggestions =
                 itemList= objLocPre.getAutoSuggestLocation(strCountry, strSearchString);
                //for (int i = 0; i < dtSuggestions.Rows.Count; i++)
                //{
                //    string strMatchText = dtSuggestions.Rows[i][0].ToString();
                //    if (Regex.IsMatch(strMatchText, strSearchString, RegexOptions.IgnoreCase))
                //    {
                //        itemList.Add(strMatchText);
                //    }
                //}


                strJSON = String.Join(System.Environment.NewLine, itemList.Take(int.Parse(limit)).ToArray());
            

            }
        }
    

         
        context.Response.ContentType = "text/plain";
        context.Response.Write(strJSON);
    }
    
    [WebMethod]
    public static string GetDate(string name)
    {
        string x = name;
        return DateTime.Now.ToString();
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}