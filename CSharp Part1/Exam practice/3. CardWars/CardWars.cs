using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

class CardWars
{
    private static readonly string[] cards = new string[] { "", "A", "10", "9", "8", "7", "6", "5", "4", "3", "2", "J", "Q", "K" };

    static void MainMine()
    {
        if (Environment.CurrentDirectory.ToLower().EndsWith("bin\\debug"))
        {
            Console.SetIn(new StreamReader("input.txt"));
        }

        BigInteger firstScore = 0, secondScore = 0;
        int firstGameWon = 0, secondGameWon = 0;
        bool firstWinsX = false, secondWinsX = false;

        int gameCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < gameCount; i++)
        {
            int firstCurrentScore = 0, secondCurrentScore = 0;
            int cardZfirst = 0, cardZsecond = 0;
            bool cardXfirst = false, cardXsecond = false;

            for (int j = 0; j < 3; j++)
            {
                string card = Console.ReadLine();
                switch (card)
                {
                    case "Z": cardZfirst++; break;
                    case "Y": firstScore -= 200; break;
                    case "X": cardXfirst = true; break;
                    default: firstCurrentScore += GetCardScore(card); break;
                }
            }

            for (int j = 0; j < 3; j++)
            {
                string card = Console.ReadLine();
                switch (card)
                {
                    case "Z": cardZsecond++; break;
                    case "Y": secondScore -= 200; break;
                    case "X": cardXsecond = true; break;
                    default: secondCurrentScore += GetCardScore(card); break;
                }
            }

            if (cardZfirst > 0)
            {
                firstScore *= (int)Math.Pow(2, cardZfirst);
            }

            if (cardZsecond > 0)
            {
                secondScore *= (int)Math.Pow(2, cardZsecond);
            }

            if (cardXfirst && !cardXsecond)
            {
                firstWinsX = true;
                break;
            }
            else if (!cardXfirst && cardXsecond)
            {
                secondWinsX = true;
                break;
            }
            else if (cardXfirst && cardXsecond)
            {
                firstScore += 50;
                secondScore += 50;
            }

            if (firstCurrentScore > secondCurrentScore)
            {
                firstScore += firstCurrentScore;
                firstGameWon++;
            }
            else if (secondCurrentScore > firstCurrentScore)
            {
                secondScore += secondCurrentScore;
                secondGameWon++;
            }
        }

        if (firstWinsX)
        {
            Console.WriteLine("X card drawn! Player one wins the match!");
        }
        else if (secondWinsX)
        {
            Console.WriteLine("X card drawn! Player two wins the match!");
        }
        else if (firstScore > secondScore)
        {
            Console.WriteLine("First player wins!\nScore: {0}\nGames won: {1}", firstScore, firstGameWon);
        }
        else if (secondScore > firstScore)
        {
            Console.WriteLine("Second player wins!\nScore: {0}\nGames won: {1}", secondScore, secondGameWon);
        }
        else
        {
            Console.WriteLine("It's a tie!\nScore: {0}", firstScore);
        }
    }

    private static int GetCardScore(string card)
    {
        int position = -1;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == card)
            {
                position = i;
                break;
            }
        }

        return position;
    }
}