using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Collections;
using Syngenta.AgriCast.Common.Service;

namespace Syngenta.AgriCast.Common.DataAccess
{
    public class CommonData
    {
        DBConnections objDB = new DBConnections();

        public DataTable getFavorites()
        {
            string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);
            try
            {
                ////Create sql command
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                //cmd.Connection.Open();
                cmd.CommandText = "SELECT sh_fav_name,sh_fav_placename,sh_fav_moduleid,sh_fav_id FROM sh_favorites WHERE sh_fav_f_user_id=19377 AND ( sh_fav_moduleid='globalsetting' OR sh_fav_moduleid='weathercast' OR sh_fav_moduleid='mySyngenta_Home') ORDER BY sh_fav_name";
                cmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
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
                throw (new Exception("Cannot get favorites.", ex));
            }

            finally
            {
                conn.Dispose();
            }
        }

        public void updateFavorite(string strNewFavName, int intFavId)
        {
            string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);
            try
            {
                ////Create sql command
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.CommandText = "update sh_favorites set sh_fav_name =" + "'" + strNewFavName + "' where sh_fav_id =" + intFavId;
                cmd.CommandType = CommandType.Text;
                int ret;
                ret = cmd.ExecuteNonQuery();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Favorites could not be updated.", ex));
            }

            finally
            {
                conn.Dispose();
            }
        }

        public void deleteFavorite(int intFavId)
        {
            string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);
            try
            {
                ////Create sql command
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.CommandText = "Delete sh_favorites where sh_fav_id=" + intFavId;
                cmd.CommandType = CommandType.Text;
                int ret;
                ret = cmd.ExecuteNonQuery();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Favorites could not be deleted.", ex));
            }

            finally
            {
                conn.Dispose();
            }
        }

        
        public DataTable GetFavPlaceDetails(string PlaceID)

        {

            string strConn = objDB.getConnectionString("LocationDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);

            try
            {
                ////Create sql command
                DataTable dtLocationDetails = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Loc_GetFavLocationDetails";

               
                SqlParameter param = new SqlParameter();
                param.SqlDbType = SqlDbType.VarChar;
                param.ParameterName = "@PlaceID";
                param.Direction = ParameterDirection.Input;
                param.Value = PlaceID;

                cmd.Parameters.Add(param);
         

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dtLocationDetails);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return dtLocationDetails;
            }

            catch (Exception ex)
            {
                throw (new Exception("Cannot get Favorite Location Details.", ex));
            }

            finally
            {
                conn.Dispose();
            }

        }
       

        //Method to calculate sunrise and sunset 
        public DateTime getSunriseOrSunset(DateTime day, double lat, double lng, string strSunriseOrSunset)
        {
            string strConn = objDB.getConnectionString("P_Grows");
            SqlConnection conn = new SqlConnection(strConn);

            try
            {
                ////Create sql command
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.CommandText = "SELECT dbo.AgriWeb_GetSunriseOrSunset ('" + day.ToString("yyyy-MM-dd HH:mm:ss") + "', " + lat + ", " + lng + ", '" + strSunriseOrSunset + "')";
                cmd.CommandType = CommandType.Text;
                DateTime dtSunriseOrSunset;
                dtSunriseOrSunset = Convert.ToDateTime(cmd.ExecuteScalar());
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return dtSunriseOrSunset;
            }

            catch (Exception ex)
            {
                throw (new Exception("Cannot get Sunrise or Sunset Data.", ex));
            }

            finally
            {
                conn.Dispose();
            }
        }
        public void SaveRatings(List<string[]> ratings, string serviceName, string userName)
        {
            string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);
            try
            {
                foreach (string[] ratingDetails in ratings)
                {

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "AgriSaveRatings";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Service", serviceName);
                    cmd.Parameters.AddWithValue("@Module", ratingDetails[2]);
                    cmd.Parameters.AddWithValue("@Section", ratingDetails[0]);
                    cmd.Parameters.AddWithValue("@Rating", ratingDetails[1]);
                    cmd.Parameters.AddWithValue("@User", userName);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Ratings could not be saved.", ex));
            }

            finally
            {
                conn.Dispose();
            }
        }

        public void SaveAuditData(IDictionary dict)
        {
            string strConn = objDB.getConnectionString("AgriCoreDBConnectionString");
            SqlConnection conn = new SqlConnection(strConn);
            try
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Agri_SaveAuditData";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userIP", dict["userIP"]);
                cmd.Parameters.AddWithValue("@userID", dict["userID"]);
                cmd.Parameters.AddWithValue("@token", dict["token"]);
                cmd.Parameters.AddWithValue("@referrer", dict["referrer"]);
                cmd.Parameters.AddWithValue("@entrancePath", dict["entrancePath"]);
                cmd.Parameters.AddWithValue("@culture", dict["culture"]);
                cmd.Parameters.AddWithValue("@sessionID", dict["sessionID"]);
                cmd.Parameters.AddWithValue("@service", dict["service"]);
                cmd.Parameters.AddWithValue("@module", dict["module"]);
                cmd.Parameters.AddWithValue("@locSearchType", dict["locSearchType"]);

                cmd.Parameters.AddWithValue("@locSearchStringType", dict["locSearchStringType"]);
                cmd.Parameters.AddWithValue("@locSearchString", dict["locSearchString"]);
                cmd.Parameters.AddWithValue("@locSearchDatasource", dict["locSearchDatasource"]);
                cmd.Parameters.AddWithValue("@numOfLocs", dict["numOfLocs"]);
                cmd.Parameters.AddWithValue("@searchLat", dict["searchLat"]);
                cmd.Parameters.AddWithValue("@searchLong", dict["searchLong"]);
                cmd.Parameters.AddWithValue("@countryName", dict["countryName"]);
                cmd.Parameters.AddWithValue("@locName", dict["locName"]);
                cmd.Parameters.AddWithValue("@weatherDatasource", dict["weatherDatasource"]);
                cmd.Parameters.AddWithValue("@weatherLat", dict["weatherLat"]);
                cmd.Parameters.AddWithValue("@weatherLong", dict["weatherLong"]);
                cmd.Parameters.AddWithValue("@weatherDateFrom", dict["weatherDateFrom"]);
                cmd.Parameters.AddWithValue("@weatherDateTo", dict["weatherDateTo"]);
                cmd.Parameters.AddWithValue("@weatherParams", dict["weatherParams"]);

                conn.Open();
                cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }


            }
            catch (Exception ex)
            {
                throw (new Exception("Audit data could not be saved.", ex));
            }

            finally
            {
                conn.Dispose();
            }
        }
    }
}