/*
 * Write a program that reads 3 integer numbers from the console and prints their sum
 */

using System;

namespace ConsoleInputOutput
{
    class SumOfIntegers
    {
        static void Main()
        {
            Console.WriteLine("Sum of three integers. Enter the numbers:");
            Console.Write("Number 1: ");
            int num1 = int.Parse(Console.ReadLine());
            Console.Write("Number 2: ");
            int num2 = int.Parse(Console.ReadLine());
            Console.Write("Number 3: ");
            int num3 = int.Parse(Console.ReadLine());

            int sum = num1 + num2 + num3;
            Console.WriteLine("The sum is {0}", sum);
        }
    }
}
