using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Syngenta.Agricast.Modals
{
    public class RS_Disease_Potato_Smith
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

        //Variables to hold start and end dates
        static DateTime startDate;
        static DateTime endDate;

        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //stores a flag to determine day/Night
        static bool blIsNight = false;

        //Indicates the range of data to be passed in the input datatable


        public static int StartHours
        {
            get { return -48; }

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
                RulesetSerieList.Rows.Add("temp");
                RulesetSerieList.Rows.Add("HumidityRel");
                return RulesetSerieList;
            }
        }

        //Variables to hold the calculated values
        static float fMin_FM24_T0_Temp = float.MinValue;
        static float fSum_FM24_T0_Humid90 = float.MinValue;
        static float fMin_FM48_TM24_Temp = float.MinValue;
        static float fSum_FM48_TM24_Humid90 = float.MinValue;


        enum RuleNames
        {
            ResLegTooColdSoil, ResLegRain, ResRainExpected

        }

        #endregion

        #region Constants
        private static string strResDisSmithMed1 = "ResDisSmithMed1";
        private static string strResDisSmithMed2 = "ResDisSmithMed2";
        private static string strResDisSmithMed3 = "ResDisSmithMed3";
        private static string strResDisSmithMed4 = "ResDisSmithMed4";
        private static string strResDisSmithHig = "ResDisSmithHig";


        //Column names
        private static string DATE = "date";
        private static string TEMP = "temp";
        private static string HUMID90 = "HumidityRel";

        //ColorCodes
        private const string RS_Disease_Potato_Smith_ResDisSmithMed1 = "B";
        private const string RS_Disease_Potato_Smith_ResDisSmithMed2 = "B";
        private const string RS_Disease_Potato_Smith_ResDisSmithMed3 = "B";
        private const string RS_Disease_Potato_Smith_ResDisSmithMed4 = "B";
        private const string RS_Disease_Potato_Smith_ResDisSmithHig = "X";
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

            //Only for Onsite Data
            TEMP = getColumnName(dtInput.Columns, "TempAir");
            HUMID90 = getColumnName(dtInput.Columns, "Humidity");
          
            dt = ApplyRules();


            return dt;
        }

       

        static private DataTable ApplyRules()
        {

            //Total rows from filtered data
            iRowsCount = dtInput.Rows.Count;

            //first generate the Output datatable
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                drOutput = dtOutput.NewRow();

                drOutput["day"] = dtInput.Rows[i][DATE].ToString();

                iCurrHour = DateTime.Parse(dtInput.Rows[i][DATE].ToString()).Hour;

                drOutput["hour"] = iCurrHour;

                iOffSet = iCurrHour + 1;

                SetDayNight();

                if (blIsNight)
                {
                    strFinalCellValue = strDefaultCellValue + "0";

                }
                else
                {
                    strFinalCellValue = strDefaultCellValue + "1";
                }

                drOutput["ColorCode"] = strFinalCellValue;

                dtOutput.Rows.Add(drOutput);

            }

            //<!--precip expected - cannot plant -->
            //<condition start="0" end="24" decimaterule="sum" column="precip" lower="20"/>
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {

                //extract all the necessary values 

                CalcRestrictions(i);

                //clear the values

                strCellValue = string.Empty;

                strRestrictionList = string.Empty;
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
        /// This Method Calculate the value for current cell based on the next 24 hours of data
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>

        static private void UpdateOutputTable(int startIndex_p, int endIndex_p, string restList)
        {
            DataRow[] dr = null;
            string strColorCode = string.Empty;
            string strValue = string.Empty;
            int iHour = 0;
            int strtTemp = startIndex_p;
            int index = 0;
            string[] strRest = null;

            while (startIndex_p <= endIndex_p)
            {
                if (startIndex_p < iRowsCount)
                {
                    dr = dtOutput.Select("day='" + dtInput.Rows[startIndex_p][DATE].ToString() + "'");
                }
                //else
                //{
                //    //Fetch Last Allowed StartIndex data for all the index beyound the total rows.
                //    dr = dtOutput.Select("day='" + dtInput.Rows[strtTemp][DATE].ToString() + "'");
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

                    //Check whether the current hour is iin day/night
                    SetDayNight();

                    if (!string.IsNullOrEmpty(strCellValue))
                    {
                        if (blIsNight)
                        {

                            //Set it to high risk
                            strFinalCellValue = strCellValue + "0";
                        }
                        else
                        {
                            //Settting this to because "01" is read as 1 in color pallete
                            //"01" - "0" - high risk "1" - day
                            strFinalCellValue = strCellValue + "1";
                        }
                    }
                    if (strFinalCellValue.Contains("00"))
                    {
                        strFinalCellValue = "0";
                    }
                    if (strFinalCellValue.Contains("01"))
                    {
                        strFinalCellValue = "1";
                    }

                    //Set the Cell Value
                    dtOutput.Rows[index]["ColorCode"] = strFinalCellValue;

                    if (restList.Equals(string.Empty))
                    {
                        strRest = null;
                    }
                    else
                    {
                        strRest = restList.Split(',');
                    }

                    // checl the restriction list for multiple restriction
                    if (strRest != null)
                    {
                        if (strRest.Length > 1)
                        {
                            dtOutput.Rows[index]["value"] = "Y";
                        }
                        else
                        {
                            dtOutput.Rows[index]["value"] = GetRestrictionLetter(strRest[0]);
                        }

                    }

                    //Set the restricitons
                    dtOutput.Rows[index]["restrictions"] = restList;

                    dtOutput.AcceptChanges();

                    //Reset the flag
                    blIsNight = false;

                    //Reset final color
                    strFinalCellValue = string.Empty;

                }

                startIndex_p++;
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

            if (strRest.Equals(strResDisSmithMed1))
            {
                strLetter = RS_Disease_Potato_Smith_ResDisSmithMed1;
            }
            else if (strRest.Equals(strResDisSmithMed2))
            {
                strLetter = RS_Disease_Potato_Smith_ResDisSmithMed2;
            }
            else if (strRest.Equals(strResDisSmithMed3))
            {
                strLetter = RS_Disease_Potato_Smith_ResDisSmithMed3;
            }
            else if (strRest.Equals(strResDisSmithMed4))
            {
                strLetter = RS_Disease_Potato_Smith_ResDisSmithMed4;
            }
            else if (strRest.Equals(strResDisSmithHig))
            {
                strLetter = RS_Disease_Potato_Smith_ResDisSmithHig;
            }

            return strLetter;
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
        /// All the calculation for which the rules are checked are done in this method
        /// </summary>
        /// <param name="index"></param>
        static void CalcRestrictions(int index)
        {
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[index][DATE].ToString());

            double i = 0.0;

            //Calculate values for Hour from -24 to hour 0

            var result_M24To0 = from dt in dtInputRaw.AsEnumerable()
                                where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-24) &&
                                DateTime.Parse(dt[DATE].ToString()) <= dateLocal
                                group dt by 1 into grp
                                select new
                                {
                                    //point to be noted
                                    dtMinTemp = grp.Min(t => double.TryParse(t[TEMP].ToString(), out i) ? i : 0.0),
                                    dtSumHumid90 = grp.Sum(t => double.TryParse(t[HUMID90].ToString(), out i) ? i : 0.0)
                                };

            //Calculate values for Hour from -48 to hour -24

            var result_M48ToM24 = from dt in dtInputRaw.AsEnumerable()
                                  where DateTime.Parse(dt[DATE].ToString()) >= dateLocal.AddHours(-48) &&
                                  DateTime.Parse(dt[DATE].ToString()) <= dateLocal.AddHours(-24)
                                  group dt by 1 into grp
                                  select new
                                  {
                                      //point to be noted
                                      dtMinTemp = grp.Min(t => double.TryParse(t[TEMP].ToString(), out i) ? i : 0.0),
                                      dtSumHumid90 = grp.Sum(t => double.TryParse(t[HUMID90].ToString(), out i) ? i : 0.0)
                                  };

            //Assign the values to all the variable
            fMin_FM24_T0_Temp = float.Parse(result_M24To0.ToList()[0].dtMinTemp.ToString());
            fSum_FM24_T0_Humid90 = float.Parse(result_M24To0.ToList()[0].dtSumHumid90.ToString());
            fMin_FM48_TM24_Temp = float.Parse(result_M48ToM24.ToList()[0].dtMinTemp.ToString());
            fSum_FM48_TM24_Humid90 = float.Parse(result_M48ToM24.ToList()[0].dtSumHumid90.ToString());


            #region Restrictions

            //<rule value="1" letter="B" adddescription="ResDisSmithMed1" hourfrom="1" hourto="24">
            //    <!--  MinTemp0h-24h > 10C  (day -1 )-->
            //    <condition start="-24" end="0" decimaterule="min" column="temp" lower="10"/>
            //    <!--  hRH900h-24h =10  (day -1 )-->
            //    <condition start="-24" end="0" decimaterule="sum" column="humid90" lower="600" upper="660"/>
            //</rule>

            if (fMin_FM24_T0_Temp > 10.0 ||
                (fSum_FM24_T0_Humid90 > 600.0 && fSum_FM24_T0_Humid90 <= 660.0))
            {
                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResDisSmithMed1))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResDisSmithMed1;
                    else
                        strRestrictionList = strRestrictionList + "," + strResDisSmithMed1;
                }
                SetCellValue();
            }


            //<rule value="1" letter="B" adddescription="ResDisSmithMed2" hourfrom="1" hourto="24">
            //    <!--  MinTemp24h-72h > 10C  (day -2 )-->
            //    <condition  start="-48" end="-24" decimaterule="min" column="temp" lower="10"/>
            //    <!--  hRH9024h-72h =10  (day -2 )-->
            //    <condition  start="-48" end="-24" decimaterule="sum" column="humid90" lower="600" upper="660"/>
            //</rule>
            if (fMin_FM48_TM24_Temp > 10.0 ||
             (fSum_FM48_TM24_Humid90 > 600.0 && fSum_FM48_TM24_Humid90 <= 660.0))
            {
                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResDisSmithMed2))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResDisSmithMed2;
                    else
                        strRestrictionList = strRestrictionList + "," + strResDisSmithMed2;
                }
                SetCellValue();
            }

            //<!--MEDIUM0-24-->
            //<rule value="1" letter="B" adddescription="ResDisSmithMed3" hourfrom="1" hourto="24">
            //    <!--  MinTemp0h-24h > 10C  (day -2 )-->
            //    <condition start="-48" end="-24" decimaterule="min" column="temp" lower="10"/>
            //    <!--  hRH900h-24h =10  (day -1 )-->
            //    <condition start="-24" end="0" decimaterule="sum" column="humid90" lower="600" upper="660"/>
            //</rule>
            if (fMin_FM48_TM24_Temp > 10.0 ||
           (fSum_FM24_T0_Humid90 > 600.0 && fSum_FM24_T0_Humid90 <= 660.0))
            {
                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResDisSmithMed3))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResDisSmithMed3;
                    else
                        strRestrictionList = strRestrictionList + "," + strResDisSmithMed3;
                }
                SetCellValue();
            }

            //<rule value="1" letter="B" adddescription="ResDisSmithMed4" hourfrom="1" hourto="24">
            //    <!--  MinTemp24h-72h > 10C  (day -1 )-->
            //    <condition  start="-24" end="0" decimaterule="min" column="temp" lower="10"/>
            //    <!--  hRH9024h-72h =10  (day -2 )-->
            //    <condition  start="-48" end="-24" decimaterule="sum" column="humid90" lower="600" upper="658"/>
            //</rule>
            if (fMin_FM24_T0_Temp > 10.0 ||
        (fSum_FM48_TM24_Humid90 > 600.0 && fSum_FM48_TM24_Humid90 <= 658.0))
            {
                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResDisSmithMed4))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResDisSmithMed4;
                    else
                        strRestrictionList = strRestrictionList + "," + strResDisSmithMed4;
                }
                SetCellValue();
            }



            /**************?HIGH RISK*************/
            //<rule value="0" letter="X" adddescription="ResDisSmithHig" hourfrom="1" hourto="24">
            //<!--  MinTemp0h-24h > 10C  (day -1 )-->
            //<condition start="-24" end="0" decimaterule="min" column="temp" lower="10"/>
            //<!--  MinTemp24h-72h > 10C  (day -2 )-->
            //<condition  start="-48" end="-24" decimaterule="min" column="temp" lower="10"/>
            //<!--  hRH900h-24h =10  (day -1 )-->
            //<condition start="-24" end="0" decimaterule="sum" column="humid90" lower="659"/>
            //<!--  hRH9024h-72h =10  (day -2 )-->
            //<condition  start="-48" end="-24" decimaterule="sum" column="humid90" lower="659"/>

            if (fMin_FM24_T0_Temp > 10.0 ||
                fMin_FM48_TM24_Temp > 10.0 ||
                fSum_FM24_T0_Humid90 > 659 ||
                fSum_FM48_TM24_Humid90 > 659)
            {
                //add the restrcition to the restriciton list
                if (!CheckRestrictioninList(strResDisSmithHig))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResDisSmithHig;
                    else
                        strRestrictionList = strRestrictionList + "," + strResDisSmithHig;
                }
                //Set high Risk
                strCellValue = "0";
            }


            //Update the Output datatable
            // hour from 1 to 24
            UpdateOutputTable(index, index + 23, strRestrictionList);

            #endregion
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


    }
}
