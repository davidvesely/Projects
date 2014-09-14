using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            int option = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Привет в Resx Helper програмата.");
                Console.WriteLine("   1. Вадене на ресурсни файлове за определена култура.");
                Console.WriteLine("   2. Липсващи файлове.");
                Console.WriteLine("   3. Различни файлове (по размер).");
                Console.WriteLine("   4. Merge на полета в различните файлове.");
                Console.WriteLine("   5. Търсене на resx с Modal, true, и др. които не трябва да се превеждат.");
                Console.WriteLine("   6. Копиране на непреведени полета от стари версии на resx.");
                Console.Write("Изберете опция: ");
            } while (int.TryParse(Console.ReadLine(), out option) == false);

            switch (option)
            {
                case 1:
                    ExtractResx.ExtractThem();
                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:

                    break;
                case 6:

                    break;
                default:
                    break;
            }
        }
    }
}
