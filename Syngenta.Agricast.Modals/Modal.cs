using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syngenta.Agricast.Modals
{
    public class Modal
    {

        
        
    }
    /// <summary>
    /// This Class is used to calculate Sunrise/set data
    /// </summary>
    public class SunriseSunset
    {
        private DateTime Sunrise;
        private DateTime Sunset;

        public enum SunThreshold
        {
            Official,
            Civil,
            Nautical,
            Astronomical
        }

        private static double ThreshToDouble(SunThreshold s)
        {
            switch (s)
            {
                case SunThreshold.Official: return 90.83333333333;
                case SunThreshold.Civil: return 96;
                case SunThreshold.Nautical: return 102;
                case SunThreshold.Astronomical: return 108;
            } throw new Exception("Unknown SunThreshold value " + s.ToString());
        }

        public DateTime getSunrise()
        {
            return Sunrise;
        }
        public DateTime getSunset()
        {
            return Sunset;
        }

        /// <summary>
        /// checks if the given date is in the night or during the day. Returns true if at night.
        /// </summary>
        /// <param name="fdate"></param>
        /// <returns></returns>
        public bool isNight(DateTime fdate)
        {
            DateTime sunRise = Sunrise.AddHours(-1);
            int minuteRise = sunRise.Hour * 60 + sunRise.Minute;
            int minuteSet = Sunset.Hour * 60 + Sunset.Minute;
            bool ssflip = false;
            if (minuteRise > minuteSet)
            {
                ssflip = true;
                int t = minuteRise;
                minuteRise = minuteSet;
                minuteSet = t;
            }
            int cMinute = fdate.Hour * 60 + fdate.Minute;
            return (cMinute <= minuteRise || cMinute >= minuteSet) ^ ssflip;
        }
    }
}
