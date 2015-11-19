using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Syngenta.Data.Access
{
    public enum enumDataSource { glb25Obs, glb25ECMWF, glbStnFcst, glbStnObsSyn, glbStnObsExt, EU_25, us04Nexrad };
    public enum enumTemporalAggregation { Hourly, EightHourly, TwelveHourly, Daily, Weekly, Decade, Monthly };
    public enum enumAggregationFunction { max, min, avg, sum, accsum, accmin, accavg, accmax};
    public enum enumTrend { TrendLTA, TrendMax, TrendMin };

    public class Serie
    {
        /// <summary> 
        /// Public constructor
        /// </summary>
        /// <param name="Name">Name</param> 
        /// <param name="Units">Units</param>
        /// <param name="AltitudeAdjustment">AltitudeAdjustment</param>
        /// <param name="AggregationFunction">AggregationFunction</param>
        /// <param name="Trend">Trend</param>
        public Serie(string name, string units, bool altitudeAdjustment,
           enumAggregationFunction? aggregationFunction, string trend)
        {
            this.Name = name;
            this.Units = units;
            this.AltitudeAdjustment = altitudeAdjustment;
            this.AggregationFunction = aggregationFunction;
            this.Trend = trend;

            if (altitudeAdjustment == true)
                this.SeriesName = aggregationFunction + name + units + trend + "_AltAdj";
            else
                this.SeriesName = aggregationFunction + name + units + trend;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Units
        /// </summary>
        public string Units { get; set; }
        /// <summary>
        /// AltitudeAdjustment
        /// </summary>
        public bool AltitudeAdjustment { get; set; }
        /// <summary>
        /// AggregationFunction
        /// </summary>
        public enumAggregationFunction? AggregationFunction { get; set; }
        /// <summary>
        /// Trend
        /// </summary>
        public string Trend { get; set; }
        /// <summary>
        /// SeriesName
        /// </summary>
        public string SeriesName { get; private set; }

        public string getSeriesName()
        {
            return this.SeriesName;
        }
    }

    public class FeatureRequest
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="Latitude">Latitude</param>
        /// <param name="Longitude">Longitude</param>
        /// <param name="Altitude">Altitude</param>
        /// <param name="MaxAltitudeDiff">Maximum Altitude Difference</param>
        /// <param name="MaxDistanceDiff">Maximum Distance Difference</param>
        /// 

        public FeatureRequest()
        {
        }

        public FeatureRequest(double latitude, double longitude, double altitude,
             double maxAltitudeDiff, double maxDistanceDiff)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
            this.MaxAltitudeDiff = maxAltitudeDiff;
            this.MaxDistanceDiff = maxDistanceDiff;
        }
        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Altitude
        /// </summary>
        public double Altitude { get; set; }
        /// <summary>
        /// Maximum Altitude Difference
        /// </summary>
        public double MaxAltitudeDiff { get; set; }
        /// <summary>
        /// Maximum Distance Difference
        /// </summary>
        public double MaxDistanceDiff { get; set; }

    }

    public class WeatherDataRequest
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="DataSource">Data source</param>
        /// <param name="GapFillSource">Gap fill source</param>
        /// <param name="FeatureRequest">FeatureRequest</param>
        /// <param name="StartDate">Start date</param>
        /// <param name="EndDate">End date</param>
        /// <param name="temporalAggregation">Temporal Aggregation</param>
        /// <param name="Series">Series</param>
        public WeatherDataRequest(enumDataSource dataSource, bool gapFillSource, FeatureRequest featureRequest,
            DateTime startDate, DateTime endDate, enumTemporalAggregation temporalAggregation, List<Serie> series)
        {
            this.DataSource = dataSource;
            this.GapFillSource = gapFillSource;
            this.FeatureRequest = featureRequest;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Series = series;
            this.temporalAggregation = temporalAggregation;
        }

        /// <summary>
        /// Data source
        /// </summary>
        public enumDataSource DataSource { get; set; }
        /// <summary>
        /// Gap fill source
        /// </summary>
        public bool GapFillSource { get; set; }
        /// <summary>
        /// Latitude of location
        /// </summary>
        public FeatureRequest FeatureRequest { get; set; }
        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Temporal Aggregation
        /// </summary>
        public enumTemporalAggregation temporalAggregation { get; set; }
        /// <summary>
        /// Trend
        /// </summary>
        public List<Serie> Series { get; set; }
    }

    public class FeatureResponse
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="DataPointID">Data point ID</param>
        /// <param name="Latitude">Latitude</param>
        /// <param name="Longitude">Longitude</param>
        /// <param name="Altitude">Altitude</param>
        /// <param name="Distance">Distance</param>
        /// <param name="BearingDegrees">Bearing degrees</param>
        internal FeatureResponse(int FeatureResponseId, double Latitude, double Longitude, double Altitude,
            double Distance, double BearingDegrees)
        {
            this.FeatureResponseId = FeatureResponseId;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Altitude = Altitude;
            this.Distance = Distance;
            this.BearingDegrees = BearingDegrees;
        }

        /// <summary>
        /// Data point ID
        /// </summary>
        public int FeatureResponseId { get; private set; }
        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; private set; }
        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; private set; }
        /// <summary>
        /// Altitude
        /// </summary>
        public double Altitude { get; private set; }
        /// <summary>
        /// Distance
        /// </summary>
        public double Distance { get; private set; }
        /// <summary>
        /// Bearing degrees
        /// </summary>
        public double BearingDegrees { get; private set; }
    }

    public class WeatherDataResponse
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="FeatureResponse">Feature Response</param>
        /// <param name="Series">Series</param>
        internal WeatherDataResponse(FeatureResponse featureResponse, List<Serie> series, DataTable weatherData)
        {
            this.FeatureResponse = featureResponse;
            this.Series = series;
            this.WeatherData = weatherData;
        }

        /// <summary>
        /// FeatureResponse
        /// </summary>
        public FeatureResponse FeatureResponse { get; private set; }
        /// <summary>
        /// Series
        /// </summary>
        public List<Serie> Series { get; private set; }
        /// <summary>
        /// Weather Data Table
        /// </summary>
        public DataTable WeatherData { get; set; }
    }
    public class WeatherData1
    {


        public WeatherDataResponse GetWeatherData(WeatherDataRequest weatherDatarequest, bool IsAgrinfo = false, int stationId = 0)
        {
            try
            {
                string str = "";
                switch (weatherDatarequest.temporalAggregation.ToString().ToLower())
                {
                    case "eighthourly":
                        str = "8hourly";
                        break;
                    case "twelvehourly":
                        str = "12hourly";
                        break;
                    default:
                        str = weatherDatarequest.temporalAggregation.ToString();
                        break;
                }

                Database dbGrows = new SqlDatabase(ConfigurationManager.ConnectionStrings["P_Grows"].ToString());
                DbCommand dbCommand = dbGrows.GetStoredProcCommand("Agriweb_Get_WeatherData");
                dbCommand.CommandTimeout = 120;
                dbGrows.AddInParameter(dbCommand, "latitude", DbType.Decimal, weatherDatarequest.FeatureRequest.Latitude);
                dbGrows.AddInParameter(dbCommand, "longitude", DbType.Decimal, weatherDatarequest.FeatureRequest.Longitude);
                dbGrows.AddInParameter(dbCommand, "altitude", DbType.Int32, weatherDatarequest.FeatureRequest.Altitude);
                if (weatherDatarequest.FeatureRequest.MaxDistanceDiff == 0d)
                    dbGrows.AddInParameter(dbCommand, "maxDistanceDiff", DbType.Int32, DBNull.Value);
                else
                    dbGrows.AddInParameter(dbCommand, "maxDistanceDiff", DbType.Int32, weatherDatarequest.FeatureRequest.MaxDistanceDiff);
                if (weatherDatarequest.FeatureRequest.MaxAltitudeDiff == 0d)
                    dbGrows.AddInParameter(dbCommand, "maxAltitudeDiff", DbType.String, DBNull.Value);
                else
                    dbGrows.AddInParameter(dbCommand, "maxAltitudeDiff", DbType.String, weatherDatarequest.FeatureRequest.MaxAltitudeDiff);
                dbGrows.AddInParameter(dbCommand, "dataSource", DbType.String, weatherDatarequest.DataSource);
                dbGrows.AddInParameter(dbCommand, "gapFillSource", DbType.Boolean, weatherDatarequest.GapFillSource);
                dbGrows.AddInParameter(dbCommand, "temporalAggregation", DbType.String, str);
                dbGrows.AddInParameter(dbCommand, "startDate", DbType.DateTime, weatherDatarequest.StartDate);
                dbGrows.AddInParameter(dbCommand, "endDate", DbType.DateTime, weatherDatarequest.EndDate);
                for (int i = 0; i < weatherDatarequest.Series.Count; i++)
                {
                    dbGrows.AddInParameter(dbCommand, "series" + (i + 1), DbType.String, weatherDatarequest.Series[i].SeriesName);
                }
                if (IsAgrinfo && stationId != 0)
                    dbGrows.AddInParameter(dbCommand, "dataPointID", DbType.Int32, stationId);
                DataSet dsData = new DataSet();
                dsData = dbGrows.ExecuteDataSet(dbCommand);
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                FeatureResponse featureResponse = null;
                WeatherDataResponse weatherDataResponse = null;
                if (IsAgrinfo && stationId != 0)
                {
                    featureResponse = new FeatureResponse(stationId, weatherDatarequest.FeatureRequest.Latitude,
                       weatherDatarequest.FeatureRequest.Longitude, 0, 0, 0);
                    weatherDataResponse = new WeatherDataResponse(featureResponse, weatherDatarequest.Series, dsData.Tables[0]);
                }
                else
                {
                    featureResponse = new FeatureResponse((int)dsData.Tables[0].Rows[0]["dataPointID"], Convert.ToDouble(dsData.Tables[0].Rows[0]["latitude"]),
                       Convert.ToDouble(dsData.Tables[0].Rows[0]["longitude"]), Convert.ToDouble(dsData.Tables[0].Rows[0]["altitude"]), Convert.ToDouble(dsData.Tables[0].Rows[0]["distance"]), Convert.ToDouble(dsData.Tables[0].Rows[0]["bearingDegrees"]));
                    weatherDataResponse = new WeatherDataResponse(featureResponse, weatherDatarequest.Series, dsData.Tables[1]);
                }

                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

                
                return weatherDataResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public static string Version()
        {
            return "$Id: WeatherData.cs 0.0v 2012-02-01 17:30:00Z sandeep rayasa $\n";
        }
    }
}

