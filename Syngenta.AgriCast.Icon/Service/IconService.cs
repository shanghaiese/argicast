using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Icon.RuleSet;
using System.Collections;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.DataAccess;
using Syngenta.AgriCast.Common;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Text;
using System.Configuration;
using System.Data;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Icon.View;
using Syngenta.AgriCast.Icon.DataAccess;
using System.Web.SessionState;
using Syngenta.Data.Access;
using Syngenta.AgriCast.Icon.DTO;
using Syngenta.AgriCast.Icon;
using System.Text.RegularExpressions;

namespace Syngenta.AgriCast.Icon.Service
{
    public class IconService
    {
        CommonUtil objCommon = new CommonUtil();
        IconData objIconData = new IconData();
        ServiceHandler objSvcHandler = new ServiceHandler();
        ServiceInfo objSvcInfo;
        TranslateData objTransData = new TranslateData();
        LocationInfo objLocInfo;
        DataPointInfo objDatapoint;
        DTOIcon objParams;
        int RiseMinutes;
        int SetMinutes;
        static double dLatitude = 0;
        static double dLongitude = 0;
        static int altitude = 0;
        static int maxAlt = 0;
        static int maxDist = 0;
        static DateTime dTSunrise;
        static DateTime dTSunset;
        string strServiceName;
        string strModuleName;
        static string strCultureCode;
        private ArrayList al = new ArrayList();
        DataSet dsData;
        DataTable dtData = new DataTable();
        int _start = 0;
        int _end = 0;
        string temporalAgg = "";
        string strImageSetName = "";
        string dtSource = "";
        DateTime current = System.DateTime.Today;
        DateTime startDate;
        DateTime endDate;
        int noofdays = 0;
        int step = 1;
        DataTable dtImgList;
        DataRow drImg;
        string iconPath = @"\Images\Icons\";
        string iconFolder = HttpRuntime.AppDomainAppPath + @"\Images\";
        WeatherData1 wData = new WeatherData1();
        FeatureRequest fReq;
        WeatherDataRequest wReq;
        WeatherDataResponse wRes;

        public void setIconWebServiceValues(double dLat, double dLong, int alt, int maxAltDiff, int maxDistDiff, DateTime dSunrise, DateTime dSunset, string strSvcName, string strModule, string strCulCode)
        {
            dLatitude = dLat;
            dLongitude = dLong;
            dTSunrise = dSunrise;
            dTSunset = dSunset;
            altitude = alt;
            maxAlt = maxAltDiff;
            maxDist = maxDistDiff;
            strServiceName = strSvcName;
            strModuleName = strModule;
            strCultureCode = strCulCode;
            objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];
            objLocInfo.DataPointInfo = null;
            objLocInfo.ServiceInfo = null;
            HttpContext.Current.Session["objLocationInfo"] = objLocInfo;
            HttpContext.Current.Session["objDataPointInfo"] = objLocInfo.DataPointInfo;
            /* SD01241630 - New Agricast webservices - can't get two views(iconsdailyandTabledata) - BEGIN -*/
            objSvcHandler.setSvcHandlerWebSvcValues(strServiceName, strModuleName);
            /* SD01241630 - New Agricast webservices - can't get two views(iconsdailyandTabledata) - END -*/
        }

