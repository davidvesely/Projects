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

        int[] newNums = new int[2];
        do
        {
            
        } while (newNums[0] != newNums[1]);
    }
}