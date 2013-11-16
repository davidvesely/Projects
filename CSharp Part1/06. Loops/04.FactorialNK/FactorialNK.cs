/*
 * Write a program that calculates N!/K! for given N and K (1<K<N)
 */

using System;

class FactorialNK
{
    static void Main()
    {
        int N = 0, K = 0;
        Console.Write("Enter N (N > K): ");
        string nStr = Console.ReadLine();
        Console.Write("Enter K (N > K): ");
        string kStr = Console.ReadLine();

        // Check for wrong input
        if (!int.TryParse(nStr, out N) ||
            !int.TryParse(kStr, out K) ||
            (K < 1) || (N < K))
        {
            Console.WriteLine("Bad input");
        }

        long factDifference = 1;
        checked // so we are sure the computation will be correct
        {
            for (int ii = N; ii > K; ii--)
            {
                factDifference *= ii;
            }
        }

        Console.WriteLine("N! / K! = {0}", factDifference);
    }
}
