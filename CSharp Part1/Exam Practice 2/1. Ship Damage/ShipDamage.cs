using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class ShipDamage
{
    static void Main()
    {
        if (Environment.CurrentDirectory.ToLower().EndsWith("bin\\debug"))
        {
            Console.SetIn(new StreamReader("input.txt"));
        }

        // Corners of the ship
        int Sx1 = int.Parse(Console.ReadLine());
        int Sy1 = int.Parse(Console.ReadLine());
        int Sx2 = int.Parse(Console.ReadLine());
        int Sy2 = int.Parse(Console.ReadLine());
        int horizon = int.Parse(Console.ReadLine());
        // Canons
        int Cx1 = int.Parse(Console.ReadLine());
        int Cy1 = int.Parse(Console.ReadLine());
        int Cx2 = int.Parse(Console.ReadLine());
        int Cy2 = int.Parse(Console.ReadLine());
        int Cx3 = int.Parse(Console.ReadLine());
        int Cy3 = int.Parse(Console.ReadLine());


    }
}