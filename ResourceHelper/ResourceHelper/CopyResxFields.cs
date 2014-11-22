using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceHelper
{
    public static class CopyResxFields
    {
        public static void CopyFields()
        {
            Console.Write("Поддиректориите няма да бъдат обработени!");
            Console.Write("Въведете source на resx файловете: ");
            string source = Console.ReadLine();
            Console.Write("Въведете destination на resx файловете за копиране на полетата: ");
            string destination = Console.ReadLine();

            if (VerifyFolder(source) && VerifyFolder(destination))
            {


            }
        }

        private static bool VerifyFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Error: {0} don't exists.", folder);
                return false;
            }

            return true;
        }
    }
}
