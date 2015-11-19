using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syngenta.AgriCast.AgriInfo.View
{
    public interface IPopup
    {
        List<string[]> al
        {
            get;
            set;
        }
        string strTransText
        {
            get;
            set;
        }
    }
}