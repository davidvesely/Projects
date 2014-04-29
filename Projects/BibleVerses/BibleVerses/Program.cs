using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Word;
using WordApplication = Microsoft.Office.Interop.Word.Application;

namespace BibleVerses
{
    public class BiblePlace
    {
        public string Book
        {
            get
            {
                int index = Array.IndexOf(VerseReader.BooksBG, this.BookBG);
                if (index >= 0)
                    return VerseReader.Books[index];
                else
                    throw new Exception("Book is not found: " + BookBG);
            }
        }
        public string BookBG { get; set; }
        public int Chapter { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string FullLocation { get; set; }

        public override string ToString()
        {
            return string.Format("Book {0}, Chapter {1}, Verses from {2} to {3}",
                Book, Chapter, Start, End);
        }

        public bool IsOverlaping(BiblePlace otherPlace)
        {
            if ((this.BookBG == otherPlace.BookBG) && (this.Chapter == otherPlace.Chapter))
            {
                int a1, a2, b1, b2;
                a1 = this.Start;
                a2 = this.End;
                b1 = otherPlace.Start;
                b2 = otherPlace.End;
                if (IsBetween(a1, b1, b2) || IsBetween(a2, b1, b2) ||
                    IsBetween(b1, a1, a2) || IsBetween(b2, a1, a2))
                {
                    return true;
                }
            }
            
            return false;
        }

        private bool IsBetween(int number, int start, int end)
        {
            return (start <= number) && (number <= end);
        }
    }

    public class BibleVerse
    {
        public string Text { get; set; }
        public BiblePlace Place { get; set; }
    }

    public class VerseReader
    {
        private static int startTextIndex = 1;
        private static readonly bool ShouldAppend = false;
        // 0 - Book; 1 - Chapter; 2 - start verse; 3 - end verse
        // Veren
        //private const string READER_URL = @"http://bible.netbg.com/bible/paralel/bible.php?b={0}&c={1}&r=&v1={2}&v2={3}&vt=1&c7=1&st=&sa=1&rt=2&l=1&cm=2";
        // Bible community
        private const string READER_URL =
            @"http://bible.netbg.com/bible/paralel/bible.php?b={0}&c={1}&r=&v1={2}&v2={3}&vt=1&c6=1&st=&sa=1&rt=2&l=1&cm=2";

        public static string[] Books = new string[] { "genesis", "exodus", "leviticus", "numbers", "deuteronomy", "joshua", "judges",
            "ruth", "1samuel", "2samuel", "1kings", "2kings", "1chronicles", "2chronicles", "ezra", "nehemiah", "esther", "job",
            "psalms", "proverbs", "ecclesiastes", "songofsolomon", "isaiah", "jeremiah", "lamentations", "ezekiel", "daniel", "hosea",
            "joel", "amos", "obadiah", "jonah", "micah", "nahum", "habakkuk", "zephaniah", "haggai", "zechariah", "malachi", "matthew",
            "mark", "luke", "john", "acts", "james", "1peter", "2peter", "1john", "2john", "3john", "jude", "romans", "1corinthians",
            "2corinthians", "galatians", "ephesians", "philippians", "colossians", "1thessalonians", "2thessalonians", "1timothy",
            "2timothy", "titus", "philemon", "hebrews", "revelation" };

        public static string[] BooksBG = new string[] { "Битие", "Изход", "Левит", "Числа", "Второзаконие", "Исус Навиев", "Съдии",
            "Рут", "1 Царе", "2 Царе", "3 Царе", "4 Царе", "1 Летописи", "2 Летописи", "Ездра", "Неемия", "Естир", "Йов",
            "Псалми", "Притчи", "Еклесиаст", "Песен на песните", "Исая", "Еремия", "Плач Еремиев", "Езекил", "Данаил", "Осия",
            "Йоил", "Амос", "Авдий", "Йона", "Михей", "Наум", "Авакум", "Софоний", "Агей", "Захария", "Малахия", "Матей", "Марко",
            "Лука", "Йоан", "Деяния", "Яков", "1 Петрово", "2 Петрово", "1 Йоаново", "2 Йоаново", "3 Йоаново", "Юда", "Римляни",
            "1 Коринтяни", "2 Коринтяни", "Галатяни", "Ефесяни", "Филипяни", "Колосяни", "1 Солунци", "2 Солунци", "1 Тимотей",
            "2 Тимотей", "Тит", "Филимон", "Евреи", "Откровение" };

        private static string RetrieveRawText(BiblePlace place)
        {
            string url = string.Format(READER_URL, place.Book, place.Chapter, place.Start, place.End);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string responseString = reader.ReadToEnd().Replace("<br>", " ");
            return responseString;
        }

        private static string ExtractCleanText(string raw)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(raw);
            HtmlNode intableNode = document.GetElementbyId("intable");

