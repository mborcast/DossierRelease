using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    class HTMLparser : IParsing
    {
        /// <summary>
        /// <para>Curls data from given URL path and writes it into a temporal file.</para>
        /// <para>This temporal file will be filtered to extract text only from certain html tags.</para>
        /// </summary>
        /// <param name="pPath">URL to curl data from</param>
        /// <returns>Filtered text from html tags</returns>
        public List<string> GetDataToParse(string pPath)
        {
            if (CurlBehaviour.mfCurlAndWrite(pPath, "temporal.txt"))
            {
                Console.Write("===> Filtering tags");

                char[]   lSeparator = { ' ', '.', ',', '\n', '\r' };
                string[] lFileWords;

                try
                {
                    lFileWords = File.ReadAllText("temporal.txt", Encoding.UTF8).Split(lSeparator);
                }
                catch (FileNotFoundException)
                {
                    Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. No temporal file found.");
                    return null;
                }

                StringBuilder lNewWord = new StringBuilder();
                List<string>  lResult  = new List<string>();

                bool lWithinBrackets = false,
                     lWithinInvalidTag = false;

                int lIdx = 0,
                    lLength;

                foreach (string lWord in lFileWords)
                {
                    lLength = lWord.Length;

                    while (lIdx < lLength)
                    {
                        if (lWord[lIdx] == '<')
                        {
                            try
                            {
                                if (lWord[lIdx + 1] == 's' &&
                                    lWord[lIdx + 2] == 'c' &&
                                    lWord[lIdx + 3] == 'r' &&
                                    lWord[lIdx + 4] == 'i' &&
                                    lWord[lIdx + 5] == 'p' &&
                                    lWord[lIdx + 6] == 't')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 's' &&
                                         lWord[lIdx + 3] == 'c' &&
                                         lWord[lIdx + 4] == 'r' &&
                                         lWord[lIdx + 5] == 'i' &&
                                         lWord[lIdx + 6] == 'p' &&
                                         lWord[lIdx + 7] == 't')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 's' &&
                                            lWord[lIdx + 2] == 't' &&
                                            lWord[lIdx + 3] == 'y' &&
                                            lWord[lIdx + 4] == 'l' &&
                                            lWord[lIdx + 5] == 'e')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 's' &&
                                         lWord[lIdx + 3] == 't' &&
                                         lWord[lIdx + 4] == 'y' &&
                                         lWord[lIdx + 5] == 'l' &&
                                         lWord[lIdx + 6] == 'e')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 't' &&
                                         lWord[lIdx + 2] == 'i' &&
                                         lWord[lIdx + 3] == 'm' &&
                                         lWord[lIdx + 4] == 'e')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                            lWord[lIdx + 2] == 't' &&
                                            lWord[lIdx + 3] == 'i' &&
                                            lWord[lIdx + 4] == 'm' &&
                                            lWord[lIdx + 5] == 'e')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 'n' &&
                                         lWord[lIdx + 2] == 'a' &&
                                         lWord[lIdx + 3] == 'v')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 'n' &&
                                         lWord[lIdx + 3] == 'a' &&
                                         lWord[lIdx + 4] == 'v')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 'a')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 'a')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 'h' &&
                                         lWord[lIdx + 2] == '2')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 'h' &&
                                         lWord[lIdx + 3] == '2')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 'h' &&
                                         lWord[lIdx + 2] == '3')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 'h' &&
                                         lWord[lIdx + 3] == '3')
                                {
                                    lWithinInvalidTag = false;
                                }
                                else if (lWord[lIdx + 1] == 'h' &&
                                         lWord[lIdx + 2] == '6')
                                {
                                    lWithinInvalidTag = true;
                                }
                                else if (lWord[lIdx + 1] == '/' &&
                                         lWord[lIdx + 2] == 'h' &&
                                         lWord[lIdx + 3] == '6')
                                {
                                    lWithinInvalidTag = false;
                                }
                            }
                            catch (IndexOutOfRangeException) { }

                            lWithinBrackets = true;
                            lIdx++;
                            continue;
                        }
                        else if (lWord[lIdx] == '>')
                        {
                            lWithinBrackets = false;
                            lIdx++;
                            continue;
                        }

                        if (!lWithinBrackets && !lWithinInvalidTag)
                        {
                            if (lWord[lIdx] != '\n' && lWord[lIdx] != '\r' && lWord[lIdx] != '\t' && lWord[lIdx] != '/')
                            {
                                //if a number is found... discard word and go check next one
                                if (lWord[lIdx] >= 48 && lWord[lIdx] <= 57)
                                {
                                    lNewWord.Clear();
                                    break;
                                }
                                lNewWord.Append(lWord[lIdx]);
                            }
                        }
                        lIdx++;
                    }

                    lIdx = 0;
                    if (lNewWord.Length > 0)
                    {
                        lResult.Add(lNewWord.ToString());
                        lNewWord.Clear();
                    }
                }
                Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
                Dossier.Utilities.mpDeleteTemporalFile("temporal.txt");
                return lResult;
            }

            //curl failed
            return null;
        }
    }
}
