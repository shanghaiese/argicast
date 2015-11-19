using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Constants
/// </summary>
namespace Syngenta.AgriCast.Common
{
    public class Constants
    {
        public Constants()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //public const string BACKGROUND_PATH = HttpRuntime.AppDomainAppPath + @"\Images\captcha2.png";
        public const int CAPTCHA_LENTGH = 8;
        public const int FONT_SIZE = 16;
        public const string FONT_FAMILY = "Arial";
        public const string TEXT_COLOR = "Gray";
        public const string ERROR_MESSAGE = "Try Another Image";
        public const string CHARACTER_SET = "123456789";


        public const string USER_RATING = "User Rating";
        /*IM01294335	New Agricast - rating not sent - Begin*/
        public const string USER_COMMENT = "User Comment";
        /*IM01294335	New Agricast - rating not sent - End*/
        //Constants used to create Favorites Datatable
        //Constants to Create FavoriteDataTable
        public const string FAV_FAVORITENAME = "FavoriteName";
        public const string FAV_SERVICENAME = "ServiceName";
        public const string FAV_MODULENAME = "ModuleName";
        public const string FAV_PLACENAME = "PlaceName";
        public const string FAV_LATITUDE = "Latitude";
        public const string FAV_LONGITUDE = "Longitude";
        public const string FAV_Key = "Key";
        public const string FAV_STARTDATE = "StartDate";
        public const string FAV_ENDDATE = "EndDate";
        public const string FAV_ALTITUDE = "Altitude";
        public const string FAV_STARTOFFSET = "StartOffset";
        public const string FAV_ENDOFFSET = "EndOffset";
        public const string FAV_PLACEID = "PlaceID";
        public const string FAV_FARMID = "FarmID";
        public const string FAV_FIELDID = "FieldID";
        public const string EMAILPDFFOLDER = "mailpdf";

        /* IM01365142 - New Agricast - Favorite table - translation for "Edit" and "Delete" - Jerrey - Begin */
        public const string FAV_EDIT = "Fav_edit";
        public const string FAV_DELETE = "Fav_delete";
        /* IM01365142 - New Agricast - Favorite table - translation for "Edit" and "Delete" - Jerrey - End */

        //Used in Row Select of favorites grid :- other constants of favs used are Altitude , lat and long.
        public const string COUNTRY_NAME = "CountryName";
        public const string COUNTRY_CODE = "Code";

        //Used in unit conversion
        public const string ResDistFromImp = "Distance: {Dist} miles {Dir} from <strong>{CityName}</strong>, elevation {Elevation} feet asl";
        public const string FEET_ASL = "ft asl";
        public const string M_ASL = "m asl";

