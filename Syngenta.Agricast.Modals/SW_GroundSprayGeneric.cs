 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using Syngenta.AgriCast.Common.Service; 

namespace Syngenta.Agricast.Modals
{
    public class SW_GroundSprayGeneric
    {
        #region Variables and Declarations

        //Input and Output datatable
        static DataTable dtInput = null;
        static DataTable dtOutput = null;
        static DataRow drInput = null;

        //Rows to hold hourly data
        static DataRow drHour_M1 = null;
        static DataRow drHour_M2 = null;
        static DataRow drHour_P1 = null;
        static DataRow drHour_P2 = null;
        static DataRow drHour_P3 = null;
        static DataRow drHour0 = null;

        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //Variables to Hold the Hour data specific to each input type
        static string fHour_M2 = string.Empty;
        static string fHour_M1 = string.Empty;
        static string fHour0 = string.Empty;
        static string fHour_P1 = string.Empty;
        static string fHour_P2 = string.Empty;
        static string fHour_P3 = string.Empty;

        //Stores the Input data count
        static int iRowsCount = 0;
        static int iCurrHour = 0;
        static int iOffSet = 0;
        static bool isRed = false;
        static bool blIsNight = false;


        static string strRestrictionList = string.Empty;
        static string strCellColor = string.Empty;
        static string strFinalCellColor = string.Empty;
        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;


        //Set Default CellValue
        static string strDefaultCellValue = "2";

        //Variables to hold Strat and End date/Hours
        static DateTime startDate;
        static DateTime endDate;

        static DataRow[] drColl = null;
        static DateTime dateLocal;

        static DataTable dtInputRaw = null;

        //Indicates the range of data to be passed in the input datatable
        public static int StartHours
        {
            get { return -2; }

        }

        //endHours + 1 : ruleset input data change in DB 
        public static int EndHours
        {
            get { return 4; }
        }

        public static DataTable RulesetSeriesList
        {
            get
            {
                DataTable RulesetSerieList = new DataTable();
                RulesetSerieList.Columns.Add("Name");

                RulesetSerieList.Rows.Add("PrecipAmount_mm");
                RulesetSerieList.Rows.Add("RelHumidity");
                RulesetSerieList.Rows.Add("WindSpeedMax_ms");
                RulesetSerieList.Rows.Add("WindSpeed_ms");
                RulesetSerieList.Rows.Add("TempAir_C");
                return RulesetSerieList;
            }
        }
      

        /// <summary>
        /// Stores the List of Restriction to be checked 
        /// </summary>
        enum RuleNames
        {
            A1, A2, A3, A4, A5, B2, B3, C1, C2, C3, C4

        }

        #endregion

        #region Constants : - SW_GroundSprayGeneric

        //Constants to Letter for Rules in the RuleSet 
        private const string SW_GroundSprayGeneric_A5 = "R";
        private const string SW_GroundSprayGeneric_A4 = "A";
        private const string SW_GroundSprayGeneric_A3 = "A";
        private const string SW_GroundSprayGeneric_C4 = "D";
        private const string SW_GroundSprayGeneric_A2 = "F";
        private const string SW_GroundSprayGeneric_A1 = "F";
        private const string SW_GroundSprayGeneric_B2 = "U";
        private const string SW_GroundSprayGeneric_B3 = "V";
        private const string SW_GroundSprayGeneric_C3 = "D";
        private const string SW_GroundSprayGeneric_C1 = "T";
        private const string SW_GroundSprayGeneric_C2 = "T";
        private const string SW_GroundSprayGeneric_Default = "X";

        //Input DataTable Column Names
        private static string DATE = "date";
        private static string PRECIP = "PrecipQuantity_hSum";
        private static string HUMIDITY = "HumidityRel";
        private static string WIND_SPEED = "WindSpeed";
        private static string WIND_GUST = "WindGustSpeed_hMax";
        private static string TEMPAIR = "TempAir";

        private static string TrnsText_A1 = "ResLegEvapRisk";
        private static string TrnsText_A2 = "ResLegEvapRisk";
        private static string TrnsText_A3 = "ResLegTooWet";
        private static string TrnsText_A4 = "ResLegTooWet";
        private static string TrnsText_A5 = "ResLegRain";

