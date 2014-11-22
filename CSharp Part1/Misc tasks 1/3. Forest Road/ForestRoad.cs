using System;
using System.Collections.Generic;
using System.Linq;

class ForestRoad
{
    static void Main()
    {
        int width = int.Parse(Console.ReadLine());
        int height = 2 * width - 1;

        for (int i = 0; i < width; i++)
        {
            Console.WriteLine(new string('.', i) + "*" + new string('.', width - i - 1));
        }
        for (int i = width - 2; i >= 0; i--)
        {
            Console.WriteLine(new string('.', i) + "*" + new string('.', width - i - 1));
        }
    }
}