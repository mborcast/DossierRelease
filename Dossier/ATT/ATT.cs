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

namespace ATT
{
    class ATT
    {
        SpeechRecognitionEngine aSpeechRecognitionEngine;
        StringBuilder           aResultString;
        Grammar                 aDictationGrammar;

        /// <summary>
        /// Class constructor. Inits speech recognition engine.
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
        /// Reads file extension and converts to .wav format.
        /// </summary>
        /// <param name="pAudioPath"></param>
        /// <param name="pOutPath"></param>
        /// <returns></returns>
        public string mfConvertIfNotWaveFile(string pAudioPath, string pOutPath)
        {
            Console.Write("Input audio file: " + pAudioPath);

            // Convert to wav if has other format
            if (Path.GetExtension(pAudioPath) != ".wav")
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Yellow, " >>>> CONVERTING TO .wav FILE");

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
                    Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. CONVERSION FAILED");
                    return null;
                }
            }

            // is a wav file
            return pAudioPath;
        }

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
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. AUDIO FILES ONLY");
                return;
            }
            catch (FormatException)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. WAVE FILES ONLY");
                return;
            }
            catch (FileNotFoundException)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. FILE NOT FOUND");
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
                    Dossier.Utilities.mpPrint(ConsoleColor.White, "\n >>>> Recognition Complete\n");
                    Console.WriteLine(aResultString.ToString());
                    break;
                }
            }
            while (lRecognizedText != null);
        }

        void _speechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Dossier.Utilities.mpPrint(ConsoleColor.White, " + Speech segment detected", false);
        }

        void _speechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
        }

        public void mpWriteResultsToFile(string pOutPath)
        {
            try
            {
                if (aResultString.Length > 0)
                {
                    File.WriteAllText(pOutPath, this.aResultString.ToString(), Encoding.UTF8);
                }
            }
            catch (NullReferenceException)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. No data");
            }
        }
    }
}
