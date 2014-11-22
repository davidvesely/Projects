using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class FallDown
{
    static void Main()
    {
        bool toDrawPlayfield = false;
        if (Environment.CurrentDirectory.ToLower().EndsWith("bin\\debug"))
        {
            Console.SetIn(new StreamReader("input.txt"));
            toDrawPlayfield = true;
        }

        byte[] grid = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            grid[i] = byte.Parse(Console.ReadLine());
        }

        if (toDrawPlayfield)
        {
            DrawGrid(grid);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 7; j >= 1; j--)
            {
                int k = j;
                while ((GetBit(grid[j], i) == false) && (k > 0))
                {
                    SwipeDown(grid, i, j);
                    k--;
                }
            }
        }

        if (toDrawPlayfield)
        {
            DrawGrid(grid);
        }

        // Draw result
        foreach (byte item in grid)
        {
            Console.WriteLine(item);
        }
    }

    private static void SwipeDown(byte[] grid, int position, int start)
    {
        for (int i = start; i >= 1; i--)
        {
            SetBit(ref grid[i], position, GetBit(grid[i - 1], position));
        }
        SetBit(ref grid[0], position, false);
    }

    private static void DrawGrid(byte[] grid)
    {
        Console.WriteLine();
        foreach (byte item in grid)
        {
            Console.WriteLine(Convert.ToString(item, 2).PadLeft(8, '0').Replace('0', '.'));
        }
    }

    private static bool GetBit(byte number, int position)
    {
        byte mask = 1;
        mask = (byte)(mask << position);
        return ((number & mask) != 0);
    }

    private static void SetBit(ref byte number, int position, bool bitValue)
    {
        if (bitValue)
        {
            int mask = 1;
            mask = mask << position;
            number = (byte)(number | mask);
        }
        else
        {
            int mask = 1;
            mask = ~(mask << position);
            number = (byte)(number & mask);
        }
    }
}