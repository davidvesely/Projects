/*
 * Write a program to print the first 100 members of the sequence
 * of Fibonacci: 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, …
 */

using System;
using System.Numerics;

namespace ConsoleInputOutput
{
    class Fibonacci
    {
        static void Main()
        {
            BigInteger previous = 0;
            BigInteger current = 1;

            for (int i = 0; i <= 100; i++)
            {
                Console.WriteLine("{0}", previous);
                BigInteger temp = current;
                current = previous + current;
                previous = temp;
            }
        }
    }
}
