using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Parser
{
    enum eDictionaries { ESP  = 1, ENG = 2 }
    enum eParseType    { HTML, TXT}

    class Parser
    {
        List<Dossier.BinarySearchTree<string>> aDictionaryCollection;
        Dossier.BinarySearchTree<string> aBlack;

        IParsing      aParsingType;
        StringBuilder aResultString;
        List<string>  aDataToParse,
                      aDataWithoutSpChars;

        /// <summary>
        /// Class constructor. Builds the given dictionary and blacklist trees.
        /// </summary>
        /// <param name="pLangSelection">Dictionary selection flag</param>
        public Parser(eDictionaries pLangSelection)
        {
            List<string> lDictionaryPaths = new List<string>();

            if ((pLangSelection & eDictionaries.ESP) != 0)
            {
                lDictionaryPaths.Add("dictionary/espanol.txt");
            }
            if ((pLangSelection & eDictionaries.ENG) != 0)
            {
                lDictionaryPaths.Add("dictionary/english.txt");
            }
            this.mpBuildBlack("dictionary/black.txt");
            this.mpBuildDictionaries(ref lDictionaryPaths);
        }

        /// <summary>
        /// Sets the proper interface type
        /// </summary>
        /// <param name="pParsingType">Selection enum variable</param>
        public void mpSetParsingType(eParseType pParsingType)
        {
            switch (pParsingType)
            {
                case eParseType.HTML:
                    Console.WriteLine("HTML Parser");
                    aParsingType = new HTMLparser(); 
                    break;
                case eParseType.TXT: 
                    Console.WriteLine("TXT Parser");
                    aParsingType = new TXTparser(); 
                    break;
                default: 
                    break;
            }
        }

        /// <summary>
        /// Reads all denied words from file and loads them into a binary tree.
        /// </summary>
        /// <param name="pBlackPath">Path to blacklist text file.</param>
        protected void mpBuildBlack(string pBlackPath)
        {
            Console.Write("Building blacklist tree from: {0}", pBlackPath);

            char[]   lSeparator = { '\n', '\r', ' ', ',' };
            string[] lInput;

            try
            {
                lInput = File.ReadAllText(pBlackPath, Encoding.UTF8).Split(lSeparator);
            }
            catch (FileNotFoundException)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. Blacklist file not found.");
                return;
            }

            aBlack = new Dossier.BinarySearchTree<string>();

            foreach (string lBlackWord in lInput)
            {
                aBlack.Insert(lBlackWord);
            }
            
            Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>> LOADED");
        }

        /// <summary>
        /// Reads all words from each file specified in the paths list and loads them into their own binary tree.
        /// </summary>
        /// <param name="pDictionarypaths">List with all the paths to dictionary files.</param>
        protected void mpBuildDictionaries(ref List<string> pDictionarypaths)
        {
            if (pDictionarypaths.Count > 0)
            {
                char[] lSeparator = { '\n', '\r' };
                this.aDictionaryCollection = new List<Dossier.BinarySearchTree<string>>();

                foreach (string lPath in pDictionarypaths)
                {
                    Console.Write("Building dictionary tree from: {0}", lPath);
                    string[] lCurrentInput;

                    try
                    {
                        lCurrentInput = File.ReadAllText(lPath, Encoding.UTF8).Split(lSeparator);
                    }
                    catch (FileNotFoundException)
                    {
                        Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> Dictionary file not found");
                        continue;
                    }

                    Dossier.BinarySearchTree<string> lNewDictionary = new Dossier.BinarySearchTree<string>();

                    foreach (string lDictionaryEntry in lCurrentInput)
                    {
                        lNewDictionary.Insert(lDictionaryEntry);
                    }
                    aDictionaryCollection.Add(lNewDictionary);
                    Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>> LOADED");
                }
            }
            else
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> No dictionary files found");
            }

        }

        /// <summary>
        /// Removes all special characters in aDataToParse (StringBuilder variable)
        /// </summary>
        void mpRemoveSpecialChars()
        {
            Console.Write("===> Removing special chars from filtered data");

            if (aDataToParse == null)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No data to parse");
                return;
            }

            this.aDataWithoutSpChars = new List<string>();
            StringBuilder lNewWord   = new StringBuilder();

            int lIdx = 0,
                lLength;

            foreach (string lWordWithSpChar in aDataToParse)
            {
                lNewWord.Append(lWordWithSpChar);
                lLength = lNewWord.Length;

                while (lIdx < lLength)
                {
                    switch (lNewWord[lIdx])
                    {
                        default: break;
                        case 'Á': case 'á':
                        case 'É': case 'é':
                        case 'Í': case 'í':
                        case 'Ó': case 'ó':
                        case 'Ú': case 'ú':
                        case 'Ñ': case 'ñ':
                            lIdx++;
                            continue;
                    }

                    //65-90 A-Z; 97-122 a-z;
                    if ((lNewWord[lIdx] >= 65 && lNewWord[lIdx] <= 90) || (lNewWord[lIdx] >= 97 && lNewWord[lIdx] <= 122))
                    {
                        lIdx++;
                        continue;
                    }
                    else
                    {
                        lNewWord.Remove(lIdx, 1);
                        lLength = lNewWord.Length;
                    }
                }

                lIdx = 0;

                if (lNewWord.Length > 1)
                {
                    //if first letter is capitalized, then proceed to second check
                    if (mfIsCapitalLetter(lNewWord[0]))
                    {
                        //if another letter is NOT capitalized, then add
                        int lAnalyzedWordLength = lNewWord.Length;

                        if (lAnalyzedWordLength > 3)
                        {
                            bool lAccepted = true;


                            for (int i = 1; i < lAnalyzedWordLength; i++)
                            {
                                if (mfIsCapitalLetter(lNewWord[i]))
                                {
                                    lAccepted = false;
                                    break;
                                }
                            }

                            if (lAccepted)
                                aDataWithoutSpChars.Add(lNewWord.ToString());
                        }
                            
                    }
                    //word is lowered
                    else
                    {
                        aDataWithoutSpChars.Add(lNewWord.ToString().ToLower());
                    }

                    //data without chars consists of lowered and first-letter-capitalized words only.
                        
                }
                lNewWord.Clear();
            }

            Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
        }

        /// <summary>
        /// <para>Filters out any denied or mispelled words: Any word contained in the blacklist or not found in the dictionaries is discarded, the remaining words are appended to the aResultString.</para>
        /// <para>A word is first compared against the blacklist and then against each of the loaded dictionary binary trees.</para>
        /// </summary>
        void mpCompareAgainstDictionary()
        {
            Console.Write("===> Comparing against dictionary data");

            if (aDataWithoutSpChars == null) 
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No data to parse or compare with");
                return;
            }

            if (aDictionaryCollection == null)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No dictionaries loaded"); 
                return;
            }

            this.aResultString    = new StringBuilder();
            List<string> lBufferA = new List<string>(),
                         lBufferB = new List<string>();

            lBufferA = this.aDataWithoutSpChars;

            foreach (Dossier.BinarySearchTree<string> lDictionary in this.aDictionaryCollection)
            {
                foreach (string lWord in lBufferA)
                {
                    if (!this.aBlack.Contains(lWord))
                    {
                        if (mfIsCapitalLetter(lWord[0]))
                        {
                            this.aResultString.Append(lWord + " ");
                        }
                        else
                        {
                            if (lDictionary.Contains(lWord))
                            {
                                this.aResultString.Append(lWord + " ");
                            }
                            else
                            {
                                lBufferB.Add(lWord);
                            }
                        }
                    }
                }

                lBufferA = lBufferB;
                lBufferB.Clear();
            }

            if (aResultString.Length > 0)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.White, " >>>>> COMPLETE");
            }
            else
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>>> FAILED. Not a single match was found");
            }
        }

        /// <summary>
        /// Gets data from a single URL or a text file by calling the appropiate overriden function in the aParsingType interface object and outputs results into a text file.
        /// </summary>
        public void mpParse(string pPath, string pOutFilePath) 
        {
            aDataToParse = aParsingType.GetDataToParse(pPath);

            if (aDataToParse != null)
            {
                this.mpRemoveSpecialChars();
                this.mpCompareAgainstDictionary();

                try
                {
                    this.mpWriteParsedData(pOutFilePath);
                }
                catch (NullReferenceException)
                {
                    Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No data to write to file");
                    return;
                }
            }
        }

        /// <summary>
        /// Gets data from all the URL's in the text file and outputs results for each one in its own file.
        /// </summary>
        /// <param name="pURLfilePath"></param>
        public void mpBatch(string pURLfilePath)
        {
            char[] lSeparator = { '\n', '\r' };
            string[] lURLlist;

            try
            {
                lURLlist = File.ReadAllText(pURLfilePath, Encoding.UTF8).Split(lSeparator);
            }
            catch (FileNotFoundException)
            {
                Dossier.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. URL list file not found.");
                return;
            }

            int lURLcount = lURLlist.Length,
                lEmpty = 0;

            for (int i = 0; i < lURLcount; i++)
            {
                if (lURLlist[i].Length > 0 && lURLlist[i] != "")
                {
                    Console.WriteLine("\n\n" + (i - lEmpty + 1) + " - Processing: " + lURLlist[i]);
                    this.mpParse(lURLlist[i], (i - lEmpty + 1).ToString() + "batch.txt");
                }
                else
                {
                    lEmpty++;
                }
            }
        }

        /// <summary>
        /// Prints aResultString contents to a file.
        /// </summary>
        /// <param name="pOutPath">Path to the output file.</param>
        void mpWriteParsedData(string pOutPath)
        {
            if (this.aResultString != null)
            {
                if (this.aResultString.Length > 0)
                {
                    File.WriteAllText(pOutPath, this.aResultString.ToString(), Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// Returns true if uppercase, false otherwise.
        /// </summary>
        /// <param name="pLetter"></param>
        bool mfIsCapitalLetter(char pLetter)
        {
            return (pLetter >= 65 && pLetter <= 90);
        }       
    }
}
