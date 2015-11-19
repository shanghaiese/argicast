using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
 
using System.Drawing;
using System.Collections;

namespace Syngenta.AgriCast.Common.Service
{
    public class PalletteConstants
    {
        #region Declaration
        DataTable dt_pal_bruchid;
        DataTable dt_pal_earlyblight;
        DataTable dt_pal_temperature_me;
        DataTable dt_pal_rain_12hour_me;
        DataTable dt_pal_rain_risk_me;
        DataTable dt_pal_rainDuration_12hour_me;
        DataTable dt_pal_humid_me;
        DataTable dt_pal_window_me;
        DataTable dt_pal_runoff;
        DataTable dt_pal_dailydswfr;
        DataTable dt_pal_sip_temperature;
        DataTable dt_pal_sip_rain;
        DataTable dt_pal_sip_rain_8hour;
        DataTable dt_pal_sip_rain_duration;
        DataTable dt_defaultPallette;
        DataTable dt_pal_sip_rain_risk;
        DataTable dt_pal_sip_humid;
        DataTable dt_pal_sip_precip_type;
        DataTable dt_pal_sip_window;
        DataTable dt_pal_relsunshine;
        DataTable dt_pal_ski;
        DataTable dt_pal_leafwetness;
        DataTable dt_pal_leafwetness_avg;
        DataTable dt_pal_frost;
        DataTable dt_pal_evaporation;
        DataTable dt_pal_winspeedbft;
        DataTable dt_pal_winspeedms;
        DataTable dt_pal_winspeedkmh;
        DataTable dt_pal_winspeedmph;
        DataTable dt_pal_winspeedkts;
        DataTable dt_pal_radiation;
        DataTable dt_pal_combinedrisk;
        DataTable dt_pal_humid;
        DataTable dt_pal_spraywin;
        DataTable dt_pal_spraywinfrcp03;
        DataTable dt_pal_spraywin_greyscale;
        DataTable dt_pal_plant;
        DataTable dt_pal_harvest;
        DataTable dt_pal_deltat;
        DataTable dt_pal_deltat_rs;
        DataTable dt_pal_redorangegreen;
        DataTable dt_pal_sip_temperaturef;
        DataTable dt_pal_rain;
        DataTable dt_pal_sip_raininch;
        DataTable dt_pal_sip_raininch_8hour;
        DataTable dt_pal_rain_risk;
        DataTable dt_pal_precip_type;
        DataTable dt_pal_clouds;
        DataTable dt_pal_temperature_f;
        DataTable dt_pal_dailydswfrBW;
        DataTable dt_pal_diseaserisk;
        DataTable dt_pal_severeweather;
        DataTable dt_pal_temperature;
       
        #endregion

        //public string getColorRuleset(string value, string Pallette)
        //{
        //    string DisplayColor = "#ffffff";
        //    switch (Pallette)
        //    {
        //        case "pal_spraywin":
        //            switch (value)
        //            {
        //                case "0":
        //                    DisplayColor = "#808080";
        //                    break;
        //                case "1":
        //                    DisplayColor = "#FF0000";
        //                    break;
        //                case "10":
        //                    DisplayColor = "#D3D3D3";
        //                    break;
        //                case "11":
        //                    DisplayColor = "#90EE90";
        //                    break;
        //                case "20":
        //                    DisplayColor = "#D3D3D3";
        //                    break;
        //                case "21":
        //                    DisplayColor = "#008000";
        //                    break;
        //                default:
        //                    DisplayColor = "#ffffff";
        //                    break;

        //            }
        //            break;
        //        default:
        //            DisplayColor = "#ffffff";
        //            break;
        //    }


        //    return DisplayColor;
        //}

