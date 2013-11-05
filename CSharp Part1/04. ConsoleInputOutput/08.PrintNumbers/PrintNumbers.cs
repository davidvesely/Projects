/*
 * Write a program that reads an integer number n from the
 * console and prints all the numbers in the interval [1..n],
 * each on a single line
 */

using System;

namespace ConsoleInputOutput
{
    class PrintNumbers
    {
        static void Main()
        {
            Console.Write("Provide count of numbers: ");
            int count;
            int.TryParse(Console.ReadLine(), out count);

            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine(i);
            }
        }
    }
}
