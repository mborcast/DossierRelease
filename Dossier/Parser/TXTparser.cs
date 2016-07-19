using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    class TXTparser : IParsing
    {
        /// <summary>
        /// Returns data read from given file.
        /// </summary>
        /// <param name="pPath">Path to text file</param>
        /// <returns></returns>
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
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. NO TXT FILE FOUND");
                return null;
            }

            List<string> lResult = new List<string>();

            foreach (string lWord in lFileWords)
            {
                lResult.Add(lWord.ToString());
            }

            Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
            return lResult;
        }
    }
}