        public DataTable getPallette(string palletteName)
        {
            DataTable dtPallette = new DataTable();
            switch (palletteName.ToLower())
            {
                case "pal_dailydswfr":
                    dtPallette = pal_dailydswfr;
                    break;
                case "pal_runoff":
                    dtPallette = pal_runoff;
                    break;
                case "pal_sip_temperature":
                    dtPallette = pal_sip_temperature;
                    break;
                case "pal_sip_rain":
                    dtPallette = pal_sip_rain;
                    break;
                case "pal_sip_rain_8hour":
                    dtPallette = pal_sip_rain_8hour;
                    break;
                case "pal_sip_rain_duration":
                    dtPallette = pal_sip_rain_duration;
                    break;
                case "pal_sip_precip_type":
                    dtPallette = pal_sip_precip_type;
                    break;
                case "pal_sip_window":
                    dtPallette = pal_sip_window;
                    break;
                case "pal_sip_humid":
                    dtPallette = pal_sip_humid;
                    break;
                case "pal_sip_rain_risk":
                    dtPallette = pal_sip_rain_risk;
                    break;
                case "pal_evaporation":
                    dtPallette = pal_evaporation;
                    break;
                case "pal_frost":
                    dtPallette = pal_frost;
                    break;
                case "pal_leafwetness_avg":
                    dtPallette = pal_leafwetness_avg;
                    break;
                case "pal_leafwetness":
                    dtPallette = pal_leafwetness;
                    break;
                case "pal_ski":
                    dtPallette = pal_ski;
                    break;
                case "pal_relsunshine":
                    dtPallette = pal_relsunshine;
                    break;
                case "pal_radiation":
                    dtPallette = pal_radiation;
                    break;
                case "pal_winspeedkts":
                    dtPallette = pal_winspeedkts;
                    break;
                case "pal_winspeedmph":
                    dtPallette = pal_winspeedmph;
                    break;
                case "pal_winspeedkmh":
                    dtPallette = pal_winspeedkmh;
                    break;
                case "pal_winspeedms":
                    dtPallette = pal_winspeedms;
                    break;
                case "pal_winspeedbft":
                    dtPallette = pal_winspeedbft;
                    break;
                case "pal_redorangegreen":
                    dtPallette = pal_redorangegreen;
                    break;
                case "pal_deltat_rs":
                    dtPallette = pal_deltat_rs;
                    break;
                case "pal_deltat":
                    dtPallette = pal_winspeedbft;
                    break;
                case "pal_spraywin":
                    dtPallette = pal_spraywin;
                    break;
                case "pal_harvest":
                    dtPallette = pal_harvest;
                    break;
                case "pal_plant":
                    dtPallette = pal_plant;
                    break;
                case "pal_spraywin_greyscale":
                    dtPallette = pal_spraywin_greyscale;
                    break;
                case "pal_spraywinfrcp03":
                    dtPallette = pal_spraywinfrcp03;
                    break;
                case "pal_humid":
                    dtPallette = pal_humid;
                    break;
                case "pal_combinedrisk":
                    dtPallette = pal_combinedrisk;
                    break;
                case "pal_clouds":
                    dtPallette = pal_clouds;
                    break;
                case "pal_precip_type":
                    dtPallette = pal_precip_type;
                    break;
                case "pal_rain_risk":
                    dtPallette = pal_rain_risk;
                    break;
                case "pal_sip_raininch_8hour":
                    dtPallette = pal_sip_raininch_8hour;
                    break;
                case "pal_sip_raininch":
                    dtPallette = pal_sip_raininch;
                    break;
                case "pal_rain":
                    dtPallette = pal_rain;
                    break;
                case "pal_sip_temperaturef":
                    dtPallette = pal_sip_temperaturef;
                    break;
                case "pal_temperature":
                    dtPallette = pal_temperature;
                    break;
                case "pal_severeweather":
                    dtPallette = pal_severeweather;
                    break;
                case "pal_diseaserisk":
                    dtPallette = pal_diseaserisk;
                    break;
                case "pal_dailydswfrBW":
                    dtPallette = pal_dailydswfrBW;
                    break;
                case "pal_temperature_f":
                    dtPallette = pal_temperature_f;
                    break;
                case "pal_bruchid":
                    dtPallette = pal_bruchid;
                    break;
                case "pal_earlyblight":
                    dtPallette = pal_earlyblight;
                    break;
                case "pal_temperature_me":
                    dtPallette = pal_temperature_me;
                    break;
                case "pal_rain_12hour_me":
                    dtPallette = pal_rain_12hour_me;
                    break;
                case "pal_rain_risk_me":
                    dtPallette = pal_rain_risk_me;
                    break;
                case "pal_rainDuration_12hour_me":
                    dtPallette = pal_rainDuration_12hour_me;
                    break;
                case "pal_humid_me":
                    dtPallette = pal_humid_me;
                    break;
                case "pal_window_me":
                    dtPallette = pal_window_me;
                    break;
                default:
                    dtPallette = defaultPallette;
                    break;
            }
            return dtPallette;
        }

        private DataTable defaultPallette
        {
            get
            {
                dt_defaultPallette = new DataTable();
                dt_defaultPallette.Columns.Add("value");
                dt_defaultPallette.Columns.Add("color");
                dt_defaultPallette.Rows.Add("21", "#FFFFFF");
                dt_defaultPallette.Rows.Add("11", "#FFFFFF");
                dt_defaultPallette.Rows.Add("0", "#FFFFFF");
                dt_defaultPallette.Rows.Add("1", "#FFFFFF");
                dt_defaultPallette.Rows.Add("2", "#FFFFFF");
                dt_defaultPallette.Rows.Add("-5", "#FFFFFF");
                dt_defaultPallette.Rows.Add("-4", "#FFFFFF");
                dt_defaultPallette.Rows.Add("-2", "#FFFFFF");
                dt_defaultPallette.Rows.Add("15", "#FFFFFF");
                dt_defaultPallette.Rows.Add("17", "#FFFFFF");
                dt_defaultPallette.Rows.Add("10", "#FFFFFF");
                dt_defaultPallette.Rows.Add("19", "#FFFFFF");
                dt_defaultPallette.Rows.Add("12", "#FFFFFF");
                dt_defaultPallette.Rows.Add("16", "#FFFFFF");
                dt_defaultPallette.Rows.Add("20", "#FFFFFF");
                return dt_defaultPallette;
            }
           
        }

        private DataTable pal_temperature_f
        {
            get
            {
                dt_pal_temperature_f = new DataTable();
                dt_pal_temperature_f.Columns.Add("value");
                dt_pal_temperature_f.Columns.Add("color");
                dt_pal_temperature_f.Rows.Add("-58", "#000066");
                //-50 degree Celcius -->
                dt_pal_temperature_f.Rows.Add("32", "#B2B2FF");
                //   0 degree Celcius (freezing point) -->
                dt_pal_temperature_f.Rows.Add("32.1", "#00FF00");
                dt_pal_temperature_f.Rows.Add("50", "#FFFF00");
                // 10 degree Celcius -->
                dt_pal_temperature_f.Rows.Add("68", "#FFCC00");
                // 20 degree Celcius -->
                dt_pal_temperature_f.Rows.Add("86", "#FF8800");
                // 30 degree Celcius -->
                dt_pal_temperature_f.Rows.Add("104", "#FF0000");
                //  40 degree Celcius -->
                dt_pal_temperature_f.Rows.Add("122", "#880000");
                //  50 degree Celcius -->
                return dt_pal_temperature_f;
            }
          
        }
        
