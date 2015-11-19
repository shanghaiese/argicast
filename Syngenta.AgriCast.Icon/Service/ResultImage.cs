using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syngenta.AgriCast.Icon.Service
{
    /// <summary>
    /// An image on the chart with a specified path for the png/jpg file and x/y position
    /// </summary>
    public class ResultImage
    {
        public string path;
		public double xvalue;
		public double yvalue;
        public ResultImage(string ppath, double pxvalue, double pyvalue)
        {
            path = ppath;
            xvalue = pxvalue;
            yvalue = pyvalue; 
        }
    }
}