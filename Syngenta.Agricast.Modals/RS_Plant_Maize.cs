using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Syngenta.Agricast.Modals
{
    public class RS_Plant_Maize
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

        enum RuleNames
        {
            ResLegTooColdSoil, ResLegRain, ResRainExpected

        }

        //Indicates the range of data to be passed in the input datatable
        

        public static int StartHours
        {
            get { return 0; }
            
        }

        //endHours + 1 : ruleset input data change in DB 
        public static int EndHours
        {
            get { return 121; }
            
        }
        public static DataTable RulesetSeriesList
        {
            get
            {
                DataTable RulesetSerieList = new DataTable();
                RulesetSerieList.Columns.Add("Name");

                RulesetSerieList.Rows.Add("PrecipQuantity_hSum");
                RulesetSerieList.Rows.Add("soiltemp");
                return RulesetSerieList;
            }
        }
        #endregion

        #region Constants


        private const string RS_Plant_Maize_ResLegTooColdSoil = "A";
        private const string RS_Plant_Maize_ResLegRain = "R";
        private const string RS_Plant_Maize_ResRainExpected = "S";
        private const string RS_Plant_Maize_Default = "X";

        private static string PRECIP = "PrecipQuantity_hSum";
        private static string SOILTEMP = "soiltemp";
        private static string DATE = "date";

        //Constants to hold Restriction Desciption
        private static string strResLegTooColdSoil = "ResLegTooColdSoil";
        private static string strResLegRain = "ResLegRain";
        private static string strResRainExpected = "ResRainExpected";


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
                //Apply all the Rules
                CalcAllRules(i);

                //Create the Output data table
                GetOutputData(i);

            }


            //<!--precip now - cannot plant -->
            //<condition decimaterule="sum" column="precip" lower="0"/>
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                CalcResLegRain(i, i + 5);

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
                       where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(120) &&
                       DateTime.Parse(dt[DATE].ToString()) >= dateLocal
                       select dt;

            var result = from dt in dtInputRaw.AsEnumerable()
                         where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(120) &&
                         DateTime.Parse(dt[DATE].ToString()) >= dateLocal
                         group dt by 1 into grp
                         select new
                         {
                             //point to be noted
                             dtMin = grp.Min(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0),
                             dtMax = grp.Max(t => double.TryParse(t[SOILTEMP].ToString(), out i) ? i : 0.0)
                         };
            float fMin = float.Parse(result.ToList()[0].dtMin.ToString());
            float fMax = float.Parse(result.ToList()[0].dtMax.ToString());

            //Apply This is Restriction only if the min value i less than 14
            //<!-- soil temp next 5 days too low - no plantings -->
            //    <condition start="0" end="120" decimaterule="min" column="soiltemp" upper="12"/>
            //and
            //<!-- soil temp next 5 days not high enough - no plantings -->
            //	<condition start="0" end="120" decimaterule="max" column="soiltemp" upper="12"/>

            if (fMin <= 12.0 && fMax <= 12.0)
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

            //    <!-- soil temp next 5 days not high enough - no plantings -->
            //<condition start="0" end="120" decimaterule="max" column="soiltemp" upper="14"/>
            if (fMax <= 14.0)
            {
                if (!CheckRestrictioninList(strResLegTooColdSoil))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResLegTooColdSoil;
                    else
                        strRestrictionList = strRestrictionList + "," + strResLegTooColdSoil;
                }

                //Set the Cell Value to "1" - Medium risk
                SetCellValue();
            }



            /*<rule value="0" letter="S" adddescription="ResRainExpected">
                <!--precip expected - cannot plant -->
                *<condition start="0" end="24" decimaterule="sum" column="precip" lower="20"/>*/

            var resHours_24 = from dt in dtInputRaw.AsEnumerable()
                              where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(24) &&
                              DateTime.Parse(dt[DATE].ToString()) >= dateLocal
                              group dt by 1 into grp
                              select new
                              {
                                  //point to be noted
                                  dtSumPrecip = grp.Sum(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0)

                              };

            float fSum_H24 = float.Parse(resHours_24.ToList()[0].dtSumPrecip.ToString());//Fetch the sum of data (hour 0 to 24)

            if (fSum_H24 > 20.0)
            {
                if (!CheckRestrictioninList(strResRainExpected))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = strResRainExpected;
                    else
                        strRestrictionList = strRestrictionList + "," + strResRainExpected;
                }

                //Set the Cell Value to "0" - High risk
                strCellValue = "0";
            }

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
                UpdateOutputTable(startIndexTemp, startIndexTemp + 5);
                isResLegRain = false;
            }


            fSum = float.MinValue;

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
                    drOutput["value"] = RS_Plant_Maize_Default;
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


        static private void UpdateOutputTable(int startIndex_p, int endIndex_p)
        {
            DataRow[] dr = null;
            string strColorCode = string.Empty;
            string strValue = string.Empty;
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

                    if (blIsNight)
                    {
                        //Set it to high risk
                        dtOutput.Rows[index]["ColorCode"] = "0";
                    }
                    else
                    {
                        //Settting this to because "01" is read as 1 in color pallete
                        //"01" - "0" - high risk "1" - day
                        dtOutput.Rows[index]["ColorCode"] = "1";
                    }

                    // if there is already a risk 
                    if (!string.IsNullOrEmpty(strValue))
                    {

                        //Check if this restriction already exists, if nmot append
                        if (!dtOutput.Rows[index]["restrictions"].ToString().Contains(RuleNames.ResLegRain.ToString()))
                        {
                            dtOutput.Rows[index]["value"] = "X";
                            dtOutput.Rows[index]["restrictions"] = dtOutput.Rows[index]["restrictions"] + "," + RuleNames.ResLegRain;
                        }
                    }
                    else
                    {
                        //<!--precip now - cannot plant -->
                        //<condition decimaterule="sum" column="precip" lower="0" hourfrom="0" hourto="6"/>
                        dtOutput.Rows[index]["value"] = RS_Plant_Maize_ResLegRain;

                        //Update the Restriction incase the Value if empty
                        dtOutput.Rows[index]["restrictions"] = RuleNames.ResLegRain.ToString();
                    }


                    dtOutput.AcceptChanges();

                    //Reset the flag
                    blIsNight = false;

                }

                startIndex_p++;
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
                strLetter = RS_Plant_Maize_ResLegTooColdSoil;
            }
            else if (strRest.Equals(strResLegRain))
            {
                strLetter = RS_Plant_Maize_ResLegRain;
            }
            else if (strRest.Equals(strResRainExpected))
            {
                strLetter = RS_Plant_Maize_ResRainExpected;
            }

            return strLetter;
        }
    }
}
