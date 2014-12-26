using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianTransliterator
{
    class Configuration
    {
        public bool prefix;
        public bool suffix;
        public bool upperFirstLetter;
        public bool allUpper;
        public bool numberSuffixRange;

        public string inputFileName;
        public string outputFileName;
        public string inputEncoding;
        public int numberSuffixStart;
        public int numberSuffixEnd;
        public List<string> prefixWords = new List<string>();
        public List<string> suffixWords = new List<string>();
        public Dictionary<char, List<string>> transliterationMapping = new Dictionary<char, List<string>>();
    }
}
