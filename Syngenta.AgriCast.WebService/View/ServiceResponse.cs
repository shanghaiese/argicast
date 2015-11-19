using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Syngenta.AgriCast.WebService.View
{
    [DataContract]
    public enum EStatusCode
    {
        [EnumMember]
        Success = 1,

        [EnumMember]
        Failed = 2
    }

    [DataContract]
    public class ServiceResponse
    {
        [DataMember]
        public EStatusCode StatusCode { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}