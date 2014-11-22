using System;
using System.Collections.Generic;
using System.Linq;

class Cartesian
{
    static void Main()
    {
        double x = double.Parse(Console.ReadLine());
        double y = double.Parse(Console.ReadLine());
        int sector = -1;
        if ((x == 0) && (y == 0))
        {
            sector = 0;
        }
        else if ((x > 0) && (y > 0))
        {
            sector = 1;
        }
        else if ((x < 0) && (y > 0))
        {
            sector = 2;
        }
        else if ((x < 0) && (y < 0))
        {
            sector = 3;
        }
        else if ((x > 0) && (y < 0))
        {
            sector = 4;
        }
        else if ((x == 0) && (y != 0))
        {
            sector = 5;
        }
        else if ((x != 0) && (y == 0))
        {
            sector = 6;
        }
        Console.WriteLine(sector);
    }
}