        private static string TrnsText_B2 = "ResLegRiskWind";
        private static string TrnsText_B3 = "ResLegWindy";
        private static string TrnsText_C1 = "ResLegTooLoTemp";
        private static string TrnsText_C2 = "ResLegTooLoTemp";
        private static string TrnsText_C3 = "ResLegHighTemp";
        private static string TrnsText_C4 = "ResLegHighTemp";
        private static string TrnsText_D1 = "";
        private static string TrnsText_D2 = "";
        #endregion RULESET : - SW_GroundSprayGeneric

        #region Methods

        /// <summary>
        /// This Method Creates a sample input datatable and 
        /// the ouput data table structure
        /// </summary>
        static private void IntialiseOutputDatatable()
        {
            //Create output datatable
            dtOutput = new DataTable();
            dtOutput.Columns.Add("day");
            dtOutput.Columns.Add("hour");
            dtOutput.Columns.Add("ColorCode");//color Value
            dtOutput.Columns.Add("value");//Text i.e.Letter
            dtOutput.Columns.Add("restrictions");


        }

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

            //Filter the Raw Data and extract only the data between start and end dates
            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();


            PRECIP = getColumnName(dtInput.Columns, "PrecipAmount");
            TEMPAIR = getColumnName(dtInput.Columns, "TempAir");
            HUMIDITY = getColumnName(dtInput.Columns, "Humidity");
            WIND_SPEED = getColumnName(dtInput.Columns, "WindSpeed_");
            WIND_GUST = getColumnName(dtInput.Columns, "WindSpeedMax");

            DataTable dt = ApplyRules();

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


        /// <summary>
        /// 
        /// This Method is used to fetch the end values based on the number of hours specified
        /// </summary>
        /// <param name="strHour"></param>
        /// <returns></returns>
        static private DataRow GetEndValues(int iHour)
        {
            DataRow dr = null;

            //Get the Hour +3 data
            drColl = dtInputRaw.Select("" + DATE + "='" + dateLocal.AddHours(double.Parse(iHour.ToString())) + "'");

            if (drColl != null && drColl.Length != 0)
            {
                dr = drColl[0];
            }

            //Reset the collection
            drColl = null;

            //return the DataRow
            return dr;

        }

