using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Web.Caching;
using System.Data;
using System.Web.SessionState;
using System.Web.UI;
using System.Collections;
using System.ComponentModel;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.DataAccess;
using System.Xml.Linq;
using System.Configuration;
using System.Text.RegularExpressions;
using Syngenta.AgriWeb.LocationSearch.DataObjects;
using Syngenta.AgriWeb.NearByStation.DataObjects;

namespace Syngenta.AgriCast.Common.Service
{
    public class ServiceHandler
    {
        bool isValid = true;
        string errormsg;
        //HttpSessionState Session = HttpContext.Current.Session;
        ServiceInfo objServiceInfo;
        private service svc = new service();
        CommonUtil util = new CommonUtil();
        TranslateData trans = new TranslateData();
        string FullPubpath;
        string FullSchemaPath;
        //Variables used for web service
        string strServiceName;
        string strModuleName;
        StreamReader str;

        private service ReadConfig()
        {
            FullPubpath = getPubPath();
            FullSchemaPath = getSchemaPath();

            if (ValidateServiceConfig(FullPubpath, FullSchemaPath))
            {
                svc = serialize<service>(FullPubpath);
                str.Dispose();
            }
            return svc;
        }

        string getPubPath()
        {
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
            // if (HttpContext.Current != null)
            if (HttpContext.Current != null && HttpContext.Current.Session["serviceInfo"] != null)
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                //web service issue 
                if (string.IsNullOrEmpty(objServiceInfo.Module))
                    objServiceInfo.Module = strModuleName;
            }
            else
            {
                objServiceInfo = new ServiceInfo();
                objServiceInfo.ServiceName = strServiceName;
                // changed start 
                objServiceInfo.Module = strModuleName;
                HttpContext.Current.Session["serviceInfo"] = objServiceInfo;
                // changed end 
            }

            //objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            string path = util.getApplPath(@"Pub\") + objServiceInfo.ServiceName + @"\service_config.xml";
            ////check if the pub path exists or not
            string strPubPath = path.Substring(0, path.LastIndexOf("\\"));

            // if any invalid pub is entered in the url, then rediect the user to default pub page
            if (!Directory.Exists(strPubPath))
            {
                string defPub = ConfigurationManager.AppSettings["defaultPub"].ToString();
                objServiceInfo.ServiceName = defPub;
                string strPath = HttpContext.Current.Request.Path;
                //remove default.aspx and pub name.

                strPath = strPath.Substring(0, strPath.LastIndexOf("/"));
                strPath = strPath.Substring(0, strPath.LastIndexOf("/"));
                strPath = strPath + "\\" + defPub;

                HttpContext.Current.Response.Redirect(strPath, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            return path;
        }
        string getSchemaPath()
        {
            //objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            string path = util.getApplPath(@"Pub\") + "service.xsd";
            return path;
        }

        /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
        private string MatchReplace(Match m)
        {
            // if match successful
            if (m.Success)
            {
                // get the folder of current publication
                var pubFolder = Path.GetDirectoryName(FullPubpath);
                var path = m.Groups["path"];

                if (string.IsNullOrEmpty(path.Value))
                    return string.Empty;

                // load child xml file
                var file = Path.Combine(pubFolder, path.Value);
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNode rootNode = doc.DocumentElement;
                return rootNode.InnerXml;
            }

            return string.Empty;
        }

        /* UAT Tracker 520	advertisemeent - where to pull the data - Begin */
        const string pubNameKey = "{pubname_placeholder}";
        /* UAT Tracker 520	advertisemeent - where to pull the data - End */

        private MemoryStream ReadXmlToMemoryStream(string url)
        {
            using (var streamReader = new System.IO.StreamReader(url, Encoding.UTF8))
            {
                string content = streamReader.ReadToEnd();

                // replace tag in xml with file
                // the tag should be <servicePages file="xxx" />
                content = Regex.Replace(content,
                    "<servicePages\\s+file=\\\"(?<path>.+)\\\"\\s*/>",
                    MatchReplace,
                    RegexOptions.IgnoreCase);

                /* UAT Tracker 520	advertisemeent - where to pull the data - Begin */
                string serviceName = objServiceInfo.ServiceName;
                if (string.IsNullOrEmpty(serviceName))
                    serviceName = ConfigurationManager.AppSettings["defaultPub"];

                content = content.Replace(pubNameKey, serviceName);
                /* UAT Tracker 520	advertisemeent - where to pull the data - End */

                byte[] arrContent = Encoding.UTF8.GetBytes(content);
                return new MemoryStream(arrContent);
            }
        }

        /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

        public T serialize<T>(string FullPubpath)
        {
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
            //str = new System.IO.StreamReader(FullPubpath);
            str = new StreamReader(ReadXmlToMemoryStream(FullPubpath));
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

            XmlSerializer xSerializer = new XmlSerializer(typeof(T));
            return (T)xSerializer.Deserialize(str);
        }

        public void createServiceSession()
        {
            svc = this.ReadConfig();
            HttpContext.Current.Session["service"] = svc;
        }

        internal bool ValidateServiceConfig(string Pub, string Schema)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            settings.Schemas = schemaSet;
            schemaSet.Add(null, Schema);
            schemaSet.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);

            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
            var stream = ReadXmlToMemoryStream(Pub);
            XmlReader validator = XmlReader.Create(stream, settings);
            //XmlReader validator = XmlReader.Create(Pub, settings);
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

            int line = 0;
            try
            {
                while (validator.Read())
                {
                    line = line + 1;
                }
            }
            catch (Exception err)
            {
                HttpContext.Current.Session["ErrorMessage"] = "The following validation error occured at line num " + line + " : ";
                isValid = false;
                throw err;
            }
            finally
            {
                validator.Close();
            }

            return isValid;
        }

        internal void MyValidationEventHandler(object sender, ValidationEventArgs args)
        {
            isValid = false;
            errormsg = args.Message.ToString();
        }

