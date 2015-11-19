using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Syngenta.Agricast.Modals
{
    public class RS_RunOff
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

        //Flag to check if Aggreate Rule is applied
        static bool isAggregate = false;

        //Holds Current Cell Value
        static string strCurrValue = string.Empty;

        //stored Cell Color
        static string strCellColor = string.Empty;

        static string strRestriction = string.Empty;
        static string strRestrictionList = string.Empty;

        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;

        static DateTime startDate;
        static DateTime endDate;

        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //stores a flag to determine day/Night
        static bool blIsNight = false;
        static int iOffSet = 0;

        //Set Default CellValue
        static string strDefaultCellValue = "2";

        static bool isApplied = false;
        enum RuleNames
        {
            RunOffRiskHi, RunOffRiskMed

        }

        //Indicates the range of data to be passed in the input datatable
        

        public static int StartHours
        {
            get { return 0; }
            
        }

        //endHours + 1 : ruleset input data change in DB 
        public static int EndHours
        {
            get { return 25; }
            
        }
        public static DataTable RulesetSeriesList
        {
            get
            {
                DataTable RulesetSerieList = new DataTable();
                RulesetSerieList.Columns.Add("Name");

                RulesetSerieList.Rows.Add("PrecipQuantity_hSum");
                return RulesetSerieList;
            }
        }
        #endregion

        #region Constants
        public const string RS_RunOff_Red = "#FF0000";
        public const string RS_RunOff_Amber = "#FFA500";
        public const string RS_RunOff_White = "#FFFFFF";

        private static string PRECIP = "PrecipQuantity_hSum";
        private static string DATE = "date";

        public const string RS_RunOff_High = "0";
        public const string RS_RunOff_Medium = "1";
        public const string RS_RunOff_Default = "2";

        #endregion
        static private void IntialiseOutputDatatable()
        {
            //Create output datatable
            dtOutput = new DataTable();
            dtOutput.Columns.Add("day");
            dtOutput.Columns.Add("hour");
            dtOutput.Columns.Add("colour");
            dtOutput.Columns.Add("CurrentValue");


            // CallApplyRules();
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

            //Assign the Value through Interface Members
            dtInputRaw = objIRuleSets.DtInput;
            sunRise = objIRuleSets.SunRise;
            sunSet = objIRuleSets.SunSet;
            startDate = objIRuleSets.StartDate;
            endDate = objIRuleSets.EndDate;
            dtOutput = objIRuleSets.DtOutput;


            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();


            PRECIP = getColumnName(dtInput.Columns, "PrecipAmount");

            
            DataTable dt = ApplyRules();

            return dt;
        }


        static private DataTable ApplyRules()
        {
            DataTable dt = null;

            iRowsCount = dtInput.Rows.Count;

            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                //Calculate first two rules
                CalcAllRules(i);

                //Create Output table
                GetOutputData(i);
            }

            //Run the Aggregation rule
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                CalcAggregateRules(i, i + 23);
            }

            return dtOutput;
        }


        static private void CalcAllRules(int index)
        {

            double i = 0.0;
            double fValuePrecip = 0.0;

            //Fetch the value for current hour
            fValuePrecip = double.TryParse(dtInput.Rows[index][PRECIP].ToString(), out i) ? i : 0.0;


            //<rule adddescription="RunOffRiskHi" value="0">
            //    <condition column="precip" lower="8" />
            //</rule>

            if (fValuePrecip > 5 && fValuePrecip <= 8)
            {
                if (!CheckRestrictioninList(RuleNames.RunOffRiskMed.ToString()))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = RuleNames.RunOffRiskMed.ToString();
                    else
                        strRestrictionList = strRestrictionList + "," + RuleNames.RunOffRiskMed.ToString();
                }

                //Set the Cell Value to "0" - High risk
                strCellValue = RS_RunOff_Medium;
            }

            //<rule adddescription="RunOffRiskMed" value="1">
            //    <condition column="precip" lower="5" upper="8" />
            //</rule>
            if (fValuePrecip > 8)
            {
                if (!CheckRestrictioninList(RuleNames.RunOffRiskHi.ToString()))
                {

                    if (strRestrictionList.Length == 0)
                        strRestrictionList = RuleNames.RunOffRiskHi.ToString();
                    else
                        strRestrictionList = strRestrictionList + "," + RuleNames.RunOffRiskHi.ToString();
                }

                //Set the Cell Value to "0" - High risk
                strCellValue = RS_RunOff_High; ;
            }



        }


        static private void GetOutputData(int index)
        {
            //Add the Data to Output Table

            drOutput = dtOutput.NewRow();

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
                strCellValue = RS_RunOff_Default; ;//set it to Default - No risk
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

            //Add a Dummy Letter
            //drOutput["value"] = "D";

            //Resrtictions
           // drOutput["restrictions"] = strRestrictionList;

            //Add the Row to Output Datatable
            dtOutput.Rows.Add(drOutput);

            //Set oto null after adding the row
            drOutput = null;

            //Clear the Restirction List
            //strRestrictionList = string.Empty;

            //clear the cell Value
            strCellValue = string.Empty;
        }
        /// <summary>
        /// Setss the Value to Medium Risk Factor(value=1)
        /// </summary>
        static private void SetCellValue()
        {
            if (!strCellValue.Equals("0"))//Not equal to 0
            {
                strCellValue = RS_RunOff_Medium; ;

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


        static private void CalcAggregateRules(int startIndex, int endIndex)
        {

            double i = 0.0;

            double fAvg_H24_Precip = 0.0;

            //Carlculate the Sum of current Hour + 5 hours
            int startIndexTemp = startIndex;

            //Store the Start Index date
            DateTime dateLocal = DateTime.Parse(dtInput.Rows[startIndex][DATE].ToString());

            //Calculate the value of Precip for start 0 to 24
            var result_F0_T24 = from dt in dtInputRaw.AsEnumerable()
                                where DateTime.Parse(dt[DATE].ToString()) < dateLocal.AddHours(24) &&
                                DateTime.Parse(dt[DATE].ToString()) >= dateLocal
                                group dt by 1 into grp
                                select new
                                {
                                    //point to be noted

                                    // dtAvgMoistureSoilClay = grp.Average(t => double.TryParse(t[MOISTURESOIL_CLAY].ToString(), out i) ? i : 0.0),
                                    dtSumPrecip = grp.Average(t => double.TryParse(t[PRECIP].ToString(), out i) ? i : 0.0)
                                };
            fAvg_H24_Precip = double.TryParse(dtInput.Rows[startIndexTemp][PRECIP].ToString(), out i) ? i : 0;


            //Check for Medium Risk

            if (fAvg_H24_Precip > 40.0 && fAvg_H24_Precip <= 50)
            {
                strRestrictionList = RuleNames.RunOffRiskMed.ToString();
                strCellValue = "1";
                isApplied = true;
            }

            //Check for high risk 
            if (fAvg_H24_Precip > 50)
            {
                strRestrictionList = RuleNames.RunOffRiskHi.ToString();
                strCellValue = "0";
                isApplied = true;

            }

            if (isApplied)
            {
                //The Letter paramenter is null becoz no letter/value specified for this ruleset
                UpdateOutputTable(startIndexTemp, startIndexTemp + 23, strRestrictionList, null, strCellValue);
                isApplied = false;
            }

            //Clear the Values in all the place holders
            fAvg_H24_Precip = 0.0f;


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
                  
                    //Begin Editing the row
                    dtOutput.Rows[index].BeginEdit();

                    //Check whether the current hour is in day/night
                    SetDayNight();

                    //If the colorcode passed is 1, then check if the existing color is not=0 before we update the output table
                    //since 00=0 and 01=1 .. high risk color codes
                    if (strColorCode.Contains(RS_RunOff_Default))
                    {
                        strFinalColor = strColorCode_p;
                    }
                    else if  (strColorCode.Equals("0") || strColorCode.Equals("1"))
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

                  
                    //dtOutput.Rows[index]["value"] = strValue_p;
                    dtOutput.AcceptChanges();

                    //Reset the flag
                    blIsNight = false;

                    //Clear Cell Value
                    strCellValue = string.Empty;
                }

                startIndex_p++;
            }

            strFinalColor = string.Empty;


        }
    }
}
