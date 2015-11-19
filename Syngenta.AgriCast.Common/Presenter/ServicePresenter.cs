using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.DTO;
using System.Collections;
using System.Web.SessionState;
using System.Data;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.ExceptionLogger;
using System.IO;
using System.Net;
using System.Configuration;
using System.Text;
using System.Xml;

namespace Syngenta.AgriCast.Common.Presenter
{
    public class ServicePresenter
    {
        Service.ServiceHandler ServiceObj = new Service.ServiceHandler();
        ServiceInfo objServiceInfo;
        DataPointInfo objDataPointInfo;
        //HttpSessionState session = HttpContext.Current.Session;
        service svc;
        IToolbar objITool;
        LocationInfo objLocInfo;
        IDefault objIDefault;
        CommonUtil util = new CommonUtil();
        ToolBarService objService = new ToolBarService();
        UserInfo objUserInfo;
        public const string LOC_DETAILS = "{Location Details}";

        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
        /* 3.3.1	Charting component should have zooming enabled. */
        /* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */
        IShowImage objIShowImg;
        /* Agricast CR - R6 - Add wind icons and legend for Humidity - End */
        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

        /// <summary>
        /// constructor
        /// </summary>
        public ServicePresenter(IToolbar ITool)
        {
            if (ITool != null)
            {
                objITool = ITool;
            }
        }

        public ServicePresenter(IDefault IDefault)
        {
            if (IDefault != null)
            {
                objIDefault = IDefault;
            }
        }

        public ServicePresenter()
        {
        }

        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
        /* 3.3.1	Charting component should have zooming enabled. */
        /* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */
        public ServicePresenter(IShowImage IShowImg)
        {
            if (IShowImg != null)
            {
                objIShowImg = IShowImg;
            }
        }

        public void getNodeList1(string nodename)
        {
            try
            {
                objIShowImg.alNodeList = ServiceObj.getNodeList(nodename);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.NODELIST_FETCH_FAILURE) + ":" + ex.Message.ToString();
            }
        }
        /* Agricast CR - R6 - Add wind icons and legend for Humidity - End */
        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

        public service readConfig()
        {
            ServiceObj.getServiceObj();
            return (service)HttpContext.Current.Session["service"];
        }

