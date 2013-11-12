/*
 * Write a program that shows the sign (+ or -) of the
 * product of three real numbers without calculating it.
 * Use a sequence of if statements
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

        // Determining the sign of the product
        // between the first and second variable
        bool isNegative;
        if ((variable1 < 0) ^ (variable2 < 0))
        {
            isNegative = true;
        }
        else
        {
            isNegative = false;
        }

        // Determine the sign between the previous result
        // and third variable
        if (isNegative ^ (variable3 < 0))
        {
            isNegative = true;
        }
        else
        {
            isNegative = false;
        }

        if (isNegative)
        {
            Console.WriteLine("The product will be negative.");
        }
        else
        {
            Console.WriteLine("The product will be positive.");
        }
    }
}