        //Method to fetch all the allowed culture codes / Languages to populated the dropdown
        public DataTable loadCultureCode()
        {

            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            //svc = this.ReadConfig(strPubName);
            if (svc != null)
            {
                serviceCultureSettings culture = new serviceCultureSettings();
                culture = svc.cultureSettings;
                DataTable dtCultureCode = util.ConvertToDataTable(culture.allowedCultureCodes.ToList());
                return dtCultureCode;
            }
            return null;
        }

        //Method to check for default and browser language
        public string getCultureCode()
        {

            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            //svc = this.ReadConfig(strPubName);
            if (svc != null)
            {
                serviceCultureSettings culture = new serviceCultureSettings();
                culture = svc.cultureSettings;
                string strCultureCode = "";
                strCultureCode = culture.@default;

                //Condition to check the default language
                if (strCultureCode == null || strCultureCode == "")
                {
                    //bool browserLang = culture.useBrowserLang;
                    //Condition to check the browser language
                    if (culture.useBrowserLang)
                    {
                        string[] languages = HttpContext.Current.Request.UserLanguages;
                        if (languages == null || languages.Length == 0)
                            return null;
                        try
                        {
                            strCultureCode = languages[0].ToLowerInvariant().Trim();
                        }

                        catch (ArgumentException)
                        {
                            return null;
                        }
                    }

                    else
                    {
                        strCultureCode = culture.allowedCultureCodes[0].code;
                    }

                }
                return strCultureCode;
            }
            return null;
        }