        /// <summary>
        /// Loops through each row of the input datatable,
        /// for each hour , it check and applies the restrictions and
        /// creates an output datarow 
        /// </summary>
        /// <param name="dtInput"></param>
        /// <returns>
        /// Output table with date,hour,value,color,letter and restrictions 
        /// </returns>
        static private DataTable ApplyRules()
        {

            //Stroe the Input data count
            iRowsCount = dtInput.Rows.Count;

            //Loop through Each Cell for each hour in 24 hours 
            //Days Desired * 24
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                //Set the Offset
                if (i < 24)
                {
                    iOffSet = i + 1;
                }
                else
                {
                    iOffSet = ((i - 24) % 24) + 1;
                }
                //Set the Current Hour.This is used in creating output data
                if (i < 24)
                {
                    iCurrHour = i;
                }
                else
                {
                    iCurrHour = (i - 24) % 24;
                }

                //Get the Datarow for Current Hour
                drHour0 = dtInput.Rows[i];

                //Check if data is present for atleast for one of the series in the Input datatable
                bool dataPresent = false;

                if (drHour0[PRECIP].ToString() != "" || drHour0[HUMIDITY].ToString() != "" || drHour0[TEMPAIR].ToString() != "" || drHour0[WIND_SPEED].ToString() != "" || drHour0[WIND_GUST].ToString() != "")
                    dataPresent = true;

                //Apply the ruleset ony if data is present 
                if (dataPresent)
                {
                    //Get the Current hour date
                    dateLocal = DateTime.Parse(drHour0[DATE].ToString());

                    //Set the Current Hour . This is used in GetOutput Data method
                    #region Check Indicies for hourly data
                    //Check for Pre Index
                    //No hour -1 and Hour -2 data will be available.
                    if (i == 0)
                    {
                        //Get the Hour -1 data
                        drHour_M1 = GetEndValues(-1);


                        //get Hour -2 data
                        drHour_M2 = GetEndValues(-2);


                        //drHour_M2 = drHour_M1 = drHour0;
                    }
                    //No Hour -2 will be available
                    else if (i == 1)
                    {
                        drHour_M1 = dtInput.Rows[i - 1];

                        //get Hour -2 data
                        drHour_M2 = GetEndValues(-2);



                    }

                    else
                    {
                        drHour_M1 = dtInput.Rows[i - 1];
                        drHour_M2 = dtInput.Rows[i - 2];
                    }


                    //Check for Post Index
                    //No Hour + 3 data
                    if (i == (iRowsCount - 3))
                    {
                        drHour_P1 = dtInput.Rows[i + 1];
                        drHour_P2 = dtInput.Rows[i + 2];

                        //Get the Hour +3 data
                        drHour_P3 = GetEndValues(+3);
                    }
                    //No Hour + 2 and hour + 3 data
                    else if (i == (iRowsCount - 2))
                    {
                        drHour_P1 = dtInput.Rows[i + 1];

                        //Get the Hour +2 data
                        drHour_P2 = GetEndValues(+2);

                        //Get the Hour +3 data
                        drHour_P3 = GetEndValues(+3);




                    }
                    //No Hour + 1,Hour + 2 and hour + 3 data
                    else if (i == (iRowsCount - 1))
                    {
                        //Get the Hour +3 data
                        drHour_P1 = GetEndValues(+1);

                        //Get the Hour +3 data
                        drHour_P2 = GetEndValues(+2);

                        //Get the Hour +3 data
                        drHour_P3 = GetEndValues(+3);


                    }
                    //Get all the Data
                    else
                    {
                        drHour_P1 = dtInput.Rows[i + 1];
                        drHour_P2 = dtInput.Rows[i + 2];
                        drHour_P3 = dtInput.Rows[i + 3];
                    }

                    #endregion

                    //Get the Day/Night
                    SetDayNight();

                    //Apply rules for precipition
                    CalcRulesForPrecipQuantity_hSum();

                    //Apply rules for humidity
                    CalcRulesForHumidityRelity();

                    //Apply rules for windspeed    
                    CalcRulesForWindSpeed();

                    //Apply Rules for WindGust
                    CalcRulesForWindGustSpeed_hMax();

                    //Apply rules for Air Temparature.
                    CalcRulesForTempAir();

                    //finalise the Cell Value Based on Night or Day
                    if (!string.IsNullOrEmpty(strCellValue))
                    {
                        //check if its night or day
                        // if night ,then Prefix 0 to Cell Value
                        if (blIsNight)
                        {
                            strFinalCellValue = strCellValue + "0";
                        }
                        else
                        {
                            strFinalCellValue = strCellValue + "1";
                        }
                    }
                    else
                    //set default value - 2 (No risk)
                    {
                        if (blIsNight)
                        {
                            strFinalCellValue = strDefaultCellValue + "0";
                        }
                        else
                        {
                            strFinalCellValue = strDefaultCellValue + "1";
                        }
                    }

                    if (strFinalCellValue.Contains("00"))
                    {
                        strFinalCellValue = "0";
                    }
                    else if (strFinalCellValue.Contains("01"))
                    {
                        strFinalCellValue = "1";
                    }
                }//end of data present Check

                //Create the Output Datatable for Each Cell .
                GetOutputData();

                //Clear final ColorCode value
                strFinalCellValue = string.Empty;

            }//End of Loop

            return dtOutput;


        }

        /// <summary>
        /// Applies all the rules related to input- Precipition 
        /// </summary>
        /// <param name="fHour_M2"></param>
        /// <param name="fHour_M1"></param>
        /// <param name="fHour0"></param>
        /// <param name="fHour_P1"></param>
        /// <param name="fHour_P2"></param>
        /// <param name="fHour_P3"></param>
        static private void CalcRulesForPrecipQuantity_hSum()
        {
            //Variables used for post application.
            float fHrP1 = float.MinValue;
            float fHrP2 = float.MinValue;


            //Set the Hourly Data of PrecipQuantity_hSumitation
            fHour_M2 = drHour_M2[PRECIP].ToString();
            fHour_M1 = drHour_M1[PRECIP].ToString();
            fHour0 = drHour0[PRECIP].ToString();

            if (drHour_P1 != null)
                fHour_P1 = drHour_P1[PRECIP].ToString();

            if (drHour_P2 != null)
                fHour_P2 = drHour_P2[PRECIP].ToString();

            if (drHour_P3 != null)
                fHour_P3 = drHour_P3[PRECIP].ToString();
            #region precipitation - Pre Application
            //A4 - Leaves too wet, risk of product run off	
            //Condition - precipAmount mm 	> 0 / hour -->(Hour -1)
            if (!string.IsNullOrEmpty(fHour_M1))
            {
                if (float.Parse(fHour_M1) > 0)
                {

                    //Check if the rule is already exists
                    if (!CheckRestrictioninList(RuleNames.A4.ToString()))
                    {
                        //check if this is the first one to be added
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A4.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A4.ToString();
                    }

                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";
                }
            }

            //A3 - Leaves wet, risk of product run off	
            //Condition = precipAmount mm 	> 0 / hour 
            //Check for Hour - 2
            if (!string.IsNullOrEmpty(fHour_M2))
            {
                if (float.Parse(fHour_M2) > 0)
                {
                    if (!CheckRestrictioninList(RuleNames.A3.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A3.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A3.ToString();
                    }

                    //Set the Color - 1 - Medium risk
                    SetCellValue();


                }
            }
            #endregion

            #region Precipitation- Application Hour(h0)
            //A5 - precipitation after application,  risk of product run off	
            //conditions -precipAmount > 0 hour0, 

            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) > 0)
                {
                    if (!CheckRestrictioninList(RuleNames.A5.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A5.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A5.ToString();
                    }


                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";

                }

            }

