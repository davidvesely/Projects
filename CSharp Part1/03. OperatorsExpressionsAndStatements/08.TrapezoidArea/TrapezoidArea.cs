/*
 * Write an expression that calculates trapezoid's
 * area by given sides a and b and height h
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class TrapezoidArea
    {
        static void Main()
        {
            Console.WriteLine("Provide information about the trapezoid:");
            Console.Write("a: ");
            int sideA = int.Parse(Console.ReadLine());
            Console.Write("b: ");
            int sideB = int.Parse(Console.ReadLine());
            Console.Write("h: ");
            int height = int.Parse(Console.ReadLine());

            double area = (sideA + sideB) / 2 * height;
            Console.WriteLine("Trapezoid's area: {0}", area);
        }
    }
}
