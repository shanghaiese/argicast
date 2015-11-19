using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syngenta.AgriCast.Common.DataAccess;

namespace Syngenta.AgriCast.Charting.Service
{
    public class TableData
    {
        TranslateData objTranslate = new TranslateData();
        //Method to fetch the translated text by passing lablename/id and the culture code
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            string strText = objTranslate.getTranslatedText(strLabelName, strCultureCode);
            return strText;
        }
    }
}
