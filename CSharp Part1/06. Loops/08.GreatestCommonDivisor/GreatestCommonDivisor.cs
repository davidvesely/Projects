/*
 * Write a program that calculates the greatest common divisor (GCD)
 * of given two numbers. Use the Euclidean algorithm (find it in Internet)
 */

using System;

class GreatestCommonDivisor
{
    static void Main()
    {
        bool isValid = true;
        int number1, number2;
        Console.Write("First number = ");
        isValid &= int.TryParse(Console.ReadLine(), out number1);
        Console.Write("Second number = ");
        isValid &= int.TryParse(Console.ReadLine(), out number2);

        // Wikipedia says that usualy the numbers are positive
        if (!isValid || (number1 < 0) || (number1 < 0))
        {
            Console.WriteLine("Bad input");
            return;
        }

        // Euclidean algorithm is very simple, it gets the difference
        // between the two numbers and the min of them, and iterates
        // while the two new numbers aren't equal
        int[] newNums = new int[2];
        newNums[0] = number1;
        newNums[1] = number2;
        while (newNums[0] != newNums[1])
        {
            int difference = Math.Abs(newNums[0] - newNums[1]);
            // Get the smaller between the previous two
            newNums[0] = Math.Min(newNums[0], newNums[1]);
            newNums[1] = difference;
        }

        Console.WriteLine("Greater common divisor of {0} and {1} is {2}", number1, number2, newNums[0]);
    }
}