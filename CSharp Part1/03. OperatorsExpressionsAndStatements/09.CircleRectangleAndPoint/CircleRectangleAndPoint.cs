/*
 * Write an expression that checks for given point (x, y) if
 * it is within the circle K( (1,1), 3) and out of the
 * rectangle R(top=1, left=-1, width=6, height=2)
 */

namespace OperatorsExpressionsAndStatements
{
    using System;

    class CircleAndPoint
    {
        static void Main()
        {
            Console.WriteLine("Enter the coordinates of a point");
            Console.WriteLine("(Hint: 2, 3.5 will satisfy the given task)");
            Console.Write("X: ");
            double x = double.Parse(Console.ReadLine());
            Console.Write("Y: ");
            double y = double.Parse(Console.ReadLine());

            // Find the distance of the point from circle's center
            int circleRadius = 3;
            int circleX = 1, circleY = 1;
            double distance = Math.Sqrt(Math.Pow(x - circleX, 2) + Math.Pow(y - circleY, 2));
            bool isInCircle = (distance < circleRadius);

            // Check if the point is inside the rectangle
            int top = 1, left = 1, width = 6, height = 2;
            bool isInRectangle = ((left <= x) && (x <= width + left) &&
                (top <= y) && (y <= height + top));

            // Display the result
            string circleResult, rectangleResult;
            if (isInCircle)
            {
                circleResult = "inside";
            }
            else
            {
                circleResult = "outside";
            }

            if (isInRectangle)
            {
                rectangleResult = "inside";
            }
            else
            {
                rectangleResult = "outside";
            }

            Console.WriteLine("The point ({0}, {1}) is {2} a circle K( ({3}, {4}), {5}),",
                    x, y, circleResult, circleX, circleY, circleRadius);
            Console.WriteLine("and {0} rectangle R(top={1}, left={2}, width={3}, height={4})",
                rectangleResult, top, left, width, height);
        }
    }
}
