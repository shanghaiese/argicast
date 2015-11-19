using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.AgriInfo.Service;
using Syngenta.AgriCast.AgriInfo.View;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using System.Collections;
using System.Web.SessionState;
using System.Data;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;

namespace Syngenta.AgriCast.AgriInfo.Presenter
{
    public class AgriInfoPresenter
    {
        IAgriInfo IWeather;
        ServiceInfo objSvcInfo;
        string controlName;
        AgriInfoService agriSvc = new AgriInfoService();
        ServiceHandler ServiceObj = new ServiceHandler();
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil objCommonUtil = new CommonUtil();

        public AgriInfoPresenter(IAgriInfo IAgri, string Name)
        {
            if (IAgri != null)
            {
                IWeather = IAgri;
            }
            controlName = Name;
        }

        public AgriInfoPresenter()
        {
        }
        public void getTranslatedText(string strLabelName, string strCultureCode)
        {
            IWeather.strTranslatedText = agriSvc.getTranslatedText(strLabelName, strCultureCode);
        }
        public void readDate()
        {
            try
            {
                string strDate = ServiceObj.readDate() + "-" + DateTime.Today.Year;
                DateTime date;

                //start of changes - 2013/03/12 
                //IWeather.StartDate = DateTime.TryParse(strDate, out date) ? date : DateTime.Today;
                IWeather.StartDate = (DateTime.TryParse(strDate, out date) ? date : DateTime.Today).AddMonths(-6);
                //end of changes - 2013/03/12 
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GEN_LOADSERIES_FALURE) + ex.Message.ToString();
            }

        }

