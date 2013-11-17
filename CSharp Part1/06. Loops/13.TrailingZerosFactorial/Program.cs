/*
 * Write a program that calculates for given N
 * how many trailing zeros present at the end of the number N!
 */

using System;

class Program
{
    static void Main()
    {
        int n;
        Console.Write("Please enter N: ");
        bool isValid = int.TryParse(Console.ReadLine(), out n);
        if (!isValid && (n < 0))
        {
            Console.WriteLine("Please enter valid number!");
            return;
        }

        int zeroCounter = 0;
        while (true)
        {
            
        }
    }
}