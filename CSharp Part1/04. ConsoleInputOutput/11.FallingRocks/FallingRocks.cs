using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGames
{
    class Rock
    {
        public int x, y;
        public char symbol;
        public ConsoleColor color;
    }

    class FallingRocks
    {
        private static char[] RockTypes = { '^', '@', '*', '&', '+', '%',
                                           '$', '#', '!', '.', ';', '-' };

        private static ConsoleColor[] RockColors = 
        {
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Magenta,
            ConsoleColor.Red,
            ConsoleColor.White,
            ConsoleColor.Yellow
        };

        private const int rockDensityAtRow = 4;
        private const int Rows = 30;
        private const int Cols = 80;
        private static List<Rock> Rocks;
        private static int DwarfPositon;
        private static string Dwarf = "(@)";
        private static int Score = 0;

        static void Main()
        {
            InitializeGame();

            while (true)
            {
                MoveDwarf();
                GenerateRocks();
                MoveRocks();
                CollisionDetect();
                Score++;
                DrawGrid();
                Thread.Sleep(150);
            }
        }

        private static void InitializeGame()
        {
            Rocks = new List<Rock>();

            Console.Clear();
            Console.SetWindowSize(Cols, Rows);
            Console.BufferWidth = Cols;
            Console.BufferHeight = Rows;
            Console.CursorVisible = false;

            // Center the dwarf
            DwarfPositon = Cols / 2;
        }

        private static void MoveDwarf()
        {
            if (Console.KeyAvailable)
            {
                // Read the first pressed key
                ConsoleKeyInfo keyPressed = Console.ReadKey(true);

                switch (keyPressed.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (DwarfPositon > Dwarf.Length / 2)
                            DwarfPositon--;
                        break;
                    case ConsoleKey.RightArrow:
                        int maxRightPos = (Console.BufferWidth - 1) - Dwarf.Length / 2;
                        if (DwarfPositon < maxRightPos)
                            DwarfPositon++;
                        break;
                    case ConsoleKey.Escape:
                        Console.WriteLine();
                        Console.WriteLine("Good game! See you later.");
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }

            // Flush the buffer with keys
            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        private static void GenerateRocks()
        {
            Random rand = new Random();
            int count = rand.Next(rockDensityAtRow + 1);
            for (int i = 0; i < count; i++)
            {
                int position = rand.Next(Cols);
                int type = rand.Next(RockTypes.Length);

                Rock rock = new Rock();
                rock.color = RockColors[rand.Next(RockColors.Length)];
                rock.symbol = RockTypes[type];
                rock.x = position;
                rock.y = 0;
                Rocks.Add(rock);
            }
        }

        private static void MoveRocks()
        {
            // Move rocks with 1 row down, and remove
            // the exiting rocks
            for (int i = Rocks.Count - 1; i >= 0; i--)
            {
                // Remove the not needed rocks
                if (Rocks[i].y == Rows - 1)
                {
                    Rocks.Remove(Rocks[i]);
                }
                else
                {
                    Rocks[i].y++;
                }
            }
        }

        private static void CollisionDetect()
        {
            // Check the last row with rocks where collision could appear
            int end;
            if (Rocks.Count < rockDensityAtRow)
            {
                end = Rocks.Count;
            }
            else
            {
                end = rockDensityAtRow;
            }

            for (int i = 0; i < end; i++)
            {
                if ((Rocks[i].y == Rows - 1) && (Rocks[i].x == DwarfPositon))
                {
                    Console.WriteLine();
                    Console.WriteLine("Game over! You scored {0} points\a", Score);
                    Environment.Exit(0);
                }
            }
        }

        private static void DrawGrid()
        {
            Console.Clear();

            // Draw rocks
            foreach (Rock rock in Rocks)
            {
                DrawAt(rock.x, rock.y, rock.symbol, rock.color);
            }

            // Draw dwarf
            if (Dwarf.Length != 3)
                throw new Exception("Dwarf should be three characters long");

            DrawAt(DwarfPositon - 1, Rows - 1, Dwarf[0], ConsoleColor.Green);
            DrawAt(DwarfPositon, Rows - 1, Dwarf[1], ConsoleColor.Green);
            DrawAt(DwarfPositon + 1, Rows - 1, Dwarf[2], ConsoleColor.Green);
        }

        private static void DrawAt(int x, int y, char c,
            ConsoleColor color = ConsoleColor.Gray)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(c);
        }
    }
}