        //TransTags
        public const string TERMS_AND_CONDITIONS = "Terms and Conditions";
        public const string PRIVACY_POLICY = "Privacy Poilcy";
        public const string CONTACTS = "Contacts";
        public const string MOBILE_SITE = "Mobile Site";
        public const string DATE_SCALE = "Date Scale";
        public const string START_DATE = "Start Date";
        public const string END_DATE = "End Date";
        public const string DURATION = "Duration";
        public const string AGGREGATION = "Aggregation";
        public const string CROP_INFORMATION = "Crop Information";
        public const string PLANTING_DATE = "cq_plantingdate";
        public const string SERIES = "Series";
        public const string APPLY_TO_CHART = "Apply To Chart";
        public const string CLEAR_SELECTION = "Clear Selection";
        public const string ALTITUDE = "Altitude";
        public const string ACCUMULATE = "accumulate";
        public const string SELECTED_YEAR = "we_selected_year";
        public const string TRENDS = "Trends";
        public const string ALTITUDE_ADJUSTMENTS = "altitude_adjustment";
        public const string METHOD = "cq_method";
        public const string GDDCAP = "gddcap";
        public const string GDDBASE = "cq_gddbase";
        public const string SAVE = "save";
        public const string SEARCH = "search";
        public const string SEARCH_IN = "SearchIn";
        public const string LIST_VIEW = "List View";
        public const string MAP_VIEW = "Map View";
        public const string NEARBY_GRID = "ResNearbyGrid";
        public const string NEARBY_STATIONS = "ResNearbyStations";
        public const string DISTANCE_FROM = "ResDistanceFrom";
        public const string NAME = "Place_Name";
        public const string LATITUDE = "Latitude";
        public const string LONGITUDE = "Longitude";
        public const string ADMIN1 = "Admin_Name_1";
        public const string ADMIN2 = "Admin_Name_2";
        public const string NEAR_STATION_NAME = "NearStationName";
        public const string NEAR_DISTANCE = "NearDistance";
        public const string PRINT_PAGE = "Print Page";
        public const string EMAIL = "email";
        public const string EXCEL = "Excel";
        public const string FAVORITES = "Favorites";
        public const string FEEDBACK = "FeedBack";
        public const string WIND = "Wind";
        public const string OK = "ok";
        public const string CANCEL = "Cancel";
        public const string FEE_TO_EMAIL_ADDRESS = "Fee_ToEmailAddress";
        public const string SUBJECT = "subject";
        public const string MAILBODY = "mailbody";
        public const string EMAIL_PAGE = "EMailPage";
        public const string FAV_ADD_TO_FAVOURITES = "Fav_AddToFavorites";
        public const string SERVICE = "Fav_Service";
        public const string LOCATION_NAME = "LocationName";
        public const string FEE_FROM_EMAIL_ADDRESS = "Fee_FromEmailAddress";
        public const string YOUR_COMMENTS = "yourcomments";
        public const string RATE_THIS_SITE = "Rate this site";
        public const string CAPTCHA_EXPLANATION = "captcha_explanation";
        public const string SEND = "All_SubmitForm";
        public const string WELCOME = "Welcome";

        /*IM01294326 -New Agricast - Feedback section - Begin*/
        public const string FEEDBACK_SUBJECT = "Feedback_Subject";
        /*IM01294326 -New Agricast - Feedback section - End*/

        /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - BEGIN */
        public const string CALENDAR_TOOLTIP = "Click to see the calendar";
        /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - END */

        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        /*Constants Used for Moss menu Top Navigation*/
        public const string MOSSMENU_WEBSERVICE_PARAM_SERVERNAME = "www3.syngenta.com";
        public const string MOSSMENU_WEBSERVICE_PARAM_COUNTRY = "country";
        public const string MOSSMENU_WEBSERVICE_DEFAULT_CULTURE = "fr-FR";
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/

        /*IM01277709 - change in spray window. begin*/
        public const string AM = "am";
        public const string PM = "pm";
        /*IM01277709 - change in spray window. end*/

        #region Generic Messages
        public const string GENERIC_ALREADYEXISTS = " already exists.";
        public const string GENERIC_ERRORONPAGE = "The following error has occured on the page ";
        public const string GENERIC_DETAILSNOTFOUND = "No details found for this location";
        public const string GENERIC_NOSERVICE = "The selected service is not present in the current publication";
        public const string CULTURE_LOADFAILURE = "The culture could not be loaded due to the following error:";
        public const string UNITS_LOADFAILURE = "The units could not be loaded due to the following error:";
        public const string TRANSLATION_FAILURE = "The following error occured during translation:";
        public const string CULTURECODE_FAILURE = "The following error occured while retrieving the culture code : ";
        public const string SUNRISEORSET_FAILURE = "The following error occured while fetching sunrise and sunset time : ";
        public const string GEN_SESSIONDETAILS_FAILURE = "The session details could not be fetched due to the following error: ";
        public const string GEN_LOADSERIES_FALURE = "The series could not be loaded due to the following error";
        public const string GEN_EMAILIDNOTFOUND = "E Mail ID not available";
        public const string GEN_LATLONGINVALID = "The given lat long does not belong to the country selected";
        public const string GEN_ADVOPTIONSERROR = "The advanced options can be set only for selected controls";
        public const string GEN_ADV_OPTIONTEXT = "Advanced Options";
        public const string GEN_USERNAME_DECRYPT = "The username could not be decrypted";


        #endregion

