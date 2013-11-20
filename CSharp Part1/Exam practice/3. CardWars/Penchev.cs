namespace _3.CardWars
{
    using System;
    using System.Numerics;
    using System.Linq;

    class CardWars
    {
        static void Main()
        {
            checked
            {
                int numberOfGames = int.Parse(Console.ReadLine());
                const byte cardsInGame = 3;

                string[] firstPlayerDrawnCards = new string[cardsInGame];
                string[] secondPlayerDrawnCards = new string[cardsInGame];

                BigInteger firstPlayerScore = 0;
                BigInteger secondPlayerScore = 0;

                byte firstPlayerWins = 0;
                byte secondPlayerWins = 0;

                while (0 < numberOfGames)
                {
                    byte firstPlayerPulledCards = 0;
                    byte secondPlayerPulledCards = 0;
                    int firstPlayerCardsScore = 0;
                    int secondPlayerCardsScore = 0;

                    while (firstPlayerPulledCards < cardsInGame)
                    {
                        firstPlayerDrawnCards[firstPlayerPulledCards] = Console.ReadLine().ToUpper();
                        firstPlayerPulledCards++;
                    }

                    while (secondPlayerPulledCards < cardsInGame)
                    {
                        secondPlayerDrawnCards[secondPlayerPulledCards] = Console.ReadLine().ToUpper();
                        secondPlayerPulledCards++;
                    }

                    // Check if someone has a X card! 
                    if (firstPlayerDrawnCards.Contains("X") && !secondPlayerDrawnCards.Contains("X"))
                    {
                        Console.WriteLine("X card drawn! Player one wins the match!");
                        break;
                    }
                    else if (!firstPlayerDrawnCards.Contains("X") && secondPlayerDrawnCards.Contains("X"))
                    {
                        Console.WriteLine("X card drawn! Player two wins the match!");
                        break;
                    }

                    //Method addingPoints calculates the sum of the currenct hand card points
                    firstPlayerCardsScore = addingPoints(firstPlayerDrawnCards);
                    secondPlayerCardsScore = addingPoints(secondPlayerDrawnCards);

                    if (firstPlayerCardsScore > secondPlayerCardsScore)
                    {
                        firstPlayerScore += firstPlayerCardsScore;
                        firstPlayerWins++;
                    }
                    else if (firstPlayerCardsScore < secondPlayerCardsScore)
                    {
                        secondPlayerScore += secondPlayerCardsScore;
                        secondPlayerWins++;
                    }

                    numberOfGames--;

                    if (numberOfGames == 0)
                    {
                        if (firstPlayerScore > secondPlayerScore)
                        {
                            Console.WriteLine("First player wins!\nScore: {0}\nGames won: {1}", firstPlayerScore, firstPlayerWins);
                        }
                        else if (secondPlayerScore > firstPlayerScore)
                        {
                            Console.WriteLine("Second player wins!\nScore: {0}\nGames won: {1}", secondPlayerScore, secondPlayerWins);
                        }
                        else
                        {
                            Console.WriteLine("It's a tie!\nScore: {0}", firstPlayerScore);
                        }
                    }
                }
            }
        }
        public static int addingPoints(string[] cards)
        {
            int points = 0;
            bool isXcardDrawn = false;
            for (int card = 0; card < 3; card++)
            {
                switch (cards[card])
                {
                    case "2":
                        points += 10;
                        break;
                    case "3":
                        points += 9;
                        break;
                    case "4":
                        points += 8;
                        break;
                    case "5":
                        points += 7;
                        break;
                    case "6":
                        points += 6;
                        break;
                    case "7":
                        points += 5;
                        break;
                    case "8":
                        points += 4;
                        break;
                    case "9":
                        points += 3;
                        break;
                    case "10":
                        points += 2;
                        break;
                    case "A":
                        points += 1;
                        break;
                    case "J":
                        points += 11;
                        break;
                    case "Q":
                        points += 12;
                        break;
                    case "K":
                        points += 13;
                        break;
                    case "Z":
                        points *= 2;
                        break;
                    case "Y":
                        points -= 200;
                        break;
                    case "X":
                        if (!isXcardDrawn)
                        {
                            points += 50;
                            isXcardDrawn = true;
                        }
                        else
                        {
                            points += 0;
                        }
                        break;
                    default:
                        break;
                }
            }

            return points;
        }
    }
}