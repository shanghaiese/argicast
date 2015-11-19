using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Syngenta.AIS.ModelLibrary;
using AIS.UsefulFunctions.DerivedCalculations;
using Syngenta.AgriCast.Common.DataAccess;
using System.Data.SqlClient;
using System.Web;
using Syngenta.AgriCast.Common.Service;

namespace Syngenta.Agricast.Modals
{
    static public class BorderWaterModel
    {
        const string TEMP_MAX = "TempAir_C_DaytimeMax";
        const string TEMP_MIN = "TempAir_C_NighttimeMin";
        const string PRECIP_SUM = "PrecipAmount_mm_DailySum";
        public const string S_TOPSOIL_MM = "CropMoistureIndex_TopSoil_mm";

        public static void CalculateBWM()
        {
            CommonUtil cUtil = new CommonUtil();

            List<Serie> lstSerie = new List<Serie>();
            lstSerie.Add(new Serie("date", string.Empty));
            lstSerie.Add(new Serie(TEMP_MAX, string.Empty));
            lstSerie.Add(new Serie(TEMP_MIN, string.Empty));
            lstSerie.Add(new Serie(PRECIP_SUM, string.Empty));

            double HE = 0, BB = 0;
            GetHEBB(GridID, out HE, out BB);

            SerieList<Serie> objSerie = new SerieList<Serie>(lstSerie, dtInput);
            CropMoistureIndexModel objcrop = new CropMoistureIndexModel();
            objcrop.Calculate(TotalWHC, Latitude, HE, BB, objSerie);

            /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - Start */
            //you have to show data till todays date -2 days
            if (EndDate.Date.Subtract(DateTime.Now.Date).Days > 0)
                EndDate = DateTime.Now.Date.AddDays(-2);

            if (StartDate > EndDate)
            {
                DateTime tmp = StartDate;
                StartDate = EndDate;
                EndDate = tmp;
            }

            if (StartDate.Date.Subtract(DateTime.Now.Date).Days > 0
                && EndDate.Date.Subtract(DateTime.Now.Date).Days > 0)
            {
                HttpContext.Current.Session["ErrorMessage"] = cUtil.getTransText(Syngenta.AgriCast.Common.Constants.INVALID_DATE_RANGE);
                throw new Exception(cUtil.getTransText(Syngenta.AgriCast.Common.Constants.INVALID_DATE_RANGE));
                return;
            }
            dtOutput = objSerie.Table.Select(string.Format("date>=#{0}# and date<=#{1}#",
                                                        StartDate.ToString("yyyy-MM-dd"),
                                                        EndDate.ToString("yyyy-MM-dd"))).CopyToDataTable();
            // dtOutput = objSerie.Table;
            /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - End */

            foreach (DataRow row in dtOutput.Rows)
            {
                row[S_TOPSOIL_MM] = Convert.ToDouble(row[S_TOPSOIL_MM]) + TotalWHC;
            }
            dtOutput.AcceptChanges();

            // if date is out of today
            if (EndDate.Subtract(StartDate).Days > dtOutput.Rows.Count)
            {
                var missedDataCount = EndDate.Subtract(StartDate).Days - dtOutput.Rows.Count + 1;
                for (var i = 0; i < missedDataCount; i++)
                {
                    DataRow row = dtOutput.NewRow();
                    row[0] = StartDate.AddDays(dtOutput.Rows.Count);
                    dtOutput.Rows.Add(row);
                }
            }

            dtOutput.AcceptChanges();
        }

        public static DataTable RulesetSeriesList
        {
            get
            {
                DataTable RulesetSerieList = new DataTable();
                RulesetSerieList.Columns.Add("Name");
                RulesetSerieList.Rows.Add(TEMP_MAX);
                RulesetSerieList.Rows.Add(TEMP_MIN);
                RulesetSerieList.Rows.Add(PRECIP_SUM);
                return RulesetSerieList;
            }
        }

