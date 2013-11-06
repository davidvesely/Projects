using System;
using System.Threading;

class FallingRocks
{
    struct Coordinates
    {
        public int X, Y;
        public char Symbol;
        public ConsoleColor Color;
        public Coordinates(int x, int y, char Symbol, ConsoleColor Color)
        {
            this.X = x;
            this.Y = y;
            this.Symbol = Symbol;
            this.Color = Color;
        }
    }

    static void Main()
    {
        int rockCount;  //number of rocks
        int sleepTime; //speed difficulty
        int score = 0;
        string label; //used to see the lenght of the text so it can be used for positioning

        Console.CursorVisible = false;
        Console.BufferHeight = Console.WindowHeight; //removing the scrolling of the console

        while (true) //checking if we want to play again
        {
            Console.Clear();
            while (true) //running the game
            {
                label = "Please choose difficulty:";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 2) / 2);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(label);

                label = "Press 1 for easy.";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 2) / 2 + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(label);

                label = "Press 2 for medium.";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 2) / 2 + 2);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(label);

                label = "Press 3 for hard.";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 2) / 2 + 3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(label);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo menuChoice = Console.ReadKey();
                    if (menuChoice.Key == ConsoleKey.D1)
                    {
                        rockCount = 30;
                        sleepTime = 150;
                        break;
                    }
                    if (menuChoice.Key == ConsoleKey.D2)
                    {
                        rockCount = 60;
                        sleepTime = 100;
                        break;
                    }
                    if (menuChoice.Key == ConsoleKey.D3)
                    {
                        rockCount = 90;
                        sleepTime = 75;
                        break;
                    }
                }
            }
            Console.Clear();
            score = game(rockCount, sleepTime); //running the game
            while (true)
            {
                label = "GAME OVER";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 3) / 2);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(label);

                switch (sleepTime)
                {
                    case 150:
                        label = "Your score is " + score + " on Easy difficulty";
                        break;
                    case 100:
                        label = "Your score is " + score + " on Medium difficulty";
                        break;
                    case 75:
                        label = "Your score is " + score + " on Hard difficulty";
                        break;
                    default:
                        break;
                }
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 3) / 2 + 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(label);

                label = "Do you want to play again?";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 3) / 2 + 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(label);

                label = "Press 1 for YES or 2 for NO.\n";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (label.Length / 2), (Console.WindowHeight - 3) / 2 + 3);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(label);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo menuChoice = Console.ReadKey();
                    if (menuChoice.Key == ConsoleKey.D1)
                    {
                        break;
                    }
                    if (menuChoice.Key == ConsoleKey.D2)
                    {
                        return;
                    }
                }
            }
        }
    }

    static int game(int rockCount, int sleepTime)
    {
        char[] rockSymbols = new char[10] { '#', '+', '&', '!', '=', '$', '~', '^', '?', '%' }; // symbols for the rocks, can be changed
        sbyte slowRocks = 1;
        double score = 0;
        int finalScore = 0;
        Random randomGenerator = new Random();
        Coordinates[] rocks = new Coordinates[rockCount];

        Coordinates dwarf = new Coordinates();
        dwarf.X = Console.WindowWidth / 2;
        dwarf.Y = Console.WindowHeight - 1;

        for (int i = 0; i < rockCount; i++) //generating random values for the coordinates,the color and the symbol of the rocks
        {
            rocks[i].X = randomGenerator.Next(1, (Console.WindowWidth - 1));
            rocks[i].Y = randomGenerator.Next(1, (Console.WindowHeight - 5));
            rocks[i].Color = (ConsoleColor)randomGenerator.Next(10, 16);
            rocks[i].Symbol = rockSymbols[randomGenerator.Next(1, rockSymbols.Length)];
        }

        while (true)
        {
            if (slowRocks > 0) //making sure the dwarf moves twice faster than the rocks
            {
                for (int i = 0; i < rockCount; i++)
                {
                    if (rocks[i].Y == (Console.WindowHeight - 1))
                    {
                        Console.SetCursorPosition(rocks[i].X, rocks[i].Y);
                        Console.Write(" ");
                        rocks[i].X = randomGenerator.Next(1, (Console.WindowWidth - 1));
                        rocks[i].Y = randomGenerator.Next(0, 3);
                        rocks[i].Color = (ConsoleColor)randomGenerator.Next(10, 16); //changing the color of the "new" rock
                        rocks[i].Symbol = rockSymbols[randomGenerator.Next(1, rockSymbols.Length)]; //changing the symbol of the "new" rock
                    }
                    else
                    {
                        Console.SetCursorPosition(rocks[i].X, rocks[i].Y);
                        Console.Write(" ");
                        rocks[i].Y++;
                    }
                    Console.SetCursorPosition(rocks[i].X, rocks[i].Y);
                    Console.ForegroundColor = rocks[i].Color;
                    Console.Write(rocks[i].Symbol);
                }
            }

            if (Console.KeyAvailable)
            {
                Console.SetCursorPosition(dwarf.X - 1, dwarf.Y);
                Console.Write(" " + " " + " "); // 3x" " because of bugged copy paste from the site.
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                if (pressedKey.Key == ConsoleKey.RightArrow) dwarf.X++;
                if (pressedKey.Key == ConsoleKey.LeftArrow) dwarf.X--;
            }

            //Checks if the dwarf is out of the console
            if ((dwarf.X == Console.WindowWidth - 2) || (dwarf.X < 1))
            {
                return (finalScore * 10);
            }

            //Checks if there is a collision with the rocks
            for (int i = 0; i < rockCount; i++)
            {
                if (((dwarf.X == rocks[i].X) && (dwarf.Y == rocks[i].Y)) ||
                    ((dwarf.X == rocks[i].X - 1) && (dwarf.Y == rocks[i].Y)) ||
                    ((dwarf.X == rocks[i].X + 1) && (dwarf.Y == rocks[i].Y)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(dwarf.X - 1, dwarf.Y);
                    Console.Write("\\@/");
                    return (finalScore * 10);
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(dwarf.X - 1, dwarf.Y);
            Console.Write("\\@/");

            Thread.Sleep(sleepTime);
            slowRocks *= -1;
            score += 0.1;
            finalScore = (int)score;
        }
    }
}