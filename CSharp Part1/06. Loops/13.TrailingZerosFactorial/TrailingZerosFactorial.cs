/*
 * Write a program that calculates for given N
 * how many trailing zeros present at the end of the number N!
 */

using System;

class TrailingZerosFactorial
{
    static void Main()
    {
        int number;
        Console.Write("Please enter N: ");
        bool isValid = int.TryParse(Console.ReadLine(), out number);
        if (!isValid && (number < 0))
        {
            Console.WriteLine("Please enter valid number!");
            return;
        }

        int zeroCounter = 0;
        for (int power = 1; Math.Pow(5, power) < number; power++)
        {
            zeroCounter += number / (int)Math.Pow(5, power);
        }

        Console.WriteLine("The number of trailing zeros is {0}", zeroCounter);
    }
}