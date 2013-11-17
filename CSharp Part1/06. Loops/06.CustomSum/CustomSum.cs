/*
 * Write a program that, for a given two integer numbers N and X,
 * calculates the sumS = 1 + 1!/X + 2!/X2 + … + N!/X^N
 */

using System;
using System.Numerics;

class CustomSum
{
    static void Main()
    {
        Console.Write("N = ");
        int n = int.Parse(Console.ReadLine());
        Console.Write("X = ");
        int x = int.Parse(Console.ReadLine());

        decimal theSum = 1;
        decimal factorial = 1;
        decimal denominator = 1;
        for (int i = 1; i <= n; i++)
        {
            factorial *= i;
            denominator *= x;
            decimal currentMember = factorial / denominator;
            theSum += currentMember;
        }

        Console.WriteLine("Sum = {0}", theSum);
    }
}