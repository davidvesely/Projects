/*
 * In the combinatorial mathematics, the Catalan numbers are
 * calculated by the following formula (2*n)! / ((n + 1)! * n!)
 * N >= 0
 * Write a program to calculate the Nth Catalan number by given N
 */

using System;
using System.Numerics;

class CatalanNumbers
{
    static void Main()
    {
        int n;
        Console.Write("N = ");
        if (!int.TryParse(Console.ReadLine(), out n) || (n < 0))
        {
            Console.WriteLine("Bad input");
            return;
        }

        // Calculate the factorial
        BigInteger factorial = 1;
        for (int i = 1; i <= n; i++)
        {
            factorial *= i;
        }

        // Continue with the catalan number divided by the factorial
        BigInteger catalanNumber = 1;
        for (int j = 0; (2*n - j) >= (n + 2); j++)
        {
            catalanNumber *= (2 * n - j);
        }

        catalanNumber /= factorial;
        Console.WriteLine("Catalan number for N={0} is {1}", n, catalanNumber);
    }
}