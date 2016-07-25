using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageToText;
using AudioToText;
using URLtoText;

namespace DossierParser
{
    class Program
    {
        /*! \mainpage Dependencias y librerías
         *
         * <ul>
         * <li> <b> Assembly System.Speech </b>
         * <br> <tt>Microsoft.NET framework v4.5 </tt>
         * </li>
         *
         * <li> <b> Tesseract.Net.SDK  </b>
         * <br> <tt>Versión: 1.8.341</tt>
         * </li>
         *
         * <li> <b> libcurl.NET (Seaside Research)  </b>
         * <br> <tt>Versión: 1.3.0.0</tt>
         * </li>
         *
         * <li> <b> libcurl Shared Library  </b>
         * <br> <tt>Versión: 7.13.0.0</tt>
         * </li>
         *
         * <li> <b> Magick.NET-Q16-x86 </b>
         * <br> <tt>Versión: 7.0.2.400 </tt>
         * </li>
         *
         * <li> <b> NReco.VideoConverter  </b>
         * <br> <tt>Versión: 1.1.1</tt>
         * </li>
         *
         * <li> <b> NAudio </b>
         * <br> <tt>Versión: 1.7.3 </tt>
         * </li>
         *
         * </ul>
         *
         * <hr>
         *
         * <h1> Usage: dossierParser </h1>
         *
         *  <h2> Procesamiento de URL: </h2>
         *  
         * <tt><b> dossierParser url rutaURL rutaSalida </b></tt>
         *     * <tt>rutaURL</tt>:     URL de la cual se desea obtener información.
         *     * <tt>rutaSalida</tt>: Ruta de archivo de salida.
         *
         * <hr>
         *
         *  <h2> Procesamiento de batch de URLs: </h2>
         *
         * <tt><b> dossierParser url -b rutaArchivoURLs rutaSalida</b></tt>
         *     * <tt>rutaArchivoURLs</tt>: Ruta del archivo de texto con todas las URL a procesar.
         *     * <tt>rutaSalida</tt>: Ruta de archivo de salida.
         *     
         * <hr>
         *
         *  <h2> Procesamiento de imágenes: </h2>
         *
         * <tt><b> dossierParser image rutaImagen rutaSalida </b></tt>
         *     * <tt>rutaImagen</tt>: Ruta de imagen a analizar con OCR.
         *     * <tt>rutaSalida</tt>: Ruta de archivo de salida.
         *
         * <hr>
         *
         *  <h2> Procesamiento de audios y videos: </h2>
         *
         * <tt><b> dossierParser [audio | video] rutaArchivo rutaSalida </b></tt>
         *     * <tt>rutaArchivo</tt>: Ruta del archivo de audio o video (en cualquier formato) a analizar.
         *     * <tt>rutaSalida</tt>:  Ruta de archivo de salida.
         *
         * <hr>
         *
         *  <h2> Procesamiento de audios y videos con filtro de corte de frecuencias: </h2>
         *
         * <tt><b> dossierParser [audio | video] -f f01 f02 rutaArchivo rutaSalida </b></tt>
         *     * <tt>f01</tt>:         Límite inferior de frecuencia de corte.
         *     * <tt>f02</tt>:         Límite superior de frecuencia de corte.
         *     * <tt>rutaArchivo</tt>: Ruta del archivo de audio o video (en cualquier formato) a analizar.
         *     * <tt>rutaSalida</tt>:  Ruta de archivo de salida.
         */
        static void Main(string[] args)
        {
            int argc = args.Length;

            if (argc == 3 || argc == 4 || argc == 6)
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
                Console.WriteLine("Usage: dossierParser url URLpath outFilePath");
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* URLpath: URL to get data from"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossierParser url -b pathToURLfile"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* pathToURLfile: Text file with all the URLs to parse batch\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossierParser image inImagePath outFilePath"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* inImagePath: Image file to apply OCR on"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossierParser [audio | video] inAudioPath outFilePath"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* inAudioPath: Audio or video file in any format to recognize text from"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));

                Console.WriteLine(string.Format("{0,-6} {1}", null, "dossierParser [audio | video] -f f01 f02 inFilePath outFilePath"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* f01: Lowest frequency allowed for frequency-domain filtering"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* f02: Hights frequency allowed for frequency domain filtering"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* inFilePath: Audio or video file in any format to recognize text from"));
                Console.WriteLine(string.Format("{0,-8} {1}", null, "* outFilePath: Path where to output results\n"));
            }

        }
    }
}