        #region ToolBar Messages

        public const string TOOLBAR_LOADFAILURE = "The following error occured while retrieving the toolbar :";
        public const string TOOLBAR_MENUTABS = "The following error occured while creating menu tabs:";
        public const string TOOLBAR_DEFAULTLOCATION = "The following error occured while setting the default location";
        public const string EXCEL_FAILURE = "The excel could not be generated due to the following error:";

        public const string EMAIL_SUCCESS = "emailsuccess";
        public const string EMAIL_FAILURE = "emailFailure";

        public const string FAVORITE_DELETE_SUCCESS = "Favourite has been deleted successfully.";
        public const string FAVORITE_DELETE_FAILURE = "Favourite could not be deleted. Please try again later.";
        public const string FAVORITE_ADD_SUCCESS = "Favorite Added successfully";
        public const string FAVORITE_ADD_FAILURE = "Please Enter a Favorite Name";
        public const string FAVORITE_UPDATE_SUCCESS = "Favourite has been updated successfully.";
        public const string FAVORITE_UPDATE_DUPLICATE = "Update Failed.Favourite with the name";
        public const string FAVORITE_UPDATE_FAILURE = "Favourite could not be updated. Please try again later.";

        public const string FAVORITE_GENERIC_FAILURE = "The favorites could not be displayed due to the following error:";
        public const string FAVORITE_SORTING_ERROR = "The following error occured during sorting: ";

        public const string FEEDBACK_CAPTCHA_LOADFAILURE = "The captcha image could not be loaded due to the following error:";
        public const string FEEDBACK_CAPTCHA_VALIDATION = "The following error occured during captcha validation:";
        public const string FEEDBACK_EMPTY_CAPTCHA = "Please enter the captcha text.";
        public const string FEEDBACK_SUCCESS = "Message sent successfully.";
        public const string FEEDBACK_FILLFORM = "Please fill the form completely.";
        public const string FEEDBACK_INCORRECT_CAPTCHA = "Incorrect captcha. Try again.";
        public const string FEEDBACK_EMAIL_VALIDATION = "Please enter a valid email id";
        public const string FEEDBACK_GENERROR = "The message could not be sent due to the following error:";
        public const string NODELIST_FETCH_FAILURE = "The following error occured while fetching the node list: ";
        public const string UNITS_CUSTOM = "Custom...";

        public const string EMAIL_INVALIDADDRESS = "Invalid E-MailId format";
        #endregion

        #region LocationSearch Mesages
        public const string LS_LOAD_FAILURE = "The following error while loading location search : ";
        public const string LS_SEARCH_FAILURE = "The following error occured while searching for the location : ";
        public const string LS_ROW_EDIT = "The row could not be edited due to the following error : ";
        public const string LS_SORT_GRIDS = "The following error  occured while sorting stations : ";
        public const string LS_NOSTATIONSFOUND = "No stations found for the selected location";
        public const string LS_FETCH_COUNTRIES_FAILURE = "The following error occured while fetching the list of countries : ";
        public const string LS_FETCH_LOCATION_FAILURE = "The following error occured while fetching location : ";
        public const string LS_SETPARAMETERS_FALURE = "The following error occured while setting station parameters : ";
        public const string LS_SETSTAIONATTRB_FALURE = "The following error occured while setting nearby station attributes : ";
        public const string LS_STATIONLOAD_FAILURE = "The following error occured while loading stations : ";
        public const string LS_SETCOLUMNTEXT_FAILURE = "The following error occured while setting column text: ";
        public const string LS_GETPROVIDERNAME_FALIURE = "The following error occured while retrieving the provider name : ";
        public const string LS_NEARBYPONITS_FAILURE = "The following error occured while retrieving the nearby point data : ";
        #endregion

        #region Icon Messages
        //ICON CONSTANTS
        public const string ICON_IMAGEFORMAT = "png";
        public const string ICON_TRANSPARENT = "t";
        public const string ICON_OPAQUE = "o";
        public const string ICON_ISNIGHT = "n";
        public const string ICON_ISDAY = "d";

