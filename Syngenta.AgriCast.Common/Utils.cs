/* Agricast CR - 3.5	R5 - External user management changes. - Begin */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Configuration;

namespace Syngenta.AgriCast.Common
{
    public sealed class Utils
    {
        private const int ICON_WIDTH = 30;
        private const int ICON_HEIGHT = 30;
        private const int MAX_STRENGTH = 5;

        public static string CreateArrow(float angles, float windSpeed)
        {
            var strength = 1f;
            var length = 6f;

            if (windSpeed > 1)
            {
                if (windSpeed <= 5)
                {
                    length = 6 + windSpeed - 1;
                }

                else if (windSpeed <= 15)
                {
                    length = 8 + (windSpeed - 5) / 2;
                    strength = 2f;
                }

                else if (windSpeed <= 40)
                {
                    length = 12 + (windSpeed - 15) / 5;
                    strength = 3f;
                }

                else if (windSpeed <= 85)
                {
                    strength = 4;
                    length = 17 + (windSpeed - 40) / 9;
                }

                else
                {
                    strength = 4;
                    length = 22 + (windSpeed - 85) / 15;
                }

            }

            return CreateArrow(angles, strength, (float)Math.Round(length, 2));
        }

        public static string CreateArrow(float angles, float strength, float length)
        {
            if (strength == 0)
                return string.Empty;

            var rootPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["iconsPath"]);

            angles = (float)Math.Round(angles, 0); // reduce number of images
            while (angles > 360) angles -= 360;
            while (angles < 0) angles += 360;

            strength = (float)Math.Round(strength, 0);
            if (strength > MAX_STRENGTH)
                strength = MAX_STRENGTH;

            if (length > Math.Min(ICON_WIDTH, ICON_HEIGHT))
                length = Math.Min(ICON_WIDTH, ICON_HEIGHT);

            string fileId = ICON_WIDTH + "x" + ICON_HEIGHT + "_bft" + strength + "-" + length + "_" + ((int)angles).ToString("d3");
            string fileName = fileId + ".png";
            string fullFilePath = Path.Combine(rootPath, fileName);

            // do only generate the image if it does not exist.
            if (System.IO.File.Exists(fullFilePath))
                return fileName;

            using (Bitmap bmap = new Bitmap(ICON_WIDTH, ICON_HEIGHT))
            using (Graphics objGraphics = Graphics.FromImage(bmap))
            {
                // Then to translate, appending to world transform.
                int penWidth = (int)strength;
                var radius = length / 2;
                double rx = ICON_WIDTH / 2.0;
                double ry = ICON_HEIGHT / 2.0;

                double coeff = Math.PI * (angles / 180.0);
                double dx = rx + radius * Math.Cos(coeff);
                double dy = ry - radius * Math.Sin(coeff);
                double dx1 = rx - radius * Math.Cos(coeff);
                double dy1 = ry + radius * Math.Sin(coeff);

                // objGraphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), 1,1, width-2,height-2); // DEBUG
                objGraphics.SmoothingMode = SmoothingMode.HighQuality;

                // Draw rotated, translated arrow to screen.
                Pen pen = new Pen(Color.Black, penWidth);
                AdjustableArrowCap arrow = new AdjustableArrowCap(3, 3, true);
                pen.CustomStartCap = arrow;
                pen.EndCap = LineCap.Round;
                

                objGraphics.DrawLine(pen, (float)dx1, (float)dy1, (float)dx, (float)dy);

                try
                {
                    bmap.Save(fullFilePath, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem while saving the file '"
                        + fullFilePath + "'. Please check if the directory exists and the ASPNET user has enough privileges. "
                        + ex.Message, ex);
                }

                return fileName;
            }
        }

        public static void AddCookie(string name, string value)
        {
            if (HttpContext.Current.Request.Cookies[name] == null)
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(name, value));
            else
                HttpContext.Current.Request.Cookies[name].Value = value;

            HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(1);
        }
    }
}

/* Agricast CR - 3.5	R5 - External user management changes. - End */