        public void loadCulture()
        {
            try
            {
                objITool.dtCultureCode = ServiceObj.loadCultureCode();

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CULTURE_LOADFAILURE) + ex.Message.ToString();
            }

        }

        public string getCultureCode()
        {
            return ServiceObj.getCultureCode();
        }

        public void getDefaultPage()
        {
            objIDefault.DefaultTab = ServiceObj.getDefaultPage();
            //return ServiceObj.getDefaultPage();
        }

        public bool checkModule(string modulename)
        {
            
            return ServiceObj.checkModule(modulename);
            //return ServiceObj.getDefaultPage();
        }

        public void loadUnits()
        {
            try
            {
                objITool.dtUnits = ServiceObj.loadUnits();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.UNITS_LOADFAILURE) + ex.Message.ToString();
            }
        }
        public void LoadCustomSettings()
        {
            try
            {
                objITool.strCustomUnits = ServiceObj.LoadCustomSettings();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.UNITS_LOADFAILURE) + ex.Message.ToString();
            }
        }
        public void createServiceSession()
        {
            try
            {
                ServiceObj.createServiceSession();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                // HttpContext.Current.Session["ErrorMessage"] = "The service session could not be created due to the following error : " + ex.Message.ToString();                
                HttpContext.Current.Response.StatusCode = 404;
                throw (new FileNotFoundException());//ex;

            }
        }

        public void setDefaultLoc()
        {
            try
            {
                IDictionary dict = null;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["AuditData"] != null)
                {
                    dict = (IDictionary)HttpContext.Current.Session["AuditData"];
                }
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                svc = readConfig();
                string defaultLoc = svc.locationSearch.defaultLocation;
                objLocInfo = LocationInfo.getLocationInfoObj;
                if (defaultLoc != "" && defaultLoc.Split(',').Count() > 1)
                {
                    if (objLocInfo.placeName == null || objLocInfo.placeName == "")
                    {
                        objLocInfo.placeName = defaultLoc.Split(',')[0];
                    }
                    if (objLocInfo.latitude == 0.0)
                    {
                        objLocInfo.latitude = Convert.ToDouble(defaultLoc.Split(',')[1]);
                    }
                    if (objLocInfo.longitude == 0.0)
                    {
                        objLocInfo.longitude = Convert.ToDouble(defaultLoc.Split(',')[2]);
                    }
                    if (dict != null)
                    {
                        dict["locSearchType"] = "defaultLocation";
                        HttpContext.Current.Session["AuditData"] = dict;
                    }
                }
                else if ((bool)(svc.locationSearch.useIPLocation))
                {
                    HttpRequest req = HttpContext.Current.Request;
                    //string url = "http://api.hostip.info/get_xml.php?position=true& ip=" + (req.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? req.ServerVariables["REMOTE_ADDR"]);
                    string url = "http://freegeoip.net/xml/" + ( req.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? req.ServerVariables["REMOTE_ADDR"] : req.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                    string proxyName = ConfigurationManager.AppSettings["proxyName"];
                    WebClient webClient = new WebClient();
                    webClient.Encoding = Encoding.UTF8;
                    string username = ConfigurationManager.AppSettings["UserName"] ?? "";
                    string domain = ConfigurationManager.AppSettings["Domain"] ?? "";
                    string password = ConfigurationManager.AppSettings["Password"] ?? "";
                    if (username != "" && password != "")
                        webClient.Credentials = new System.Net.NetworkCredential(username, password, domain);
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "test");
                    WebProxy proxy = new WebProxy(proxyName, true);
                    if (username != "" && password != "")
                        proxy.Credentials = new System.Net.NetworkCredential(username, password, domain);
                    webClient.Proxy = proxy;
                    try
                    {
                        string xml = webClient.DownloadString(url);
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(xml);
                        DataSet dsTest = new DataSet();
                        dsTest.ReadXml(new XmlNodeReader(xdoc));
                        if (dsTest != null && dsTest.Tables.Count > 0)
                        {
                            //if (dsTest.Tables.Contains("Hostip") && dsTest.Tables["Hostip"].Rows.Count > 0)
                            //{
                            //    if (dsTest.Tables["Hostip"].Rows[0]["countryAbbrev"].ToString() != "XX")
                            //    {
                            //        objLocInfo.placeName = dsTest.Tables["Hostip"].Rows[0]["name"].ToString().IndexOf(',') > 0 ? dsTest.Tables["Hostip"].Rows[0]["name"].ToString().Split(',')[0] : dsTest.Tables["Hostip"].Rows[0]["name"].ToString();
                            //        objLocInfo.CountryCode = dsTest.Tables["Hostip"].Rows[0]["countryAbbrev"].ToString();
                            //    }
                            //}
                            //if (dsTest.Tables.Contains("point") && dsTest.Tables["point"].Rows.Count > 0)
                            //{
                            //    string lnglat = dsTest.Tables["point"].Rows[0]["coordinates"].ToString();
                            //    if (lnglat != "")
                            //    {
                            //        objLocInfo.latitude = Double.Parse(lnglat.Split(',')[1].ToString());
                            //        objLocInfo.longitude = Double.Parse(lnglat.Split(',')[0].ToString());
                            //    }
                            //}
                            if (dsTest.Tables[0].Rows.Count > 0)
                            {
                                objLocInfo.placeName = dsTest.Tables[0].Rows[0]["City"].ToString();
                                objLocInfo.CountryCode = dsTest.Tables[0].Rows[0]["CountryCode"].ToString();
                                objLocInfo.latitude = Double.Parse(dsTest.Tables[0].Rows[0]["Latitude"].ToString());
                                objLocInfo.longitude = Double.Parse(dsTest.Tables[0].Rows[0]["Longitude"].ToString());
                            }
                        }
                        if (dict != null && objLocInfo.CountryCode != "")
                        {
                            dict["locSearchType"] = "UserIP";
                            HttpContext.Current.Session["AuditData"] = dict;
                        }
                    }
                    catch (Exception ex)
                    {
                        AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                        AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.TOOLBAR_DEFAULTLOCATION) + " : " + ex.Message.ToString();
            }
        }

        public void loadMenuTabs()
        {
            try
            {
                objIDefault.dtMenuData = ServiceObj.loadMenuTabs();

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.TOOLBAR_MENUTABS) + ":" + ex.Message.ToString();
            }
        }

        public void getNodeList(string nodename)
        {
            try
            {
                objIDefault.alNodeList = ServiceObj.getNodeList(nodename);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.NODELIST_FETCH_FAILURE) + ":" + ex.Message.ToString();
            }
        }

        /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - BEGIN*/
        //public void GetMossMenuItems(string ExtNavigation)
        public void GetMossMenuItems(string ExtNavigation, string MossCultureCode)
        {
            //DataSet ds = ServiceObj.GetMossMenuItems(ExtNavigation);
            DataSet ds = ServiceObj.GetMossMenuItems(ExtNavigation, MossCultureCode);

            if (ds != null)
                //objIDefault.dsMossMenu = ServiceObj.GetMossMenuItems(ExtNavigation);
                objIDefault.dsMossMenu = ServiceObj.GetMossMenuItems(ExtNavigation, MossCultureCode);
            else
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                /*IM01335823- New Agricast - Session timed out - Return URL and Iframe - Jerrey - Start */
                /* as BWM is only for moss site, this logic no longer been used. */
                ///*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                ///* after BWM page called, goto another pub, moss=true */
                //if (objServiceInfo.Module.ToLower() != "watermodel")
                //    objServiceInfo.Moss = "false";
                ///*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                /*IM01335823- New Agricast - Session timed out - Return URL and Iframe - Jerrey - End */
            }
        }
        /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - END*/

        public void GetRoles()
        {
            objIDefault.AllowedRoles = ServiceObj.GetRoles();
        }
        public void GetSecuritySetting()
        {
            objIDefault.IsSecure = ServiceObj.GetSecuritySetting();
        }
        public void GetEncryptionKey()
        {
            objIDefault.encryptKey = ServiceObj.GetEncryptionKey();
        }
        public void GetExtNavigation()
        {
            objIDefault.ExtNavigation = ServiceObj.GetExtNavigation();
        }
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        public void GetMossReturnUrl()
        {
            objIDefault.MossReturnUrl = ServiceObj.GetMossReturnUrl();
        }
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/

        public List<string[]> getModuleNodeList(string nodename)
        {
            return ServiceObj.getNodeList(nodename);
        }
        public ArrayList GetServiceDetails()
        {

            if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
            {

                objServiceInfo = new ServiceInfo();
            }
            else
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            }
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["objUserInfo"] == null)
            {

                objUserInfo = new UserInfo();
            }
            else
            {
                objUserInfo = (UserInfo)HttpContext.Current.Session["objUserInfo"];
            }

            ArrayList alError = new ArrayList();
            alError.Add(objServiceInfo.ServiceName);
            alError.Add(objServiceInfo.Module);
            if (objUserInfo != null) alError.Add(objUserInfo.UserName);
            else alError.Add(string.Empty);
            return alError;

        }
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {

            return objService.getTranslatedText(strLabelName, strCultureCode);
        }
        public void ExportToExcel(DataSet source)
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
            {

                objServiceInfo = new ServiceInfo();
            }
            else
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            }
            objLocInfo = LocationInfo.getLocationInfoObj;

            try
            {
                if (source != null)
                {
                    String fileName = HttpContext.Current.Request.PhysicalApplicationPath + "/Temp/report.xls";
                    System.IO.StreamWriter excelDoc;
                    string Loc = getTranslatedText(LOC_DETAILS, getCultureCode());
                    excelDoc = new System.IO.StreamWriter(fileName);
                    const string startExcelXML = "<xml version>\r\n<Workbook " +
                          "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                          " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                          "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                          "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                          "office:spreadsheet\">\r\n <Styles>\r\n " +
                          "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                          "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                          "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                          "\r\n <Protection/>\r\n </Style>\r\n " +
                          "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                          "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                          "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                          " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                          "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                          "ss:Format=\"0.0\"/>\r\n </Style>\r\n " +
                          "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                          "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                          "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                          "ss:Format=\"General Date\"/>\r\n </Style>\r\n " +
                          "</Styles>\r\n ";
                    const string endExcelXML = "</Workbook>";

                    int rowCount = 0;
                    int sheetCount = 1;


                    excelDoc.Write(startExcelXML);

                    for (int i = 0; i < source.Tables.Count; i++)
                    {
                        /* IM01246267 - New agricast - export to excel not working - Begin*/
                        //Excel Sheet Name Cannot Exceed 31 characters. truncate the sheet name if it  exceeds.
                        string strSheetName = source.Tables[i].TableName;
                        if (!string.IsNullOrEmpty(strSheetName))
                        {
                            if (strSheetName.Length > 31)
                            {
                                strSheetName = strSheetName.Substring(0, 30) + i;
                            }
                        }
                        //excelDoc.Write("<Worksheet ss:Name=\"" + source.Tables[i].TableName + "\">");
                        excelDoc.Write("<Worksheet ss:Name=\"" + strSheetName + "\">");

                        /* IM01246267 - New agricast - export to excel not working - Begin*/
                        excelDoc.Write("<Table>");
                        excelDoc.Write("<Column ss:AutoFitWidth=\"1\" ss:Width=\"108.75\" ss:Span=\"1\"/>");


                        //Adding the ist row in each table as the location details
                        excelDoc.Write("<Row><Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                        excelDoc.Write(Loc + " : " + objLocInfo.placeName + " , " + objLocInfo.latitude + " , " + objLocInfo.longitude);
                        excelDoc.Write("</Data></Cell>");
                        excelDoc.Write("</Row>");

                        excelDoc.Write("<Row><Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                        excelDoc.Write("");
                        excelDoc.Write("</Data></Cell>");
                        excelDoc.Write("</Row>");
                        excelDoc.Write("<Row>");

                        for (int x = 0; x < source.Tables[i].Columns.Count; x++)
                        {
                            excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");

                            excelDoc.Write(source.Tables[i].Columns[x].ColumnName);

                            excelDoc.Write("</Data></Cell>");
                        }
                        excelDoc.Write("</Row>");

                        foreach (DataRow x in source.Tables[i].Rows)
                        {
                            rowCount++;
                            //if the number of rows is > 64000 create a new page to continue output
                            if (rowCount == 64000)
                            {
                                rowCount = 0;
                                sheetCount++;
                                excelDoc.Write("</Table>");
                                excelDoc.Write(" </Worksheet>");
                                excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                                excelDoc.Write("<Table>");
                            }
                            excelDoc.Write("<Row>"); //ID=" + rowCount + "
                            for (int y = 0; y < source.Tables[i].Columns.Count; y++)
                            {
                                System.Type rowType;
                                rowType = x[y].GetType();
                                switch (rowType.ToString())
                                {
                                    case "System.String":
                                        if (y == 0 && objServiceInfo.ServiceName == "geotroll1")
                                        {
                                            var datetime = DateTime.Parse(x[y].ToString());
                                            string XMLstring = datetime.ToString("dd/MM/yyyy");
                                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                                          "<Data ss:Type=\"String\">");
                                            excelDoc.Write(XMLstring);
                                            excelDoc.Write("</Data></Cell>");
                                        }
                                        else
                                        {
                                            string XMLstring = x[y].ToString();
                                            XMLstring = XMLstring.Trim();
                                            XMLstring = XMLstring.Replace("&", "&");
                                            XMLstring = XMLstring.Replace(">", ">");
                                            XMLstring = XMLstring.Replace("<", "<");
                                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                                           "<Data ss:Type=\"String\">");
                                            excelDoc.Write(XMLstring);
                                            excelDoc.Write("</Data></Cell>");
                                        }

                                     //string XMLstring = x[y].ToString();
                                     //       XMLstring = XMLstring.Trim();
                                     //       XMLstring = XMLstring.Replace("&", "&");
                                     //       XMLstring = XMLstring.Replace(">", ">");
                                     //       XMLstring = XMLstring.Replace("<", "<");
                                     //       excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                     //                      "<Data ss:Type=\"String\">");
                                     //       excelDoc.Write(XMLstring);
                                     //       excelDoc.Write("</Data></Cell>");
                                        break;
                                    //case "System.DateTime":
                                    //    //Excel has a specific Date Format of YYYY-MM-DD followed by  
                                    //    //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                                    //    //The Following Code puts the date stored in XMLDate 
                                    //    //to the format above
                                    //    DateTime XMLDate = (DateTime)x[y];
                                    //    string XMLDatetoString = ""; //Excel Converted Date
                                    //    XMLDatetoString = XMLDate.Year.ToString() +
                                    //         "-" +
                                    //         (XMLDate.Month < 10 ? "0" +
                                    //         XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                    //         "-" +
                                    //         (XMLDate.Day < 10 ? "0" +
                                    //         XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                    //         "T" +
                                    //         (XMLDate.Hour < 10 ? "0" +
                                    //         XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                    //         ":" +
                                    //         (XMLDate.Minute < 10 ? "0" +
                                    //         XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                    //         ":" +
                                    //         (XMLDate.Second < 10 ? "0" +
                                    //         XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                    //         ".000";
                                    //    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                    //                 "<Data ss:Type=\"DateTime\">");
                                    //    excelDoc.Write(XMLDatetoString);
                                    //    excelDoc.Write("</Data></Cell>");
                                    //    break;
                                    case "System.DateTime":
                                        excelDoc.Write(string.Format("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">{0:s}</Data></Cell>", x[y]));
                                        // {0:s}={0:yyyy'-'MM'-'dd'T'HH':'mm':'ss} SortableDateTi­mePattern, culture independent
                                        break;

                                    case "System.Boolean":
                                        excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                                    "<Data ss:Type=\"String\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                        break;
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Byte":
                                        excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                                "<Data ss:Type=\"Number\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                        break;
                                    case "System.Decimal":
                                    case "System.Double":
                                        excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                              "<Data ss:Type=\"Number\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                        break;
                                    case "System.DBNull":
                                        excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                              "<Data ss:Type=\"String\">");
                                        excelDoc.Write("");
                                        excelDoc.Write("</Data></Cell>");
                                        break;
                                    default:
                                        throw (new Exception(rowType.ToString() + " not handled."));
                                }
                            }
                            excelDoc.Write("</Row>");
                        }

                        excelDoc.Write("</Table>");
                        excelDoc.Write(" </Worksheet>");
                    }
                    excelDoc.Write(endExcelXML);

                    excelDoc.Close();

                    FileStream sourceFile = new FileStream(HttpRuntime.AppDomainAppPath + @"Temp\report.xls", FileMode.Open);
                    string name = objServiceInfo.Module.ToString();
                    float FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();

                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.Buffer = true;
                    //HttpContext.Current.Response.ContentType = "application/ms-excel";
                    HttpContext.Current.Response.ContentType = "application/vnd.xls";
                    HttpContext.Current.Response.AddHeader("Content-Length", getContent.Length.ToString());
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + name + ".xls");
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
                    HttpContext.Current.Response.BinaryWrite(getContent);
                }
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = "No data to be exported.";
                }
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The following error occured while exporting data to excel: " + ex.Message.ToString();
            }

        }

        public void getTransText(string key)
        {
            objIDefault.TransString = util.getTransText(key);
        }

        public void SaveRatings(List<string[]> al)
        {
            util.SaveRatings(al);
        }

        public void SaveAuditData()
        {
            util.SaveAuditData();
        }

        public DataTable ChangeUnits(DataTable dt, string Units, string WUnits)
        {
            Units objUnits = new Units();
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = objUnits.GetChangedUnits(dt.Rows[i], Units, WUnits);
                    if (dt.Columns.Contains("name"))
                        dt.Rows[i]["name"] = dr["name"].ToString();
                    if (dt.Columns.Contains("trnsTag"))
                        dt.Rows[i]["trnsTag"] = dr["trnsTag"].ToString();
                    if (dt.Columns.Contains("colorPaletteName"))
                        dt.Rows[i]["colorPaletteName"] = dr["colorPaletteName"].ToString();


                }
            }
            return dt;
        }
        public void ReadQueryStringValues()
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
            {

                objServiceInfo = new ServiceInfo();
            }
            else
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            }

            objLocInfo = LocationInfo.getLocationInfoObj;

            objDataPointInfo = DataPointInfo.getDataPointObject;
            if (HttpContext.Current.Request.QueryString["module"] != null)
            {
                //if (objSvcInfo.Module != HttpContext.Current.Request.QueryString["module"])
                //{
                //    if (HttpContext.Current.Session["Rating"] != null)
                //    {
                //        SaveRatings();

                //    }
                //}
                objServiceInfo.Module = HttpContext.Current.Request.QueryString["module"];
            }


            if (HttpContext.Current.Request.QueryString["Culture"] != null)
            {
                objServiceInfo.Culture = HttpContext.Current.Request.QueryString["Culture"];
            }
            if (HttpContext.Current.Request.QueryString["Country"] != null)
            {
                objServiceInfo.Country = HttpContext.Current.Request.QueryString["Country"];
                objLocInfo.CountryCode = HttpContext.Current.Request.QueryString["Country"];
            }
            if (HttpContext.Current.Request.QueryString["Unit"] != null)
            {
                objServiceInfo.ServiceName = HttpContext.Current.Request.QueryString["Unit"];
            }
            if (HttpContext.Current.Request.QueryString["Placename"] != null)
            {
                objLocInfo.searchLocation = HttpContext.Current.Request.QueryString["Placename"];
                //Clearing off datapoint object in case search crtieria is given in query string
                objDataPointInfo = null;
                objLocInfo.DataPointInfo = null;
                //HttpContext.Current.Session["objDataPointInfo"] = null;

            }

            if (HttpContext.Current.Request.QueryString["Latitude"] != null && HttpContext.Current.Request.QueryString["Longitude"] != null)
            {
                objLocInfo.latitude = Convert.ToDouble(HttpContext.Current.Request.QueryString["Latitude"]);
                objLocInfo.longitude = Convert.ToDouble(HttpContext.Current.Request.QueryString["Longitude"]);

                if (HttpContext.Current.Request.QueryString["Placename"] == null)
                    objLocInfo.placeName = "Lat: " + objLocInfo.latitude + " Long: " + objLocInfo.longitude;

                //Clearing off datapoint object in case search crtieria is given in query string
                objDataPointInfo = null;
                objLocInfo.DataPointInfo = null;
                // HttpContext.Current.Session["objDataPointInfo"] = null;
            }

        }

        public void DisplayLegend(string node, string name)
        {

            if (node == "legend")
            {
                objIDefault.dtLegenddetails = ServiceObj.GetLegendData(name);

            }

        }
        public DataTable DisplayLegends(string node, string name)
        {

            if (node == "legend")
            {
                return ServiceObj.GetLegendData(name);

            }
            return null;

        }
        public void SaveServiceAuditData(IDictionary dict)
        {
            util.SaveServiceAuditData(dict);
        }

        /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - Begin */
        public string GetServicePageTransTag(string module)
        {
            return ServiceObj.getModuleTransTag(module);
        }
        /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - End */
    }
}