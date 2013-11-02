/*
 * Write a program that exchanges bits 3, 4 and 5 with bits 24, 25 and 26 of given 32-bit unsigned integer.
 * Write a program that exchanges bits {p, p+1, …, p+k-1) with bits {q, q+1, …, q+k-1} of given 32-bit unsigned integer.
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class ChangeBits
    {
        static void Main()
        {
            Console.Write("Choose a 32-bit unsigned integer (up to 4,294,967,295): ");
            uint number = uint.Parse(Console.ReadLine());
            Console.WriteLine("Binary representation: {0}", Convert.ToString(number, 2));

            int firstBitStart = 3, secondBitStart = 29, bitCount = 3;

            for (int i = 0; i < bitCount; i++)
            {
                // First sequence bit
                int firstBitPos = firstBitStart + i;
                uint mask = (uint)(1 << firstBitPos);
                uint firstBit = (number & mask) >> firstBitPos;
                // Second sequence bit
                int secondBitPos = secondBitStart + i;
                mask = (uint)(1 << secondBitPos);
                uint secondBit = (number & mask) >> secondBitPos;

                // Swap the bits at the given positions
                if (firstBit == 1)
                {
                    mask = (uint)(1 << secondBitPos);
                    number = number | mask;
                }
                else
                {
                    mask = (uint)(~(1 << secondBitPos));
                    number = number & mask;
                }

                if (secondBit == 1)
                {
                    mask = (uint)(1 << firstBitPos);
                    number = number | mask;
                }
                else
                {
                    mask = (uint)(~(1 << firstBitPos));
                    number = number & mask;
                }
            }
            Console.WriteLine("Result after swapping the bits: {0} ({1})", number, Convert.ToString(number, 2));
        }
    }
}
