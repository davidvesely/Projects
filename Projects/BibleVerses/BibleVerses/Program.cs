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

        public override string ToString()
        {
            return string.Format("Book {0}, Chapter {1}, Verses from {2} to {3}",
                Book, Chapter, Start, End);
        }
    }

    public class VerseReader
    {
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
            "Псалми", "Притчи", "Еклесиаст", "Песен на песните", "Исая", "Еремия", "Плач Еремиев", "Езекил", "Даниил", "Осия",
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

            string responseString = reader.ReadToEnd();

            return responseString;
        }

        private static string ExtractCleanText(string raw)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(raw);
            HtmlNode intableNode = document.GetElementbyId("intable");

            return string.Join(" ", 
                intableNode.ChildNodes
                .Select(childNode =>
                    childNode.InnerText
                    .Substring(childNode.InnerText.LastIndexOf("&nbsp;") + 6)
                    .Replace("\n", string.Empty)
                    .Replace("&nbsp;", " ")
                    .Trim()));
        }

        private static IEnumerable<string> GetAllTexts()
        {
            List<string> texts = new List<string>();
            using (StreamReader sr = new StreamReader("d:\\texts.txt", Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    BiblePlace place = GetBiblePlace(line);
                    string rawText = RetrieveRawText(place);
                    string cleanText = ExtractCleanText(rawText);
                    texts.Add(cleanText);
                    Console.WriteLine("Place {0} is OK", line);
                    Thread.Sleep(500);
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
                End = verseEnd
            };
        }

        private static void CreateDoc(List<string> verseList)
        {
            WordApplication mf = new WordApplication();
            mf.Documents.Add();

            //Insert a paragraph at the beginning of the document
            Paragraph oPara1 = mf.ActiveDocument.Content.Paragraphs.Add();
            oPara1.Range.Text = "Test";
            //oPara1.Range.Font.Bold = True
            //oPara1.Format.SpaceAfter = 24    //24 pt spacing after paragraph.
            oPara1.Range.InlineShapes.AddPicture("D:\\pic.jpg");
            //add picture
            oPara1.Range.InsertParagraphAfter();

            //dynamic sign = mf.ActiveDocument.Signatures.AddSignatureLine(null);
            //sign.Sign(null, "Arjun Paudel", "Developer", "myemail@email.com");
            //mf.ActiveDocument.Signatures.Commit();

            try
            {
                mf.ActiveDocument.SaveAs("d:\\test1.docx");
                mf.ActiveDocument.Close();
                mf.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(mf);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oPara1);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch
            {
                oPara1 = null;
                mf = null;
            }
        }

        static void Main(string[] args)
        {
            List<string> texts = GetAllTexts().ToList();
            CreateDoc(texts);
        }
    }
}
