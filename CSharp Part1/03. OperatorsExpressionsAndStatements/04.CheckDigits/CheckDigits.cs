/*
 * Write an expression that checks for given integer if
 * its third digit (right-to-left) is 7. E. g. 1732 -> true
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CheckDigits
    {
        static void Main()
        {
            Console.WriteLine("Provide number for a check");
            int number = int.Parse(Console.ReadLine());
            int checkNumber = number / 100;
            if (checkNumber % 10 == 7)
            {
                Console.WriteLine("Third digit is 7");
            }
        }
    }
}
