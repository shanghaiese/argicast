using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Reflection;
using System.Data;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;

namespace Syngenta.AgriCast.Common.Presenter
{
    public class Controls 
    {
        ServicePresenter objSvcPre = new ServicePresenter();
        public Control LoadCtrl(Page page, PlaceHolder Container, string Filename)
        {
            Control ctr;
            string fullName = "UserControls\\" + Filename + ".ascx";
            ctr = page.LoadControl(fullName);
            Container.Controls.Add(ctr);
            Container.ID = Filename + "1";            
            return ctr;
        }
     
        public void LoadCtrl<T>(Page page, PlaceHolder Container, string Filename, string name, string node) where T : UserControl
        {
            try
            {

                string fullName = "UserControls\\" + Filename + ".ascx";
                T ctr = (T)page.LoadControl(fullName);
                ctr.ID = name + "1";
                ctr.ClientIDMode = ClientIDMode.Static;
                PropertyInfo propertyInfo = ctr.GetType().GetProperty("Name");
                if(propertyInfo != null)
                propertyInfo.SetValue(ctr, Convert.ChangeType(name, propertyInfo.PropertyType), null);
                PropertyInfo propertyInfoNode = ctr.GetType().GetProperty("Node");
                if (propertyInfoNode != null)
                {
                    propertyInfoNode.SetValue(ctr, Convert.ChangeType(node, propertyInfoNode.PropertyType), null);
                }
                Container.Controls.Add(ctr);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The following error occured while loading controls: " + ex.Message.ToString();
            }
        }
        //overloaded method to handle charting in agriinfo
        public void LoadCtrl<T>(Page page, PlaceHolder Container, string Filename, string name, string node, DataTable Data) where T : UserControl
        {
            try
            {
                string fullName = "UserControls\\" + Filename + ".ascx";
                T ctr = (T)page.LoadControl(fullName);
                ctr.ID = name + "1";
                ctr.ClientIDMode = ClientIDMode.Static;
                PropertyInfo propertyInfo = ctr.GetType().GetProperty("Name");
                propertyInfo.SetValue(ctr, Convert.ChangeType(name, propertyInfo.PropertyType), null);
                PropertyInfo propertyInfoNode = ctr.GetType().GetProperty("Node");
                if (propertyInfoNode != null)
                {
                    propertyInfoNode.SetValue(ctr, Convert.ChangeType(node, propertyInfoNode.PropertyType), null);
                }
                PropertyInfo propertyInfoData = ctr.GetType().GetProperty("agriInfoData");
                propertyInfoData.SetValue(ctr, Convert.ChangeType(Data, propertyInfoData.PropertyType), null);
                Container.Controls.Add(ctr);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The following error occured while loading controls: " + ex.Message.ToString();
            }
        }
        //overloaded method for excel sheet generation for tables
        public void LoadCtrl<T>(Page page, PlaceHolder Container, string Filename, string name, List<string[]> list) where T : UserControl
        {
            try
            {
                string fullName = "UserControls\\" + Filename + ".ascx";
                T ctr = (T)page.LoadControl(fullName);
                ctr.ID = name + "1";
                ctr.ClientIDMode = ClientIDMode.Static;
                PropertyInfo propertyInfo = ctr.GetType().GetProperty("Name");
                propertyInfo.SetValue(ctr, Convert.ChangeType(name, propertyInfo.PropertyType), null);
                PropertyInfo propertyInfoList = ctr.GetType().GetProperty("ExcelList");
                propertyInfoList.SetValue(ctr, Convert.ChangeType(list, propertyInfoList.PropertyType), null);
                Container.Controls.Add(ctr);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The following error occured while loading controls: " + ex.Message.ToString();
            }
        }
        //overloaded method for excel sheet generation for agriinfo and charts
        public void LoadCtrl<T>(Page page, PlaceHolder Container, string Filename, string name, string node, Boolean ExcelFlag,string Selected) where T : UserControl
        {
            try
            {
                string fullName = "UserControls\\" + Filename + ".ascx";
                T ctr = (T)page.LoadControl(fullName);
                ctr.ID = name + "1";
                ctr.ClientIDMode = ClientIDMode.Static;
                PropertyInfo propertyInfo = ctr.GetType().GetProperty("Name");
                propertyInfo.SetValue(ctr, Convert.ChangeType(name, propertyInfo.PropertyType), null);
                PropertyInfo propertyInfoNode = ctr.GetType().GetProperty("Node");
                if (propertyInfoNode != null)
                {
                    propertyInfoNode.SetValue(ctr, Convert.ChangeType(node, propertyInfoNode.PropertyType), null);
                }
                PropertyInfo propertyInfoFlag = ctr.GetType().GetProperty("ExcelFlag");
                propertyInfoFlag.SetValue(ctr, Convert.ChangeType(ExcelFlag, propertyInfoFlag.PropertyType), null);
                PropertyInfo propertyInfoSelected = ctr.GetType().GetProperty("Selected");
                if (propertyInfoSelected != null)
                {
                    propertyInfoSelected.SetValue(ctr, Convert.ChangeType(Selected, propertyInfoSelected.PropertyType), null);
                }
                Container.Controls.Add(ctr);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = "The following error occured while loading controls: " + ex.Message.ToString();
            }
        }
        public  Control FindControlRecursive(Control ctrl, string id)
        {
            if (ctrl == null) return null;
            if (ctrl.ID == id) return ctrl;
            return FindControlRecursive(ctrl.Controls, id);
        }
        public  Control FindControlRecursive(ControlCollection collection, string id)
        {
            IEnumerator cenum = collection.GetEnumerator();
            while (cenum.MoveNext())
            {
                Control curctrl = (Control)cenum.Current;
                if (curctrl.ID == id) return curctrl;
                curctrl = FindControlRecursive(curctrl.Controls, id);
                if (curctrl != null) return curctrl;
            }
            return null;
        }

    }
}