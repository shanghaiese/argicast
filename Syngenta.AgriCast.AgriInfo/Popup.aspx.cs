using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.AgriInfo.Presenter;
using Syngenta.AgriCast.AgriInfo.DTO;
using Syngenta.AgriCast.AgriInfo.View;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common;
namespace Syngenta.AgriCast.AgriInfo
{
    public partial class Popup : System.Web.UI.Page,IPopup
    {
        PopupPresenter objPopPresenter;
        AdvancedOptionInfo objAdvancedOptionInfo;  
        SeriesInfo objSeriesInfo;
        ServicePresenter objSvcPre = new ServicePresenter();
        ServiceInfo objSvcInfo;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
              //  btnsave.Attributes.Add("onclick", "javascript:closePopup();");
              
                //ScriptManager.RegisterStartupScript(this, typeof(string),"focus", "self.focus();", true);
                if (HttpContext.Current.Request.QueryString.Count != 0)
                {
                    setControlText();
                    objAdvancedOptionInfo = new AdvancedOptionInfo();
                    objSeriesInfo = SeriesInfo.getSeriesInfoObject;

                    objAdvancedOptionInfo.Series = HttpContext.Current.Request.QueryString["series"];
                    objAdvancedOptionInfo.Serie = HttpContext.Current.Request.QueryString["serie"];
                    if (!IsPostBack)
                    {
                        objPopPresenter = new PopupPresenter(this);
                        objPopPresenter.GetAdvancedOptions(objAdvancedOptionInfo.Series, objAdvancedOptionInfo.Serie);

                        if (al[0][1] != "")
                        {
                            string[] AggFunctions = al[0][1].Split(',');
                            foreach (string Aggregation in AggFunctions)
                            {
                                if (Aggregation.Trim() != "")
                                    ddlAggregationFunction.Items.Add(newItem(Aggregation, Aggregation));
                            }
                            ddlAggregationFunction.DataBind();
                        }
                        else
                        {
                            lblAggregationFunction.Attributes.Add("class","hide");
                            ddlAggregationFunction.CssClass = "hide";
                        }
                        if (al[1][1].ToString() != "")
                        {
                            if (al[1][1].Trim().ToLower() == "true")
                            {
                                cbAccumulate.Checked = true;
                            }
                        }
                        else
                        {
                            lblAccumulate.Attributes.Add("class", "hide");
                            cbAccumulate.CssClass = "hide";
                        }

                        if (al[2][1].ToString() != "0" && (al[2][1].ToString() != ""))
                        {
                            int Year = Int32.Parse(al[2][1]);
                            int currYear = DateTime.Now.Year;
                            ddlYear.Items.Add("");
                            for (int i = currYear; i >= Year; i--)
                            {
                                ddlYear.Items.Add(i.ToString());
                            }
                            ddlYear.DataBind();
                        }
                        else
                        {
                            lblYearCompare.Attributes.Add("class", "hide");
                            ddlYear.CssClass = "hide";
                        }

                        if (al[3][1] != "")
                        {
                            string[] Trends = al[3][1].Split(',');
                            ddlTrends.Items.Add("");
                            foreach (string trend in Trends)
                            {
                                ddlTrends.Items.Add(newItem(trend, trend));
                            }
                            ddlTrends.DataBind();
                        }
                        else
                        {
                            ddlTrends.CssClass = "hide";
                            lblTrends.Attributes.Add("class", "hide");
                            ddlTrends.Visible = false;
                            lblTrends.Visible = false;
                        }

                        if (al[4][1] != "")
                        {
                            if (al[4][1].Trim().ToLower() != "true")
                            {
                                cbAltitude.Enabled = false;
                            }
                        }
                        else
                        {
                            lblAltitude.Attributes.Add("class", "hide");
                            cbAltitude.CssClass = "hide";
                        }

                        if (al.Count == 8)
                        {

                            if (al[5][1] != "")
                            {
                                string[] Methods = al[5][1].Split(',');
                                ddlMethod.Items.Add("");
                                foreach (string Method in Methods)
                                {
                                    ddlMethod.Items.Add(newItem(Method, Method));
                                }
                                ddlMethod.DataBind();
                            }
                            else
                            {
                                ddlMethod.CssClass = "hide";
                                lblMethod.Attributes.Add("class", "hide");
                                ddlMethod.Visible = false;
                                lblMethod.Visible = false;
                            }

                            if (al[6][1] != "")
                            {
                                txtCap.Text = al[6][1].ToString();
                            }
                            else
                            {
                                txtCap.CssClass = "hide";
                                lblCap.Attributes.Add("class", "hide");
                                txtCap.Visible = false;
                                lblCap.Visible = false;
                            }

                            if (al[7][1] != "")
                            {
                                txtBase.Text = al[7][1].ToString();
                            }
                            else
                            {
                                txtBase.CssClass = "hide";
                                lblBase.Attributes.Add("class", "hide");
                                txtBase.Visible = false;
                                lblBase.Visible = false;
                            }
                        }
                        else
                        {
                            ddlMethod.CssClass = "hide";
                            lblMethod.Attributes.Add("class", "hide");
                            ddlMethod.Visible = false;
                            lblMethod.Visible = false;
                            txtCap.CssClass = "hide";
                            lblCap.Attributes.Add("class", "hide");
                            txtCap.Visible = false;
                            lblCap.Visible = false;
                            txtBase.CssClass = "hide";
                            lblBase.Attributes.Add("class", "hide");
                            txtBase.Visible = false;
                            lblBase.Visible = false;
                        }

                        if (objSeriesInfo.SeriesList != null)
                        {
                            foreach (AdvancedOptionInfo obj in objSeriesInfo.SeriesList)
                            {
                                if ((obj.Series == objAdvancedOptionInfo.Series) && (obj.Serie == objAdvancedOptionInfo.Serie))
                                {
                                    if (obj.Aggregation.ToString() != "")
                                    ddlAggregationFunction.SelectedValue = obj.Aggregation.ToString();
                                    if(obj.Accumulate.ToString()!="")
                                    cbAccumulate.Checked = bool.Parse(obj.Accumulate.ToString());
                                    ddlYear.SelectedValue = obj.Year.ToString();
                                    if(obj.Trend.ToString()!="")
                                    ddlTrends.SelectedValue = obj.Trend.ToString();
                                    if (obj.Altitude.ToString() != "")
                                    cbAltitude.Checked = bool.Parse(obj.Altitude.ToString());
                                    if (obj.Method.ToString() != "")
                                        ddlMethod.SelectedValue = obj.Method.ToString();
                                    if (obj.Cap.ToString() != "")
                                        txtCap.Text = obj.Cap.ToString();
                                    if (obj.Base.ToString() != "")
                                        txtBase.Text = obj.Base.ToString();
                                    


                                }

                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The page could not be loaded due to the following error: " + ex.Message.ToString();
            }
        }
        public void setControlText()
        {
            try
            {
                lblAggregationFunction.InnerText = TranslatedText(Constants.AGGREGATION);
                lblAccumulate.InnerText = TranslatedText(Constants.ACCUMULATE);
                lblYearCompare.InnerText = TranslatedText(Constants.SELECTED_YEAR);
                lblTrends.InnerText = TranslatedText(Constants.TRENDS);
                lblAltitude.InnerText = TranslatedText(Constants.ALTITUDE_ADJUSTMENTS);
                lblMethod.InnerText = TranslatedText(Constants.METHOD);
                lblCap.InnerText = TranslatedText(Constants.GDDCAP);
                lblBase.InnerText = TranslatedText(Constants.GDDBASE);
                btnsave.Text = TranslatedText(Constants.SAVE);
                //lblMasl.InnerText = TranslatedText("meteraslabbr");
                
            }

            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = "The text could not be translated due the following error : " + ex.Message.ToString();
            }

        }
       public  ListItem newItem(string Value, string Text)
        {
            ListItem item = new ListItem();
            item.Text = TranslatedText(Text);
            item.Value = Value;
            return item;
        }
       private string TranslatedText(string label)
       {
           if (objSvcInfo == null)
               objSvcInfo = (ServiceInfo)Session["serviceInfo"];
           objPopPresenter = new PopupPresenter(this);
           objPopPresenter.getTranslatedText(label, objSvcInfo.Culture);
           return strTransText;
       }
        protected void btnsave_Click(object sender, EventArgs e)
        {
            int i;
            try
            {
                objAdvancedOptionInfo.Aggregation = (ddlAggregationFunction.SelectedValue != null) ? ddlAggregationFunction.SelectedValue.ToString() : "";
                objAdvancedOptionInfo.Accumulate = (cbAccumulate.CssClass != "hide")?cbAccumulate.Checked.ToString():"";
                int SaveYear = (ddlYear.SelectedItem != null) ? int.TryParse(ddlYear.SelectedValue, out i) ? i : 0 : 0;
                if (SaveYear != 0)
                    objAdvancedOptionInfo.Year = SaveYear;

                objAdvancedOptionInfo.Trend = (ddlTrends.SelectedValue != null) ? ddlTrends.SelectedValue.ToString() : "";
                objAdvancedOptionInfo.Altitude = (cbAltitude.CssClass != "hide") ? cbAltitude.Checked.ToString() : "";
                objAdvancedOptionInfo.Method = (ddlMethod.SelectedValue != null) ? ddlMethod.SelectedValue.ToString() : "";
                objAdvancedOptionInfo.Cap = (txtCap.Text != null) ? txtCap.Text.ToString() : "";
                objAdvancedOptionInfo.Base = (txtBase.Text != null) ? txtBase.Text.ToString() : "";

                if (!CheckSeriesExist())
                {
                    objSeriesInfo.SeriesList.Add(objAdvancedOptionInfo);
                }

              //  ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "closePopup();", true);
                ClientScript.RegisterClientScriptBlock( typeof(string), "CLOSE_WINDOW", "javascript:closePopup();", true);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The advanced options could not be saved due to the following error: " + ex.Message.ToString();
            }
           

            
        }
        public bool CheckSeriesExist()
        {
            if (objSeriesInfo.SeriesList != null)
            {
                foreach (AdvancedOptionInfo obj in objSeriesInfo.SeriesList)
                {
                    if ((obj.Series == objAdvancedOptionInfo.Series) && (obj.Serie == objAdvancedOptionInfo.Serie))
                    {
                        obj.Aggregation = objAdvancedOptionInfo.Aggregation;
                        obj.Accumulate = objAdvancedOptionInfo.Accumulate;
                        obj.Year = objAdvancedOptionInfo.Year;
                        obj.Trend = objAdvancedOptionInfo.Trend;
                        obj.Altitude = objAdvancedOptionInfo.Altitude;
                        return true;
                    }

                }
            }
            else
            {
                objSeriesInfo.SeriesList = new List<AdvancedOptionInfo>();
            }
            return false;
        }

        public List<string[]> al
        {
            get;
            set;
        }




        public string strTransText
        {
            get;
            set;
        }
    }
}