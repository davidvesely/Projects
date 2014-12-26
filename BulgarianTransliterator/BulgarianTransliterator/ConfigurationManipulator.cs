using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BulgarianTransliterator
{
    class ConfigurationManipulator
    {
        private Configuration config;
        private XmlDocument document;
        private const int MAX_CONFIG_VERSION = 1;
        private string[,] defaultTransliterationMapping = new string[,] {
            {"а a"},
            {"б b"},
            {"в v w"},
            {"г i j"},
            {"д d"},
            {"е e"},
            {"ж j z zh g"},
            {"з j z zh"},
            {"и i j"},
            {"й i j ij"},
            {"к k"},
            {"л l"},
            {"м m"},
            {"н n"},
            {"о o u"},
            {"п p"},
            {"р r"},
            {"с s c"},
            {"т t"},
            {"у u y"},
            {"ф f h"},
            {"х h x ch"},
            {"ц tz z c"},
            {"ч ch c 4"},
            {"ш sh h s 6"},
            {"щ sht 6 6t st"},
            {"ь u y"},
            {"ъ u y o a"},
            {"ю iu u"},
            {"я q ia a"}};

        public ConfigurationManipulator(ref Configuration paramConfig)
        {
            this.config = paramConfig;
            this.document = new XmlDocument();
        }

        public bool LoadConfiguration()
        {
            if (IsStoredConfigAvailable() == true)
            {
                if (LoadStoredConfig() == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (WriteDefaultConfiguration() == true)
                {
                    if (LoadStoredConfig() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsStoredConfigAvailable()
        {
            if (File.Exists("config.xml"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LoadValidateApplication()
        {
            string appName = document.SelectSingleNode("/configuration/application/name").InnerText;
            if (appName != "BulgarianTransliterator")
            {
                return false;
            }
            return true;
        }

        private bool LoadVersion()
        {
            int parsedVersion = Convert.ToInt32(document.SelectSingleNode("/configuration/formatCompatibility/version").InnerText);
            if (parsedVersion > 1)
            {
                Console.WriteLine("Error: /configuration/formatCompatibility/version = {0} in config.xml.", parsedVersion);
                Console.WriteLine("Error: This build of the application supports up to {0}. Exiting.", MAX_CONFIG_VERSION);
                return false;
            }
            return true;
        }

        private bool LoadFileNames()
        {
            config.inputFileName = document.SelectSingleNode("/configuration/fileNames/input").InnerText;
            config.outputFileName = document.SelectSingleNode("/configuration/fileNames/output").InnerText;
            return true;
        }

        private bool LoadFeatures()
        {
            // TODO: Perform some validation.
            config.allUpper = Convert.ToBoolean(Convert.ToInt32(document.SelectSingleNode("/configuration/features/allUpper").InnerText));
            config.numberSuffixRange = Convert.ToBoolean(Convert.ToInt32(document.SelectSingleNode("/configuration/features/numberSuffixRange").InnerText));
            config.prefix = Convert.ToBoolean(Convert.ToInt32(document.SelectSingleNode("/configuration/features/prefix").InnerText));
            config.suffix = Convert.ToBoolean(Convert.ToInt32(document.SelectSingleNode("/configuration/features/suffix").InnerText));
            config.upperFirstLetter = Convert.ToBoolean(Convert.ToInt32(document.SelectSingleNode("/configuration/features/upperFirstLetter").InnerText));
            return true;
        }

        private bool LoadPrefixes()
        {
            XmlNode prefixes = document.SelectSingleNode("/configuration/prefixes");
            foreach (XmlNode prefix in prefixes)
            {
                config.prefixWords.Add(prefix.InnerText);
            }
            return true;
        }

        private bool LoadSuffixes()
        {
            XmlNode suffixes = document.SelectSingleNode("/configuration/suffixes");
            foreach (XmlNode suffix in suffixes)
            {
                config.suffixWords.Add(suffix.InnerText);
            }
            return true;
        }

        private bool LoadNumberSuffixRange()
        {
            config.numberSuffixStart = Convert.ToInt32(document.SelectSingleNode("/configuration/numberSuffixRange/start").InnerText);
            config.numberSuffixEnd = Convert.ToInt32(document.SelectSingleNode("/configuration/numberSuffixRange/end").InnerText);
            return true;
        }

        private bool LoadTransliterationMapping()
        {
            XmlNode mapping = document.SelectSingleNode("/configuration/transliterationMapping");
            foreach (XmlNode sourceChar in mapping)
            {
                List<string> sourceCharMapping = new List<string>();
                foreach (XmlNode correspondingString in sourceChar)
                {
                    sourceCharMapping.Add(correspondingString.InnerText);
                }
                config.transliterationMapping.Add(sourceChar.Name[0], sourceCharMapping);
            }

            return true;
        }

        public bool LoadStoredConfig()
        {
            document.Load("config.xml");
            if (LoadValidateApplication() == false ||
                LoadVersion() == false ||
                LoadFileNames() == false ||
                LoadInputEncoding() == false ||
                LoadFeatures() == false ||
                LoadPrefixes() == false ||
                LoadSuffixes() == false ||
                LoadNumberSuffixRange() == false ||
                LoadTransliterationMapping() == false)
            {
                return false;
            }


            return true;
        }

        private bool LoadInputEncoding()
        {
            config.inputEncoding = document.SelectSingleNode("/configuration/inputEncoding/value").InnerText;
            return true;
        }


        public bool WriteDefaultConfiguration()
        {
            Console.WriteLine("No configuration found. Creating default in file config.xml.");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");

            using (XmlWriter writer = XmlWriter.Create("config.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("configuration");

                WriteDefaultApplication(writer);
                WriteDefaultFormatCompatibility(writer);
                WriteDefaultFileNames(writer);
                WriteDefaultInputEncoding(writer);
                WriteDefaultFeatures(writer);
                WriteDefaultPrefixes(writer);
                WriteDefaultSuffixes(writer);
                WriteDefaultNumberSuffixRange(writer);
                WriteDefaultTransliterationMapping(writer);

                writer.WriteEndElement(); // Configuration
                writer.WriteEndDocument();
            }

            return true;
        }

        private void WriteDefaultInputEncoding(XmlWriter writer)
        {
            writer.WriteStartElement("inputEncoding");
            writer.WriteElementString("value", "windows-1251");
            writer.WriteEndElement();
        }

        private void WriteDefaultFileNames(XmlWriter writer)
        {
            writer.WriteStartElement("fileNames");
            writer.WriteElementString("input", "input.txt");
            writer.WriteElementString("output", "output.txt");
            writer.WriteEndElement();
        }

        private void WriteDefaultTransliterationMapping(XmlWriter writer)
        {
            writer.WriteStartElement("transliterationMapping");

            foreach (string elem in defaultTransliterationMapping)
            {
                string[] map = elem.Split();
                writer.WriteStartElement(map[0]);
                for (int i = 1; i < map.Length; i++)
                {
                    writer.WriteElementString("value", map[i]);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // TransliterationMapping
        }

        private void WriteDefaultNumberSuffixRange(XmlWriter writer)
        {
            writer.WriteStartElement("numberSuffixRange");
            writer.WriteElementString("start", "1");
            writer.WriteElementString("end", "99");
            writer.WriteEndElement();
        }

        private void WriteDefaultSuffixes(XmlWriter writer)
        {
            writer.WriteStartElement("suffixes");
            writer.WriteElementString("suffix", "1");
            writer.WriteElementString("suffix", "123");
            writer.WriteElementString("suffix", "qwe");
            writer.WriteEndElement();
        }

        private void WriteDefaultPrefixes(XmlWriter writer)
        {
            writer.WriteStartElement("prefixes");
            writer.WriteElementString("prefix", "1");
            writer.WriteElementString("prefix", "123");
            writer.WriteElementString("prefix", "qwe");
            writer.WriteEndElement();
        }

        private void WriteDefaultFeatures(XmlWriter writer)
        {
            writer.WriteStartElement("features");
            writer.WriteElementString("allUpper", "0");
            writer.WriteElementString("numberSuffixRange", "0");
            writer.WriteElementString("prefix", "0");
            writer.WriteElementString("suffix", "0");
            writer.WriteElementString("upperFirstLetter", "0");
            writer.WriteEndElement();
        }

        private void WriteDefaultFormatCompatibility(XmlWriter writer)
        {
            writer.WriteStartElement("formatCompatibility");
            writer.WriteElementString("version", MAX_CONFIG_VERSION.ToString());
            writer.WriteEndElement();
        }

        private void WriteDefaultApplication(XmlWriter writer)
        {
            writer.WriteStartElement("application");
            writer.WriteElementString("name", "BulgarianTransliterator");
            writer.WriteEndElement();
        }
    }
}
