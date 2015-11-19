
using System.Xml.Serialization;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;

[DataContract]
public  class ServiceOutput {

    private Response ResponseField;
    private List<Chart> ChartField;
    private List<Icons> iconsField;
    private List<tables> tablesField;
    private List<Legend> legendsField;

  
    [DataMember(Order = 0)]
    public Response FeatureResponse
    {
        get
        {
            return this.ResponseField;
        }
        set
        {
            this.ResponseField = value;
        }
    }
   
 
    [DataMember(Order = 1)]
    public List<Chart> Charts
    {
        get {
            return this.ChartField;
        }
        set {
            this.ChartField = value;
        }
    }

 
    [DataMember(Order = 2)]
    public List<Icons> Icons
    {
        get
        {
            return this.iconsField;
        }
        set
        {
            this.iconsField = value;
        }
    }

 
    [DataMember(Order = 3)]
    public List<tables> tables
    {
        get
        {
            return this.tablesField;
        }
        set
        {
            this.tablesField = value;
        }
    }

 
    [DataMember(Order = 4)]
    public List<Legend> Legends
    {
        get
        {
            return this.legendsField;
        }
        set
        {
            this.legendsField = value;
        }
    }
}

[DataContract]
public  class Chart {
    
    private string moduleIDField;
    
    private string chartUrlField;
    

    
    [DataMember(Order = 0)]
    public string ModuleID {
        get {
            return this.moduleIDField;
        }
        set {
            this.moduleIDField = value;
        }
    }
    

    
    [DataMember(Order = 1)]
    public string ChartUrl {
        get {
            return this.chartUrlField;
        }
        set {
            this.chartUrlField = value;
        }
    }
}

[DataContract]
public class Response {

    private FeatureRequest featureRequestedField;

    private StationDetails StationDetailsField;
    
    private string timeZoneOffsetField;
    
    private string sunriseField;
    
    private string sunsetField;
    
    [DataMember(Order = 0)]
    public FeatureRequest FeatureRequest
    {
        get {
            return this.featureRequestedField;
        }
        set {
            this.featureRequestedField = value;
        }
    }
    
    [DataMember(Order = 1)]
    public string TimeZoneOffset {
        get {
            return this.timeZoneOffsetField;
        }
        set {
            this.timeZoneOffsetField = value;
        }
    }
    
    [DataMember(Order = 2)]
    public string Sunrise {
        get {
            return this.sunriseField;
        }
        set {
            this.sunriseField = value;
        }
    }
    
    [DataMember(Order = 3)]
    public string Sunset {
        get {
            return this.sunsetField;
        }
        set {
            this.sunsetField = value;
        }
    }

    [DataMember(Order = 4)]
    public StationDetails StationDetails
    {
        get
        {
            return this.StationDetailsField;
        }
        set
        {
            this.StationDetailsField = value;
        }
    }
}

public class FeatureRequest
{
    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="Latitude">Latitude</param>
    /// <param name="Longitude">Longitude</param>
    /// <param name="Altitude">Altitude</param>
    /// <param name="MaxAltitudeDiff">Maximum Altitude Difference</param>
    /// <param name="MaxDistanceDiff">Maximum Distance Difference</param>
    /// 

    public FeatureRequest()
    {
    }

    public FeatureRequest(double latitude, double longitude, double altitude,
         double maxAltitudeDiff, double maxDistanceDiff)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.Altitude = altitude;
        this.MaxAltitudeDiff = maxAltitudeDiff;
        this.MaxDistanceDiff = maxDistanceDiff;
    }
    /// <summary>
    /// Latitude
    /// </summary>
    public double Latitude { get; set; }
    /// <summary>
    /// Longitude
    /// </summary>
    public double Longitude { get; set; }
    /// <summary>
    /// Altitude
    /// </summary>
    public double Altitude { get; set; }
    /// <summary>
    /// Maximum Altitude Difference
    /// </summary>
    public double MaxAltitudeDiff { get; set; }
    /// <summary>
    /// Maximum Distance Difference
    /// </summary>
    public double MaxDistanceDiff { get; set; }

}

public class StationDetails
{
    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="DataPointID">Data point ID</param>
    /// <param name="Latitude">Latitude</param>
    /// <param name="Longitude">Longitude</param>
    /// <param name="Altitude">Altitude</param>
    /// <param name="Distance">Distance</param>
    /// <param name="BearingDegrees">Bearing degrees</param>
    public StationDetails(string name, double Latitude, double Longitude, double Altitude,
        double Distance, double BearingDegrees)
    {
        this.Name = name;
        this.Latitude = Latitude;
        this.Longitude = Longitude;
        this.Altitude = Altitude;
        this.Distance = Distance;
        this.BearingDegrees = BearingDegrees;
    }

