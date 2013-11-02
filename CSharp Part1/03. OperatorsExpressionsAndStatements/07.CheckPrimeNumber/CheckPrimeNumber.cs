/*
 * Write an expression that checks if given positive
 * integer number n (n ≤ 100) is prime. E.g. 37 is prime
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CheckPrimeNumber
    {
        static void Main()
        {
            Console.WriteLine("Provide number for prime check");
            int number = int.Parse(Console.ReadLine());
            bool isPrime = true;

            for (int i = 2; i < number; i++)
            {
                // Check every number smaller than the
                // given one if is divisor
                if (number % i == 0)
                {
                    isPrime = false;
                    break; // Stop the cycle
                }
            }

            if (isPrime)
            {
                Console.WriteLine("{0} is prime", number);
            }
            else
            {
                Console.WriteLine("{0} isn't prime", number);
            }
        }
    }
}
