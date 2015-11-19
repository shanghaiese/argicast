using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.DataAccess;

namespace Syngenta.AgriCast.AgriInfo.DataAccess
{
    public class AgriInfoDB
    {
        TranslateData objTranslate = new TranslateData();
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            string strText = objTranslate.getTranslatedText(strLabelName, strCultureCode);
            return strText;
        }
    }
}