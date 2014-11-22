using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceHelper
{
    public static class ExtractResx
    {
        public static void ExtractThem()
        {
            Console.Write("Подайте source на проекта: ");
            string source = Console.ReadLine();
            Console.Write("Подайте destination за resx файловете: ");
            string destination = Console.ReadLine();

            if (VerifyFolders(source, destination))
            {
                Console.Write("Изберете културата (празно за всички resx): ");
                string culture = Console.ReadLine();
                Extracting(source, destination);
            }
        }

        private static bool VerifyFolders(string source, string destination)
        {
            if (!Directory.Exists(source))
            {
                Console.WriteLine("Error: {0} don't exists.", source);
                return false;
            }
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            else
            {
                if (Directory.EnumerateFileSystemEntries(destination).Any() == true)
                {
                    Console.WriteLine("Error: {0} isn't empty.", destination);
                    return false;
                }
            }

            return true;
        }

        private static void Extracting(string source, string destination)
        {
            foreach (string file in Directory.EnumerateFiles(
                source, "*.resx", SearchOption.AllDirectories))
            {
                Console.WriteLine(file);
            }
        }
    }
}
