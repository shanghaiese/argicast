using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Syngenta.AgriCast.Common.DataAccess;
using System.Xml;
using System.Collections;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;
using Syngenta.AIS.LocationSearch.Internal.Package;
using Syngenta.AgriWeb.LocationSearch.DataObjects;
using Syngenta.AgriWeb.LocationSearch.DataObjects;

namespace Syngenta.AgriCast.LocationSearch.DataAccess
{
    class locSearchData
    {
        //string strConn;
        DBConnections objDB = new DBConnections();
        TranslateData objTranslate = new TranslateData();

        public DataTable getLocResults(string strSearch, string strCntry)
        {
            throw new System.NotImplementedException();
        }

        //Method to fetch the translated text by passing lablename/id and the culture code
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            string strText = objTranslate.getTranslatedText(strLabelName, strCultureCode);
            return strText;
        }


        //public DataTable getFavorites()
        //{
        //    string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
        //    SqlConnection conn = new SqlConnection(strConn);
        //    try
        //    {  
        //        //Create sql command
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = conn;
        //        cmd.Connection.Open();
        //        cmd.CommandText = "SELECT sh_fav_name,sh_fav_placename,sh_fav_moduleid,sh_fav_id FROM  WHERE sh_fav_f_user_id=19377 AND ( sh_fav_moduleid='globalsetting' OR sh_fav_moduleid='weathercast' OR sh_fav_moduleid='mySyngenta_Home') ORDER BY sh_fav_name";
        //        cmd.CommandType = CommandType.Text;
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }

        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (new Exception("Cannot get favorites.", ex));
        //    }

        //    finally
        //    {
        //        conn.Dispose();
        //    }  
        //}

        //public void updateFavorite(string strNewFavName, int intFavId)
        //{
        //    string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
        //    SqlConnection conn = new SqlConnection(strConn);
        //    try
        //    {
        //        ////Create sql command
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = conn;
        //        cmd.Connection.Open();
        //        cmd.CommandText = "update sh_favorites set sh_fav_name =" + "'" + strNewFavName + "' where sh_fav_id =" + intFavId;
        //        cmd.CommandType = CommandType.Text;
        //        int ret;
        //        ret = cmd.ExecuteNonQuery();
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (new Exception("Cannot update favorites.", ex));
        //    }
        //    finally
        //    {
        //        conn.Dispose();
        //    }    
        //}

        //public DataSet getCountry()
        //public DataTable getCountry(string strCountryCodes)
        //{           
        //    //Setting connection string value
        //    string strConn = objDB.getConnectionString("LocationDBConnectionString");
        //    SqlConnection conn = new SqlConnection(strConn);
        //    try
        //    {
        //        //Create sql command
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = conn;
        //        cmd.Connection.Open();
        //        cmd.CommandText = "Select code,dbo.ReturnStrName(la.Area_Name,la.Native_Area_Name) AS Name  FROM Location_Areas LA inner join [ISOCountryCodes] c on la.areaid=c.AreaID WHERE la.areaid in(" + strCountryCodes + ") ORDER BY Name";
        //        cmd.CommandType = CommandType.Text;
        //        DataTable dt = new DataTable();
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (new Exception("Cannot get country list.", ex));
        //    }
        //    finally
        //    {
        //        conn.Dispose();
        //    }

        //}

        public List<Location> getLocationDetails(string strName, string strCountry, LocationSearchSource eProvider, double lat, double lang, string culture)
        {
            /*INCIDENT IM02468422 - //overwrite - Changes the Token in Application - BEGIN */
            //var result = LocationSearchConsumer.GenerateLocationObj(strCountry, strName, eProvider, culture, "New Agricast", "getLocationDetails");
            var result = LocationSearchConsumer.GenerateLocationObj(strCountry, strName, eProvider, culture, "FBF9DCA5-849E-4B0F-AE51-DD64F7614A16", "getLocationDetails");
            /*INCIDENT IM02468422 - //overwrite - Changes the Token in Application - BEGIN */
            Regex rx = new Regex(@"^[0-9]+$");
            if (rx.IsMatch(strName))
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)HttpContext.Current.Session["AuditData"];
                    dict["locSearchStringType"] = "Postal Code";
                    HttpContext.Current.Session["AuditData"] = dict;
                }
            }
            return result;

        }

        //public DataSet getLocationDetails(string strName, string strCountry, string strProvider, double lat, double lang, string culture)
        //{

        //    var output = getLocationDetails(strName, strCountry, strProvider, lat, lang, culture, 1);
        //    DataSet ds = new DataSet();
        //    DataSet dsLocation = new DataSet();
        //    bool isPostalCode = false;
        //    //If the user enter the coordinates
        //    if (strName.IndexOf(',') > 0 && strName.Split(',')[0] != null)
        //    {
        //        double.TryParse(strName.Split(',')[0], out lat);
        //        double.TryParse(strName.Split(',')[1], out lang);
        //    }
        //    else
        //    {
        //        Regex rx = new Regex(@"^[0-9]+$");
        //        //If the user enters the location name
        //        //Condition to check for Postal code
        //        if (rx.IsMatch(strName))
        //        {
        //            lat = 0.00;
        //            isPostalCode = true;
        //            if (HttpContext.Current != null)
        //            {
        //                if (HttpContext.Current.Session["AuditData"] != null)
        //                {
        //                    IDictionary dict = (IDictionary)HttpContext.Current.Session["AuditData"];
        //                    dict["locSearchStringType"] = "Postal Code";
        //                    HttpContext.Current.Session["AuditData"] = dict;
        //                }
        //            }

        //        }
        //        else
        //        {
        //            lat = 0.00;
        //        }
        //    }
        //    if (culture.Length > 2)
        //    {
        //        CultureInfo cult = new CultureInfo(culture);
        //        culture = cult.TwoLetterISOLanguageName;
        //    }


        //    //if (lat == 0.00)
        //    //{
        //    if (strProvider.Contains("geoname"))
        //    {
        //        string proxyName = ConfigurationManager.AppSettings["proxyName"];
        //        WebClient webClient = new WebClient();
        //        webClient.Encoding = Encoding.UTF8;
        //        XmlDocument xdoc = new XmlDocument();
        //        string username = ConfigurationManager.AppSettings["UserName"];
        //        string domain = ConfigurationManager.AppSettings["Domain"];
        //        string password = ConfigurationManager.AppSettings["Password"];
        //        webClient.Credentials = new System.Net.NetworkCredential(username, password, domain);
        //        webClient.Headers.Add(HttpRequestHeader.UserAgent, "test");
        //        WebProxy proxy = new WebProxy(proxyName, true);
        //        proxy.Credentials = new System.Net.NetworkCredential(username, password, domain);
        //        webClient.Proxy = proxy;
        //        string xml;
        //        //Condition to check for postal code and to fetch the records accordingly
        //        if (isPostalCode)
        //        {
        //            xml = webClient.DownloadString("http://ws.geonames.net/postalCodeSearch?style=full&featureClass=P&username=lostinswiss&placename=" + strName + "&country=" + strCountry);
        //        }
        //        else
        //        {
        //            xml = webClient.DownloadString("http://ws.geonames.net/search?style=full&featureClass=P&username=lostinswiss&name_startsWith=" + strName + "&country=" + strCountry);
        //        }
        //        xdoc.LoadXml(xml);
        //        ds.ReadXml(new XmlNodeReader(xdoc));
        //        if (Int32.Parse(ds.Tables["geonames"].Rows[0]["TotalResultsCount"].ToString()) > 0 && !isPostalCode)
        //        {
        //            ds.Tables["geoname"].Columns.Add("AlternatePlaceName");
        //            foreach (DataRow dr in ds.Tables["alternateName"].Rows)
        //            {
        //                if (dr["lang"].ToString().ToLower() == culture.ToLower())
        //                {
        //                    foreach (DataRow drow in ds.Tables["geoname"].Rows)
        //                    {
        //                        if (drow["geoname_id"].ToString() == dr["geoname_id"].ToString())
        //                            drow["AlternatePlaceName"] = dr["alternateName_Text"];

        //                    }
        //                    //break;
        //                }
        //            }
        //            DataView view = new DataView(ds.Tables["geoname"]);
        //            string[] columnNames = { "name", "lat", "lng", "geonameID", "countryCode", "countryName", "adminName1", "adminName2", "AlternatePlaceName" };
        //            DataTable dtGeoname = view.ToTable(true, columnNames);
        //            dtGeoname.TableName = "geoname";

        //            DataView view1 = new DataView(ds.Tables["geonames"]);
        //            string[] columnNames1 = { "totalResultsCount", "geonames_Id" };
        //            DataTable dtGeonames = view1.ToTable(true, columnNames1);
        //            dtGeonames.TableName = "geonames";


        //            dsLocation.Tables.Add(dtGeonames);
        //            dsLocation.Tables.Add(dtGeoname);

        //            //DataTable dtGeoname = new DataTable();
        //            //var geoname = from DataRow row in ds.Tables["geoname"].AsEnumerable()
        //            //                       select new
        //            //                   {
        //            //                       name = row.Field<string>("name"),
        //            //                       lat = row.Field<double>("lat"),
        //            //                       lng = row.Field<double>("lng"),
        //            //                       geonameID = row.Field<string>("geonameID"),
        //            //                       countryCode = row.Field<string>("countryCode"),
        //            //                       countryName = row.Field<string>("countryName"),
        //            //                       adminName1 = row.Field<string>("adminName1"),
        //            //                       adminName2 = row.Field<string>("adminName2")


        //            //                   };

        //            //IEnumerable<DataRow> query = ds.Tables["geoname"].Select(
        //            //    delegate (ds.Tables["geoname"] detail)
        //            //{
        //            //    var newRow = ds1.tbl2.Newtbl2Row();
        //            //    newRow.BalancelDue = detail.BalancelDue;
        //            //    newRow.ShipToFax = detail.ShipToFax;
        //            //    return newRow;
        //            //});


        //            //((DataRow[])geoname.ToList()[0][0]).CopyToDataTable(dtGeoname, LoadOption.PreserveChanges);

        //            //geoname.CopyToDataTable(dtGeoname, LoadOption.PreserveChanges);

        //            //    DataTable dtGeonames = new DataTable();
        //            //var geonames = from row in ds.Tables["geonames"].AsEnumerable()
        //            //               select new
        //            //                   {
        //            //                       totalResultsCount = row.Field<int>("name"),
        //            //                       geonames_Id = row.Field<double>("lat")
        //            //                   };

        //            //dsLocation.Tables["geoname"].Columns.Remove("toponymName");
        //            //dsLocation.Tables["geoname"].Columns.Remove("fcl");
        //            //dsLocation.Tables["geoname"].Columns.Remove("fcode");
        //            //dsLocation.Tables["geoname"].Columns.Remove("fclName");
        //            //dsLocation.Tables["geoname"].Columns.Remove("fcodeName");
        //            //dsLocation.Tables["geoname"].Columns.Remove("population");
        //            //dsLocation.Tables["geoname"].Columns.Remove("alternateNames");
        //            //dsLocation.Tables["geoname"].Columns.Remove("elevation");
        //            //dsLocation.Tables["geoname"].Columns.Remove("continentCode");
        //            //dsLocation.Tables["geoname"].Columns.Remove("adminCode1");
        //            //dsLocation.Tables["geoname"].Columns.Remove("adminCode2");
        //            //dsLocation.Tables["geoname"].Columns.Remove("adminCode3");




        //            //dsLocation.Relations.Clear();
        //            //dsLocation.Tables["alternateName"].ChildRelations.Clear();
        //            ////  dsLocation.Tables["geoname"].Constraints.Remove("ForeignKeyConstraint geoname_alternateName");

        //            //dsLocation.Tables["alternateName"].Clear();
        //            //// dsLocation.Tables.Remove("alternateName");

        //            dsLocation.AcceptChanges();
        //        }
        //        else if (Int32.Parse(ds.Tables["geonames"].Rows[0]["TotalResultsCount"].ToString()) > 0 && isPostalCode)
        //        {
        //            ds.Tables["code"].Columns.Remove("adminCode1");
        //            ds.Tables["code"].Columns.Remove("adminCode2");
        //            ds.Tables["code"].Columns.Remove("adminCode3");
        //            ds.AcceptChanges();
        //            dsLocation = ds;
        //        }
        //    }
        //    else
        //    {
        //        string strConn = objDB.getConnectionString("LocationDBConnectionString");
        //        SqlConnection conn = new SqlConnection(strConn);
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = conn;
        //            cmd.CommandText = "dbo.AgriWeb_LocationSearch";
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@ISOCode", strCountry);
        //            cmd.Parameters.AddWithValue("@searchStr", strName);
        //            cmd.Parameters.AddWithValue("@maxRows", 50);
        //            cmd.Parameters.AddWithValue("@debug", 0);
        //            conn.Open();
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dsLocation);
        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }

        //            if (dsLocation.Tables.Count > 0 && dsLocation.Tables[0].Columns.Contains("LocationName"))
        //            {
        //                dsLocation.Tables[0].Columns["LocationName"].ColumnName = "name";
        //                dsLocation.Tables[0].Columns["Lat"].ColumnName = "lat";
        //                dsLocation.Tables[0].Columns["Long"].ColumnName = "lng";
        //                dsLocation.Tables[0].Columns.Add("adminName1").DefaultValue = "";
        //                dsLocation.Tables[0].Columns.Add("adminName2").DefaultValue = "";

        //                foreach (DataRow dr in dsLocation.Tables[0].Rows)
        //                {
        //                    if (dr["name"].ToString().Contains(","))
        //                    {

        //                        dr["adminName1"] = dr["name"].ToString().Split(',')[1];
        //                        if (dr["name"].ToString().Substring(dr["name"].ToString().IndexOf(',') + 1).Contains(','))
        //                            dr["adminName2"] = dr["name"].ToString().Split(',')[2];
        //                        dr["name"] = dr["name"].ToString().Split(',')[0];
        //                    }
        //                }
        //                dsLocation.Tables[0].AcceptChanges();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw (new Exception("Cannot get Location data.", ex));
        //        }
        //        finally
        //        {
        //            conn.Dispose();
        //        }
        //    }
        //    //}
        //    //else
        //    //{
        //    //    //Setting connection string value
        //    //    string strConn = objDB.getConnectionString("P2_ForecastDBConnectionString");
        //    //    SqlConnection conn = new SqlConnection(strConn);
        //    //    try
        //    //    {
        //    //        //Create sql command
        //    //        SqlCommand cmd = new SqlCommand();
        //    //        cmd.Connection = conn;
        //    //        cmd.Connection.Open();
        //    //        cmd.CommandText = "SELECT Top 1 StationProviderID AS Station_ID, REPLACE(Name, '_', ' ') as Station_Name, DistanceKm As StaDist,TimezoneID AS TZone_ID," +
        //    //                            "Altitude AS STATION_ALTITUDE,Longitude AS STATION_LONGITUDE,Latitude AS STATION_LATITUDE,TimezoneOffset,DstOn," + "ProviderName FROM " +
        //    //                            "Agricast_StationDetails_Location(" + lat + "," + lang + ")";
        //    //        cmd.CommandType = CommandType.Text;
        //    //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    //        da.Fill(dsLocation);
        //    //        if (conn.State == ConnectionState.Open)
        //    //        {
        //    //            conn.Close();
        //    //        }
        //    //    }
        //    //catch (Exception ex)
        //    //{
        //    //    throw (new Exception("Cannot get Location data.", ex));
        //    //}
        //    //finally
        //    //{
        //    //    conn.Dispose();
        //    //}
        //    //}                     
        //    return dsLocation;
        //}

        //Method to get locations for autosuggestion
        public List<string> getAutoSuggestLocation(string strCountryCode, string strSearchString, LocationSearchSource provider)
        {
            List<string> names = new List<string>();
            List<Location> locations = getLocationDetails(strSearchString, strCountryCode, provider, 0, 0, "en-GB");
            if (locations != null && locations.Count > 0)
            {

                /*IM01246266 - New Agricast - can't save a favourite - BEGIN*/
                var AllRecords = from row in locations.AsEnumerable()
                                 select (
                                     //string.Format("{0},{1},{2}|{3}", row.Field<string>("name"), row.Field<string>("adminName1"), row.Field<string>("adminName2"), row["lat"].ToString() + "," + row["lng"].ToString()).TrimEnd(',')
                                      string.Format("{0},{1},{2}", row.Name, row.AdminName1, row.AdminName2).TrimEnd(',') +
                                      string.Format("|{0}", row.Latitude.ToString() + "," + row.Longitude.ToString()).TrimEnd(',')
                                      + string.Format("|{0}", row.PlaceId.ToString())
                                      );

                /*IM01246266 - New Agricast - can't save a favourite - END*/
                //var Top10Records = AllRecords.Take(10);
                //DataRow[] AllRecords = dt.AsEnumerable().Select(item => item["name"] + "," + item["adminName1"] + "," + item["adminName2"]).Take(10);

                names = AllRecords.Take(10).ToList();
                return names;
            }
            else
                return names;

        }

        //Method to fetch the offset time for a given station id
        public DataTable getTimeZoneOffset(int istationID)
        {
            string strConn = objDB.getConnectionString("P2_ForecastDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);
            try
            {
                ////Create sql command
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.CommandText = " SELECT TimezoneOffset,DstOn FROM V_Agricast_TimezoneDetails WHERE TimezoneID=(SELECT TimezoneID AS TZone_ID " +
                                    "FROM Agricast_StationDetails_Station(" + istationID + ",0,0))";
                cmd.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw (new Exception("Cannot get Timezone offset.", ex));
            }
            finally
            {
                conn.Dispose();
            }
        }
    }
}
