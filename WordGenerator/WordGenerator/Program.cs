using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            FilterTexts();
        }

        private static  void FilterTexts()
        {
            Encoding encoding = Encoding.UTF8;
            string[] files = Directory.GetFiles("Tales");
            char[] allowedLetters = File.ReadAllText("letters.txt", encoding).ToLower().ToCharArray();
            foreach (string file in files)
            {
                Console.WriteLine(file);
                string outputFile = "FilteredTales\\" + Path.GetFileName(file);
                //string[] content = File.ReadAllText(file, encoding).Split(".!?".ToCharArray());
                string[] content = Regex.Split(File.ReadAllText(file, encoding), @"(?<=[.?!]|(\r\n))")
                    .Where(a => a != "\r\n" && !string.IsNullOrEmpty(a)).Select(a => a.Trim()).ToArray();
                using (StreamWriter fileWriter = new StreamWriter(outputFile, false, encoding))
                {
                    foreach (string line in content)
                    {
                        bool isAllowed = true;
                        foreach (char character in line.ToLower())
                        {
                            if (Char.IsLetter(character) && !allowedLetters.Contains(character))
                            {
                                isAllowed = false;
                                break;
                            }
                        }
                        if (isAllowed)
                            fileWriter.WriteLine(line);
                    }
                }
            }
        }

        private static void GenerateWordList()
        {
            Encoding windows1251 = Encoding.GetEncoding("windows-1251");
            char[] allowedLetters = File.ReadAllText("letters.txt", windows1251).ToLower().ToCharArray();
            using (StreamWriter fileWriter = new StreamWriter("output.txt", false, windows1251))
            using (StreamReader reader = new StreamReader("input.txt", windows1251))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    bool includeCurrentWord = true;
                    foreach (char letter in line.ToLower())
                    {
                        if (!allowedLetters.Contains(letter))
                        {
                            includeCurrentWord = false;
                            break;
                        }
                    }

                    if (includeCurrentWord)
                    {
                        fileWriter.WriteLine(line);
                        Console.WriteLine(line);
                    }
                }
            }
        }
    }
}
