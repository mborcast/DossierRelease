using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;

namespace ImageToText
{
    class ImageProcessing
    {
        /// <summary>
        /// Escala una imagen para evitar restricciones de licencia de la librería OCR.
        /// </summary>
        /// <param name="pImage">Ruta de la imagen a analizar.</param>
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
        /// Aplica un conjunto de transformaciones para hacer más nítida una imagen. La imagen resultante es guardada como una copia en un archivo temporal.
        /// </summary>
        /// <param name="pImagePath">Ruta de la imagen a analizar.</param>
        /// <returns>Devuelve la ruta del archivo temporal generado.</returns>
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
