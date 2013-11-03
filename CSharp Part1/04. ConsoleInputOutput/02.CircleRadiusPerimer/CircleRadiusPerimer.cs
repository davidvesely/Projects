/*
 * Write a program that reads the radius r of a circle and prints its perimeter and area
 */

using System;

namespace ConsoleInputOutput
{
    class CircleRadiusPerimer
    {
        static void Main()
        {
            Console.Write("Enter the radius of a circle: ");
            double radius = double.Parse(Console.ReadLine());

            double perimeter = 2 * Math.PI * radius;
            double area = Math.PI * radius * radius;
            Console.WriteLine("Perimeter C={0:F5}; area A={1:F5}", perimeter, area);
        }
    }
}
