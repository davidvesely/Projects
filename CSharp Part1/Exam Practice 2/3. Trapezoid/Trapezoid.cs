using System;
using System.Collections.Generic;
using System.Linq;

class Trapezoid
{
    static void Main()
    {
        int number = int.Parse(Console.ReadLine());
        Console.WriteLine(new string('.', number) + new string('*', number));
        for (int i = 1; i < number; i++)
        {
            Console.Write(new string('.', number - i));
            Console.Write("*");
            Console.Write(new string('.', (i - 1) + (number - 1)));
            Console.WriteLine("*");
        }
        Console.WriteLine(new string('*', number * 2));
    }
}