/*
 * Write a program that calculates N!*K! / (K-N)! for given N and K (1<N<K)
 */

using System;

class AnotherFactorialEquation
{
    static void Main()
    {
        int N = 0, K = 0;
        Console.Write("Enter N (N < K): ");
        string nStr = Console.ReadLine();
        Console.Write("Enter K (N < K): ");
        string kStr = Console.ReadLine();

        // Check for wrong input
        if (!int.TryParse(nStr, out N) ||
            !int.TryParse(kStr, out K) ||
            (N < 1) || (N > K))
        {
            Console.WriteLine("Bad input");
        }

        decimal result = 1;
        // N factorial
        for (int i = 1; i <= N; i++)
        {
            result *= i;
        }

        // K! / (K-N)!
        /*
         * K! / N! is shortened as K*(K-1)*(K-2)*(K-3)*...(N+1)
         * E.g.: 100! / 96! = 100 * 99 * 98 * 97 (remaining are removed)
         * therefore K! / (K-N)! = K*(K-1)*(K-2)...(K-N + 1)
         * K-N + 1 = K - (N-1)
         * that is why I iterate from 0 to N-1
         */

        for (int i = 0; i <= N - 1; i++)
        {
            result *= (K - i);
        }

        Console.WriteLine("N! * K! / (K-N)! = {0}", result);
    }
}
