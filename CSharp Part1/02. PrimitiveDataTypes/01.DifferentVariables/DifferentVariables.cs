/* 
 * Declare five variables choosing for each of them the most
 * appropriate of the types byte, sbyte, short, ushort, int,
 * uint, long, ulong to represent the following values:
 * 52130, -115, 4825932, 97, -10000.
 */

namespace PrimitiveDataTypes
{
    using System;

    class DifferentVariables
    {
        static void Main()
        {
            ushort varUShort = 52130;
            SByte varSByte = -115;
            int varInt = 4825932;
            byte varByte = 97;
            short varShort = -10000;

            Console.WriteLine("{0} {1} {2} {3} {4}", varUShort, varSByte, varInt, varByte, varShort);
        }
    }
}
