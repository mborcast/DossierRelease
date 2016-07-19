using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class ParserMain
    {
        public static void Execute(string[] args)
        {
            int argc = args.Length;

            if (argc == 3)
            {
                //Parser parser = new Parser(eDictionaries.ESP | eDictionaries.ENG);
                Parser parser = new Parser(eDictionaries.ENG);
                parser.mpSetParsingType(eParseType.HTML);

                if (args[1] == "-b")
                {
                    parser.mpBatch(args[2]);
                }
                else
                {
                    parser.mpParse(args[1], args[2]);
                }
            }
            else
            {
                Console.WriteLine("Usage: dossier url URLpath");
                Console.WriteLine(string.Format("{0,-10} {1}", null, "* URLpath: URL to get data from"));
                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossier url -b pathToURLfile"));
                Console.WriteLine(string.Format("{0,-10} {1}", null, "* pathToURLfile: Text file with all the URLs to batch"));
            }
             // dossier image path/to/image outPath
        }

        public static void ParseOthers(string pPath, string pOutFilePath)
        {
            Parser parser = new Parser(eDictionaries.ESP | eDictionaries.ENG);
            parser.mpSetParsingType(eParseType.TXT);

            parser.mpParse(pPath, pOutFilePath);
        }

    }
}

