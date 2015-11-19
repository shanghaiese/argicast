using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
 

namespace Syngenta.AgriCast.Charting.DTO
{
    public class DTOchart  
    {
        #region Variables
        private ArrayList seriesDataList = new ArrayList(10);
        public ResultShade[] shades;
        public ResultMarker[] markers; 
        public double PrimaryLabelValue;
        public double SecondaryLabelValue;
        public double  minorStep;
        public double majorStep;        
        public double Ymax;
        public double Ymin;
        public double Y2max;
        public double Y2min;    
        public int noofvalues = 0;
        
        #endregion Variables
        #region Properties

        /// <summary>
        /// it will set the axis of the graph is visible or not.
        /// Add for IM01977477:AIS - Modify kecp01fao publication - 20140802
        /// </summary>
        public bool isInvisible
        {
            get;
            set;
        }
        
        /// <summary>
        /// it will set the watermark for graph background image.
        /// </summary>
        public string WaterMark
        {
            get;
            set;
        }
              
        /// <summary>
        /// it will set the height of graph.
        /// </summary>
        public int Height
        {
            get;          
            set;           
        }
        /// <summary>
        /// it will set the width of image.
        /// </summary>
        public int Width
        {
            get;            
            set;            
        }
        /// <summary>
        /// it will set the footer text of image.
        /// </summary>
        public string FooterText  
        {
            get;
            set;
        }
        
        /// <summary>
        /// it will set the labels for top axis.
        /// </summary>
        public string[] TopLabels
        {
            get;
            set;
        }

        /// <summary>
        /// it will set the labels for bottom axis.
        /// </summary>
        public double[] BottomLabels
        {
            get;
            set;
        }

        /// <summary>
        /// it will set the labels for Primary Y axis.
        /// </summary>
        public string[] PrimaryLabels
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the labels for Secondary Y axis.
        /// </summary>
        public string[] SecondaryLabels
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the label for right axis.
        /// </summary>
        public string RightAxisLabel
        {
            get;
            set;
        }
        
        
        /// <summary>
        /// it will set the label for left axis.
        /// </summary>
        public string LeftAxisLabel
        {
            get;
            set;
        }        
        
        
        /// <summary>
        /// It will set the label for bottom axis.
        /// </summary>
        public string BottomAxisLabel
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the scale for bottom axis.
        /// </summary>
        public double BottomScale
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the scale for top axis.
        /// </summary>
        public double TopScale
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the scale for right axis.
        /// </summary>
        public string RightScaleType
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the scale for left axis.
        /// </summary>
        public string LeftScaleType
        {
            get;
            set;
        }
       
        /// <summary>
        /// add a ResultSerie object to the list
        /// </summary>
        /// <param name="result"></param>
        public void addSeries(Series serie)
        {
            if (serie != null) seriesDataList.Add(serie);
        }

        /// <summary>
        /// return an Enumerator for all ResultSeries
        /// </summary>
        /// <returns></returns>
        public IEnumerator getSeriesList()
        {
            return seriesDataList.GetEnumerator();
        }

        /// <summary>
        /// returns the number of ResultSeries
        /// </summary>
        /// <returns></returns>
        public int getSeriesCount()
        {
            return seriesDataList.Count;
        }

        public string Title
        {
            get;
            set;
        }

        public bool TodayMarker
        {
            get ;
            set;
        }
        public bool HasFeedback
        {
            get;
            set;
        }
        public string warning
        {
            get;
            set;
        }
        
        #endregion Properties

    }
}
