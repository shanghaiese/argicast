using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Configuration;
using Syngenta.AgriCast.Common;

public partial class Print : System.Web.UI.Page
{
    StreamReader sr = null;
    StringBuilder sb = null;
    StringWriter sw = null;
    string strHtmlfilePath = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        string strTempDirPath = string.Empty;

        // Create the Image converter. Optionally you can specify the virtual browser 
        // width as parameter. 1024 pixels is default, 0 means autodetect
        ExpertPdf.HtmlToPdf.ImgConverter imgConverter = new ExpertPdf.HtmlToPdf.ImgConverter();

        if (ConfigurationManager.AppSettings.AllKeys.Contains("UserName") && ConfigurationManager.AppSettings.AllKeys.Contains("Password"))
        {
            //use the Account details which has fuill access on the site(all folders)             
            string Username = ConfigurationManager.AppSettings["Domain"].ToString() + "\\" + ConfigurationManager.AppSettings["UserName"].ToString();
            string Password = ConfigurationManager.AppSettings["Password"].ToString();

            //add the authentication options to PDF Converter
            imgConverter.AuthenticationUsername = Username;
            imgConverter.AuthenticationPassword = Password;            
        }
       
        // set the license key
        imgConverter.LicenseKey = ConfigurationManager.AppSettings["pdfLicence"]; ;
        // Performs the conversion and get the image bytes that you can further 
        // save to a file or send as a browser response
        // imgConverter.PageWidth = Convert.ToInt32(Request.Params["Width"]);

        imgConverter.PageWidth = 1000;
        //imgConverter.PageHeight=800
        //Read the HTML from its location path
        strHtmlfilePath = Session["htmlpagepath"].ToString();

        if (strHtmlfilePath != null)
            sr = new StreamReader(strHtmlfilePath);
         
        sb = new StringBuilder();
        sw = new StringWriter(sb); 
        sw.Write(sr.ReadToEnd());

        imgConverter.ConversionDelay = 2;
        byte[] imgBytes = imgConverter.GetImageBytesFromHtmlString(sb.ToString(), System.Drawing.Imaging.ImageFormat.Jpeg);
        // send the image as a response to the browser for download

        string fileName = RandomString();
        fileName = "Print" + "_" + fileName + ".jpeg";

        //Get the Location path  excluding html file name 
        strTempDirPath = strHtmlfilePath.Substring(0, strHtmlfilePath.LastIndexOf('/'));

        //Added by Rajesh for Mais Expert CR - Start
        if (!Directory.Exists((strTempDirPath)))
            Directory.CreateDirectory(strTempDirPath);
        //Added by Rajesh for Mais Expert CR - End
        File.WriteAllBytes((strTempDirPath +"\\" + fileName), imgBytes);

        //Set the Relative Path : Temp\mailpdf
        imgPrint.Src = "~/" + ConfigurationManager.AppSettings["tempfolder"] + "/" + Constants.EMAILPDFFOLDER + "/" + fileName;



        
        btnClose.Attributes.Add("onclick", "javascript:fnCloseWindow();");

        
        btnPrint.Attributes.Add("onclick", "javascript:fnPrintWindow();");




    }

    private string RandomString()
    {
        StringBuilder builder = new StringBuilder();
        Random random = new Random();
        char ch;
        for (int i = 0; i < 8; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }
        return builder.ToString();
    }
}