        public const string ICONS_LOAD_FAILURE = "The icons could not be loaded due the following error: ";
        public const string ICONS_DISPLAY_FAILURE = "The icons could not be displayed due the following error:";
        #endregion

        #region Charting Messages
        public const string CHART_LOAD_FAILURE = "The following error occured while loading charts : ";
        public const string CHART_FETCH_FAILURE = "The following error occured while retrieving chart data :";
        public const string CHART_UNIQUEID_FAILURE = "The following error occured while generating unique chart id ";
        public const string CHART_CONFIGURE_FAILURE = "The following error occured while configuring charts";
        public const string CHART_GENWATERMARK_FAILURE = "The following error occured while generating watermark";
        public const string CHART_GENAXES_FAILURE = "The following error occured while formatting axes";
        public const string CHART_FORMATAXES_FAILURE = "The following error occured while formatting x axis";
        #endregion

        #region DefaultPage Messages
        public const string DEF_LOAD_FAILURE = "The page could not be loaded due to the following error";
        public const string DEF_LOADMODULES_FAILURE = "Loading modules failed due to the following error";
        public const string DEF_DISPLAYLEGENDS_FAILURE = "Could not display legends due to following error";
        public const string DEF_SAVEEXCEL_NODATA = "No data available for excel download";
        public const string DEF_SAVEEXCEL_STARTDATE = "Start Date cannot be empty";
        public const string DEF_SAVEEXCEL_NOSTATIONS = "No data to be exported to excel as no station has been selected";
        public const string DEF_PDF_FAIL = "The following error occured on the page. Unable to Generate PDF";
        #endregion

        #region AgriInfo Messages
        public const string AGRIINFO_LOADSERIES_FALURE = "The series could not be loaded due to the following error";
        public const string AGRIINFO_READSERIES_FALURE = "The series details could not be read from the config due to the following error: ";
        public const string AGRIINFO_HAPPLY_NOSETTINGS = "Please select settings to be applied to chart.";
        public const string AGRIINFO_HAPPLY_NOSTATION = "No station selected.";
        public const string AGRIINFO_HAPPLY_CHARTFAILURE = "Unable to generate the chart due to";
        public const string AGRIINFO_EXPORTDATA_FAILURE = "An error has occured while fetching the data for excel";
        public const string AGRIINFO_STARTDATE_OUTOFRANGE = "Start date out of range";
        public const string AGRIINFO_PLANTINGDATE_OUTOFRANGE = "Planting date out of range";
        public const string AGRIINFO_ADVOPTIONS = "The advanced options could not be displayed as the following error occured on the page: ";
        public const string AGRIINFO_ALT_NOT_NEGATIVE = "The altitude should be greater than zero.";
        public const string AGRIINFO_ALT_NUMBER = "Altitude can only be numeric.";
        /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
        public const string AGRIINFO_PLANTINGDATE_ENDDATE = "When GDD is selected,Planting date should be within the duration from start date";
        /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/

        #endregion

        #region Tables Messages
        public const string TAB_LOAD_FAILURE = "The tables could not be loaded due to the following error ";
        public const string TAB_DISPLAY_FAILURE = "The table could not be displayed due to the following error:";
        public const string TAB_CELLSMERGE_FAILURE = "The header cells could not be merged due to the following error";
        public const string TAB_RULESETLOAD_FAILURE = "The ruleset could not be obtained due to the following error";
        public const string TAB_ALIGNMENT_FAILURE = "The allignment could not be read due to the following error: ";
        public const string TAB_PALLETE_FAILURE = "The pallette could not be created due to the following error";
        public const string TAB_DATAFETCH_FAILURE = "The data could not be fetched  due to the following error";
        public const string TAB_GETCONTRAST_FAILURE = "The contrast color could not be obtained  due to the following error";

        //IM01173270 - New Agricast - Spray window - "Hour" not translatable - BEGIN
        public const string TAB_HOUR_COLUMN = "ResHoursOfDay";
        //IM01173270 - New Agricast - Spray window - "Hour" not translatable - END
        #endregion

