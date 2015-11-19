using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Syngenta.AgriCast.Charting.View;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Charting.Service;
using Syngenta.AgriCast.Charting.DTO;
using ChartFX.WebForms;
using ChartFX.WebForms.Adornments;
using ChartFX.WebForms.Annotation;
using ChartFX.WebForms.DataProviders;
using ChartFX.WebForms.Dhtml;
using ChartFX.WebForms.Galleries;
using ChartFX.WebForms.Internal;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using System.Text.RegularExpressions;
using Syngenta.AgriCast.Common;
using System.Configuration;
using Syngenta.Agricast.Modals;


namespace Syngenta.AgriCast.Charting.Presenter
{
    public class ChartPresenter
    {
        ChartService ChartSvc = new ChartService();
        IChart Ichart;
        DataPointInfo dpi;
        ServiceHandler sh = new ServiceHandler();
        DTOchart chartParams = new DTOchart();
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil util = new CommonUtil();
        Chart pChart = new Chart();
        string watermark = "";
        DataSet data = new DataSet();
        double StartDateOA = 0;
        double StepOA = 1.0 / 24.0;
        int noofvalues = 0;
        string lf = " H ";//label format in hours for X axis.
        string culture = "en-GB";
        CultureInfo myCulture;
        string chartName;
        ServiceInfo svi;
        string imgFolder = @"~/temp/cfx/";
        string imgName = "";
        Annotations annots = new Annotations();
        System.Drawing.Image imageObj = System.Drawing.Image.FromFile(HttpRuntime.AppDomainAppPath + @"images\warning.png");
        /// <summary>
        /// constructor
        /// </summary>
        public ChartPresenter(IChart IChart, string name)
        {
            if (IChart != null)
            {
                Ichart = IChart;
                pChart = IChart.fChart;
            }
            chartName = name;
            svi = ServiceInfo.ServiceConfig;
            culture = svi.Culture;
            dpi = LocationInfo.getLocationInfoObj.DataPointInfo;
            // Ichart.imageUrl = GetUniqueId() + ".png";

            /* alignment issue for agriInfo chart - jerrey - Start */
            var nodeList = sh.getNodeList("servicePage");
            if (nodeList != null)
            {
                foreach (string[] node in nodeList)
                {
                    if (node[0].ToString().ToLower() == "agriinfo")
                        Ichart.cssStyle = "paddingLeftFixed";
                }
            }
            /* alignment issue for agriInfo chart - jerrey - End */
        }

        public ChartPresenter(string strName)
        {
            chartName = strName;
            dpi = new DataPointInfo();
            svi = new ServiceInfo();
        }
        public ChartPresenter()
        {

        }

        /// <summary>
        /// gets the chart data from DB
        /// </summary>
        public string getChartData(DataTable agriInfoData)
        {
            try
            {
                myCulture = new CultureInfo(culture);
                /* for agricast service issue:  && Ichart!=null - ganesh*/
                if (HttpContext.Current != null && Ichart != null)
                {

                    Ichart.imageUrl = GenerateChart(agriInfoData);

                    /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
                    /* 3.3.1	Charting component should have zooming enabled. */

                    Ichart.bigImageUrl = GenerateChart(agriInfoData, true);
                    /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

                    //to enable/disable feedback
                    Ichart.hasFeedback = chartParams.HasFeedback;
                    Ichart.cultureCode = culture;
                    return "";
                }
                else
                {
                    return GenerateChart(agriInfoData);
                }

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_LOAD_FAILURE) + ex.Message.ToString();
                return "";
            }
        }

        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
        /* 3.3.1	Charting component should have zooming enabled. */
        private string GenerateChart(DataTable agriInfoData, bool isBigSize = false)
        {
            imgName = GetUniqueId() + (isBigSize ? "-bigger" : string.Empty);
            // imgName = GetUniqueId();
            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */
            imgName = imgName + (svi.IsMobile ? ".jpg" : ".png");

            #region // check if file exists
            string fullPath = "";
            string folderPath = "";
            if (HttpContext.Current != null)
            {
                HttpRequest req = HttpContext.Current.Request;
                //string fullPath = imgFolder + imgName;
                fullPath = req.MapPath(imgFolder + imgName);
                folderPath = req.MapPath(imgFolder);
                if (File.Exists(fullPath))
                {
                    return imgFolder + imgName;
                }
            }
            else
            {
                fullPath = HttpRuntime.AppDomainAppPath + @"Temp\cfx\" + imgName;
                ChartSvc.setChartWebServiceValues(dpi.stationLatitude, dpi.stationLongitude, dpi.altitude, dpi.NearbyPointSettings.MaxAllowedDistance, dpi.NearbyPointSettings.MaxAllowedAltitude, svi.ServiceName, svi.Module);
                folderPath = HttpRuntime.AppDomainAppPath + @"Temp\cfx\";
                if (File.Exists(fullPath))
                {
                    return imgFolder + imgName;
                }
            }
            #endregion

            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
            /* 3.3.1	Charting component should have zooming enabled. */
            pChart = new Chart();
            chartParams = new DTOchart();
            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

            ChartSvc.getChartData(chartParams, chartName, culture, agriInfoData);

            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
            /* 3.3.1	Charting component should have zooming enabled. */
            if (isBigSize)
            {
                var oWidth = chartParams.Width;
                var bigWidth = 0;
                int.TryParse(ConfigurationManager.AppSettings["bigChartWidth"], out bigWidth);
                // check if bigChartWidth exists, if no, default value is 1000
                bigWidth = (bigWidth == 0 ? 1000 : bigWidth);
                chartParams.Width = bigWidth;
                // height is in the same scale to width zooming
                chartParams.Height = chartParams.Height * bigWidth / oWidth;
            }
            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */
            bool flag = false;
            if (agriInfoData == null || agriInfoData.Rows.Count == 0)
                ConfigureForecastChart();
            else
            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
            {
                if (agriInfoData.Rows[0]["name"].ToString().ToLower() == "watermodel")
                    ConfigureBWMChart();
                else
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                    configureHistoryChart();
                flag = true;
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
            }
            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
            if (chartParams.warning != null && chartParams.warning != "" && (agriInfoData == null || agriInfoData.Rows.Count == 0))
            {
                TitleDockable warning = new TitleDockable(chartParams.warning);
                warning.Dock = DockArea.Bottom;
                warning.TextColor = Color.Red;
                warning.Font = new Font("Arial", 9, FontStyle.Bold);
                pChart.Titles.Add(warning);

                CustomLegendItem cli = new CustomLegendItem();
                cli.Text = util.getTransText("wm_warning");
                cli.Picture = imageObj;
                pChart.LegendBox.CustomItems.Add(cli);
            }
            else if (chartParams.warning != null && chartParams.warning != "")
            {
                CustomLegendItem cli = new CustomLegendItem();
                cli.Text = util.getTransText("wm_warningAgrinfo");
                cli.Picture = imageObj;
                pChart.LegendBox.CustomItems.Add(cli);
            }


            TitleDockable footer = new TitleDockable(HttpUtility.HtmlDecode(chartParams.FooterText));
            footer.Dock = DockArea.Bottom;
            pChart.Titles.Add(footer);
            if (flag)
            {
                if (pChart.Series.Count > 0 && pChart.Series[0].Text.ToLower().Contains("precipitation"))
                    pChart.Series[0].SendToBack();
            }

            //footer.Separation = 10;             
            pChart.EnableViewState = false;
            pChart.Data.InterpolateHidden = true;
            pChart.ImageSettings.ExcludeMarkersFromMap = true;

            //if (!Directory.Exists(req.MapPath(imgFolder)))
            //    Directory.CreateDirectory(imgFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            pChart.RenderToStream(fs, null, null);
            fs.Close();
            return imgFolder + imgName;
        }

