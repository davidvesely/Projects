/*
 * Write an expression that checks if given integer is odd or even
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class OddOrEven
    {
        static void Main()
        {
            Console.WriteLine("Enter a number, that will be checked if is even or odd");
            int number = int.Parse(Console.ReadLine());
            if (number % 2 == 0)
            {
                Console.WriteLine("Number {0} is even", number);
            }
            else
            {
                Console.WriteLine("Number {0} is odd", number);
            }
        }
    }
}
