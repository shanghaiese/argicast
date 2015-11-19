using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Specialized;
using System.Collections;

namespace Syngenta.AgriCast.Charting.Service
{
    public class ImageUtil
    {
        public enum ColorReduction : byte
        {
            NEUQUANT,
            OCTREE
        };
        private const ColorReduction defaultColorReduction = ColorReduction.OCTREE;
        public static void SaveGIFWithNewColorTable(
            Bitmap image,
            string filename,
            bool fTransparent)
        {
            SaveGIFWithNewColorTable(image, filename, fTransparent, defaultColorReduction);
        }
        public static Bitmap GenerateGIFWithNewColorTable(
            Bitmap image, bool fTransparent)
        {
            return GenerateGIFWithNewColorTable(image, fTransparent, defaultColorReduction);
        }
        public static void SaveGIFWithNewColorTable(
            Bitmap image,
            string filename,
            bool fTransparent,
            ColorReduction colorReductionAlgorithm
            )
        {
            Bitmap bitmap = GenerateGIFWithNewColorTable(image, fTransparent, colorReductionAlgorithm);
            bitmap.Save(filename, ImageFormat.Gif);
            // Bitmap goes out of scope here and is also marked for
            // garbage collection.
            // Pal is referenced by bitmap and goes away.
            bitmap.Dispose();
        }
        /// <summary>
        /// Color quantization overview:
        /// http://algolist.manual.ru/graphics/quant/qoverview.php
        /// MSDN Article on changing palettes on GIFs
        /// http://support.microsoft.com/default.aspx?scid=kb;en-us;Q319061
        /// Median Cut Algorithm (simple, but slow)
        /// http://rsb.info.nih.gov/ij/developer/source/ij/process/MedianCut.java.html (Java)
        /// NeuQuant Algorithm (best quality, but slow)
        /// http://www.acm.org/~dekker/NEUQUANT.HTML (Java)
        /// Octree Quantization (fast)
        /// http://www.microsoft.com/msj/archive/S3F1.aspx (Documentation)
        /// http://www.microsoft.com/downloads/details.aspx?FamilyID=cb9a0bc8-c96b-4c3e-9652-df609352fa89&DisplayLang=en (Download)
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fTransparent"></param>
        /// <returns></returns>
        public static Bitmap GenerateGIFWithNewColorTable(
            Bitmap image,
            bool fTransparent, ColorReduction colorReductionAlgorithm)
        {
            // Make a new 8-BPP indexed bitmap that is the same size as the source image.
            int nColors = 256;
            int Width = image.Width;
            int Height = image.Height;
            // Always use PixelFormat8bppIndexed because that is the color
            // table-based interface to the GIF codec.
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            // Initialize a new color table with entries that are determined
            IDictionary colorOccurences = new Hashtable();
            IDictionary colorReducedOccurences = new Hashtable();
            IDictionaryEnumerator cenum;
            Color[] cpixels = new Color[Width * Height];
            int ix = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color c = image.GetPixel(x, y);
                    cpixels[ix++] = c;
                    if (colorOccurences.Contains(c))
                    {
                        int v = (int)colorOccurences[c];
                        colorOccurences[c] = v++;
                    }
                    else colorOccurences[c] = 1;
                }
            }
            IDictionary colorIndexMap = new Hashtable();
            ColorPalette pal;
            if (colorOccurences.Count <= 256)
            {
                nColors = colorOccurences.Count;
                pal = GetColorPalette(nColors);
                ix = 0;
                cenum = colorOccurences.GetEnumerator();
                while (cenum.MoveNext())
                {
                    Color c = (Color)cenum.Key;
                    //ix = colorOccurences
                    colorIndexMap.Add(c, ix);
                    pal.Entries[ix++] = c;
                }
            }
            else
            {
                // color reduction
                DateTime sDate = DateTime.Now;
                ColorQuantizer cq;
                switch (colorReductionAlgorithm)
                {
                    default: cq = new OctreeQuantizer(255, cpixels); break;
                    case ColorReduction.NEUQUANT: cq = new NeuQuant(30, cpixels); break;
                }
                cq.init();
                cenum = colorOccurences.GetEnumerator();
                while (cenum.MoveNext())
                {
                    colorIndexMap.Add(cenum.Key, cq.lookup((Color)(cenum.Key)));
                }
                nColors = cq.getColorCount();
                // GIF codec supports 256 colors maximum, monochrome minimum.
                // Create a color palette big enough to hold the colors you want.
                pal = GetColorPalette(nColors);
                for (int x = 0; x < nColors; x++) pal.Entries[x] = cq.getColor(x);
                //Common.logDebug("time for color reduction (" + colorOccurences.Count + " to 256) needed = " + ((TimeSpan)(DateTime.Now - sDate)).TotalMilliseconds + " ms");
            }
            // Set the palette into the new Bitmap object.
            bitmap.Palette = pal;
            // Lock a rectangular portion of the bitmap for writing.
            BitmapData bitmapData;
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            // Write to the temporary buffer that is provided by LockBits.
            // Copy the pixels from the source image in this loop.
            // Because you want an index, convert RGB to the appropriate
            // palette index here.
            IntPtr pixels = bitmapData.Scan0;
            //unsafe
            //{
            //    // Get the pointer to the image bits. This is the unsafe operation.
            //    byte* pBits;
            //    if (bitmapData.Stride > 0) pBits = (byte*)pixels.ToPointer();
            //    else
            //        // If the Stide is negative, Scan0 points to the last
            //        // scanline in the buffer. To normalize the loop, obtain
            //        // a pointer to the front of the buffer that is located
            //        // (Height-1) scanlines previous.
            //        pBits = (byte*)pixels.ToPointer() + bitmapData.Stride * (Height - 1);
            //    uint stride = (uint)Math.Abs(bitmapData.Stride);

            //    for (uint row = 0; row < Height; ++row)
            //    {
            //        for (uint col = 0; col < Width; ++col)
            //        {
            //            // The destination pixel.
            //            // The pointer to the color index byte of the destination; this real pointer causes this
            //            // code to be considered unsafe.
            //            byte* p8bppPixel = pBits + row * stride + col;
            //            Color pixel = cpixels[Width * row + col];
            //            // determine index of that color
            //            object index = colorIndexMap[pixel];
            //            if (index != null)
            //            {
            //                *p8bppPixel = (byte)(int)index;
            //            }
            //            else
            //            {
            //                *p8bppPixel = 0;
            //            }
            //        } /* end loop for col */
            //    } /* end loop for row */
            //} /* end unsafe */
            // To commit the changes, unlock the portion of the bitmap.
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        static private ColorPalette GetColorPalette(int nColors)
        {
            // Assume monochrome image.
            PixelFormat bitscolordepth = PixelFormat.Format1bppIndexed;
            ColorPalette palette;    // The Palette we are stealing
            Bitmap bitmap;     // The source of the stolen palette
            // Determine number of colors.
            if (nColors < 2) nColors = 2;
            if (nColors > 2) bitscolordepth = PixelFormat.Format4bppIndexed;
            if (nColors > 16) bitscolordepth = PixelFormat.Format8bppIndexed;
            // Make a new Bitmap object to get its Palette.
            bitmap = new Bitmap(1, 1, bitscolordepth);
            palette = bitmap.Palette;   // Grab the palette
            bitmap.Dispose();           // cleanup the source Bitmap
            return palette;             // Send the palette back
        }

    }
    public abstract class ColorQuantizer
    {
        public abstract void init();
        public abstract Color getColor(int ix);
        public abstract int getColorCount();
        public abstract int lookup(Color pixel);

    }
}