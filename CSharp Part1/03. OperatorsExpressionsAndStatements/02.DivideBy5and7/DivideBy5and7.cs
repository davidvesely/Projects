/*
 * Write a boolean expression that checks for given
 * integer if it can be divided (without remainder)
 * by 7 and 5 in the same time.
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class DivideBy5And7
    {
        static void Main()
        {
            int number = 4375;
            if ((number % 5 == 0) && (number % 7 == 0))
            {
                Console.WriteLine("{0} can be divided by " +
                    "7 and 5 in the same time", number);
            }
            else
            {
                Console.WriteLine("{0} can not be divided by " +
                    "7 and 5 in the same time", number);
            }
        }
    }
}
