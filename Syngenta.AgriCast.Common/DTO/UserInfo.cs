using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.DTO;
using System.Web.SessionState;
using System.Data;

namespace Syngenta.AgriCast.Common.DTO
{
    public class UserInfo
    {
        private int intUserId;
        private int strUserName;
        private int boolAuth;
        private UserInfo objUserInfo;

        public int UserId
        {
            get;

            set;

        }

        public string UserName
        {
            get;

            set;
        }

        public bool IsAuthenticated
        {
            get;

            set;
        }


        public string PrePubName
        {
            get;

            set;
        }
        public UserInfo getUserInfoObject
        {
            get
            {
                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objUserInfo"] == null)
                    {
                        Session["objUserInfo"] = new UserInfo();
                    }
                    return (UserInfo)Session["objUserInfo"];
                }
                else
                {
                    if (objUserInfo == null) objUserInfo = new UserInfo();
                    return objUserInfo;
                }
            }
            set
            {
                HttpSessionState Session = null;
                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objUserInfo"] = value;
                }
                else
                {
                    objUserInfo = value;
                }
            }
        }

        public DataTable DtFavorites
        {
            get;
            set;
        }
    }
}
