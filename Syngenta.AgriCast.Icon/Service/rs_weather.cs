using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using Syngenta.AgriCast.Icon;

namespace Syngenta.AgriCast.Icon.RuleSet
{
    public class rs_weather
    {
          string COLTEMPERATURE = "TempAir_C";
          string COLCLOUD = "Cloudcover";
          string COLPRECIP = "precipamount";
          string COLPRECIPPROBABILITY = "PrecipitationProbability";
          string COLPRECIPTYPEMIN = "PrecipTypemin";
          string COLPRECIPTYPEMAX = "PrecipTypemax";
          string COLTEMPGROUND = "tempground";
          string COLTHUNDERSTROMPROBABILITY = "ThunderstormProbability_Pct";
          string COLVISIBILITY = "Visibility";
          string COLWIND = "WindSpeed";
          string COLWINDDIRECTION = "Winddirection";
          DataTable dtImages = new DataTable();
          DataRow drImg;
          string strImageSetName = "";

          public DataTable applyRuleSet(DataRow dr, string strImgSet)
         {
             strImageSetName = strImgSet;
            dtImages.Columns.Add("ImgName");
            dtImages.Columns.Add("TransId");
            dtImages.Columns.Add("ReplaceText");
            dtImages.Columns.Add("Value");
            
            ArrayList ar = new ArrayList();
            COLTEMPERATURE = getColumnName(dr.Table.Columns, "TempAir");
            COLCLOUD = getColumnName(dr.Table.Columns, "CloudCover");
            COLPRECIPPROBABILITY = getColumnName(dr.Table.Columns, "PrecipitationProbability");
            COLPRECIP = getColumnName(dr.Table.Columns, "PrecipAmount");
            COLPRECIPTYPEMAX = getColumnName(dr.Table.Columns, "Preciptypemax");
            COLPRECIPTYPEMIN = getColumnName(dr.Table.Columns, "Preciptypemin");
            COLTEMPGROUND = getColumnName(dr.Table.Columns, "tempground");
            COLTHUNDERSTROMPROBABILITY = getColumnName(dr.Table.Columns, "ThunderstormProbability");
            COLVISIBILITY = getColumnName(dr.Table.Columns, "Visibility");
            COLWIND = getColumnName(dr.Table.Columns, "Windspeed");
            COLWINDDIRECTION = getColumnName(dr.Table.Columns, "Winddirection");

            ar.Add(checkTemp(Convert.ToDouble(dr[COLTEMPERATURE])));
            ar.Add(checkCloud(Convert.ToDouble(dr[COLCLOUD])));
            ar.Add(checkPrecipitation(Convert.ToDouble(dr[COLPRECIP]), Convert.ToDouble(dr[COLPRECIPPROBABILITY]), Convert.ToDouble(dr[COLPRECIPTYPEMIN]), Convert.ToDouble(dr[COLPRECIPTYPEMAX])));
            ar.Add(checkFrost(Convert.ToDouble(dr[COLTEMPERATURE]), Convert.ToDouble(dr[COLTEMPGROUND]), Convert.ToDouble(dr[COLPRECIPPROBABILITY])));
            ar.Add(checkStorm(Convert.ToDouble(dr[COLTHUNDERSTROMPROBABILITY])));
            ar.Add(checkVisibility(Convert.ToDouble(dr[COLVISIBILITY])));
            checkWind(Convert.ToDouble(dr[COLWIND]));
            checkWindDirection(Convert.ToDouble(dr[COLWINDDIRECTION])); 
            return dtImages;
        }

        string getColumnName(DataColumnCollection cols, string phrase)
        {
            var colName = from c in cols.Cast<DataColumn>()
                          where c.ColumnName.ToLower().Contains(phrase.ToLower())
                          select c.ColumnName.ToString();
            return colName.ToList()[0].ToString();
        }

