/*
 * Write a program that enters the coefficients a, b and c of a quadratic equation
 *       a*x2 + b*x + c = 0
 * and calculates and prints its real roots. 
 * Note that quadratic equations may have 0, 1 or 2 real roots
 */

using System;

class QuadraticEquationSolver
{
    static void Main()
    {
        Console.WriteLine("Please provide the coefficients of the\n" + 
            "quadratic equation a*x2 + b*x + c = 0");
        Console.Write("a: ");
        int a = int.Parse(Console.ReadLine());
        Console.Write("b: ");
        int b = int.Parse(Console.ReadLine());
        Console.Write("c: ");
        int c = int.Parse(Console.ReadLine());

        double discriminant = (b * b) - (4 * a * c);

        if (discriminant < 0)
        {
            Console.WriteLine("There are no real roots.");
        }
        else if (discriminant == 0)
        {
            double root = -b / (2 * a);
            Console.WriteLine("The only root of the equation is {0:F4}", root);
        }
        else
        {
            double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            Console.WriteLine("Roots of the equation are: {0:F4} and {1:F4}", root1, root2);
        }
    }
}