        public static string ThresholdRange
        {
            get
            {
                /**
                 *  if total WHC is between 50 mm and 75 mm: Green between 100- 75%, amber between 74 - 65%, red < 65 %
                 *  if total WHC is between 75 mm and 100 mm: Green between 100- 75%, amber between 74 - 60%, red < 60 %
                 *  if total WHC is between 100 mm and 125 mm: Green between 100- 75%, amber between 74 - 60%, red < 60 %
                 *  if total WHC is between 125 mm and 150 mm: Green between 100- 65%, amber between 64 - 59 %, red < 59 %
                 *  if total WHC is between 150 mm and 175 mm: Green between 100- 60%, amber between 59- 50 %, red <  50 %
                 *  if total WHC is between 175 mm and 200 mm: Green between 100- 60%, amber between 59- 50 %, red <  50 %
                 *  if total WHC is between 200 mm and 225 mm: Green between 100- 55%, amber between 54- 45 %, red < 45 %
                 *  if total WHC is between 225 mm and 250 mm: Green between 100- 55%, amber between 54- 45 %, red < 45 %
                 *  if total WHC is between 250 mm and 275 mm: Green between 100-50 %, amber between 49 - 40 %, red < 39 %
                 *  if total WHC is between 275 mm and 300 mm: Green between 100-50 %, amber between 49 - 40 %, red < 39 %
                 *  if total WHC is above 300 mm : Green between 100 - 50 %, amber between 49- 40 %, red < 40 %
                 */
                int rangeMin = 0;
                int rangeMax = 0;

                if (TotalWHC > 300)
                {
                    rangeMin = 40;
                    rangeMax = 50;
                }
                else if (TotalWHC > 250)
                {
                    rangeMin = 39;
                    rangeMax = 50;
                }
                else if (TotalWHC > 200)
                {
                    rangeMin = 45;
                    rangeMax = 55;
                }
                else if (TotalWHC > 150)
                {
                    rangeMin = 50;
                    rangeMax = 60;
                }
                else if (TotalWHC > 125)
                {
                    rangeMin = 59;
                    rangeMax = 65;
                }
                else if (TotalWHC > 75)
                {
                    rangeMin = 60;
                    rangeMax = 75;
                }
                else if (TotalWHC > 50)
                {
                    rangeMin = 65;
                    rangeMax = 75;
                }

                return string.Format("{0}-{1}",
                    Math.Round(TotalWHC * rangeMin / 100),
                    Math.Round(TotalWHC * rangeMax / 100));
            }
        }

        public static int GridID { get; set; }
        public static double Latitude { get; set; }
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; }
        public static double TopSoilDepth { get; set; }
        public static double SubSoilDepth { get; set; }
        public static double TopSoilWHC { get; set; }
        public static double SubSoilWHC { get; set; }

        public static double TotalWHC
        {
            get
            {
                /* calculate WHC
                - the WHC are in mm/mm of soil
                - the depth are in cm
                - the result is in mm
                */
                return ((TopSoilWHC * TopSoilDepth) + (SubSoilWHC * SubSoilDepth)) * 10;
            }
        }
        public static double TotalDepth
        {
            get
            {
                return TopSoilDepth + SubSoilDepth;
            }
        }

        public static int Base
        {
            get;
            set;
        }
        public static int Cap
        {
            get;
            set;
        }
        public static string Method
        {
            get;
            set;
        }

        public static DataTable dtInput
        {
            get;
            set;
        }
        public static DataTable dtOutput
        {
            get;
            set;
        }

        // get HE/BB from stored procedure.
        public static void GetHEBB(int gridid, out double HE, out double BB)
        {
            DBConnections objDB = new DBConnections();
            string strConn = objDB.getConnectionString("P_Grows");
            SqlConnection conn = new SqlConnection(strConn);
            HE = 0;
            BB = 0;

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "AgriInfo_Get_ThornthwaiteHeatIndex";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@GridID", gridid);

                SqlParameter param = new SqlParameter();
                param.SqlDbType = SqlDbType.Decimal;
                param.ParameterName = "@HE";
                param.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.SqlDbType = SqlDbType.Decimal;
                param.ParameterName = "@BB";
                param.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param);

                conn.Open();
                cmd.ExecuteNonQuery();

                HE = Convert.ToDouble(cmd.Parameters[1].Value);
                BB = Convert.ToDouble(cmd.Parameters[2].Value);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }
        }
    }

}
