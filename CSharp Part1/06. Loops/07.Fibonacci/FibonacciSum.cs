/*
 * Write a program that reads a number N and calculates the sum of the
 * first N members of the sequence of Fibonacci: 0, 1, 1, 2, 3, 5, 8,
 * 13, 21, 34, 55, 89, 144, 233, 377, … 
 * Each member of the Fibonacci sequence (except the first two) is
 * a sum of the previous two members
 */

using System;
using System.Numerics;

class FibonacciSum
{
    static void Main()
    {
        int n;
        BigInteger first = 0;
        BigInteger second = 1;
        BigInteger sum = 0;

        Console.Write("N = ");
        string strN = Console.ReadLine();

        if (!int.TryParse(strN, out n))
        {
            Console.WriteLine("Invalid number: {0}", strN);
            return;
        }

        for (int i = 0; i < n; i++)
        {
            sum += first; // Sum with current element

            BigInteger temp = first;
            first = second; // Current element becomes previous
            second += temp; // Current new element (sum of the previous)
        }

        Console.WriteLine("The sum of the element to N is {0}.", sum);
    }
}