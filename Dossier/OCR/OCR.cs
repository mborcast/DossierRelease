using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patagames.Ocr;
using System.IO;

namespace OCR
{
    class OCR
    {
        OcrApi        aHandler;
        StringBuilder aRecognizedText;

        public OCR()
        {
            aHandler        = OcrApi.Create();
            aRecognizedText = new StringBuilder();
            aHandler.Init();
        }

        /// <summary>
        /// Starts OCR on given image.
        /// </summary>
        /// <param name="pImagePath">Image to evaluate.</param>
        public void mpStartOCR(string pImagePath)
        {
            string lPathToTemporal = ImageProcessing.mfProcessAndSaveAsNewImage(pImagePath);

            if (lPathToTemporal != null)
            {
                Console.Write("Recognizing text" );
                try
                {
                    aRecognizedText.Append(aHandler.GetTextFromImage(lPathToTemporal));
                }
                catch (Patagames.Ocr.Exceptions.NoLicenseException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(" >>>> FAILED. IMAGE FILES ONLY");
                    Console.WriteLine(e.Message);
                }

                
                try
                {
                    if (aRecognizedText.Length > 0)
                    {
                        File.WriteAllText("tempOCRresults.txt", this.aRecognizedText.ToString(), Encoding.UTF8);
                    }
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine(" >>>> FAILED. NO DATA");
                }
                Dossier.Utilities.mpDeleteTemporalFile(lPathToTemporal);
            }

        }
    }
}