        private DataTable pal_dailydswfrBW
        {
            get
            {
                dt_pal_dailydswfrBW = new DataTable();
                dt_pal_dailydswfrBW.Columns.Add("value");
                dt_pal_dailydswfrBW.Columns.Add("color");
                dt_pal_dailydswfrBW.Rows.Add("21", "#FFFFFF");
                dt_pal_dailydswfrBW.Rows.Add("11", "#808080");
                dt_pal_dailydswfrBW.Rows.Add("1", "#000000");
                return dt_pal_dailydswfrBW;
            }
           
        }
        
        private DataTable pal_diseaserisk
        {
            get
            {
                dt_pal_diseaserisk = new DataTable();
                dt_pal_diseaserisk.Columns.Add("value");
                dt_pal_diseaserisk.Columns.Add("color");
                dt_pal_diseaserisk.Rows.Add("0", "#FFFFFF");
                dt_pal_diseaserisk.Rows.Add("1", "#fff700");
                dt_pal_diseaserisk.Rows.Add("2", "#ffbb00");
                dt_pal_diseaserisk.Rows.Add("3", "#ff0000");
                return dt_pal_diseaserisk;
            }
           
        }

        private DataTable pal_severeweather
        {
            get
            {
                dt_pal_severeweather = new DataTable();
                dt_pal_severeweather.Columns.Add("value");
                dt_pal_severeweather.Columns.Add("color");
                dt_pal_severeweather.Rows.Add("0", "#ffEC00");
                dt_pal_severeweather.Rows.Add("1", "#fff700");
                dt_pal_severeweather.Rows.Add("2", "#ffb400");
                dt_pal_severeweather.Rows.Add("3", "#FB5438");
                return dt_pal_severeweather;
            }
           
        }

        private DataTable pal_temperature
        {
            get
            {
                dt_pal_temperature = new DataTable();
                dt_pal_temperature.Columns.Add("value");
                dt_pal_temperature.Columns.Add("color");
                dt_pal_temperature.Rows.Add("-50", "#000066");
                dt_pal_temperature.Rows.Add("0", "#B2B2FF");
                dt_pal_temperature.Rows.Add("0.1", "#00FF77");
                dt_pal_temperature.Rows.Add("10", "#FFFF00");
                dt_pal_temperature.Rows.Add("20", "#FFCC00");
                dt_pal_temperature.Rows.Add("30", "#FF8800");
                dt_pal_temperature.Rows.Add("40", "#FF0000");
                dt_pal_temperature.Rows.Add("50", "#880000");
                return dt_pal_temperature;
            }
           
        }

        private DataTable pal_sip_temperaturef
        {
            get
            {
                dt_pal_sip_temperaturef = new DataTable();
                dt_pal_sip_temperaturef.Columns.Add("value");
                dt_pal_sip_temperaturef.Columns.Add("color");
                dt_pal_sip_temperaturef.Rows.Add("32", "#FFFFFF");
                dt_pal_sip_temperaturef.Rows.Add("68", "#FEB700");
                dt_pal_sip_temperaturef.Rows.Add("104", "#FE6200");
                return dt_pal_sip_temperaturef;
            }
           
        }

        private DataTable pal_rain
        {
            get
            {
                dt_pal_rain = new DataTable();
                dt_pal_rain.Columns.Add("value");
                dt_pal_rain.Columns.Add("color");
                dt_pal_rain.Rows.Add("0.0", "#FFFFFF");
                dt_pal_rain.Rows.Add("0.6", "#0000FF");
                dt_pal_rain.Rows.Add("1.3", "#000088");
                return dt_pal_rain;
            }
           
        }

        private DataTable pal_sip_raininch
        {
            get
            {
                dt_pal_sip_raininch = new DataTable();
                dt_pal_sip_raininch.Columns.Add("value");
                dt_pal_sip_raininch.Columns.Add("color");
                dt_pal_sip_raininch.Rows.Add("0.0", "#FFFFFF");
                dt_pal_sip_raininch.Rows.Add("0.25", "#D9E3EF");
                dt_pal_sip_raininch.Rows.Add("0.50", "#7FA1C8");
                dt_pal_sip_raininch.Rows.Add("0.82", "#004392");
                return dt_pal_sip_raininch;
            }
           
        }

        private DataTable pal_sip_raininch_8hour
        {
            get
            {
                dt_pal_sip_raininch_8hour = new DataTable();
                dt_pal_sip_raininch_8hour.Columns.Add("value");
                dt_pal_sip_raininch_8hour.Columns.Add("color");
                dt_pal_sip_raininch_8hour.Rows.Add("0.0", "#FFFFFF");
                dt_pal_sip_raininch_8hour.Rows.Add("0.94", "#D9E3EF");
                dt_pal_sip_raininch_8hour.Rows.Add("1.89", "#7FA1C8");
                dt_pal_sip_raininch_8hour.Rows.Add("2.83", "#004392");
                return dt_pal_sip_raininch_8hour;
            }
           
        }

        private DataTable pal_rain_risk
        {
            get
            {
                dt_pal_rain_risk = new DataTable();
                dt_pal_rain_risk.Columns.Add("value");
                dt_pal_rain_risk.Columns.Add("color");
                dt_pal_rain_risk.Rows.Add("0", "#FFFFFF");
                dt_pal_rain_risk.Rows.Add("50", "#0000FF");
                dt_pal_rain_risk.Rows.Add("100", "#000088");
                return dt_pal_rain_risk;
            }
            
        }

