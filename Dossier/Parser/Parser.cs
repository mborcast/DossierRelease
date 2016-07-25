using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace URLtoText
{
    enum eDictionaries { ESP  = 1, ENG = 2 }
    enum eParseType    { HTML, TXT}

    class Parser
    {
        List<DossierParser.BinarySearchTree<string>> aDictionaryCollection;
        DossierParser.BinarySearchTree<string> aBlack;

        IParsing      aParsingType;
        StringBuilder aResultString;
        List<string>  aDataToParse,
                      aDataWithoutSpChars;

        /// <summary>
        /// Constructor. Define los archivos de diccionarios y de lista negra a ser cargados.
        /// </summary>
        /// <param name="pLangSelection">Flag para seleccionar los idiomas a utilizar.</param>
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
        /// Asigna el tipo de interfase a utilizar.
        /// </summary>
        /// <param name="pParsingType">Enumerado de selección</param>
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
        /// Lee las palabras prohibidas de un archivo de lista negra y las carga en un árbol binario.
        /// </summary>
        /// <param name="pBlackPath">Ruta del archivo de lista negra a leer.</param>
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
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. Blacklist file not found.");
                return;
            }

            aBlack = new DossierParser.BinarySearchTree<string>();

            foreach (string lBlackWord in lInput)
            {
                aBlack.Insert(lBlackWord);
            }
            
            DossierParser.Utilities.mpPrint(ConsoleColor.White, " >>>> LOADED");
        }

        /// <summary>
        /// Lee todas las palabras contenidas en el archivo especificado en cada una de las rutas de una lista de diccionarios habilitados y las carga en su propio árbol binario.
        /// </summary>
        /// <param name="pDictionarypaths">Lista que contiene las rutas de los archivos de diccionario a utilizar.</param>
        protected void mpBuildDictionaries(ref List<string> pDictionarypaths)
        {
            if (pDictionarypaths.Count > 0)
            {
                char[] lSeparator = { '\n', '\r' };
                this.aDictionaryCollection = new List<DossierParser.BinarySearchTree<string>>();

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
                        DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> Dictionary file not found");
                        continue;
                    }

                    DossierParser.BinarySearchTree<string> lNewDictionary = new DossierParser.BinarySearchTree<string>();

                    foreach (string lDictionaryEntry in lCurrentInput)
                    {
                        lNewDictionary.Insert(lDictionaryEntry);
                    }
                    aDictionaryCollection.Add(lNewDictionary);
                    DossierParser.Utilities.mpPrint(ConsoleColor.White, " >>>> LOADED");
                }
            }
            else
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> No dictionary files found");
            }

        }

        /// <summary>
        /// Filtra el texto contenido en "aDataToParse" para conservar vocales y consonantes únicamente.
        /// </summary>
        void mpRemoveSpecialChars()
        {
            Console.Write("===> Removing special chars from filtered data");

            if (aDataToParse == null)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No data to parse");
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

            DossierParser.Utilities.mpPrint(ConsoleColor.White, " >>>> DONE");
        }

        /// <summary>
        /// Compara cada una de las palabras en "aDataWithoutSpChars" contra todos los diccionarios y la lista negra de manera que toda palabra
        /// prohibida o mal escrita sea descartada.
        /// </summary>
        void mpCompareAgainstDictionary()
        {
            Console.Write("===> Comparing against dictionary data");

            if (aDataWithoutSpChars == null) 
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No data to parse or compare with");
                return;
            }

            if (aDictionaryCollection == null)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No dictionaries loaded"); 
                return;
            }

            this.aResultString    = new StringBuilder();
            List<string> lBufferA = new List<string>(),
                         lBufferB = new List<string>();

            lBufferA = this.aDataWithoutSpChars;

            foreach (DossierParser.BinarySearchTree<string> lDictionary in this.aDictionaryCollection)
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
                DossierParser.Utilities.mpPrint(ConsoleColor.White, " >>>>> COMPLETE");
            }
            else
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>>> FAILED. Not a single match was found");
            }
        }

        /// <summary>
        /// Parsea la información de una URL o un archivo de texto para luego imprimirla en un archivo de texto.
        /// </summary>
        public void mpParse(string pPath, string pOutFilePath)
        {
            // Calls the appropiate overriden function in the aParsingType interface object
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
                    DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> ERROR. No data to write to file");
                    return;
                }
            }
        }

        /// <summary>
        /// Parsea en batch la información de todas las URL especificadas en un archivo de texto para luego imprimirla en un archivo de texto para cada URL.
        /// </summary>
        /// <param name="pURLfilePath">Ruta del archivo con todas las URL a analizar.</param>
        public void mpBatch(string pURLfilePath, string pOutPattern)
        {
            char[] lSeparator = { '\n', '\r' };
            string[] lURLlist;

            try
            {
                lURLlist = File.ReadAllText(pURLfilePath, Encoding.UTF8).Split(lSeparator);
            }
            catch (FileNotFoundException)
            {
                DossierParser.Utilities.mpPrint(ConsoleColor.Red, " >>>> FAILED. URL list file not found.");
                return;
            }

            int lURLcount = lURLlist.Length,
                lEmpty = 0;

            for (int i = 0; i < lURLcount; i++)
            {
                if (lURLlist[i].Length > 0 && lURLlist[i] != "")
                {
                    Console.WriteLine("\n\n" + (i - lEmpty + 1) + " - Processing: " + lURLlist[i]);
                    this.mpParse(lURLlist[i], pOutPattern + (i - lEmpty + 1).ToString() + "txt");
                }
                else
                {
                    lEmpty++;
                }
            }
        }

        /// <summary>
        /// Imprime las palabras parseadas a un archivo de texto si y sólo si se cuenta con datos.
        /// </summary>
        /// <param name="pOutPath">Ruta al archivo de salida.</param>
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
        /// Analiza si un caracter es una letra mayúscula o minúscula
        /// </summary>
        /// <param name="pLetter"></param>
        /// <returns>True si la letra es mayúscula, falso de lo contrario.</returns>
        bool mfIsCapitalLetter(char pLetter)
        {
            return (pLetter >= 65 && pLetter <= 90);
        }       
    }
}
