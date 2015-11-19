using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syngenta.AgriCast.Common.DTO
{

    /// <summary>
    /// Summary description for CustomProfile
    /// </summary>
    [Serializable]
    public class ProfileDTO
    {
        public ProfileDTO()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Company
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string PostalCode
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string City
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        public string PhoneNumber
        {
            get;
            set;
        }

        public string FaxNumber
        {
            get;
            set;
        }

        public string MobileNumber
        {
            get;
            set;
        }

        public string PreferredCultureCode
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string ModifiedAtDateTimeUtc
        {
            get;
            set;
        }

        public string ModifiedByUserID 
        {
            get;
            set;
        }

        //Hold Collection of Subscription Objects
        public Dictionary<string, Subscription> SubcriptionList = new Dictionary<string, Subscription>();

        //Hold Collection of Favorite Objects
        public Dictionary<string, Favorite> FavoriteList = new Dictionary<string, Favorite>();
    }

    [Serializable]
    public class Subscription
    {
        public Subscription()
        {

        }

        DateTime startDateUtc;

        public DateTime StartDateUtc
        {
            get { return startDateUtc; }
            set { startDateUtc = value; }
        }
        DateTime endDateUtc;

        public DateTime EndDateUtc
        {
            get { return endDateUtc; }
            set { endDateUtc = value; }
        }
        int maxNumofUsers;

        public int MaxNumofUsers
        {
            get { return maxNumofUsers; }
            set { maxNumofUsers = value; }
        }
        int actualNumofUsers;

        public int ActualNumofUsers
        {
            get { return actualNumofUsers; }
            set { actualNumofUsers = value; }
        }

    }

    [Serializable]
    public class Favorite
    {
        public Favorite()
        {

        }

        // string name;

        //public string Name
        //{
        //    get { return name; }
        //    set { name = value; }
        //}

        DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        double longitude;

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        double latitude;

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        int altitude;

        public int Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }

        int startOffset;

        public int StartOffset
        {
            get { return startOffset; }
            set { startOffset = value; }
        }
        int endOffset;

        public int EndOffset
        {
            get { return endOffset; }
            set { endOffset = value; }
        }
        int placeID;

        public int PlaceID
        {
            get { return placeID; }
            set { placeID = value; }
        }
        string placeName;

        public string PlaceName
        {
            get { return placeName; }
            set { placeName = value; }
        }
        string moduleName;

        public string ModuleName
        {
            get { return moduleName; }
            set { moduleName = value; }
        }
        string serviceName;

        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }
        int farmID;

        public int FarmID
        {
            get { return farmID; }
            set { farmID = value; }
        }
        int fieldID;

        public int FieldID
        {
            get { return fieldID; }
            set { fieldID = value; }
        }
    }
}



