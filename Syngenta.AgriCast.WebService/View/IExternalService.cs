/* Agricast CR - 3.5	R5 - External user management changes. - Begin */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace Syngenta.AgriCast.WebService.View
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IExternalService" in both code and config file together.
    [ServiceContract]
    public interface IExternalService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/getSecurityKey?token={token}&userID={userID}", ResponseFormat = WebMessageFormat.Xml)]
        string GetSecurityKey(string token, string userID);

        [OperationContract]
        [WebGet(UriTemplate = "/Version", ResponseFormat = WebMessageFormat.Xml)]
        string Version();

        // method for testing decryption
        //[OperationContract]
        string DecryptSecurityKey(string encryptedKey);
    }
}

/* Agricast CR - 3.5	R5 - External user management changes. - End */
