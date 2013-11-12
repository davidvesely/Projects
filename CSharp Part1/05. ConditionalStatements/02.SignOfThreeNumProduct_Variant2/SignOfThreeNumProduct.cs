/*
 * Write a program that shows the sign (+ or -) of the
 * product of three real numbers without calculating it.
 * Use a sequence of if statements
 * 
 * Variant with only ordinary if statements
 */

using System;

class SignOfThreeNumProduct
{
    static void Main()
    {
        Console.Write("Enter variable 1: ");
        int variable1 = int.Parse(Console.ReadLine());
        Console.Write("Enter variable 2: ");
        int variable2 = int.Parse(Console.ReadLine());
        Console.Write("Enter variable 3: ");
        int variable3 = int.Parse(Console.ReadLine());
        int minusCount = 0;

        if (variable1 < 0)
        {
            minusCount++;
        }

        if (variable2 < 0)
        {
            minusCount++;
        }

        if (variable3 < 0)
        {
            minusCount++;
        }

        if (minusCount == 0 || minusCount == 2)
        {
            Console.WriteLine("The product will be positive.");
        }
        else
        {
            Console.WriteLine("The product will be negative.");
        }
    }
}
