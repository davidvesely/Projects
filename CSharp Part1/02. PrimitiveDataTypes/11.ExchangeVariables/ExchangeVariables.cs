/* 
 * Declare  two integer variables and assign them
 * with 5 and 10 and after that exchange their values.
 */

namespace PrimitiveDataTypes
{
    using System;

    class ExchangeVariables
    {
        static void Main()
        {
            int firstValue = 5, secondValue = 10;
            int tempValue;
            Console.WriteLine("{0} {1}", firstValue, secondValue);

            // Exchange their values with a temporary variable
            tempValue = firstValue;
            firstValue = secondValue;
            secondValue = tempValue;
            Console.WriteLine("{0} {1}", firstValue, secondValue);
        }
    }
}
