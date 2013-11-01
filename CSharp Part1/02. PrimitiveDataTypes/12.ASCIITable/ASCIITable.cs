/* 
 * Find online more information about ASCII (American
 * Standard Code for Information Interchange) and write
 * a program that prints the entire ASCII table of
 * characters on the console.
 */

namespace PrimitiveDataTypes
{
    using System;

    class ASCIITable
    {
        static void Main()
        {
            // Print ASCII table
            // (Note that characters 11-13 f*cked up the table :)
            // 11 - Vertical tab
            // 12 - New page
            // 13 - Carriage return
            for (int i = 1; i <= 255; i++)
            {
                Console.Write("| {0,3}  {1} ", i, (char)i);

                // Print new line on every 8 printed elements
                if ((i % 8) == 0)
                {
                    Console.WriteLine("|");
                }
            }
            Console.WriteLine();
        }
    }
}
