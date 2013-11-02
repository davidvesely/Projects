/*
 * We are given integer number n, value v (v=0 or 1) and a position p.
 * Write a sequence of operators that modifies n to hold the value v at
 * the position p from the binary representation of n.
 * Example: n = 5 (00000101), p=3, v=1  13 (00001101)
 * n = 5 (00000101), p=2, v=0  1 (00000001)
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class ChangeBits
    {
        static void Main()
        {
            Console.Write("Choose a number: ");
            int number = int.Parse(Console.ReadLine());
            Console.WriteLine("Binary representation: {0}",
                Convert.ToString(number, 2));
            Console.Write("Select position of a bit: ");
            int position = int.Parse(Console.ReadLine());
            Console.Write("New value of the bit: ");
            int newBitValue = int.Parse(Console.ReadLine());

            if ((newBitValue != 0) && (newBitValue != 1))
            {
                return; // Exit from the program in case of incorrect values
            }

            int newNumber;
            // The operations depend of new value for the bit
            if (newBitValue == 1)
            {
                int mask = 1 << position;
                newNumber = number | mask;
            }
            else
            {
                int mask = 1 << position;
                // XOR the mask (for example ~100 = 11111111111111111111111111111011)
                mask = ~mask; 
                newNumber = number & mask;
            }

            Console.WriteLine("{0} ({1})", newNumber, Convert.ToString(newNumber, 2));
        }
    }
}
