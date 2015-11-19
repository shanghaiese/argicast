using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace Syngenta.Agricast.Modals
{
    public class RS_Bruchid
    {
        #region Variables and Declarations

        //Input and Output datatable
        static DataTable dtInput = null;
        static DataRow drInput = null;
        static DataTable dtOutput = null;
        static DataTable dtInputRaw = null;

        static int iRowsCount = 0;
        static int iCurrHour = 0;
        static int iOffSet = 0;

        //Holds Current Cell Value
        static string strCurrValue = string.Empty;

        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;

        static string strCellLetter = string.Empty;

        //Set Default CellValue
        static string strDefaultCellValue = "2";

        //Variables to hold start and end dates
        static DateTime startDate;
        static DateTime endDate;

        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;


        //Indicates the range of data to be passed in the input datatable


        public static int StartHours
        {
            get { return -3; }

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
                RulesetSerieList.Rows.Add("temp_max");
                return RulesetSerieList;
            }
        }

        static bool blIsNight = false;

        #endregion

        #region Constants
        private static string TEMP_MAX = "temp_max";
        private static string DATE = "date";

        //color codes
        private static string strDefault_Val = "2";
        private static string strRule2_val = "1";


        //Letter/Value
        private static string RS_Bruchid_Default = "X";//Rule 1 in pub.config.xml(gbcp07ri)
        private static string RS_Bruchid_Rule2 = "Y";

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

            //Filter the Raw Data and extract only the data between start and end dates
            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();

            //Assign the Value through Interface Members
            dtInputRaw = objIRuleSets.DtInput;
            startDate = objIRuleSets.StartDate;
            endDate = objIRuleSets.EndDate;
            dtOutput = objIRuleSets.DtOutput;
            sunRise = objIRuleSets.SunRise;
            sunSet = objIRuleSets.SunSet;
            TEMP_MAX = getColumnName(dtInput.Columns, "temp");

            //Apply rules
            dt = ApplyRules();

            return dt;
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

        static private DataTable ApplyRules()
        {
            //Declare an Output Datarow object
            DataRow drOutput = null;
            DateTime dateLocal = DateTime.Now;

            //Store the Input data count
            iRowsCount = dtInput.Rows.Count;
            DataRow drMax_M1 = null;
            DataRow drMax_M2 = null;
            DataRow drMax_M3 = null;

            double fMax_M1 = float.MinValue;
            double fMax_M2 = float.MinValue;
            double fMax_M3 = float.MinValue;


            //Loop through Each CellValue
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                //Fetch the Current Hour
                dateLocal = DateTime.Parse(dtInput.Rows[i][DATE].ToString());

                //get Temp_max of cuurrent hour - 1
                drMax_M1 = dtInputRaw.Select("" + DATE + " = '" + dateLocal.AddHours(-1) + "'")[0];

                //get Temp_max of cuurrent hour - 2
                drMax_M2 = dtInputRaw.Select("" + DATE + " = '" + dateLocal.AddHours(-2) + "'")[0];

                //get Temp_max of cuurrent hour - 2
                drMax_M3 = dtInputRaw.Select("" + DATE + " = '" + dateLocal.AddHours(-3) + "'")[0];

                fMax_M1 = (double.TryParse(drMax_M1[TEMP_MAX].ToString(), out fMax_M1) ? fMax_M1 : 0);
                fMax_M2 = (double.TryParse(drMax_M2[TEMP_MAX].ToString(), out fMax_M2) ? fMax_M2 : 0);
                fMax_M3 = (double.TryParse(drMax_M3[TEMP_MAX].ToString(), out fMax_M3) ? fMax_M3 : 0);

                //Get the Maximium of M1(-1 hour) and M2 (-2 hour)data
                double dMax_M1_M2 = Math.Max(fMax_M1, fMax_M2);

                //Get the Maximium of M1(-1 hour) and M2 (-2 hour)data
                double dMax_M2_M3 = Math.Max(fMax_M2, fMax_M3);

                //if true ,then allow spraying
                if (dMax_M1_M2 > 20 || dMax_M2_M3 > 20)
                {
                    strCellValue = strRule2_val;//Colorcode

                    strCellLetter = RS_Bruchid_Rule2;//Letter

                }
                else
                //assign default rule :- rule 1
                {
                    strCellValue = strDefault_Val;//ColoroCode
                    strCellLetter = RS_Bruchid_Default;//Letter
                }


                #region Output Data
                //Add the Data to Output Datatable

                drOutput = dtOutput.NewRow();

                drOutput["day"] = dateLocal;
                drOutput["hour"] = dateLocal.Hour;

                //set the offset
                iOffSet = dateLocal.Hour + 1;

                SetDayNight();

                if (blIsNight)
                {
                    strFinalCellValue = strCellValue + "0";
                }
                else
                {
                    strFinalCellValue = strCellValue + "1";
                }


                if (strFinalCellValue.Contains("00"))
                {
                    strFinalCellValue = "0";
                }
                else if (strFinalCellValue.Contains("01"))
                {
                    strFinalCellValue = "1";
                }

                //set the final Colorcode
                drOutput["ColorCode"] = strFinalCellValue;

                //Set the letter
                drOutput["value"] = strCellLetter;

                dtOutput.Rows.Add(drOutput);

                //cleare the placeholders
                strFinalCellValue = string.Empty;
                strCellLetter = string.Empty;
                strCellValue = string.Empty;

                #endregion
            }//end of loop

            return dtOutput;
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
