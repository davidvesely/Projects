/* 
 * Which of the following values can be assigned to a variable
 * of type float and which to a variable of type double:
 * 34.567839023, 12.345, 8923.1234857, 3456.091?
 */

namespace PrimitiveDataTypes
{
    using System;

    class DoubleFloatComparison
    {
        static void Main()
        {
            double value1 = 34.567839023d;
            float value2 = 12.345f;
            double value3 = 8923.1234857d;
            float value4 = 3456.091f;

            Console.WriteLine("{0} {1} {2} {3}", value1, value2, value3, value4);
        }
    }
}
