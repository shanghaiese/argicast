using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syngenta.AgriCast.Common.DTO
{
    public class StationInfo
    {
        private int intAlt;
        private string strStnName;
        private float fStnLat;
        private float fStnLong;
        private int intGridID;
        private string strDirLetter;
        private int intStnId;
        private string strStnProvider;

        public int altitude
        {
            get
            {
                return intAlt;
            }
            set
            {
                intAlt = value;
            }
        }

        public string directionLetter
        {
            get
            {
                return strDirLetter;
            }
            set
            {
                strDirLetter = value;
            }
        }

        public float stationLatitude
        {
            get
            {
                return fStnLat;
            }
            set
            {
                fStnLat = value;
            }
        }

        public float stationLongitude
        {
            get
            {
                return fStnLong;
            }
            set
            {
                fStnLong = value;
            }
        }

        public string stationName
        {
            get
            {
                return strStnName;
            }
            set
            {
                strStnName = value;
            }
        }

        public int stationID
        {
            get
            {
                return intStnId;
            }
            set
            {
                intStnId = value;
            }
        }
        public int gridID
        {
            get
            {
                return intGridID;
            }
            set
            {
                intGridID = value;
            }
        }

        public string StationProvider
        {
            get
            {
                return strStnProvider;
            }
            set
            {
                strStnProvider = value;
            }
        }

        public bool CheckStationValidity()
        {
            return true;
        }
    }
}