        //To check Temperature
         string checkTemp(double fTemp)
        {
            drImg = dtImages.NewRow();
            string strImg = null;
            //float fTemp = (float) dr["temp"];
            if (fTemp <= 8)
            {
                drImg["ImgName"] = null;
                drImg["TransId"] = "wt_temp0";
                drImg["ReplaceText"] = "temp";
                drImg["Value"] = Math.Round(fTemp);
                dtImages.Rows.Add(drImg);
                //return "wt_temp0";
                return strImg;
            }
            else if (fTemp > 8 && fTemp <= 17)
            {
                drImg["ImgName"] = null;
                drImg["TransId"] = "wt_temp1";
                drImg["ReplaceText"] = "temp";
                drImg["Value"] = Math.Round(fTemp);
                dtImages.Rows.Add(drImg);
                //return "wt_temp1";
                return strImg;
            }
            else if (fTemp > 17 && fTemp <= 25)
            {
                drImg["ImgName"] = null;
                drImg["TransId"] = "wt_temp2";
                drImg["ReplaceText"] = "temp";
                drImg["Value"] = Math.Round(fTemp);
                dtImages.Rows.Add(drImg);
                //return "wt_temp2";
                return strImg;
            }
            else
            {
                drImg["ImgName"] = null;
                drImg["TransId"] = "wt_temp3";
                drImg["ReplaceText"] = "temp";
                drImg["Value"] = Math.Round(fTemp);
                dtImages.Rows.Add(drImg);
                //return "wt_temp3";
                return strImg;
            }
            

        }

        //To check clouds
         string checkCloud(double fCloud)
        {
            drImg = dtImages.NewRow();
            string strCloudImg = null;
            if (fCloud <= 15)
            {
                drImg["ImgName"] = null;
                drImg["TransId"] = "wt_cloud0";
                drImg["ReplaceText"] = "cloud";
                drImg["Value"] = Math.Round(fCloud);
                dtImages.Rows.Add(drImg);
                //return "wt_cloud0";
                return strCloudImg;
            }
            else if (fCloud > 15 && fCloud <= 40)
            {
                drImg["ImgName"] = strImageSetName + "c1";
                drImg["TransId"] = "wt_cloud1";
                drImg["ReplaceText"] = "cloud";
                drImg["Value"] = Math.Round(fCloud);
                dtImages.Rows.Add(drImg);
                //return "c1";
                strCloudImg = "c1";
                return strCloudImg;
            }
            else if (fCloud > 40 && fCloud <= 70)
            {
                drImg["ImgName"] = strImageSetName +"c2";
                drImg["TransId"] = "wt_cloud2";
                drImg["ReplaceText"] = "cloud";
                drImg["Value"] = Math.Round(fCloud);
                dtImages.Rows.Add(drImg);
                //return "c2";
                strCloudImg = "c2";
                return strCloudImg;
            }
            else if (fCloud > 70 && fCloud <= 90)
            {
                drImg["ImgName"] = strImageSetName + "c3";
                drImg["TransId"] = "wt_cloud2";
                drImg["ReplaceText"] = "cloud";
                drImg["Value"] = Math.Round(fCloud);
                dtImages.Rows.Add(drImg);
                //return "c3";
                strCloudImg = "c3";
                return strCloudImg;
            }
            else if (fCloud > 90)
            {
                drImg["ImgName"] = strImageSetName + "c4";
                drImg["TransId"] = "wt_cloud3";
                drImg["ReplaceText"] = "cloud";
                drImg["Value"] = Math.Round(fCloud);
                dtImages.Rows.Add(drImg);
                //return "c4";
                strCloudImg = "c4";
                return strCloudImg;
            }
            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "cloud";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                return strCloudImg;
            }
        }

