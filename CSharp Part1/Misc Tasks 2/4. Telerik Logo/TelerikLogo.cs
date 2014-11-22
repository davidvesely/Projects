using System;
using System.Collections.Generic;
using System.Linq;

class TelerikLogo
{
    static void Main()
    {
        int x = int.Parse(Console.ReadLine());
        int y = x;
        int z = x / 2 + 1;
        int start = z - 1;
        int sideSpace = z - 1;
        // Width and height are supposed to be equal
        int width = 2*z + 2*y - 3;
        int height = 3*x - 2;
        char[,] field = new char[height, width];

        // Initialize field
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                field[i, j] = '.';
            }
        }

        bool hasEnded = false;
        int column = -1, row = start + 1;
        int dirColumn = 1, dirRow = -1;
        while (!hasEnded)
        {
            column += dirColumn;
            row += dirRow;
            field[row, column] = '*';
            //DrawField(field);

            if ((row == 0) || (row == (height - 1)))
            {
                dirRow *= -1;
            }

            if ((row > z) && ((column == (width - sideSpace - 1) || (column == sideSpace))))
            {
                dirColumn *= -1;
            }

            hasEnded = (dirColumn == 1) && (dirRow == 1) && (row == (z - 1)) && (column == (width - 1));
        }
        DrawField(field);
    }

    private static void DrawField(char[,] field)
    {
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                Console.Write(field[i, j]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}