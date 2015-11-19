using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Syngenta.Agricast.Modals
{
    public class RS_FrostText
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

        static  string strCellText = string.Empty;
        //Set Default CellValue
        static string strDefaultCellValue = "2";

        //stored Cell Color
        string strCellColor = string.Empty;

        private static string TEMP_MIN5 = "temp_min";
        
        private static string DATE = "date";

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
                return RulesetSerieList;
            }
        }

        enum RuleNames
        {
            dash, dd_frostlight, dd_frostmod, dd_frostheavy, dd_frostextreme

        }

        #endregion

        #region Methods
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

            TEMP_MIN5 = getColumnName(dtInput.Columns, "TempAir");
          
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
                strCurrValue = dtInput.Rows[i][TEMP_MIN5].ToString();

                //Get the Date
                //trCurrValue = DateTime.Parse(strCurrValue).ToShortDateString();

                //Set the Current Hour . This is used in GetOutput Data method
                iCurrHour = i;

              

                if (!string.IsNullOrEmpty(strCurrValue))
                {
                    //Get the Frost Text for the day
                    GetFrostText(strCurrValue);
                }
                else
                {
                    strCellValue = "0.0";
                }
                //Add the day and Cell Text to Output data table 
                //Add a Row to Output Datatable
                drOutput = dtOutput.NewRow();

                drOutput["day"] = dtInput.Rows[i][DATE].ToString(); //Day
                drOutput["hour"] = iCurrHour;
                drOutput["restrictions"] = strCellText;
                drOutput["ColorCode"] = strCellValue;
                dtOutput.Rows.Add(drOutput);

                //Reset the Value
                strCellValue = string.Empty;
                strCellText = string.Empty;

            }//end of loop
            return dtOutput;
        }

        static private void GetFrostText(string strVal)
        {
            if (!string.IsNullOrEmpty(strVal))
            {
                //condition :- lower="0"  
                //Text : - "-"
                if (float.Parse(strCurrValue) > 0.0)
                {
                    strCellText = RuleNames.dash.ToString();

                }

                //condition :- > -3.5 and < = 0.0
                //Text : - "Light"
                if ((float.Parse(strCurrValue) > -3.5) && (float.Parse(strCurrValue) <= 0.0))
                {
                    strCellText = RuleNames.dd_frostlight.ToString();

                }


                //condition :- > -6.5 and < =-3.5
                //Text : - "Light"
                if ((float.Parse(strCurrValue) > -6.5) && (float.Parse(strCurrValue) <= -3.5))
                {
                    strCellText = RuleNames.dd_frostmod.ToString();
                }


                //condition :- > -11.5 and < = -6.5
                //Text : - "Light"
                if ((float.Parse(strCurrValue) > -11.5) && (float.Parse(strCurrValue) <= -6.5))
                {
                    strCellText = RuleNames.dd_frostheavy.ToString();
                }

                //condition :- upper="-11.5"  
                //Text : - "Extreme"
                if (float.Parse(strCurrValue) < -11.5)
                {
                    strCellText = RuleNames.dd_frostextreme.ToString();
                }

                //Set the DB(input) value as Cell ColorCode Value
                strCellValue = strCurrValue;
            }
            else
            {
                strCellValue = "0.0";
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

        #endregion

    }
}
