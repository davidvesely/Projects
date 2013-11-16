/*
 * Write a program that finds the greatest of given 5 variables
 */

using System;

class GreatestFromFiveVars
{
    static void Main()
    {
        Console.WriteLine("Enter five numbers:");
        int number1 = int.Parse(Console.ReadLine());
        int number2 = int.Parse(Console.ReadLine());
        int number3 = int.Parse(Console.ReadLine());
        int number4 = int.Parse(Console.ReadLine());
        int number5 = int.Parse(Console.ReadLine());

        int max = number1;
        
        if (max < number2)
        {
            max = number2;
        }

        if (max < number3)
        {
            max = number3;
        }

        if (max < number4)
        {
            max = number4;
        }

        if (max < number5)
        {
            max = number5;
        }

        Console.WriteLine("Greatest of these five variables is: {0}", max);

        // Second more shorter variant
        int max2 = Math.Max(number1, number2);
        max2 = Math.Max(max2, number3);
        max2 = Math.Max(max2, number4);
        max2 = Math.Max(max2, number5);
        Console.WriteLine("Second try: greatest of these five variables is: {0}", max);
    }
}
