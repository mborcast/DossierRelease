using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Diagnostics;
using System.IO;
using NReco.VideoConverter;

namespace AudioToText
{
    class ATTmain
    {
        const string TEMP_WAVE_FILE_PATH = "tempWave.wav";
        const string TEMP_ATT_RESULTS_PATH = "tempATTresults.txt";
        const string TEMP_WAVE_FROM_VIDEO_PATH = "tempWavFromVideo.wav";
        const string TEMP_FILTERED_FILE_PATH = "tempFiltered.wav";


        /// <summary>
        /// Ejecuta el programa (FFT.exe) de filtro de frecuencias. El archivo de audio filtrado se crea como una copia temporal: "tempFiltered.wav"
        /// </summary>
        /// <param name="pWaveFilePath">Ruta del archivo de audio en formato .wav</param>
        /// <param name="pLowerPass">Parámetro opcional. Límite inferior de corte de frecuencia.</param>
        /// <param name="pUpperPass">Parámetro opcional. Límite superior de corte de frecuencia.</param>
        static void mpLaunchFrequencyFilter(string pWaveFilePath, int pLowerPass, int pUpperPass)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.CreateNoWindow  = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName        = "FFT.exe";
            startInfo.WindowStyle     = ProcessWindowStyle.Hidden;
            startInfo.Arguments       = pLowerPass + " " + pUpperPass + " " + pWaveFilePath + " " + TEMP_FILTERED_FILE_PATH;

            try
            {
                Process lExeProcess = Process.Start(startInfo);
                lExeProcess.WaitForExit();
                lExeProcess.Close();
            }
            catch (Exception) { }

        }

        /// <summary>
        /// Realiza el reconocimiento de texto en un archivo de audio.
        /// Si dicho archivo requiere convertirse a .wav, se crea una copia temporal en este formato: "tempWave.wav"
        /// El texto resultante del reconocimiento es enviado al archivo temporal: "tempATTresults.txt"
        /// </summary>
        /// <param name="pAudioPathToRecognizeFrom">Ruta del archivo de audio a analizar.</param>
        /// <param name="pFilterAudio">Parámetro opcional. Aplicar el filtrado de frecuencias al archivo de audio?.</param>
        /// <param name="pLowerPass">Parámetro opcional. Límite inferior de corte de frecuencia.</param>
        /// <param name="pUpperPass">Parámetro opcional. Límite superior de corte de frecuencia.</param>
        static bool mfRecognizeTextFrom(string pAudioPathToRecognizeFrom, bool pFilterAudio=false, int pLowerPass=-1, int pUpperPass=-1)
        {
            if (pAudioPathToRecognizeFrom != null)
            {
                ATT audio2Text = new ATT();

                string lWavePath = audio2Text.mfConvertIfNotWaveFile(pAudioPathToRecognizeFrom, TEMP_WAVE_FILE_PATH);

                if (lWavePath != null)
                {
                    if (pFilterAudio)
                    {
                        mpLaunchFrequencyFilter(lWavePath, pLowerPass, pUpperPass);
                        if (File.Exists(TEMP_FILTERED_FILE_PATH))
                        {
                            audio2Text.mpStartSpeechRecognition(TEMP_FILTERED_FILE_PATH);
                        }
                        else
                        {
                            DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. Wave file could not be filtered.");
                            DossierParser.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FILE_PATH);
                            return false;
                        }
                    }
                    else
                    {
                        audio2Text.mpStartSpeechRecognition(lWavePath);
                    }

                    if (!audio2Text.mfWriteResultsToFile(TEMP_ATT_RESULTS_PATH))
                    {
                        return false;
                    }
                }

                if (File.Exists(TEMP_WAVE_FILE_PATH))
                {
                    DossierParser.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FILE_PATH);
                }
                if (File.Exists(TEMP_FILTERED_FILE_PATH))
                {
                    DossierParser.Utilities.mpDeleteTemporalFile(TEMP_FILTERED_FILE_PATH);
                }
                return true;
            }
            else
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. AUDIO FILE DOES NOT EXIST");
                return false;
            }
        }

        /// <summary>
        /// Guarda el audio de un archivo de video en el archivo temporal: "tempWavFromVideo.wav"
        /// </summary>
        /// <param name="pVideoPath">Ruta del archivo de video a analizar.</param>
        static void mpCreateWaveFileFromVideo(string pVideoPath)
        {
            try
            {
                new FFMpegConverter().ConvertMedia(pVideoPath, TEMP_WAVE_FROM_VIDEO_PATH, "wav");
            }
            catch (FileNotFoundException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. VIDEO FILE DOES NOT EXIST");
            }
        }

        public static void Execute(string[] args)
        {
            int argc = args.Length;

            if (argc == 3) 
            {
                switch(args[0])
                {
                case "audio": // => Recognition from a wave file WITHOUT filtering
                    mfRecognizeTextFrom(args[1]);

                    URLtoText.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[2]);
                    DossierParser.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                    break;
                case "video": // => Recognition from a video file WITHOUT filtering
                    mpCreateWaveFileFromVideo(args[1]);
                    mfRecognizeTextFrom(TEMP_WAVE_FROM_VIDEO_PATH);

                    URLtoText.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[2]);

                    DossierParser.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FROM_VIDEO_PATH);
                    DossierParser.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                    break;
                default:
                    break;
                }
            }
            else if (argc == 6) 
            {
                if (args[1] == "-f")
                {
                    switch (args[0])
                    {
                        case "audio": // => Recognition from a wave file WITH filtering
                            if (mfRecognizeTextFrom(args[4], true, Convert.ToInt32(args[2]), Convert.ToInt32(args[3])))
                            {
                                URLtoText.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[5]);
                            }

                            DossierParser.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                            break;
                        case "video": // => Recognition from a video file WITH filtering
                            mpCreateWaveFileFromVideo(args[4]);
                            if (mfRecognizeTextFrom(TEMP_WAVE_FROM_VIDEO_PATH, true, Convert.ToInt32(args[2]), Convert.ToInt32(args[3])))
                            {
                                URLtoText.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[5]);
                            }

                            DossierParser.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FROM_VIDEO_PATH);
                            DossierParser.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
    }
}