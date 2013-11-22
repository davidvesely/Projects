using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Bittris
{
    static void Main()
    {
        if (Environment.CurrentDirectory.ToLower().EndsWith("bin\\debug"))
        {
            Console.SetIn(new StreamReader("input.txt"));
        }

        bool gameOver = false;
        int score = 0;
        int[] playfield = new int[4];
        int N = int.Parse(Console.ReadLine());

        for (int i = 0; (i < N) && (gameOver == false); i++)
        {
            Console.WriteLine(); //test
            int row = int.Parse(Console.ReadLine());
            int currentScore = BitCount(row);
            string[] moves = new string[3];
            for (int j = 0; j < 3; j++)
                moves[j] = Console.ReadLine();

            int element = row & 255; // Get only low 8 bits
            Console.WriteLine(Convert.ToString(element | playfield[0], 2).PadLeft(8, '0')); // test
            int currentRow = 0;
            for (int j = 0; j < 3; j++)
            {
                switch (moves[j])
                {
                    case "R":
                        if (GetBit(element, 0) == false)
                        {
                            element = element >> 1;
                        }
                        break;
                    case "L":
                        if (GetBit(element, 7) == false)
                        {
                            element = element << 1;
                        }
                        break;
                    default:
                        break;
                }
                // Should move down?
                if (((element & playfield[currentRow + 1]) == 0) &&
                    (currentRow < 3))
                {
                    currentRow++; // move down
                    Console.WriteLine(Convert.ToString(element | playfield[currentRow], 2).PadLeft(8, '0')); // test
                }
                else
                {
                    // The piece is blocked below
                    playfield[currentRow] |= element;
                    // If whole row is with '1' bits
                    if (playfield[currentRow] == 255)
                    {
                        playfield[currentRow] = 0;
                        currentScore *= 10;
                    }
                    break;
                }
            }

            score += currentScore;
        }
    }

    // Sparse
    private static int BitCount(int n)
    {
        int count = 0;
        while (n != 0)
        {
            count++;
            n &= (n - 1);
        }
        return count;
    }

    private static bool GetBit(int number, int position)
    {
        int mask = 1;
        mask = mask << position;
        return ((number & mask) != 0);
    }

    private static int SetBit(int number, int position, bool bitValue)
    {
        if (bitValue)
        {
            int mask = 1;
            mask = mask << position;
            number = number | mask;
        }
        else
        {
            int mask = 1;
            mask = ~(mask << position);
            number = number & mask;
        }

        return number;
    }
}