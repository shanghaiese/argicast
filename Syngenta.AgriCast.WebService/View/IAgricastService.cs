using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Data;
using System.Xml;
using Syngenta.AgriCast.WebService.View;
using System.Runtime.Serialization;


namespace Syngenta.AgriCast.WebService.View
{
    [ServiceContract]//(SessionMode = SessionMode.Required)]
    public interface IAgricastService
    { 
        [OperationContract]
        //[ServiceKnownType(typeof(Chart))]
        //[ServiceKnownType(typeof(Icons))]
        //[ServiceKnownType(typeof(Response))]
        [WebGet(UriTemplate = "/getAgricastServiceData?strToken={strToken}&xmlFeatureRequest={xmlFeatureRequest}&strServiceID={strServiceID}&strModuleIDs={strModuleIDs}&dStartDate={dStartDate}&dEndDate={dEndDate}&strCultureCode={strCultureCode}&strUnit={strUnit}", ResponseFormat = WebMessageFormat.Xml)]
        //ServiceOutput getAgricastServiceData(string strToken, string xmlFeatureRequest, string strServiceID, string strModuleIDs, DateTime dStartDate, DateTime dEndDate, string strCultureCode);

        /*Unit Implementation in Web Services - Begin*/
        ServiceOutput getAgricastServiceData(string strToken, string xmlFeatureRequest, string strServiceID, string strModuleIDs, string dStartDate, string dEndDate, string strCultureCode, string strUnit);
        /*Unit Implementation in Web Services - End*/
        [OperationContract]
        [WebGet(UriTemplate = "/Version", ResponseFormat = WebMessageFormat.Xml)] 
        string Version(); 

         
        int iTimeZoneOffset 
        {
            get;
            set;
        }

        int iDstOn
        {
            get; 
            set;
        }
    }
}
