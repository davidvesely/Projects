using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianTransliterator
{
    class Transliterator
    {
        private Configuration config = new Configuration();
        private StreamReader inputHandler;
        private StreamWriter outputHandler;

        private void PrintVersion()
        {
            Console.WriteLine("Bulgarian Transliterator V 1.0.0. Free and open source.");
            Console.WriteLine("Author: Gogothebee, https://github.com/gogothebee");
        }

        private bool LoadConfig()
        {
            ConfigurationManipulator configManipulator = new ConfigurationManipulator(ref config);
            return configManipulator.LoadConfiguration();
        }

        public bool Transliterate()
        {
            PrintVersion();
            if (LoadConfig() == false ||
                OpenFiles() == false)
            {
                return false;
            }

            IterateAllWords();

            CloseFiles();
            return true;
        }

        private void HandleSingleWord(string word)
        {
            if (word.Length == 0)
            {
                return;
            }
            word = word.ToLower();
            string outLine = "";
            TransliterateSingleWord(word, 0, outLine); // Recursive call with starting position 0
        }

        private void IterateAllWords()
        {
            string word = null;
            while ((word = inputHandler.ReadLine()) != null)
            {
                HandleSingleWord(word);
            }
        }

        private void TransliterateSingleWord(string sourceWord, int pos, string resultWord)
        {
            if (sourceWord.Length == pos) ////If the whole source word is already transliterated
            {
                outputHandler.WriteLine(resultWord); // Write the accumulated result in the output file
                ApplyWordTransformations(resultWord);
            }
            else
            {
                // We're still recursively transliterating the source word
                List<string> mappedStrings;
                if (config.transliterationMapping.TryGetValue(sourceWord[pos], out mappedStrings))
                {
                    foreach (string mapppedString in mappedStrings)
                    {
                        TransliterateSingleWord(sourceWord, pos + 1, resultWord + mapppedString);
                    }
                }
            }
        }

        private void CloseFiles()
        {
            inputHandler.Close();
            outputHandler.Close();
        }

        private bool OpenFiles()
        {
            try
            {
                inputHandler = new StreamReader(config.inputFileName, Encoding.GetEncoding(config.inputEncoding));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Input file {0} not found.", config.inputFileName);
                return false;
            }
            outputHandler = new StreamWriter(config.outputFileName);
            return true;
        }

        private void ApplyWordTransformations(string word)
        {
            if (config.prefix == true)
            {
                ApplyPrefix(word);
            }
            if (config.suffix == true)
            {
                ApplySuffix(word);
            }
            if (config.upperFirstLetter == true)
            {
                ApplyUpperFirstLetter(word);
            }
            if (config.allUpper == true)
            {
                ApplyAllUpper(word);
            }
            if (config.numberSuffixRange == true)
            {
                ApplyNumberRangeSuffix(word);
            }
        }

        private void ApplyNumberRangeSuffix(string word)
        {
            for (int i = config.numberSuffixStart; i <= config.numberSuffixEnd; i++)
            {
                outputHandler.WriteLine(word + i);
            }
        }

        private void ApplyAllUpper(string word)
        {
            outputHandler.WriteLine(word.ToUpper());
        }

        private void ApplyUpperFirstLetter(string word)
        {
            outputHandler.WriteLine(Char.ToUpper(word[0]) + word.Substring(1));
        }

        private void ApplyPrefix(string word)
        {
            foreach (string prefix in config.prefixWords)
            {
                outputHandler.WriteLine(prefix + word);
            }
        }

        private void ApplySuffix(string word)
        {
            foreach (string suffix in config.suffixWords)
            {
                outputHandler.WriteLine(word + suffix);
            }
        }
    }
}