            string cleanText = 
                string.Join(" ", 
                intableNode.ChildNodes
                .Select(childNode =>
                    childNode.InnerText
                    .Substring(childNode.InnerText.LastIndexOf("&nbsp;") + 6)
                    .Replace("\n", string.Empty)
                    .Replace("&nbsp;", " ")
                    .Trim())
                );
            // Capitalize first letter
            if (cleanText.Length > 1)
                cleanText = char.ToUpper(cleanText[0]) + cleanText.Substring(1);
            return cleanText;
        }

        private static List<BibleVerse> GetAllTexts()
        {
            List<BibleVerse> texts = new List<BibleVerse>();
            using (StreamReader sr = new StreamReader("d:\\texts.txt", Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "break")
                        break;

                    try
                    {
                        BiblePlace place = GetBiblePlace(line);
                        string rawText = RetrieveRawText(place);
                        string cleanText = ExtractCleanText(rawText);
                        texts.Add(new BibleVerse() { Place = place, Text = cleanText });
                        Console.WriteLine("Place {0} is OK", line);
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Problem with row: {0}", line);
                        Console.WriteLine("      {0}", ex.Message);
                    }
                }
            }
            return texts;
        }

        private static BiblePlace GetBiblePlace(string line)
        {
            int posDigit = line.IndexOfAny("0123456789".ToCharArray(), 1);
            string book = line.Substring(0, posDigit - 1).TrimEnd();
            int pos_doubleDot = line.IndexOf(':', posDigit);
            int chapter = int.Parse(line.Substring(posDigit, pos_doubleDot - posDigit));
            string verseNumbers = line.Substring(pos_doubleDot + 1);
            int verseStart, verseEnd;
            // Reading verse numbers
            if (verseNumbers.Contains("-"))
            {
                int dashIndex = verseNumbers.IndexOf('-');
                verseStart = int.Parse(verseNumbers.Substring(0, dashIndex));
                verseEnd = int.Parse(verseNumbers.Substring(dashIndex + 1));
            }
            else if (verseNumbers.Contains(","))
            {
                int firstCommaIndex = verseNumbers.IndexOf(',');
                int lastCommaIndex = verseNumbers.LastIndexOf(',');
                verseStart = int.Parse(verseNumbers.Substring(0, firstCommaIndex).Trim());
                verseEnd = int.Parse(verseNumbers.Substring(lastCommaIndex + 1).Trim());
            }
            else
            {
                verseStart = int.Parse(verseNumbers);
                verseEnd = int.Parse(verseNumbers);
            }
            return new BiblePlace
            {
                BookBG = book,
                Chapter = chapter,
                Start = verseStart,
                End = verseEnd,
                FullLocation = line
            };
        }

        private static void CreateDoc(List<BibleVerse> verseList)
        {
#if DEBUG
            string fileName = "D:\\test.docx";
            Console.WriteLine("Filename is {0}\n", fileName);
#else
            Console.Write("\nPlease enter the destination docx file: ");
            string fileName = Console.ReadLine();
#endif
            bool isNewFile = true;
            WordApplication application = new WordApplication();
            // Delete the file before writing
            if (!ShouldAppend)
                File.Delete(fileName);

            if (File.Exists(fileName))
            {
                application.Documents.Open(fileName);
                isNewFile = false;
            }
            else
            {
                application.Documents.Add();
            }

            object pageBreak = WdBreakType.wdPageBreak;
            Paragraph paragraph;
            for (int i = 0; i < verseList.Count; i++)
			{
                // The number of text
                paragraph = application.ActiveDocument.Content.Paragraphs.Add();
                paragraph.Range.Text = (i + startTextIndex).ToString();
                paragraph.set_Style(application.ActiveDocument.Styles["Heading 1"]);
                paragraph.Range.InsertParagraphAfter();
                // Verse
                paragraph = application.ActiveDocument.Content.Paragraphs.Add();
                paragraph.Range.Text = verseList[i].Text;
                paragraph.Range.InsertParagraphAfter();
                // Place
                paragraph = application.ActiveDocument.Content.Paragraphs.Add();
                paragraph.Range.Text = verseList[i].Place.FullLocation;
                paragraph.set_Style(application.ActiveDocument.Styles["Heading 2"]);
                paragraph.Range.InsertParagraphAfter();
                // Page Break
                if (i < verseList.Count - 1)
                    paragraph.Range.InsertBreak(ref pageBreak);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(paragraph);

                // Check for overlapping
                BiblePlace currentPlace = verseList[i].Place;
                var q = verseList.GetRange(0, i).Where(a => a.Place.IsOverlaping(currentPlace));
                if (q.Any())
                {
                    Console.WriteLine("Place {0} is overlapping the following places: {1}\n",
                        currentPlace.ToString(), string.Join(";", q.Select(a => a.Place)));
                }
            }

            try
            {
                if (isNewFile)
                    application.ActiveDocument.SaveAs(fileName);
                else
                    application.ActiveDocument.Save();
                application.ActiveDocument.Close();
                application.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch
            {
                paragraph = null;
                application = null;
            }
        }

        static void Main(string[] args)
        {
            List<BibleVerse> texts = GetAllTexts();
            CreateDoc(texts);
        }
    }
}
