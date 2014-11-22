using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

class Pillars
{
    static void Main()
    {
        if (Environment.CurrentDirectory.ToLower().EndsWith("bin\\debug"))
        {
            Console.SetIn(new StreamReader("input.txt"));
        }

        bool[,] grid = new bool[8, 8];
        for (int i = 0; i < 8; i++)
        {
            byte number = byte.Parse(Console.ReadLine());
            for (int j = 0; j < 8; j++) // Cols
            {
                grid[i, j] = GetBit(number, j);
                //if (grid[i, j])
                //{
                //    Console.Write("1 ");
                //}
                //else
                //{
                //    Console.Write("0 ");
                //}
            }
            //Console.WriteLine();
        }

        int pillar = -1;
        int countBits = 0;
        for (int i = 0; i < 8; i++)
        {
            int bitsLeft = 0, bitsRight = 0;
            // Left part
            for (int j = 0; j < i; j++) // Cols
            {
                for (int k = 0; k < 8; k++) // Rows
                {
                    if (grid[k, j])
                    {
                        bitsLeft++;
                    }
                }
            }

            // Right part
            for (int j = i + 1; j < 8; j++) // Cols
            {
                for (int k = 0; k < 8; k++) // Rows
                {
                    if (grid[k, j])
                    {
                        bitsRight++;
                    }
                }
            }

            if (bitsLeft == bitsRight)
            {
                pillar = i;
                countBits = bitsLeft; // Left and right are equal
            }
        }

        if (pillar != -1)
        {
            Console.WriteLine(pillar);
            Console.WriteLine(countBits);
        }
        else
        {
            Console.WriteLine("No");
        }
    }

    private static bool GetBit(byte number, int position)
    {
        byte mask = 1;
        mask = (byte)(mask << position);
        return ((number & mask) != 0);
    }
}