        private DataTable pal_precip_type
        {
            get
            {
                dt_pal_precip_type = new DataTable();
                dt_pal_precip_type.Columns.Add("value");
                dt_pal_precip_type.Columns.Add("color");
                dt_pal_precip_type.Rows.Add("0", "#FFFFFF");
                dt_pal_precip_type.Rows.Add("1", "#000099");
                dt_pal_precip_type.Rows.Add("2", "#0033FF");
                dt_pal_precip_type.Rows.Add("3", "#AAAAAA");
                dt_pal_precip_type.Rows.Add("4", "#555555");
                return dt_pal_precip_type;
            }
            
        }

        private DataTable pal_clouds
        {
            get
            {
                dt_pal_clouds = new DataTable();
                dt_pal_clouds.Columns.Add("value");
                dt_pal_clouds.Columns.Add("color");
                dt_pal_clouds.Rows.Add("0", "#FFFFFF");
                //Blue
                dt_pal_clouds.Rows.Add("100", "#606060");
                //Dark Gray
                return dt_pal_clouds;
            }
           
        }

        private DataTable pal_combinedrisk
        {
            get
            {
                dt_pal_combinedrisk = new DataTable();
                dt_pal_combinedrisk.Columns.Add("value");
                dt_pal_combinedrisk.Columns.Add("color");
                dt_pal_combinedrisk.Rows.Add("0", "#FFFFFF");
                dt_pal_combinedrisk.Rows.Add("25", "#88FF88");
                dt_pal_combinedrisk.Rows.Add("50", "#FFFF00");
                dt_pal_combinedrisk.Rows.Add("75", "#FF9900");
                dt_pal_combinedrisk.Rows.Add("100", "#FF0000");
                return dt_pal_combinedrisk;
            }
           
        }

        private DataTable pal_humid
        {
            get
            {
                dt_pal_humid = new DataTable();
                dt_pal_humid.Columns.Add("value");
                dt_pal_humid.Columns.Add("color");
                dt_pal_humid.Rows.Add("0", "#66CC33");
                dt_pal_humid.Rows.Add("100", "#000099");
                return dt_pal_humid;
            }
            
        }
        
        private DataTable pal_spraywin
        {
            get
            {
                dt_pal_spraywin = new DataTable();
                dt_pal_spraywin.Columns.Add("value");
                dt_pal_spraywin.Columns.Add("color");
                dt_pal_spraywin.Rows.Add("0", "#FFFFFF");
                dt_pal_spraywin.Rows.Add("1", "#88FF88");
                dt_pal_spraywin.Rows.Add("10", "#FFFF00");
                dt_pal_spraywin.Rows.Add("11", "#FF9900");
                dt_pal_spraywin.Rows.Add("20", "#FF0000");
                dt_pal_spraywin.Rows.Add("21", "#FF0000");
                return dt_pal_spraywin;
            }
           
        }

        private DataTable pal_spraywinfrcp03
        {
            get
            {
                dt_pal_spraywinfrcp03 = new DataTable();
                dt_pal_spraywinfrcp03.Columns.Add("value");
                dt_pal_spraywinfrcp03.Columns.Add("color");
                dt_pal_spraywinfrcp03.Rows.Add("0", "#808080");
                dt_pal_spraywinfrcp03.Rows.Add("1", "#FF0000");
                dt_pal_spraywinfrcp03.Rows.Add("10", "#D3D3D3");
                dt_pal_spraywinfrcp03.Rows.Add("11", "#90EE-90");
                dt_pal_spraywinfrcp03.Rows.Add("20", "#FFFFFF");
                dt_pal_spraywinfrcp03.Rows.Add("21", "#008000");
                return dt_pal_spraywinfrcp03;
            }
           
        }

        private DataTable pal_spraywin_greyscale
        {
            get
            {
                dt_pal_spraywin_greyscale = new DataTable();
                dt_pal_spraywin_greyscale.Columns.Add("value");
                dt_pal_spraywin_greyscale.Columns.Add("color");
                dt_pal_spraywin_greyscale.Rows.Add("0", "#808080");
                dt_pal_spraywin_greyscale.Rows.Add("1", "#808080");
                dt_pal_spraywin_greyscale.Rows.Add("10", "#D3D3D3");
                dt_pal_spraywin_greyscale.Rows.Add("11", "#D3D3D3");
                dt_pal_spraywin_greyscale.Rows.Add("20", "#FFFFFF");
                dt_pal_spraywin_greyscale.Rows.Add("21", "#FFFFFF");
                return dt_pal_spraywin_greyscale;
            } 
           
        }

        private DataTable pal_plant
        {
            get
            {
                dt_pal_plant = new DataTable();
                dt_pal_plant.Columns.Add("value");
                dt_pal_plant.Columns.Add("color");
                dt_pal_plant.Rows.Add("0", "#808080");
                dt_pal_plant.Rows.Add("1", "#FF0000");
                dt_pal_plant.Rows.Add("10", "#D3D3D3");
                dt_pal_plant.Rows.Add("11", "#90EE90");
                dt_pal_plant.Rows.Add("20", "#D3D3D3");
                dt_pal_plant.Rows.Add("21", "#008000");
                return dt_pal_plant;
            }
           
        }

        private DataTable pal_harvest
        {
            get
            {
                dt_pal_harvest = new DataTable();
                dt_pal_harvest.Columns.Add("value");
                dt_pal_harvest.Columns.Add("color");
                dt_pal_harvest.Rows.Add("0", "#808080");
                dt_pal_harvest.Rows.Add("1", "#FF0000");
                dt_pal_harvest.Rows.Add("10", "#D3D3D3");
                dt_pal_harvest.Rows.Add("11", "#90EE90");
                dt_pal_harvest.Rows.Add("20", "#D3D3D3");
                dt_pal_harvest.Rows.Add("21", "#008000");
                return dt_pal_harvest;
            }
            
        }