        //To check precipitation
         string checkPrecipitation(double fPecipAvg, double fProbPrecip, double fPrecipTypMin, double fPrecipTypMax)
        {
            drImg = dtImages.NewRow();
            string strPrecipImg = null;

            if (fPecipAvg > 0.02 && fProbPrecip > 20)
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "wt_and";
                drImg["ReplaceText"] = "and_precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
            }
            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "and_precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
            }
             
            drImg = dtImages.NewRow();
            if (fPecipAvg > 0.02 && (fProbPrecip > 20 && fProbPrecip < 65))
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "wt_chanceof";
                drImg["ReplaceText"] = "precipprob";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
            }
            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "precipprob";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
            }
            drImg = dtImages.NewRow();
            if ((fPecipAvg > 0.02 && fPecipAvg <= 0.2) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax <= 2.5))
            {
                drImg["ImgName"] = strImageSetName + "pr1";
                drImg["TransId"] = "wt_precip1";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "pr1";
                return strPrecipImg;
            }
            else if ((fPecipAvg > 0.2 && fPecipAvg <= 0.4) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax <= 2.5))
            {
                drImg["ImgName"] = strImageSetName + "pr2";
                drImg["TransId"] = "wt_precip1";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "pr2";
                return strPrecipImg;
            }
            else if ((fPecipAvg > 0.4 && fPecipAvg <= 0.8) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax <= 2.5))
            {
                drImg["ImgName"] = strImageSetName + "pr3";
                drImg["TransId"] = "wt_precip2";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "pr3";
                return strPrecipImg;
            }
            else if ((fPecipAvg > 0.8 && fPecipAvg <= 1.6) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax <= 2.5))
            {
                drImg["ImgName"] = strImageSetName + "pr4";
                drImg["TransId"] = "wt_precip2";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "pr4";
                return strPrecipImg;
            }
            else if ((fPecipAvg >= 1.6) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax <= 2.5))
            {
                drImg["ImgName"] = strImageSetName + "pr5";
                drImg["TransId"] = "wt_precip3";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                strPrecipImg = "pr5";
                dtImages.Rows.Add(drImg);
                return strPrecipImg;
            }
            else if ((fPecipAvg >= 0.02 && fPecipAvg <= 0.6) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "prs1";
                drImg["TransId"] = "wt_snowrain1";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "prs1";
                return strPrecipImg;
            }

            else if ((fPecipAvg > 0.6) && (fProbPrecip >= 20) && (fPrecipTypMin <= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "prs2";
                drImg["TransId"] = "wt_snowrain2";
                drImg["ReplaceText"] = "precip";
                dtImages.Rows.Add(drImg);
                strPrecipImg = "prs2";
                return strPrecipImg;
            }

            else if ((fPecipAvg >= 0.02 && fPecipAvg <= 0.2) && (fProbPrecip >= 20) && (fPrecipTypMin >= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "ps1";
                drImg["TransId"] = "wt_snow1";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "ps1";
                return strPrecipImg;
            }

            else if ((fPecipAvg > 0.2 && fPecipAvg <= 0.4) && (fProbPrecip >= 20) && (fPrecipTypMin >= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "ps2";
                drImg["TransId"] = "wt_snow1";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "ps2";
                return strPrecipImg;
            }

            else if ((fPecipAvg > 0.4 && fPecipAvg <= 0.8) && (fProbPrecip >= 20) && (fPrecipTypMin >= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "ps3";
                drImg["TransId"] = "wt_snow2";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "ps3";
                return strPrecipImg;
            }

            else if ((fPecipAvg > 0.8 && fPecipAvg <= 1.6) && (fProbPrecip >= 20) && (fPrecipTypMin >= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "ps4";
                drImg["TransId"] = "wt_snow2";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "ps4";
                return strPrecipImg;
            }

            else if ((fPecipAvg > 1.6) && (fProbPrecip >= 20) && (fPrecipTypMin >= 2) && (fPrecipTypMax >= 2.9))
            {
                drImg["ImgName"] = strImageSetName + "ps5";
                drImg["TransId"] = "wt_snow3";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = Math.Round(fPecipAvg);
                dtImages.Rows.Add(drImg);
                strPrecipImg = "ps5";
                return strPrecipImg;
            }

            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "precip";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                return strPrecipImg;
            }

        }

        //To check Frost
         string checkFrost(double fTemp, double fTemp_Ground, double fProbPrecip)
        {
            drImg = dtImages.NewRow();
            string stFrostImg = null;

            if ((fTemp <= 0) && (fTemp_Ground >= 0) && (fProbPrecip <= 20))
            {
                drImg["ImgName"] = null;
                drImg["TransId"] = "wt_frost1";
                drImg["ReplaceText"] = "frost";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                return stFrostImg;
            }
            else if ((fTemp <= 0) && (fTemp_Ground <= 0) && (fProbPrecip <= 20))
            {
                drImg["ImgName"] = strImageSetName + "ps0";
                drImg["TransId"] = "wt_frost1";
                drImg["ReplaceText"] = "frost";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                stFrostImg = "ps0";
                return stFrostImg;
            }
            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "frost";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                return stFrostImg;
            }
        }

         //To check chance of Frost
         string checkchanceOfFrost(double fTemp, double fTemp_Ground, double fProbPrecip)
         {
             drImg = dtImages.NewRow();
             string stFrostchanceImg = null;

             if ((fTemp <= 0) && (fTemp_Ground >= 0) && (fProbPrecip <= 20))
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_chanceof";
                 drImg["ReplaceText"] = "frostprob";
                 drImg["Value"] = "";
                 dtImages.Rows.Add(drImg);
             }
             else 
             {
                 drImg["ImgName"] = "";
                 drImg["TransId"] = "";
                 drImg["ReplaceText"] = "frostprob";
                 drImg["Value"] = "";
                 dtImages.Rows.Add(drImg);
             }
             return stFrostchanceImg;
         }

        //To check Storm
         string checkStorm(double fProbThunder)
        {
            drImg = dtImages.NewRow();
            string strStormImg = null;

            if (fProbThunder >= 50 && fProbThunder <80)
            {
                drImg["ImgName"] = strImageSetName + "s1";
                drImg["TransId"] = "wt_storm1";
                drImg["ReplaceText"] = "storm";
                drImg["Value"] = Math.Round(fProbThunder);
                dtImages.Rows.Add(drImg);
                strStormImg = "s1";
                return strStormImg;
            }
            else if (fProbThunder >= 80)
            {
                drImg["ImgName"] = strImageSetName + "s2";
                drImg["TransId"] = "wt_storm2";
                drImg["ReplaceText"] = "storm";
                drImg["Value"] = Math.Round(fProbThunder);
                dtImages.Rows.Add(drImg);
                strStormImg = "s2";
                return strStormImg;
            }
            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "storm";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                return strStormImg;
            }
        }

        // To check Visibility
         string checkVisibility(double fVisibility)
        {
            drImg = dtImages.NewRow();
            string strVisibilityImg = null;
             //In actual application the limits are 20,40 and 80, here we are dividing by 100 as we get the data in KM already.
            if (fVisibility >= 0.2 && fVisibility <= 0.4)
            {
                drImg["ImgName"] = strImageSetName + "f1";
                drImg["TransId"] = null;
                drImg["ReplaceText"] = "visibility"; 
                drImg["Value"] = Math.Round(fVisibility);
                dtImages.Rows.Add(drImg);
                strVisibilityImg = "f1";
                return strVisibilityImg;
            }
            else if (fVisibility <= 0.2)
            {
                drImg["ImgName"] = strImageSetName + "f2";
                drImg["TransId"] = null; 
                drImg["ReplaceText"] = "visibility";
                drImg["Value"] = Math.Round(fVisibility);
                dtImages.Rows.Add(drImg);
                strVisibilityImg = "f2";
                return strVisibilityImg;
            }
            else
            {
                drImg["ImgName"] = "";
                drImg["TransId"] = "";
                drImg["ReplaceText"] = "visibility";
                drImg["Value"] = "";
                dtImages.Rows.Add(drImg);
                return strVisibilityImg;
            }
        }

        //To check Wind
         string checkWind(double fWind)
         {
             drImg = dtImages.NewRow();
             string strWindImg = null;

             if (fWind <= 1.5)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wind0";
                 drImg["ReplaceText"] = "wind";
                 drImg["Value"] = Math.Round(fWind);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else if (fWind >= 1.5 && fWind < 3)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wind1";
                 drImg["ReplaceText"] = "wind";
                 drImg["Value"] = Math.Round(fWind);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else if (fWind >= 3 && fWind < 6)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wind2";
                 drImg["ReplaceText"] = "wind";
                 drImg["Value"] = Math.Round(fWind);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else if (fWind >= 6 && fWind < 9)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wind3";
                 drImg["ReplaceText"] = "wind";
                 drImg["Value"] = Math.Round(fWind);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else if (fWind >= 9)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wind4";
                 drImg["ReplaceText"] = "wind";
                 drImg["Value"] = Math.Round(fWind);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else
             {
                 drImg["ImgName"] = "";
                 drImg["TransId"] = "";
                 drImg["ReplaceText"] = "wind";
                 drImg["Value"] = "";
                 dtImages.Rows.Add(drImg);
             }
             return strWindImg;
         }

        //To check Wind Direction
         string checkWindDirection(double iWindDir)
         {
             drImg = dtImages.NewRow();
             string strWindImg = null;

             if (iWindDir >= 60)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = null;
                 drImg["ReplaceText"] = "winddir";
                 drImg["Value"] = Math.Round(iWindDir);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else if (iWindDir >= 30 && iWindDir < 60)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wdirpred";
                 drImg["ReplaceText"] = "winddir";
                 drImg["Value"] = Math.Round(iWindDir);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else if (iWindDir <= 30)
             {
                 drImg["ImgName"] = null;
                 drImg["TransId"] = "wt_wdir";
                 drImg["ReplaceText"] = "winddir";
                 drImg["Value"] = Math.Round(iWindDir);
                 dtImages.Rows.Add(drImg);
                 return strWindImg;
             }
             else
             {
                 drImg["ImgName"] = "";
                 drImg["TransId"] = "";
                 drImg["ReplaceText"] = "winddir";
                 drImg["Value"] = "0";
                 dtImages.Rows.Add(drImg);
             }
             return strWindImg;
             
         }
    }
}