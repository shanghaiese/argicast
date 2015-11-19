using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Charting.Presenter;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Charting.View;
using System.Data;
using ChartFX.WebForms;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;

public partial class Mobile_ChartView : System.Web.UI.Page, IChart
{
    ServiceInfo objSvcInfo;
    ChartPresenter objPresenter; 
    LocationInfo objLocInfo;
    DataPointInfo objDataPointInfo;
    ServiceHandler objServiceHandler = new ServiceHandler();
    ServicePresenter objSerPre = new ServicePresenter();
    CommonUtil objCommonUtil = new CommonUtil();
  
    List<string[]> al;
    ChartFX.WebForms.Chart Chart1 = new ChartFX.WebForms.Chart();
    const string SERVICEPAGENAME = "servicePage"; 

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session == null || Session["serviceInfo"] == null)
        {
            objSvcInfo = new ServiceInfo();
        }
        else
        {
            objSvcInfo = (ServiceInfo)Session["serviceInfo"];
        }
        try
        {
            List<string[]> objList = objServiceHandler.getNodeList(SERVICEPAGENAME);

            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].Contains("chart"))
                {
                    //Load spray window user control
                    Name = objList[i][1].ToString();
                    Node = objList[i][0].ToString();
                    break;
                }
            }
            ChangeLabelText();
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSerPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }

    private void loadData()
        {
            try
            {
                objPresenter = new ChartPresenter(this, Name);
                objPresenter.getChartData(null);
                //Chart1.ClientIDMode = ClientIDMode.Static;
                chartImg.Src = imageUrl;
                lblLocation.Text = objLocInfo.placeName;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSerPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_CHART_FAILURE) + " : " + ex.Message.ToString();
                return;
            }
        }
    void ChangeLabelText()
    {
        lblLocation.Text = getTranslatedText("Location");
    }
    string getTranslatedText(string text)
    {
        try
        {
            string strCul = objSvcInfo.Culture;
            text = objSerPre.getTranslatedText(text, strCul);
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSerPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_TRANS_ERROR) + " : " + ex.Message.ToString();

        }
        return text;
    }

        protected void Page_prerender(object sender, EventArgs e)
        { 
           objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];
           if (objLocInfo.DataPointInfo != null)
                 loadData();
            //Enabling or disabling feedback based on config setting
           //chartFeedback.Visible = (hasFeedback) ? true : false;
            // if (cultureCode != null)
            //     objPresenter.getTranslatedText(LABEL_RATING_TEXT, cultureCode);
            //lblRating.InnerText = transText;
             

            }
        #region IChart members
        public string title
        {
            get; 
            set;            
        }

        public string footerText
        {
            get;
            set;
        }

        public string cultureCode
        {
            get;
            set;
        }

        public string imageUrl
        {
            get;
            set;
             
        }

        public DataTable serieData
        {
            get;
            set;
        }

        public ChartFX.WebForms.Chart fChart
        {
            get { return Chart1; }
            set { Chart1 = value; }
        }
        public bool hasFeedback
        {
            get;
            set;
        }
        public string transText
        {
            get;
            set;
        }

        /* alignment issue for agriInfo chart - jerrey - Start */
        public string cssStyle
        {
            get;
            set;
        }
        /* alignment issue for agriInfo chart - jerrey - End */

   #endregion
   public string Node
    {
        get;
        set;
    }
       
  public string Name
    {
        get;
        set;
    }

  #region IChart Members

  public string bigImageUrl
  {
      get;
      set;
  }

  #endregion
}