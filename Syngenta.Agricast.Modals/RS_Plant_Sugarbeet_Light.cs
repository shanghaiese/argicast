using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Syngenta.Agricast.Modals
{
    public class RS_Plant_Sugarbeet_Light
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
        static bool isResLegFrost = false;
        static bool isResLegTooWetGround = false;

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
            get { return -48; }
            
        }


        //endHours + 1 : ruleset input data change in DB 
        public static int EndHours
        {
            get { return 73; }
            
        }
        public static DataTable RulesetSeriesList
        {
            get
            {
                DataTable RulesetSerieList = new DataTable();
                RulesetSerieList.Columns.Add("Name");

                RulesetSerieList.Rows.Add("PrecipQuantity_hSum");
                RulesetSerieList.Rows.Add("soiltemp");
                RulesetSerieList.Rows.Add("MoistureSoilSand010cm");
                RulesetSerieList.Rows.Add("temp_min5");
                return RulesetSerieList;
            }
        }
        #endregion

        #region Constants


        private const string RS_Plant_Sugarbeet_Light_ResLegTooColdSoil = "A";
        private const string RS_Plant_Sugarbeet_Light_ResLegTooColdSoil24 = "A";
        private const string RS_Plant_Sugarbeet_Light_ResLegTooColdSoil48 = "A";
        private const string RS_Plant_Sugarbeet_Light_ResLegTooColdSoil72 = "A";
        private const string RS_Plant_Sugarbeet_Light_ResLegTooWetGround = "M";
        private const string RS_Plant_Sugarbeet_Light_ResLegRain = "R";
        private const string RS_Plant_Sugarbeet_Light_ResLegFrost = "C";
        private const string RS_Plant_Sugarbeet_Light_Default = "X";

        private static string PRECIP = "PrecipQuantity_hSum";
        private static string SOILTEMP = "soiltemp";
        private static string MOISTURESOIL_SAND = "MoistureSoilSand010cm";
        private static string DATE = "date";
        private static string TEMPMIN = "temp_min5";

        //Constants to hold Restriction Desciption
        private static string strResLegTooColdSoil = "ResLegTooColdSoil";
        private static string strResLegTooColdSoil24 = "ResLegTooColdSoil24";
        private static string strResLegTooColdSoil48 = "ResLegTooColdSoil48";
        private static string strResLegTooColdSoil72 = "ResLegTooColdSoil72";
        private static string strResLegRain = "ResLegRain";
        private static string strResLegTooWetGround = "ResLegTooWetGround";
        private static string strResLegFrost = "ResLegFrost";

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



            //Filter the Raw Data and extract only the data between start and end dates
            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();

            //Check if all the required data is present
            PRECIP = getColumnName(dtInput.Columns, "PrecipAmount");
            SOILTEMP = getColumnName(dtInput.Columns, "soilTemp");
            MOISTURESOIL_SAND = getColumnName(dtInput.Columns, "MoistureSoilClay010cm");
            TEMPMIN = getColumnName(dtInput.Columns, "temp_min5");

            dt = ApplyRules();


            return dt;
        }

        /// <summary>
        /// this Method is used to interpolate the Specified Input Variables
        /// </summary>
        static private void InterPolateValues()
        {

            DataRow[] drArrayTemp = null;
            float fMoistureSand = 0.0f;
            float fTempMin5 = 0.0f;

            //Assuming that there will be only one value for "Soil Temp" and "Moisture Clay" for a particular day
            //interpolate the values. the Exisiting values will be overridden in case if there are multiple values for a day.
            var res = from p in dtInputRaw.AsEnumerable()
                      group p by DateTime.Parse(p[DATE].ToString()).ToShortDateString() into Result
                      select new
                      {
                          DateCount = Result.Count(),
                          Date = Result.Key

                      };

            //Get the Value for each day
            foreach (var r in res)
            {
                drArrayTemp = dtInputRaw.Select("" + DATE + ">= '" + r.Date + "' and " + DATE + " <'" + DateTime.Parse(r.Date).AddHours(24) + "'");

                //Find Mositure clay Value
                for (int i = 0; i < drArrayTemp.Length; i++)
                {
                    if (!string.IsNullOrEmpty(drArrayTemp[i][MOISTURESOIL_SAND].ToString()))
                    {
                        fMoistureSand = float.Parse(drArrayTemp[i][MOISTURESOIL_SAND].ToString());
                        break;
                    }
                }

                //Find Soil Temp
                for (int i = 0; i < drArrayTemp.Length; i++)
                {
                    if (!string.IsNullOrEmpty(drArrayTemp[i][TEMPMIN].ToString()))
                    {
                        fTempMin5 = float.Parse(drArrayTemp[i][TEMPMIN].ToString());
                        break;
                    }
                }

                //Update the Input Raw datatable
                for (int i = 0; i < drArrayTemp.Length; i++)
                {

                    drArrayTemp[i][MOISTURESOIL_SAND] = fMoistureSand;
                    drArrayTemp[i][TEMPMIN] = fTempMin5;
                }

                dtInputRaw.AcceptChanges();
            }

        }

        static private DataTable ApplyRules()
        {
            //Total rows from filtered data
            iRowsCount = dtInput.Rows.Count;

            //<!-- soil temp last 3 days too low - no plantings -->
            //<condition start="-72" end="-1" decimaterule="min" column="soiltemp" upper="14"/>
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                strCellValue = string.Empty;


                //Apply all the Rules
                CalcAllRules(i);

                //Create the Output data table
                GetOutputData(i);


            }

            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                //Hourfrom 0 to HourTo =6
                CalcResLegRain(i, i + 5);

                //-12 hours to +11 hours
                // -12 condtion is checked before updating output datatable
                CalcResLegFrost(i, i + 10);

                CalcResLegTooWetGround(i, i + 23);
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
        /// This Method Calculate the value for current cell based on the previous 72 hours of data
        /// </summary>
        /// <param name="startIndex"></param>
        static private void CalcAllRules(int startIndex)
        {
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[startIndex][DATE].ToString());
            double i = 0.0;
            var temp = from dt in dtInputRaw.AsEnumerable()
                       where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(6) &&
                       DateTime.Parse(dt[DATE].ToString()) >= dateLocal
                       select dt;

            

            /*<rule value="0" letter="M" adddescription="ResLegTooWetGround" hourfrom="0" hourto="24">
            <!-- soil moisture too high - no planting-->
            <condition decimaterule="avg" column="MoistureSoilSand010cm" interpolaterule="dailyto24" lower="60" />*/

            var result_F0_T24 = from dt in dtInputRaw.AsEnumerable()
                                where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(24) &&
                                DateTime.Parse(dt[DATE].ToString()) >= dateLocal
                                group dt by 1 into grp
                                select new
                                {
                                    //point to be noted

                                    //dtAvgMoistureSoilSand = grp.Average(t => double.TryParse(t[MOISTURESOIL_SAND].ToString(), out i) ? i : 0.0),
                                    dtAvgTemp = grp.Average(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0)
                                };

            
            #region Rules for ResLegTooColdSoil

            var temp0 = from dt in dtInputRaw.AsEnumerable()
                        where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-48) &&
                         DateTime.Parse(dt[DATE].ToString()) < dateLocal
                        select dt;

            //<!-- soil temp now too low - no plantings -->
            //    <condition start="-48" end="-1" decimaterule="avg" column="temp" upper="8"/>
            var result_FM48_TM1 = from dt in dtInputRaw.AsEnumerable()
                                  where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-48) &&
                                  DateTime.Parse(dt[DATE].ToString()) < dateLocal
                                  group dt by 1 into grp
                                  select new
                                  {
                                      //point to be noted
                                      dtAvgTemp = grp.Average(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0)
                                  };

            var tempM1 = from dt in dtInputRaw.AsEnumerable()
                         where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(24) &&
                         DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(0)
                         select dt;


            /************************/
            //<!-- soil temp next 24 hrs too low - no plantings -->
            //    <condition start="0" end="24" decimaterule="avg" column="temp" upper="0" />

            //ReferenceEquals TO CONDITION "fAvg_H24_MOISTURESOIL_SAND"
            /*************************/


            //<!-- soil temp next 48 hrs too low - no plantings -->
            //    <condition start="25" end="48" decimaterule="avg" column="temp" upper="8"/>
            var temp1 = from dt in dtInputRaw.AsEnumerable()
                        where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(48) &&
                        DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(24)
                        select dt;

            var result_F25_T48 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(48) &&
                                 DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(24)
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtAvgTemp = grp.Average(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0)
                                 };


            var temp2 = from dt in dtInputRaw.AsEnumerable()
                        where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(72) &&
                         DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(48)
                        select dt;

            //<!-- soil temp next 72hrs too low - no plantings -->
            //<condition start="49" end="72" decimaterule="avg" column="temp" upper="8"/>
            var result_F49_T72 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(72) &&
                                 DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(48)
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtAvgTemp = grp.Average(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0)
                                 };


            //Check all the Conditions

            //Fetch the avg of temp (hour -48 to -1)
            float fAvg_FM48_TM1_Temp = float.Parse(result_FM48_TM1.ToList()[0].dtAvgTemp.ToString());

            if (fAvg_FM48_TM1_Temp <= 0.0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegTooColdSoil;
                }

                //Set the Cell Value to "0" - High risk
                strCellValue = "0";
            }

            //Fetch the avg of temp (hour 0 to 24)
            float fAvg_F0_T24_Temp = float.Parse(result_F0_T24.ToList()[0].dtAvgTemp.ToString());

            if (fAvg_F0_T24_Temp <= 0.0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil24))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil24;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegTooColdSoil24;
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }

            //Fetch the avg of temp (hour 25 to 48)
            float fAvg_F25_T48_Temp = float.Parse(result_F25_T48.ToList()[0].dtAvgTemp.ToString());

            if (fAvg_F25_T48_Temp <= 0.0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil48))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil48;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegTooColdSoil48;
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }

            //Fetch the avg of temp (hour 49 to 72)
            float fAvg_F49_T72_Temp = float.Parse(result_F49_T72.ToList()[0].dtAvgTemp.ToString());

            if (fAvg_F49_T72_Temp <= 0.0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil72))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil72;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegTooColdSoil72;
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }
            #endregion

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
                    drOutput["value"] = RS_Plant_Sugarbeet_Light_Default;
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
        /// Takes the Restrcition Descripotion and Returns its associated Letter/Value
        /// </summary>
        /// <param name="strRest"></param>
        /// <returns></returns>
        static private string GetRestrictionLetter(string strRest)
        {
            string strLetter = string.Empty;

            if (strRest.Equals(strResLegTooColdSoil))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegTooColdSoil;
            }
            else if (strRest.Equals(strResLegFrost))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegFrost;
            }
            else if (strRest.Equals(strResLegRain))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegRain;
            }
            else if (strRest.Equals(strResLegTooWetGround))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegTooWetGround;
            }
            else if (strRest.Equals(strResLegTooColdSoil24))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegTooColdSoil24; ;
            }
            else if (strRest.Equals(strResLegTooColdSoil48))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegTooColdSoil48;
            }
            else if (strRest.Equals(strResLegTooColdSoil72))
            {
                strLetter = RS_Plant_Sugarbeet_Light_ResLegTooColdSoil72;
            }
            return strLetter;
        }

        /// <summary>
        /// This Method Calculate the value for current cell based on the next 6 hours of data
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        static private void CalcResLegRain(int startIndex, int endIndex)
        {

            double fSum = 0.0f;
            int startIndexTemp = startIndex;


            double i = 0.0;

            //Store the Start Index date
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[startIndexTemp][DATE].ToString());

            //Get the Current Hour Data
            fSum = double.TryParse(dtInput.Rows[startIndexTemp][PRECIP].ToString(), out i) ? i : 0;

            if (fSum > 0.0)
            {
                isResLegRain = true;
            }
            else
            {
                isResLegRain = false;
            }

            //Update the Output only if the resriction is applied
            if (isResLegRain)
            {
                strCellValue = "0";//High Risk
                UpdateOutputTable(startIndexTemp, startIndexTemp + 5, strResLegRain, RS_Plant_Sugarbeet_Light_ResLegRain, strCellValue);
                isResLegRain = false;
            }

            
            //clear the values
            fSum = 0.0;

        }

        /// <summary>
        /// This Method Calculate the value for current cell based on the next 6 hours of data
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        static private void CalcResLegFrost(int startIndex, int endIndex)
        {
            ///Assign the start index
            int startIndexTemp = startIndex;
            double fMin_FM12_T11 = 0.0;

            double i = 0.0;

            //Store the Start Index date
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[startIndexTemp][DATE].ToString());

            //Get the Current Hour Data
            fMin_FM12_T11 = double.TryParse(dtInput.Rows[startIndexTemp][TEMPMIN].ToString(), out i) ? i : 0;


            if (fMin_FM12_T11 <= 0.0)
            {
                isResLegFrost = true;
            }
            else
            {
                isResLegFrost = false;
            }



            //Update the Output only if the resriction is applied
            if (isResLegFrost)
            {
                strCellValue = "1";//Medium Risk

                //Set the start and end index for resLegFrost Rule -12 to +11
                if (startIndexTemp > 11)
                {
                    startIndexTemp = startIndexTemp - 12;
                }
                //hour -12(startIndex -12) to hour 11(startIndex + 10)
                UpdateOutputTable(startIndexTemp, startIndex + 10, strResLegFrost, RS_Plant_Sugarbeet_Light_ResLegFrost, strCellValue);
                isResLegFrost = false;
            }

            //clear the values
            fMin_FM12_T11 =float.MaxValue;
   

        }

        static private void CalcResLegTooWetGround(int startIndex, int endIndex)
        {

            double i = 0.0;

            double fAvg_H24_MoistureSoil_Sand = 0.0;

            //Carlculate the Sum of current Hour + 5 hours
            int startIndexTemp = startIndex;

            //Store the Start Index date
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[startIndex][DATE].ToString());

            //Get the Current Hour Data
            fAvg_H24_MoistureSoil_Sand = double.TryParse(dtInput.Rows[startIndexTemp][MOISTURESOIL_SAND].ToString(), out i) ? i : 0;



            if (fAvg_H24_MoistureSoil_Sand > 60.0)
            {
                isResLegTooWetGround = true;
            }
            else
            {
                isResLegTooWetGround = false;
            }

            if (isResLegTooWetGround)
            {
                strCellValue = "0";//High Risk
                UpdateOutputTable(startIndexTemp, startIndexTemp + 23, strResLegTooWetGround, RS_Plant_Sugarbeet_Light_ResLegTooWetGround, strCellValue);
                isResLegTooWetGround = false;
            }


            //Clear the Values in all the place holders
            fAvg_H24_MoistureSoil_Sand = 0.0f;

        }

        static private void UpdateOutputTable(int startIndex_p, int endIndex_p, string restName, string strValue_p, string strColorCode_p)
        {
            DataRow[] dr = null;
            string strColorCode = string.Empty;
            string strValue = string.Empty;
            string strFinalColor = "0";//Defaulted to high risk
            int iHour = 0;
            int strtTemp = startIndex_p;
            int index = 0;
            while (startIndex_p <= endIndex_p)
            {
                if (startIndex_p < iRowsCount)
                {
                    dr = dtOutput.Select("day='" + dtInput.Rows[startIndex_p][DATE].ToString() + "'");
                }
                //else
                //{
                //    //Fetch Last Allowed StartIndex data for all the index beyound the total rows.
                //    dr = dtOutput.Select("day='" + dtInput.Rows[strtTemp]["datelocal"].ToString() + "'");
                //}

                //Fetch the ColorCode
                strColorCode = dr[0]["ColorCode"].ToString();

                //Fetch the value
                strValue = dr[0]["value"].ToString();

                //Fetch the Hour
                iHour = int.Parse(dr[0]["hour"].ToString());

                //Set the Offset
                iOffSet = iHour + 1;

                //Check if the Colorcode is "0" and Value has a text
                if (!string.IsNullOrEmpty(strColorCode))
                {
                    //strValue = empty when there is no risk involved
                    if (startIndex_p < iRowsCount)
                    {
                        index = startIndex_p;
                    }
                    else
                    {
                        index = strtTemp;
                    }

                    //Begin Editing the row
                    dtOutput.Rows[index].BeginEdit();

                    //Check whether the current hour is in day/night
                    SetDayNight();

                    //If the colorcode passed is 1, then check if the existing color is not=0 before we update the output table
                    //since 00=0 and 01=1 .. high risk color codes
                    if (strColorCode.Equals("0") || strColorCode.Equals("1"))
                    {
                        strFinalColor = "0";//High risk
                    }
                    else
                    {
                        strFinalColor = strColorCode_p;
                    }

                    if (blIsNight)
                    {
                        //Set it to high risk
                        strFinalColor = strFinalColor + "0";
                    }
                    else
                    {
                        //Settting this to because "01" is read as 1 in color pallete
                        //"01" - "0" - high risk "1" - day
                        strFinalColor = strFinalColor + "1";
                    }

                    if (strFinalColor.Contains("00"))
                    {
                        strFinalColor = "0";
                    }
                    else if (strFinalColor.Contains("01"))
                    {
                        strFinalColor = "1";
                    }

                    //Set the Finale Color Code
                    dtOutput.Rows[index]["ColorCode"] = strFinalColor;

                    // if there is already a risk 
                    if (!string.IsNullOrEmpty(strValue))
                    {

                        //Check if this restriction already exists, if nmot append
                        if (!dtOutput.Rows[index]["restrictions"].ToString().Contains(restName))
                        {
                            dtOutput.Rows[index]["value"] = "X";
                            dtOutput.Rows[index]["restrictions"] = dtOutput.Rows[index]["restrictions"] + "," + restName;
                        }
                    }
                    else
                    {
                        //<!--precip now - cannot plant -->
                        //<condition decimaterule="sum" column="precip" lower="0" hourfrom="0" hourto="6"/>
                        dtOutput.Rows[index]["value"] = strValue_p;

                        //Update the Restriction incase the Value if empty
                        dtOutput.Rows[index]["restrictions"] = restName;
                    }


                    dtOutput.AcceptChanges();

                    //Reset the flag
                    blIsNight = false;

                }

                startIndex_p++;
            }

            strFinalColor = string.Empty;


        }


    }
}
