/*
 * Sort 3 real values in descending order using nested if statements
 */

using System;

class SortThreeVariables
{
    static void Main()
    {
        Console.Write("Enter variable 1: ");
        double variable1 = double.Parse(Console.ReadLine());
        Console.Write("Enter variable 2: ");
        double variable2 = double.Parse(Console.ReadLine());
        Console.Write("Enter variable 3: ");
        double variable3 = double.Parse(Console.ReadLine());

        if (variable1 > variable2)
        {
            if (variable1 > variable3)
            {
                if (variable2 > variable3)
                {
                    Console.WriteLine("{0} {1} {2}", variable1, variable2, variable3);
                }
                else
                {
                    Console.WriteLine("{0} {1} {2}", variable1, variable3, variable2);
                }
            }
            else
            {
                Console.WriteLine("{0} {1} {2}", variable3, variable1, variable2);
            }
        }
        else
        {
            if (variable2 > variable3)
            {
                if (variable1 > variable3)
                {
                    Console.WriteLine("{0} {1} {2}", variable2, variable1, variable3);
                }
                else
                {
                    Console.WriteLine("{0} {1} {2}", variable2, variable3, variable1);
                }
            }
            else
            {
                Console.WriteLine("{0} {1} {2}", variable3, variable2, variable1);
            }
        }
    }
}
