using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Syngenta.Agricast.Modals
{
   static public class GDD
   {

       static string method;
       static int tbase=0;
       static int tcap=0;
       static int tmin=0;
       static int tmax=0;
       static string TEMP_MAX = "Tempair_C_Daytimemax";
       static string TEMP_MIN = "Tempair_C_NightTimemin";
        
       static void IntialiseOutputDatatable()
       {
           //Create output datatable
           dtOutput = new DataTable();
           dtOutput.Columns.Add("date",typeof(DateTime));           
           dtOutput.Columns.Add("value");
       }
       public static void CalculateGDD()
       {            
           method = Method;
           tcap = Cap != 0?Cap:1000;
           tbase = Base;
           TEMP_MAX = getColumnName(dtInput.Columns, "daytimemax");
           TEMP_MIN = getColumnName(dtInput.Columns, "nighttimemin");
           if (method.ToLower() == "eu")
           {
               tmin = -1000;
               tmax = 1000;
           }
           else
           {
               tmin = tbase; 
               tmax = 1000;
           }

           DataRow drOut = null;
           double preVal = 0d;
           IntialiseOutputDatatable();
           for (int i = 0; i < dtInput.Rows.Count; i++)
           {
               double min = (dtInput.Rows[i][TEMP_MIN].ToString() != "") ? double.Parse(dtInput.Rows[i][TEMP_MIN].ToString()) : double.MinValue;
               double max = (dtInput.Rows[i][TEMP_MAX].ToString() != "") ? double.Parse(dtInput.Rows[i][TEMP_MAX].ToString()) : double.MaxValue; 
               double curVal = Calculate(min,max,tmin,tmax,tbase,tcap);
               preVal = preVal + curVal;
               drOut = dtOutput.NewRow();
               drOut["date"] = dtInput.Rows[i][0];
               if (DateTime.Parse(dtInput.Rows[i][0].ToString()) <= DateTime.Today)
               {                   
                   drOut["value"] = preVal;                   
               }
               dtOutput.Rows.Add(drOut);
           }
       }
       /// <summary>
       /// Calculate the GDD for a day
       /// For a EU calculation, use Tmin=-1000, Tmax=1000, Tcap=1000
       /// For a US calculation, use Tmin=Tbase, Tmax=1000
       /// 
       /// Tmean = (Math.Max(dailyTmin, Tmin) + Math.Min(dailyTmax, Tmax)) / 2;
       /// result = Math.Max(Tmean - Tbase, 0);
       /// result = Math.Min(result, Tcap);
       /// </summary>
       /// <param name="type">Calculation Algorithm</param>
       /// <param name="Tmin">Min temprature of the day</param>
       /// <param name="Tmax">Max temperature of the day</param>
       /// <param name="Tbase">Base temperature for the calculation</param>
       /// <param name="Tcap">Cap temperature for the calculation. Set it to 1000 if this parameter is unnecessary</param>
       /// <returns></returns>
        static double Calculate(double dailyTmin, double dailyTmax, double Tmin, double Tmax, double Tbase, double Tcap)
            {
                double Tmean = (Math.Max(dailyTmin, Tmin) + Math.Min(dailyTmax, Tmax)) / 2;
                double result = Math.Max(Tmean - Tbase, 0);
                result = Math.Min(result, Tcap);
                return result;
            }

       public static string Version()
            {
                return "$Id: GDD.cs 266 2012-02-09 20:34:19Z Jerome1 $\n";
            }

       /// <summary>
       /// Gets the Columne name from the Collections
       /// </summary>
       /// <param name="cols"></param>
       /// <param name="phrase"></param>
       /// <returns></returns>
       static string getColumnName(DataColumnCollection cols, string phrase)
       {
           var colName = from c in cols.Cast<DataColumn>()
                         where c.ColumnName.ToLower().Contains(phrase.ToLower())
                         select c.ColumnName.ToString();
           return colName.ToList()[0].ToString();
       }
        
       public static DataTable RulesetSeriesList
       {
           get
           {
               DataTable RulesetSerieList = new DataTable();
               RulesetSerieList.Columns.Add("Name");
               RulesetSerieList.Rows.Add("TempAir_C_DayTimeMax");
               RulesetSerieList.Rows.Add("TempAir_C_NightTimeMin");
               return RulesetSerieList;
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

        }
}
