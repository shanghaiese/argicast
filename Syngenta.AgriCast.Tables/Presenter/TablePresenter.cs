using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Drawing;
using Syngenta.AgriCast.Tables;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Tables.Service;
using Syngenta.AgriCast.Tables.View;
using Syngenta.AgriCast.Common.Service;
using System.Globalization;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common;



namespace Syngenta.AgriCast.Tables.Presenter
{
    public class TablePresenter
    {
        ITable Itable;
        IRuleSets IRule;
        string controlName;
        CommonUtil objComm = new CommonUtil();
        // int LastColIndex;
        const string FIRSTCOLNAME = "hour";
        int sHours = 0;
        int eHours = 0;
        DataTable RulesetList;
        string strCulCode;


        public TablePresenter()
        {

        }

        public TablePresenter(ITable ITableView, string Name)
        {
            if (ITableView != null)
            {
                Itable = ITableView;
            }
            controlName = Name;
        }
        ServiceHandler objSvc = new ServiceHandler();
        ServiceInfo objSvcInfo;
        CommonUtil objComUtil = new CommonUtil();
        TableService objTblSvc = new TableService();
        CultureInfo VariantCulture;
        ServicePresenter objSvcPre = new ServicePresenter();
        string strDateFormat = string.Empty;

        public void GetTableSeries(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                Itable.dsSeriesData = objSvc.GetTableSeries(allign, controlName);

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LOADSERIES_FALURE) + " : " + ex.Message.ToString();
            }
        }
        public void GetCompleteRuleset(string name, string allign, string aggregation, string datasource, int start, int end, string ruleset, string pallette, string Nodename, IRuleSets IRobj)
        {
            try
            {
                if (IRobj != null)
                {
                    IRule = IRobj;
                }
                GetcompleteSeriesList(allign);
                GetTableSeries(allign);
                Itable.dtTableSeries = Itable.dsSeriesData.Tables[0];
                Itable.dtTableLegends = Itable.dsSeriesData.Tables[1];

                //Getting start hours and end hours(to fetch extra data)
                GetRulesetData(ruleset, IRule);
                GetSeriesData(RulesetList, aggregation, datasource, start, end, sHours, eHours);

                DataTable dtByDays = Itable.dtSeries;

                DataColumn dc = new DataColumn();
                dc.ColumnName = getTranslatedText(FIRSTCOLNAME, objSvcInfo.Culture);
                dc.DefaultValue = null;
                dtByDays.Columns.Add(dc);


                int LastColIndex = dtByDays.Columns.Count - 1;
                for (int i = 0; i < dtByDays.Rows.Count; i++)
                {
                    DateTime Date = Convert.ToDateTime(dtByDays.Rows[i][0].ToString());
                    dtByDays.Rows[i][LastColIndex] = Date.Hour;
                }
                IRule.DtInput = dtByDays;

                IRule.StartDate = System.DateTime.Today.AddDays(start);
                IRule.EndDate = System.DateTime.Today.AddDays(end);
                if (IRule.StartDate > IRule.EndDate)
                {
                    DateTime tmp = IRule.StartDate;
                    IRule.StartDate = IRule.EndDate;
                    IRule.EndDate = tmp;
                }
                //if (start < end)
                //{
                //    IRule.StartDate = System.DateTime.Today.AddDays(start);
                //    IRule.EndDate = System.DateTime.Today.AddDays(end);
                //}
                //else
                //{
                //    IRule.StartDate = System.DateTime.Today.AddDays(end);
                //    IRule.EndDate = System.DateTime.Today.AddDays(start);

                //}

                ////Removing the last extra day at the end
                ////eg:- start 0, end 2 should get 2 days of data starting from current date
                //if (IRule.StartDate <= System.DateTime.Today && IRule.EndDate <= System.DateTime.Today)
                //{
                //    IRule.StartDate = IRule.StartDate.AddDays(1);
                //}
                //else
                //{
                //    IRule.EndDate = IRule.EndDate.AddDays(-1);
                //}

                GetRulesetData(ruleset, IRobj);

                //Itable.alSeries = new ArrayList();

                //for (int i = 1; i < LastColIndex; i++)
                //{
                //    Itable.alSeries.Add(dtByDays.Columns[i].ColumnName.ToString() + "," + Itable.dtTableSeries.Rows[i - 1][1].ToString() + "," + Itable.dtTableSeries.Rows[i - 1][0].ToString() + "," + Itable.dtTableSeries.Rows[i - 1][3].ToString());

                //}
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_RULESETLOAD_FAILURE) + ex.Message.ToString();

            }
        }
        public void GetRulesetData(string ruleset, IRuleSets IRule)
        {
            try
            {
                switch (ruleset.ToLower())
                {
                    case "rs_bruchid":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Bruchid.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Bruchid.StartHours;
                            eHours = RS_Bruchid.EndHours;
                            RulesetList = RS_Bruchid.RulesetSeriesList;
                        }
                        break;
                    case "rs_deltat":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_DeltaT.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_DeltaT.StartHours;
                            eHours = RS_DeltaT.EndHours;
                            RulesetList = RS_DeltaT.RulesetSeriesList;
                        }
                        break;
                    case "rs_disease_potato_smith":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Disease_Potato_Smith.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Disease_Potato_Smith.StartHours;
                            eHours = RS_Disease_Potato_Smith.EndHours;
                            RulesetList = RS_Disease_Potato_Smith.RulesetSeriesList;
                        }
                        break;
                    case "rs_earlyblight":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_EarlyBlight.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_EarlyBlight.StartHours;
                            eHours = RS_EarlyBlight.EndHours;
                            RulesetList = RS_EarlyBlight.RulesetSeriesList;
                        }
                        break;
                    case "rs_frosttext":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_FrostText.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_FrostText.StartHours;
                            eHours = RS_FrostText.EndHours;
                            RulesetList = RS_FrostText.RulesetSeriesList;
                        }
                        break;
                    case "rs_harvest":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Harvest.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Harvest.StartHours;
                            eHours = RS_Harvest.EndHours;
                            RulesetList = RS_Harvest.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_cotton":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Cotton.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Cotton.StartHours;
                            eHours = RS_Plant_Cotton.EndHours;
                            RulesetList = RS_Plant_Cotton.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_maize":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Maize.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Maize.StartHours;
                            eHours = RS_Plant_Maize.EndHours;
                            RulesetList = RS_Plant_Maize.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_maize_heavy":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Maize_Heavy.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Maize_Heavy.StartHours;
                            eHours = RS_Plant_Maize_Heavy.EndHours;
                            RulesetList = RS_Plant_Maize_Heavy.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_maize_light":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Maize_Light.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Maize_Light.StartHours;
                            eHours = RS_Plant_Maize_Light.EndHours;
                            RulesetList = RS_Plant_Maize_Light.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_maize_medium":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Maize_Medium.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Maize_Medium.StartHours;
                            eHours = RS_Plant_Maize_Medium.EndHours;
                            RulesetList = RS_Plant_Maize_Medium.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_sorghum":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Sorghum.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Sorghum.StartHours;
                            eHours = RS_Plant_Sorghum.EndHours;
                            RulesetList = RS_Plant_Sorghum.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_sugarbeet_heavy":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Sugarbeet_Heavy.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Sugarbeet_Heavy.StartHours;
                            eHours = RS_Plant_Sugarbeet_Heavy.EndHours;
                            RulesetList = RS_Plant_Sugarbeet_Heavy.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_sugarbeet_light":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Sugarbeet_Light.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Sugarbeet_Light.StartHours;
                            eHours = RS_Plant_Sugarbeet_Light.EndHours;
                            RulesetList = RS_Plant_Sugarbeet_Light.RulesetSeriesList;
                        }
                        break;
                    case "rs_plant_sugarbeet_medium":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_Plant_Sugarbeet_Medium.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_Plant_Sugarbeet_Medium.StartHours;
                            eHours = RS_Plant_Sugarbeet_Medium.EndHours;
                            RulesetList = RS_Plant_Sugarbeet_Medium.RulesetSeriesList;
                        }
                        break;
                    case "rs_preciptype":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_PrecipType.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_PrecipType.StartHours;
                            eHours = RS_PrecipType.EndHours;
                            RulesetList = RS_PrecipType.RulesetSeriesList;
                        }
                        break;
                    case "rs_runoff":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_RunOff.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_RunOff.StartHours;
                            eHours = RS_RunOff.EndHours;
                            RulesetList = RS_RunOff.RulesetSeriesList;
                        }
                        break;
                    case "rs_severeweathere":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = RS_SevereWeathere.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = RS_SevereWeathere.StartHours;
                            eHours = RS_SevereWeathere.EndHours;
                            RulesetList = RS_SevereWeathere.RulesetSeriesList;
                        }
                        break;
                    case "sw_aerialspraygeneric":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_AerialSprayGeneric.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_AerialSprayGeneric.StartHours;
                            eHours = SW_AerialSprayGeneric.EndHours;
                            RulesetList = SW_AerialSprayGeneric.RulesetSeriesList;
                        }
                        break;
                    case "sw_groundsprayfungicide":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayFungicide.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayFungicide.StartHours;
                            eHours = SW_GroundSprayFungicide.EndHours;
                            RulesetList = SW_GroundSprayFungicide.RulesetSeriesList;
                        }
                        break;
                    case "sw_groundspraygeneric":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayGeneric.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayGeneric.StartHours;
                            eHours = SW_GroundSprayGeneric.EndHours;
                            RulesetList = SW_GroundSprayGeneric.RulesetSeriesList;
                        }
                        break;
                    case "sw_groundsprayclomazone":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayGenericClomazone.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayGenericClomazone.StartHours;
                            eHours = SW_GroundSprayGenericClomazone.EndHours;
                            RulesetList = SW_GroundSprayGenericClomazone.RulesetSeriesList;
                        }
                        break;
                        
                    case "sw_groundsprayherbicide_ld":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayHerbicide_LD.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayHerbicide_LD.StartHours;
                            eHours = SW_GroundSprayHerbicide_LD.EndHours;
                            RulesetList = SW_GroundSprayHerbicide_LD.RulesetSeriesList;
                        }
                        break;
                    case "sw_groundsprayherbicide_pe":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayHerbicide_PE.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayHerbicide_PE.StartHours;
                            eHours = SW_GroundSprayHerbicide_PE.EndHours;
                            RulesetList = SW_GroundSprayHerbicide_PE.RulesetSeriesList;
                        }
                        break;
                    case "sw_groundsprayherbicide_sd":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayHerbicide_SD.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayHerbicide_SD.StartHours;
                            eHours = SW_GroundSprayHerbicide_SD.EndHours;
                            RulesetList = SW_GroundSprayHerbicide_SD.RulesetSeriesList;
                        }
                        break;
                    case "sw_groundsprayvineandtree":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_GroundSprayVineAndTree.CalculateRuleSets(IRule);
                        else
                        {
                            sHours = SW_GroundSprayVineAndTree.StartHours;
                            eHours = SW_GroundSprayVineAndTree.EndHours;
                            RulesetList = SW_GroundSprayVineAndTree.RulesetSeriesList;
                        }
                        break;

                    /*French Mobile Site - New RuleSet - Begin*/
                    case "sw_ground_frcp01":
                        if (IRule.DtInput != null && IRule.DtInput.Rows.Count != 0)
                            IRule.DtOutput = SW_Ground_frcp01.CalculateRuleSets(IRule);

                        else
                        {
                            sHours = SW_Ground_frcp01.StartHours;
                            eHours = SW_Ground_frcp01.EndHours;
                            RulesetList = SW_Ground_frcp01.RulesetSeriesList;
                        }
                        break;

                    /*French Mobile Site - New RuleSet - End*/
                }
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_RULESETLOAD_FAILURE) + ex.Message.ToString();
            }
        }
        public void CreateTables(string name, string allign, string aggregation, string datasource, int start, int end)
        {
            objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

            GetcompleteSeriesList(allign);

            GetTableSeries(allign);

            Itable.dtTableSeries = Itable.dsSeriesData.Tables[0];

            //To change first column as based on the chosen unit
            Itable.dtTableSeries = objSvcPre.ChangeUnits(Itable.dtTableSeries, objSvcInfo.Unit, objSvcInfo.WUnit);

            Itable.dtTableLegends = Itable.dsSeriesData.Tables[1];
            GetSeriesData(Itable.dtTableSeries, aggregation, datasource, start, end, sHours, eHours);

            //Condition to check for the availability of the data from database
            if (Itable.dtSeries != null)
            {
                Itable.dtByDays = Itable.dtSeries;
                DataColumn dc = new DataColumn();
                dc.ColumnName = getTranslatedText(FIRSTCOLNAME, objSvcInfo.Culture);
                dc.DefaultValue = null;
                Itable.dtByDays.Columns.Add(dc);


                int LastColIndex = Itable.dtByDays.Columns.Count - 1;
                for (int i = 0; i < Itable.dtByDays.Rows.Count; i++)
                {
                    DateTime Date = Convert.ToDateTime(Itable.dtByDays.Rows[i][0].ToString());
                    Itable.dtByDays.Rows[i][LastColIndex] = Date.Hour;
                }


                Itable.alSeries = new ArrayList();

                for (int i = 1; i < LastColIndex; i++)
                {
                    Itable.alSeries.Add(Itable.dtByDays.Columns[i].ColumnName.ToString() + "," + Itable.dtTableSeries.Rows[i - 1][1].ToString() + "," + Itable.dtTableSeries.Rows[i - 1][0].ToString() + "," + Itable.dtTableSeries.Rows[i - 1][3].ToString());

                }
            }

        }
        public void GetSeriesData(DataTable series, string aggregation, string datasource, int start, int end, int startHrs, int endHrs)
        {
            Itable.dtSeries = objTblSvc.GetSeriesData(series, aggregation, datasource, start, end, startHrs, endHrs);
        }

        /* SOCB - Alignment Issue - Jerrey - SOCB*/
        public int ReadChartWidth(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                string settings = objSvc.ReadAllignment(allign, controlName);

                return 0;
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_ALIGNMENT_FAILURE) + ":" + ex.Message.ToString();
            }

            return 0;
        }
        /* EOCB - Alignment Issue - Jerrey - EOCB*/

        public void ReadAllignment(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                Itable.strAllignDetails = objSvc.ReadAllignment(allign, controlName);

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_ALIGNMENT_FAILURE) + ":" + ex.Message.ToString();
            }
        }
        public PaletteMap createPallette(string seriesPallette)
        {
            PaletteMap objPm = new PaletteMap();
            try
            {
                PalletteConstants objPalCon = new PalletteConstants();
                DataTable dt = objPalCon.getPallette(seriesPallette);
                foreach (DataRow dr in dt.Rows)
                {
                    string text = dt.Columns.Count > 2 ? dr[2].ToString() : "";
                    objPm.addLimit(Convert.ToDouble(dr[0].ToString()), dr[1].ToString(), text);
                }

                if (HttpContext.Current != null)
                {
                    Itable.pmPallette = objPm;
                }

                return objPm;

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_PALLETE_FAILURE) + " : " + ex.Message.ToString();
                return objPm;
            }
        }
        public void GetcompleteSeriesList(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                Itable.alSeriesLegend = objSvc.getNodeList(allign);

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LOADSERIES_FALURE) + " : " + ex.Message.ToString();
            }
        }
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return objTblSvc.getTranslatedText(strLabelName, strCultureCode);
        }
        public void getCultureCode()
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                Itable.strCulCode = objSvcInfo.Culture;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.CULTURE_LOADFAILURE) + " : " + ex.Message.ToString();
            }
        }
        /*IM01277709 - change in spray window. begin*/
        //added new param showAMPM
        /*IM01277709 - change in spray window. end*/
        public DataTable GenerateTransposedtblDaysRows(DataTable dt, string serie, string aggregation, bool hasRuleset, int iStep,string showAMPM)
        {
            try
            {
                int HourColIndex;
                DataTable outputTable = new DataTable();

                //WEb service Issue
                if (HttpContext.Current != null && Itable != null)
                {
                    VariantCulture = new CultureInfo(Itable.strCulCode);
                }
                else
                {
                    VariantCulture = new CultureInfo(strCulCode);
                }

                if (hasRuleset == true)
                {
                    HourColIndex = 1;
                }
                else
                {
                    HourColIndex = dt.Columns.Count - 1;
                }

                // Header row's first column is same as in inputTable

                //IM01173270 - New Agricast - Spray window - "Hour" not translatable - BEGIN
                //outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());
                outputTable.Columns.Add(objComUtil.getTransText(Constants.TAB_HOUR_COLUMN));
                //IM01173270 - New Agricast - Spray window - "Hour" not translatable - END
                if (aggregation.ToLower() == "hourly")
                {
                    // Add columns
                    for (int i = 0; i <= 23; i++)
                    {
                        DataColumn col = new DataColumn();
                        /*IM01277709 - change in spray window. begin*/
                        //col.ColumnName = i.ToString();
                        //check if the showAMPM is specified for tbldaysrows
                        //if so , append AM/PM to the hour
                        if (!string.IsNullOrEmpty(showAMPM) && showAMPM.ToLower().Equals("true"))
                        {
                            if (i < 12)
                            {
                                col.ColumnName = i.ToString() +  getTranslatedText(Constants.AM,objSvcInfo.Culture).ToUpper();
                            }
                            else
                            {
                                // hour shud be displayed in 12 hour format.
                                // 12th hour should be displayed as 12pm, 13 as 1pm and so on..
                                int j = 0;
                                if (i % 12 != 0) j = i % 12; else j = i;
                                col.ColumnName = j.ToString() + getTranslatedText(Constants.PM, objSvcInfo.Culture).ToUpper();
                            }
                        }
                        else
                        {
                            col.ColumnName = i.ToString();
                        }
                        /*IM01277709 - change in spray window. begin*/
                        outputTable.Columns.Add(col);
                    }
                }
                else
                {
                    for (int i = 0; i < 24 / iStep; i++)
                    {
                        DataColumn col = new DataColumn();
                        col.ColumnName = i.ToString();
                        if (i > 0)
                        {
                            col.ColumnName = (Convert.ToInt32(outputTable.Columns[i].ColumnName) + iStep).ToString();
                        }
                        outputTable.Columns.Add(col);
                    }


                }


                /*27-dec-2012 - glbsip03 - details error Couldn't store <1/1/2013> in date Column --Begin */
                string str = string.Empty;
                string[] strDates = null;
                /*27-dec-2012 - glbsip03 - details error Couldn't store <1/1/2013> in date Column --Begin */

                foreach (DataRow dr in dt.Rows)
                {
                    /*27-dec-2012 - glbsip03 - details error Couldn't store <1/1/2013> in date Column --Begin */
                    if (!string.IsNullOrEmpty(dr[0].ToString()))
                    {
                        //split the dates based on " " i,e split date and time
                        strDates = dr[0].ToString().Split(' ');

                        if (strDates != null && strDates.Length > 0)
                        {
                            dr[0] = strDates[0].Trim();
                        }

                        //if (dr[0].ToString().Length > 9)
                        //    /* UAT Tracker 516	glbsip03 - details error Couldn't store <10/10/000> in date Column. Begin */
                        //    /* dr[0] = dr[0].ToString().Remove(09); */
                        //    dr[0] = dr[0].ToString().Remove(10).Trim();
                        //* UAT Tracker 516	glbsip03 - details error Couldn't store <10/10/000> in date Column. Begin */
                    }
                    /*27-dec-2012 glbsip03 - details error Couldn't store <1/1/2013> in date Column --End */
                }


                /*Issue with NonHourly config for tbldaysrows - BEGIN*/
                //for (int j = 0, i = 0; j < dt.Rows.Count; j++, i++)
                for (int j = 0, i = 0; j < dt.Rows.Count; j++, i++)
                {
                    if (i >= 24 / iStep)
                        i = 0;

                    if (i == 0)
                    {
                        DataRow drow = outputTable.NewRow();

                        strDateFormat = VariantCulture.DateTimeFormat.ShortDatePattern;

                        /* IM01749293 - New Agricast - Spray Window - wrong date formatting in HU - Added by Jerrey - Begin */
                        if (VariantCulture.ToString().ToUpper() == "HU-HU")
                            strDateFormat = "dddd " + strDateFormat.Substring(4, strDateFormat.Length - 5);
                        else
                        /* IM01749293 - New Agricast - Spray Window - wrong date formatting in HU - Added by Jerrey - Begin */
                            strDateFormat = "dddd " + strDateFormat.Substring(0, strDateFormat.Length - 5);

                        ////check the culture code for "US" and change the date format
                        //if (VariantCulture.ToString().Equals("en-US", StringComparison.InvariantCultureIgnoreCase))
                        //    strDateFormat = "dddd dd/M";//day/month format for US
                        //else
                        //    strDateFormat = "dddd M/dd";//month/day format for Non US


                        drow[i] = Convert.ToDateTime(dt.Rows[j][0]).ToString(strDateFormat, VariantCulture);
                        outputTable.Rows.Add(drow);
                    }
                    int numOfRows = outputTable.Rows.Count;
                    DataRow nrow = outputTable.Rows[numOfRows - 1];
                    nrow[i + 1] = ((dt.Rows[j][serie].ToString().Contains('.')) ? (float.Parse(dt.Rows[j][serie].ToString()) > 200 ? String.Format("{0:0}", dt.Rows[j][serie]).ToString() : String.Format("{0:0.0}", dt.Rows[j][serie]).ToString()) : dt.Rows[j][serie].ToString());


                    //if ((iStep != 1 && dt.Rows[j][HourColIndex].ToString() == iStep.ToString()) || (iStep == 1 && dt.Rows[j][HourColIndex].ToString() == "0"))
                    //{
                    //    DataRow drow = outputTable.NewRow();
                    //    drow[0] = Convert.ToDateTime(dt.Rows[j][0]).ToString("dddd M/dd", VariantCulture);
                    //    drow[dt.Rows[j][HourColIndex].ToString()] = ((dt.Rows[j][serie].ToString().Contains('.')) ? String.Format("{0:0.0}", dt.Rows[j][serie]).ToString() : dt.Rows[j][serie].ToString());
                    //    //drow[1] = ((dt.Rows[j][serie].ToString().Contains('.')) ? String.Format("{0:0.0}", dt.Rows[j][serie]).ToString() : dt.Rows[j][serie].ToString());
                    //    outputTable.Rows.Add(drow);
                    //}
                    //int numOfRows = outputTable.Rows.Count;
                    //DataRow nrow = outputTable.Rows[numOfRows - 1];
                    //if (aggregation.ToLower() == "hourly")
                    //    nrow[dt.Rows[j][HourColIndex].ToString()] = ((dt.Rows[j][serie].ToString().Contains('.')) ? String.Format("{0:0.0}", dt.Rows[j][serie]).ToString() : dt.Rows[j][serie].ToString());
                    //else
                    //{
                    //    if ((j + 1) < dt.Rows.Count)
                    //        nrow[dt.Rows[j][HourColIndex].ToString()] = ((dt.Rows[j + 1][serie].ToString().Contains('.')) ? String.Format("{0:0.0}", dt.Rows[j + 1][serie]).ToString() : dt.Rows[j + 1][serie].ToString());                           
                    //}

                }

                if (serie.ToLower().Contains("winddirection"))
                {

                    for (int i = 0; i < outputTable.Rows.Count; i++)
                    {
                        for (int j = 1; j < outputTable.Columns.Count; j++)
                        {
                            //If session is not available i.e. if the call to method is from web service then set the language using web service value
                            /* Wind Direction not avaliable in web service issue - begin*/
                            //ITable will be null for web service
                            if (HttpContext.Current == null || Itable == null)
                                objComm.setWebServiceCultureCode(strCulCode);
                            int dirdeg;
                            outputTable.Rows[i][j] = objComm.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);

                        }
                    }
                }

                //Web Service Issue
                if (HttpContext.Current != null && Itable != null)
                    Itable.dt = outputTable;

                return outputTable;
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_DATAFETCH_FAILURE) + " : " + ex.Message.ToString();
                DataTable outputTable = new DataTable();
                return outputTable;
            }
        }
        public DataTable GenerateTransposedtblSeriesRows(DataTable dt, int numOfDays, string aggregation, bool hasRuleset, int iStep)
        {
            try
            {
                int HourColIndex = dt.Columns.Count - 1;
                DataTable outTable = new DataTable();
                DataTable outputTable = new DataTable();
                if (hasRuleset == true)
                {
                    HourColIndex = 1;
                }
                else
                {
                    HourColIndex = dt.Columns.Count - 1;
                }
                if (aggregation.ToLower() == "hourly")
                {

                    foreach (DataRow dro in dt.Rows)
                    {
                        // dro[0] = dro[0].ToString().Remove(9);
                        dro[0] = DateTime.Parse(dro[0].ToString()).Date;

                    }


                    DateTime nextDate = DateTime.Today;
                    if (dt.Rows.Count > 0)
                        nextDate = Convert.ToDateTime(dt.Rows[0][0].ToString()).AddDays(numOfDays);
                    if (hasRuleset)
                    {
                        if (dt.Select("day = '" + nextDate + "'").Count() > 0)//Date on onsite DB
                        {
                            outTable = dt.Select("day = '" + nextDate + "'").CopyToDataTable();


                            // Header row's first column is same as in inputTable

                            //IM01173270 - New Agricast - Spray window - "Hour" not translatable - BEGIN
                            //outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());
                            //first column is the {hour} column header text
                            // we need to get the trans text using the key "ResHoursOfDay"
                            outputTable.Columns.Add(objComUtil.getTransText(Constants.TAB_HOUR_COLUMN));
                            //IM01173270 - New Agricast - Spray window - "Hour" not translatable - END


                            for (int i = 0; i <= 23; i++)
                            {
                                DataColumn col = new DataColumn();
                                col.ColumnName = i.ToString();
                                outputTable.Columns.Add(col);

                            }

                            foreach (DataRow dr in outTable.Rows)
                            {
                                if (dr[0].ToString().Length >= 9)
                                    dr[0] = dr[0].ToString().Remove(9);
                            }
                            for (int j = 0; j < outTable.Columns.Count; j++)
                            {
                                if (j >= 1)
                                {
                                    DataRow drow = outputTable.NewRow();
                                    drow[0] = outTable.Columns[j].ColumnName;
                                    outputTable.Rows.Add(drow);

                                }
                            }

                            for (int i = 0; i < outputTable.Rows.Count; i++)
                            {

                                for (int j = 0; j < outputTable.Columns.Count; j++)
                                {
                                    if (j < outTable.Rows.Count)
                                    {
                                        outputTable.Rows[i][j + 1] = outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString().Contains('.') ? (float.Parse(outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString()) > 200 ? String.Format("{0:0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1]) : String.Format("{0:0.0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1])) : outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString();

                                    }
                                    if (j > 0)
                                    {
                                        if (outputTable.Rows[i][0].ToString().ToLower().Contains("winddirection"))
                                        {
                                            //If session is not available i.e. if the call to method is from web service then set the language using web service value
                                            /* Wind Direction not avaliable in web service issue - begin*/
                                            //ITable will be null for web service
                                            if (HttpContext.Current == null || Itable == null)
                                                objComm.setWebServiceCultureCode(strCulCode);
                                            int dirdeg;
                                            outputTable.Rows[i][j] = objComm.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);

                                        }

                                    }

                                }

                            }

                        }
                    }
                    else
                    {

                        if (dt.Select("Date = '" + nextDate + "'").Count() > 0)//Date on onsite DB
                        {
                            outTable = dt.Select("Date = '" + nextDate + "'").CopyToDataTable();


                            // Header row's first column is same as in inputTable
                            //IM01173270 - New Agricast - Spray window - "Hour" not translatable - BEGIN
                            //   outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());
                            //first column is the {hour} column header text
                            // we need to get the trans text using the key "ResHoursOfDay"
                            outputTable.Columns.Add(objComUtil.getTransText(Constants.TAB_HOUR_COLUMN));
                            //IM01173270 - New Agricast - Spray window - "Hour" not translatable - END

                            for (int i = 0; i <= 23; i++)
                            {
                                DataColumn col = new DataColumn();
                                col.ColumnName = i.ToString();
                                outputTable.Columns.Add(col);

                            }

                            foreach (DataRow dr in outTable.Rows)
                            {
                                if (dr[0].ToString().Length >= 9)
                                    dr[0] = dr[0].ToString().Remove(9);
                            }
                            for (int j = 0; j < outTable.Columns.Count - 1; j++)
                            {
                                if (j >= 1)
                                {
                                    DataRow drow = outputTable.NewRow();
                                    drow[0] = outTable.Columns[j].ColumnName;
                                    outputTable.Rows.Add(drow);

                                }
                            }

                            for (int i = 0; i < outputTable.Rows.Count; i++)
                            {

                                for (int j = 0; j < outputTable.Columns.Count; j++)
                                {
                                    if (j < outTable.Rows.Count)
                                    {
                                        // outputTable.Rows[i][j + 1] = outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString().Contains('.') ? String.Format("{0:0.0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1]) : outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString();
                                        outputTable.Rows[i][j + 1] = outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString().Contains('.') ? (float.Parse(outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString()) > 200 ? String.Format("{0:0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1]) : String.Format("{0:0.0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1])) : outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString();
                                    }
                                    if (j > 0)
                                    {
                                        if (outputTable.Rows[i][0].ToString().ToLower().Contains("winddirection"))
                                        {
                                            //If session is not available i.e. if the call to method is from web service then set the language using web service value
                                            if (HttpContext.Current == null || Itable == null)
                                                objComm.setWebServiceCultureCode(strCulCode);
                                            int dirdeg;
                                            outputTable.Rows[i][j] = objComm.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);

                                        }

                                    }

                                }

                            }

                        }
                    }
                }
                else
                {


                    // Header row's first column is same as in inputTable

                    // Header row's first column is same as in inputTable
                    //IM01173270 - New Agricast - Spray window - "Hour" not translatable - BEGIN
                    //  outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());
                    //first column is the {hour} column header text
                    // we need to get the trans text using the key "ResHoursOfDay"
                    outputTable.Columns.Add(objComUtil.getTransText(Constants.TAB_HOUR_COLUMN));
                    //IM01173270 - New Agricast - Spray window - "Hour" not translatable - END


                    // for (int i = 0; i < dt.Rows.Count; i++)
                    for (int i = 0, j = 0; i < (numOfDays * 24) / iStep; i++, j++)
                    {
                        if (i < dt.Rows.Count)
                        {
                            DataColumn col = new DataColumn();
                            col.ColumnName = dt.Rows[i][0].ToString();
                            if (j >= 24 / iStep)
                                j = 0;
                            string HourDisplay = "";
                            if (j == 0)
                                HourDisplay = "0-" + iStep;
                            else
                                HourDisplay = (j * iStep).ToString() + "-" + (((j + 1) * iStep) == 24 ? 0 : (j + 1) * iStep).ToString();

                            //if (Convert.ToDateTime(dt.Rows[i][0]).Hour == iStep)
                            //{
                            //    HourDisplay = "0-" + iStep;
                            //} 
                            //else
                            //{
                            //    int NextStep = Convert.ToInt32(Convert.ToInt32(outputTable.Columns[i].ColumnName.Substring(outputTable.Columns[i].ColumnName.LastIndexOf("-") + 1)) + Convert.ToInt32(iStep));
                            //    if (NextStep == 24)
                            //        NextStep = 0;
                            //    HourDisplay = outputTable.Columns[i].ColumnName.Substring(outputTable.Columns[i].ColumnName.LastIndexOf("-") + 1) + "-" + NextStep.ToString();
                            //    //HourDisplay = outputTable.Columns[i].ColumnName.Substring(outputTable.Columns[i].ColumnName.LastIndexOf("-") + 1) + "-" + Convert.ToInt32(Convert.ToInt32(outputTable.Columns[i].ColumnName.Substring(outputTable.Columns[i].ColumnName.LastIndexOf("-") + 1)) + Convert.ToInt32(Itable.Step));
                            //}
                            col.ColumnName = Convert.ToDateTime(dt.Rows[i][0]).ToShortDateString() + "," + HourDisplay;
                            col.Caption = HourDisplay;
                            outputTable.Columns.Add(col);
                        }

                    }
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        DataRow dr = outputTable.NewRow();
                        dr[0] = dt.Columns[i].ColumnName;
                        outputTable.Rows.Add(dr);
                    }

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        for (int j = 0; j < outputTable.Columns.Count; j++)
                        {
                            if (((j + 1) < outputTable.Columns.Count) && (i + 1) < dt.Columns.Count)

                                outputTable.Rows[i][j + 1] = ((dt.Rows[j][i + 1].ToString().Contains('.')) ? (float.Parse(dt.Rows[j][i + 1].ToString()) > 200 ? String.Format("{0:0}", dt.Rows[j][i + 1]).ToString() : String.Format("{0:0.0}", dt.Rows[j][i + 1]).ToString()) : dt.Rows[j][i + 1].ToString());
                        }
                    }

                    for (int i = 0; i < outputTable.Rows.Count; i++)
                    {
                        for (int j = 1; j < outputTable.Columns.Count; j++)
                        {
                            if (outputTable.Rows[i][0].ToString().ToLower().Contains("winddirection"))
                            {
                                //If session is not available i.e. if the call to method is from web service then set the language using web service value
                                /* Wind Direction not avaliable in web service issue - begin*/
                                //ITable will be null for web service
                                if (HttpContext.Current == null || Itable == null)
                                    objComm.setWebServiceCultureCode(strCulCode);
                                int dirdeg;
                                outputTable.Rows[i][j] = objComm.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                            }
                        }
                    }
                    if (!hasRuleset)
                        outputTable.Rows.RemoveAt(outputTable.Rows.Count - 1);

                }
                //&&  Itable!=null - added by jerrey - agricast cr 
                if (HttpContext.Current != null && Itable != null)
                    Itable.dt = outputTable;

                return outputTable;
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_DATAFETCH_FAILURE) + " : " + ex.Message.ToString();
                DataTable dtOut = new DataTable();
                return dtOut;
            }
        }
        //public void getColorRuleset(string value, string Pallette)
        //{
        //    try
        //    {
        //        PalletteConstants objPalConst = new PalletteConstants();
        //        Itable.RuleSetColor = objPalConst.getColorRuleset(value, Pallette);
        //    }

        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["ErrorMessage"] = "The ruleset color could not be fetched  due to the following error: " + ex.Message.ToString();
        //    }
        //}
        public void GetContrastColor(Color color)
        {
            try
            {
                //objComUtil = new CommonUtil();
                Itable.FontColor = objComUtil.ContrastColor(color);
            }

            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_GETCONTRAST_FAILURE) + " : " + ex.Message.ToString();
            }
        }
        protected DataTable getHeaderDetails(DataTable dtTransData, int iStep)
        {
            DataTable dtHeaders = new DataTable();
            dtHeaders.TableName = "Headers";
            dtHeaders.Columns.Add("HeaderText");
            dtHeaders.Columns.Add("Colspan");
            DataRow drHeader;
            for (int i = 0; i < dtTransData.Columns.Count; i++)
            {
                if (i > 0)
                {
                    drHeader = dtHeaders.NewRow();
                    drHeader["HeaderText"] = dtTransData.Columns[i].ColumnName;
                    drHeader["Colspan"] = 24 / iStep;
                    dtHeaders.Rows.Add(drHeader);
                }
            }

            return dtHeaders;
        }
        /*Unit Implementation in Web Services - Begin*/
        //Added a paramter strUnit
        public DataSet getTableDataForService(string allign, string name, IRuleSets IRobj, string strServiceName, string strModuleName, string strCul, string strUnit)
        /*Unit Implementation in Web Services - End*/
        {
            try
            {
                int step;
                const string THREEHOURLY = "3hourly";
                const string SIXHOURLY = "6hourly";
                const string EIGHTHOURLY = "8hourly";
                const string TWELVEHOURLY = "12hourly";
                const string HOURLY = "hourly";
                const string TBLDAYSROWS = "tblDaysRows";
                const string TBLSERIESROWS = "tblSeriesRows";
                string strDateFormat = "dddd M/dd";//Default format is  month/day

                DataSet dsTblSeries = new DataSet();
                DataTable dtTableSeries = new DataTable();
                DataTable dtSeriesData = new DataTable();
                DataSet dsTable = new DataSet();


                if (IRobj != null)
                {
                    IRule = IRobj;
                }

                strCulCode = strCul;
                objSvc.setSvcHandlerWebSvcValues(strServiceName, strModuleName);
                string strTblAttributes = objSvc.ReadAllignment(allign, name);
                int numOfDays = Convert.ToInt32(strTblAttributes.Split(',')[1]);
                string datasource = strTblAttributes.Split(',')[2];
                string aggregation = strTblAttributes.Split(',')[5];
                int start = Convert.ToInt32(strTblAttributes.Split(',')[3]);
                int end = Convert.ToInt32(strTblAttributes.Split(',')[4]);
                string ruleset = strTblAttributes.Split(',')[6];
                string pallette = strTblAttributes.Split(',')[7];
                string Nodename = strTblAttributes.Split(',')[8];

                /*IM01277709 - change in spray window. begin*/
                //if displayAMPM is specified , then the length will be 12
                string showAMPM = string.Empty;
                if (strTblAttributes.Split(',').Length == 12)
                    showAMPM = strTblAttributes.Split(',')[11];
                /*IM01277709 - change in spray window. END*/

                GetRulesetData(ruleset, IRule);
                List<string[]> alSeriesLegend = objSvc.getNodeList(allign);
                dsTblSeries = objSvc.GetTableSeries(allign, name);
                if (ruleset == string.Empty || ruleset == "")
                    dtTableSeries = dsTblSeries.Tables[0];
                else
                    dtTableSeries = RulesetList;

                /*Unit Implementation in Web Services - Begin*/
                if (dtTableSeries != null)
                {

                    dtTableSeries = objSvcPre.ChangeUnits(dtTableSeries, strUnit, string.Empty);
                }
                /*Unit Implementation in Web Services - End*/

                dtSeriesData = objTblSvc.GetSeriesData(dtTableSeries, aggregation, datasource, start, end, sHours, eHours);
                DataTable dtByDays = dtSeriesData;
                DataColumn dc = new DataColumn();
                dc.ColumnName = "";//FIRSTCOLNAME;
                dc.DefaultValue = null;
                dtByDays.Columns.Add(dc);

                if (ruleset == string.Empty || ruleset == "")
                {

                    int LastColIndex = dtByDays.Columns.Count - 1;
                    for (int i = 0; i < dtByDays.Rows.Count; i++)
                    {
                        DateTime Date = Convert.ToDateTime(dtByDays.Rows[i][0].ToString());
                        dtByDays.Rows[i][LastColIndex] = Date.Hour;
                    }


                    ArrayList alSeries = new ArrayList();

                    for (int i = 1; i < LastColIndex; i++)
                    {
                        alSeries.Add(dtByDays.Columns[i].ColumnName.ToString() + "," + dtTableSeries.Rows[i - 1][1].ToString() + "," + dtTableSeries.Rows[i - 1][0].ToString() + "," + dtTableSeries.Rows[i - 1][3].ToString());

                    }

                    if (allign.ToLower() == TBLDAYSROWS.ToLower())
                    {
                        if (aggregation.ToLower() == THREEHOURLY)
                        {
                            step = 3;
                        }
                        else if (aggregation.ToLower() == SIXHOURLY)
                        {
                            step = 6;
                        }
                        else if (aggregation.ToLower() == EIGHTHOURLY)
                        {
                            step = 8;
                        }
                        else if (aggregation.ToLower() == TWELVEHOURLY)
                        {
                            step = 12;
                        }
                        else if (aggregation.ToLower() == HOURLY)
                        {
                            step = 1;
                        }
                        else
                        {
                            step = 24;
                        }



                        foreach (string ser in alSeries)
                        {
                            string seriesPallette = ser.Split(',')[1].ToString();
                            string seriesName = ser.Split(',')[0].ToString();
                            string serie = ser.Split(',')[2].ToString();
                            /*IM01277709 - change in spray window. begin*/
                            //DataTable dtTrans = GenerateTransposedtblDaysRows(dtByDays, seriesName, aggregation, false, step);
                            DataTable dtTrans = GenerateTransposedtblDaysRows(dtByDays, seriesName, aggregation, false, step,showAMPM);
                            /*IM01277709 - change in spray window. end*/
                            if (dsTable.Tables.Count == 0 && step != 1)
                            {
                                dsTable.Tables.Add(getHeaderDetails(dtTrans, step));
                            }
                            PaletteMap objPm;
                            DataTable dtColor = new DataTable();
                            DataTable dt = new DataTable();
                            dt.TableName = getTranslatedText(ser.Split(',')[3], strCulCode); //"tbl" + seriesName;
                            DataRow dr;
                            if (seriesPallette.ToString() != "" && seriesPallette.ToLower() != "none")
                            {
                                objPm = createPallette(seriesPallette);

                                for (int k = 0; k < dtTrans.Rows.Count; k++)
                                {
                                    dr = dtColor.NewRow();
                                    for (int j = 0; j < dtTrans.Columns.Count; j++)
                                    {
                                        if (j > 0)
                                        {
                                            //Web service issue for French MobileSite - BEGIN*/
                                            //Color should always be added irrespective of whether the data is present or not.
                                            //"IF" block moved out of the null check statements.
                                            if (k == 0)
                                            {
                                                DataColumn dcColor = new DataColumn();
                                                dcColor.ColumnName = "Color" + (j - 1).ToString();
                                                dtColor.Columns.Add(dcColor);
                                            }
                                            //Web service issue for French MobileSite - END*/

                                            if (dtTrans.Rows[k][j].ToString() != "" && dtTrans.Rows[k][j].ToString() != "&nbsp;")
                                            {

                                                dr["Color" + (j - 1).ToString()] = objPm.getColor(Convert.ToDouble(dtTrans.Rows[k][j].ToString()), seriesName);
                                            }
                                        }

                                    }
                                    dtColor.Rows.Add(dr);
                                }
                            }
                            DataRow drTbl;
                            for (int m = 0; m < dtTrans.Rows.Count; m++)
                            {
                                drTbl = dt.NewRow();
                                for (int n = 0; n < (dtTrans.Columns.Count); n++)
                                {
                                    if (m == 0)
                                    {
                                        DataColumn dcVal = new DataColumn();
                                        DataColumn dcToolTip = new DataColumn();
                                        DataColumn dcColor = new DataColumn();
                                        dcVal.ColumnName = "Value" + n.ToString();
                                        dcToolTip.ColumnName = "ToolTip" + n.ToString();
                                        dcColor.ColumnName = "Color" + n.ToString();
                                        dt.Columns.Add(dcVal);
                                        dt.Columns.Add(dcToolTip);
                                        dt.Columns.Add(dcColor);
                                    }
                                    /* //Web service issue for French MobileSite - WS not working for non hourly configuration - BEGIN*/

                                    // drTbl["Value" + n.ToString()] = n == 0 ? dtTrans.Rows[m][n] : dtTrans.Rows[m][(n - 1).ToString()];
                                    drTbl["Value" + n.ToString()] = n == 0 ? dtTrans.Rows[m][n] : dtTrans.Rows[m][((n - 1) * step).ToString()];
                                    /*French mobile Site Web Service issue - WS not working for non hourly configuration - BEGIN*/

                                    drTbl["ToolTip" + n.ToString()] = "";
                                    if (dtColor.Rows.Count > 0)
                                        drTbl["Color" + n.ToString()] = n == 0 ? "" : dtColor.Rows[m]["Color" + (n - 1).ToString()];

                                    else
                                        drTbl["Color" + n.ToString()] = "";
                                }
                                dt.Rows.Add(drTbl);
                            }

                            //else
                            //{
                            //    dt = dtTrans;
                            //    dt.TableName = "tbl" + seriesName;
                            //}

                            dsTable.Tables.Add(dt);
                        }
                    }

                    else if (allign.ToLower() == TBLSERIESROWS.ToLower())
                    {
                        DataTable dtTransData = new DataTable();
                        PaletteMap objPm;

                        DataRow drColSeries;
                        //gvDynamic = new GridView();
                        if (aggregation.ToLower() != HOURLY)
                        {

                            if (aggregation.ToLower() == THREEHOURLY)
                            {
                                step = 3;
                            }
                            else if (aggregation.ToLower() == SIXHOURLY)
                            {
                                step = 6;
                            }
                            else if (aggregation.ToLower() == EIGHTHOURLY)
                            {
                                step = 8;
                            }
                            else if (aggregation.ToLower() == TWELVEHOURLY)
                            {
                                step = 12;
                            }
                            else
                            {
                                step = 24;
                            }
                            dtTransData = GenerateTransposedtblSeriesRows(dtByDays, numOfDays, aggregation, false, step);

                            if (dsTable.Tables.Count == 0 && step != 1)
                            {
                                dsTable.Tables.Add(getHeaderDetails(dtTransData, step));
                            }
                            string seriesPallette = "";
                            DataTable dtColorSeries = new DataTable();
                            for (int i = 0; i < dtTransData.Rows.Count; i++)
                            {
                                drColSeries = dtColorSeries.NewRow();
                                string serieName = dtTransData.Rows[i][0].ToString();
                                for (int j = 0; j < dtTransData.Columns.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        dtTransData.Rows[i][0] = getTranslatedText(alSeries[i].ToString().Split(',')[3], strCulCode);

                                    }

                                    if (j > 0)
                                    {
                                        foreach (string ser in alSeries)
                                        {
                                            if (ser.Split(',')[0].ToString() == serieName)
                                            {
                                                seriesPallette = ser.Split(',')[1].ToString();
                                                break;
                                            }

                                        }
                                        if (seriesPallette.ToString() != "" && seriesPallette.ToLower() != "none")
                                        {
                                            objPm = createPallette(seriesPallette);

                                            //Web service issue for French MobileSite - BEGIN*/
                                            //Changes made - the block below was moved out of if condition
                                            if (i == 0)
                                            {
                                                DataColumn dcColorSeries = new DataColumn();
                                                dcColorSeries.ColumnName = "Color" + j.ToString();
                                                dtColorSeries.Columns.Add(dcColorSeries);
                                            }
                                            //Web service issue for French MobileSite - END*/

                                            if (dtTransData.Rows[i][j].ToString() != "" && dtTransData.Rows[i][j].ToString() != "&nbsp;")
                                            {

                                                drColSeries["Color" + j.ToString()] = objPm.getColor(Convert.ToDouble(dtTransData.Rows[i][j].ToString()), dtTransData.Rows[i][0].ToString());

                                            }
                                        }
                                    }
                                }

                                dtColorSeries.Rows.Add(drColSeries);
                            }
                            DataTable dtTblSeries = new DataTable();
                            //dtTblSeries.TableName = "tbl_" + seriesName;
                            DataRow drTblSeries;
                            for (int m = 0; m < dtTransData.Rows.Count; m++)
                            {
                                drTblSeries = dtTblSeries.NewRow();
                                for (int n = 0; n < (dtTransData.Columns.Count); n++)
                                {
                                    if (m == 0)
                                    {
                                        DataColumn dcVal = new DataColumn();
                                        DataColumn dcToolTip = new DataColumn();
                                        DataColumn dcColor = new DataColumn();
                                        dcVal.ColumnName = "Value" + n.ToString();
                                        dcToolTip.ColumnName = "ToolTip" + n.ToString();
                                        dcColor.ColumnName = "Color" + n.ToString();
                                        dtTblSeries.Columns.Add(dcVal);
                                        dtTblSeries.Columns.Add(dcToolTip);
                                        dtTblSeries.Columns.Add(dcColor);
                                    }
                                    //if (n > 0)
                                    {
                                        drTblSeries["Value" + n.ToString()] = dtTransData.Rows[m][n];
                                        drTblSeries["ToolTip" + n.ToString()] = "";
                                        drTblSeries["Color" + n.ToString()] = n == 0 ? "" : dtColorSeries.Rows[m]["Color" + (n).ToString()];
                                    }
                                }
                                dtTblSeries.Rows.Add(drTblSeries);
                            }

                            dsTable.Tables.Add(dtTblSeries);
                        }

                        else
                        {
                            int k = 0;
                            int istep = 1;
                            while (k <= numOfDays)
                            {
                                if (start > end)
                                {
                                    int tmp = start;
                                    start = end;
                                    end = tmp;
                                }

                                dtTransData = GenerateTransposedtblSeriesRows(dtByDays, k, aggregation, false, istep);
                                if (dsTable.Tables.Count == 0)
                                {
                                    dsTable.Tables.Add(getHeaderDetails(dtTransData, istep));
                                }

                                string seriesPallette = "";
                                DataTable dtColorSeries = new DataTable();
                                for (int i = 0; i < dtTransData.Rows.Count; i++)
                                {
                                    drColSeries = dtColorSeries.NewRow();
                                    string serieName = dtTransData.Rows[i][0].ToString();
                                    for (int j = 0; j <= 24; j++)
                                    {
                                        if (j == 0)
                                        {
                                            dtTransData.Rows[i][0] = getTranslatedText(alSeries[i].ToString().Split(',')[3], strCulCode);

                                        }

                                        if (j > 0)
                                        {
                                            foreach (string ser in alSeries)
                                            {
                                                if (ser.Split(',')[0].ToString() == serieName)
                                                {
                                                    seriesPallette = ser.Split(',')[1].ToString();
                                                    break;
                                                }

                                            }
                                            if (seriesPallette.ToString() != "" && seriesPallette.ToLower() != "none")
                                            {
                                                objPm = createPallette(seriesPallette);
                                                if (dtTransData.Rows[i][j].ToString() != "" && dtTransData.Rows[i][j].ToString() != "&nbsp;")
                                                {
                                                    if (i == 0)
                                                    {
                                                        DataColumn dcColorSeries = new DataColumn();
                                                        dcColorSeries.ColumnName = "Color" + j.ToString();
                                                        dtColorSeries.Columns.Add(dcColorSeries);
                                                    }

                                                    drColSeries["Color" + j.ToString()] = objPm.getColor(Convert.ToDouble(dtTransData.Rows[i][j].ToString()), dtTransData.Rows[i][0].ToString());

                                                }
                                            }
                                        }
                                    }
                                    dtColorSeries.Rows.Add(drColSeries);
                                }
                                DataTable dtTblSeries = new DataTable();
                                //dtTblSeries.TableName = "tbl_" + seriesName;
                                DataRow drTblSeries;
                                for (int m = 0; m < dtTransData.Rows.Count; m++)
                                {
                                    drTblSeries = dtTblSeries.NewRow();
                                    for (int n = 0; n < (dtTransData.Columns.Count - 1); n++)
                                    {
                                        if (m == 0)
                                        {
                                            DataColumn dcVal = new DataColumn();
                                            DataColumn dcColor = new DataColumn();
                                            DataColumn dcToolTip = new DataColumn();
                                            dcVal.ColumnName = "Value" + n.ToString();
                                            dcToolTip.ColumnName = "ToolTip" + n.ToString();
                                            dcColor.ColumnName = "Color" + n.ToString();
                                            dtTblSeries.Columns.Add(dcVal);
                                            dtTblSeries.Columns.Add(dcToolTip);
                                            dtTblSeries.Columns.Add(dcColor);
                                        }
                                        drTblSeries["Value" + n.ToString()] = dtTransData.Rows[m][n.ToString()];
                                        drTblSeries["ToolTip" + n.ToString()] = "";
                                        drTblSeries["Color" + n.ToString()] = dtColorSeries.Rows[m]["Color" + (n + 1).ToString()];
                                    }
                                    dtTblSeries.Rows.Add(drTblSeries);
                                }
                                dsTable.Tables.Add(dtTblSeries);
                                k++;

                            }

                        }

                    }

                    return dsTable;
                }
                else
                {
                    PaletteMap objPm;
                    DataTable dtColorTooltip = new DataTable();
                    DataTable dtRuleSetData = new DataTable();
                    DataTable dtRSdata = new DataTable();
                    DataRow drRColor;
                    int LastColIndex = dtByDays.Columns.Count - 1;
                    for (int i = 0; i < dtByDays.Rows.Count; i++)
                    {
                        DateTime Date = Convert.ToDateTime(dtByDays.Rows[i][0].ToString());
                        dtByDays.Rows[i][LastColIndex] = Date.Hour;
                    }
                    IRule.DtInput = dtByDays;
                    if (start < end)
                    {
                        IRule.StartDate = System.DateTime.Today.AddDays(start);
                        IRule.EndDate = System.DateTime.Today.AddDays(end);
                    }
                    else
                    {
                        IRule.StartDate = System.DateTime.Today.AddDays(end);
                        IRule.EndDate = System.DateTime.Today.AddDays(start);

                    }

                    GetRulesetData(ruleset, IRobj);

                    //Itable.alSeries = new ArrayList();
                    ArrayList alSeries = new ArrayList();
                    for (int i = 1; i < LastColIndex; i++)
                    {
                        if (ruleset == string.Empty || ruleset == "")
                            alSeries.Add(dtByDays.Columns[i].ColumnName.ToString() + "," + dtTableSeries.Rows[i - 1][1].ToString() + "," + dtTableSeries.Rows[i - 1][0].ToString() + "," + dtTableSeries.Rows[i - 1][3].ToString());
                        else
                            alSeries.Add(dtByDays.Columns[i].ColumnName.ToString() + "," + dtTableSeries.Rows[i - 1][0].ToString());
                    }
                    if (allign.ToLower() == TBLDAYSROWS.ToLower())
                    {
                        /*IM01277709 - change in spray window. begin*/
                        //DataTable dtRulesetTrans = GenerateTransposedtblDaysRows(IRule.DtOutput, "value", aggregation, true, 1,showAMPM);
                        DataTable dtRulesetTrans = GenerateTransposedtblDaysRows(IRule.DtOutput, "value", aggregation, true, 1, showAMPM);
                        /*IM01277709 - change in spray window. end*/

                        objPm = createPallette(pallette);

                        for (int k = 0; k < dtRulesetTrans.Rows.Count; k++)
                        {
                            drRColor = dtColorTooltip.NewRow();
                            for (int j = 0; j < dtRulesetTrans.Columns.Count; j++)
                            {
                                if (j > 0)
                                {
                                    if (k == 0)
                                    {
                                        DataColumn dcColor = new DataColumn();
                                        dcColor.ColumnName = "Color" + (j - 1).ToString();
                                        dtColorTooltip.Columns.Add(dcColor);
                                        DataColumn dcToolTip = new DataColumn();
                                        dcToolTip.ColumnName = "ToolTip" + (j - 1).ToString();
                                        dtColorTooltip.Columns.Add(dcToolTip);
                                    }

                                    strDateFormat = VariantCulture.DateTimeFormat.ShortDatePattern;

                                    strDateFormat = "dddd " + strDateFormat.Substring(0, strDateFormat.Length - 5);

                                    ////check the culture code for "US" and change the date format
                                    //if (VariantCulture.ToString().Equals("en-US", StringComparison.InvariantCultureIgnoreCase))
                                    //    strDateFormat = VariantCulture.dateTimeInfo.ShortDatePattern.ToString().Substring(0, 5);//day/month format for US
                                    //else
                                    //    strDateFormat = "dddd M/dd";//month/day format for Non US

                                    var queryColor = from row in IRule.DtOutput.AsEnumerable()
                                                     where row.Field<string>("Day").Trim() == DateTime.ParseExact(HttpUtility.HtmlDecode(dtRulesetTrans.Rows[k][0].ToString()), strDateFormat, VariantCulture).ToShortDateString()
                                                                 && row.Field<string>("Hour").ToString(VariantCulture) == dtRulesetTrans.Columns[j].ColumnName
                                                     select row.Field<string>("ColorCode");
                                    if (queryColor.ToList().Count > 0 && queryColor.ToList()[0] != null && queryColor.ToList()[0] != "")
                                    {
                                        drRColor["Color" + (j - 1).ToString()] = objPm.getColor(Convert.ToDouble(queryColor.ToList()[0]), name);
                                    }


                                    var queryRestriction = from row in IRule.DtOutput.AsEnumerable()
                                                           where row.Field<string>("Day").Trim() == DateTime.ParseExact(HttpUtility.HtmlDecode(dtRulesetTrans.Rows[k][0].ToString()), strDateFormat, VariantCulture).ToShortDateString()
                                                    && row.Field<string>("Hour").ToString(VariantCulture) == dtRulesetTrans.Columns[j].ColumnName
                                                           select row.Field<string>("restrictions");


                                    if (queryRestriction.ToList().Count > 0 && queryRestriction.ToList()[0] != null)
                                    {
                                        string[] ToolTips = queryRestriction.ToList()[0].Split(',');
                                        string ToolTip = "";
                                        foreach (string restriction in ToolTips)
                                        {
                                            ToolTip = ToolTip + getTranslatedText(restriction, strCulCode) + ",";
                                        }
                                        ToolTip = ToolTip.Remove(ToolTip.LastIndexOf(','));

                                        drRColor["ToolTip" + (j - 1).ToString()] = ToolTip;
                                        //dtRulesetTrans.Rows[k][j].ToolTip = ToolTip.Remove(ToolTip.LastIndexOf(','));
                                    }
                                }

                            }
                            dtColorTooltip.Rows.Add(drRColor);
                        }
                        DataRow drTbl;
                        for (int m = 0; m < dtRulesetTrans.Rows.Count; m++)
                        {
                            drTbl = dtRSdata.NewRow();

                            if (m == 0)
                            {
                                DataColumn dcDate = new DataColumn();
                                dcDate.ColumnName = "Date";
                                dtRSdata.Columns.Add(dcDate);
                            }


                            for (int n = 0; n < (dtRulesetTrans.Columns.Count); n++)
                            {
                                if (m == 0)
                                {
                                    DataColumn dcVal = new DataColumn();
                                    DataColumn dcToolTip = new DataColumn();
                                    DataColumn dcColor = new DataColumn();
                                    dcVal.ColumnName = "Value" + n.ToString();
                                    dcToolTip.ColumnName = "ToolTip" + n.ToString();
                                    dcColor.ColumnName = "Color" + n.ToString();
                                    if (n != 0)
                                        dtRSdata.Columns.Add(dcVal);
                                    dtRSdata.Columns.Add(dcToolTip);
                                    dtRSdata.Columns.Add(dcColor);
                                }
                                if (n == 0)
                                {
                                    drTbl["Date"] = dtRulesetTrans.Rows[m][n];
                                    drTbl["ToolTip" + n.ToString()] = "";
                                    drTbl["Color" + n.ToString()] = "";
                                }
                                else
                                {
                                    drTbl["Value" + n.ToString()] = dtRulesetTrans.Rows[m][(n - 1).ToString()];
                                    drTbl["ToolTip" + n.ToString()] = dtColorTooltip.Rows[m]["ToolTip" + (n - 1).ToString()];
                                    drTbl["Color" + n.ToString()] = dtColorTooltip.Rows[m]["Color" + (n - 1).ToString()];
                                }
                            }
                            dtRSdata.Rows.Add(drTbl);
                        }
                        dsTable.Tables.Add(dtRSdata);
                    }
                    return dsTable;
                }

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                throw ex;
            }

        }
        public DataTable getTableDataForExcelExport(string allign, string name, IRuleSets IRobj)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                strCulCode = objSvcInfo.Culture;
                PaletteMap objPm;
                DataSet dsTblSeries = new DataSet();
                DataTable dtTableSeries = new DataTable();
                DataTable dtSeriesData = new DataTable();
                DataSet dsTable = new DataSet();

                DataTable dt;

                string strTblAttributes = objSvc.ReadAllignment(allign, name);
                int numOfDays = Convert.ToInt32(strTblAttributes.Split(',')[1]);
                string datasource = strTblAttributes.Split(',')[2];
                string aggregation = strTblAttributes.Split(',')[5];
                int start = Convert.ToInt32(strTblAttributes.Split(',')[3]);
                int end = Convert.ToInt32(strTblAttributes.Split(',')[4]);
                //To handle current date
                //end = (end > 0 ? end - 1 : end + 1);
                string ruleset = strTblAttributes.Split(',')[6];
                string pallette = strTblAttributes.Split(',')[7];
                string Nodename = strTblAttributes.Split(',')[8];
                List<string[]> alSeriesLegend = objSvc.getNodeList(allign);
                dsTblSeries = objSvc.GetTableSeries(allign, name);
                dtTableSeries = dsTblSeries.Tables[0];
                //To change first column as based on the chosen unit
                dtTableSeries = objSvcPre.ChangeUnits(dtTableSeries, objSvcInfo.Unit, objSvcInfo.WUnit);

                if (ruleset == string.Empty || ruleset == "")
                {
                    dtSeriesData = objTblSvc.GetSeriesData(dtTableSeries, aggregation, datasource, start, end, sHours, eHours);


                    dt = dtSeriesData.Copy();
                    dt.TableName = getTranslatedText(Nodename, strCulCode);
                    dt.Columns[0].ColumnName = getTranslatedText(dt.Columns[0].ColumnName, strCulCode);
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        int count = 1;
                        string ColName = getTranslatedText(dtTableSeries.Rows[i - 1]["trnsTag"].ToString(), strCulCode);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {

                            if (dt.Columns[j].ColumnName == ColName)
                            {
                                ColName = ColName + "_" + count.ToString();
                            }

                        }

                        dt.Columns[i].ColumnName = ColName;

                    }
                    return dt;
                }
                else
                {

                    if (IRobj != null)
                    {
                        IRule = IRobj;
                    }
                    IRule.DtInput.Clear();
                    IRule.DtOutput.Clear();
                    GetRulesetData(ruleset, IRule);
                    dtSeriesData = objTblSvc.GetSeriesData(RulesetList, aggregation, datasource, start, end, sHours, eHours);


                    dt = dtSeriesData.Copy();
                    dt.TableName = name;
                    dt.Columns.Add("Hour");
                    int LastColIndex = dt.Columns.Count - 1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DateTime Date = Convert.ToDateTime(dt.Rows[i][0].ToString());
                        dt.Rows[i][LastColIndex] = Date.Hour;
                    }
                    IRule.DtInput = dt;

                    IRule.StartDate = System.DateTime.Today.AddDays(start);
                    IRule.EndDate = System.DateTime.Today.AddDays(end);
                    if (IRule.StartDate > IRule.EndDate)
                    {
                        DateTime tmp = IRule.StartDate;
                        IRule.StartDate = IRule.EndDate;
                        IRule.EndDate = tmp;
                    }
                    //if (start < end)
                    //{
                    //    IRule.StartDate = System.DateTime.Today.AddDays(start);
                    //    IRule.EndDate = System.DateTime.Today.AddDays(end);
                    //}
                    //else
                    //{
                    //    IRule.StartDate = System.DateTime.Today.AddDays(end);
                    //    IRule.EndDate = System.DateTime.Today.AddDays(start);

                    //}

                    GetRulesetData(ruleset, IRule);
                    DataTable dtOut = IRule.DtOutput.Copy();


                    //dtOut.Rows.Add(DateTime.Today.ToShortDateString(), "1", "2", "X","ResLegRain,ResLegWind");
                    //dtOut.Rows.Add(DateTime.Today.ToShortDateString(), "1", "2", "X","ResLegRain,ResLegWind");


                    //create pallette
                    objPm = createPallette(pallette);


                    for (int i = 0; i < dtOut.Rows.Count; i++)
                    {

                        //get the recommendation text in case it exists
                        //if not, get the color  name
                        if (dtOut.Rows[i][2].ToString() != string.Empty)
                            dtOut.Rows[i][2] = objPm.getColor(double.Parse(dtOut.Rows[i][2].ToString()), name, "excel") == string.Empty ? System.Drawing.ColorTranslator.FromHtml(objPm.getColor(double.Parse(dtOut.Rows[i][2].ToString()), name)).Name : getTranslatedText(objPm.getColor(double.Parse(dtOut.Rows[i][2].ToString()), name, "excel"), strCulCode);


                        //get the restrictions
                        if (dtOut.Rows[i][3].ToString() != string.Empty)
                        {
                            //Get the Restrictions
                            string[] RestrictionLists = null;
                            if (!string.IsNullOrEmpty(dtOut.Rows[i][4].ToString()))
                            {
                                RestrictionLists = dtOut.Rows[i][4].ToString().Split(',');

                                string restriction = "";
                                foreach (string restrictionText in RestrictionLists)
                                {
                                    restriction = restriction + getTranslatedText(restrictionText, strCulCode) + ",";
                                }
                                dtOut.Rows[i][4] = restriction.Remove(restriction.LastIndexOf(','));
                            }
                        }

                    }

                    //remove the value column as it shoudnt be displayed in the excel
                    dtOut.Columns.Remove("value");
                    dtOut.AcceptChanges();

                    //Change the column names as per culture selected
                    for (int j = 0; j < dtOut.Columns.Count; j++)
                    {
                        dtOut.Columns[j].ColumnName = dtOut.Columns[j].ColumnName != "ColorCode" ? getTranslatedText(dtOut.Columns[j].ColumnName.ToString(), strCulCode) : getTranslatedText("Recommendation", strCulCode);
                    }



                    //Set the table name that comes up as sheet name
                    dtOut.TableName = getTranslatedText(Nodename, strCulCode);
                    return dtOut;
                }

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                return null;
            }
        }
        public string checkUnitPalette(string seriePalette)
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
            {

                objSvcInfo = new ServiceInfo();
            }
            else
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            }
            if (objSvcInfo.Unit.ToLower() == "metric")
            {
                switch (seriePalette)
                {
                    case "pal_sip_temperaturef": seriePalette = "pal_sip_temperature";
                        break;
                    case "pal_sip_raininch_8hour": seriePalette = "pal_sip_rain_8hour";
                        break;
                    case "pal_sip_raininch": seriePalette = "pal_sip_rain";
                        break;
                    case "pal_winspeedmph": seriePalette = "pal_winspeedkmh";
                        break;

                }
            }
            else if (objSvcInfo.Unit.ToLower() == "imperial")
            {
                switch (seriePalette)
                {
                    case "pal_sip_temperature": seriePalette = "pal_sip_temperaturef";
                        break;
                    case "pal_sip_rain_8hour": seriePalette = "pal_sip_raininch_8hour";
                        break;
                    case "pal_sip_rain": seriePalette = "pal_sip_raininch";
                        break;
                    case "pal_winspeedkmh": seriePalette = "pal_winspeedmph";
                        break;

                }
            }
            if (objSvcInfo.WUnit.ToLower() == "beaufort")
            {
                switch (seriePalette)
                {
                    case "pal_winspeedkmh": seriePalette = "pal_winspeedbft";
                        break;
                    case "pal_winspeedmph": seriePalette = "pal_winspeedbft";
                        break;

                }
            }

            return seriePalette;
        }

    }


}
