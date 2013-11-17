/*
 * Write a program that prints all possible cards from a standard deck
 * of 52 cards (without jokers). The cards should be printed with their
 * English names. Use nested for loops and switch-case
 */

using System;
using System.Collections.Generic;
using System.Linq;

class DeckOfCards
{
    private static string[] CardRanks = { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven",
                                     "Eight", "Nine", "Ten", "Jack", "Queen", "King" };
    private static string[] CardColors = { "Clubs", "Diamonds", "Hearts", "Spades" };

    static void Main()
    {
        foreach (string color in CardColors)
        {
            foreach (string rank in CardRanks)
            {
                Console.WriteLine("{0} of {1}", rank, color);
            }
        }
    }
}