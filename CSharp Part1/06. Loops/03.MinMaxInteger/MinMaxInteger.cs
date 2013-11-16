/*
 * Write a program that reads from the console a sequence of N
 * integer numbers and returns the minimal and maximal of them
 */

using System;

class MinMaxInteger
{
    static void Main()
    {
        int range;
        Console.Write("Please enter count of desired numbers: ");
        bool isValid = int.TryParse(Console.ReadLine(), out range);
        if (!isValid && (range <= 0))
        {
            Console.WriteLine("Please enter valid number!");
            return;
        }

        int[] numbers = new int[range];
        for (int i = 0; i < range; i++)
        {
            Console.Write("Element {0}: ", i);
            isValid &= int.TryParse(Console.ReadLine(), out numbers[i]);
        }

        if (!isValid)
        {
            Console.WriteLine("Please enter valid numbers!");
            return;
        }

        int min, max;
        // Start checking for min and max number from first element
        min = numbers[0];
        max = numbers[0];

        foreach (int number in numbers)
        {
            if (min < number)
            {
                min = number;
            }

            if (max > number)
            {
                max = number;
            }
        }

        Console.WriteLine("Max: {0}, min: {1}", min, max);
    }
}
