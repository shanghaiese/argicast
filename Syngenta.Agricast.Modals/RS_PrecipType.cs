using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace Syngenta.Agricast.Modals
{
    public class RS_PrecipType
    {
        #region Variables and Declarations

        //Input and Output datatable
        static DataTable dtInput = null;
        static DataRow drInput = null;
        static DataTable dtOutput = null;

        static int iRowsCount = 0;
        static int iCurrHour = 0;

        //Holds Current Cell Value
        static string strCurrValue = string.Empty;

        static string strCellValue = string.Empty;
        static string strFinalCellValue = string.Empty;

        static DataTable dtInputRaw = null;

        //Variables to hold start and end dates
        static DateTime startDate;
        static DateTime endDate;

        //Variables to hold SunRise / sunSet Values
        static DateTime sunRise = DateTime.Now;
        static DateTime sunSet = DateTime.Now;

        //Set Default CellValue
        static string strDefaultCellValue = "2";

        private static string PRECIPTYPE = "preciptype";
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

                RulesetSerieList.Rows.Add("preciptype");
                return RulesetSerieList;
            }
        }
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

        static public DataTable CalculateRuleSets(IRuleSets objIRuleSets)
        {
            //IntialiseOutputDatatable();

            //Assign the Value through Interface Members
            dtInputRaw = objIRuleSets.DtInput;
            startDate = objIRuleSets.StartDate;
            endDate = objIRuleSets.EndDate;
            dtOutput = objIRuleSets.DtOutput;
            sunRise = objIRuleSets.SunRise;
            sunSet = objIRuleSets.SunSet;


            //One day data= data from 1 am of current date to 12am of next daya
            //for eg. 17/5/2012 data = 17/5/2012 1:00:00 am to 18/5/2012 12:00:00 am
            dtInput = dtInputRaw.Select("" + DATE + " >= '" + startDate.AddHours(1) + "' and " + DATE + " <='" + endDate + "'").CopyToDataTable();

            //Check if all the required data is present
            PRECIPTYPE = getColumnName(dtInput.Columns, "PrecipType");

            

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
                strCurrValue = dtInput.Rows[i][PRECIPTYPE].ToString();

                //Set the Current Hour . This is used in GetOutput Data method
                iCurrHour = i;

                if (!string.IsNullOrEmpty(strCurrValue))
                {
                    //Call the method to check constraints and set cell color
                    CalcRulesForPrecipType(strCurrValue);
                }

                //Add a Row to Output Datatable
                drOutput = dtOutput.NewRow();

                //Add the Values to Output datatable
                drOutput["day"] = dtInput.Rows[i][DATE].ToString(); //Day

                drOutput["hour"] = iCurrHour.ToString();

                drOutput["colorcode"] = strCellValue;

                dtOutput.Rows.Add(drOutput);

                //set the colorcode value to default code = 0
                strCellValue = "0";

            }//end of loop

            return dtOutput;
        }

        static private void CalcRulesForPrecipType(string strCellValue_p)
        {
            //check the value and set the color
            if (!string.IsNullOrEmpty(strCellValue_p))
            {
                //Value Range :- lower="-0.5" upper="0.5"
                if (float.Parse(strCellValue_p) > -0.5 && float.Parse(strCellValue_p) < 0.5)
                {

                    strCellValue = "0";
                }

                //Value Range :- lower="0.5" upper="1.5"
                if (float.Parse(strCellValue_p) > 0.5 && float.Parse(strCellValue_p) < 1.5)
                {

                    strCellValue = "1";
                }

                //Value Range :- lower="1.5" upper="2.5"
                if (float.Parse(strCellValue_p) > 1.5 && float.Parse(strCellValue_p) < 2.5)
                {
                    strCellValue = "2";
                }

                //Value Range :- lower="2.5" upper="3.5"
                if (float.Parse(strCellValue_p) > 2.5 && float.Parse(strCellValue_p) < 3.5)
                {
                    strCellValue = "3";
                }

                //Value Range :- lower="3.5" upper="4.5"
                if (float.Parse(strCellValue_p) > 3.5 && float.Parse(strCellValue_p) < 4.5)
                {
                    strCellValue = "4";
                }

            }//end of If
            else
            {
                //set White as Default Color Code
                strCellValue = "0";
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

    }
}
