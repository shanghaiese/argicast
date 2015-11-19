using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace Syngenta.Agricast.Modals
{
    public class RS_EarlyBlight
    {
        #region Variables and Declarations

        //Input and Output datatable
        static DataTable dtInput = null;
        static DataRow drInput = null;
        static DataTable dtOutput = null;

        static DataTable dtInputRaw = null;

        static int iRowsCount = 0;
        static int iCurrHour = 0;

        //Holds Current Cell Value
        static string strCurrValue = string.Empty;

        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;

        //Variables to hold start and end dates
        static DateTime startDate;
        static DateTime endDate;

        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //stores a flag to determine day/Night
        static bool blIsNight = false;
        static int iOffSet = 0;

        //Set Default CellValue
        static string strDefaultCellValue = "2";

        //Indicates the range of data to be passed in the input datatable


        public static int StartHours
        {
            get { return 2; }

        }



        //endHours + 1 : ruleset input data change in DB 
        public static int EndHours
        {
            get { return 1; }
        }

        public static DataTable RulesetSeriesList
        {
            get
            {
                DataTable RulesetSerieList = new DataTable();
                RulesetSerieList.Columns.Add("Name");
                RulesetSerieList.Rows.Add("humid95");
                RulesetSerieList.Rows.Add("temp_min");
                return RulesetSerieList;
            }
        }

        #endregion

        #region Constants
        private static string TEMP_MIN = "temp_min";
        private static string HUMID95 = "humid95";
        private static string DATE = "date";

        //color codes
        private static string strRule1_val = "1";
        private static string strRule2_val = "2";
        private static string strRule3_val = "3";
        private static string strRuleDef_val = "0";
        #endregion
        static private void IntialiseOutputDatatable()
        {

            //Create output datatable
            dtOutput = new DataTable();
            dtOutput.Columns.Add("day");
            dtOutput.Columns.Add("hour");
            dtOutput.Columns.Add("ColourCode");
            dtOutput.Columns.Add("value");//Text i.e.Letter
            dtOutput.Columns.Add("restrictions");


        }

        /// <summary>
        /// This Method calls the ApplyRules()
        /// </summary>
        /// <returns>
        /// Output data table from ApplyRules() .
        /// </returns>
        //static public DataTable CalculateRuleSets(DataTable p_dtInput, DateTime p_sunRise, DateTime p_sunSet)
        static public DataTable CalculateRuleSets(IRuleSets objIRuleSets)
        {
            //IntialiseOutputDatatable();
            DataTable dt = null;

            //Assign the Value through Interface Members
            dtInputRaw = objIRuleSets.DtInput;
            startDate = objIRuleSets.StartDate;
            endDate = objIRuleSets.EndDate;
            dtOutput = objIRuleSets.DtOutput;
            sunRise = objIRuleSets.SunRise;
            sunSet = objIRuleSets.SunSet;


            //Filter the Raw Data and extract only the data between start and end dates
            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();

            TEMP_MIN = getColumnName(dtInput.Columns, "TempAir");
            HUMID95 = getColumnName(dtInput.Columns, "Humidity");

            dt = ApplyRules();

            return dt;
        }

        static private DataTable ApplyRules()
        {
            //Declare an Output Datarow object
            DataRow drOutput = null;

            //Store the Input data count
            iRowsCount = dtInput.Rows.Count;


            //Loop through Each CellValue
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                //Take the first Row
                drInput = dtInput.Rows[i];

                //Set the Current Hour . This is used in GetOutput Data method
                iCurrHour = i;

                if (drInput != null)
                {
                    //Call the method to check constraints and set cell color
                    CalcAllRules();
                }

                //Add a Row to Output Datatable
                drOutput = dtOutput.NewRow();

                //Add the Values to Output datatable
                drOutput["day"] = dtInput.Rows[i][DATE].ToString(); //Day

                drOutput["hour"] = DateTime.Parse(dtInput.Rows[i][DATE].ToString()).Hour;

                iOffSet = iCurrHour + 1;

                //set day/night
                SetDayNight();

                if (blIsNight)
                {
                    strCellValue = strCellValue + "0";
                }
                else
                {
                    strCellValue = strCellValue + "1";
                }
                drOutput["colorcode"] = strCellValue;

                dtOutput.Rows.Add(drOutput);

                //set the colorcode value to default code = 0
                strCellValue = "0";

            }//end of loop

            return dtOutput;
        }

        static private void CalcAllRules()
        {
            float fTempMin = float.MinValue;
            float fHumid95 = float.MinValue;

            //fetch the values from the datarow
            fTempMin = float.Parse(drInput[TEMP_MIN].ToString());
            fHumid95 = float.Parse(drInput[HUMID95].ToString());

            //<condition decimaterule="min" column="temp_min" upper="8"/>
            if (fTempMin <= 8.0)
            {
                strCellValue = strRule1_val;
            }

            //	<condition decimaterule="min" column="temp_min" lower="8"/>
            //<condition decimaterule="sum" column="humid95"  upper="12"/>
            if (fTempMin > 8.0)
            {
                //if humid <= 12 then ,rule 2 or else rule 3
                if (fHumid95 <= 12)
                {
                    strCellValue = strRule2_val;
                }

                //if humid > 12 then ,rule 2 or else rule 3
                if (fHumid95 > 12)
                {
                    strCellValue = strRule3_val;
                }
            }
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

        /// <summary>
        /// Checks if the current hour is day/night based on the Sunrise 
        /// and Sunset values obtained from the Calling Method
        /// </summary>
        static private void SetDayNight()
        {

            //Check isNight/day Based on the Sun Rise + Sun Set Values
            if (sunSet.ToString("tt").Equals("pm", StringComparison.InvariantCultureIgnoreCase))
            {
                sunSet.AddHours(12);
            }

            //If the Offset Hour is between Sunrise and Sunset , 
            //then its a day hour.else Night hour
            if (iOffSet > sunRise.Hour && iOffSet <= sunSet.Hour)
            {
                blIsNight = false;
            }
            else
            {
                blIsNight = true;
            }

        }

    }
}