        public void loadSeries()
        {
            try
            {
                IWeather.ds = ServiceObj.loadSeries();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.AGRIINFO_LOADSERIES_FALURE) + ex.Message.ToString();
            }

        }
        public void LoadGddSeries()
        {
            IWeather.dtGdd = ServiceObj.LoadGddSeries();
        }
        public void getSessionDetails()
        {
            try
            {
                HttpSessionState Session = HttpContext.Current.Session;
                ArrayList ar = new ArrayList();
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
                ar.Add(objSvcInfo.ServiceName.ToString());
                ar.Add(objSvcInfo.Module.ToString());
                ar.Add(objSvcInfo.Culture.ToString());
                IWeather.ar = ar;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GEN_SESSIONDETAILS_FAILURE) + ";" + ex.Message.ToString();
            }

        }
        //public DataTable ReadSeriesDetails(string[] seriesList)
        //{

        //    DataTable dtSeries = new DataTable();
        //    dtSeries.Columns.Add("Name");
        //    dtSeries.Columns.Add("aggregationfunction");
        //    dtSeries.Columns.Add("markerType");
        //    dtSeries.Columns.Add("axisPosition");
        //    dtSeries.Columns.Add("stacked");
        //    dtSeries.Columns.Add("gallery");
        //    dtSeries.Columns.Add("color");
        //    dtSeries.Columns.Add("panel");
        //    foreach (string SerieName in seriesList)
        //    {
        //        DataRow dr = dtSeries.NewRow();

        //        if (SerieName.StartsWith("daily"))
        //        {
        //            string serie = SerieName.Substring(9);
        //            var aggFunction = from row in IWeather.dtDaily.AsEnumerable()
        //                              where row.Field<string>("name").Trim() == serie
        //                              select new
        //                              {
        //                                  aggregationFunction = row.Field<string>("aggregationFunction"),
        //                                  markerType = row.Field<string>("markerType"),
        //                                  axisPosition = row.Field<string>("axisPosition"),
        //                                  stacked = row.Field<bool>("stacked"),
        //                                  gallery = row.Field<string>("gallery"),
        //                                  color = row.Field<string>("color")
        //                              };




        //            dr[0] = serie;
        //            dr[1] = aggFunction.ToList()[0].aggregationFunction;
        //            dr[2] = aggFunction.ToList()[0].markerType;
        //            dr[3] = aggFunction.ToList()[0].axisPosition;
        //            dr[4] = aggFunction.ToList()[0].stacked;
        //            dr[5] = aggFunction.ToList()[0].gallery;
        //            dr[6] = aggFunction.ToList()[0].color;
        //            dr[7] = 1;
        //            dtSeries.Rows.Add(dr);
        //        }
        //        else if (SerieName.StartsWith("extreme"))
        //        {
        //            string serie = SerieName.Substring(11);
        //            var aggFunction = from row in IWeather.dtExtreme.AsEnumerable()
        //                              where row.Field<string>("name").Trim() == serie
        //                              select new
        //                              {
        //                                  aggregationFunction = row.Field<string>("aggregationFunction"),
        //                                  markerType = row.Field<string>("markerType"),
        //                                  axisPosition = row.Field<string>("axisPosition"),
        //                                  stacked = row.Field<bool>("stacked"),
        //                                  gallery = row.Field<string>("gallery"),
        //                                  color = row.Field<string>("color")
        //                              };

        //            dr[0] = serie;
        //            dr[1] = aggFunction.ToList()[0].aggregationFunction;
        //            dr[2] = aggFunction.ToList()[0].markerType;
        //            dr[3] = aggFunction.ToList()[0].axisPosition;
        //            dr[4] = aggFunction.ToList()[0].stacked;
        //            dr[5] = aggFunction.ToList()[0].gallery;
        //            dr[6] = aggFunction.ToList()[0].color;
        //            dr[7] = 2;
        //            dtSeries.Rows.Add(dr);
        //        }
        //        else if (SerieName.StartsWith("longterm"))
        //        {
        //            string serie = SerieName.Substring(12);
        //            var aggFunction = from row in IWeather.dtLongTerm.AsEnumerable()
        //                              where row.Field<string>("name").Trim() == serie
        //                              select new
        //                              {
        //                                  aggregationFunction = row.Field<string>("aggregationFunction"),
        //                                  markerType = row.Field<string>("markerType"),
        //                                  axisPosition = row.Field<string>("axisPosition"),
        //                                  stacked = row.Field<bool>("stacked"),
        //                                  gallery = row.Field<string>("gallery"),
        //                                  color = row.Field<string>("color")
        //                              };

        //            dr[0] = serie;
        //            dr[1] = aggFunction.ToList()[0].aggregationFunction;
        //            dr[2] = aggFunction.ToList()[0].markerType;
        //            dr[3] = aggFunction.ToList()[0].axisPosition;
        //            dr[4] = aggFunction.ToList()[0].stacked;
        //            dr[5] = aggFunction.ToList()[0].gallery;
        //            dr[6] = aggFunction.ToList()[0].color;
        //            dr[7] = 3;
        //            dtSeries.Rows.Add(dr);
        //        }
        //         else if (SerieName.StartsWith("gdd"))
        //        {
        //            string serie = SerieName.Substring(7);
        //            var aggFunction = from row in IWeather.dtGdd.AsEnumerable()
        //                              where row.Field<string>("name").Trim() == serie
        //                              select new
        //                              {
        //                                  aggregationFunction = row.Field<string>("aggregationFunction"),
        //                                  markerType = row.Field<string>("markerType"),
        //                                  axisPosition = row.Field<string>("axisPosition"),
        //                                  stacked = row.Field<bool>("stacked"),
        //                                  gallery = row.Field<string>("gallery"),
        //                                  color = row.Field<string>("color")
        //                              };

        //            dr[0] = serie;
        //            dr[1] = aggFunction.ToList()[0].aggregationFunction;
        //            dr[2] = aggFunction.ToList()[0].markerType;
        //            dr[3] = aggFunction.ToList()[0].axisPosition;
        //            dr[4] = aggFunction.ToList()[0].stacked;
        //            dr[5] = aggFunction.ToList()[0].gallery;
        //            dr[6] = aggFunction.ToList()[0].color;
        //            dr[7] = 4;
        //            dtSeries.Rows.Add(dr);
        //        }

        //    }
        //    dtSeries.TableName = "ChartSerie";
        //    return dtSeries;
        //}
        public DataSet loadSeriesExcel()
        {
            return ServiceObj.loadSeries();

        }
        public DataTable LoadGddSeriesExcel()
        {
            return ServiceObj.LoadGddSeries();
        }
        public DataTable ReadSeriesDetailsExcel(string[] seriesList)
        {

            DataTable dtSeries = new DataTable();
            try
            {
                dtSeries.Columns.Add("Name");
                dtSeries.Columns.Add("trnsTag");
                dtSeries.Columns.Add("aggregationfunction");
                dtSeries.Columns.Add("markerType");
                dtSeries.Columns.Add("axisPosition");
                dtSeries.Columns.Add("stacked");
                dtSeries.Columns.Add("gallery");
                dtSeries.Columns.Add("color");
                dtSeries.Columns.Add("panel");
                dtSeries.Columns.Add("title");
                DataSet ds = loadSeriesExcel();

                DataTable dtGdd = LoadGddSeriesExcel();
                foreach (string SerieName in seriesList)
                {
                    DataRow dr = dtSeries.NewRow();

                    if (SerieName.StartsWith("node"))
                    {
                        //getting the node num to get the table
                        int i = int.Parse((SerieName.Remove(5)).Substring(4));
                        //getting the serie name
                        string serie = SerieName.Substring(9);

                        //Reading the different values form the corresponding datatable
                        var aggFunction = from row in ds.Tables[i].AsEnumerable()
                                          where row.Field<string>("name").Trim() == serie
                                          select new
                                          {
                                              trnsTag = row.Field<string>("trnsTag"),
                                              aggregationFunction = row.Field<string>("aggregationFunction"),
                                              markerType = row.Field<string>("markerType"),
                                              axisPosition = row.Field<string>("axisPosition"),
                                              stacked = row.Field<bool>("stacked"),
                                              gallery = row.Field<string>("gallery"),
                                              color = row.Field<string>("color")
                                          };

                        dr["Name"] = serie;
                        dr["trnsTag"] = aggFunction.ToList()[0].trnsTag;
                        dr["aggregationfunction"] = aggFunction.ToList()[0].aggregationFunction;
                        dr["markerType"] = aggFunction.ToList()[0].markerType;
                        dr["axisPosition"] = aggFunction.ToList()[0].axisPosition;
                        dr["stacked"] = aggFunction.ToList()[0].stacked;
                        dr["gallery"] = aggFunction.ToList()[0].gallery;
                        dr["color"] = aggFunction.ToList()[0].color;
                        dr["panel"] = i;
                        dr["title"] = "Weather";
                        dtSeries.Rows.Add(dr);
                    }
                    else if (SerieName.StartsWith("gdd"))
                    {
                        string serie = SerieName.Substring(7);
                        var aggFunction = from row in dtGdd.AsEnumerable()
                                          where row.Field<string>("name").Trim() == serie
                                          select new
                                          {
                                              aggregationFunction = row.Field<string>("aggregationFunction"),
                                              markerType = row.Field<string>("markerType"),
                                              axisPosition = row.Field<string>("axisPosition"),
                                              stacked = row.Field<bool>("stacked"),
                                              gallery = row.Field<string>("gallery"),
                                              color = row.Field<string>("color")
                                          };

                        dr[0] = serie;
                        dr[1] = aggFunction.ToList()[0].aggregationFunction;
                        dr[2] = aggFunction.ToList()[0].markerType;
                        dr[3] = aggFunction.ToList()[0].axisPosition;
                        dr[4] = aggFunction.ToList()[0].stacked;
                        dr[5] = aggFunction.ToList()[0].gallery;
                        dr[6] = aggFunction.ToList()[0].color;
                        dr[7] = 4;
                        dtSeries.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            }
            dtSeries.TableName = "ChartSerie";
            return dtSeries;
        }
        public DataTable GetChartDataForExport(string chartName, DataTable dtSeriesDetails)
        {
            return agriSvc.getChartData(chartName, dtSeriesDetails);
        }
        public string GetChartName()
        {
            return agriSvc.GetChartName();
            // return "daily";
        }
        public void getGDDvalues()
        {
            IWeather.GddValues = ServiceObj.getGDDDefaults();
        }

        /* IM01277495 - issue reported by alec - BEGIN*/
        public string getServicePageDetails(string controlName)
        {
            return ServiceObj.getServicePageDetails(controlName);
        }
        /* IM01277495 - issue reported by alec - END*/
    }
}