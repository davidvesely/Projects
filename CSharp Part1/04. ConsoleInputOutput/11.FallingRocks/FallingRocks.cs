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

        static void Main()
        {
            InitializeGame();

            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();
            do
            {
                Thread.Sleep(120);
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
        }

        private static char[] GenerateRocks(int cols)
        {
            char[] row = new char[cols];
            //row.
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
                row[position] = Rocks[type];
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
        }
    }
}