        // this is now obsolete -->
        private DataTable pal_deltat
        {
            get
            {
                dt_pal_deltat = new DataTable();
                dt_pal_deltat.Columns.Add("value");
                dt_pal_deltat.Columns.Add("color");
                dt_pal_deltat.Rows.Add("0", "#d6d6d6");
                dt_pal_deltat.Rows.Add("2.01", "#fef561");
                dt_pal_deltat.Rows.Add("8", "#d6d6d6");
                dt_pal_deltat.Rows.Add("10", "#fea98c");
                return dt_pal_deltat;
            }
           
        }

        private DataTable pal_deltat_rs
        {
            get
            {
                dt_pal_deltat_rs = new DataTable();
                dt_pal_deltat_rs.Columns.Add("value");
                dt_pal_deltat_rs.Columns.Add("color");
                dt_pal_deltat_rs.Rows.Add("0", "#fea98c");
                dt_pal_deltat_rs.Rows.Add("1", "#d6d6d6");
                dt_pal_deltat_rs.Rows.Add("2", "#fef561");
                return dt_pal_deltat_rs;
            }
           
        }

        private DataTable pal_redorangegreen
        {
            get
            {
                dt_pal_redorangegreen = new DataTable();
                dt_pal_redorangegreen.Columns.Add("value");
                dt_pal_redorangegreen.Columns.Add("color");
                dt_pal_redorangegreen.Rows.Add("01", "#FF0000");
                dt_pal_redorangegreen.Rows.Add("11", "#FFA500");
                dt_pal_redorangegreen.Rows.Add("21", "#00FF00");
                dt_pal_redorangegreen.Rows.Add("00", "#880000");
                dt_pal_redorangegreen.Rows.Add("10", "#884400");
                dt_pal_redorangegreen.Rows.Add("20", "#008000");
                return dt_pal_redorangegreen;
            }
            
        }
        
        /* win speed color table: Beaufort 0 - 12 Bft wind speed m/s = 0.836 * beaufort ^ (3/2)*/
        private DataTable pal_winspeedbft
        {
            get
            {
                dt_pal_winspeedbft = new DataTable();
                dt_pal_winspeedbft.Columns.Add("value");
                dt_pal_winspeedbft.Columns.Add("color");
                dt_pal_winspeedbft.Rows.Add("0", "#FFFFFF");
                dt_pal_winspeedbft.Rows.Add("12", "#777777");
                return dt_pal_winspeedbft;
            }
            
        }
        // win speed color table: Meter / Second 0 - 10 ms 0 - 12 Bft -->

        private DataTable pal_winspeedms
        {
            get
            {
                dt_pal_winspeedms = new DataTable();
                dt_pal_winspeedms.Columns.Add("value");
                dt_pal_winspeedms.Columns.Add("color");
                dt_pal_winspeedms.Rows.Add("0", "#FFFFFF");
                dt_pal_winspeedms.Rows.Add("33", "#000000");
                return dt_pal_winspeedms;
            }
            
        }
        // win speed color table: Kilometer / Hour -->
        private DataTable pal_winspeedkmh
        {
            get
            {
                dt_pal_winspeedkmh = new DataTable();
                dt_pal_winspeedkmh.Columns.Add("value");
                dt_pal_winspeedkmh.Columns.Add("color");
                dt_pal_winspeedkmh.Rows.Add("0", "#FFFFFF");
                dt_pal_winspeedkmh.Rows.Add("120", "#000000");
                return dt_pal_winspeedkmh;
            }
            
        }
        //win speed color table: Miles / Hour -->
        private DataTable pal_winspeedmph
        {
            get
            {
                dt_pal_winspeedmph = new DataTable();
                dt_pal_winspeedmph.Columns.Add("value");
                dt_pal_winspeedmph.Columns.Add("color");
                dt_pal_winspeedmph.Rows.Add("0", "#FFFFFF");
                dt_pal_winspeedmph.Rows.Add("74", "#000000");
                return dt_pal_winspeedmph;
            }
           
        }
        // win speed color table: Kts (SeaMiles / Hour) -->

        private DataTable pal_winspeedkts
        {
            get
            {
                dt_pal_winspeedkts = new DataTable();
                dt_pal_winspeedkts.Columns.Add("value");
                dt_pal_winspeedkts.Columns.Add("color");
                dt_pal_winspeedkts.Rows.Add("0", "#FFFFFF");
                dt_pal_winspeedkts.Rows.Add("64", "#000000");
                return dt_pal_winspeedkts;
            }
           
        }
        //Radiation -->

        private DataTable pal_radiation
        {
            get
            {
                dt_pal_radiation = new DataTable();
                dt_pal_radiation.Columns.Add("value");
                dt_pal_radiation.Columns.Add("color");
                dt_pal_radiation.Rows.Add("0", "#FFFFFF");
                dt_pal_radiation.Rows.Add("333", "#FFFF00");
                dt_pal_radiation.Rows.Add("1000", "FF0000");
                return dt_pal_radiation;
            }
            
        }

