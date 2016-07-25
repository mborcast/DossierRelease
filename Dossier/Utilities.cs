using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DossierParser
{
    class Utilities
    {
        /// <summary>
        /// Elimina un archivo.
        /// </summary>
        /// <param name="pTempPath">Ruta del archivo a eliminar</param>
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

        /// <summary>
        /// Imprime una línea en la consola con un color determinado.
        /// </summary>
        /// <param name="pColor">Color del mensaje.</param>
        /// <param name="pMessage">Mensaje a mostrar.</param>
        /// <param name="lineBreak">Parámetro opcional. Imprimir y cambiar de línea?</param>
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
