/*
 * Write a program that gets a number n and after that
 * gets more n numbers and calculates and prints their sum
 */

using System;

namespace ConsoleInputOutput
{
    class NumbersSum
    {
        static void Main()
        {
            Console.Write("Please enter the count of numbers: ");
            int count;
            int.TryParse(Console.ReadLine(), out count);
            long sum = 0;

            for (int i = 0; i < count; i++)
            {
                Console.Write("{0}: ", i + 1);
                int number;
                int.TryParse(Console.ReadLine(), out number);
                sum += number;
            }

            Console.WriteLine("Sum of the number is {0}", sum);
        }
    }
}
