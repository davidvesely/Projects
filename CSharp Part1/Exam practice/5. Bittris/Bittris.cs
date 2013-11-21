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
        bool[,] playfield = new bool[4, 8];
        int N = int.Parse(Console.ReadLine());

        for (int i = 0; (i < N) && (gameOver == false); i++)
        {
            int row = int.Parse(Console.ReadLine());
            string move1 = Console.ReadLine();
            string move2 = Console.ReadLine();
            string move3 = Console.ReadLine();
            //int currentScore = 
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
}