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
        int[] Cx = new int[3];
        int[] Cy = new int[3];
        Cx[0] = int.Parse(Console.ReadLine());
        Cy[0] = int.Parse(Console.ReadLine());
        Cx[1] = int.Parse(Console.ReadLine());
        Cy[1] = int.Parse(Console.ReadLine());
        Cx[2] = int.Parse(Console.ReadLine());
        Cy[2] = int.Parse(Console.ReadLine());

        int SxMin = Math.Min(Sx1, Sx2);
        int SxMax = Math.Max(Sx1, Sx2);
        int SyMin = Math.Min(Sy1, Sy2);
        int SyMax = Math.Max(Sy1, Sy2);

        // Hits
        int[] heightC = new int[3];
        int[] hitCy = new int[3];
        for (int i = 0; i < 3; i++)
		{
            heightC[i] = horizon - Cy[i];
            hitCy[i] = horizon + heightC[i];
		}

        int score = 0;
        for (int i = 0; i < 3; i++)
        {
            // Check for hits under the ship
            if ((hitCy[i] >= SyMin) && (hitCy[i] <= SyMax) &&
                (Cx[i] >= SxMin) && (Cx[i] <= SxMax))
            {
                if (((hitCy[i] == SyMin) || (hitCy[i] == SyMax)) &&
                    ((Cx[i] == SxMin) || (Cx[i] == SxMax)))
                {
                    score += 25;
                }
                else if (((hitCy[i] == SyMin) || (hitCy[i] == SyMax)) ^ 
                    ((Cx[i] == SxMin) || (Cx[i] == SxMax)))
                {
                    score += 50;
                }
                else
                {
                    score += 100;
                }
            }
        }

        Console.WriteLine("{0}%", score);
    }
}