using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Syngenta.AgriCast.Common.View
{
    public interface IToolbar
    {
        string selLang
        {
            get;
             
        }

        string selUnit
        {
            get;
            set;
        }

        string setUserLink
        {
            get;
            set;
        }

        string strCulCode
        {
            get;
            set;
        }

        bool showEmail
        {
            get;
            set;
        }

        bool showExportExcel
        {
            get;
            set;
        }

        bool showFavorites
        {
            get;
            set;
        }

        bool showFeedback
        {
            get;
            set;
        }

        bool showCulture
        {
            get;
            set;
        }
        bool IsFavAdded
        {
            get;
            set;
        }
        bool IsFavUpdated
        {
            get;
            set;
        }
        bool showPrint
        {
            get;
            set;
        }

        bool showSavedata
        {
            get;
            set;
        }

        bool showUnits
        {
            get;
            set;
        }

        DataTable dtCultureCode
        {
            get;
            set;
        }

        DataTable dtUnits
        {
            get;
            set;
        }

        DataTable dtFavorites
        {
            get;
            set;
        }
        DataTable dtPlaceDetails
        {
            get;
            set;
        }
        ArrayList alEmailSettings
        {
            get;
            set;
        }

        string strText
        {
            get;
            set;
        }
         int captchaLength
        {
            get;
            set;
        }
         string Favorite
         {
             get;
             set;

         }
         double fontSize
         {
             get;
             set;
         }
        string fontFamily
         {
             get;
             set;
         }
        string backgroundImagePath
        {
            get;
            set;
        }
        string textColor
        {
            get;
            set;
        }
         string successMessage
        {
            get;
            set;
        }
         string errorMessage
         {
             get;
             set;
         }
         string characterSet
         {
             get;
             set;
         }
        string feedbackName
         {
             get;
             set;
         }
        string feedbackEmail
        {
            get;
            set;
        }
        string feedbackMessage
        {
            get;
            set;
        }

        string strRandomText
        {
            get;
            set;
        }

        string DefaultUnitSettings
        {
            get;
            set;
        }
        string strCustomUnits
        {
            get;
            set;
        }
        List<string[]> alNodeList
        {
            get;
            set;
        }
    }
}
