using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syngenta.AgriCast.Common.View
{
    public interface IShowImage
    {
        List<string[]> alNodeList
        {
            get;
            set;
        }
    }
}