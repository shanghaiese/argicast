using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syngenta.AgriCast.Icon.DTO
{
    public class DTOIcon
    {
        /// <summary>
        /// it will set the No. of days desired.
        /// </summary>
        public int iNoOfdays
        {
            get;
            set;
        }
        /// <summary>
        /// it will set the step value.
        /// </summary>
        public int iStep
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
        public string strFeedbackEnabled
        {
            get;
            set;
        }
        public bool headerRow
        {
            get;
            set;
        }
    }
}