        // Relative Sunshine -->
        private DataTable pal_relsunshine
        {
            get
            {
                dt_pal_relsunshine = new DataTable();
                dt_pal_relsunshine.Columns.Add("value");
                dt_pal_relsunshine.Columns.Add("color");
                dt_pal_relsunshine.Rows.Add("0", "#FFFFFF");
                dt_pal_relsunshine.Rows.Add("60", "#FFCA00");
                dt_pal_relsunshine.Rows.Add("100", "#FF6800");
                return dt_pal_relsunshine;
            }
            
        }
        //SKI WINDOW COLOURS -->
        private DataTable pal_ski
        {
            get
            {
                dt_pal_ski = new DataTable();
                dt_pal_ski.Columns.Add("value");
                dt_pal_ski.Columns.Add("color");
                dt_pal_ski.Rows.Add("0", "#FF0000");
                dt_pal_ski.Rows.Add("1", "#0000FF");
                dt_pal_ski.Rows.Add("2", "#FFFF00");
                dt_pal_ski.Rows.Add("3", "#FFFFFF");
                return dt_pal_ski;
            }
            
        }
        // leaf wetness colours -->

        private DataTable pal_leafwetness
        {
            get
            {
                dt_pal_leafwetness = new DataTable();
                dt_pal_leafwetness.Columns.Add("value");
                dt_pal_leafwetness.Columns.Add("color");
                dt_pal_leafwetness.Rows.Add("0", "#FFFFFF");
                dt_pal_leafwetness.Rows.Add("1", "#5555FF");
                return dt_pal_leafwetness;
            }
           
        }

        private DataTable pal_leafwetness_avg
        {
            get
            {
                dt_pal_leafwetness_avg = new DataTable();
                dt_pal_leafwetness_avg.Columns.Add("value");
                dt_pal_leafwetness_avg.Columns.Add("color");
                dt_pal_leafwetness_avg.Rows.Add("0", "#FFFFFF");
                dt_pal_leafwetness_avg.Rows.Add("60", "#5555FF");
                return dt_pal_leafwetness_avg;
            }
            
        }

        private DataTable pal_frost
        {
            get
            {
                dt_pal_frost = new DataTable();
                dt_pal_frost.Columns.Add("value");
                dt_pal_frost.Columns.Add("color");
                dt_pal_frost.Rows.Add("0", "#FFFFFF");
                dt_pal_frost.Rows.Add("1", "#5555DD");
                return dt_pal_frost;
            }
           
        }
        // evaporation colours -->

        private DataTable pal_evaporation
        {
            get
            {
                dt_pal_evaporation = new DataTable();
                dt_pal_evaporation.Columns.Add("value");
                dt_pal_evaporation.Columns.Add("color");
                dt_pal_evaporation.Rows.Add("0", "#BBBBFF");
                dt_pal_evaporation.Rows.Add("1", "#FFFFFF");
                dt_pal_evaporation.Rows.Add("3", "#FFFF88");
                return dt_pal_evaporation;
            }
          
        }

        private DataTable pal_sip_rain_risk
        {
            get
            {
                dt_pal_sip_rain_risk = new DataTable();
                dt_pal_sip_rain_risk.Columns.Add("value");
                dt_pal_sip_rain_risk.Columns.Add("color");
                dt_pal_sip_rain_risk.Rows.Add("0", "#FFFFFF");
                dt_pal_sip_rain_risk.Rows.Add("-1", "#D9E3EF");
                dt_pal_sip_rain_risk.Rows.Add("3", "#7FA1C8");
                dt_pal_sip_rain_risk.Rows.Add("2", "#004392");
                dt_pal_sip_rain_risk.Rows.Add("33", "#D9E3EF");
                dt_pal_sip_rain_risk.Rows.Add("66", "#7FA1C8");
                dt_pal_sip_rain_risk.Rows.Add("100", "#004392");
                return dt_pal_sip_rain_risk;
            }
           
        }

        private DataTable pal_sip_humid
        {
            get
            {
                dt_pal_sip_humid = new DataTable();
                dt_pal_sip_humid.Columns.Add("value");
                dt_pal_sip_humid.Columns.Add("color");
                dt_pal_sip_humid.Rows.Add("0", "#D9EFF8");
                dt_pal_sip_humid.Rows.Add("50", "#7FC9E7");
                dt_pal_sip_humid.Rows.Add("100", "#0093CF");
                return dt_pal_sip_humid;
            }
            
        }
        
        private DataTable pal_sip_window
        {
            get
            {
                dt_pal_sip_window = new DataTable();
                dt_pal_sip_window.Columns.Add("value");
                dt_pal_sip_window.Columns.Add("color");
                dt_pal_sip_window.Columns.Add("text");
                //Day
                dt_pal_sip_window.Rows.Add("1", "#FB5438","ResColLegNotRec");
                dt_pal_sip_window.Rows.Add("11", "#FFB400","ResColLegRestrict");
                dt_pal_sip_window.Rows.Add("21", "#AAB400","ResColLegRecomm");
                //Night
                dt_pal_sip_window.Rows.Add("0", "#E7B09C","ResColLegNotRec");
                dt_pal_sip_window.Rows.Add("10", "#F2CD74","ResColLegRestrict");
                dt_pal_sip_window.Rows.Add("20", "#C2C76C", "ResColLegRecomm");
                return dt_pal_sip_window;
            }
            
        }
        
        private DataTable pal_sip_precip_type
        {
            get
            {
                dt_pal_sip_precip_type = new DataTable();
                dt_pal_sip_precip_type.Columns.Add("value");
                dt_pal_sip_precip_type.Columns.Add("color");
                dt_pal_sip_precip_type.Rows.Add("0", "#FFFFFF");
                dt_pal_sip_precip_type.Rows.Add("1", "#7FA1C8");
                dt_pal_sip_precip_type.Rows.Add("2", "#004392");
                dt_pal_sip_precip_type.Rows.Add("3", "#AAAAAA");
                dt_pal_sip_precip_type.Rows.Add("4", "#555555");

                return dt_pal_sip_precip_type;
            }
          
        }