    public StationDetails() { }
    /// <summary>
    /// Station Name
    /// </summary>
    public string Name { get;  set; }
    /// <summary>
    /// Latitude
    /// </summary>
    public double Latitude { get;  set; }
    /// <summary>
    /// Longitude
    /// </summary>
    public double Longitude { get;  set; }
    /// <summary>
    /// Altitude
    /// </summary>
    public double Altitude { get;  set; }
    /// <summary>
    /// Distance
    /// </summary>
    public double Distance { get;  set; }
    /// <summary>
    /// Bearing degrees
    /// </summary>
    public double BearingDegrees { get;  set; }
}

[DataContract]
public  class Icons {
    
    private string moduleIDField;
    
    private List<IconData> icondataField;
    

    
    [DataMember(Order = 0)]
    public string ModuleID {
        get {
            return this.moduleIDField;
        }
        set {
            this.moduleIDField = value;
        }
    }
         

    
    [DataMember(Order = 1)]
    public List<IconData> IconList
    {
        get {
            return this.icondataField;
        }
        set {
            this.icondataField = value;
        }
    }
}

 [DataContract] 
public  class IconData
{
    private string dateField;

    private string iconPathField;

    private string iconToolTipField; 


    
    [DataMember(Order = 0)]
    public string Date
    {
        get
        {
            return this.dateField;
        }
        set
        {
            this.dateField = value;
        }
    }


    
    [DataMember(Order = 1)]
    public string IconPath
    {
        get
        {
            return this.iconPathField;
        }
        set
        {
            this.iconPathField = value;
        }
    }


    
    [DataMember(Order = 2)]
    public string IconToolTip
    {
        get
        {
            return this.iconToolTipField;
        }
        set
        {
            this.iconToolTipField = value;
        }
    }
}

[DataContract]
public  class tables
{
    private string moduleIDField;
    private List<table> tableListField;

    [DataMember(Order = 1)]
    public List<table> tableList
    {
        get
        {
            return this.tableListField;
        }
        set
        {
            this.tableListField = value;
        }
    }


    
    [DataMember(Order = 0)]
    public string ModuleID
    {
        get
        {
            return this.moduleIDField;
        }
        set
        {
            this.moduleIDField = value;
        }
    }
}

[DataContract]
public  class table
{
    private string tableNameField;
    private List<HeaderRow> headerRowField;
    private List<tableRow> tableRowField;

    [DataMember(Order = 0)]
    public string TableName
    {
        get
        {
            return this.tableNameField;
        }
        set
        {
            this.tableNameField = value;
        }
    }


    
    [DataMember(Order = 1)]
    public List<HeaderRow> HeaderRows
    {
        get
        {
            return this.headerRowField;
        }
        set
        {
            this.headerRowField = value;
        }
    }


    
    [DataMember(Order = 2)]
    public List<tableRow> tableRows
    {
        get
        {
            return this.tableRowField;
        }
        set
        {
            this.tableRowField = value;
        }
    } 
}

[DataContract]
public  class tableRow
{
    private List<tableCell> tableCellField;

    [DataMember]
    public List<tableCell> tableCells
    {
        get
        {
            return this.tableCellField;
        }
        set
        {
            this.tableCellField = value;
        }
    }
}

[DataContract]
public  class tableCell
{

    private string valueField;

    private string colorField;

    private string toolTipField;
   
    private string headerField;

    private string imageField;

    private string bgColorField;

    [DataMember(Order = 0)]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }

    [DataMember(Order = 1)]
    public string Color
    {
        get
        {
            return this.colorField;
        }
        set
        {
            this.colorField = value;
        }
    }

    [DataMember(Order = 2)]
    public string ToolTip
    {
        get
        {
            return this.toolTipField;
        }
        set
        {
            this.toolTipField = value;
        }
    }

    [DataMember(Order = 3)]
    public string CellImage
    {
        get
        {
            return this.imageField;
        }
        set
        {
            this.imageField = value;
        }
    }

    [DataMember(Order = 4)]
    public string bgColor
    {
        get
        {
            return this.bgColorField;
        }
        set
        {
            this.bgColorField = value;
        }
    }

    [DataMember]  
    public string Header
    {
        get
        {
            return this.headerField;
        }
        set
        {
            this.headerField = value;
        }
    }
}

[DataContract]
public  class HeaderCell
{
    private string valueField;

    private string colspanField;

    [DataMember(Order = 0)]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    } 

    [DataMember(Order = 1)]
    public string Colspan
    {
        get
        {
            return this.colspanField;
        }
        set
        {
            this.colspanField = value;
        }
    }
}

[DataContract]
public  class HeaderRow
{
    private List<HeaderCell> headerCellField; 

    [DataMember]
    public List<HeaderCell> HeaderCells
    {
        get
        {
            return this.headerCellField;
        }
        set
        {
            this.headerCellField = value;
        }
    } 
}

[DataContract]
public  class Legend
{

    private string moduleIDField;

    private string htmlStringField;

    [DataMember(Order = 0)]
    public string ModuleID
    {
        get
        {
            return this.moduleIDField;
        }
        set
        {
            this.moduleIDField = value;
        }
    }


    
    [DataMember(Order = 1)]
    public string HTMLString
    {
        get
        {
            return this.htmlStringField;
        }
        set
        {
            this.htmlStringField = value;
        }
    }
}

