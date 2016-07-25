using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patagames.Ocr;
using System.IO;

namespace ImageToText
{
    class OCR
    {
        OcrApi        aHandler;
        StringBuilder aRecognizedText;

        /// <summary>
        /// Constructor. Inicializa la librería OCR.
        /// </summary>
        public OCR()
        {
            aHandler        = OcrApi.Create();
            aRecognizedText = new StringBuilder();
            aHandler.Init();
        }

        /// <summary>
        /// Aplica el reconocimiento de texto en una imagen.
        /// </summary>
        /// <param name="pImagePath">Ruta de la imagen a analizar.</param>
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
                DossierParser.Utilities.mpDeleteTemporalFile(lPathToTemporal);
            }

        }
    }
}
