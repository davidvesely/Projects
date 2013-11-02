/*
 * Write an expression that extracts from a given integer i
 * the value of a given bit number b. Example: i=5; b=2 -> value=1
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CheckBits3
    {
        static void Main()
        {
            Console.Write("Provide the position: ");
            int position = int.Parse(Console.ReadLine());
            Console.Write("Provide a number: ");
            int number = int.Parse(Console.ReadLine());
            int mask = 1 << position;
            int bit = ((number & mask) >> position);
            Console.WriteLine("Binary representation: " + Convert.ToString(number, 2));
            Console.WriteLine("i={0}; b={1} -> value={2}", number, position, bit);
        }
    }
}
