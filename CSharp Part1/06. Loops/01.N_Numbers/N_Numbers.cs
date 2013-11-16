// Write a program that prints all the numbers from 1 to N

using System;

class N_Numbers
{
    static void Main()
    {
        int range;
        Console.Write("Please enter count of desired numbers: ");
        bool isValid = int.TryParse(Console.ReadLine(), out range);
        if (!isValid && (range < 0))
        {
            Console.WriteLine("Please enter valid number!");
            return;
        }

        for (int i = 1; i <= range; i++)
        {
            Console.Write("{0} ", i);
        }
        Console.WriteLine();
    }
}
