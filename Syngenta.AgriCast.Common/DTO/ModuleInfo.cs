using System;
using System.Web; 
using System.ComponentModel; 
using Syngenta.AgriCast.Common.Service;
using System.Web.SessionState;

namespace Syngenta.AgriCast.Common
{
    public class ModuleInfo
    {
        private int intModuleId;
        private int strModel;
        private ModuleInfo objModule;
        public int Model
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int ModuleId
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ModuleInfo getModuleInfoObject
        {
            get
            {
                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objModuleInfo"] == null)
                    {
                        Session["objModuleInfo"] = new ModuleInfo();
                    }
                    return (ModuleInfo)Session["objModuleInfo"];
                }
                else
                {
                    if (objModule == null) objModule = new ModuleInfo();
                    return objModule;
                }
            }
            set
            {
                HttpSessionState Session = null;
                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objModuleInfo"] = value;
                }
                else
                {
                    objModule = value;
                }
            }
        }
    }
}