            #endregion

            #region Precipitation - Post Application
            //A5 - precipitation after application,  risk of product run off	
            //conditions -precipAmount > 0 hour + 1,hour + 2 


            if (!string.IsNullOrEmpty(fHour_P1))
                fHrP1 = float.Parse(fHour_P1);
            if (!string.IsNullOrEmpty(fHour_P2))
                fHrP2 = float.Parse(fHour_P2);


            if (fHrP1 > 0 || fHrP2 > 0)
            {
                if (!CheckRestrictioninList(RuleNames.A5.ToString()))
                {
                    if (strRestrictionList.Length == 0)
                        strRestrictionList = RuleNames.A5.ToString();
                    else
                        strRestrictionList = strRestrictionList + "," + RuleNames.A5.ToString();
                }


                //Set the Cell Value - 0 - high Risk
                strCellValue = "0";


            }



            //A5 - precipitation after application,  risk of product run off	
            //conditions -precipAmount > 2 Hour + 3, 


            if (!string.IsNullOrEmpty(fHour_P3))
            {
                if (float.Parse(fHour_P3) > 2)
                {
                    if (!CheckRestrictioninList(RuleNames.A5.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A5.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A5.ToString();
                    }


                    //Set the Color - 1 - Medium risk
                    SetCellValue();
                }
            }
            #endregion



        }

