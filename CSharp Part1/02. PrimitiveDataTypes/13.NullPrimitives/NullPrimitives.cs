/* 
 * Create a program that assigns null values to an integer
 * and to double variables. Try to print them on the console,
 * try to add some values or the null literal to them and see
 * the result.
 */

namespace PrimitiveDataTypes
{
    using System;

    class NullPrimitives
    {
        private static void Main()
        {
            int? peaches = null, plums = null;
            double? price, weight;

            Console.WriteLine("{0} {1} {2} {3}", peaches, plums);
        }
    }
}
