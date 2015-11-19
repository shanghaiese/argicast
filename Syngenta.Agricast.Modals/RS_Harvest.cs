using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Syngenta.Agricast.Modals
{
    public class RS_Harvest
    {
        #region Variables and Declarations

        //Input and Output datatable
        static DataTable dtInput = null;
        static DataTable dtInputRaw = null;
        static DataRow drInput = null;
        static DataTable dtOutput = null;
        static DataRow drOutput = null;
        static DataTable dtCellValues = null;
        static int iRowsCount = 0;
        static int iCurrHour = 0;
        static int iOffSet = 0;

        //Flag to check if Aggreate Rule is applied
        static bool isAggregate = false;

        //Holds Current Cell Value
        static string strCurrValue = string.Empty;
        static bool isResLegRain = false;
        static bool isResRainExpected = false;

        //stored Cell Color
        static string strCellColor = string.Empty;

        static string strRestriction = string.Empty;

        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;

        //Set Default CellValue
        static string strDefaultCellValue = "2";


        static string strRestrictionList = string.Empty;


        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //stores a flag to determine day/Night
        static bool blIsNight = false;

        static DateTime startDate;
        static DateTime endDate;

        //Indicates the range of data to be passed in the input datatable


        public static int StartHours
        {
            get { return -96; }

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

                RulesetSerieList.Rows.Add("PrecipQuantity_hSum");
                RulesetSerieList.Rows.Add("soiltemp");
                RulesetSerieList.Rows.Add("HumidityRel");
                return RulesetSerieList;
            }
        }

        enum RuleNames
        {
            strResLegTooWetGround, strResLegTooWet, strResLegTooWetRecent, strResLegTooColdLong, strResLegTooCold, strResLegTooHumid

        }

        #endregion

        #region Constants


        private const string RS_Harvest_ResLegTooWetGround = "A";
        private const string RS_Harvest_ResLegTooWet = "B";
        private const string RS_Harvest_ResLegTooWetRecent = "C";
        private const string RS_Harvest_ResLegTooColdLong = "D";
        private const string RS_Harvest_ResLegTooCold = "E";
        private const string RS_Harvest_ResLegTooHumid = "F";
        private const string RS_Harvest_Default = "X";

        private static string PRECIP = "PrecipQuantity_hSum";
        private static string TEMP = "soiltemp";
        private static string HUMID = "HumidityRel";
        private static string DATE = "date";

        //Constants to hold Restriction Desciption
        private static string strResLegTooWetGround = "ResLegTooWetGround-96";
        private static string strResLegTooWet = "ResLegTooWet-48";
        private static string strResLegTooWetRecent = "ResLegTooWetRecent-23";
        private static string strResLegTooColdLong = "ResLegTooColdLong-48";
        private static string strResLegTooCold = "ResLegTooCold";
        private static string strResLegTooHumid = "ResLegTooHumid-6";

        #endregion

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


            //Set the Cell Value to default
            strCellValue = strDefaultCellValue;

                     //Filter the Raw Data and extract only the data between start and end dates
            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();

            //Check if all the required data is present
            PRECIP = getColumnName(dtInput.Columns, "PrecipAmount");
            TEMP = getColumnName(dtInput.Columns, "TempAir");
            HUMID = getColumnName(dtInput.Columns, "Humidity");

   
            dt = ApplyRules();


            return dt;
        }


        static private DataTable ApplyRules()
        {

            //Total rows from filtered data
            iRowsCount = dtInput.Rows.Count;

            //<!-- soil temp last 3 days too low - no plantings -->
            //<condition start="-72" end="-1" decimaterule="min" column="soiltemp" upper="14"/>
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                CalcAllRules(i);

                //Add the values to Output Data
                GetOutputData(i);
            }
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

        static private void CalcAllRules(int startIndex)
        {
            //Get the Current cell date and time
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[startIndex][DATE].ToString());


            //check for Condition
            //<rule value="1" letter="A" adddescription="ResLegTooWetGround-96">
            //    <condition start="-96" end="0" decimaterule="sum" column="precip" lower="200" />
            //</rule>

            double i = 0.0;
            double fAvg = 0.0;


            //<rule value="1" letter="A" adddescription="ResLegTooWetGround-96">
            //    <condition start="-96" end="0" decimaterule="sum" column="precip" lower="200" />
            var resulthours_96 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-96) &&
                                 DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtSum = grp.Sum(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0)
                                 };


            //<rule value="1" letter="D" adddescription="ResLegTooColdLong-48">
            //	<condition start="-48" end="0" decimaterule="sum" column="temp" upper="720" />
            //<rule value="1" letter="B" adddescription="ResLegTooWet-48">
            //    <condition start="-48" end="0" decimaterule="sum" column="precip" lower="1" />
            var resulthours_48 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-48) &&
                                 DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtSumPrecip = grp.Sum(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0),
                                     dtSumTemp = grp.Sum(t => double.TryParse(t[TEMP].ToString(), out i) ? i : 0.0)
                                 };


            //<rule value="1" letter="C" adddescription="ResLegTooWetRecent-23">
            //<condition start="-23" end="1" decimaterule="sum" column="precip" lower="0" />
            var resulthours_23 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-23) &&
                                 DateTime.Parse(dt[DATE].ToString()) <= dateLocal.AddHours(1)
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtSumPrecip = grp.Sum(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0),

                                 };

            //<rule value="1" letter="F" adddescription="ResLegTooHumid-6">
            // <rule value="0" letter="F" adddescription="ResLegTooHumid-6">
            var resulthours_06 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-6) &&
                                 DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //Average of Current Hour to Hour - 6
                                     dtAvgTemp = grp.Average(t => double.TryParse(t[TEMP].ToString(), out i) ? i : 0.0),
                                     dtAvgHumid = grp.Average(t => double.TryParse(t[HUMID].ToString(), out i) ? i : 0.0)
                                 };

            //	<rule value="1" letter="E" adddescription="ResLegTooCold">
            //<condition start="0" end="1" decimaterule="avg" column="temp" upper="10" />
            DataRow[] drInput_H0 = dtInputRaw.Select("" + DATE + "='" + dateLocal + "'");
            DataRow[] drInput_H1 = dtInputRaw.Select("" + DATE + "='" + dateLocal.AddHours(1) + "'"); ;

            if (drInput_H0 != null && drInput_H1 != null)
            {

                fAvg = ((double.TryParse(drInput_H0[0][TEMP].ToString(), out i) ? i : 0.0) +
                        (double.TryParse(drInput_H1[0][TEMP].ToString(), out i) ? i : 0.0)) / 2.0;
            }

            float fSum_H96 = float.Parse(resulthours_96.ToList()[0].dtSum.ToString());//Fetch the sum of data (hour -96 to 0)
            float fSum_H48_Precip = float.Parse(resulthours_48.ToList()[0].dtSumPrecip.ToString());//Precipitation:-(hour -48 to 0)
            float fSum_H48_Temp = float.Parse(resulthours_48.ToList()[0].dtSumPrecip.ToString());//Temparature:-(hour -48 to 0)
            float fAvg_H06_Temp = float.Parse(resulthours_06.ToList()[0].dtAvgTemp.ToString());//Temparature:-(hour -6 to 0)
            float fAvg_H06_Humid = float.Parse(resulthours_06.ToList()[0].dtAvgHumid.ToString());//Humidity:-(hour -6 to 0)
            float fSum_H23_Precip = float.Parse(resulthours_23.ToList()[0].dtSumPrecip.ToString());//Precipitation:-(hour -23 to 1)


            //	<rule value="1" letter="A" adddescription="ResLegTooWetGround-96">
            //	<condition start="-96" end="0" decimaterule="sum" column="precip" lower="200" />
            if (fSum_H96 > 200.0)
            {

                if (!CheckRestrictioninList(strResLegTooWetGround))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooWetGround + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooWetGround + ",";
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }

            //<rule value="1" letter="B" adddescription="ResLegTooWet-48">
            //<condition start="-48" end="0" decimaterule="sum" column="precip" lower="1" />
            if (fSum_H48_Precip > 1.0)
            {

                if (!CheckRestrictioninList(strResLegTooWet))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooWet + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooWet + ",";
                }
                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }


            //<rule value="1" letter="C" adddescription="ResLegTooWetRecent-23">
            //<condition start="-23" end="1" decimaterule="sum" column="precip" lower="0" />
            if (fSum_H23_Precip > 0)
            {
                if (!CheckRestrictioninList(strResLegTooWetRecent))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooWetRecent + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooWetRecent + ",";
                }
                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }


            //<rule value="1" letter="D" adddescription="ResLegTooColdLong-48">
            //<condition start="-48" end="0" decimaterule="sum" column="temp" upper="720" />
            if (fSum_H48_Temp <= 720)
            {

                if (!CheckRestrictioninList(strResLegTooColdLong))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdLong + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooColdLong + ","; ;
                }
                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }


            //<rule value="1" letter="E" adddescription="ResLegTooCold">
            //<condition start="0" end="1" decimaterule="avg" column="temp" upper="10" />
            if (fAvg <= 10.0)
            {

                if (!CheckRestrictioninList(strResLegTooCold))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooCold + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooCold + ","; ;
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }

            //<rule value="1" letter="F" adddescription="ResLegTooHumid-6">
            //<condition start="-6" end="0" decimaterule="avg" column="humid" lower="75" upper="90" />
            //<condition start="-6" end="0" decimaterule="avg" column="temp" upper="15" />
            if ((fAvg_H06_Humid > 75 && fAvg_H06_Humid <= 90) ||
                (fAvg_H06_Temp <= 15))
            {
                if (!CheckRestrictioninList(strResLegTooHumid))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooHumid + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooHumid + ","; ;
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();

            }

            //<rule value="0" letter="F" adddescription="ResLegTooHumid-6">
            //<condition start="-6" end="0" decimaterule="avg" column="humid" lower="90" />
            //<condition start="-6" end="0" decimaterule="avg" column="temp" upper="15" />
            if ((fAvg_H06_Humid > 90) ||
                (fAvg_H06_Temp <= 15))
            {
                if (!CheckRestrictioninList(strResLegTooHumid))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooHumid + ",";
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegTooHumid + ",";
                }

                //Set the Cell Value to "0" - high risk
                strCellValue = "0";

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
            strRest = strRest + ",";

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

        /// <summary>
        /// Setss the Value to Medium Risk Factor(value=1)
        /// </summary>
        static private void SetCellValue()
        {
            if (!strCellValue.Equals("0"))//Not equal to 0
            {
                strCellValue = "1";

            }
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
                    drOutput["value"] = RS_Harvest_Default;
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


        static private string GetRestrictionLetter(string strRest)
        {
            string strLetter = string.Empty;

            if (strRest.Equals(strResLegTooWetGround))
            {
                strLetter = RS_Harvest_ResLegTooWetGround;
            }
            else if (strRest.Equals(strResLegTooWet))
            {
                strLetter = RS_Harvest_ResLegTooWet;
            }
            else if (strRest.Equals(strResLegTooWetRecent))
            {
                strLetter = RS_Harvest_ResLegTooWetRecent;
            }
            else if (strRest.Equals(strResLegTooColdLong))
            {
                strLetter = RS_Harvest_ResLegTooColdLong;
            }
            else if (strRest.Equals(strResLegTooCold))
            {
                strLetter = RS_Harvest_ResLegTooCold;
            }

            else if (strRest.Equals(strResLegTooHumid))
            {
                strLetter = RS_Harvest_ResLegTooHumid;
            }

            return strLetter;
        }
    }
}
