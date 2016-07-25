using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageToText
{
    class OCRmain
    {
        public static void Execute(string[] args)
        {
            int argc = args.Length;

            if (argc == 3)
            {
                OCR ocr = new OCR();
                ocr.mpStartOCR(args[1]);
                URLtoText.ParserMain.ParseOthers("tempOCRresults.txt", args[2]);
                DossierParser.Utilities.mpDeleteTemporalFile("tempOCRresults.txt");

            }
            else
            {
                Console.WriteLine("Usage: dossier image inImagePath outFilePath");
                Console.WriteLine(string.Format("{0,-10} {1}", null, "* inImagePath: Image file to apply OCR on"));
                Console.WriteLine(string.Format("{0,-10} {1}", null, "* outFilePath: Path where to output results"));
            }
        }
    }
}
  