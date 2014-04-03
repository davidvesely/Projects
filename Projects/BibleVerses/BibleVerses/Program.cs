using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BibleVerses
{
    public class VerseReader
    {
        // 0 - Book; 1 - Chapter; 2 - start verse; 3 - end verse
        private const string READER_URL = @"http://bible.netbg.com/bible/paralel/bible.php?b={0}&c={1}&r=&v1={2}&v2={3}&vt=1&c7=1&st=&sa=1&rt=2&l=1&cm=2";

        private string[] books = new string[] { "genesis", "exodus", "leviticus", "numbers", "deuteronomy", "joshua", "judges",
            "ruth", "1samuel", "2samuel", "1kings", "2kings", "1chronicles", "2chronicles", "ezra", "nehemiah", "esther", "job",
            "psalms", "proverbs", "ecclesiastes", "songofsolomon", "isaiah", "jeremiah", "lamentations", "ezekiel", "daniel", "hosea",
            "joel", "amos", "obadiah", "jonah", "micah", "nahum", "habakkuk", "zephaniah", "haggai", "zechariah", "malachi", "matthew",
            "mark", "luke", "john", "acts", "james", "1peter", "2peter", "1john", "2john", "3john", "jude", "romans", "1corinthians",
            "2corinthians", "galatians", "ephesians", "philippians", "colossians", "1thessalonians", "2thessalonians", "1timothy",
            "2timothy", "titus", "philemon", "hebrews", "revelation" };

        private string[] booksBG = new string[] { "Битие", "Изход", "Левит", "Числа", "Второзаконие", "Исус Навиев", "Съдии",
            "Рут", "1 Царе", "2 Царе", "3 Царе", "4 Царе", "1 Летописи", "2 Летописи", "Ездра", "Неемия", "Естир", "Йов",
            "Псалми", "Притчи", "Еклесиаст", "Песен на песните", "Исая", "Еремия", "Плач Еремиев", "Езекил", "Даниил", "Осия",
            "Йоил", "Амос", "Авдий", "Йона", "Михей", "Наум", "Авакум", "Софоний", "Агей", "Захария", "Малахия", "Матей", "Марко",
            "Лука", "Йоан", "Деяния", "Яков", "1 Петрово", "2 Петрово", "1 Йоаново", "2 Йоаново", "3 Йоаново", "Юда", "Римляни",
            "1 Коринтяни", "2 Коринтяни", "Галатяни", "Ефесяни", "Филипяни", "Колосяни", "1 Солунци", "2 Солунци", "1 Тимотей",
            "2 Тимотей", "Тит", "Филимон", "Евреи", "Откровение" };

        private static string RetrieveText(string book, int chapter, int start, int end)
        {
            string url = string.Format(READER_URL, book, chapter, start, end);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

            string responseString = reader.ReadToEnd();

            return responseString;
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(RetrieveText("genesis", 1, 2, 5));
        }
    }
}
