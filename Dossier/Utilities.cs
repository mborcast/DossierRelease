using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dossier
{
    class Utilities
    {
        public static void mpDeleteTemporalFile(string pTempPath)
        {
            try
            {
                File.Delete(pTempPath);
            }
            catch (IOException)
            {
                return;
            }
        }

        public static void mpPrint(ConsoleColor pColor, string pMessage, bool lineBreak = true)
        {
            Console.ForegroundColor = pColor;
            if (lineBreak)
                Console.WriteLine(pMessage);
            else
                Console.Write(pMessage);

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