        //Method to fetch the units specified for the user to populated the units dropdown
        public DataTable loadUnits()
        {

            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                serviceUnitsSettings units = new serviceUnitsSettings();
                units = svc.unitsSettings;
                ArrayList al = new ArrayList();
                foreach (string unit in units.Units.ToString().Split(','))
                {
                    al.Add(unit);
                }
                // al.Add(units.custom.ToString());
                DataTable dtUnits = new DataTable();
                dtUnits.Columns.Add("units");
                DataRow dr;
                for (int i = 0; i < al.Count; i++)
                {
                    dr = dtUnits.NewRow();
                    dr[0] = al[i].ToString();
                    dtUnits.Rows.Add(dr);
                }
                return dtUnits;
            }
            return null;
        }
        public string LoadCustomSettings()
        {

            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            string strCustom = string.Empty;
            if (svc != null)
            {
                serviceUnitsSettings units = new serviceUnitsSettings();
                units = svc.unitsSettings;
                strCustom = "Wind-" + units.custom.Wind;
                //strCustom = "#Rain-" + units.custom.Rain;

            }
            return strCustom;
        }

        public NearByPointSettings getNearbyDataPoint()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            NearByPointSettings arNearByPointDetails = null;
            if (svc != null)
            {
                //svc = this.ReadConfig(strPubName);
                var nearByPoint = from p in svc.servicePage.ToList()
                                  where p.name.ToLower() == objServiceInfo.Module.ToLower()
                                  select p.nearbyDataPoint;

                if (nearByPoint.ToList().Count != 0)
                {
                    arNearByPointDetails = new NearByPointSettings();
                    arNearByPointDetails.MaxAllowedDistance = nearByPoint.ToList()[0].maxAllowedDistance;
                    arNearByPointDetails.MaxAllowedAltitude= nearByPoint.ToList()[0].maxAllowedAltitude;
                    arNearByPointDetails.ResultCount = nearByPoint.ToList()[0].resultCount;
                    arNearByPointDetails.DataSource = nearByPoint.ToList()[0].defaultDataSource;
                }
            }
            return arNearByPointDetails;

        }
        //Method to fetch the allowed countries for the user to populate the country dropdown
        public DataTable loadCountries()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                if (svc.locationSearch != null)
                {
                    return util.ConvertToDataTable(svc.locationSearch.allowedCountries.ToList());
                }
            }
            return null;
        }

        //Method to get the provider name
        public LocationSearchSource getProviderName()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            if (svc != null)
            {
                if (svc.locationSearch != null)
                {
                    return svc.locationSearch.provider;
                }
            }
            return LocationSearchSource.defaultService;
        }

        public ArrayList getDefaultEmailSetttings()
        {
            getServiceObj();
            ArrayList al = new ArrayList();

            if (svc != null)
            {
                al.Add(svc.emailSettings.senderName);
                al.Add(svc.emailSettings.senderAddress);
                al.Add(svc.emailSettings.subject);
                if (svc.emailSettings.defaultBody.ToString() != "System.Object")
                    al.Add((((System.Xml.XmlNode[])(svc.emailSettings.defaultBody))[0]).OuterXml);
                else
                    al.Add("");
                return al;
            }
            return null;
        }

        public DataTable loadMenuTabs()
        {
            List<string[]> arPages = getNodeList("service");
            int num = arPages.Count;
            DataTable dtPages = new DataTable();
            dtPages.Columns.Add("Name");
            dtPages.Columns.Add("Text");
            DataRow dr;
            for (int i = 0; i < num; i++)
            {
                dr = dtPages.NewRow();
                dr[0] = arPages[i][1].ToString();
                dr[1] = trans.getTranslatedText(arPages[i][2].ToString(), objServiceInfo.Culture);
                dtPages.Rows.Add(dr);
            }
            return dtPages;

        }

        //Method to get the name of the default page
        public string getDefaultPage()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                var page = from p in svc.servicePage.ToList() where p.defaultPage == true select p.name;
                return page.ToList()[0].ToString();
            }
            return null;
        }
    
        public bool checkModule(string modulename)
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                /* IM01363503 - {Aggregation should be smaller than duration.} error in Bodenwasser Modell - Jerrey - Begin */
                var page = from p in svc.servicePage.ToList() select p.name.ToLower();  // var page = from p in svc.servicePage.ToList() select p.name;
                if (page.ToList().Contains(modulename.ToLower()))   // if (page.ToList().Contains(modulename))
                    return true;
                else
                    return false;
                /* IM01363503 - {Aggregation should be smaller than duration.} error in Bodenwasser Modell - Jerrey - End */
            }
            return false;
        }
        //Method to read the Series data for Icon generation
        public DataTable GetIconSeries(string controlName)
        {
            DataTable dtIcon = new DataTable();

            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }

            if (svc != null)
            {
                var page = from p in svc.servicePage.ToList()
                           where p.name == objServiceInfo.Module
                           select p.icon.SingleOrDefault(x => x.name == controlName);

                if (page != null)
                {
                    dtIcon = util.ConvertToDataTable(page.ToList()[0].series.ToList());
                }

            }
            return dtIcon;
        }

        public DataTable getIconSettings(string controlName)
        {
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

                getServiceObj();

                /* SD01241630 - New Agricast webservices - can't get two views(iconsdailyandTabledata) - BEGIN -*/
                //When modules in different servicepages are called in web service, update the serviceinfo object with 
                // current module passed from the web service presenter 
                if (!string.IsNullOrEmpty(strModuleName) && !objServiceInfo.Module.ToLower().Equals(strModuleName.ToLower()))
                    objServiceInfo.Module = strModuleName;
                /* SD01241630 - New Agricast webservices - can't get two views(vew_dailyandTabledata) - END -*/
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.icon.SingleOrDefault(x => x.name == controlName);

                DataTable dt = util.ConvertToDataTable(page.ToList());
                return dt;
            }
            return null;
        }


        public DataTable GetChartSeries(string chartName)
        {
            DataTable dtChart = new DataTable();
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }

            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.chart.SingleOrDefault(x => x.name == chartName).series;

                if (page.ToList().Count > 0)
                {
                    dtChart = util.ConvertToDataTable(page.ToList()[0]);
                }

            }

            dtChart.TableName = "ChartSerie";

            return dtChart;
        }

        public DataTable getChartSettings(string chartName)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session["serviceInfo"] != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }
            DataTable dt;

            if (svc != null)
            {
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                //if (objServiceInfo.Module.ToLower().Contains("history"))
                if (objServiceInfo.Module.ToLower().Contains("history") || objServiceInfo.Module.ToLower().Contains("watermodel"))
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/  
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.agriInfo.chart;
                    dt = util.ConvertToDataTable(page.ToList());

                }
                else
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.chart.SingleOrDefault(x => x.name == chartName);
                    dt = util.ConvertToDataTable(page.ToList());

                }

                dt.TableName = "ChartSettings";

                return dt;
            }
            return null;
        }

        public DataTable GetAxes(string chartName, string type)
        {
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }

            if (svc != null)
            {
                DataTable dt = new DataTable();
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/  
                //if (objServiceInfo.Module.ToLower().Contains("history"))
                if (objServiceInfo.Module.ToLower().Contains("history") || objServiceInfo.Module.ToLower().Contains("watermodel"))
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/  
                {
                    if (type == "P")
                    {
                        var Axis = from p in svc.servicePage
                                   where p.name.ToLower() == objServiceInfo.Module.ToLower()
                                   select p.agriInfo.chart.PrimaryAxis;
                        dt = util.ConvertToDataTable(Axis.ToList());

                        dt.TableName = "PrimaryAxis";
                    }
                    else
                    {
                        var Axis = from p in svc.servicePage
                                   where p.name.ToLower() == objServiceInfo.Module.ToLower()
                                   select p.agriInfo.chart.SecondaryAxis;
                        dt = util.ConvertToDataTable(Axis.ToList());

                        dt.TableName = "SecondaryAxis";
                    }

                }
                else
                {
                    if (type == "P")
                    {
                        var Axis = from p in svc.servicePage
                                   where p.name.ToLower() == objServiceInfo.Module.ToLower()
                                   select p.chart.SingleOrDefault(x => x.name == chartName).PrimaryAxis;
                        dt = util.ConvertToDataTable(Axis.ToList());

                        dt.TableName = "PrimaryAxis";
                    }
                    else
                    {
                        var Axis = from p in svc.servicePage
                                   where p.name.ToLower() == objServiceInfo.Module.ToLower()
                                   select p.chart.SingleOrDefault(x => x.name == chartName).SecondaryAxis;
                        dt = util.ConvertToDataTable(Axis.ToList());

                        dt.TableName = "SecondaryAxis";
                    }
                }

                return dt;
            }
            return null;
        }

        //public string GetAgriInfoDatasource()
        //{
        //     objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
        //    getServiceObj();
        //    if (svc != null)
        //    {
        //        var page = from p in svc.servicePage
        //                   where p.name.ToLower() == objServiceInfo.Module.ToLower()
        //                   select p.agriInfo;

        //      return  page.ToList()[0].weather.datasource.ToString();
        //    }
        //}
        public DataSet loadSeries()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

            DataSet dsSeries = new DataSet();

            getServiceObj();
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.agriInfo.weather.node;


                for (int i = 0; i < page.ToList()[0].Count(); i++)
                {
                    DataTable dt = new DataTable();
                    dt = util.ConvertToDataTable(page.ToList()[0][i].series);
                    dt.TableName = page.ToList()[0][i].trnsTag;
                    dsSeries.Tables.Add(dt);
                }


            }

            return dsSeries;

        }

        public string readDate()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

            string date = string.Empty;

            getServiceObj();
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.agriInfo.defaultStartDate;
                date = page.ToList()[0].ToString();
            }

            return date;

        }

        public string getLegendAttributes(string strLegendName)
        {
            //Web Service Issue fix
            if (HttpContext.Current != null)
            {
                //DataTable dtLegend = new DataTable();
                //dtLegend.Columns.Add("name");
                //dtLegend.Columns.Add("htmlPath");
                //dtLegend.Columns.Add("trnsTag");

                FullPubpath = getPubPath();
                /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */

                var reader = ReadXmlToMemoryStream(FullPubpath);

                // commented for CR1
                //XmlReader reader = new XmlTextReader(FullPubpath);
                //XmlDocument xdoc = new XmlDocument();
                //xdoc.Load(FullPubpath);

                /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

                XDocument doc = XDocument.Load(reader);

                for (int j = 0; j < doc.Descendants().ToList().Count; j++)
                {
                    if (doc.Descendants().ToList()[j].FirstAttribute != null && doc.Descendants().ToList()[j].FirstAttribute.Value == strLegendName)
                    {
                        //dr = dtLegend.NewRow();
                        //dr["Node"] = doc.Descendants().ToList()[j].Name.LocalName;
                        //dr["NodeName"] = doc.Descendants().ToList()[j].FirstAttribute.Value;
                        //dr["ModuleNmae"] = doc.Descendants().ToList()[j].Parent.FirstAttribute.Value;
                        //dtLegend.Rows.Add(dr);                  
                        try
                        {
                            return doc.Descendants().ToList()[j].Attribute("htmlPath").Value;
                        }
                        catch (Exception) { }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
            return null;
        }

        //Method to read the Series data for Tables
        public DataSet GetTableSeries(string allign, string controlName)
        {
            DataSet ds = new DataSet();
            DataTable dtSeries = new DataTable();
            DataTable dtLegend = new DataTable();
            dtSeries.Columns.Add("name");
            dtSeries.Columns.Add("colorPaletteName");
            dtSeries.Columns.Add("aggregationFunction");
            dtSeries.Columns.Add("trnsTag");
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
            dtSeries.Columns.Add("isInvisible");
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
            dtLegend.Columns.Add("name");
            dtLegend.Columns.Add("htmlPath");
            dtLegend.Columns.Add("trnsTag");

            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
                //objServiceInfo.Module = "Tables";
            }

            if (svc != null)
            {
                if (allign.ToLower() == "tblDaysRows".ToLower())
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.tblDaysRows.SingleOrDefault(x => x.name == controlName);

                    ArrayList al = new ArrayList();
                    al.Add(page.ToList()[0].series.Items);
                    for (int i = 0; i < page.ToList()[0].series.Items.Count(); i++)
                    {
                        if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblDaysRowsSeriesSerie")
                        {
                            DataRow dr = dtSeries.NewRow();
                            dr[0] = ((serviceServicePageTblDaysRowsSeriesSerie)(page.ToList()[0].series.Items[i])).name;
                            dr[1] = ((serviceServicePageTblDaysRowsSeriesSerie)(page.ToList()[0].series.Items[i])).colorPaletteName;
                            dr[2] = ((serviceServicePageTblDaysRowsSeriesSerie)(page.ToList()[0].series.Items[i])).aggregationFunction;
                            dr[3] = ((serviceServicePageTblDaysRowsSeriesSerie)(page.ToList()[0].series.Items[i])).trnsTag;
                            dtSeries.Rows.Add(dr);

                        }


                        else if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblDaysRowsSeriesLegend")
                        {
                            DataRow dr = dtLegend.NewRow();
                            dr[0] = ((serviceServicePageTblDaysRowsSeriesLegend)(page.ToList()[0].series.Items[i])).name;
                            dr[1] = ((serviceServicePageTblDaysRowsSeriesLegend)(page.ToList()[0].series.Items[i])).htmlPath;
                            dr[2] = ((serviceServicePageTblDaysRowsSeriesLegend)(page.ToList()[0].series.Items[i])).trnsTag;
                            dtLegend.Rows.Add(dr);
                        }

                    }
                }
                else
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.tblSeriesRows.SingleOrDefault(x => x.name == controlName);

                    ArrayList al = new ArrayList();
                    al.Add(page.ToList()[0]);

                    for (int i = 0; i < page.ToList()[0].series.Items.Count(); i++)
                    {
                        if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblSeriesRowsSeriesSerie")
                        {
                            DataRow dr = dtSeries.NewRow();
                            dr[0] = ((serviceServicePageTblSeriesRowsSeriesSerie)(page.ToList()[0].series.Items[i])).name;
                            dr[1] = ((serviceServicePageTblSeriesRowsSeriesSerie)(page.ToList()[0].series.Items[i])).colorPaletteName;
                            dr[2] = ((serviceServicePageTblSeriesRowsSeriesSerie)(page.ToList()[0].series.Items[i])).aggregationFunction;
                            dr[3] = ((serviceServicePageTblSeriesRowsSeriesSerie)(page.ToList()[0].series.Items[i])).trnsTag;
                            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
                            dr[4] = ((serviceServicePageTblSeriesRowsSeriesSerie)(page.ToList()[0].series.Items[i])).isInvisible;
                            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
                            dtSeries.Rows.Add(dr);

                        }
                        else if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblSeriesRowsSeriesLegend")
                        {
                            DataRow dr = dtLegend.NewRow();
                            dr[0] = ((serviceServicePageTblSeriesRowsSeriesLegend)(page.ToList()[0].series.Items[i])).name;
                            dr[1] = ((serviceServicePageTblSeriesRowsSeriesLegend)(page.ToList()[0].series.Items[i])).htmlPath;
                            dr[2] = ((serviceServicePageTblSeriesRowsSeriesLegend)(page.ToList()[0].series.Items[i])).trnsTag;
                            dtLegend.Rows.Add(dr);
                        }
                    }
                }


                ds.Tables.Add(dtSeries);
                ds.Tables.Add(dtLegend);
            }
            return ds;
        }

        public List<string[]> GetAdvancedOptions(string series, string serie)
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            List<string[]> al = new List<string[]>();
            if (svc != null)
            {
                if (series.StartsWith("gdd"))
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.agriInfo.gdd.series;

                    for (int i = 0; i < page.ToList()[0].Count(); i++)
                    {
                        if (serie.Trim().ToLower() == page.ToList()[0][i].name.ToLower())
                        {
                            al.Add(new string[] { "allowedAggregationFunction", "" });
                            al.Add(new string[] { "allowAccumalation", "" });
                            al.Add(new string[] { "minComparisonYear", "" });
                            al.Add(new string[] { "trend", "" });
                            al.Add(new string[] { "altitudeAdjustment", "" });
                            al.Add(new string[] { "gddMethod", (page.ToList()[0][i].advancedOption.method == null) ? "" : page.ToList()[0][i].advancedOption.method.ToString() });
                            al.Add(new string[] { "gddCap", (page.ToList()[0][i].advancedOption.cap == null) ? "" : page.ToList()[0][i].advancedOption.cap.ToString() });
                            al.Add(new string[] { "gddBase", (page.ToList()[0][i].advancedOption.@base == null) ? "" : page.ToList()[0][i].advancedOption.@base.ToString() });


                        }
                    }
                }
                else
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.agriInfo.weather.node;

                    for (int i = 0; i < page.ToList()[0].Count(); i++)
                    {
                        if (series.Trim().ToLower() == page.ToList()[0][i].id.ToLower())
                        {
                            for (int j = 0; j < page.ToList()[0][i].series.Count(); j++)
                            {
                                if (serie.Trim().ToLower() == page.ToList()[0][i].series[j].name.ToLower())
                                {
                                    al.Add(new string[] { "allowedAggregationFunction", (page.ToList()[0][i].series[j].advancedOption.allowedAggregationFunction == null) ? "" : page.ToList()[0][i].series[j].advancedOption.allowedAggregationFunction.ToString() });
                                    al.Add(new string[] { "allowAccumalation", (page.ToList()[0][i].series[j].advancedOption.allowAccumalation == null) ? "" : page.ToList()[0][i].series[j].advancedOption.allowAccumalation.ToString() });
                                    al.Add(new string[] { "minComparisonYear", (page.ToList()[0][i].series[j].advancedOption.minComparisonYear == 0) ? "" : page.ToList()[0][i].series[j].advancedOption.minComparisonYear.ToString() });
                                    al.Add(new string[] { "trend", (page.ToList()[0][i].series[j].advancedOption.trend == null) ? "" : page.ToList()[0][i].series[j].advancedOption.trend.ToString() });
                                    al.Add(new string[] { "altitudeAdjustment", (page.ToList()[0][i].series[j].advancedOption.altitudeAdjustment == null) ? "" : page.ToList()[0][i].series[j].advancedOption.altitudeAdjustment.ToString() });
                                }
                            }
                        }

                    }
                }

            }
            return al;

        }

        public string ReadAllignment(string allign, string controlName)
        {
            //objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            string strAllignDetails = "tblDaysRows";
            int NumOfDays = 0;
            //getServiceObj();
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

                getServiceObj();

                /* SD01241630 - New Agricast webservices - can't get two views(vew_dailyandTabledata) - BEGIN -*/
                //When modules in different servicepages are called in web service, update the serviceinfo object with 
                // current module passed from the web service presenter 
                if (!string.IsNullOrEmpty(strModuleName) && !objServiceInfo.Module.ToLower().Equals(strModuleName.ToLower()))
                    objServiceInfo.Module = strModuleName;
                /* SD01241630 - New Agricast webservices - can't get two views(vew_dailyandTabledata) - END -*/

            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
                //objServiceInfo.Module = "Tables";
            }


            if (svc != null)
            {
                DataTable dt;
                if (allign.ToLower() == "tbldaysrows")
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.tblDaysRows.SingleOrDefault(x => x.name == controlName);

                    dt = util.ConvertToDataTable(page.ToList());

                }

                else
                {
                    var page = from p in svc.servicePage
                               where p.name.ToLower() == objServiceInfo.Module.ToLower()
                               select p.tblSeriesRows.SingleOrDefault(x => x.name == controlName);

                    dt = util.ConvertToDataTable(page.ToList());
                    strAllignDetails = "tblSeriesRows";
                }
                if ((dt.Rows[0]["endDate"].ToString() != "") && (dt.Rows[0]["startDate"].ToString() != ""))
                {
                    if (Convert.ToInt32(dt.Rows[0]["startDate"]) >= Convert.ToInt32(dt.Rows[0]["endDate"]))
                        NumOfDays = Convert.ToInt32(dt.Rows[0]["startDate"].ToString()) - Convert.ToInt32(dt.Rows[0]["endDate"].ToString());
                    else
                        NumOfDays = Convert.ToInt32(dt.Rows[0]["endDate"].ToString()) - Convert.ToInt32(dt.Rows[0]["startDate"].ToString());
                }

                //please dont change sequence
                /*IM01277709 - change in spray window. begin*/
                // add the "displayAMPM attribute value to the allignment string. append it at the end.. 
               
                //strAllignDetails = strAllignDetails + "," + NumOfDays + "," + dt.Rows[0]["dataSource"].ToString() + "," + Convert.ToInt32(dt.Rows[0]["startDate"].ToString()) + "," + Convert.ToInt32(dt.Rows[0]["endDate"].ToString()) + "," + dt.Rows[0]["temporalAggregation"].ToString() + "," + dt.Rows[0]["ruleset"].ToString() + "," + dt.Rows[0]["colorPaletteName"].ToString() + "," + dt.Rows[0]["trnsTag"].ToString() + "," + dt.Rows[0]["feedback"].ToString() + "," + dt.Rows[0]["collapsible"].ToString();
                if (dt.Columns.Contains("displayAMPM"))
                {
                    //append displayAMPM only for tblDaysRows
                    strAllignDetails = strAllignDetails + "," + NumOfDays + "," + dt.Rows[0]["dataSource"].ToString() + "," + Convert.ToInt32(dt.Rows[0]["startDate"].ToString()) + "," + Convert.ToInt32(dt.Rows[0]["endDate"].ToString()) + "," + dt.Rows[0]["temporalAggregation"].ToString() + "," + dt.Rows[0]["ruleset"].ToString() + "," + dt.Rows[0]["colorPaletteName"].ToString() + "," + dt.Rows[0]["trnsTag"].ToString() + "," + dt.Rows[0]["feedback"].ToString() + "," + dt.Rows[0]["collapsible"].ToString() + "," + dt.Rows[0]["displayAMPM"].ToString();
                }
                else
                {
                   //tblseries
                    strAllignDetails = strAllignDetails + "," + NumOfDays + "," + dt.Rows[0]["dataSource"].ToString() + "," + Convert.ToInt32(dt.Rows[0]["startDate"].ToString()) + "," + Convert.ToInt32(dt.Rows[0]["endDate"].ToString()) + "," + dt.Rows[0]["temporalAggregation"].ToString() + "," + dt.Rows[0]["ruleset"].ToString() + "," + dt.Rows[0]["colorPaletteName"].ToString() + "," + dt.Rows[0]["trnsTag"].ToString() + "," + dt.Rows[0]["feedback"].ToString() + "," + dt.Rows[0]["collapsible"].ToString() ;
                }
                /*IM01277709 - change in spray window. END*/
                return strAllignDetails;
            }
            return null;
        }

        //public ArrayList GetcompleteSeriesList(string allign)
        //{
        //    ArrayList al = new ArrayList();
        //    objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
        //    getServiceObj();

        //    if (svc != null)
        //    {
        //        if (allign.ToLower() == "tblDaysRows".ToLower())
        //        {
        //            var page = from p in svc.servicePage
        //                       where p.name.ToLower() == objServiceInfo.Module.ToLower()
        //                       select p.tblDaysRows.SingleOrDefault(x => x.name == "Tables");

        //            for (int i = 0; i < page.ToList()[0].series.Items.Count(); i++)
        //            {

        //                if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblDaysRowsSeriesSerie")
        //                {
        //                    al.Add(((serviceServicePageTblDaysRowsSeriesSerie)(page.ToList()[0].series.Items[i])).name);

        //                }


        //                else if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblDaysRowsSeriesLegend")
        //                {
        //                    al.Add(((serviceServicePageTblDaysRowsSeriesLegend)(page.ToList()[0].series.Items[i])).name);
        //                }

        //            }
        //        }
        //        else
        //        {
        //            var page = from p in svc.servicePage
        //                       where p.name.ToLower() == objServiceInfo.Module.ToLower()
        //                       select p.tblSeriesRows.SingleOrDefault(x => x.name == "Tables");

        //            for (int i = 0; i < page.ToList()[0].series.Items.Count(); i++)
        //            {

        //                if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblSeriesRowsSeriesSerie")
        //                {
        //                    al.Add(((serviceServicePageTblSeriesRowsSeriesSerie)(page.ToList()[0].series.Items[i])).name);

        //                }


        //                else if (page.ToList()[0].series.Items[i].ToString() == "serviceServicePageTblSeriesRowsSeriesLegend")
        //                {
        //                    al.Add(((serviceServicePageTblSeriesRowsSeriesLegend)(page.ToList()[0].series.Items[i])).name);
        //                }

        //            }
        //        }

        //    }

        //    return al;
        //} s


        public DataTable getWebServiceNodeList(string strModuleIDs)
        {
            FullPubpath = getPubPath();
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */

            var reader = ReadXmlToMemoryStream(FullPubpath);

            // commented for CR1
            //XmlReader reader = new XmlTextReader(FullPubpath);
            //XmlDocument xdoc = new XmlDocument();
            //xdoc.Load(FullPubpath);

            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

            XDocument doc = XDocument.Load(reader);
            DataTable dtNodeData = new DataTable();
            dtNodeData.Columns.Add("Node");
            dtNodeData.Columns.Add("NodeName");
            dtNodeData.Columns.Add("ModuleName");
            DataRow dr;

            for (int i = 0; i <= (strModuleIDs.Split(';').Length - 1); i++)
            {
                for (int j = 0; j < doc.Descendants().ToList().Count; j++)
                {
                    if (doc.Descendants().ToList()[j].FirstAttribute != null && doc.Descendants().ToList()[j].FirstAttribute.Value.ToLower() == strModuleIDs.Split(';')[i].ToLower())
                    {
                        dr = dtNodeData.NewRow();
                        dr["Node"] = doc.Descendants().ToList()[j].Name.LocalName;
                        dr["NodeName"] = doc.Descendants().ToList()[j].FirstAttribute.Value;
                        if (doc.Descendants().ToList()[j].Name.LocalName == "legend")
                        {
                            dr["ModuleName"] = "";
                        }
                        else
                        {
                            dr["ModuleName"] = doc.Descendants().ToList()[j].Parent.FirstAttribute.Value;
                        }
                        dtNodeData.Rows.Add(dr);
                        break;
                    }
                }
            }
            reader.Close();
            return dtNodeData;
        }

        public List<string[]> getNodeList(string nodename)
        {
            FullPubpath = getPubPath();
            //objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            }
            else
            {
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
                //objServiceInfo.Module = "Tables";
            }

            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
            // commented for CR1
            //XmlReader reader = new XmlTextReader(FullPubpath);
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

            XmlDocument xdoc = new XmlDocument();
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
            xdoc.Load(ReadXmlToMemoryStream(FullPubpath));
            // commented for CR1
            //xdoc.Load(FullPubpath);
            XDocument doc = XDocument.Load(ReadXmlToMemoryStream(FullPubpath));
            // commented for CR1
            //XDocument doc = XDocument.Load(reader);
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */

            List<string[]> al = new List<string[]>();
            if (xdoc.GetElementsByTagName(nodename).Count != 0)
            {
                switch (util.getlowercase(nodename))
                {
                    case "service":
                        var nn = from p in doc.Descendants()
                                 where util.getlowercase(p.Name) == util.getlowercase(nodename)
                                 select p.Elements();
                        foreach (var n in nn.ToList()[0].ToList())
                            //SOCB on 20-Mar-2012 for reading only mobile template
                            //widgetOnly =true in service page indicates that it is used only for moss entry purpose and should not be shown in the actual website.
                            if (util.getlowercase(n.Name) == "servicepage" && !(n.Attribute("widgetOnly") != null && Boolean.Parse(n.Attribute("widgetOnly").Value).Equals(true))
                                && ((objServiceInfo.IsMobile == false && (n.Attribute("template") == null || util.getlowercase(n.Attribute("template").Value) != "mobile")) ||
                                   (objServiceInfo.IsMobile == true && n.Attribute("template") != null && util.getlowercase(n.Attribute("template").Value) == "mobile"))
                                )
                                //EOCB on 20-Mar-2012 for reading only mobile template
                                al.Add(new string[] { n.Name.ToString(), n.Attribute("name").Value, n.Attribute("trnsTag").Value });
                        break;
                    case "servicepage":
                        nn = from p in doc.Descendants()
                             where util.getlowercase(p.Name) == util.getlowercase(nodename) && util.getlowercase(p.Attribute("name").Value) == util.getlowercase(objServiceInfo.Module)
                             select p.Elements();


                        if (nn.ToList().Count == 0)
                        {
                            //nn = from p in doc.Descendants()
                            //     where util.getlowercase(p.Name) == util.getlowercase(nodename) && util.getlowercase(p.Attribute("defaultPage").Value) == "true"
                            //     select p.Elements();

                            string defpage = getDefaultPage();

                            //objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                            //getServiceObj();

                            //if (svc != null)
                            //{
                            //    var page = from p in svc.servicePage.ToList() where p.defaultPage == true select p.name;
                            //    return page.ToList()[0].ToString();
                            //}


                            nn = from p in doc.Descendants()
                                 where util.getlowercase(p.Name) == util.getlowercase(nodename) && util.getlowercase(p.Attribute("name").Value) == util.getlowercase(defpage)
                                 select p.Elements();


                        }


                        foreach (var n in nn.ToList()[0].ToList())
                            al.Add(new string[] { n.Name.ToString(), (string)n.Attributes("name").SingleOrDefault() ?? "" });
                        break;
                    default:
                        nn = from p in doc.Descendants()
                             where util.getlowercase(p.Name) == util.getlowercase(nodename) && (util.getlowercase(p.Parent.Name) == "servicepage" && util.getlowercase(p.Parent.Attribute("name").Value) == util.getlowercase(objServiceInfo.Module))
                             select p.Elements();

                        foreach (var n in nn.ToList()[0].ToList())
                            if (util.getlowercase(n.Name) == "series")
                            {
                                var elements = from p in n.Descendants()
                                               select p;
                                foreach (var s in elements.ToArray())
                                    al.Add(new string[] { s.Name.ToString(), s.Attribute("name").Value });
                            }
                        break;
                }

            }
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - Begin */
            // commented for CR1
            //reader.Close();
            /* Agricast CR - R1 - Concept of global publication / instance configuration. - End */
            return al;
        }

        public void getServiceObj()
        {
            if (HttpContext.Current.Session["service"] != null)
            {
                svc = (service)HttpContext.Current.Session["service"];
            }
            else
            {
                createServiceSession();
                svc = (service)HttpContext.Current.Session["service"];
            }
        }

        /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - BEGIN*/
        //public DataSet GetMossMenuItems(string ExtNavigation)
        public DataSet GetMossMenuItems(string ExtNavigation, string MossCultureCode)
        /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - BEGIN*/
        {

            string fileName = HttpRuntime.AppDomainAppPath + @"\navigation.xml";
            DataSet ds = new DataSet();
            try
            {
                TopNavigation objTopNav = new TopNavigation();
                //Get the xml based on the external navigation setting
                /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
                #region Uncomment this code during deployment
                if (!string.IsNullOrEmpty(MossCultureCode))
                {
                    //check for the "-" in culture code and pass only the language part to web service
                    if (MossCultureCode.IndexOf("-") != 1)
                    {
                       MossCultureCode= MossCultureCode.Split('-')[0];
                    }
                }
                XmlNode node = objTopNav.ExposeNavigationInXML(Constants.MOSSMENU_WEBSERVICE_PARAM_SERVERNAME, Constants.MOSSMENU_WEBSERVICE_PARAM_COUNTRY, ExtNavigation, MossCultureCode);
                string xmloutstring = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + node.OuterXml.ToString();
                #endregion

                #region to be used only while executing local code
                //XmlDocument xDoc = new XmlDocument();
                //xDoc.Load(HttpRuntime.AppDomainAppPath + @"sampleNavigation.xml");
                //string xmloutstring = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + xDoc.OuterXml.ToString();

                #endregion
                //get the string into the string reader object
                StringReader str = new StringReader(xmloutstring);
                //Read the xml into the dataset 
                ds.ReadXml(str);
                if (ds.Tables[0].TableName.ToLower().Contains("error"))
                    ds = null;
            }
            catch
            {
                ds = null;
            }
            return ds;
        }
        public string GetExtNavigation()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                //if (svc.extNavigation.ToString() == "System.Xml.XmlNode[]") 
                //{
                //    return (((System.Xml.XmlNode[])(svc.extNavigation))[0]).Value;
                //}
                return svc.extNavigation.ToString();
            }
            return string.Empty;
        }

        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        public string GetMossReturnUrl()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                return svc.mossReturnUrl;
            }
            return string.Empty;
        }
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/

        public string[] loadPageSettings()
        {

            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            string[] arPageSettings = null;
            if (svc != null)
            {
                var PageSettings = from p in svc.servicePage.ToList()
                                   where p.name == objServiceInfo.Module
                                   select p.pageSettings;

                if (PageSettings.ToList().Count != 0)
                {
                    arPageSettings = new string[5];
                    arPageSettings[0] = PageSettings.ToList()[0].allowFavorites.ToString();
                    arPageSettings[1] = PageSettings.ToList()[0].allowPrintPage.ToString();
                    arPageSettings[2] = PageSettings.ToList()[0].allowEmail.ToString();
                    arPageSettings[3] = PageSettings.ToList()[0].allowExportToExcel.ToString();
                    arPageSettings[4] = PageSettings.ToList()[0].allowFeedback.ToString();
                }
                return arPageSettings;
            }
            return null;
        }
        public DataTable LoadGddSeries()
        {
            DataTable dtGdd = new DataTable();
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

            getServiceObj();
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.agriInfo;

                if (page.ToList().Count > 0 && page.ToList()[0].gdd != null)
                {
                    dtGdd = util.ConvertToDataTable(page.ToList()[0].gdd.series);
                }


            }

            return dtGdd;
        }
        public List<string> getGDDDefaults()
        {
            List<string> gddValues = new List<string>();
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.agriInfo.gdd;
                if (page.ToList() != null && page.ToList()[0] != null)
                {
                    gddValues.Add(page.ToList()[0].defaultMethod);
                    gddValues.Add(page.ToList()[0].defaultBase.ToString());
                    gddValues.Add(page.ToList()[0].defaultCap.ToString());
                }
            }
            return gddValues;
        }
        public void setSvcHandlerWebSvcValues(string strSvcName, string strModule)
        {
            //objServiceInfo = new ServiceInfo();
            //objServiceInfo.Module = strModuleName;
            //objServiceInfo.ServiceName = strServiceName;
            strServiceName = strSvcName;
            strModuleName = strModule;
        }
        public string GetChartName()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            string Name = string.Empty;
            getServiceObj();
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.agriInfo;

                Name = page.ToList()[0].chart.name;


            }
            return Name;
        }

        public bool GetSecuritySetting()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {

                return svc.secured;

            }
            return false;
        }
        public string GetEncryptionKey()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null && svc.encryptKey != null)
            {
                return svc.encryptKey.ToString();
            }
            return string.Empty;
        }
        public string GetRoles()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {

                return svc.allowedRoles;

            }
            return string.Empty;
        }
        //Method to read legend details based on legend name
        public DataTable GetLegendData(string name)
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();
            DataTable dt = new DataTable();
            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.legend.SingleOrDefault(x => x.name == name);
                if (page.ToList() != null && page.ToList()[0] != null)
                {
                    dt = util.ConvertToDataTable(page.ToList());
                }
            }
            return dt;
        }

        //Method to fetch showMap value
        public bool CheckMapVisibility()
        {
            objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            getServiceObj();

            if (svc != null)
            {
                if (svc.locationSearch != null)
                {
                    return svc.locationSearch.showMap;
                }
            }
            return false;
        }

        /*Wind Icon as a Sepearate component -- BEGIN*/
        public DataTable getWindIconSettings(string controlName)
        {
            int NumOfDays = 0;
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

                getServiceObj();

                /* SD01241630 - New Agricast webservices - can't get two views(iconsdailyandTabledata) - BEGIN -*/
                //When modules in different servicepages are called in web service, update the serviceinfo object with 
                // current module passed from the web service presenter 
                if (!string.IsNullOrEmpty(strModuleName) && !objServiceInfo.Module.ToLower().Equals(strModuleName.ToLower()))
                    objServiceInfo.Module = strModuleName;
                /* SD01241630 - New Agricast webservices - can't get two views(vew_dailyandTabledata) - END -*/
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }
            if (svc != null)
            {


                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.windIcon.SingleOrDefault(x => x.name == controlName);

                DataTable dt = util.ConvertToDataTable(page.ToList());
                DataColumn dc = new DataColumn("NumOfDays");
                dt.Columns.Add(dc);

                if ((dt.Rows[0]["endDate"].ToString() != "") && (dt.Rows[0]["startDate"].ToString() != ""))
                {
                    if (Convert.ToInt32(dt.Rows[0]["startDate"]) >= Convert.ToInt32(dt.Rows[0]["endDate"]))
                        NumOfDays = Convert.ToInt32(dt.Rows[0]["startDate"].ToString()) - Convert.ToInt32(dt.Rows[0]["endDate"].ToString());
                    else
                        NumOfDays = Convert.ToInt32(dt.Rows[0]["endDate"].ToString()) - Convert.ToInt32(dt.Rows[0]["startDate"].ToString());

                    //add the numof days to dt
                    dt.Rows[0][dc] = NumOfDays;
                }
                return dt;

            }
            return null;
        }

        public DataSet getWindIconSeries(string controlName)
        {
            DataSet ds = new DataSet();
            DataTable dtWindIcons = new DataTable();

            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }

            if (svc != null)
            {
                var page = from p in svc.servicePage
                           where p.name.ToLower() == objServiceInfo.Module.ToLower()
                           select p.windIcon.SingleOrDefault(x => x.name == controlName).series;

                if (page != null)
                {
                    dtWindIcons = util.ConvertToDataTable(page.ToList()[0]);
                }
            }


            dtWindIcons.TableName = "WindIconSerie";
            ds.Tables.Add(dtWindIcons);
            return ds;
        }
        /*Wind Icon as a Sepearate component -- BEGIN*/

        /* IM01277495 - issue reported by alec - BEGIN*/
       public string getServicePageDetails(string controlName)
        {
            string transtag=string.Empty;
            if (HttpContext.Current != null)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                getServiceObj();
            }
            else
            {
                svc = new service();
                svc = this.ReadConfig();
                objServiceInfo = new ServiceInfo();
                objServiceInfo.Module = strModuleName;
            }

            if (svc != null)
            {
            var page = from p in svc.servicePage
                       where p.name.ToLower() == objServiceInfo.Module.ToLower()
                       select p;   

                if (page != null)
                {
                   transtag= page.ToList()[0].trnsTag;
                }
            }

            return transtag;
        }
        /* IM01277495 - issue reported by alec - BEGIN*/

       /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - Begin */
       public string getModuleTransTag(string moduleName)
       {
           string transtag = string.Empty;
           if (HttpContext.Current != null)
           {
               objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
               getServiceObj();
           }
           else
           {
               svc = new service();
               svc = this.ReadConfig();
               objServiceInfo = new ServiceInfo();
               objServiceInfo.Module = strModuleName;
           }

           if (svc != null)
           {
               var page = from p in svc.servicePage
                          where p.name.ToLower() == moduleName.ToLower()
                          select p;

               if (page != null && page.FirstOrDefault() != null)
               {
                   transtag = page.First().trnsTag;
               }
           }

           if (string.IsNullOrEmpty(transtag))
               return moduleName;
           return transtag;
       }
       /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - End */
    }

    public class NearByPointSettings
    {
        public int MaxAllowedDistance {get;set;}
        public int MaxAllowedAltitude {get;set;}
        public int ResultCount  {get;set;}
        public enumDataSources DataSource {get;set;}
    }
}
