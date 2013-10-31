/* 
 * Write a program that safely compares floating-point
 * numbers with precision of 0.000001. 
 * Examples:
 * (5.3 ; 6.01) -> false;
 * (5.00000001 ; 5.00000003) -> true
 */

namespace PrimitiveDataTypes
{
    using System;

    class CompareFloatVariables
    {
        static void Main()
        {
            double precision = 0.000001;
            double value1, value2;
            bool areEqual;

            value1 = 5.00000001;
            value2 = 5.00000003;
            // There I assume that if the difference is less
            // than the precision (0.000001) the values are equal
            areEqual = Math.Abs(value1 - value2) < precision;
            Console.WriteLine("({0}; {1}) -> {2}\n", value1, value2, areEqual);

            value1 = 5.3;
            value2 = 6.01;
            // There I assume that if the difference is less
            // than the precision (0.000001) the values are equal
            areEqual = Math.Abs(value1 - value2) < precision;
            Console.WriteLine("({0}; {1}) -> {2}\n", value1, value2, areEqual);
        }
    }
}