        private string GetUniqueId()
        {
            string Uid = "";
            try
            {
                Uid = Guid.NewGuid().ToString()
                    + HttpUtility.HtmlDecode(dpi.stationName)
                    + DateTime.Now.ToString();
                //+ svi.ServiceName + chartName + svi.Culture.Replace("-", "");
                return util.strReplace(Uid);
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_UNIQUEID_FAILURE) + ex.Message.ToString();
                return Uid;
            }
        }


        private void ConfigureForecastChart()
        {
            try
            {
                pChart.Height = chartParams.Height;
                pChart.Width = chartParams.Width;
                pChart.Data.Series = chartParams.getSeriesCount();
                pChart.LegendBox.Visible = true;
                pChart.LegendBox.Dock = DockArea.Bottom;
                pChart.LegendBox.PlotAreaOnly = false;
                pChart.LegendBox.Border = DockBorder.External;
                addWatermark();
                /* IM01258137 - HtmlDecode - Jerrey - Start */
                //pChart.Titles.Add(new TitleDockable(chartParams.Title));
                string destStr = HttpUtility.HtmlDecode(chartParams.Title);
                pChart.Titles.Add(new TitleDockable(destStr));
                /* IM01258137 - HtmlDecode - Jerrey - End */
                Title title = pChart.Titles[0];
                float titleSize = title.Font.Size + 1;
                FontStyle titlestyle = FontStyle.Bold;
                title.Font = new Font(title.Font.Name, titleSize, titlestyle);

                pChart.Gallery = Gallery.Lines;
                Axis AxisX = pChart.AxisX;
                Axis AxsY1 = new AxisY();
                AxsY1 = pChart.AxisY;
                Axis AxsY2 = new AxisY();
                AxsY2 = pChart.AxisY2;
                IEnumerator venum = chartParams.getSeriesList();
                ArrayList al = new ArrayList();
                ListProvider listProv = new ListProvider(al);
                int x = 0;
                int valuesetcount = 0;
                while (venum.MoveNext())
                {
                    Series rsi = (Series)venum.Current;
                    if (rsi.values == null || rsi.values.Length == 0)
                    {
                        continue;
                    }
                    valuesetcount++;
                    listProv.List.Add(rsi.values);
                    if (noofvalues < rsi.values.Length) noofvalues = rsi.values.Length;
                }
                //pChart.Data.Points = noofvalues;
                pChart.DataSource = listProv;
                if (valuesetcount > 0)
                {
                    x = 0;
                    venum.Reset();
                    bool hlcdisabled = false;
                    bool ohlcdisabled = false;
                    int ohlccount = 0;
                    int hlccount = 0;
                    while (venum.MoveNext())
                    {
                        Series rsi = (Series)venum.Current;
                        string gallery = "";
                        if (rsi != null) gallery = rsi.Gallery;
                        if (gallery == "hlc") hlccount++;
                        else
                        {
                            if (hlccount > 0 && (hlccount % 3 > 0)) hlcdisabled = true;
                            hlccount = 0;
                        }
                        if (gallery == "ohlc") ohlccount++;
                        else
                        {
                            if (ohlccount > 0 && (ohlccount % 4 > 0)) ohlcdisabled = true;
                            ohlccount = 0;
                        }
                    }
                    if (hlccount > 0 && (hlccount % 3 > 0)) hlcdisabled = true;
                    if (ohlccount > 0 && (ohlccount % 4 > 0)) ohlcdisabled = true;
                    // do set Series attributes
                    venum.Reset();
                    while (venum.MoveNext())
                    {
                        Series rsi = (Series)venum.Current;
                        SeriesAttributes attributes = null;
                        if (pChart.Series.Count != 0 && x < pChart.Series.Count)
                            attributes = pChart.Series[x];
                        else
                        {
                            continue;
                        }
                        attributes.Color = ColorTranslator.FromHtml(rsi.Color);
                        attributes.MarkerSize = 2;
                        attributes.Line.Width = 1;
                        pChart.DataSourceSettings.Fields.Add(new FieldMap(rsi.SerieName, FieldUsage.Value));
                        if (rsi == null) { x++; continue; }
                        attributes.Gallery = Gallery.Lines;
                        string gallery = rsi.Gallery;
                        switch (gallery)
                        {
                            case "bar":
                                attributes.Gallery = Gallery.Bar;
                                attributes.Volume = 100;
                                attributes.Line.Width = 0;
                                attributes.MarkerSize = 0;
                                attributes.FillMode = FillMode.Solid;
                                break;
                            case "hlc":
                                if (!hlcdisabled)
                                {
                                    attributes.Gallery = Gallery.HighLowClose;
                                }
                                break;
                            case "ohlc":
                                if (!ohlcdisabled)
                                {
                                    attributes.Gallery = Gallery.OpenHighLowClose;
                                }
                                break;
                            case "step": attributes.Gallery = Gallery.Step; break;
                            case "curve": attributes.Gallery = Gallery.Curve; break;
                            case "line":
                                attributes.Gallery = Gallery.Lines;
                                break;
                        }
                        if (rsi.MarkerType != "")
                            attributes.MarkerShape = (MarkerShape)Enum.Parse(typeof(MarkerShape), rsi.MarkerType, true);
                        //switch (rsi.MarkerType.ToLower())
                        //{
                        //    case "circle": attributes.MarkerShape = MarkerShape.Circle; break;
                        //    case "marble": attributes.MarkerShape = MarkerShape.Marble; break;
                        //    case "square": attributes.MarkerShape = MarkerShape.Diamond; break;
                        //    case "invertedtriangle": attributes.MarkerShape = MarkerShape.InvertedTriangle; break;
                        //    case "triangle": attributes.MarkerShape = MarkerShape.Triangle; break;
                        //    case "hline": attributes.MarkerShape = MarkerShape.HorizontalLine; break;
                        //    case "vline": attributes.MarkerShape = MarkerShape.VerticalLine; break;
                        //    case "rect": attributes.MarkerShape = MarkerShape.Rect; break;
                        //    case "cross": attributes.MarkerShape = MarkerShape.Cross; break;
                        //    case "none": attributes.MarkerShape = MarkerShape.None; break;
                        //}

                        if (rsi.Position == "secondary")
                        {
                            pChart.Series[x].AxisY = pChart.AxisY2;
                            if (rsi.Scale == "fixed")
                            {
                                AxsY2.Min = chartParams.Y2min;
                                AxsY2.Max = chartParams.Y2max;
                            }
                        }
                        else
                        {
                            pChart.Series[x].AxisY = pChart.AxisY;
                            if (rsi.Scale == "fixed")
                            {
                                AxsY1.Min = chartParams.Ymin;
                                AxsY1.Max = chartParams.Ymax;
                            }
                        }
                        if (rsi.inverted)
                        {
                            AxisY axisy3 = new AxisY();
                            pChart.AxesY.Add(axisy3);
                            pChart.Series[x].AxisY = axisy3;
                            axisy3.Max = 1000;
                            axisy3.Min = 0;
                            axisy3.Step = 100;
                            axisy3.Inverted = true;
                            axisy3.Visible = false;
                            //axisy3.Labels.Add("");
                            //axisy3.Labels[0] = "0%";
                            //axisy3.Labels[1] = "100%";                            
                            //axisy3.Grids.Major.Visible = false;
                            // axisy3.Grids.Minor.Visible = false;

                            double maxrange = (chartParams.Ymax - chartParams.Ymin) / 10;
                            double markerValue = chartParams.Ymax - maxrange;

                            CustomGridLine cgl = new CustomGridLine(markerValue, "");
                            cgl.Color = Color.Black;
                            cgl.Width = 1;
                            cgl.Alignment = StringAlignment.Far;
                            //cgl.OutsideText = true;                             
                            //pChart.Series[x].AxisY.CustomGridLines.Add(cgl);
                            pChart.AxisY.CustomGridLines.Add(cgl);
                        }
                        if (rsi.Stacked) attributes.Stacked = true;
                        pChart.Series[x].Text = rsi.SerieName;

                        x++;
                    }

                    AxisX AxisX2 = new AxisX();
                    if (chartParams.TopLabels != null)
                    {
                        pChart.AxesX.Add(AxisX2);
                        AxisX2 = pChart.AxesX[1];
                    }

                    if (lf != null)
                    {
                        AxisX.LabelsFormat.CustomFormat = lf;
                        bool dateformat = (lf.IndexOf("d") >= 0)
                            || (lf.IndexOf("M") >= 0) || (lf.IndexOf("y") >= 0);
                        bool timeformat = lf.IndexOf("t") >= 0
                            || lf.IndexOf("H") >= 0 || lf.IndexOf("s") >= 0;
                        if (timeformat) AxisX.LabelsFormat.Format = AxisFormat.Time;
                        if (dateformat) AxisX.LabelsFormat.Format = AxisFormat.Date;
                        if (timeformat && dateformat) AxisX.LabelsFormat.Format = AxisFormat.DateTime;
                    }

                    AxisX.LabelsFormat.Culture = myCulture;
                    AxisX.MinorStep = chartParams.minorStep;
                    if (chartParams.majorStep > 0) AxisX.Step = chartParams.majorStep;
                    AxisX.Grids.Minor.TickMark = TickMark.Outside;
                    if (chartParams.minorStep > 0) AxisX.Grids.Major.TickMark = TickMark.Cross;

                    if (chartParams.BottomLabels != null && chartParams.BottomLabels.Length > 1)
                    {
                        StepOA = chartParams.BottomLabels[1] - chartParams.BottomLabels[0];
                        StartDateOA = chartParams.BottomLabels[0];
                        double step = chartParams.majorStep / StepOA;
                        double[] myStep = new double[chartParams.BottomLabels.Length];
                        int i = 0;
                        for (; i < chartParams.BottomLabels.Length; i++)
                        {
                            if (i == 0 && step > 0)
                            {
                                myStep[0] = step - 0.5;
                                if (myStep[0] == 0) myStep[0] = 0.5;
                                AxisX.Labels[i] = "";
                            }
                            else
                            {
                                string label = DateTime.FromOADate(chartParams.BottomLabels[i]).ToString(lf, myCulture.DateTimeFormat);
                                AxisX.Labels[i] = label;
                                myStep[i] = step;
                            }
                        }
                        if (chartParams.majorStep > 0)
                        {
                            AxisX.CustomSteps = myStep;
                            AxisX.Step = step;
                            AxisX.MinorStep /= StepOA;
                        }
                    }

                    formatAxis(pChart.AxesY[0], pChart.AxesY[1], chartParams);
                    //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
                    if (chartParams.isInvisible)
                    {
                        pChart.AxesY[1] = new AxisY() { Min = 0, Max = 0 };
                    }
                    //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
                    formatXAxis(AxisX, AxisX2, chartParams);
                    if (chartParams.markers != null) formatMarkers(pChart, chartParams);
                    pChart.AxisY2.Grids.Major.Visible = false;
                    pChart.AxisY2.Grids.Minor.Visible = false;
                    AxisX.Grids.Minor.TickMark = TickMark.Outside;
                    AxisX.Grids.Major.Color = Color.Gray;
                    AxisX.Grids.Major.Style = System.Drawing.Drawing2D.DashStyle.Solid;
                    AxisX2.Grids.Major.Style = System.Drawing.Drawing2D.DashStyle.Solid;
                    pChart.BackColor = Color.White;
                    SimpleBorder myBorder = new SimpleBorder(SimpleBorderType.Color);
                    myBorder.Color = ColorTranslator.FromHtml("#d7d2cc");
                    pChart.Border = myBorder;
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_CONFIGURE_FAILURE) + ex.Message.ToString();
                return;
            }
        }
        private void configureHistoryChart()
        {
            try
            {
                pChart.Height = chartParams.Height;
                pChart.Width = chartParams.Width;
                pChart.Data.Series = chartParams.getSeriesCount();
                pChart.LegendBox.Visible = true;
                pChart.LegendBox.Dock = DockArea.Bottom;
                pChart.LegendBox.PlotAreaOnly = false;
                pChart.LegendBox.Border = DockBorder.External;

                /*IM01166162 - AgriInfo UI Issues - Begin*/
                //pChart.Titles.Add(new TitleDockable(chartParams.Title));
                //Title title = pChart.Titles[0];
                //float titleSize = 0.0f;
                //titleSize = title.Font.Size + 1;
                //FontStyle titlestyle = FontStyle.Bold;
                //title.Font = new Font(title.Font.Name, titleSize, titlestyle);

                string strTitle = chartParams.Title;
                string[] strTitleArray = null;
                if (!string.IsNullOrEmpty(strTitle))
                {
                    if (strTitle.IndexOf('#') != -1)
                    {
                        strTitleArray = strTitle.Split('#');
                    }

                    if (strTitleArray != null && strTitleArray.Length > 0)
                    {
                        foreach (string str in strTitleArray)
                        {
                            /* IM01258137 - HtmlDecode - Jerrey - Start */
                            //TitleDockable title = new TitleDockable(str);
                            string destStr = HttpUtility.HtmlDecode(str);
                            TitleDockable title = new TitleDockable(destStr);
                            /* IM01258137 - HtmlDecode - Jerrey - End */
                            float titleSize = title.Font.Size + 1;

                            FontStyle titlestyle = FontStyle.Bold;
                            title.Font = new Font(title.Font.Name, titleSize, titlestyle);

                            title.PlotAreaOnly = false;
                            pChart.Titles.Add(title);

                        }
                    }

                }
                /*IM01166162 - AgriInfo UI Issues - End*/
                addWatermark();
                pChart.Gallery = Gallery.Lines;
                Axis AxisX = pChart.AxisX;
                Axis AxsY1 = new AxisY();
                Axis AxsY2 = new AxisY();
                IEnumerator venum = chartParams.getSeriesList();
                ArrayList al = new ArrayList();
                ListProvider listProv = new ListProvider(al);
                Pane panel = null;

                int x = 0;
                int valuesetcount = 0;
                int pane = -1;
                int paneCount = 0;
                bool setSAxis = false;
                bool isSetCustomStepPrimary = true;//used for custome step(0.2)default to true
                bool isSetCustomStepSec = true;//used for custome step(0.2)default to true
                while (venum.MoveNext())
                {
                    Series rsi = (Series)venum.Current;
                    if (rsi.values == null || rsi.values.Length == 0 || !(rsi.hasvalues))
                    {
                        continue;
                    }
                    valuesetcount++;
                    listProv.List.Add(rsi.values);
                    if (noofvalues < rsi.values.Length) noofvalues = rsi.values.Length;
                }
                pChart.Data.Points = noofvalues;
                pChart.DataSource = listProv;
                if (valuesetcount > 0)
                {
                    x = 0;
                    venum.Reset();
                    bool hlcdisabled = false;
                    bool ohlcdisabled = false;
                    int ohlccount = 0;
                    int hlccount = 0;
                    while (venum.MoveNext())
                    {
                        Series rsi = (Series)venum.Current;
                        string gallery = "";
                        if (rsi != null) gallery = rsi.Gallery;
                        if (gallery == "hlc") hlccount++;
                        else
                        {
                            if (hlccount > 0 && (hlccount % 3 > 0)) hlcdisabled = true;
                            hlccount = 0;
                        }
                        if (gallery == "ohlc") ohlccount++;
                        else
                        {
                            if (ohlccount > 0 && (ohlccount % 4 > 0)) ohlcdisabled = true;
                            ohlccount = 0;
                        }
                    }
                    if (hlccount > 0 && (hlccount % 3 > 0)) hlcdisabled = true;
                    if (ohlccount > 0 && (ohlccount % 4 > 0)) ohlcdisabled = true;
                    // do set Series attributes
                    venum.Reset();
                    while (venum.MoveNext())
                    {
                        for (int i = 0; i < noofvalues; i++)
                        {
                            if (pChart.Data[x, i] == ChartFX.WebForms.Chart.Hidden)
                            {
                                pChart.Data[x, i] = ChartFX.WebForms.Chart.Hidden;
                                pChart.Points[x, i].MarkerSize = 5;
                                pChart.Points[x, i].MarkerShape = MarkerShape.X;
                                pChart.Points[x, i].Color = Color.White;
                                pChart.Points[x, i].Border.Visible = false;
                            }
                        }
                        Series rsi = (Series)venum.Current;
                        SeriesAttributes attributes;
                        if (pChart.Series.Count > 0 && rsi.hasvalues)
                            attributes = pChart.Series[x];
                        else
                            continue;
                        attributes.Color = ColorTranslator.FromHtml(rsi.Color);
                        attributes.MarkerSize = 3;
                        attributes.Line.Width = 1;
                        attributes.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;

                        pChart.DataSourceSettings.Fields.Add(new FieldMap(rsi.SerieName, FieldUsage.Value));
                        if (rsi == null) { x++; continue; }
                        attributes.Gallery = Gallery.Lines;
                        string gallery = rsi.Gallery;
                        switch (gallery)
                        {
                            case "bar":
                                attributes.Gallery = Gallery.Bar;
                                attributes.Volume = 84;
                                attributes.Line.Width = 0;
                                break;
                            case "hlc":
                                if (!hlcdisabled)
                                {
                                    attributes.Gallery = Gallery.HighLowClose;
                                }
                                break;
                            case "ohlc":
                                if (!ohlcdisabled)
                                {
                                    attributes.Gallery = Gallery.OpenHighLowClose;
                                }
                                break;
                            case "step": attributes.Gallery = Gallery.Step; break;
                            case "curve": attributes.Gallery = Gallery.Curve; break;
                            case "line":
                                attributes.Gallery = Gallery.Lines;
                                break;
                        }
                        if (rsi.MarkerType != "")
                            attributes.MarkerShape = (MarkerShape)Enum.Parse(typeof(MarkerShape), rsi.MarkerType, true);

                        if (pane == -1)
                        {
                            pane = rsi.pane;
                            panel = pChart.MainPane;
                        }
                        bool paneChanged = false;
                        if (pane != rsi.pane)
                        {
                            panel = new Pane();
                            pChart.Panes.Add(panel);
                            pChart.Height = Int32.Parse(pChart.Height.Value.ToString()) + 250;
                            pane = rsi.pane;
                            paneCount++;
                            paneChanged = true;
                        }
                        //Font fnt = panel.Title.Font;
                        //fnt = new Font(fnt.FontFamily, 10, FontStyle.Bold);
                        //panel.Title.Text = rsi.pane == 0 ? "Daily" : rsi.pane == 1 ? "Extremes" : "LongTerm";
                        //panel.Title.LineAlignment = StringAlignment.Center;                        
                        //panel.Title.Font=fnt;

                        double min = 0d;
                        double max = 0d;
                        min = double.TryParse(rsi.values.Min().ToString(), out min) ? min : 0d;
                        max = double.TryParse(rsi.values.Max().ToString(), out max) ? max : 0d;

                        if (rsi.Position == "secondary")
                        {
                            if (pChart.Panes[paneCount].Axes.Count < 2)
                                pChart.Panes[paneCount].Axes.Add(new AxisY());

                            pChart.Series[x].AxisY = pChart.Panes[paneCount].Axes[1];
                            if (rsi.axisLabel.ToString() != "")
                                pChart.Panes[paneCount].Axes[1].Title.Text = rsi.axisLabel.ToString();
                            setSAxis = true;

                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            //if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0)
                            if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0 || max > 1)
                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            {
                                //even if one of the series is false, set the custome step to false
                                isSetCustomStepSec = false;
                            }
                        }
                        else
                        {
                            pChart.Series[x].AxisY = pChart.AxisY;
                            if (rsi.axisLabel.ToString() != "")
                                pChart.Panes[paneCount].AxisY.Title.Text = rsi.axisLabel.ToString();

                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            //if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0)
                            if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0 || max > 1)
                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            {
                                //even if one of the series is false, set the custome step to false
                                isSetCustomStepPrimary = false;
                            }
                        }

                        if (paneChanged || pane == 0)
                        {
                            //if (setPAxis)paneCount > 0 ? paneCount - 1 : paneCount
                            if (isSetCustomStepPrimary)//check the flag
                            {
                                pChart.Panes[paneCount].AxisY.Step = 0.2;
                                pChart.Panes[paneCount].AxisY.LabelsFormat.Decimals = 1;
                            }
                            else
                            {
                                /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                                /* commented for the issue */
                                /*pChart.Panes[paneCount].AxisY.Step = 0.0;*/
                                double vStep = Math.Ceiling(Math.Abs(((min < 0) ? (max - min) : max) / 10));
                                if (vStep > pChart.Panes[paneCount].AxisY.Step)
                                    pChart.Panes[paneCount].AxisY.Step = vStep;
                                /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - End */
                                pChart.Panes[paneCount].AxisY.LabelsFormat.Decimals = 0;
                            }

                            //set the secondary access
                            if (setSAxis)
                            {
                                if (isSetCustomStepSec)
                                {
                                    pChart.Panes[paneCount].Axes[1].Step = 0.2;
                                    pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = 1;
                                }
                                else
                                {
                                    /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                                    /* commented for the issue */
                                    /*pChart.Panes[paneCount].Axes[1].Step = 0.0;*/
                                    double vStep = Math.Ceiling(Math.Abs(((min < 0) ? (max - min) : max) / 10));
                                    if (vStep > pChart.Panes[paneCount].Axes[1].Step)
                                        pChart.Panes[paneCount].Axes[1].Step = vStep;
                                    /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - End */
                                    pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = 0;
                                }
                                setSAxis = false;
                            }

                            /*Commented for Chart issue - 08-06-2012 */

                            //if(isSetCustomStepSec)
                            //pChart.Panes[paneCount].Axes[1].Step = 0.2;
                            //pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = 1;
                            //setSAxis = false;
                            //}
                            //else
                            //{
                            //    pChart.Panes[paneCount].Axes[1].Step = 0.0;
                            //    pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = 0;
                            //}

                            /*End of Commented code for Chart issue - 08-06-2012 */
                        }

                        //    if (rsi.Scale == "fixed")
                        //    {
                        //        AxsY2.Min = rsi.MinorY;
                        //        AxsY2.Max = rsi.MajorY;
                        //    }
                        //else
                        //{
                        //    min = double.TryParse(pChart.Panes[paneCount].Axes[1].Min.ToString(), out min) ? min : 0.0;
                        //    max = double.TryParse(pChart.Panes[paneCount].Axes[1].Max.ToString(), out max) ? max : 1.0;
                        //    min = min < (rsi.values.Min() ?? 0) ? min : (rsi.values.Min() ?? 0);
                        //    max = max > (rsi.values.Max() ?? 0) ? max : (rsi.values.Max() ?? 0);
                        //    scale = new NiceScale(min, max);
                        //    pChart.Panes[paneCount].Axes[1].Max = scale.getNiceMax();
                        //    pChart.Panes[paneCount].Axes[1].Min = scale.getNiceMin();
                        //    pChart.Panes[paneCount].Axes[1].Step = scale.getTickSpacing();
                        //    pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = scale.getNiceMax() > 10 ? 0 : 1;
                        //}

                        //if (rsi.Scale == "fixed")
                        //{
                        //    AxsY1.Min = rsi.MinorY;
                        //    AxsY1.Max = rsi.MajorY;
                        //}
                        //    //else
                        //    //{
                        //    //    min = double.TryParse(pChart.Panes[paneCount].AxisY.Min.ToString(), out min) ? min : 0.0;
                        //    //    max = double.TryParse(pChart.Panes[paneCount].AxisY.Max.ToString(), out max) ? max : 1.0;
                        //    //    min = min < (rsi.values.Min() ?? 0) ? min : (rsi.values.Min() ?? 0);
                        //    //    max = max > (rsi.values.Max() ?? 0) ? max : (rsi.values.Max() ?? 0);
                        //    //    scale = new NiceScale(min, max);
                        //    //    pChart.Panes[paneCount].AxisY.Max = scale.getNiceMax();
                        //    //    pChart.Panes[paneCount].AxisY.Min = scale.getNiceMin();
                        //    //    pChart.Panes[paneCount].AxisY.Step = scale.getTickSpacing();
                        //    //    pChart.Panes[paneCount].AxisY.LabelsFormat.Decimals = scale.getNiceMax() > 10 ? 0 : 1;
                        //    //}
                        //}
                        if (rsi.hasGaps && annots.List.Count <= paneCount)
                        {
                            pChart.Extensions.Add(annots);

                            AnnotationPicture ap = new AnnotationPicture();
                            ap.Picture = imageObj;
                            ap.Top = paneCount > 0 ? ((paneCount * 210) + 40) : pChart.Panes[paneCount].BoundingRectangle.Top - 8;
                            ap.Left = pChart.Panes[paneCount].BoundingRectangle.Left - 8;
                            ap.Width = 24;
                            ap.Height = 24;
                            ap.Mode = AnnImageMode.Stretch;
                            ap.Border.Color = Color.Transparent;
                            annots.List.Add(ap);
                        }
                        if (rsi.Stacked) attributes.Stacked = true;
                        pChart.Series[x].Pane = pChart.Panes[paneCount];
                        pChart.Series[x].Text = rsi.SerieName;
                        setSAxis = false;
                        x++;
                    }

                    lf = getLegendFormat(true);
                    AxisX.LabelsFormat.CustomFormat = lf;
                    AxisX.LabelsFormat.Format = AxisFormat.Date;
                    AxisX.LabelsFormat.Culture = myCulture;
                    if (chartParams.BottomLabels != null && chartParams.BottomLabels.Length > 0)
                    {
                        if (chartParams.BottomLabels.Length > 1)
                        {
                            StepOA = chartParams.BottomLabels[1] - chartParams.BottomLabels[0];
                        }
                        StartDateOA = chartParams.BottomLabels[0];
                        double step = 0d;
                        double[] myStep = new double[chartParams.BottomLabels.Length];
                        int i = 0;
                        for (; i < chartParams.BottomLabels.Length; i++)
                        {
                            string label;
                            label = DateTime.FromOADate(chartParams.BottomLabels[i]).ToString(lf, myCulture.DateTimeFormat);
                            AxisX.Labels[i] = label;
                            myStep[i] = step;
                        }
                        if (chartParams.majorStep > 0)
                        {
                            AxisX.CustomSteps = myStep;
                            AxisX.Step = step;
                            // AxisX.MinorStep /= StepOA;
                        }
                    }

                    if (chartParams.markers != null)
                    {
                        formatMarkers(pChart, chartParams);
                        LegendItemAttributes lia = new LegendItemAttributes();
                        pChart.LegendBox.ItemAttributes[pChart.AxisX.CustomGridLines] = lia;
                        lia.Visible = false;
                    }
                    formatXAxis(AxisX, null, chartParams);
                    AxisX.LabelAngle = 0;
                    pChart.AxisX.Grids.Major.Visible = false;
                    pChart.AxisX.Grids.Minor.Visible = false;
                    pChart.AxisX.Grids.Major.TickMark = TickMark.Outside;
                    pChart.Border = new SimpleBorder(SimpleBorderType.Light);



                }
                else
                {
                    //throw new System.InvalidOperationException(util.getTransText("err_nochartdatapresent"));
                    return;
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_CONFIGURE_FAILURE) + ":" + ex.Message.ToString();
                return;
            }
        }

        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/

        private void ConfigureBWMChart()
        {
            try
            {
                int thresholdPaneIndex = -1;
                double[] thresholdValueList = new double[] { 0.0, 0.0, 0.0 };
                var colors = new Color[] { 
                                    Color.FromArgb(128, 251, 84, 56), 
                                    Color.FromArgb(128, 255, 180, 0), 
                                    Color.FromArgb(128, 170, 180, 0) 
                                };
                var labels = new string[] { 
                                    util.getTransText("wm_highstress_BWM"),
                                    util.getTransText("wm_warning_BWM"),
                                    util.getTransText("wm_good_BWM") 
                                };

                pChart.Height = chartParams.Height;
                pChart.Width = chartParams.Width;
                pChart.Data.Series = chartParams.getSeriesCount();
                pChart.LegendBox.Visible = true;
                pChart.LegendBox.Dock = DockArea.Bottom;
                pChart.LegendBox.PlotAreaOnly = false;
                pChart.LegendBox.Border = DockBorder.External;
                /* IM01321207 :- New Agricast - BWM (decp02iap) - Alignment of the legend (temperature/water) - Jerrey - Begin */
                //if (svi.ServiceName.ToLower() == "decp02iap") // not sure whether this is only for decp02iap or not?
                //{
                pChart.LegendBox.ContentLayout = ContentLayout.Near;
                pChart.LegendBox.MarginX = 50;
                //}
                /* IM01321207 :- New Agricast - BWM (decp02iap) - Alignment of the legend (temperature/water) - Jerrey - End */
                #region add title
                string strTitle = chartParams.Title;
                string[] strTitleArray = null;
                if (!string.IsNullOrEmpty(strTitle))
                {
                    if (strTitle.IndexOf('#') != -1)
                    {
                        strTitleArray = strTitle.Split('#');
                    }

                    if (strTitleArray != null && strTitleArray.Length > 0)
                    {
                        int i = 0;
                        foreach (string str in strTitleArray)
                        {
                            /* IM01258137 - HtmlDecode - Jerrey - Start */
                            //TitleDockable title = new TitleDockable(str);
                            string destStr = HttpUtility.HtmlDecode(str);
                            TitleDockable title = new TitleDockable(destStr);
                            /* IM01258137 - HtmlDecode - Jerrey - End */
                            float titleSize = title.Font.Size + 1;

                            if (i < 2)
                                title.Font = new Font(title.Font.Name, titleSize + 2, FontStyle.Bold);
                            else
                                title.Font = new Font(title.Font.Name, titleSize, FontStyle.Regular);

                            title.PlotAreaOnly = false;
                            pChart.Titles.Add(title);
                            i++;
                        }
                    }
                }
                #endregion

                pChart.Gallery = Gallery.Lines;
                Axis AxisX = pChart.AxisX;
                Axis AxsY1 = new AxisY();
                Axis AxsY2 = new AxisY();
                IEnumerator venum = chartParams.getSeriesList();
                ArrayList al = new ArrayList();
                ListProvider listProv = new ListProvider(al);

                //pChart.Panes.Add(new Pane());

                Pane panel = null;

                int x = 0;
                int valuesetcount = 0;
                int pane = -1;
                int paneCount = 0;
                bool setSAxis = false;
                bool isSetCustomStepPrimary = true;//used for custome step(0.2)default to true
                bool isSetCustomStepSec = true;//used for custome step(0.2)default to true
                while (venum.MoveNext())
                {
                    Series rsi = (Series)venum.Current;
                    if (rsi.values == null || rsi.values.Length == 0 || !(rsi.hasvalues))
                    {
                        continue;
                    }
                    valuesetcount++;
                    listProv.List.Add(rsi.values);
                    if (noofvalues < rsi.values.Length) noofvalues = rsi.values.Length;
                }
                pChart.Data.Points = noofvalues;
                pChart.DataSource = listProv;
                if (valuesetcount > 0)
                {
                    x = 0;
                    venum.Reset();
                    bool hlcdisabled = false;
                    bool ohlcdisabled = false;
                    int ohlccount = 0;
                    int hlccount = 0;
                    while (venum.MoveNext())
                    {
                        Series rsi = (Series)venum.Current;
                        string gallery = "";
                        if (rsi != null) gallery = rsi.Gallery;
                        if (gallery == "hlc") hlccount++;
                        else
                        {
                            if (hlccount > 0 && (hlccount % 3 > 0)) hlcdisabled = true;
                            hlccount = 0;
                        }
                        if (gallery == "ohlc") ohlccount++;
                        else
                        {
                            if (ohlccount > 0 && (ohlccount % 4 > 0)) ohlcdisabled = true;
                            ohlccount = 0;
                        }
                    }
                    if (hlccount > 0 && (hlccount % 3 > 0)) hlcdisabled = true;
                    if (ohlccount > 0 && (ohlccount % 4 > 0)) ohlcdisabled = true;
                    // do set Series attributes
                    venum.Reset();
                    while (venum.MoveNext())
                    {
                        Series rsi = (Series)venum.Current;
                        SeriesAttributes attributes;
                        if (pChart.Series.Count > 0 && rsi.hasvalues)
                            attributes = pChart.Series[x];
                        else
                            continue;
                        attributes.Color = ColorTranslator.FromHtml(rsi.Color);
                        attributes.MarkerSize = 3;
                        attributes.Line.Width = 1;
                        pChart.DataSourceSettings.Fields.Add(new FieldMap(rsi.SerieName, FieldUsage.Value));
                        if (rsi == null) { x++; continue; }
                        attributes.Gallery = Gallery.Lines;
                        string gallery = rsi.Gallery;
                        switch (gallery)
                        {
                            case "bar":
                                attributes.Gallery = Gallery.Bar;
                                attributes.Volume = 84;
                                attributes.Line.Width = 0;
                                break;
                            case "hlc":
                                if (!hlcdisabled)
                                {
                                    attributes.Gallery = Gallery.HighLowClose;
                                }
                                break;
                            case "ohlc":
                                if (!ohlcdisabled)
                                {
                                    attributes.Gallery = Gallery.OpenHighLowClose;
                                }
                                break;
                            case "step": attributes.Gallery = Gallery.Step; break;
                            case "curve": attributes.Gallery = Gallery.Curve; break;
                            case "line":
                                attributes.Gallery = Gallery.Lines;
                                break;
                        }
                        if (rsi.MarkerType != "")
                            attributes.MarkerShape = (MarkerShape)Enum.Parse(typeof(MarkerShape), rsi.MarkerType, true);

                        if (pane == -1)
                        {
                            pane = rsi.pane;
                            panel = pChart.MainPane;
                        }
                        bool paneChanged = false;
                        if (pane != rsi.pane)
                        {
                            panel = new Pane();
                            pChart.Panes.Add(panel);
                            pChart.Height = Int32.Parse(pChart.Height.Value.ToString()) + 250;
                            pane = rsi.pane;
                            paneCount++;
                            paneChanged = true;
                        }

                        double min = 0d;
                        double max = 0d;
                        min = double.TryParse(rsi.values.Min().ToString(), out min) ? min : 0d;
                        max = double.TryParse(rsi.values.Max().ToString(), out max) ? max : 0d;

                        if (rsi.Position == "secondary")
                        {
                            if (pChart.Panes[paneCount].Axes.Count < 2)
                                pChart.Panes[paneCount].Axes.Add(new AxisY());

                            pChart.Series[x].AxisY = pChart.Panes[paneCount].Axes[1];
                            if (rsi.axisLabel.ToString() != "")
                                pChart.Panes[paneCount].Axes[1].Title.Text = rsi.axisLabel.ToString();

                            setSAxis = true;

                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            //if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0)
                            if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0 || max > 1)
                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            {
                                //even if one of the series is false, set the custome step to false
                                isSetCustomStepSec = false;
                            }
                        }
                        else
                        {
                            pChart.Series[x].AxisY = pChart.AxisY;
                            if (rsi.axisLabel.ToString() != "")
                                pChart.Panes[paneCount].AxisY.Title.Text = rsi.axisLabel.ToString();

                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            //if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0)
                            if ((Math.Abs(max - min) == 0.0 && max > 1.0) || Math.Abs(max - min) > 1.0 || max > 1)
                            /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                            {
                                //even if one of the series is false, set the custome step to false
                                isSetCustomStepPrimary = false;
                            }
                        }

                        if (paneChanged || pane == 0)
                        {
                            //if (setPAxis)paneCount > 0 ? paneCount - 1 : paneCount
                            if (isSetCustomStepPrimary)//check the flag
                            {
                                pChart.Panes[paneCount].AxisY.Step = 0.2;
                                pChart.Panes[paneCount].AxisY.LabelsFormat.Decimals = 1;
                            }
                            else
                            {
                                /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                                /* commented for the issue */
                                /*pChart.Panes[paneCount].AxisY.Step = 0.0;*/
                                double vStep = Math.Ceiling(Math.Abs(((min < 0) ? (max - min) : max) / 10));
                                if (vStep > pChart.Panes[paneCount].AxisY.Step)
                                    pChart.Panes[paneCount].AxisY.Step = vStep;
                                /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - End */
                                pChart.Panes[paneCount].AxisY.LabelsFormat.Decimals = 0;
                            }

                            //set the secondary access
                            if (setSAxis)
                            {
                                if (isSetCustomStepSec)
                                {
                                    pChart.Panes[paneCount].Axes[1].Step = 0.2;
                                    pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = 1;
                                }
                                else
                                {
                                    /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                                    /* commented for the issue */
                                    /*pChart.Panes[paneCount].Axes[1].Step = 0.0;*/
                                    double vStep = Math.Ceiling(Math.Abs(((min < 0) ? (max - min) : max) / 10));
                                    if (vStep > pChart.Panes[paneCount].Axes[1].Step)
                                        pChart.Panes[paneCount].Axes[1].Step = vStep;
                                    /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - End */
                                    pChart.Panes[paneCount].Axes[1].LabelsFormat.Decimals = 0;
                                }
                                setSAxis = false;
                            }

                        }

                        if (rsi.hasGaps && annots.List.Count <= paneCount)
                        {
                            pChart.Extensions.Add(annots);

                            AnnotationPicture ap = new AnnotationPicture();
                            ap.Picture = imageObj;
                            ap.Top = paneCount > 0 ? ((paneCount * 210) + 40) : pChart.Panes[paneCount].BoundingRectangle.Top - 8;
                            ap.Left = pChart.Panes[paneCount].BoundingRectangle.Left - 8;
                            ap.Width = 24;
                            ap.Height = 24;
                            ap.Mode = AnnImageMode.Stretch;
                            ap.Border.Color = Color.Transparent;
                            annots.List.Add(ap);
                        }
                        if (rsi.Stacked) attributes.Stacked = true;
                        pChart.Series[x].Pane = pChart.Panes[paneCount];
                        pChart.Series[x].Text = rsi.SerieName;

                        #region // set threshold of moisture pane
                        if (rsi.SerieName == util.getTransText("wm_watersurfacesss"))
                        {
                            /**
                             * If the total soil depth is below 30 cm, there is no threshold => white background. 
                             * An error message should be there "Too thin soil", to be tranlated
                             * If the total water holding capacity is below 50 mm, no threshold. 
                             * An error message should be there "Too low water holding capacity", to be translated
                             */
                            if (BorderWaterModel.TotalDepth < 30)
                            {
                                HttpContext.Current.Session["ErrorMessage"] = util.getTransText("Too thin soil");
                            }
                            else if (BorderWaterModel.TotalWHC < 50)
                            {
                                HttpContext.Current.Session["ErrorMessage"] = util.getTransText("Too low water holding capacity");
                            }
                            else
                            {
                                var thresholds = BorderWaterModel.ThresholdRange.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                                double minValue = 0.0;
                                double maxValue = 0.0;

                                if (thresholds.Length == 2)
                                {
                                    thresholdPaneIndex = paneCount;
                                    for (var i = 0; i <= thresholds.Length; i++)
                                    {
                                        // set the max value & min value of thresholds.
                                        if (i == 0)
                                            minValue = rsi.values.Where(v => v.HasValue).Min().Value < 0
                                                ? rsi.values.Where(v => v.HasValue).Min().Value : 0;
                                        else
                                            minValue = Convert.ToDouble(thresholds[i - 1]);

                                        if (i == thresholds.Length)
                                            maxValue = (rsi.values.Where(v => v.HasValue).Max().Value > minValue)
                                                ? rsi.values.Where(v => v.HasValue).Max().Value : minValue + 10;
                                        else
                                            maxValue = Convert.ToDouble(thresholds[i]);

                                        // add section to pane
                                        AxisSection section = new AxisSection();
                                        pChart.Panes[paneCount].AxisY.Sections.Add(section);
                                        section.BackColor = colors[i];
                                        section.From = minValue;
                                        section.To = maxValue;
                                        section.Grids.Major.Visible = false;
                                        section.Grids.Minor.Visible = false;

                                        thresholdValueList[i] = maxValue;
                                    }
                                }

                                pChart.Series[x].AxisY.Min = rsi.values.Where(v => v.HasValue).Min().Value < 0
                                    ? rsi.values.Where(v => v.HasValue).Min().Value : 0;
                                pChart.Series[x].AxisY.Max = maxValue;

                                /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed - Jerrey - Begin */
                                pChart.Series[x].AxisY.Step = Math.Ceiling(maxValue / 10);
                                /* IM01319967 - New Agricast - BWM - chart scale when few days didplayed -  Jerrey - End */
                            }
                        }
                        #endregion

                        x++;
                    }

                    lf = getLegendFormat();
                    AxisX.LabelsFormat.CustomFormat = lf;
                    AxisX.LabelsFormat.Format = AxisFormat.Date;
                    AxisX.LabelsFormat.Culture = myCulture;
                    if (chartParams.BottomLabels != null && chartParams.BottomLabels.Length > 0)
                    {
                        if (chartParams.BottomLabels.Length > 1)
                        {
                            StepOA = chartParams.BottomLabels[1] - chartParams.BottomLabels[0];
                        }
                        StartDateOA = chartParams.BottomLabels[0];
                        double step = 0d;
                        double[] myStep = new double[chartParams.BottomLabels.Length];
                        int i = 0;
                        for (; i < chartParams.BottomLabels.Length; i++)
                        {
                            string label;
                            label = DateTime.FromOADate(chartParams.BottomLabels[i]).ToString(lf, myCulture.DateTimeFormat);
                            AxisX.Labels[i] = label;
                            myStep[i] = step;
                        }
                        if (chartParams.majorStep > 0)
                        {
                            AxisX.CustomSteps = myStep;
                            AxisX.Step = step;
                            // AxisX.MinorStep /= StepOA;
                        }
                    }

                    if (chartParams.markers != null)
                    {
                        formatMarkers(pChart, chartParams);
                        LegendItemAttributes lia = new LegendItemAttributes();
                        pChart.LegendBox.ItemAttributes[pChart.AxisX.CustomGridLines] = lia;
                        lia.Visible = false;
                    }
                    formatXAxis(AxisX, null, chartParams);
                    AxisX.LabelAngle = 0;

                    pChart.AxisX.Grids.Major.Visible = false;
                    pChart.AxisX.Grids.Minor.Visible = false;
                    pChart.AxisX.Grids.Major.TickMark = TickMark.Outside;
                    pChart.Border = new SimpleBorder(SimpleBorderType.Light);

                    for (var i = 0; i < pChart.Panes.Count; i++)
                    {
                        pChart.Panes[i].Proportion = 50 * (i + 1);
                        for (var n = 0; n < pChart.Panes[i].Axes.Count; n++)
                        {
                            pChart.Panes[i].Axes[n].Grids.Major.Visible = false;
                            pChart.Panes[i].Axes[n].Grids.Minor.Visible = false;
                        }
                    }

                    // add annotation text on threshold
                    if (thresholdPaneIndex != -1)
                    {
                        Annotations annts = new Annotations();
                        pChart.Extensions.Add(annts);
                        var stepHeight = pChart.Panes[paneCount].BoundingRectangle.Height
                            //commented by Jerrey
                            /* / (pChart.Panes[paneCount].AxisY.Max / Math.Abs(pChart.Panes[paneCount].AxisY.MinorStep)); */
                                            / (pChart.Panes[paneCount].AxisY.Max / Math.Abs(pChart.Panes[paneCount].AxisY.Step));

                        for (var i = 0; i < thresholdValueList.Length; i++)
                        {
                            AnnotationText text = new AnnotationText();
                            text.Font = new Font("Arial", 9);
                            text.Text = labels[i];
                            text.Border.Color = Color.Transparent;
                            text.Align = StringAlignment.Center;
                            text.LineAlignment = StringAlignment.Center;
                            text.Left = (int)(pChart.Width.Value - 150);
                            text.Top = (int)(pChart.Panes[paneCount].BoundingRectangle.Bottom - 8 // adjust 8px to fit the position
                                //commented by Jerrey
                                /* - thresholdValueList[i] / Math.Abs(pChart.Panes[paneCount].AxisY.MinorStep) */
                                            - thresholdValueList[i] / Math.Abs(pChart.Panes[paneCount].AxisY.Step)
                                            * stepHeight);
                            text.PlotAreaOnly = true;
                            text.SizeToFit();
                            annts.List.Add(text);
                        }
                    }

                    addWatermark();
                }
                else
                {
                    throw new System.InvalidOperationException(util.getTransText("err_nochartdatapresent"));
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_CONFIGURE_FAILURE) + ":" + ex.Message.ToString();
                return;
            }

        }

        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

        private void addWatermark()
        {
            try
            {
                SolidBackground s = new SolidBackground();
                s.Color = ColorTranslator.FromHtml("#FFFFFF");
                pChart.Background = s;
                watermark = chartParams.WaterMark;
                if (watermark != null && watermark.Length > 0)
                {
                    if (File.Exists(watermark))
                    {
                        ImageBackground ib = new ImageBackground(watermark);
                        ib.Mode = ImageMode.Center;
                        pChart.Background = ib;
                        pChart.PlotAreaColor = Color.FromArgb(0, Color.White);
                    }
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_GENWATERMARK_FAILURE) + ":" + ex.Message.ToString();
                return;
            }
        }

        public void formatAxis(Axis AxisY, Axis AxisY2, DTOchart chartParams)
        {
            try
            {
                AxisY.Max = chartParams.Ymax;
                AxisY.Min = chartParams.Ymin;
                AxisY2.Max = chartParams.Y2max;
                AxisY2.Min = chartParams.Y2min;

                if (AxisY.Max - AxisY.Min > 10) AxisY.LabelsFormat.Decimals = 0;
                else AxisY.LabelsFormat.Decimals = 1;
                if (AxisY2.Max - AxisY2.Min > 10) AxisY2.LabelsFormat.Decimals = 0;
                else AxisY2.LabelsFormat.Decimals = 1;
                AxisY.LabelsFormat.Culture = myCulture;
                AxisY2.LabelsFormat.Culture = myCulture;
                AxisY.Position = AxisPosition.Near;
                AxisY2.Position = AxisPosition.Far;

                // coloring of the axis text
                string color = (string)"Blue";
                //Font fnt = AxisY.Title.Font;
                // fnt = new Font(fnt.FontFamily, fnt.Size, FontStyle.Bold);
                if (color != null)
                {
                    AxisY.Title.TextColor = ColorTranslator.FromHtml(color);
                    AxisY.Title.Text += chartParams.LeftAxisLabel;
                    //AxisY.Title.Font = fnt;
                }
                color = (string)"Orange";
                if (color != null)
                {
                    AxisY2.Title.TextColor = ColorTranslator.FromHtml(color);
                    AxisY2.Title.Text += chartParams.RightAxisLabel;
                    //AxisY2.Title.Font = fnt;
                }

                // horizontal shaded areas
                Axis AxisT = AxisY;
                // check AxisT for condition any Series not being allocated to it
                if (AxisT.Min >= float.MaxValue && AxisT.Max <= float.MinValue) AxisT = AxisY2;
                if (AxisT.Min >= float.MaxValue && AxisT.Max <= float.MinValue) return;

                if (chartParams.PrimaryLabels != null)
                {
                    for (int x = 0; x < chartParams.PrimaryLabels.Length; x++) AxisY.Labels[x] = chartParams.PrimaryLabels[x];
                    AxisY.LabelValue = chartParams.PrimaryLabelValue;
                }
                if (chartParams.SecondaryLabels != null)
                {
                    for (int x = 0; x < chartParams.SecondaryLabels.Length; x++) AxisY2.Labels[x] = chartParams.SecondaryLabels[x];
                    AxisY2.LabelValue = chartParams.SecondaryLabelValue;
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_GENAXES_FAILURE) + ":" + ex.Message.ToString();
                return;
            }
        }
        private bool formatXAxis(Axis AxisX, Axis AxisX2, DTOchart chartParams)
        {
            try
            {
                // show vertical shaded areas
                if (chartParams.shades != null)
                {
                    for (int z = 0; z < chartParams.shades.Length; z++)
                    {
                        if (chartParams.shades[z] == null) break;
                        AxisSection axs = new AxisSection();
                        axs.From = (chartParams.shades[z].min - StartDateOA + StepOA / 2) / StepOA;
                        axs.To = (chartParams.shades[z].max - StartDateOA + StepOA / 2) / StepOA;
                        axs.BackColor = Color.FromArgb(150, chartParams.shades[z].color);
                        AxisX.Sections.Add(axs);
                    }
                }

                if (AxisX2 != null)
                {
                    AxisX2.Visible = true;
                    AxisX2.Position = AxisPosition.Far;
                    AxisX2.Style |= AxisStyles.Centered;
                    int step = (int)Math.Round(1.0 / StepOA, 0);
                    AxisX2.Step = step;
                    AxisX2.Min = 0;
                    AxisX2.Max = noofvalues;
                    double[] myStep = new double[noofvalues / step];
                    for (int x = 0; x < myStep.Length; x++)
                    {
                        myStep[x] = step;
                    }
                    AxisX2.CustomSteps = myStep;

                    if (chartParams.TopLabels != null)
                    {
                        for (int x = 0; x < chartParams.TopLabels.Length; x++)
                            AxisX2.Labels[(int)myStep[0] + step * x] = chartParams.TopLabels[x];
                    }
                    AxisX2.Line.Color = Color.Black;
                }
                return true;
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = util.getTransText(Constants.CHART_FORMATAXES_FAILURE) + ":" + ex.Message.ToString();
                return false;
            }
        }
        private void formatMarkers(Chart cChart, DTOchart gParams)
        {
            int cl = cChart.AxisX.CustomGridLines.Count;
            ArrayList verticalMarkers = new ArrayList();
            ArrayList horizontalMarkers = new ArrayList();
            double timeline = cChart.AxisX.Max - cChart.AxisX.Min;
            double[] legends = gParams.BottomLabels;
            if (legends != null && legends.Length > 0)
                timeline = legends[legends.Length - 1] - legends[0];
            for (int x = 0; x < gParams.markers.Length; x++)
            {
                ResultMarker marker = gParams.markers[x];
                //if (!marker.vertical && marker.panel != panel) continue;
                //if (marker.vertical) continue;
                if (marker.vertical) verticalMarkers.Add(marker);
                else horizontalMarkers.Add(marker);
            }
            verticalMarkers.Sort();
            for (int x = 0; x < verticalMarkers.Count; x++)
            {
                ResultMarker before = null;
                if (x > 1) before = (ResultMarker)verticalMarkers[x - 1];
                ResultMarker marker = (ResultMarker)verticalMarkers[x];
                ResultMarker after = null;
                if (x < verticalMarkers.Count - 1) after = (ResultMarker)verticalMarkers[x + 1];
                // left/right alignment of the label text, depending on how close to the left start
                if (marker.pos - legends[0] < timeline * 0.02f)
                    marker.faralign = true;
                if (before != null && after != null)
                {
                    double diffb = marker.pos - before.pos;
                    double diffa = after.pos - marker.pos;
                    // flip the text if the marker to the left is closer, the gap is small and the 
                    // marker displays at a different date.
                    if (diffb < diffa && diffb < timeline * 0.04f
                        && before.label != marker.label) marker.faralign = true;
                }
            }
            int ix = 0;//panel * 2 - 1;
            foreach (ResultMarker marker in verticalMarkers)
            {
                cChart.AxisX.CustomGridLines.Add(new CustomGridLine());
                formatConstantLine(ix, cChart.AxisX.CustomGridLines[cl++], marker);
            }
            foreach (ResultMarker marker in horizontalMarkers)
            {
                cChart.AxisY.CustomGridLines.Add(new CustomGridLine());
                formatConstantLine(ix, cChart.AxisY.CustomGridLines[cl++], marker);
            }
        }
        private void formatConstantLine(int axisix, CustomGridLine line, ResultMarker marker)
        {
            double xvalue = marker.pos;
            if (marker.vertical)
                xvalue = (marker.pos - StartDateOA + StepOA) / StepOA;
            line.Value = xvalue;
            line.Color = marker.color;
            //if (axisix <= 1) axisix = (int)AxisItem.Y;
            //line.Axis = !marker.vertical;// ? AxisItem.X : ((AxisItem)axisix);
            line.Text = marker.label;
            line.Width = marker.width;
            line.TextColor = marker.color;
            if (marker.vertical)
            {
                line.LineAlignment = marker.faralign ? StringAlignment.Far : StringAlignment.Near;
                // up/down alignment of the text
                if (marker.value < pChart.AxisY.Max / 2) line.Alignment = StringAlignment.Far;
            }
            else
            {
                line.LineAlignment = StringAlignment.Far;
                line.Alignment = StringAlignment.Far;
            }
        }

        /*Unit Implementation in Web Services - Begin*/
        public void setChartInputValues(double dStnLat, double dStnLong, int altitude, int maxAltDiff, int maxDistDiff, string strModule, string strSvcName, string strCul, string strUnit)
        {
            /*Unit Implementation in Web Services - End*/
            dpi.stationLatitude = dStnLat;
            dpi.stationLongitude = dStnLong;
            //dpi.stationName = strStnName;
            dpi.NearbyPointSettings.MaxAllowedDistance = maxDistDiff;
            dpi.altitude = altitude;
            dpi.NearbyPointSettings.MaxAllowedAltitude = maxAltDiff;
            svi.Module = strModule;
            svi.ServiceName = strSvcName;
            svi.Culture = strCul;

            /* changed for agricast service */
            if (HttpContext.Current.Session != null && (ServiceInfo)HttpContext.Current.Session["serviceInfo"] != null)
            {
                var objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                objServiceInfo.Module = strModule;
                objServiceInfo.Culture = strCul;
                /*Unit Implementation in Web Services - Begin*/
                objServiceInfo.Unit = strUnit;
                /*Unit Implementation in Web Services - Begin*/
                HttpContext.Current.Session["serviceInfo"] = objServiceInfo;
            }
        }
        public DataTable GetChartDataForExport(DataTable agriInfoData)
        {
            return ChartSvc.getChartDataForExport(chartParams, chartName, culture, agriInfoData);
        }

        string getLegendFormat(bool formatflag = false)
        {
            Dictionary<string, string> agInfo = new Dictionary<string, string>();
            if (HttpContext.Current.Session["IAgriInfo"] != null)
                agInfo = (Dictionary<string, string>)HttpContext.Current.Session["IAgriInfo"];

            DateTime desiredDateFrom = DateTime.Parse(agInfo["startDate"].ToString());
            DateTime desiredDateTo = DateTime.Parse(agInfo["endDate"].ToString());
            string scale = agInfo["aggregation"].ToLower();
            int days = ((TimeSpan)(desiredDateTo - desiredDateFrom)).Days;
            bool addyear = desiredDateTo.Year != desiredDateFrom.Year;
            if (formatflag)
                addyear = true;
            int limitforday = 300;
            int limitformonth = 2922;
            bool addday = days < limitforday;
            bool addmonth = days < limitformonth;
            if (scale == "monthly") addday = false;
            string format = "";
            if (addday) format = "dd";
            if (addmonth)
            {
                if (addday)
                {
                    if (!addyear) format += "\n";
                    else format += " ";
                }
                format += "MMM";
            }
            if (addyear)
            {
                if (format.Length > 0) format += "\n";
                format += "yyyy";
            }
            return format;
        }
        public void getTranslatedText(string strLabelName, string strCultureCode)
        {
            Ichart.transText = ChartSvc.getTranslatedText(strLabelName, strCultureCode);
        }
    }
}
