using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace URLtoText
{
    class TXTparser : IParsing
    {
        /// <summary>
        /// Carga la información de un archivo de texto en una lista.
        /// </summary>
        /// <param name="pPath">Ruta al archivo de texto a analizar.</param>
        /// <returns>Una lista con las líneas de texto contenidas en el archivo</returns>
        public List<string> GetDataToParse(string pPath)
        {
            Console.Write("Reading file: " + pPath);

            char[]   lSeparator = { ' ', '\n' };
            string[] lFileWords;

            try
            {
                lFileWords = File.ReadAllText(pPath, Encoding.UTF8).Split(lSeparator);
            }
            catch (FileNotFoundException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. NO TXT FILE FOUND");
                return null;
            }

            List<string> lResult = new List<string>();

            foreach (string lWord in lFileWords)
            {
                lResult.Add(lWord.ToString());
            }

            DossierParser.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
            return lResult;
        }
    }
}
