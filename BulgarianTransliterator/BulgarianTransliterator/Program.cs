using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianTransliterator
{
    class BulgarianTransliterator
    {
        static void Main(string[] args)
        {
            Transliterator bulgarianTransliterator = new Transliterator();
            if (bulgarianTransliterator.Transliterate() == true)
            {
                Console.WriteLine("Transliteration successful.");
            }
            else
            {
                Console.WriteLine("Transliteration failed.");
            }
        }
    }
}
