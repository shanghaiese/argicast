using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syngenta.AgriCast.Charting.DTO
{
    public class Series 
    {  
        
        public double MajorY;
        public double MinorY;        
        public double?[] values = {0.0};
        public string Scale
        {
            get;
            set;
        }
        public string MarkerType 
        {
            get;
            set;
        }
        public string SerieName
        {
            get;
            set;
        }
        public string Gallery
        {
            get;
            set;
        }
        public bool Stacked
        {
            get;
            set;
        }
        public string Color
        {
            get;
            set;
        }
        public string Position
        {
            get;
            set;
        }
        public int pane
        {
            get;
            set;
        }
        public string axisLabel
        {
            get;
            set;
        }
        public bool hasvalues
        {
            get;
            set; 
        }
        public string serieType
        { 
            get;
            set;
        }
        public bool inverted
        {
            get;
            set;
        }
        public bool hasGaps
        {
            get;
            set;
        }
    }

    public class ResultShade
    {
        public double max;
        public double min;
        public System.Drawing.Color color;        
        public string label; 
        
        public ResultShade(double pMin, double pMax, string colorHTML)
        {
            max = pMax; min = pMin; color = System.Drawing.ColorTranslator.FromHtml(colorHTML);             
        }
        
    }

    public class ResultMarker : IComparable
    {
        public bool vertical;
        public string label;
        public string shortlabel;
        public System.Drawing.Color color;
        public int panel;
        public int width = 1;
        static Type type = new ResultMarker().GetType();
        public static Type Type
        {
            get { return type; }
        }
        /// <summary>
        /// the position. if vertical and datescale, this value is an OA date
        /// </summary>
        public double pos; 
        public float value;
        public bool faralign;
        private ResultMarker()
        {
        }
        public ResultMarker(double pPos, float pValue, string colorHTML, string pLabel)
            : this(1, pPos, pValue, colorHTML, pLabel)
        {
        }
        public ResultMarker(int pPanel, double pPos, float pValue, string colorHTML, string pLabel)
        {
            panel = pPanel;
            color = System.Drawing.ColorTranslator.FromHtml(colorHTML);
            pos = pPos;
            value = pValue;
            label = pLabel;
            vertical = true;
            shortlabel = "";
        }
        public int CompareTo(object obj2)
        {
            ResultMarker m2 = (ResultMarker)obj2;
            if (pos > m2.pos) return 1;
            if (pos < m2.pos) return -1;
            return 0;
        }
    }

    //public class NiceScale
    //{
    //    private double minPoint;
    //    private double maxPoint;
    //    private double maxTicks = 5;
    //    private double tickSpacing;
    //    private double range;
    //    private double niceMin;
    //    private double niceMax;

    //    /**      * Instantiates a new instance of the NiceScale class.      *      
    //     * * @param min the minimum data point on the axis      
    //     * * @param max the maximum data point on the axis      */

    //    public NiceScale(double min, double max)
    //    {
    //        this.minPoint = min;
    //        this.maxPoint = max;
    //        calculate();
    //    }
    //    /**      * Calculate and update values for tick spacing and nice      
    //     * * minimum and maximum data points on the axis.      */
    //    private void calculate()
    //    {
    //        this.range = niceNum(maxPoint - minPoint, false);
    //        this.tickSpacing = niceNum(range / (maxTicks - 1), true);
    //        this.niceMin = Math.Floor(minPoint / tickSpacing) * tickSpacing;
    //        this.niceMax = Math.Ceiling(maxPoint / tickSpacing) * tickSpacing;
    //    }

    //    /**      * Returns a "nice" number approximately equal to range Rounds      
    //     * * the number if round = true Takes the ceiling if round = false.      
    //     * *      * @param range the data range      * @param round whether to round the result      
    //     * * @return a "nice" number to be used for the data range      */
    //    private double niceNum(double range, bool round)
    //    {
    //        double exponent; /** exponent of range */
    //        double fraction; /** fractional part of range */
    //        double niceFraction; /** nice, rounded fraction */
    //        exponent = Math.Floor(Math.Log10(range));
    //        fraction = range / Math.Pow(10, exponent);
    //        if (round)
    //        {
    //            if (fraction < 1.5)
    //                niceFraction = 1;
    //            else if (fraction < 3)
    //                niceFraction = 2;
    //            else if (fraction < 7)
    //                niceFraction = 5;
    //            else
    //                niceFraction = 10;
    //        }
    //        else
    //        {
    //            if (fraction <= 1)
    //                niceFraction = 1;
    //            else if (fraction <= 2)
    //                niceFraction = 2;
    //            else if (fraction <= 5)
    //                niceFraction = 5;
    //            else
    //                niceFraction = 10;
    //        }
    //        return niceFraction * Math.Pow(10, exponent);
    //    }

    //    /**      * Sets the minimum and maximum data points for the axis.      
    //     * *      * @param minPoint the minimum data point on the axis      
    //     * * @param maxPoint the maximum data point on the axis      */

    //    public void setMinMaxPoints(double minPoint, double maxPoint)
    //    {
    //        this.minPoint = minPoint; this.maxPoint = maxPoint; calculate();
    //    }

    //    /**      * Sets maximum number of tick marks we're comfortable with      
    //     * *      * @param maxTicks the maximum number of tick marks for the axis      */
    //    public void setMaxTicks(double maxTicks)
    //    {
    //        this.maxTicks = maxTicks; calculate();
    //    }

    //    /**      * Gets the tick spacing.      *      
    //     * * @return the tick spacing      */
    //    public double getTickSpacing()
    //    {
    //        return tickSpacing;
    //    }

    //    /**      * Gets the "nice" minimum data point.      *     
    //     * * @return the new minimum data point for the axis scale      */
    //    public double getNiceMin()
    //    {
    //        return niceMin;
    //    }

    //    /**      * Gets the "nice" maximum data point.      *      
    //     * * @return the new maximum data point for the axis scale      */
    //    public double getNiceMax()
    //    {
    //        return niceMax;
    //    }
    //}
}