        /// <summary>
        ///  Applies all the rules related to input- WindSpeed 
        /// </summary>
        /// <param name="fHour0"></param>
        static private void CalcRulesForWindSpeed()
        {

            //Set the Hourly Data of PrecipQuantity_hSumitation
            fHour0 = drHour0[WIND_SPEED].ToString();

            #region Winspeed Application Hour(h0)
            //B2 - windspeed too high, risk of drift	
            //Condition:- windspeed m/s 	> = 5
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) >= 5)
                {
                    if (!CheckRestrictioninList(RuleNames.B2.ToString()))
                    {

                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.B2.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.B2.ToString();
                    }

                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";

                }
            }
            #endregion

            //clear the Variables which stores input data
            ClearInputVariables();
        }

        /// <summary>
        ///  Applies all the rules related to input- Wind Gust 
        /// </summary>
        /// <param name="fhour0"></param>
        static private void CalcRulesForWindGustSpeed_hMax()
        {

            fHour0 = drHour0[WIND_GUST].ToString();

            #region Winspeed Application Hour(h0)
            //B3 - Wind gusts high, risk of drift	
            //condtion :- windgust m/s 	> = 5
            if (!string.IsNullOrEmpty(fHour0))
            {

                if (float.Parse(fHour0) >= 5)
                {
                    if (!CheckRestrictioninList(RuleNames.B3.ToString()))
                    {

                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.B3.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.B3.ToString();
                    }

                    //Set the Color - 1 - Medium risk
                    SetCellValue();
                }
            }
            #endregion

            //clear the Variables which stores input data
            ClearInputVariables();
        }

        /// <summary>
        ///  Applies all the rules related to input- Humidity 
        /// </summary>
        /// <param name="fHour_M1"></param>
        /// <param name="fHour0"></param>
        /// <param name="fHour_P1"></param>
        /// <param name="fHour_P2"></param>
        static private void CalcRulesForHumidityRelity()
        {
            //Variables used for post application.
            float fHrP1 = float.MinValue;
            float fHrP2 = float.MinValue;

            fHour_M1 = drHour_M1[HUMIDITY].ToString();
            fHour0 = drHour0[HUMIDITY].ToString();
            if (drHour_P1 != null)
                fHour_P1 = drHour_P1[HUMIDITY].ToString();

            if (drHour_P2 != null)
                fHour_P2 = drHour_P2[HUMIDITY].ToString();

            #region Humidity - PreApplication
            //A3 - Leaves wet, risk of product run off	
            //Condtion :- humidityRel % = 100 (hour -1)
            if (!string.IsNullOrEmpty(fHour_M1))
            {
                if (float.Parse(fHour_M1) == 100)
                {
                    if (!CheckRestrictioninList(RuleNames.A3.ToString()))
                    {

                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A3.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A3.ToString();
                    }

                    //Set the Color - 1 - Medium risk
                    SetCellValue();
                }
            }
            #endregion

            #region Humidity - Applicaition Hour(H0)

            //A4 - Leaves too wet, risk of product run off
            //Condition:=humidityRel % = 100 (Application Hour)
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) == 100)
                {
                    if (!CheckRestrictioninList(RuleNames.A4.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A4.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A4.ToString();
                    }


                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";

                }
            }

            //A3 - Leaves wet, risk of product run off	
            //humidityRel %> 87 at Application Hour
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) > 87)
                {
                    if (!CheckRestrictioninList(RuleNames.A3.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A3.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A3.ToString();
                    }

                    //Set the Color - 1 - Medium risk
                    SetCellValue();

                }
            }

            //A2 - Relative humidity low, risk of droplet evaporation	
            //condition :- humidityRel % 	30 < x <= 50
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) > 30 && float.Parse(fHour0) <= 50)
                {
                    if (!CheckRestrictioninList(RuleNames.A2.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A2.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A2.ToString();
                    }


                    //Set the Color - 1 - Medium risk
                    SetCellValue();

                }
            }
            //A1 - Relative humidity too low, risk of droplet evaporation	
            //condition :- humidityRel % 	<= 30
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) <= 30)
                {
                    if (!CheckRestrictioninList(RuleNames.A1.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.A1.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.A1.ToString();
                    }


                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";

                }
            }
            #endregion

            #region Humidity - Post Application

            //A4 - Leaves too wet, risk of product run off
            //Condition:=humidityRel % = 100 (Application Hour)
            // humidityRel % = 100(hour + 1 or hour + 2)

            if (!string.IsNullOrEmpty(fHour_P1))
                fHrP1 = float.Parse(fHour_P1);
            if (!string.IsNullOrEmpty(fHour_P2))
                fHrP2 = float.Parse(fHour_P2);


            if (fHrP1 == 100 || fHrP2 == 100)
            {
                if (!CheckRestrictioninList(RuleNames.A4.ToString()))
                {
                    if (strRestrictionList.Length == 0)
                        strRestrictionList = RuleNames.A4.ToString();
                    else
                        strRestrictionList = strRestrictionList + "," + RuleNames.A4.ToString();
                }

                //Set the Color - 1 - Medium risk
                SetCellValue();


            }
            #endregion

            //clear the Variables which stores input data
            ClearInputVariables();
        }


        /// <summary>
        ///  Applies all the rules related to input- Air Temperature 
        /// </summary>
        /// <param name="fHour_M1"></param>
        /// <param name="fHour0"></param>
        /// <param name="fHour_P1"></param>
        /// <param name="fHour_P2"></param>
        /// <param name="fHour_P3"></param>
        static private void CalcRulesForTempAir()
        {

            fHour_M1 = drHour_M1[TEMPAIR].ToString();
            fHour0 = drHour0[TEMPAIR].ToString();

            if (drHour_P1 != null)
                fHour_P1 = drHour_P1[TEMPAIR].ToString();

            if (drHour_P2 != null)
                fHour_P2 = drHour_P2[TEMPAIR].ToString();


            if (drHour_P3 != null)
                fHour_P3 = drHour_P3[TEMPAIR].ToString();

            #region Air Temperature - Pre Application

            //C4 - Temperature too high, risk of droplet evaporation, local thermics and drift	
            //tempair °C > 32 (Hour -1 )
            if (!string.IsNullOrEmpty(fHour_M1))
            {
                if (float.Parse(fHour_M1) > 32)
                {
                    if (!CheckRestrictioninList(RuleNames.C4.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C4.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C4.ToString();
                    }


                    //Set the Color - 1 - Medium risk
                    SetCellValue();

                }
            }
            #endregion

            #region Air Temperature - Application Hour(h0)


            //C3 - Temperature high, risk of droplet evaporation, local thermics and drift
            //condtion :- tempair °C > 28
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) > 28)
                {
                    if (!CheckRestrictioninList(RuleNames.C3.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C3.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C3.ToString();
                    }


                    //Set the Color - 1 - Medium risk
                    SetCellValue();


                }
            }

            //C4 - Temperature too high, risk of droplet evaporation, local thermics and drift	
            //tempair °C > 32 (Application Hour)
            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) > 32)
                {
                    if (!CheckRestrictioninList(RuleNames.C4.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C4.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C4.ToString();
                    }


                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";


                }
            }
            //C1 - Temperature too low, risk of reduced product efficiency	
            //condition :- tempair °C 	<= 5

            if (!string.IsNullOrEmpty(fHour0))
            {
                if (float.Parse(fHour0) <= 5)
                {
                    if (!CheckRestrictioninList(RuleNames.C1.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C1.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C1.ToString();
                    }


                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";


                }
            }


            //C2 - Temperature low, risk of reduced product efficiency	
            //Condition :- tempair °C = 5 < x <= 10
            if (!string.IsNullOrEmpty(fHour0))
            {
                if ((float.Parse(fHour0) > 5 && float.Parse(fHour0) <= 10))
                {
                    if (!CheckRestrictioninList(RuleNames.C2.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C2.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C2.ToString();
                    }

                    //Set the Color - 1 - Medium risk
                    SetCellValue();


                }
            }
            #endregion

            #region Air Temperature - Post Application

            //C1 - Temperature too low, risk of reduced product efficiency	
            //condition :- tempair °C 	<= 5
            //tempair °C 	<= 5 at hour + 1 and hour + 2  and Hour + 3
            if ((!string.IsNullOrEmpty(fHour_P1)) && (!string.IsNullOrEmpty(fHour_P2)) &&
                (!string.IsNullOrEmpty(fHour_P3)))
            {
                if (float.Parse(fHour_P1) <= 5 &&
                    float.Parse(fHour_P2) <= 5 &&
                    float.Parse(fHour_P3) <= 5)
                {
                    if (!CheckRestrictioninList(RuleNames.C1.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C1.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C1.ToString();
                    }


                    //Set the Cell Value - 0 - high Risk
                    strCellValue = "0";


                }
            }

            //C2 - Temperature low, risk of reduced product efficiency	
            //Condition :- tempair °C = 5 < x <= 10
            //tempair °C = 5 < x <= 10 --- at hour + 1 and hour + 2  and Hour + 3
            if ((!string.IsNullOrEmpty(fHour_P1)) && (!string.IsNullOrEmpty(fHour_P2)) &&
               (!string.IsNullOrEmpty(fHour_P3)))
            {
                if ((float.Parse(fHour_P1) > 5 && float.Parse(fHour_P1) <= 10) &&
                (float.Parse(fHour_P2) > 5 && float.Parse(fHour_P2) <= 10) &&
                (float.Parse(fHour_P3) > 5 && float.Parse(fHour_P3) <= 10))
                {
                    if (!CheckRestrictioninList(RuleNames.C2.ToString()))
                    {
                        if (strRestrictionList.Length == 0)
                            strRestrictionList = RuleNames.C2.ToString();
                        else
                            strRestrictionList = strRestrictionList + "," + RuleNames.C2.ToString();
                    }

                    //Set the Color - 1 - Medium risk
                    SetCellValue();


                }
            }
            #endregion

            //clear the Variables which stores input data
            ClearInputVariables();
        }

        /// <summary>
        /// Creates an output datarow for each cell and adds it to Output datatable
        /// </summary>
        static private void GetOutputData()
        {
            DataRow drOutput = null;
            string[] strRestList = null;
            if (!string.IsNullOrEmpty(strRestrictionList))
            {
                strRestList = strRestrictionList.Split(',').ToArray();
            }


            //Add the Output detials
            drOutput = dtOutput.NewRow();

            drOutput["day"] = drHour0[DATE].ToString();//Day

            drOutput["hour"] = iCurrHour.ToString(); //Hour

            drOutput["ColorCode"] = strFinalCellValue; ;

            //loop through each of the restriction and assignt e
            if (strRestList != null)
            {

                //Letter
                //Set the Letter as "X" if there are multiple restriction for a cell value
                if (strRestList.Length > 1)
                {
                    drOutput["value"] = SW_GroundSprayGeneric_Default;
                }
                else
                {
                    //if there is only one restriction ,
                    //Get the letter from GetRestrictionLetter() by passing the rulecode/rulename
                    drOutput["value"] = GetRestrictionLetter(strRestList[0]);
                }

                //Resrtictions 
                //Get the TransTags for restriction from the mappings
                if (!string.IsNullOrEmpty(strRestrictionList))
                {
                    string[] strArrRest = strRestrictionList.Split(',');
                    string strFinalRest = string.Empty;
                    string strTransTag = string.Empty;
                    foreach (string strRest in strArrRest)
                    {
                        //get the trans tag key
                        strTransTag = GetRestrictionTransTag(strRest);

                        if (strFinalRest.Length == 0)
                            strFinalRest = strTransTag;
                        else
                            //aad this to the list only if it does not exists
                            if (strFinalRest.IndexOf(strTransTag) == -1)
                                strFinalRest = strFinalRest + "," + strTransTag;

                    }

                    drOutput["restrictions"] = strFinalRest;
                }

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
        /// Returns the Letter to be dispalyed in the cell based on the Restriction Name
        /// </summary>
        /// <param name="strRestName"></param>
        /// <returns></returns>
        static private string GetRestrictionLetter(string strRestName)
        {
            string strLetter = string.Empty;

            switch (strRestName)
            {
                case "A1": strLetter = SW_GroundSprayGeneric_A1;
                    break;
                case "A2": strLetter = SW_GroundSprayGeneric_A2;
                    break;
                case "A3": strLetter = SW_GroundSprayGeneric_A3;
                    break;
                case "A4": strLetter = SW_GroundSprayGeneric_A4;
                    break;
                case "A5": strLetter = SW_GroundSprayGeneric_A5;
                    break;
                case "B2": strLetter = SW_GroundSprayGeneric_B2;
                    break;
                case "B3": strLetter = SW_GroundSprayGeneric_B3;
                    break;
                case "C1": strLetter = SW_GroundSprayGeneric_C1;
                    break;
                case "C2": strLetter = SW_GroundSprayGeneric_C2;
                    break;
                case "C3": strLetter = SW_GroundSprayGeneric_C3;
                    break;
                case "C4": strLetter = SW_GroundSprayGeneric_C4;
                    break;
                default: strLetter = SW_GroundSprayGeneric_Default;
                    break;
            }

            //Return the Letter
            return strLetter;


        }

        static private string GetRestrictionTransTag(string sRes)
        {
            string strTransTag = string.Empty;

            switch (sRes)
            {
                case "A1": strTransTag = TrnsText_A1;
                    break;
                case "A2": strTransTag = TrnsText_A2;
                    break;
                case "A3": strTransTag = TrnsText_A3;
                    break;
                case "A4": strTransTag = TrnsText_A4;
                    break;
                case "A5": strTransTag = TrnsText_A5;
                    break;
                case "B2": strTransTag = TrnsText_B2;
                    break;
                case "B3": strTransTag = TrnsText_B3;
                    break;
                case "C1": strTransTag = TrnsText_C1;
                    break;
                case "C2": strTransTag = TrnsText_C2;
                    break;
                case "C3": strTransTag = TrnsText_C3;
                    break;
                case "C4": strTransTag = TrnsText_C4;
                    break;
                default: strTransTag = sRes;
                    break;
            }

            //Return the Transtag
            return strTransTag;
        }

        /// <summary>
        /// Sets the Default color Green based on isNight Criteria
        /// </summary>

        static private void ClearInputVariables()
        {
            fHour_M2 = string.Empty;
            fHour_M1 = string.Empty;
            fHour0 = string.Empty;
            fHour_P1 = string.Empty;
            fHour_P2 = string.Empty;
            fHour_P3 = string.Empty;
        }
        #endregion
    }
}
