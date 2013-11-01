/* 
 * 4. Declare an integer variable and assign it with the value 254 in hexadecimal format.
 *    Use Windows Calculator to find its hexadecimal representation.
 * 5. Declare a character variable and assign it with the symbol that hasUnicode code 72.
 *    Hint: first use the Windows Calculator to find the hexadecimal representation of 72.
 * 6. Declare a boolean variable called isFemale and assign
 *    an appropriate value corresponding to your gender.
 */

namespace PrimitiveDataTypes
{
    using System;

    class MoreVariables
    {
        static void Main()
        {
            int hexValue = 0xFE;
            char character = '\u0048'; // 'H' character
            bool isFemale = false;

            Console.WriteLine("{0} {1} {2}", hexValue, character, isFemale);
        }
    }
}
