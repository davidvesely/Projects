/*
 * Write a program that exchanges bits 3, 4 and 5 with bits 24, 25 and 26 of given 32-bit unsigned integer.
 * Write a program that exchanges bits {p, p+1, …, p+k-1) with bits {q, q+1, …, q+k-1} of given 32-bit unsigned integer.
 * 
 * Cooler variant with methods
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

            int firstBitStart = 3, secondBitStart = 24, bitCount = 3;

            for (int i = 0; i < bitCount; i++)
            {
                int firstBitPos = firstBitStart + i;
                int secondBitPos = secondBitStart + i;
                bool firstBit = GetBit(number, firstBitPos);
                bool secondBit = GetBit(number, secondBitPos);

                // Set the new exchanged bits at the given positions
                number = SetBit(number, firstBitPos, secondBit);
                number = SetBit(number, secondBitPos, firstBit);
            }
            Console.WriteLine("Result after swapping the bits: {0} ({1})", number, Convert.ToString(number, 2));
        }

        private static bool GetBit(uint number, int position)
        {
            uint mask = (uint)(1 << position);
            return ((number & mask) != 0);
        }

        private static uint SetBit(uint number, int position, bool bitValue)
        {
            if (bitValue)
            {
                uint mask = (uint)(1 << position);
                number = number | mask;
            }
            else
            {
                uint mask = (uint)(~(1 << position));
                number = number & mask;
            }

            return number;
        }
    }
}
