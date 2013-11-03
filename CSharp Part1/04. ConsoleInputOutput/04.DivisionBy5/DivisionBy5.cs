/*
 * Write a program that reads two positive integer numbers and
 * prints how many numbers p exist between them such that the
 * reminder of the division by 5 is 0 (inclusive). Example: p(17,25) = 2
 */

using System;

namespace ConsoleInputOutput
{
    class DivisionBy5
    {
        static void Main()
        {
            int number1, number2;
            Console.Write("Number 1: ");
            int.TryParse(Console.ReadLine(), out number1);
            Console.Write("Number 2: ");
            int.TryParse(Console.ReadLine(), out number2);

            if ((number1 <= 0) || (number2 <= 0))
            {
                Console.WriteLine("\nThe integers must be positive and not equal to zero!");
                return;
            }

            int counter = 0;
            // Start and end ensures that if Number1 > Number2 the program will still work correctly
            int start = Math.Min(number1, number2);
            int end = Math.Max(number1, number2);
            for (int i = start; i <= end; i++)
            {
                if (i % 5 == 0)
                {
                    counter++;
                }
            }

            Console.WriteLine("p({0},{1}) = {2}", number1, number2, counter);
        }
    }
}
