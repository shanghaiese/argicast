<%@ Application Language="C#" %>


<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        Dictionary<string, object> glob = new Dictionary<string,object>();
        //Dictionary<string, string> culture = new Dictionary<string,string>();
        //culture.Add("Stations","Staionos");
        //glob.Add("fr-FR",culture);
        //Application["globalization"] = glob;       
        double expiry = double.Parse(ConfigurationManager.AppSettings["CacheExpiry"].ToString());
        HttpRuntime.Cache.Insert("globalization",glob,null,DateTime.Now.AddMinutes(expiry),Cache.NoSlidingExpiration);
        
        Syngenta.AgriCast.ExceptionLogger.AgriCastLogger.Configure("AgriCast", "ExceptionLogger.xml");
       // Syngenta.AgriCast.ExceptionLogger.AgriCastLogger.Configure("AgriCastAudit", "AuditLogger.xml"); 
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown 
        //Application.Remove("globalization");
        //Application.Clear();  
        HttpRuntime.Cache.Remove("globalization");                   
    }
        
    void Application_Error(object sender, EventArgs e) 
    {       
        // Code that runs when an unhandled error occurs    
        //Response.Redirect("ErrorPage.aspx");         
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
        HttpContext.Current.Session["ErrorMessage"] = null;

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
        
        //string[] filePaths = System.IO.Directory.GetFiles(HttpRuntime.AppDomainAppPath + @"Temp\");
        //foreach (string filePath in filePaths)
        //    System.IO.File.Delete(filePath);
        (from f in new System.IO.DirectoryInfo(HttpRuntime.AppDomainAppPath + @"Temp\").GetFiles()          
         select f).ToList().ForEach(f => f.Delete()); 

        (from f in new System.IO.DirectoryInfo(HttpRuntime.AppDomainAppPath + @"Temp\cfx").GetFiles()
         where f.CreationTime < DateTime.Now.Subtract(TimeSpan.FromHours(1))
         select f).ToList().ForEach(f =>f.Delete()); 

    }
       
</script>
