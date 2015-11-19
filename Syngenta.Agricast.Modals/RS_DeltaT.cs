using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace Syngenta.Agricast.Modals
{
    public class RS_DeltaT
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


        //Set Default CellValue
        static string strDefaultCellValue = "2";

        //stored Cell Color
        string strCellColor = string.Empty;

        private static string DATE = "date";
        private static string DELTAT = "deltat";

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
                RulesetSerieList.Rows.Add("deltat");
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
            dtInput = objIRuleSets.DtInput;
            //sunRise = objIRuleSets.SunRise;
            //sunSet = objIRuleSets.SunSet;
            dtOutput = objIRuleSets.DtOutput;


            DELTAT = getColumnName(dtInput.Columns, "deltat");

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
                strCurrValue = dtInput.Rows[i][DELTAT].ToString();

                //Set the Current Hour . This is used in GetOutput Data method
                iCurrHour = i;

                if (!string.IsNullOrEmpty(strCurrValue))
                {
                    //Call the method to check constraints and set cell color
                    CalcRulesFordeltat(strCurrValue);
                }

                //Add a Row to Output Datatable 
                drOutput = dtOutput.NewRow();

                //Add the Values to Output datatable
                drOutput["day"] = dtInput.Rows[i][DATE].ToString(); //Day

                drOutput["hour"] = iCurrHour.ToString();

                drOutput["ColorCode"] = strCellValue;

                dtOutput.Rows.Add(drOutput);


                //Set Celel Value to Empty
                strCellValue = null;

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

        static private void CalcRulesFordeltat(string strCellValue_p)
        {
            //check the value and set the color
            if (!string.IsNullOrEmpty(strCellValue_p))
            {
                //Value Range :- upper=0
                if (float.Parse(strCellValue_p) < 0.0)
                {

                    //Set the Color - 0 - High risk
                    strCellValue = "0";
                }

                //Value Range :- lower="0.0" upper="2.0"
                if (float.Parse(strCellValue_p) >= 0.0 && float.Parse(strCellValue_p) <= 2.0)
                {

                    //Set the Color - 0 - Medium risk
                    strCellValue = "1";
                }

                //Value Range :- lower="8.0" upper="9.999"
                if (float.Parse(strCellValue_p) >= 8.0 && float.Parse(strCellValue_p) <= 9.999)
                {

                    //Set the Color - 0 - Medium risk
                    strCellValue = "1";
                }

                //Value Range :- Yellow
                if (float.Parse(strCellValue_p) > 9.999)
                {

                    //Set the Color - 0 - High risk
                    strCellValue = "0";
                }

            }//end of If
            else
            {

                //Set the Default Color
                strCellValue = strDefaultCellValue;
            }

            //If the Value does not satisfy any of the conditions m,then set default color
            if (String.IsNullOrEmpty(strCellValue))
            {
                strCellValue = strDefaultCellValue;
            }

        }
    }
}