        /*Wind Icon as a Sepearate component -- BEGIN*/
        #region Wind Icon Messages
        public const string WINDICONS_LOAD_FAILURE = "The Wind Icons could not be loaded due to the following error ";
        public const string WINDICONS_DISPLAY_FAILURE = "The Wind Icons could not be displayed due to the following error:";
        #endregion
        /*Wind Icon as a Sepearate component -- ENd*/
        #region Login Messages
        public const string LOGIN_INVALID_CREDENTIALS = "Invalid UserName/Passowrd.";
        public const string LOGIN_NOPUBACCESS = "You do not have access to this Publication";
        public const string LOGIN_GENERIC_ERROR = "An error occurred during login. Please try later.";
        public const string LOGIN_CLICK = "Click";
        public const string LOGIN_HERE = "here";
        public const string LOGIN_LOGIN_AGAIN = "to login again.";
        #endregion

        #region Mobile Messages
        public const string MOB_LOAD_FAILURE = "The page could not be loaded due to the following error";
        public const string MOB_SAVE_FAILURE = "An error occured on the page. Settings could not be saved.";
        public const string MOB_TRANS_ERROR = "Error occured during translation.";
        public const string MOB_LOAD_CHART_FAILURE = "An error occured while loading chart data.";
        public const string MOB_DISPLAYTABLE_ERROR = "An error occured while displaying tables.";
        public const string MOB_MODIFYDATA_ERROR = "An error occured while modifying data.";
        public const string MOB_DISPLAYLEGEND_ERROR = "An error occured while displaying legends.";
        public const string MOB_SEARCHLOCATION_ERROR = " The following error occured while searching for the location : ";
        public const string MOB_CREATEHTML_ERROR = " The following error occured while creating html. ";
        public const string MOB_GETCOUNTRY_ERROR = " The following error occured while fetching country. ";
        public const string MOB_GETLOCATION_ERROR = " The following error occured while fetching location details. ";
        public const string MOB_PREFILL_ERROR = " The following error occured while prefilling page. ";
        public const string MOB_LOADSTATION_ERROR = " The following error occured while loading stations. ";
        public const string MOB_LOADMODULE_ERROR = " The following error occured while loading modules. ";


        #endregion


        #region Webservice Messages
        public const string WS_GETSVCDATA_FAILURE = "An error occured while fetching data.";
        #endregion

        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
        public const string SOIL_INFORMATION = "Soil Information";
        public const string INVALID_DATE_RANGE = "No data found, invalid date range.";
        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

        /*IM01246233 :- New Agricast - missing translation tags - Begin */
        public const string CHART_ZOOM_TITLE = "Click_here_to_view_bigger_chart";//Click here to view bigger chart
        public const string EMAIL_ID_EMPTY_CHECK = "Please_enter_an_E-Mail_ID";//Please enter an E-Mail ID
        public const string FAV_NAME_EMPTY_CHECK = "Please_enter_a_valid_name";//'Please enter a valid name.'
        /*IM01246233 :- New Agricast - missing translation tags - End */

        /*IM01246263 - New Agricast - Add a new translation tag  - BEGIN*/
        public const string FEEDBACK_HEADER = "FeedBack_Header";
        /*IM01246263 - New Agricast - Add a new translation tag  - End*/

        /* July 23, Spritzwetter - Added by Jerrey - Begin */
        public const string OPTION_INSUFFICIENTLY = "txtOptionInsufficiently";
        public const string OPTION_DEFICIENT = "txtOptionDeficient";
        public const string OPTION_SUFFICIENTLY = "txtOptionSufficiently";
        public const string OPTION_SATISFACTORY = "txtOptionSatisfactory";
        public const string OPTION_GOOD = "txtOptionGood";
        public const string OPTION_VERY_WELL = "txtOptionVeryWell";
        public const string HINTS_MESSAGE = "txtHintsMessage";
        /* July 23, Spritzwetter - Added by Jerrey - End */

        /* Nov 12th, Added by Jerrey*/
        public const string ERROR_AGGREGATION_RANGE = "Aggregation should be smaller than duration.";
    }
}


