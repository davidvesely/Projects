/*
 * Write an if statement that examines two integer variables and exchanges
 * their values if the first one is greater than the second one 
 */

using System;

class ExchangeVariables
{
    static void Main()
    {
        Console.Write("Enter variable 1: ");
        int variable1 = int.Parse(Console.ReadLine());
        Console.Write("Enter variable 2: ");
        int variable2 = int.Parse(Console.ReadLine());

        // Exchange the value of the variables
        if (variable1 > variable2)
        {
            int tempVariable = variable2;
            variable2 = variable1;
            variable1 = tempVariable;
        }

        Console.WriteLine("Sorted variables in asc order: {0} {1}", variable1, variable2);
    }
}
