using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeasideResearch.LibCurlNet;
using System.IO;

namespace URLtoText
{
    class CurlBehaviour
    {
        static StringBuilder aURLdata;
        static CURLcode      aCURLcode;
        static Easy          aCurlHandler;

        /// <summary>
        /// Callback para extraer la información devuelta por cURL.
        /// </summary>
        static Int32 OnCurlResponse(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            aURLdata.Append(System.Text.Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        /// <summary>
        /// Llama a cURL para extraer información de una URL, la respuesta obtenida es impresa en un archivo de texto.
        /// </summary>
        /// <param name="pURL">URL de la cual se extraerá información.</param>
        /// <param name="pOutPath">Ruta del archivo de salida con la información obtenida de la URL.</param>
        /// <returns>True si la transferencia fue exitosa; false ante el rediccionamiento a otra página, si la URL es una página de infotecnia o si la transferencia falló.</returns>
        public static bool mfCurlAndWrite(string pURL, string pOutPath)
        {
            bool lResult = false;

            if (pURL.Contains("infotecnia"))
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Yellow, " >>>> Aborting. Infotecnia URLs are not allowed.");
                return false;
            }

            try
            {   
                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

                aCurlHandler = new Easy();
                Easy.WriteFunction writeCallback = new Easy.WriteFunction(OnCurlResponse);

                aCurlHandler.SetOpt(CURLoption.CURLOPT_URL, pURL);
                aCurlHandler.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, writeCallback);

                //Spoof the user agent of another browser version and operating system. This would be Chrome 12 in Mac OS X 10.6.8
                aCurlHandler.SetOpt(CURLoption.CURLOPT_USERAGENT, "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_6_8) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.112 Safari/534.30");

                //Fail the request if the HTTP code returned is equal to or larger than 400
                aCurlHandler.SetOpt(CURLoption.CURLOPT_FAILONERROR, 1);

                aCurlHandler.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, 1);

                aCurlHandler.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, 0);

                if (aURLdata == null)
                    aURLdata = new StringBuilder();
                else
                    aURLdata.Clear();

                aCURLcode = aCurlHandler.Perform();

                if (aCURLcode == CURLcode.CURLE_OK)
                {
                    string lResponseUrl = "";

                    aCurlHandler.GetInfo(CURLINFO.CURLINFO_EFFECTIVE_URL, ref lResponseUrl);

                    if (!lResponseUrl.Equals(pURL))
                    {
                        DossierParser.Utilities.mpPrint(ConsoleColor.Yellow, " >>>>> Aborting. Request and Response URLs mismatch");
                        return false;
                    }

                    File.WriteAllText(pOutPath, aURLdata.ToString(), Encoding.UTF8);
                    lResult = true;
                }
                else
                {
                    DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> " + aCURLcode + ": " + aCurlHandler.StrError(aCURLcode));
                    lResult = false;
                }

                aCurlHandler.Cleanup();
                Curl.GlobalCleanup();

                return lResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
