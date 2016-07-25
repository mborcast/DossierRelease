using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Globalization;

using NAudio.Wave;

namespace AudioToText
{
    class ATT
    {
        SpeechRecognitionEngine aSpeechRecognitionEngine;
        StringBuilder           aResultString;
        Grammar                 aDictationGrammar;

        /// <summary>
        /// Constructor. Inicializa el Speech Recognition Engine de Microsoft para reconocer palabras en español.
        /// </summary>
        public ATT()
        {
            //enable speech recognition language
            aSpeechRecognitionEngine   = new SpeechRecognitionEngine(new CultureInfo("es-ES"));

            aDictationGrammar = new DictationGrammar();
            aSpeechRecognitionEngine.LoadGrammar(aDictationGrammar);

            aSpeechRecognitionEngine.BabbleTimeout              = new TimeSpan(Int32.MaxValue);
            aSpeechRecognitionEngine.InitialSilenceTimeout      = new TimeSpan(Int32.MaxValue);
            aSpeechRecognitionEngine.EndSilenceTimeout          = new TimeSpan(100000000);
            aSpeechRecognitionEngine.EndSilenceTimeoutAmbiguous = new TimeSpan(100000000);

            aResultString = new StringBuilder();
        }

        /// <summary>
        /// Analiza la extensión del archivo de audio y de ser necesario, lo convierte a un formato .wav
        /// </summary>
        /// <param name="pAudioPath">Ruta del archivo de audio a analizar.</param>
        /// <param name="pOutPath">Ruta del nuevo archivo de audio convertido</param>
        /// <returns>Devuelve la ruta del nuevo archivo de audio convertido</returns>
        public string mfConvertIfNotWaveFile(string pAudioPath, string pOutPath)
        {
            Console.Write("Input audio file: " + pAudioPath);

            // Convert to wav if has other format
            if (Path.GetExtension(pAudioPath) != ".wav")
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Yellow, " >>>> CONVERTING TO .wav FILE");

                MediaFoundationReader lReader = new MediaFoundationReader(pAudioPath);
                WaveFileWriter.CreateWaveFile(pOutPath, lReader);

                if (File.Exists(pOutPath))
                {
                    //conversion successful
                    return pOutPath;
                }
                else
                {
                    //conversion failed
                    DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. CONVERSION FAILED");
                    return null;
                }
            }

            // is a wav file
            return pAudioPath;
        }

        /// <summary>
        /// Realiza el reconocimiento de un audio con formato .wav exclusivamente. 
        /// Este proceso continuará hasta que el engine no detecte más segmentos reconocibles.
        /// Cada resultado obtenido se adjunta en la variable "aResultString"
        /// </summary>
        /// <param name="pWavePath"></param>
        public void mpStartSpeechRecognition(string pWavePath)
        {
            Console.WriteLine("Recognizing text from: " + pWavePath);

            try
            {
                //define engine input type
                aSpeechRecognitionEngine.SetInputToWaveFile(pWavePath);
            }
            catch (InvalidOperationException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. AUDIO FILES ONLY");
                return;
            }
            catch (FormatException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. WAVE FILES ONLY");
                return;
            }
            catch (FileNotFoundException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. FILE NOT FOUND");
                return;
            }

            RecognitionResult lRecognizedText;

            aSpeechRecognitionEngine.SpeechRecognized += _speechRecognitionEngine_SpeechRecognized;
            aSpeechRecognitionEngine.SpeechDetected   += _speechRecognitionEngine_SpeechDetected;

            do
            {
                try
                {
                    //get recognized text
                    lRecognizedText = aSpeechRecognitionEngine.Recognize();
                    aResultString.Append(lRecognizedText.Text + " ");
                }
                catch
                {
                    //break whenever no more audio is recognized
                    DossierParser.Utilities.mpPrint(ConsoleColor.White, "\n >>>> Recognition Complete\n");
                    Console.WriteLine(aResultString.ToString());
                    break;
                }
            }
            while (lRecognizedText != null);
        }

        /// <summary>
        /// Imprime un aviso en consola cada vez que el engine detecta un segmento reconocible.
        /// </summary>
        void _speechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            DossierParser.Utilities.mpPrint(ConsoleColor.White, " + Speech segment detected", false);
        }

        /// <summary>
        /// Imprime un aviso en consola cada vez que el engine finaliza de procesar un segmento detectado previamente.
        /// </summary>
        void _speechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            DossierParser.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
        }

        /// <summary>
        /// Imprime el texto obtenido en un archivo de texto si y solo si se encontraron resultados.
        /// </summary>
        /// <param name="pOutPath">Ruta del archivo de salida</param>
        public bool mfWriteResultsToFile(string pOutPath)
        {
            try
            {
                if (aResultString.Length > 0)
                {
                    File.WriteAllText(pOutPath, this.aResultString.ToString(), Encoding.UTF8);
                    return true;
                }
                else
                {
                    DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. No results to write.");
                }
            }
            catch (NullReferenceException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. No data");
            }
            return false;
        }
    }
}
