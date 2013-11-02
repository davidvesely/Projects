/*
 * Write an expression that checks if given 
 * point (x, y) is within a circle K(O, 5)
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CircleAndPoint
    {
        static void Main()
        {
            Console.WriteLine("Enter the coordinates of a point");
            Console.Write("X: ");
            int x = int.Parse(Console.ReadLine());
            Console.Write("Y: ");
            int y = int.Parse(Console.ReadLine());

            int circleRadius = 5;
            int circleX = 0, circleY = 0;
            // Find the distance of the point from circle's center
            double distance = Math.Sqrt(Math.Pow(x - circleX, 2) + Math.Pow(y - circleY, 2));
            if (distance < circleRadius)
            {
                Console.WriteLine("The point ({0}, {1}) is inside a circle K( ({2}, {3}), {4})",
                    x, y, circleX, circleY, circleRadius);
            }
            else
            {
                Console.WriteLine("The point ({0}, {1}) is not inside a circle K( ({2}, {3}), {4})",
                    x, y, circleX, circleY, circleRadius);
            }
        }
    }
}
