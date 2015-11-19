using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;

namespace Syngenta.AgriCast.Common
{
    public class Units
    {
        public DataRow  GetChangedUnits(DataRow dr, string Unit, string WUnit)
        {
            string Rowname = string.Empty;
            if (dr.Table.Columns.Contains("name"))
            {
                 Rowname = dr["name"].ToString().ToLower();
            }
            else if (dr.Table.Columns.Contains("trnsTag")) 
            {
                 Rowname = dr["trnsTag"].ToString().ToLower();
            }
          

            if (Rowname.ToLower().Contains("temp"))
            {
                if (Unit.ToLower() == "imperial")
                {
                    if (Rowname.Contains("_c_") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Replace("_c_", "_f_");

                    }
                    else if (Rowname.EndsWith("_c") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_f");
                    }

                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString().ToLower() == "temperature") ? "temperaturef" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString().ToLower() == "temperaturet") ? "temperaturef" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString().ToLower() == "temperature_max") ? "temperaturef_max" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString().ToLower() == "temperature_min") ? "temperaturef_min" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("labelText"))
                    {
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperature") ? "temperaturef" : (dr["labelText"]);
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperaturet") ? "temperaturef" : (dr["labelText"]);
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperature_max") ? "temperaturef_max" : (dr["labelText"]);
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperature_min") ? "temperaturef_min" : (dr["labelText"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_temperature") ? "pal_sip_temperaturef" : (dr["colorPaletteName"]);
                    }
                }


                if (Unit.ToLower() == "metric")
                {

                    if (Rowname.Contains("_f_") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Replace("_f_", "_c_");
                    }
                    else if (Rowname.EndsWith("_f") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_c");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "temperaturef") ? "temperature" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "temperaturef_max") ? "temperature_max" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "temperaturef_min") ? "temperature_min" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("labelText"))
                    {
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperaturef") ? "temperature" : (dr["labelText"]); 
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperaturef_max") ? "temperature_max" : (dr["labelText"]);
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "temperaturef_min") ? "temperature_min" : (dr["labelText"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_temperaturef") ? "pal_sip_temperature" : (dr["colorPaletteName"]);
                    }
                }
            }

            if (Rowname.ToLower().Contains("precip"))
            {
                if (Unit.ToLower() == "imperial")
                {
                    if (Rowname.Contains("_mm_") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Replace("_mm_", "_in_");
                    }
                    else if (Rowname.EndsWith("_mm") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_in");
                    }
                    //Changing transTags if required
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "precipitation") ? "precipitationinch" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "precip2") ? "Precip2inch" : (dr["trnsTag"]);
                       
                    }
                    if (dr.Table.Columns.Contains("labelText"))
                    {
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "precipitation") ? "precipitationinch" : (dr["labelText"]);                        
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_rain_8hour") ? "pal_sip_raininch_8hour" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_rain") ? "pal_sip_raininch" : (dr["colorPaletteName"]);
                    }
                }

                if (Unit.ToLower() == "metric")
                {
                    if (Rowname.Contains("_in_") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Replace("_in_", "_mm_");
                    }
                    else if (Rowname.EndsWith("_in") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mm");
                    }

                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "precipitationinch") ? "precipitation" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "Precip2inch") ? "Precip2" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("labelText"))
                    {
                        dr["labelText"] = (dr["labelText"].ToString().ToLower() == "precipitationinch") ? "precipitation" : (dr["labelText"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_raininch_8hour") ? "pal_sip_rain_8hour" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_raininch") ? "pal_sip_rain" : (dr["colorPaletteName"]);
                    }
                }
            }
    
            if (Rowname.ToLower().Contains("visib"))
            {
                if (Unit.ToLower() == "imperial")
                {
                    if (Rowname.Contains("_km_") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Replace("_km_", "_mil_");
                    }
                    else if (Rowname.EndsWith("_km") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mil"); 
                    }

                 

                }
                if (Unit.ToLower() == "metric")
                {

                    if (Rowname.Contains("_mil_") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Replace("_mil_", "_km_");
                    }
                    else if (Rowname.EndsWith("_mil") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_km");
                    }
                }
            }

            if (Rowname.ToLower().Contains("wind"))
            {
                if (Unit.ToLower() == "imperial")
                {
                    if (Rowname.Contains("_kph_") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Replace("_kph_", "_mph_");
                    }
                    else if (Rowname.EndsWith("_kph") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mph"); 
                    }
                    else if (Rowname.Contains("_ms_") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Replace("_ms_", "_mph_");
                    }
                    else if (Rowname.EndsWith("_ms") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mph");
                    }

                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_kmh") ? "we_wind_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_kmh") ? "wind_gusts_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_ms") ? "we_wind_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_ms") ? "wind_gusts_mph" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedkmh") ? "pal_winspeedmph" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedms") ? "pal_winspeedmph" : (dr["colorPaletteName"]);
                    }

                }
                if (Unit.ToLower() == "metric")
                {
                    if (Rowname.Contains("_mph_") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Replace("_mph_", "_kph_");
                    }
                    else if (Rowname.EndsWith("_mph") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_kph");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_mph") ? "we_wind_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "wind_gusts_mph") ? "we_windgusts_kmh" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedmph") ? "pal_winspeedkmh" : (dr["colorPaletteName"]);
                    }

                }

                //Code to handle custom units
                if (WUnit.ToLower() == "beaufort")
                {
                    if (Rowname.Contains("_mph_"))
                    {
                        Rowname = Rowname.Replace("_mph_", "_bft_");
                    }
                    else if (Rowname.EndsWith("_mph"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_bft");
                    }
                    else if(Rowname.Contains("_kph_"))
                    {
                        Rowname = Rowname.Replace("_kph_", "_bft_");
                    }
                    else if (Rowname.EndsWith("_kph"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_bft");
                    }
                    else if (Rowname.Contains("_ms_"))
                    {
                        Rowname = Rowname.Replace("_ms_", "_bft_");
                    }
                    else if (Rowname.EndsWith("_ms"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_bft");
                    }
                    //Changing transTags if required
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_mph") ? "we_wind_bft" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "wind_gusts_mph") ? "we_windgusts_bft" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_kmh") ? "we_wind_bft" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_kmh") ? "we_windgusts_bft" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_ms") ? "we_wind_bft" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_ms") ? "we_windgusts_bft" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedmph") ? "pal_winspeedbft" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedkmh") ? "pal_winspeedbft" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedms") ? "pal_winspeedbft" : (dr["colorPaletteName"]);
                    }

                }
                else if (WUnit.ToLower() == "mph")
                {
                    if (Rowname.Contains("_bft_"))
                    {
                        Rowname = Rowname.Replace("_bft_", "_mph_");
                    }
                    else if (Rowname.EndsWith("_bft"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mph");
                    }
                    else if (Rowname.Contains("_kph_"))
                    {
                        Rowname = Rowname.Replace("_kph_", "_mph_");
                    }
                    else if (Rowname.EndsWith("_kph"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mph");
                    }
                    else if (Rowname.Contains("_ms_"))
                    {
                        Rowname = Rowname.Replace("_ms_", "_mph_");
                    }
                    else if (Rowname.EndsWith("_ms"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_mph");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required pal_winspeedms
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_bft") ? "we_wind_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_bft") ? "wind_gusts_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_kmh") ? "we_wind_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_kmh") ? "wind_gusts_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_ms") ? "we_wind_mph" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_ms") ? "wind_gusts_mph" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedbft") ? "pal_winspeedmph" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedkmh") ? "pal_winspeedmph" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedms") ? "pal_winspeedmph" : (dr["colorPaletteName"]);
                    }

                }
                else if (WUnit.ToLower() == "kmh")
                {
                    if (Rowname.Contains("_bft_"))
                    {
                        Rowname = Rowname.Replace("_bft_", "_kph_");
                    }
                    else if (Rowname.EndsWith("_bft"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_kph");
                    }
                    else if (Rowname.Contains("_mph_"))
                    {
                        Rowname = Rowname.Replace("_mph_", "_kph_");
                    }
                    else if (Rowname.EndsWith("_mph"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_kph");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_bft") ? "we_wind_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_bft") ? "we_windgusts_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_mph") ? "we_wind_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "wind_gusts_mph") ? "we_windgusts_kmh" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {

                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedbft") ? "pal_winspeedkmh" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedmph") ? "pal_winspeedkmh" : (dr["colorPaletteName"]);
                    }

                }
                else if (WUnit.ToLower() == "ms")
                {
                    if (Rowname.Contains("_bft_"))
                    {
                        Rowname = Rowname.Replace("_bft_", "_ms_");
                    }
                    else if (Rowname.EndsWith("_bft"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_ms");
                    }
                    else if (Rowname.Contains("_mph_"))
                    {
                        Rowname = Rowname.Replace("_mph_", "_ms_");
                    }
                    else if (Rowname.EndsWith("_mph"))
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_ms");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_bft") ? "we_wind_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_windgusts_bft") ? "we_windgusts_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "we_wind_mph") ? "we_wind_kmh" : (dr["trnsTag"]);
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "wind_gusts_mph") ? "we_windgusts_kmh" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {

                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedbft") ? "pal_winspeedkmh" : (dr["colorPaletteName"]);
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_winspeedmph") ? "pal_winspeedkmh" : (dr["colorPaletteName"]);
                    }

                }

            }
            if (Rowname.ToLower().Contains("dewpoint"))
            {
                if (Unit.ToLower() == "imperial")
                {

                    if (Rowname.Contains("_c_") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Replace("_c_", "_f_");
                    }
                    else if (Rowname.EndsWith("_c") && Unit.ToLower() == "imperial")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_f");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "dewpoint") ? "dewpointf" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {
                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_temperature") ? "pal_sip_temperaturef" : (dr["colorPaletteName"]);
                    }
                }
                if (Unit.ToLower() == "metric")
                {

                    if (Rowname.Contains("_f_") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Replace("_f_", "_c_");
                    }
                    else if (Rowname.EndsWith("_f") && Unit.ToLower() == "metric")
                    {
                        Rowname = Rowname.Remove(Rowname.LastIndexOf('_'));
                        Rowname = Rowname + ("_c");
                    }
                    if (dr.Table.Columns.Contains("trnsTag"))
                    {
                        //Changing transTags if required
                        dr["trnsTag"] = (dr["trnsTag"].ToString() == "dewpointf") ? "dewpoint" : (dr["trnsTag"]);
                    }
                    if (dr.Table.Columns.Contains("colorPaletteName"))
                    {

                        //Changing colorPalettes if required
                        dr["colorPaletteName"] = (dr["colorPaletteName"].ToString() == "pal_sip_temperaturef") ? "pal_sip_temperaturef" : (dr["colorPaletteName"]);
                    }
                }
            }
            //dr["name"] = Rowname;
            if (dr.Table.Columns.Contains("name"))
            {
                dr["name"] = Rowname;
            }
           
            return dr;
        }

        
    }
}