        private DataTable pal_dailydswfr
        {
            get
            {
                dt_pal_dailydswfr = new DataTable();
                dt_pal_dailydswfr.Columns.Add("value");
                dt_pal_dailydswfr.Columns.Add("color");
                dt_pal_dailydswfr.Rows.Add("21", "#008000");
                dt_pal_dailydswfr.Rows.Add("11", "#ffbb00");
                dt_pal_dailydswfr.Rows.Add("1", "#ff0000");
                return dt_pal_dailydswfr;
            }
            
        }

        private DataTable pal_runoff
        {
            get
            {
                dt_pal_runoff = new DataTable();
                dt_pal_runoff.Columns.Add("value");
                dt_pal_runoff.Columns.Add("color");
                dt_pal_runoff.Rows.Add("0", "#FF0000");
                dt_pal_runoff.Rows.Add("1", "#FFA500");
                dt_pal_runoff.Rows.Add("2", "#FFFFFF");

                return dt_pal_runoff;
            }
           
        }

        private DataTable pal_sip_temperature
        {
            get
            {
                dt_pal_sip_temperature = new DataTable();
                dt_pal_sip_temperature.Columns.Add("value");
                dt_pal_sip_temperature.Columns.Add("color");
                dt_pal_sip_temperature.Rows.Add("0", "#FFFFFF");
                dt_pal_sip_temperature.Rows.Add("20", "#FEB700");
                dt_pal_sip_temperature.Rows.Add("40", "#FE6200");
                return dt_pal_sip_temperature;
            }
            // set { dt_pal_runoff = value; }
        }

        private DataTable pal_sip_rain
        {
            get
            {
                dt_pal_sip_rain = new DataTable();
                dt_pal_sip_rain.Columns.Add("value");
                dt_pal_sip_rain.Columns.Add("color");
                dt_pal_sip_rain.Rows.Add("0", "#FFFFFF");
                dt_pal_sip_rain.Rows.Add("0.4", "#D9E3EF");
                dt_pal_sip_rain.Rows.Add("0.8", "#7FA1C8");
                dt_pal_sip_rain.Rows.Add("1.3", "#004392");
                return dt_pal_sip_rain;
            }
            // set { dt_pal_runoff = value; }
        }
        
        private DataTable pal_sip_rain_8hour
        {
            get
            {
                dt_pal_sip_rain_8hour = new DataTable();
                dt_pal_sip_rain_8hour.Columns.Add("value");
                dt_pal_sip_rain_8hour.Columns.Add("color");
                dt_pal_sip_rain_8hour.Rows.Add("0", "#FFFFFF");
                dt_pal_sip_rain_8hour.Rows.Add("1.5", "#D9E3EF");
                dt_pal_sip_rain_8hour.Rows.Add("3", "#7FA1C8");
                dt_pal_sip_rain_8hour.Rows.Add("4.5", "#004392");
                return dt_pal_sip_rain_8hour;
            }
            // set { dt_pal_runoff = value; }
        }

        private DataTable pal_sip_rain_duration
        {
            get
            {
                dt_pal_sip_rain_duration = new DataTable();
                dt_pal_sip_rain_duration.Columns.Add("value");
                dt_pal_sip_rain_duration.Columns.Add("color");
                dt_pal_sip_rain_duration.Rows.Add("0", "#FFFFFF");
                dt_pal_sip_rain_duration.Rows.Add("20", "#D9E3EF");
                dt_pal_sip_rain_duration.Rows.Add("40", "#7FA1C8");
                dt_pal_sip_rain_duration.Rows.Add("60", "#004392");
                return dt_pal_sip_rain_duration;
            }
            // set { dt_pal_runoff = value; }
        }
        private DataTable pal_bruchid
        {
            get
            {
                dt_pal_bruchid = new DataTable();
                dt_pal_bruchid.Columns.Add("value");
                dt_pal_bruchid.Columns.Add("color");
                dt_pal_bruchid.Rows.Add("2", "White");
                dt_pal_bruchid.Rows.Add("1", "Green");
                return dt_pal_bruchid;
            }
            // set { dt_pal_bruchid = value; }
        }
        private DataTable pal_earlyblight
        {
            get
            {
                dt_pal_earlyblight = new DataTable();
                dt_pal_earlyblight.Columns.Add("value");
                dt_pal_earlyblight.Columns.Add("color");
                dt_pal_earlyblight.Rows.Add("1", "Green");
                dt_pal_earlyblight.Rows.Add("2", "Yellow");
                dt_pal_earlyblight.Rows.Add("3", "Red");
                return dt_pal_earlyblight;
            }
            // set { dt_pal_earlyblight = value; }
        }
        private DataTable pal_temperature_me
        {
            get
            {
                dt_pal_temperature_me = new DataTable();
                dt_pal_temperature_me.Columns.Add("value");
                dt_pal_temperature_me.Columns.Add("color");
                dt_pal_temperature_me.Rows.Add("0", "#FFFFFF");
                dt_pal_temperature_me.Rows.Add("20", "#FEB700");
                dt_pal_temperature_me.Rows.Add("40", "#FE6200");
                return dt_pal_temperature_me;
            }
            // set { dt_pal_temperature_me = value; }
        }
        private DataTable pal_rain_12hour_me
        {
            get
            {
                dt_pal_rain_12hour_me = new DataTable();
                dt_pal_rain_12hour_me.Columns.Add("value");
                dt_pal_rain_12hour_me.Columns.Add("color");
                dt_pal_rain_12hour_me.Rows.Add("0", "#FFFFFF");
                dt_pal_rain_12hour_me.Rows.Add("1.5", "#D9E3EF");
                dt_pal_rain_12hour_me.Rows.Add("3", "#7FA1C8");
                dt_pal_rain_12hour_me.Rows.Add("4.5", "#004392");
                return dt_pal_rain_12hour_me;
            }
            // set { dt_pal_rain_12hour_me = value; }
        }

