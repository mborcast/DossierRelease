using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR;
using ATT;
using Parser;

namespace Dossier
{
    class Program
    {
        static void Main(string[] args)
        {/*
             * dossier audio path/to/audio outpath=> 3
             *           0        1          2
             *           
             * dossier audio -f f01 f02 path/to/audio outpath => 6
             *            0   1  2   3        4           5
             * 
             * dossier video path/to/video outpath => 3
             *           0        1          2
             *           
             * dossier video -f f01 f02 path/to/video outpath => 6
             *            0   1  2   3        4           5
             * 
             * dossier image path/to/image outPath
             * 
             * dossier url www.foobar.com outpath => 3
             * 
             * dossier url -b path/to/urlsfile => 3
             */

            int argc = args.Length;

            if (argc == 3 || argc == 6)
            {
                switch (args[0])
                {
                case "audio":
                case "video":
                    ATTmain.Execute(args);
                    break;
                case "image":
                    OCRmain.Execute(args);
                    break;
                case "url":
                    ParserMain.Execute(args);
                    break;
                default:
                    break;
                }
            }
            else
            {
                Console.WriteLine("Usage: dossier url URLpath outFilePath");
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* URLpath: URL to get data from"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossier url -b pathToURLfile"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* pathToURLfile: Text file with all the URLs to parse batch\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossier image inImagePath outFilePath"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* inImagePath: Image file to apply OCR on"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossier [audio | video] inAudioPath outFilePath"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* inAudioPath: Audio or video file in any format to recognize text from"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossier [audio | video] -f f01 f02 inFilePath outFilePath"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* f01: Lowest frequency allowed for frequency-domain filtering"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* f02: Hights frequency allowed for frequency domain filtering"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* inFilePath: Audio or video file in any format to recognize text from"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));
            }

        }
    }
}
