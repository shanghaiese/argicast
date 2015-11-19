using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Captcha : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string text = string.Empty;
        if (this.Session["captcha"] != null)
            text = (string)this.Session["captcha"];

        int imageWidth = Convert.ToInt32(((16 + 8) * text.Length));
        int imageHeight = Convert.ToInt32(16 * 2.5);


        Bitmap bmp = new Bitmap(imageWidth, imageHeight);
        var Grph = Graphics.FromImage(bmp);
        Grph.FillRectangle(new SolidBrush(Color.Lavender), 0, 0, bmp.Width, bmp.Height);

        var grp = Graphics.FromImage(bmp);
        System.Drawing.Image background = System.Drawing.Image.FromFile(HttpRuntime.AppDomainAppPath + @"\Images\captcha2.png");
        grp.DrawImage(background, new Rectangle(0, 0, bmp.Width, bmp.Height));


        Grph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        Grph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        int xPos = 10;

        Font f = new System.Drawing.Font("Arial", 16);

        char[] textArray = text.ToCharArray();
        int yPosition = 0;
        Random r = new Random();

        for (int i = 0; i < textArray.Length; i++)
        {
            if (i % 2 == 0)
                Grph.RotateTransform(5);
            else
                Grph.RotateTransform(-5);

            yPosition = (int)(r.NextDouble() * 10);

            Grph.DrawString(textArray[i].ToString(), f, new SolidBrush(Color.FromName("Gray")), xPos, yPosition);
            xPos += 20;

        }

        using (MemoryStream objMS = new MemoryStream())
        {
            bmp.Save(objMS, ImageFormat.Jpeg);

            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/Jpeg";
            HttpContext.Current.Response.BinaryWrite(objMS.ToArray());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
    }
}