/*
 * Write a program that finds the biggest of three integers using nested if statements
 */

using System;

class BiggestOfThreeNumbers
{
    static void Main()
    {
        Console.Write("Enter variable 1: ");
        int variable1 = int.Parse(Console.ReadLine());
        Console.Write("Enter variable 2: ");
        int variable2 = int.Parse(Console.ReadLine());
        Console.Write("Enter variable 3: ");
        int variable3 = int.Parse(Console.ReadLine());

        int max;

        if (variable1 > variable2)
        {
            if (variable1 > variable3)
            {
                max = variable1;
            }
            else
            {
                max = variable3;
            }
        }
        else
        {
            if (variable2 > variable3)
            {
                max = variable2;
            }
            else
            {
                max = variable3;
            }
        }

        Console.WriteLine("The biggest number is {0}", max);
    }
}
