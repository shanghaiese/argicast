using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Syngenta.AgriCast.Common;
using System.Configuration;
using System.Web.Caching;


namespace Syngenta.AgriCast.Common.DataAccess
{
    public class TranslateData
    {
        DBConnections objDB = new DBConnections();
        string strText = string.Empty;


        /*First Time Page load Takes a long time issue - BEGIN*/
        /// <summary>
        /// This Method will be used to get all the translation tags for a  particular culture code from for all from DB and stores it in 
        /// global object
        /// </summary>
        /// <param name="strCultureCode"></param>
        public void GetTransTextOnFirstLoad(string strCultureCode)
        {
            if (HttpRuntime.Cache != null)
            {
                double expiry = double.Parse(ConfigurationManager.AppSettings["CacheExpiry"].ToString());
                Dictionary<string, object> glob = (Dictionary<string, object>)HttpRuntime.Cache["globalization"];

                if (glob == null)
                {
                    glob = new Dictionary<string, object>();
                    HttpRuntime.Cache.Insert("globalization", glob, null, DateTime.Now.AddMinutes(expiry), Cache.NoSlidingExpiration);
                }
                try
                {
                    Dictionary<string, string> culture = (Dictionary<string, string>)glob[strCultureCode];

                }
                //if not present in global object then get it from database
                catch (Exception ex)
                {
                    if (ex is System.NullReferenceException || ex is System.Collections.Generic.KeyNotFoundException)
                    {

                        //If the Culture code is not found in the global object
                        //Fetch all the Transtext from db for this culture code
                        string strConn = objDB.getConnectionString("LanguageDBConnectionString");
                        string strLabelName = string.Empty;
                        string strText = string.Empty;
                        SqlConnection conn = new SqlConnection(strConn);

                        ////Create sql command
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Connection.Open();
                        //cmd.CommandText = "SELECT Text FROM LangText LT LEFT OUTER JOIN LangLabels LL ON LT.LabelID=LL.LabelID INNER JOIN LangCultures LC on LC.LangID = LT.LangID WHERE LabelName='" + strLabelName + "' AND culturecode='" + strCultureCode + "'";
                        //cmd.CommandText = "SELECT dbo.AgriWeb_GetFullText('" + strLabelName + "', '" + strCultureCode + "')";
                        cmd.CommandText = "EXEC AgriWeb_GetTransTextOnFirstLoad '" + strCultureCode + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 120;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        DataTable dtTransText = new DataTable();
                        da.Fill(dtTransText);

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }


                        Dictionary<string, string> culture = new Dictionary<string, string>();
                        //Loop thru the DataTable and form the Culture Dictionary
                        if (dtTransText != null && dtTransText.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtTransText.Rows)
                            {
                                if (dr["LabelName"] != DBNull.Value)
                                    strLabelName = dr["LabelName"].ToString();
                                if (dr["FinalText"] != DBNull.Value)
                                    strText = dr["FinalText"].ToString();
                                if (!string.IsNullOrEmpty(strLabelName) && !string.IsNullOrEmpty(strText))
                                {
                                    /* IM01403827 - Error Warning on German Sprayweather Site - Jerrey - Begin */
                                    if (culture.ContainsKey(strLabelName))
                                        culture[strLabelName] = strText;
                                    else
                                    /* IM01403827 - Error Warning on German Sprayweather Site - Jerrey - End */
                                        culture.Add(strLabelName, strText);
                                }
                                /*IM01173327 - Taking the Right Translation Tag - BEGIN */
                                //clear the Label Name and Final Text once added to the cache object
                                strLabelName = string.Empty;
                                strText = string.Empty;
                                /*IM01173327 - Taking the Right Translation Tag - BEGIN */
                            }
                        }

                        glob.Add(strCultureCode, culture);
                        //HttpRuntime.Cache.Insert("globalization", glob, null, DateTime.Now.AddMinutes(expiry), Cache.NoSlidingExpiration);
                    }


                }
            }
        }
        /*First Time Page load Takes a long time issue - END*/


        /// <summary>
        /// Method to get the translated text by passing label name or text to be translated and culture code
        /// </summary>
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            try
            {
                if (HttpRuntime.Cache != null)
                {
                    double expiry = double.Parse(ConfigurationManager.AppSettings["CacheExpiry"].ToString());
                    Dictionary<string, object> glob = (Dictionary<string, object>)HttpRuntime.Cache["globalization"];

                    if (glob == null)
                    {
                        glob = new Dictionary<string, object>();
                        HttpRuntime.Cache.Insert("globalization", glob, null, DateTime.Now.AddMinutes(expiry), Cache.NoSlidingExpiration);
                    }
                    //SOCB on 15-Mar-2012 for translation change
                    //check in global object if translation exists
                    try
                    {
                        if (!glob.ContainsKey(strCultureCode))
                        {
                            GetTransTextOnFirstLoad(strCultureCode);
                        }
                            
                            Dictionary<string, string> culture = (Dictionary<string, string>)glob[strCultureCode];
                            strText = culture[strLabelName].ToString();
                        
                    }
                    //if not present in global object then get it from database
                    catch (System.NullReferenceException)
                    {
                        //strText = getTranslatedTextDB(strLabelName, strCultureCode);
                        strText = "{" + strLabelName + "}";
                        //Dictionary<string, string> culture = new Dictionary<string, string>();
                        //culture.Add(strLabelName, strText);
                        //glob.Add(strCultureCode, culture);
                        //HttpRuntime.Cache.Insert("globalization", glob, null, DateTime.Now.AddMinutes(expiry), Cache.NoSlidingExpiration);
                    }
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        //strText = getTranslatedTextDB(strLabelName, strCultureCode);
                        strText = "{" + strLabelName + "}";
                        //Dictionary<string, string> culture;
                        //try
                        //{
                        //    culture = (Dictionary<string, string>)glob[strCultureCode];
                        //}
                        //catch (System.Collections.Generic.KeyNotFoundException)
                        //{
                        //    culture = new Dictionary<string, string>();
                        //    glob.Add(strCultureCode, culture);
                        //}
                        //culture.Add(strLabelName, strText);
                        //glob.Remove(strCultureCode); 
                    }
                    //HttpRuntime.Cache.Insert("globalization", glob, null, DateTime.Now.AddMinutes(expiry), Cache.NoSlidingExpiration);
                    // HttpRuntime.Cache["globalization"] = glob;
                    //EOCB on 15-Mar-2012 for translation change
                }
                else
                {
                    strText = getTranslatedTextDB(strLabelName, strCultureCode);
                }

                return strText;
            }
            catch (Exception)
            {
                return strText;
            }
        }


        /// <summary>
        /// Method to get the translated text by passing label name or text to be translated and culture code from database
        /// </summary>
        public string getTranslatedTextDB(string strLabelName, string strCultureCode)
        {
            string strText;
            string strConn = objDB.getConnectionString("LanguageDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);

            ////Create sql command
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Connection.Open();
            //cmd.CommandText = "SELECT Text FROM LangText LT LEFT OUTER JOIN LangLabels LL ON LT.LabelID=LL.LabelID INNER JOIN LangCultures LC on LC.LangID = LT.LangID WHERE LabelName='" + strLabelName + "' AND culturecode='" + strCultureCode + "'";
            //cmd.CommandText = "SELECT dbo.AgriWeb_GetFullText('" + strLabelName + "', '" + strCultureCode + "')";
            cmd.CommandText = "EXEC AgriWeb_Get_FullText '" + strLabelName + "', '" + strCultureCode + "'";
            cmd.CommandType = CommandType.Text;
            strText = Convert.ToString(cmd.ExecuteScalar());
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            if (strText == null || strText.ToString().Trim() == "")
            {
                //SOCB - Append {} to text if not translation found  
                strText = "{" + strLabelName + "}";
            }
            return strText;
        }
    }
}