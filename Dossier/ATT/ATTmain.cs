using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Diagnostics;
using System.IO;
using NReco.VideoConverter;

namespace ATT
{
    class ATTmain
    {
        const string TEMP_WAVE_FILE_PATH = "tempWave.wav";
        const string TEMP_ATT_RESULTS_PATH = "tempATTresults.txt";
        const string TEMP_WAVE_FROM_VIDEO_PATH = "tempWavFromVideo.wav";
        const string TEMP_FILTERED_FILE_PATH = "tempFiltered.wav";


        /// <summary>
        /// Run FFT.exe, creates "tempFiltered.wav"
        /// </summary>
        /// <param name="pWaveFilePath"></param>
        /// <param name="pLowerPass"></param>
        /// <param name="pUpperPass"></param>
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
        /// <para> May create "tempWave.wav and erases it after recognition finishes </para>
        /// <para> Creates "tempATTresults.txt </para>
        /// </summary>
        /// <param name="pAudioPathToRecognizeFrom"></param>
        /// <param name="pFilterAudio"></param>
        /// <param name="pLowerPass"></param>
        /// <param name="pUpperPass"></param>
        static void mpRecognizeTextFrom(string pAudioPathToRecognizeFrom, bool pFilterAudio=false, int pLowerPass=-1, int pUpperPass=-1)
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
                        audio2Text.mpStartSpeechRecognition(TEMP_FILTERED_FILE_PATH);
                    }
                    else
                    {
                        audio2Text.mpStartSpeechRecognition(lWavePath);
                    }

                    audio2Text.mpWriteResultsToFile(TEMP_ATT_RESULTS_PATH);
                }

                if (File.Exists(TEMP_WAVE_FILE_PATH))
                {
                    Dossier.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FILE_PATH);
                }
                if (File.Exists(TEMP_FILTERED_FILE_PATH))
                {
                    Dossier.Utilities.mpDeleteTemporalFile(TEMP_FILTERED_FILE_PATH);
                }
            }
            else
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. AUDIO FILE DOES NOT EXIST");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pVideoPath"></param>
        static void mpCreateWaveFileFromVideo(string pVideoPath)
        {
            try
            {
                new FFMpegConverter().ConvertMedia(pVideoPath, TEMP_WAVE_FROM_VIDEO_PATH, "wav");
            }
            catch (FileNotFoundException)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. VIDEO FILE DOES NOT EXIST");
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
                    mpRecognizeTextFrom(args[1]);

                    Parser.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[2]);
                    Dossier.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                    break;
                case "video": // => Recognition from a video file WITHOUT filtering
                    mpCreateWaveFileFromVideo(args[1]);
                    mpRecognizeTextFrom(TEMP_WAVE_FROM_VIDEO_PATH);

                    Parser.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[2]);

                    Dossier.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FROM_VIDEO_PATH);
                    Dossier.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
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
                            mpRecognizeTextFrom(args[4], true, Convert.ToInt32(args[2]), Convert.ToInt32(args[3]));
                            Parser.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[5]);

                            Dossier.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                            break;
                        case "video": // => Recognition from a video file WITH filtering
                            mpCreateWaveFileFromVideo(args[4]);
                            mpRecognizeTextFrom(TEMP_WAVE_FROM_VIDEO_PATH, true, Convert.ToInt32(args[2]), Convert.ToInt32(args[3]));

                            Parser.ParserMain.ParseOthers(TEMP_ATT_RESULTS_PATH, args[5]);

                            Dossier.Utilities.mpDeleteTemporalFile(TEMP_WAVE_FROM_VIDEO_PATH);
                            Dossier.Utilities.mpDeleteTemporalFile(TEMP_ATT_RESULTS_PATH);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
    }
}