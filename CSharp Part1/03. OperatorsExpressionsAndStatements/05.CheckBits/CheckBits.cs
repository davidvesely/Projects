/*
 * Write a boolean expression for finding if the bit 3
 * (counting from 0) of a given integer is 1 or 0
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CheckBits
    {
        static void Main()
        {
            Console.WriteLine("Provide number for a check");
            int number = int.Parse(Console.ReadLine());
            int mask = 1 << 3;
            int thirdBit = number & mask;
            thirdBit = thirdBit >> 3;
            if (thirdBit == 1)
            {
                Console.WriteLine("The third bit (counting from 0) is 1");
            }
            else
            {
                Console.WriteLine("The third bit (counting from 0) is 0");
            }
        }
    }
}