        private DataTable pal_rain_risk_me
        {
            get
            {
                dt_pal_rain_risk_me = new DataTable();
                dt_pal_rain_risk_me.Columns.Add("value");
                dt_pal_rain_risk_me.Columns.Add("color");
                dt_pal_rain_risk_me.Rows.Add("0", "#FFFFFF");
                dt_pal_rain_risk_me.Rows.Add("33", "#D9E3EF");
                dt_pal_rain_risk_me.Rows.Add("66", "#7FA1C8");
                dt_pal_rain_risk_me.Rows.Add("100", "#004392");
                return dt_pal_rain_risk_me;
            }
            // set { dt_pal_rain_risk_me = value; }
        }

        private DataTable pal_rainDuration_12hour_me
        {
            get
            {
                dt_pal_rainDuration_12hour_me = new DataTable();
                dt_pal_rainDuration_12hour_me.Columns.Add("value");
                dt_pal_rainDuration_12hour_me.Columns.Add("color");
                dt_pal_rainDuration_12hour_me.Rows.Add("0", "#FFFFFF");
                dt_pal_rainDuration_12hour_me.Rows.Add("240", "#D9E3EF");
                dt_pal_rainDuration_12hour_me.Rows.Add("480", "#7FA1C8");
                dt_pal_rainDuration_12hour_me.Rows.Add("720", "#004392");
                return dt_pal_rainDuration_12hour_me;
            }
            // set { dt_pal_rainDuration_12hour_me = value; }
        }

        private DataTable pal_humid_me
        {
            get
            {
                dt_pal_humid_me = new DataTable();
                dt_pal_humid_me.Columns.Add("value");
                dt_pal_humid_me.Columns.Add("color");
                dt_pal_humid_me.Rows.Add("0", "#0093CF");
                dt_pal_humid_me.Rows.Add("33", "#7FC9E7");
                dt_pal_humid_me.Rows.Add("66", "#7FA1C8");
                return dt_pal_humid_me;
            }
            // set { dt_pal_humid_me = value; }
        }

        private DataTable pal_window_me
        {
            get
            {
                dt_pal_window_me = new DataTable();
                dt_pal_window_me.Columns.Add("value");
                dt_pal_window_me.Columns.Add("color");
                //Day
                dt_pal_window_me.Rows.Add("1", "#FFFFFF");
                dt_pal_window_me.Rows.Add("11", "#FEB700");
                dt_pal_window_me.Rows.Add("21", "#FE6200");
                //Night
                dt_pal_window_me.Rows.Add("0", "#FFFFFF");
                dt_pal_window_me.Rows.Add("10", "#FEB700");
                dt_pal_window_me.Rows.Add("20", "#FE6200");
                return dt_pal_window_me;
            }
            // set { dt_pal_window_me = value; }
        }
   
   
        
    }

    public class PaletteMap
        {
            public string id;
            private IDictionary palettelist;
            public PaletteMap()
            {

                palettelist = new SortedList();
            }
            public void addLimit(double limit, string colortxt,string text)
            {
               // Color color = ColorTranslator.FromHtml(colortxt);

                //create a string array of the color and the text(in case of rulesets) and add it as the value in palettelist
                string[] color_text = { colortxt, text };
                palettelist.Add(limit, color_text);
            }
            public string getColor(double value, string series)
            {
                IDictionaryEnumerator penum = palettelist.GetEnumerator(); 
                Color lowercolor = Color.Pink;
                Color highercolor = Color.Pink;
                bool lowinit = false;
                bool highinit = false;
                double lowerlimit = 0;
                double higherlimit = 0;
                while (penum.MoveNext())
                {
                    lowerlimit = higherlimit;
                    higherlimit = Convert.ToDouble(penum.Key);
                    //Read the value into a string array
                    string[] color_text = (string[])penum.Value;
                    //Take the color from the value 
                    Color color = ColorTranslator.FromHtml(color_text[0]);
                    highercolor = color;
                    if (value < higherlimit)
                    {
                        if (!highinit) lowercolor = highercolor;
                        break;
                    }
                    highinit = true;
                    lowercolor = highercolor; lowinit = highinit;
                }
                if (highinit && lowercolor != highercolor && value != lowerlimit)
                {
                    double full = higherlimit - lowerlimit;
                    if (full != 0)
                    {
                        double f1 = (higherlimit - value) / full;
                        double f2 = (value - lowerlimit) / full;
                        lowercolor = Color.FromArgb(
                            (int)((highercolor.R * f2 + lowercolor.R * f1)),
                            (int)((highercolor.G * f2 + lowercolor.G * f1)),
                            (int)((highercolor.B * f2 + lowercolor.B * f1)));
                    }
                }
                return ColorTranslator.ToHtml(lowercolor);
            }
         //overloaded method to read text instead of color for excel
            public string getColor(double value, string series,string source)
            {
                 IDictionaryEnumerator penum = palettelist.GetEnumerator(); 
                string text = string.Empty;
                while (penum.MoveNext())
                {
                    //if value matches the  key, return the text
                    if (value == Convert.ToDouble(penum.Key))
                    {
                        //Read the value into a string array and use the text from it
                        string[] color_text = (string[])penum.Value;
                        text = color_text[1].ToString();
                        break;
                    }
                }
                return text;
            }



        }
}