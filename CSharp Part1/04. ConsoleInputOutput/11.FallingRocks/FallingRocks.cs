using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGames
{
    class FallingRocks
    {
        private static readonly string Rocks = "^@*&+%$#!.;-";
        private static Random Generator = new Random();
        private const int Rows = 33;
        private const int Cols = 80;
        private static List<char[]> Grid;
        private static int DwarfPositon;
        private static string Dwarf = "(0)";

        static void Main()
        {
            InitializeGame();

            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();
            do
            {
                Thread.Sleep(150);
                Grid.Insert(0, GenerateRocks(Cols));
                if (Grid.Count > Rows)
                {
                    Grid.RemoveAt(Grid.Count - 1);
                }

                DrawGrid(Grid);

                if (Console.KeyAvailable)
                {
                    keyPressed = Console.ReadKey(true);
                }
            } while (keyPressed.Key != ConsoleKey.Escape);
        }

        private static void InitializeGame()
        {
            Console.Clear();
            Console.SetWindowSize(Cols, Rows);
            Console.BufferWidth = Cols;
            Console.BufferHeight = Rows;
            Console.CursorVisible = false;

            Grid = new List<char[]>(Rows);
            DwarfPositon = Cols / 2;
        }

        private static char[] GenerateRocks(int cols)
        {
            char[] row = new char[cols];
            // Generate maximum of 5 rocks at row
            int count = Generator.Next(5);
            for (int i = 0; i < count; i++)
            {
                int position = Generator.Next(0, cols - 1);
                int type = Generator.Next(Rocks.Length - 1);
                int size = 1;

                if ((Rocks[type] == '+') || (Rocks[type] == '-'))
                {
                    size = Generator.Next(1, 3);
                }

                // Ordinary rocks
                if (size == 1)
                {
                    row[position] = Rocks[type];
                }
                // Bigger rocks
                else if (size > 1)
                {
                    // Check for index out of range in left
                    if (position > 0)
                        row[position - 1] = Rocks[type];

                    row[position] = Rocks[type];
                    // Check for index out of range in right
                    if (position < (cols - 1))
                        row[position + 1] = Rocks[type];
                }
            }

            return row;
        }

        private static void DrawGrid(List<char[]> grid)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(grid[i]);
            }

            Console.SetCursorPosition(DwarfPositon, Rows - 1);
            Console.Write("{0}", Dwarf);
        }
    }
}
