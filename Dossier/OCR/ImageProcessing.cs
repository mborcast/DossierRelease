using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;

namespace OCR
{
    class ImageProcessing
    {
        /// <summary>
        /// Resizes image if its current dimensions create license conflicts
        /// </summary>
        /// <param name="pImage">Image to resize</param>
        static void mpResizeToAvoidLicenseException(ref MagickImage pImage)
        {
            int lImgWidth = pImage.Width;
            int lImgHeight = pImage.Height;

            int lDiff = lImgWidth % 100;

            if ((lDiff >= 50) || (lDiff == 0))
            {
                int lNewWidth = lImgWidth + 25 - lDiff;
                int lNewHeight = lNewWidth * lImgHeight / lImgWidth;

                pImage.Resize(lNewWidth, lNewHeight);
            }

        }

        /// <summary>
        /// Apply transformations to image and save as a temporal file.
        /// </summary>
        /// <param name="pImagePath">Path to image to be transformed</param>
        /// <returns></returns>
        public static string mfProcessAndSaveAsNewImage(string pImagePath)
        {
            MagickImage lImage;
            Console.WriteLine("Processing image from: " + pImagePath);

            try
            {
                lImage = new MagickImage(pImagePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(" >>>> FAILED. IMAGE FILE NOT FOUND");
                return null;
            }

            //convert infile -contrast-stretch 10.5x80% outfile
            Console.WriteLine(" + Contrast stretch");
            lImage.ContrastStretch(new Percentage(20), new Percentage(60));

            //convert infile -sharpen 1 outfile
            Console.WriteLine(" + Sharpen");
            lImage.Sharpen(1, 0);

            //convert infile -resize 750% outfield
            Console.WriteLine(" + Resize 750%");
            lImage.Resize(new Percentage(750));

            //convert infile -resize 6110x8642 outfile
            Console.WriteLine(" + No license resize");
            mpResizeToAvoidLicenseException(ref lImage);

            string lNewPath = "temporal." + lImage.Format.ToString().ToLower();
            lImage.Write(lNewPath);

            return lNewPath;
        }
    }
}
