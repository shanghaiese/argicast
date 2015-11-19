using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ChartFX.WebForms;
using System.Data;

namespace Syngenta.AgriCast.Charting.View
{
    public interface IChart
    { 
        string title
        {
            get;
            set;
        }
        string footerText
        {
            get;
            set;
        }
        string cultureCode
        {
            get;
            set;
        }    
       
        string imageUrl
        {
            get;
            set;
        }

        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
        /* 3.3.1	Charting component should have zooming enabled. */
        string bigImageUrl
        {
            get;
            set;
        }
        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

         Chart fChart
        {
            get;
            set;
        }

         DataTable serieData
         {
             get;
             set;
         }
         bool hasFeedback
         {
             get;
             set;
         }

         string transText
         {
             get;
             set;
         }

        /* alignment issue for agriInfo chart - jerrey - Start */
         string cssStyle
         {
             get;
             set;
         }
        /* alignment issue for agriInfo chart - jerrey - End */
    }
}
