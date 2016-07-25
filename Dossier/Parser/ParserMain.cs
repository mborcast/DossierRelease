using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLtoText
{
    class ParserMain
    {
        public static void Execute(string[] args)
        {
            Parser parser;
            int argc = args.Length;

            if (argc == 3)
            {
                parser = new Parser(eDictionaries.ESP | eDictionaries.ENG);
                parser.mpSetParsingType(eParseType.HTML);

                parser.mpParse(args[1], args[2]);
                return;
            }
            else if (argc == 4)
            {
                if (args[1] == "-b")
                {
                    parser = new Parser(eDictionaries.ESP | eDictionaries.ENG);
                    parser.mpBatch(args[2], args[3]);
                    return;
                }
            }

            Console.WriteLine("Usage: dossier url URLpath");
            Console.WriteLine(string.Format("{0,-10} {1}", null, "* URLpath: URL to get data from"));
            Console.WriteLine(string.Format("{0,-6} {1}", null, "dossier url -b pathToURLfile"));
            Console.WriteLine(string.Format("{0,-10} {1}", null, "* pathToURLfile: Text file with all the URLs to batch"));
        }

        /// <summary>
        /// Método empleado por el ATT y OCR para parsear sus resultados.
        /// </summary>
        /// <param name="pPath">Ruta del archivo temporal de ATT u OCR.</param>
        /// <param name="pOutFilePath">Ruta del archivo de salida.</param>
        public static void ParseOthers(string pPath, string pOutFilePath)
        {
            Parser parser = new Parser(eDictionaries.ESP | eDictionaries.ENG);
            parser.mpSetParsingType(eParseType.TXT);

            parser.mpParse(pPath, pOutFilePath);
        }

    }
}

