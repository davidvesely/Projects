/* 
 * Write a program that prints an isosceles triangle of 9 copyright symbols ©.
 * Use Windows Character Map to find the Unicode code of the © symbol.
 * Note: the © symbol may be displayed incorrectly.
 */

namespace PrimitiveDataTypes
{
    using System;

    class IsoscelesTriangle
    {
        static void Main()
        {
            char copyright = '\u00A9';
            string triangle = "  " + copyright + '\n';
            triangle += " " + new String(copyright, 3) + '\n';
            triangle += new String(copyright, 5);

            Console.WriteLine(triangle);
        }
    }
}
