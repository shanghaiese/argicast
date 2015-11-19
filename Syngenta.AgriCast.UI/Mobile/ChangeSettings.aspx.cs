using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using System.Data;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;

public partial class Mobile_ChangeSettings : System.Web.UI.Page
{
    ServiceInfo svcInfo = new ServiceInfo();
    LocationInfo objLocInfo = new LocationInfo();
    ServiceHandler objSh = new ServiceHandler();
    ServicePresenter objSerPre = new ServicePresenter();
    CommonUtil objCommonUtil = new CommonUtil();
    ServicePresenter objSvcPre = new ServicePresenter();
    #region events
    protected void Page_Load(object sender, EventArgs e)
    {
        //svcInfo = ServiceInfo.ServiceConfig;

        if (Session["serviceInfo"] != null)
        {
            svcInfo = (ServiceInfo)Session["serviceInfo"];
        }
        else
        {
            svcInfo = ServiceInfo.ServiceConfig;
        }
        try
        {
            ChangeLabelText();
            if (!IsPostBack)
            {
                //ddlUnits.SelectedValue = svcInfo.Unit;
                //ddlWind.SelectedValue = svcInfo.WUnit;


                //populate unit dropdown
                DataTable dtUnit = objSh.loadUnits();
                //Get the Translated text and add it as a new Column .
                //assign this new column as TextField
                DataColumn dcTransText = new DataColumn("transtext");
                dtUnit.Columns.Add(dcTransText);
                //get transtext for the Units column
                foreach (DataRow dr in dtUnit.Rows)
                {
                    dr["transtext"] = getTranslatedText(dr["units"].ToString());
                }
                ddlUnits.DataSource = dtUnit;
                ddlUnits.DataValueField = "units";
                ddlUnits.DataTextField = "transtext";
                ddlUnits.DataBind();


                //populate culture dropdown
                ddlCulture.DataSource = objSh.loadCultureCode();
                ddlCulture.DataTextField = "Value";
                ddlCulture.DataValueField = "code";
                ddlCulture.DataBind();


                //populate wind unit dropdown
                string strCustomUnits = objSh.LoadCustomSettings();
                string[] settings = strCustomUnits.Split('#');

                for (int i = 0; i < settings.Count(); i++)
                {
                    string Parameter = settings[i].Remove(settings[i].IndexOf("-"));
                    string[] ParameterSettings = settings[i].Substring(settings[i].IndexOf("-") + 1).Split(',');

                    switch (Parameter.ToLower())
                    {

                        case "wind": ddlWind.Items.Clear();
                            foreach (string param in ParameterSettings)
                            {

                                ddlWind.Items.Add(new ListItem(getTranslatedText(param), param));
                            }
                            ddlWind.DataBind();

                            break;
                        default:
                            break;
                    }
                }

                if (svcInfo.Unit != "")
                    ddlUnits.SelectedValue = svcInfo.Unit;
                if (svcInfo.WUnit != "")
                    ddlWind.SelectedValue = svcInfo.WUnit;
                if (svcInfo.Culture != "")
                    ddlCulture.SelectedValue = svcInfo.Culture;
            }
            else
            {

                svcInfo.Unit = ddlUnits.SelectedValue.ToString();
                svcInfo.Culture = ddlCulture.SelectedValue.ToString();
                svcInfo.WUnit = ddlWind.SelectedValue.ToString();
                //HttpContext.Current.Session["serviceInfo"] = svcInfo;
                CreateCookies();

            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }


    //protected void ddlCulture_SelectedIndexChanged(object sender, EventArgs e)
    //{
       
       
    //}




    //protected void ddlUnits_SelectedIndexChanged(object sender, EventArgs e)
    //{
        
    //}
    //protected void ddlWind_SelectedIndexChanged(object sender, EventArgs e)
    //{
       
    //}

    #endregion

    #region methods

    private void CreateCookies()
    {
        HttpCookie LocationCookie = new HttpCookie("ais_LocationInfo", objLocInfo.SerializeCookieString());
        LocationCookie.Expires = DateTime.Now.AddDays(365);
        Response.Cookies.Add(LocationCookie);
    }


    string getTranslatedText(string text)
    {
        if (Session["serviceInfo"] != null)
        {
            svcInfo = (ServiceInfo)Session["serviceInfo"];
        }
        else
        {
            svcInfo = ServiceInfo.ServiceConfig;
        }
        try
        {
            string strCul = svcInfo.Culture;
            text = objSerPre.getTranslatedText(text, strCul);
           
        
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_TRANS_ERROR) + " : " + ex.Message.ToString();
        
        }
        return text;
    }

    void ChangeLabelText()
    {
        lblCulture.InnerText = getTranslatedText("Language");
        lblUnit.InnerText = getTranslatedText("Unit");
        lblWind.InnerText = getTranslatedText("Wind");
        btnSave.Text = getTranslatedText("Save");
    }

    #endregion
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            svcInfo.Culture = ddlCulture.SelectedValue;
            Session["serviceInfo"] = svcInfo;
            svcInfo.Unit = ddlUnits.SelectedValue.ToString();
            svcInfo.WUnit = ddlWind.SelectedValue.ToString();
            Response.Redirect("Default.aspx");
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_SAVE_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }
}