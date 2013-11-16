/*
 * Write a program that prints all the numbers from 1 to N,
 * that are not divisible by 3 and 7 at the same time.
 */

using System;

class NumbersNotDivisible3and7
{
    static void Main()
    {
        int range;
        Console.Write("Please enter count of desired numbers: ");
        bool isValid = int.TryParse(Console.ReadLine(), out range);
        if (!isValid && (range < 0))
        {
            Console.WriteLine("Please enter valid number!");
            return;
        }

        Console.WriteLine("Numbers, not divisible by 3 and 7:");
        for (int i = 1; i <= range; i++)
        {
            if ((i % 3 != 0) && (i % 7 != 0))
            {
                Console.WriteLine("{0} ", i);
            }
        }
    }
}
