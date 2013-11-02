/*
 * Write a boolean expression that returns if the bit at position p
 * (counting from 0) in a given integer number v has value of 1.
 * Example: v=5; p=1 -> false
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CheckBits2
    {
        static void Main()
        {
            Console.Write("Provide the position: ");
            int position = int.Parse(Console.ReadLine());
            Console.Write("Provide a number: ");
            int number = int.Parse(Console.ReadLine());
            // Create a mask: move the bit 1 to the given position
            int mask = 1 << position;
            // Bitwise operation which will get the bit at the given position
            // and moves it back to position 0, so it will become 1 or 0
            int bit = ((number & mask) >> position);
            bool hasValue1 = (bit == 1);
            Console.WriteLine("Binary representation: " + Convert.ToString(number, 2));
            Console.WriteLine("value={0}; position={1} - > {2}", number, position, hasValue1);
        }
    }
}
