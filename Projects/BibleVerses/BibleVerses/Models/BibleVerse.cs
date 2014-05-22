using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibleVerses.Models
{
    public class BibleVerse
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Text { get; set; }
        public int BiblePlaceID { get; set; }
        [ForeignKey("BiblePlaceID")]
        public virtual BiblePlace BiblePlace { get; set; }
    }

    public class BiblePlace
    {
        public BiblePlace()
        {
            BibleVerses = new List<BibleVerse>();
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [NotMapped]
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
        public virtual ICollection<BibleVerse> BibleVerses { get; set; }
        public override string ToString()
        {
            //return string.Format("Book {0}, Chapter {1}, Verses from {2} to {3}",
            //    Book, Chapter, Start, End);
            if (Start != End)
                return string.Format("{0} {1}:{2}-{3}", BookBG, Chapter, Start, End);
            else
                return string.Format("{0} {1}:{2}", BookBG, Chapter, Start);
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
}
