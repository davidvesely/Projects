using System;
using System.Collections.Generic;
using System.Linq;

class MissCat
{
    static void Main()
    {
        int[] catContestant = new int[10];
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            int catCurrent = int.Parse(Console.ReadLine());
            catContestant[catCurrent - 1]++;
        }

        int catWinner = 10;
        int catMaxVotes = catContestant[9];
        for (int i = 8; i >= 0; i--)
        {
            if (catContestant[i] > catMaxVotes)
            {
                catWinner = i + 1;
                catMaxVotes = catContestant[i];
            }
        }
        Console.WriteLine(catWinner);
    }
}