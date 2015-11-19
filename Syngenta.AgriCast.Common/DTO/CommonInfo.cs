using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syngenta.AgriCast.Common.DTO
{
    public class CommonInfo
    {
        private int intModuleId;
        private string strCulture;
        private string strModel;
        private string strUnit;
        private int intUserId;
        private string strUserName;
        private string strService;
        private bool boolAuth;
    
        public int UserId
        {
            get
            {
                return intUserId;
            }
            set
            {
                intUserId = value;
            }
        }

        public string UserName
        {
            get
            {
                return strUserName;
            }
            set
            {
                strUserName = value;
            }
        }

        public string Culture
        {
            get
            {
                return strCulture;
            }
            set
            {
                strCulture = value;
            }
        }

        public string Unit
        {
            get
            {
                return strUnit;
            }
            set
            {
                strUnit = value;
            }
        }

        public int ModuleID
        {
            get
            {
                return intModuleId;
            }
            set
            {
                intModuleId = value;
            }
        }

        public string ServiceName
        {
            get
            {
                return strService;
            }
            set
            {
                strService = value;
            }
        }

        public string Model
        {
            get
            {
                return strModel;
            }
            set
            {
                strModel = value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return boolAuth;
            }
            set
            {
                boolAuth = value;
            }
        }
    }
}
