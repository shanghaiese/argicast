using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Icon.Service;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using Syngenta.AgriCast.Icon.View;
using Syngenta.AgriCast.Icon.Presenter;
using System.Globalization;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using System.Data;
using System.Text.RegularExpressions;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common;


namespace Syngenta.AgriCast.Icon
{
    public partial class Icon : System.Web.UI.UserControl, IIcon
    {
        IconPresenter objIconPre;
        ServicePresenter objSvcPre = new ServicePresenter();
        ArrayList arDays = new ArrayList();
        LocationInfo objLocInfo;
        CommonUtil objComUtil = new CommonUtil();
        int width;
        const string RATE_IT = "Rate it";
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private void LoadIcons()
        {
            objIconPre = new IconPresenter(this, Name);
            objIconPre.getIconData();
            Service.IconService obj = new IconService();
            displayIcons(dtIconList);
        }

        protected void Page_prerender(object sender, EventArgs e)
        {
            try
            {
                objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];
                if (objLocInfo.DataPointInfo != null && objLocInfo.DataPointInfo.stationLatitude != 0 && objLocInfo.DataPointInfo.stationLongitude != 0 && objLocInfo.DataPointInfo.stationName != "")
                {
                    LoadIcons();
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.ICONS_LOAD_FAILURE) + ex.Message.ToString();
            }
        }

        //Method to display the icons in tabular format
        public void displayIcons(DataTable dt)
        {
            try
            {
                if (dt != null && dt.Rows.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    Table tb = new Table();
                    width = 100 / dt.Rows.Count;
                    sb.Append("<table id=\"weathericontable\" class=\"iconHeaderTableClass\">");
                    CultureInfo VariantCulture;
                    objIconPre.getCultureCode();
                    VariantCulture = new CultureInfo(strCulCode);
                    if (plotHeader)
                    {
                        sb.Append("<tr>");
                        for (int l = 0; l < (iDesiredNoOfDays); l++)
                        {
                            string strDay = (DateTime.Today.AddDays(l)).ToString("dddd", VariantCulture);
                            arDays.Add(strDay);
                            sb.Append("<td class=\"iconColumnClass\" style=\"width:10%\">" + strDay);
                        }
                        sb.Append("</tr>");
                    }
                    else
                    {
                        for (int l = 0; l < (iDesiredNoOfDays); l++)
                        {
                            string strDay = (DateTime.Today.AddDays(l)).ToString("dddd", VariantCulture);
                            arDays.Add(strDay);                             
                        }
                    }
                    sb.Append("<tr>");
                    int iTemp = 0;
                    for (int j = 0; j < (iDesiredNoOfDays); j++)
                    {
                        sb.Append("<td class=\"iconColumnClass\">");
                        sb.Append("<table>");
                        sb.Append("<tr>");
                        for (int i = 0; i < iColumnsPerDay; i++)
                        {
                            string file;
                            if (iTemp < dt.Rows.Count)
                                file = "../../Images/Icons/" + dt.Rows[iTemp]["ImageUrl"].ToString().Substring(dt.Rows[iTemp]["ImageUrl"].ToString().LastIndexOf("\\") + 1);
                            else
                                file = "";

                            string strToolTipText = "";

                            // To check if the aggreation is daily. If ture then the tooltip text will be formed.
                            if (iColumnsPerDay == 1)
                            {
                                strToolTipText = Regex.Replace(dt.Rows[iTemp]["TooltipText"].ToString(), "<.*?>", string.Empty);
                                strToolTipText = strToolTipText.Replace("{day}", arDays[iTemp].ToString()).Replace("{}",""); 
                            }
                            iTemp = iTemp + 1;
                            sb.Append("<td style=\"text-align:center;width:");
                            sb.Append(width);
                            if (file != "")
                            {
                                sb.Append("%\"><img title=\"" + strToolTipText + "\"");
                                sb.Append(" src=\"");
                                sb.Append(file);
                                sb.Append("\"");
                                sb.Append(" alt=\"\"/></td>");

                            }
                            else
                                sb.Append("%\"></td>");
                        }
                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                    sb.Append("</table>");
                    Control control = PlaceHolder1;
                    ControlCollection Controls = control.Controls;
                    Literal lt = new Literal();
                    StringDictionary pFiles = new StringDictionary();
                    lt.Text = sb.ToString();
                    if (bool.Parse(iFeedbackEnabled))
                    {
                        Label lblRating = new Label();
                        objIconPre.getTranslatedText(RATE_IT, strCulCode);
                        lblRating.Text = iTransText;
                        lblRating.CssClass = "label110";
                        HtmlGenericControl divRating = new HtmlGenericControl("div");
                        divRating.ID = "rating_Icon";
                        divRating.Attributes.Add("class", "RateIt");
                        divRating.Attributes.Add("data-accesskey", "rate");
                        divRating.ClientIDMode = ClientIDMode.Static;
                        //divRating.Attributes.Add("data-rateit-backingfld", "select#range");
                        HtmlGenericControl divOuter = new HtmlGenericControl("div");
                        divOuter.Attributes.Add("class", "Right");
                        divOuter.Controls.Add(lblRating);
                        divOuter.Controls.Add(divRating);
                        Controls.Add(divOuter);
                    }
                    Controls.Add(lt);
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.ICONS_DISPLAY_FAILURE) + ex.Message.ToString();
            }
        }        

        public string Name
        { get; set; }

        public int iDesiredNoOfDays
        {
            get;
            set;
        }

        public ArrayList alIocnList
        {
            get;
            set;
        }


        public int iColumnsPerDay
        {
            get;
            set;
        }


        public string strCulCode
        {
            get;
            set;
        }
        public string iTransText
        {
            get;
            set;
        }

        public DataTable dtIconList
        {
            get;
            set;
        }
        public string iFeedbackEnabled
        {
            get;
            set;
        }
        public bool plotHeader
        {
            get;
            set;
        }
    }
}