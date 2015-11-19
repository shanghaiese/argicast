using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Syngenta.Agricast.Modals
{
    public class RS_Harvest_SugarBeet_Heavy
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
            get { return -120; }

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
                RulesetSerieList.Rows.Add("MoistureSoilClay030cm");
                return RulesetSerieList;
            }
        }
        #endregion

        #region Constants


        private const string RS_Harvest_Heavy_SugarBeet_ResLegTooWetSoilPrecip48 = "M";
        private const string RS_Harvest_Heavy_SugarBeet_ResLegTooDrySoil120 = "M";
        private const string RS_Harvest_Heavy_SugarBeet_ResLegRain24 = "R";
        private const string RS_Harvest_Heavy_SugarBeet_ResLegTooColdSoil = "C";
        private const string RS_Harvest_Heavy_SugarBeet_ResLegTooColdSoil48 = "C";
        private const string RS_Harvest_Heavy_SugarBeet_Default = "X";

        private static string PRECIP = "PrecipQuantity_hSum";
        private static string SOILTEMP = "soiltemp";
        private static string MOISTURESOIL_CLAY = "MoistureSoilClay030cm";
        private static string DATE = "date";

        //Constants to hold Restriction Desciption
        private static string strResLegTooWetSoilPrecip48 = "ResLegTooWetSoilPrecip48";
        private static string strResLegTooDrySoil120 = "ResLegTooDrySoil-120";
        private static string strResLegRain24 = "ResLegRain-24";
        private static string strResLegTooColdSoil = "ResLegTooColdSoil";
        private static string strResLegTooColdSoil48 = "ResLegTooColdSoil-48";

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

            //Before Filtering the RawData, Interpolate the input Values
            InterPolateValues();

            //Filter the Raw Data and extract only the data between start and end dates
            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();

            //Check if all the required data is present
            PRECIP = getColumnName(dtInput.Columns, "PrecipAmount");
            SOILTEMP = getColumnName(dtInput.Columns, "soilTemp");
            MOISTURESOIL_CLAY = getColumnName(dtInput.Columns, "MoistureSoilClay030cm");


            dt = ApplyRules();

            return dt;
        }

        /// <summary>
        /// this Method is used to interpolate the Specified Input Variables
        /// </summary>
        static private void InterPolateValues()
        {

            DataRow[] drArrayTemp = null;
            float fMoistureClay = 0.0f;
            float fSoilTemp = 0.0f;

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
                    if (!string.IsNullOrEmpty(drArrayTemp[i][MOISTURESOIL_CLAY].ToString()))
                    {
                        fMoistureClay = float.Parse(drArrayTemp[i][MOISTURESOIL_CLAY].ToString());
                        break;
                    }
                }

                //Find Soil Temp
                for (int i = 0; i < drArrayTemp.Length; i++)
                {
                    if (!string.IsNullOrEmpty(drArrayTemp[i][SOILTEMP].ToString()))
                    {
                        fSoilTemp = float.Parse(drArrayTemp[i][SOILTEMP].ToString());
                        break;
                    }
                }

                //Update the Input Raw datatable
                for (int i = 0; i < drArrayTemp.Length; i++)
                {

                    drArrayTemp[i][MOISTURESOIL_CLAY] = fMoistureClay;
                    drArrayTemp[i][SOILTEMP] = fSoilTemp;
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
            double i = 0.0;
            //Fetch MoistureSoilClay030cm and soiltemp values for the current hour
            double fCurrHour_SoilClay = double.Parse(dtInput.Rows[startIndex][MOISTURESOIL_CLAY].ToString());
            double fCurrHour_SoilTemp = double.TryParse(dtInput.Rows[startIndex][SOILTEMP].ToString(), out i) ? i : 0;


            //check for Condition
            //<condition start="-48" end="0" decimaterule="sum" column="precip" lower="5" />

            //<condition start="-48" end="0" decimaterule="sum" column="precip" lower="5" />
            var result_FM48_T0 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-48) &&
                                 DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtSumPrecip = grp.Sum(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0)
                                 };

            //<condition start="-120" end="0" decimaterule="max" column="MoistureSoilClay030cm" interpolaterule="dailyto24"
            //           upper="1" />

            var result_FM120_T0 = from dt in dtInputRaw.AsEnumerable()
                                  where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-120) &&
                                  DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                  group dt by 1 into grp
                                  select new
                                  {
                                      //point to be noted
                                      dtMaxSoilClay = grp.Max(t => double.TryParse(t[MOISTURESOIL_CLAY].ToString(), out i) ? i : 0.0)
                                  };


            //<condition start="-24" end="0" decimaterule="sum" column="precip" lower="2" upper="5" />
            var result_FM24_T0 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-24) &&
                                 DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtSumPrecip = grp.Sum(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0),

                                 };



            //<condition start="-48" end="0" decimaterule="sum" column="precip" lower="5" />
            var result_FM48_T1 = from dt in dtInputRaw.AsEnumerable()
                                 where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-48) &&
                                 DateTime.Parse(dt[DATE].ToString()) < dateLocal
                                 group dt by 1 into grp
                                 select new
                                 {
                                     //point to be noted
                                     dtMinSoilTemp = grp.Min(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0)
                                 };



            float fSum_H48_T0_Precip = float.Parse(result_FM48_T0.ToList()[0].dtSumPrecip.ToString());//Fetch the sum of data (hour -96 to 0)
            float fMax_FM120_T0_SoilClay = float.Parse(result_FM120_T0.ToList()[0].dtMaxSoilClay.ToString());//Precipitation:-(hour -48 to 0)
            float fSum_FM24_T0_Precip = float.Parse(result_FM24_T0.ToList()[0].dtSumPrecip.ToString());//Temparature:-(hour -48 to 0)
            float fMin_FM48_T1_SoilTemp = float.Parse(result_FM48_T1.ToList()[0].dtMinSoilTemp.ToString());//Temparature:-(hour -6 to 0)



            //<rule value="0" letter="M" adddescription="ResLegTooWetSoilPrecip48">
            //     <!-- soil moisture too high - no planting-->
            //     <condition decimaterule="avg" column="MoistureSoilClay030cm" interpolaterule="dailyto24" lower="99" />
            //     <condition start="-48" end="0" decimaterule="sum" column="precip" lower="5" />


            if (fCurrHour_SoilClay > 99 || fSum_H48_T0_Precip > 5.0)
            {

                if (!CheckRestrictioninList(strResLegTooWetSoilPrecip48))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooWetSoilPrecip48 + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooWetSoilPrecip48 + ",";
                }

                //Set the Cell Value to "0" - high risk
                strCellValue = "0";
            }

            //<rule value="1" letter="M" adddescription="ResLegTooWetSoilPrecip48">
            //    <!-- soil moisture too high - no planting-->
            //    <condition decimaterule="avg" column="MoistureSoilClay030cm" interpolaterule="dailyto24" lower="99" />
            //    <condition start="-48" end="0" decimaterule="sum" column="precip" lower="1" upper="5" />
            //</rule>
            if (fCurrHour_SoilClay > 99 ||
                (fSum_H48_T0_Precip > 1 && fSum_H48_T0_Precip <= 5))
            {

                if (!CheckRestrictioninList(strResLegTooWetSoilPrecip48))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooWetSoilPrecip48 + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooWetSoilPrecip48 + ",";
                }
                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }


            //<rule value="1" letter="M" adddescription="ResLegTooDrySoil-120">
            //    <!-- soil moisture too low - no planting-->
            //    <condition start="-120" end="0" decimaterule="max" column="MoistureSoilClay030cm" interpolaterule="dailyto24"
            //        upper="1" />
            //</rule>
            if (fMax_FM120_T0_SoilClay <= 1)
            {
                if (!CheckRestrictioninList(strResLegTooDrySoil120))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooDrySoil120 + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooDrySoil120 + ",";
                }
                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }

            //<rule value="0" letter="R" adddescription="ResLegRain-24">
            //    <!--precip expected - cannot plant -->
            //    <condition start="-24" end="0" decimaterule="sum" column="precip" lower="5" />
            //</rule>
            if (fSum_FM24_T0_Precip > 5)
            {
                if (!CheckRestrictioninList(strResLegRain24))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegRain24 + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegRain24 + ",";
                }
                //Set the Cell Value to "0" - High risk
                strCellValue = "0";
            }

            //<rule value="1" letter="R" adddescription="ResLegRain-24">
            //    <!--precip expected - cannot plant -->
            //    <condition start="-24" end="0" decimaterule="sum" column="precip" lower="2" upper="5" />
            //</rule>
            if (fSum_FM24_T0_Precip > 2 && fSum_FM24_T0_Precip <= 5)
            {
                if (!CheckRestrictioninList(strResLegRain24))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegRain24 + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegRain24 + ",";
                }
                //Set the Cell Value to "0" - High risk
                SetCellValue();
            }

            //<rule value="0" letter="C" adddescription="ResLegTooColdSoil">
            //    <!-- soil  - no plantings -->
            //    <condition decimaterule="avg" column="soiltemp" upper="0" interpolaterule="dailyto24" />
            //</rule>
            if (fCurrHour_SoilTemp <= 0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooColdSoil + ",";
                }

                //set high Risk
                strCellValue = "0";
            }

            //<rule value="1" letter="C" adddescription="ResLegTooColdSoil-48">
            //    <!-- soil  - no plantings -->
            //    <condition start="-48" end="-1" decimaterule="min" column="soiltemp" upper="0" interpolaterule="dailyto24" />
            //</rule>
            if (fMin_FM48_T1_SoilTemp <= 0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil48))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil48 + ",";
                    else
                        strRestrictionList = strRestrictionList + strResLegTooColdSoil48 + ",";
                }

                //set high Risk
                SetCellValue();
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
                int ind = strRestrictionList.LastIndexOf(',');
                strRestrictionList = strRestrictionList.Remove(ind);

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
                    drOutput["value"] = RS_Harvest_Heavy_SugarBeet_Default;
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

            if (strRest.Equals(strResLegRain24))
            {
                strLetter = RS_Harvest_Heavy_SugarBeet_ResLegRain24;
            }
            else if (strRest.Equals(strResLegTooColdSoil))
            {
                strLetter = RS_Harvest_Heavy_SugarBeet_ResLegTooColdSoil;
            }
            else if (strRest.Equals(strResLegTooColdSoil48))
            {
                strLetter = RS_Harvest_Heavy_SugarBeet_ResLegTooColdSoil48;
            }
            else if (strRest.Equals(strResLegTooDrySoil120))
            {
                strLetter = RS_Harvest_Heavy_SugarBeet_ResLegTooDrySoil120;
            }
            else if (strRest.Equals(strResLegTooWetSoilPrecip48))
            {
                strLetter = RS_Harvest_Heavy_SugarBeet_ResLegTooWetSoilPrecip48;
            }


            return strLetter;
        }
    }
}