        void populateObject()
        {
            /*Changes by jerrey - web service issue - added new condition*/
            //&& objLocInfo != null && objDatapoint != null)
            if (HttpContext.Current != null)
            {
                objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];

                //Web Service Issue
                if (objLocInfo.DataPointInfo != null && objLocInfo.ServiceInfo != null)
                {
                    objDatapoint = objLocInfo.DataPointInfo;
                    objSvcInfo = objLocInfo.ServiceInfo;
                }

                else
                {
                    objDatapoint = new DataPointInfo();
                    objLocInfo = new LocationInfo();
                    objSvcInfo = new ServiceInfo();

                    objDatapoint.SunRise = dTSunrise;
                    objDatapoint.SunSet = dTSunset;

                    objDatapoint.stationLatitude = dLatitude;
                    objDatapoint.stationLongitude = dLongitude;
                    objDatapoint.altitude = altitude;
                    objDatapoint.NearbyPointSettings.MaxAllowedDistance = maxDist;
                    objDatapoint.NearbyPointSettings.MaxAllowedAltitude = maxAlt;
                    objSvcInfo.Culture = strCultureCode;
                    objSvcHandler.setSvcHandlerWebSvcValues(strServiceName, strModuleName);

                    //Web service issue fix 
                    objLocInfo.DataPointInfo = objDatapoint;
                    HttpContext.Current.Session["objDataPointInfo"] = objDatapoint;

                    objLocInfo.ServiceInfo = objSvcInfo;

                    HttpContext.Current.Session["objLocationInfo"] = objLocInfo;
                }
            }
        }

        //Icon Generation calculations
        private void initSSMinutes()
        {
            DateTime currDate = DateTime.Now;
            DateTime rdate = objDatapoint.SunRise;
            DateTime sdate = objDatapoint.SunSet;
            RiseMinutes = rdate.Hour * 60 + rdate.Minute;
            SetMinutes = sdate.Hour * 60 + sdate.Minute;
            if (SetMinutes > RiseMinutes) SetMinutes -= 24 * 60;
        }

        void getPubSettings(string controlName)
        {
            dsData = new DataSet();
            dsData.Tables.Add(objSvcHandler.getIconSettings(controlName));
            dtSource = dsData.Tables["ServiceServicePageIcon"].Rows[0]["dataSource"].ToString();
            temporalAgg = dsData.Tables["ServiceServicePageIcon"].Rows[0]["temporalAggregation"].ToString();
            _start = Int32.Parse(dsData.Tables["ServiceServicePageIcon"].Rows[0]["startdate"].ToString());
            _end = Int32.Parse(dsData.Tables["ServiceServicePageIcon"].Rows[0]["enddate"].ToString());
            strImageSetName = dsData.Tables["ServiceServicePageIcon"].Rows[0]["imageSetName"].ToString();
            noofdays = _end - _start;
            dsData.Tables.Add(objSvcHandler.GetIconSeries(controlName));
        }

        DataTable getData(DataTable series, string aggregation, string dataSource, int start, int end)
        {
            Serie serie = new Serie("", "", false, null, "");
            List<Serie> lst = new List<Serie>();
            enumDataSource source;
            switch (dataSource.ToUpper())
            {
                case "GLBSTNFCST":
                    source = enumDataSource.glbStnFcst;
                    break;
                case "GLB25ECMWF":
                    source = enumDataSource.glb25ECMWF;
                    break;
                case "GLB25OBS":
                    source = enumDataSource.glb25Obs;
                    break;
                case "GLB25OBSEXT":
                    source = enumDataSource.glbStnObsExt;
                    break;
                case "GLB25OBSSYN":
                    source = enumDataSource.glbStnObsSyn;
                    break;
                default:
                    source = enumDataSource.glbStnFcst;
                    break;
            }

            enumTemporalAggregation tmpAggregation;
            switch (aggregation.ToLower())
            {
                case "hourly":
                    tmpAggregation = enumTemporalAggregation.Hourly;
                    step = 1;
                    break;
                case "daily":
                    tmpAggregation = enumTemporalAggregation.Daily;
                    step = 24;
                    break;
                case "8hourly":
                    tmpAggregation = enumTemporalAggregation.EightHourly;
                    step = 8;
                    break;
                case "12hourly":
                    tmpAggregation = enumTemporalAggregation.TwelveHourly;
                    step = 12;
                    break;
                default:
                    tmpAggregation = enumTemporalAggregation.Hourly;
                    break;
            }
            tmpAggregation = enumTemporalAggregation.Hourly;
            foreach (DataRow dr in series.Rows)
            {

                if (tmpAggregation == enumTemporalAggregation.Daily || tmpAggregation == enumTemporalAggregation.Hourly)
                    lst.Add(
                       new Serie(
                          dr["Name"].ToString()
                          , ""
                          , false
                          , null
                          , ""));
                else
                    lst.Add(
                     new Serie(
                        dr["Name"].ToString()
                        , ""
                        , false
                        , (enumAggregationFunction)Enum.Parse(typeof(enumAggregationFunction), dr["aggregationfunction"].ToString(), true)
                        , ""));
            }

            startDate = current.AddDays(start);
            //endDate = current.AddDays(end > 0 ? end - 1 : end + 1);
            endDate = current.AddDays(end);
            if (startDate > endDate)
            {
                DateTime tmp = startDate;
                startDate = endDate;
                endDate = tmp;
            }

            fReq = new FeatureRequest(objDatapoint.stationLatitude, objDatapoint.stationLongitude, objDatapoint.altitude, objDatapoint.NearbyPointSettings.MaxAllowedAltitude, objDatapoint.NearbyPointSettings.MaxAllowedDistance);
            wReq = new WeatherDataRequest(source, false, fReq, startDate, endDate, tmpAggregation, lst);
            wRes = wData.GetWeatherData(wReq);
            return wRes.WeatherData;

        }

        //Sets the value of parameters required for Icon generation
        private void setParams(DataSet config)
        {
            objParams.Height = Int32.Parse(config.Tables["ServiceServicePageIcon"].Rows[0]["height"].ToString());
            objParams.Width = Int32.Parse(config.Tables["ServiceServicePageIcon"].Rows[0]["width"].ToString());
            objParams.iNoOfdays = noofdays;
            objParams.iStep = step;
            objParams.strFeedbackEnabled = config.Tables["ServiceServicePageIcon"].Rows[0]["feedback"].ToString();
        }

        public DataTable getIconData(DTOIcon objParam, string controlName)
        {
            populateObject();
            objParams = objParam;
            getPubSettings(controlName);
            dtData = getData(dsData.Tables["ServiceServicePageIconSerie"], temporalAgg, dtSource, _start, _end).Copy();
            return dtData;
        }

        //Returns the list of iocns. This is called from Mobile page
        public ArrayList getWeatherIcons(DTOIcon objParam, string controlName)
        {
            dtImgList = new DataTable();
            dtImgList = getWeatherIconsWithTooltip(objParam, controlName);
            ArrayList images = new ArrayList();
            for (int i = 0; i < dtImgList.Rows.Count; i++)
                images.Add(dtImgList.Rows[i][0].ToString());
            return images;

        }

        //Returns list of icons with tooltip text.
        public DataTable getWeatherIconsWithTooltip(DTOIcon objParam, string controlName)
        {

            dtImgList = new DataTable();
            populateObject();
            objParams = objParam;
            if (HttpContext.Current != null)
            {
                getPubSettings(controlName);
            }
            objParams.headerRow = dsData.Tables["ServiceServicePageIcon"].Columns.Contains("plotColumnHeader") ? bool.Parse(dsData.Tables["ServiceServicePageIcon"].Rows[0]["plotColumnHeader"].ToString()) : true;
            dtData = getData(dsData.Tables["ServiceServicePageIconSerie"], temporalAgg, dtSource, _start, _end).Copy();
            //Error in Agricast - Reported by Jerome - Begin*/
            // DataTable dt = step != 1 ? decimate(dtData, dsData.Tables["ServiceServicePageIconSerie"], step) : dtData;
            DataTable dt = decimate(dtData, dsData.Tables["ServiceServicePageIconSerie"], step);
            //Error in Agricast - Reported by Jerome - End*/
            dt.TableName = "IconData";
            dsData.Tables.Add(dt);

            setParams(dsData);
            initSSMinutes();
            dtImgList.Columns.Add("ImageUrl");
            dtImgList.Columns.Add("TooltipText");

            rs_weather ruleset = null;
            ArrayList images = new ArrayList();
            DateTime cdate = DateTime.Now;
            cdate = new DateTime(cdate.Year, cdate.Month, cdate.Day, 0, 0, 0, 0);
            // get time offest for image placement
            int middlenight = 60 * step / 2;
            if (step == 12) middlenight = (RiseMinutes + SetMinutes) / 2;
            if (middlenight > 12 * 60) middlenight -= 12 * 60;
            if (step == 12) middlenight += step * 60; // offset the aggregated data            
            DataRow dr;
            cdate = cdate.AddDays(noofdays);

            string isPrintorPDF = "";
            if (CommonUtil.IsSessionAvailable() && HttpContext.Current.Session["isPrintorPDF"] != null)
            {
                isPrintorPDF = HttpContext.Current.Session["isPrintorPDF"].ToString();
                HttpContext.Current.Session.Remove("isPrintorPDF");
            }
            for (int i = 0; i < noofdays * 24 / step; i++)
            {
                DateTime positionDate = cdate.AddMinutes(middlenight);
                bool isnight = objCommon.isNight(positionDate, objDatapoint.stationLatitude, objDatapoint.stationLongitude);
                if (i < dsData.Tables["IconData"].Rows.Count)
                {
                    dr = dsData.Tables["IconData"].Rows[i];
                    //string img = runImageRuleset(ruleset, i, isnight, dr, objParams.Width, objParams.Height, objSvcInfo.IsMobile?"gif":Constants.ICON_IMAGEFORMAT);
                    string img = runImageRuleset(ruleset, i, isnight, dr, objParams.Width, objParams.Height, isPrintorPDF != "" ? "jpeg" : Constants.ICON_IMAGEFORMAT);

                    if (img != null)
                    {
                        images.Add(img);
                    }
                }
                middlenight += step * 60;
            }

            for (int j = 0; j < dtImgList.Rows.Count; j++)
            {
                dtImgList.Rows[j]["ImageUrl"] = images[j].ToString();
            }
            return dtImgList;
        }

        /// <summary>
        /// generate an image for a weather icon, depending on the given ruleset<br/>
        /// isnight must be passed since daybaseix is not exact enough, especially when timezone is changed (++ML)
        /// </summary>
        /// <param name="ruleset"></param>
        /// <param name="daybaseix"></param>
        /// <returns></returns>
        public string runImageRuleset(rs_weather ruleset, int daybaseix, bool isnight, DataRow dr, int width, int height, string imageformat)
        {
            //RuleSet objRuleset = new RuleSet();
            string imgname = "";
            string inputimgformat = Constants.ICON_IMAGEFORMAT;
            int offset = 2;
            string[] images = new string[15 + offset];

            // add the base day/night image as the first element
            string opaque = (objSvcInfo.IsMobile || imageformat == "jpeg") ? "true" : "false"; //IconConstants.OPAQUE;             
            if (opaque == null || opaque.ToLower() == "false")
            { //transparent background                 
                images[0] = iconFolder + Constants.ICON_TRANSPARENT + "." + inputimgformat;
            }
            else
            { //opaque background                
                images[0] = iconFolder + Constants.ICON_OPAQUE + "." + inputimgformat;
            }
            if (isnight)
            { //night time                 
                images[1] = iconFolder + strImageSetName + Constants.ICON_ISNIGHT + "." + inputimgformat;
            }
            else
            { //day time                 
                images[1] = iconFolder + strImageSetName + Constants.ICON_ISDAY + "." + inputimgformat;
            }
            int count = offset;

            ruleset = new rs_weather();
            ArrayList ar = new ArrayList();
            DataTable dtImg = new DataTable();
            string strSentenceBase = objTransData.getTranslatedText("wt_sentencebase", objSvcInfo.Culture);
            dtImg = ruleset.applyRuleSet(dr, strImageSetName);
            //for(int j = 0; j < ar.Count; j++)
            for (int j = 0; j < dtImg.Rows.Count; j++)
            {

                if (dtImg.Rows[j]["ImgName"].ToString() != null && dtImg.Rows[j]["ImgName"].ToString() != "")
                {
                    string singleimgname = dtImg.Rows[j]["ImgName"].ToString();
                    images[count++] = iconFolder + singleimgname + "." + inputimgformat;
                }

                //if (dtImg.Rows[j]["TransId"].ToString() != null && dtImg.Rows[j]["TransId"].ToString() != "")
                //{
                string transText = objTransData.getTranslatedText(dtImg.Rows[j]["TransId"].ToString(), objSvcInfo.Culture);
                transText = " " + transText + " ";
                string key = dtImg.Rows[j]["ReplaceText"].ToString();
                strSentenceBase = strSentenceBase.Replace("{" + key + "}", transText);
                if (key.ToLower() == "winddir")
                {
                    int degrees = Convert.ToInt32(dtImg.Rows[j]["Value"]);
                    strSentenceBase = strSentenceBase.Replace("{" + key + "}", objCommon.getTextDirection(degrees));
                }
                //}

            }

            //Replace Multiple spaces with Single Space
            string strPattern = "[\\s]+";
            Regex reg = new Regex(strPattern, RegexOptions.IgnoreCase);
            strSentenceBase = reg.Replace(strSentenceBase, " ");

            //Adding Tooltip text in the Datatable
            drImg = dtImgList.NewRow();
            drImg["TooltipText"] = strSentenceBase;
            dtImgList.Rows.Add(drImg);



            imgname += "." + inputimgformat;
            //build the image
            return CreateImage(
                images,
                imageformat,
                width,
                height
                );
        }
        /// <summary>
        /// combine the filenames of a list of paths/filenames 
        /// </summary>
        /// <param name="imageList"></param>
        /// <returns></returns>
        private string mergeFilenames(string[] imageList)
        {
            string newname = "";
            foreach (string path in imageList)
            {
                if (path != null)
                    newname += Path.GetFileNameWithoutExtension(path);
            }
            return newname;
        }
        /// <summary>
        /// merge image using specific sizes 
        /// </summary>
        /// <param name="imgs"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public string CreateImage(string[] imgs, string format, int width, int height)
        {
            string temppath = HttpRuntime.AppDomainAppPath + iconPath;
            //build filename
            if (format == null || format == "") format = "png";
            string imgfilename = mergeFilenames(imgs) + "." + format;
            if (width != 0 && height != 0)
                imgfilename = width.ToString() + "x" + height.ToString() + "_" + imgfilename;
            ////build final path
            string imgpath = Path.Combine(temppath, imgfilename);

            //create image if needed
            if (!File.Exists(imgpath))
            {
                Byte[] buffer;
                if (width != 0 && height != 0)
                {
                    buffer = mergeImages(imgs, format, width, height);
                }
                else
                {
                    buffer = mergeImages(imgs, format, 0, 0);
                }
                //write the file to disk
                FileStream file = File.Open(imgpath, FileMode.Create);
                file.Write(buffer, 0, buffer.Length);
                file.Close();
            }
            return imgpath;
        }

        /// <summary>
        /// merge list of image paths into a raw image (scaled) 
        /// first element will be the base image
        /// </summary>
        /// <param name="imageList"></param>
        /// <param name="imgtype"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Byte[] mergeImages(string[] imageList, string imgtype, int width, int height)
        {
            //open all individual images
            Image[] images = new Image[imageList.Length];
            for (int i = 0; i < imageList.Length; i++)
            {
                if (imageList[i] != null)
                    images[i] = Image.FromFile(imageList[i]);
            }
            MemoryStream s = new MemoryStream();
            // merge the image and resize if necessary
            Image mergedImage = _mergeImages(images);
            ImageFormat imgFormat = getImageFormat(imgtype);
            if (width != 0 && height != 0)
            {
                imgResize(mergedImage, width, height).Save(s, imgFormat);
            }
            else
            {
                mergedImage.Save(s, imgFormat);
            }
            //close individual images
            foreach (Image image in images)
            {
                if (image != null)
                    image.Dispose();
            }
            return s.ToArray();
        }
        /// <summary>
        /// resize an image
        /// </summary>
        /// <param name="input"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Image imgResize(Image input, int width, int height)
        {
            //image dumped to stream and converted back to image
            //this is necessary to obtain final image!
            MemoryStream tmp = new MemoryStream();
            input.Save(tmp, ImageFormat.Png);
            Image newimg = Image.FromStream(tmp);
            //resize image using "GetThumbnailImage"
            System.Drawing.Image.GetThumbnailImageAbort dummyCallback = new System.Drawing.Image.GetThumbnailImageAbort(dummyFunction);
            return newimg.GetThumbnailImage(width, height, dummyCallback, IntPtr.Zero);
        }
        ///// <summary>
        ///// needed for "GetThumbnailImage
        ///// </summary>
        ///// <returns></returns>
        private bool dummyFunction()
        {
            return false;
        }
        /// <summary>
        /// helper for "mergeImages" to merge a list of .NET images 
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        private Image _mergeImages(Image[] images)
        {
            Graphics img = Graphics.FromImage(images[0]);
            //draw each image on top of base image
            for (int i = 1; i < images.Length; i++)
            {
                if (images[i] != null)
                {
                    //draw the image overlay
                    img.DrawImageUnscaled(images[i], 0, 0);
                    img.Save();
                }
            }
            img.Dispose();
            return images[0];
        }
        /// <summary>
        /// convert a string to an ImageFormat type 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private ImageFormat getImageFormat(string format)
        {
            switch (format.ToLower())
            {
                case "png":
                    return ImageFormat.Png;
                case "jpg":
                    return ImageFormat.Jpeg;
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "gif":
                    return ImageFormat.Gif;
                case "tiff":
                    return ImageFormat.Tiff;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return objTransData.getTranslatedText(strLabelName, strCultureCode);
        }

        /// <summary>
        /// decimate the hourly data. 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private DataTable decimate(DataTable dtData, DataTable series, int decimateFactor)
        {
            if (dtData == null) return null;
            DataTable dtFinal = new DataTable();
            dtData.Columns.Add("Preciptypemin");
            dtData.Columns.Add("Preciptypemax");
            dtData.Columns.Add("tempground");

            DataRow serierow = series.NewRow();
            serierow["name"] = "Preciptypemin";
            serierow["aggregationFunction"] = "min";
            series.Rows.Add(serierow);

            serierow = series.NewRow();
            serierow["name"] = "Preciptypemax";
            serierow["aggregationFunction"] = "max";
            series.Rows.Add(serierow);

            serierow = series.NewRow();
            serierow["name"] = "tempground";
            serierow["aggregationFunction"] = "min";
            series.Rows.Add(serierow);
            series.AcceptChanges();

            for (int row = 0; row < dtData.Rows.Count; row++)
            {
                if (dtData.Rows[row]["PrecipitationType"].ToString() != "")
                {
                    dtData.Rows[row]["Preciptypemax"] = Convert.ToDouble(dtData.Rows[row]["PrecipitationType"].ToString()) < 1 ? 2.5 : dtData.Rows[row]["PrecipitationType"];
                    dtData.Rows[row]["Preciptypemin"] = Convert.ToDouble(dtData.Rows[row]["PrecipitationType"].ToString()) < 1 ? 2.5 : dtData.Rows[row]["PrecipitationType"];
                }
                dtData.Rows[row]["tempground"] = dtData.Rows[row]["TempAir_C"];
            }

            dtFinal.Columns.Add("date");
            foreach (DataRow dr in series.Rows)
            {
                string colName = dr["name"].ToString();
                string rule = dr["aggregationFunction"].ToString();
                dtFinal.Columns.Add(colName);
                Type typ = dtData.Rows[0][colName].GetType();
                double[] source = (from DataRow row in dtData.Select()
                                   select Convert.ToDouble(row[colName].ToString() != "" ? row[colName].ToString() : "0")).ToArray();

                int max = source.Length / decimateFactor;
                double[] dest = new double[max];
                double value;
                int i = 0;
                switch (rule)
                {
                    case "min":
                        for (int x = 0; x < max; x++)
                        {
                            value = double.MaxValue;
                            for (int y = 0; y < decimateFactor; y++, i++)
                            {
                                if (value > source[i]) value = source[i];
                            }
                            dest[x] = value;
                        }
                        break;
                    case "max":
                        for (int x = 0; x < max; x++)
                        {
                            value = double.MinValue;
                            for (int y = 0; y < decimateFactor; y++, i++)
                            {
                                if (value < source[i]) value = source[i];
                            }
                            dest[x] = value;
                        }
                        break;
                    case "sum":
                        for (int x = 0; x < max; x++)
                        {
                            value = 0;
                            for (int y = 0; y < decimateFactor; y++, i++) value += source[i];
                            dest[x] = value;
                        }
                        break;
                    case "dev": // calculate standard deviation GAP CHECK missing
                        for (int x = 0; x < max; x++)
                        {
                            value = 0;
                            double sum1 = 0;
                            double sum2 = 0;

                            for (int y = 0; y < decimateFactor; y++, i++)
                            {
                                sum1 += Math.Pow(source[i], 2);
                                sum2 += source[i];
                            }
                            sum1 = decimateFactor * sum1;
                            sum2 = Math.Pow(sum2, 2);
                            value = (float)Math.Sqrt((sum1 - sum2) /
                                (decimateFactor * (decimateFactor - 1)));
                            dest[x] = value;
                        }
                        break;
                    default:  // average
                        for (int x = 0; x < max; x++)
                        {
                            value = 0;
                            for (int y = 0; y < decimateFactor; y++, i++) value += source[i];
                            dest[x] = value / decimateFactor;
                        }
                        break;
                }
                for (int j = 0; j < dest.Length; j++)
                {
                    if (dtFinal.Rows.Count > j)
                    {
                        dtFinal.Rows[j][colName] = dest[j];
                    }
                    else
                    {
                        DataRow newrow = dtFinal.NewRow();
                        dtFinal.Rows.Add(newrow);
                        //Error in Agricast - Reported by Jerome - Begin*/
                        if (decimateFactor == 1)
                            dtFinal.Rows[j]["date"] = dtData.Rows[j * decimateFactor]["date"];
                        else
                            dtFinal.Rows[j]["date"] = dtData.Rows[j * decimateFactor + 1]["date"];
                        //Error in Agricast - Reported by Jerome - End*/

                        dtFinal.Rows[j][colName] = dest[j];
                    }

                }
            }

            return dtFinal;
        }
    }
}