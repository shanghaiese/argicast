using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace Syngenta.Agricast.Modals
{
    public class RS_SevereWeathere
    {
        #region Variables and Declarations

        //Input and Output datatable
        static DataTable dtInput = null;
        static DataRow drInput = null;
        static DataTable dtOutput = null;


        //Holds Current Cell Value
        static string strCurrValue = string.Empty;

        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;


        //Set Default CellValue
        static string strDefaultCellValue = "2";

        static string strRestrictionList = string.Empty;

        static DataTable dtInputRaw = null;

        //Variables to hold start and end dates
        static DateTime startDate;
        static DateTime endDate;

        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //Stores the Input data count
        static int iRowsCount = 0;
        static int iCurrHour = 0;
        static int iOffSet = 0;
        static bool blIsNight = false;

        //Indicates the range of data to be passed in the input datatable


        public static int StartHours
        {
            get { return 0; }

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

                RulesetSerieList.Rows.Add("temp_min");
                RulesetSerieList.Rows.Add("temp_min5");
                RulesetSerieList.Rows.Add("temp_max");
                RulesetSerieList.Rows.Add("PrecipType");
                RulesetSerieList.Rows.Add("PrecipProbability_h");
                RulesetSerieList.Rows.Add("precipextrapolated");
                RulesetSerieList.Rows.Add("HumidityRel");
                RulesetSerieList.Rows.Add("WindGustSpeed_hMax");
                RulesetSerieList.Rows.Add("ThunderstormProbability_h");
                return RulesetSerieList;
            }
        }
        #endregion

        #region Constants
        private static string DATE = "date";
        private static string TEMP_MIN = "temp_min";
        private static string TEMP_MIN_5 = "temp_min5";
        private static string TEMP_MAX = "temp_max";
        private static string PRECIPTYPE = "PrecipType";
        private static string PROB_PRECIP = "PrecipProbability_h";
        private static string PRECIP_EXTRA = "precipextrapolated";
        private static string HUMID = "HumidityRel";
        private static string WINDGUSTS = "WindGustSpeed_hMax";
        private static string PROB_THUNDER = "ThunderstormProbability_h";

        //Descriptions
        private static string strResLegHeat = "ResLegHeat";
        private static string strResLegFrost = "ResLegFrost";
        private static string strResLegExtremeCold = "ResLegExtremeCold";
        private static string strResLegHeavyRain = "ResLegHeavyRain";
        private static string strResLegHeavySnow = "ResLegHeavySnow";
        private static string strResLegHumidity = "ResLegHumidity";
        private static string strResLegHighWinds = "ResLegHighWinds";
        private static string strResLegThunderstrom = "ResLegThunderstrom";


        //Color Codes
        private const string RS_SevereWeathere_ResLegHeat = "T";
        private const string RS_SevereWeathere_ResLegFrost = "F";
        private const string RS_SevereWeathere_ResLegExtremeCold = "C";
        private const string RS_SevereWeathere_ResLegHeavyRain = "R";
        private const string RS_SevereWeathere_ResLegHeavySnow = "S";
        private const string RS_SevereWeathere_ResLegHumidity = "H";
        private const string RS_SevereWeathere_ResLegHighWinds = "W";
        private const string RS_SevereWeathere_ResLegThunderstrom = "Th";
        private const string RS_SevereWeathere_Default = "X";



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

            dtInputRaw = objIRuleSets.DtInput;
            startDate = objIRuleSets.StartDate;
            endDate = objIRuleSets.EndDate;
            dtOutput = objIRuleSets.DtOutput;
            sunRise = objIRuleSets.SunRise;
            sunSet = objIRuleSets.SunSet;


            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();


            HUMID = getColumnName(dtInput.Columns, "Humidity");
            WINDGUSTS = getColumnName(dtInput.Columns, "WindSpeedMax");
            PRECIPTYPE = getColumnName(dtInput.Columns, "PrecipType");
            TEMP_MIN = getColumnName(dtInput.Columns, "temp_min");
            TEMP_MIN_5 = getColumnName(dtInput.Columns, "temp_min5");
            TEMP_MAX = getColumnName(dtInput.Columns, "temp_max");
            PROB_PRECIP = getColumnName(dtInput.Columns, "PrecipProbability_h");
            PRECIP_EXTRA = getColumnName(dtInput.Columns, "precipextrapolated");

            PROB_THUNDER = getColumnName(dtInput.Columns, "ThunderstormProbability_h");

            

            DataTable dt = ApplyRules();

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
                //Get the Current row data
                drInput = dtInput.Rows[i];

                //Set the Current Hour . This is used in GetOutput Data method
                iCurrHour = i;


                //Call the method to check constraints and set cell color
                CalcAllRules();

                //Call the Output method
                GetOutputData(i);




            }//end of loop

            return dtOutput;
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


        static private void CalcAllRules()
        {
            //<rule id="rule1" letter="T" adddescription="ResLegHeat" value="0">
            //    <condition column="temp_max" lower="30" />
            if (float.Parse(drInput[TEMP_MAX].ToString()) > 30.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegHeat))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegHeat;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegHeat;
                }

            }

            //<rule id="rule2" letter="F" adddescription="ResLegFrost" value="0">
            //    <condition column="temp_min5" upper="0" />
            //</rule>
            if (float.Parse(drInput[TEMP_MIN_5].ToString()) <= 0.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegFrost))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegFrost;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegFrost;
                }

            }

            //<rule id="rule3" letter="C" adddescription="ResLegExtremeCold" value="0">
            //    <condition column="temp_min" upper="-20" />
            //</rule>
            if (float.Parse(drInput[TEMP_MIN].ToString()) <= -20.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegExtremeCold))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegExtremeCold;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegExtremeCold;
                }

            }

            //<rule id="rule6" letter="H" adddescription="ResLegHumidity">
            //      <condition column="humid" lower="90" />
            //      <condition column="temp_min" lower="0" />
            //  </rule>
            if (float.Parse(drInput[HUMID].ToString()) > 90.0 ||
                float.Parse(drInput[TEMP_MIN].ToString()) > 0.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegHumidity))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegHumidity;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegHumidity;
                }

            }

            //<rule id="rule7" letter="W" adddescription="ResLegHighWinds" value="0">
            //    <condition column="windgusts" lower="20" />
            //</rule>
            if (float.Parse(drInput[WINDGUSTS].ToString()) > 20.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegHighWinds))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegHumidity;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegHighWinds;
                }

            }

            //<rule id="rule8" letter="Th" adddescription="ResLegThunderstrom" value="0">
            //    <condition column="probthunder" lower="75" />
            //</rule>
            if (float.Parse(drInput[PROB_THUNDER].ToString()) > 75.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegThunderstrom))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegThunderstrom;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegThunderstrom;
                }

            }

            //    <rule id="rule4" letter="R" adddescription="ResLegHeavyRain" value="0">
            //    <condition column="preciptype" lower="0" upper="2" />
            //    <condition column="probprecip" lower="50" />
            //    <condition column="precipextrapolated" lower="3" />
            //</rule>
            if ((float.Parse(drInput[PRECIPTYPE].ToString()) > 0.0 && float.Parse(drInput[PRECIPTYPE].ToString()) <= 2.0) ||
                float.Parse(drInput[PROB_PRECIP].ToString()) > 50.0 ||
            float.Parse(drInput[PRECIP_EXTRA].ToString()) > 3.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegHeavyRain))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegHeavyRain;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegHeavyRain;
                }

            }

            //<rule id="rule5" letter="S" adddescription="ResLegHeavySnow" value="0">
            //    <condition column="preciptype" lower="2" />
            //    <condition column="probprecip" lower="50" />
            //    <condition column="precipextrapolated" lower="2" />
            //</rule>
            if (float.Parse(drInput[PRECIPTYPE].ToString()) > 2.0 ||
               float.Parse(drInput[PROB_PRECIP].ToString()) > 50.0 ||
               float.Parse(drInput[PRECIP_EXTRA].ToString()) > 2.0)
            {
                //Set the High Risk Value
                strCellValue = "0";

                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResLegHeavySnow))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegHeavySnow;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegHeavySnow;
                }

            }
        }

        /// <summary>
        /// This Method checks if the restriction already exist.
        /// If yes , then returns true
        /// </summary>
        /// <param name="strRest"></param>
        /// <returns></returns>
        static private bool CheckRestrictioninList(string strRest)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(strRestrictionList))
            {
                int index = strRestrictionList.IndexOf(strRest);
                if (index != -1)
                {

                    //Rule Exist , return True
                    retVal = true;
                }
                else
                {
                    //Rule Does not exist , return false
                    retVal = false;
                }
            }
            return retVal;
        }


        static private void GetOutputData(int index)
        {
            //Create a new row
            DataRow drOutput = dtOutput.NewRow();

            string[] strRestList = null;
            if (!string.IsNullOrEmpty(strRestrictionList))
            {
                strRestList = strRestrictionList.Split(',').ToArray();
            }

            drOutput["day"] = dtInput.Rows[index][DATE].ToString();

            //set the current hour
            iCurrHour = DateTime.Parse(dtInput.Rows[index][DATE].ToString()).Hour;
            drOutput["hour"] = iCurrHour;

            //Check if the current hour belongs to day/night
            iOffSet = iCurrHour + 1;

            //check if the current hour false is day/night
            SetDayNight();

            if (string.IsNullOrEmpty(strCellValue))
            {
                strCellValue = "2";//set it to Default - No risk
            }

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

            //Assign the ColorCode
            drOutput["ColorCode"] = strFinalCellValue;

            //loop through each of the restriction and assignt e
            if (strRestList != null)
            {

                //Letter
                //Set the Letter as "X" if there are multiple restriction for a cell value
                if (strRestList.Length > 1)
                {
                    drOutput["value"] = RS_SevereWeathere_Default;
                }
                else
                {
                    //if there is only one restriction ,
                    //Get the letter from GetRestrictionLetter() by passing the rulecode/rulename
                    drOutput["value"] = GetRestrictionLetter(strRestList[0]);
                }

                //Resrtictions
                drOutput["restrictions"] = strRestrictionList;

            }


            //Add the Row to Output Datatable
            dtOutput.Rows.Add(drOutput);

            //Set oto null after adding the row
            drOutput = null;

            //Clear the Restirction List
            strRestrictionList = string.Empty;

            //clear the cell Value
            strCellValue = string.Empty;



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


        /// <summary>
        /// Returns the Letter to be dispalyed in the cell based on the Restriction Name
        /// </summary>
        /// <param name="strRestName"></param>
        /// <returns></returns>
        static private string GetRestrictionLetter(string strRestName)
        {
            string strLetter = string.Empty;


            if (strRestName.Equals(strResLegHeat))
            {
                strLetter = RS_SevereWeathere_ResLegHeat;
            }
            else if (strRestName.Equals(strResLegFrost))
            {
                strLetter = RS_SevereWeathere_ResLegFrost;
            }
            else if (strRestName.Equals(strResLegExtremeCold))
            {
                strLetter = RS_SevereWeathere_ResLegExtremeCold;
            }
            else if (strRestName.Equals(strResLegHeavyRain))
            {
                strLetter = RS_SevereWeathere_ResLegHeavyRain;
            }
            else if (strRestName.Equals(strResLegHeavySnow))
            {
                strLetter = RS_SevereWeathere_ResLegHeavySnow;
            }
            else if (strRestName.Equals(strResLegHumidity))
            {
                strLetter = RS_SevereWeathere_ResLegHumidity;
            }
            else if (strRestName.Equals(strResLegHighWinds))
            {
                strLetter = RS_SevereWeathere_ResLegHighWinds;
            }
            else if (strRestName.Equals(strResLegThunderstrom))
            {
                strLetter = RS_SevereWeathere_ResLegThunderstrom;
            }
            //Return the Letter
            return strLetter;


        }
    }
}
