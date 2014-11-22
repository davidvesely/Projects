using System;
using System.IO;
class Bittris2
{
    static int result;
    static int[] numbers = new int[4];
    static int currNumber;
    static int numberInputs;
    static bool ifMoved = false;

    static void Main()
    {
        if (Environment.CurrentDirectory.ToLower().EndsWith("bin\\debug"))
        {
            Console.SetIn(new StreamReader("input.txt"));
        }

        numberInputs = int.Parse(Console.ReadLine());
        for (int i = 1; i <= numberInputs / 4; i++)
        {
            FullRound();
        }
        Console.WriteLine(result);
    }

    static bool MoveDown(int shift, int i)
    {
        bool moveAvailable = true;
        if ((shift == 1) && ((numbers[i - 1] & 1 << 7) != 0)) shift = 0;
        if ((shift == -1) && ((numbers[i - 1] & 1) != 0)) shift = 0;
        if (shift == 1) numbers[i - 1] = numbers[i - 1] << 1;
        if (shift == -1) numbers[i - 1] = numbers[i - 1] >> 1;
        for (int h = 0; h < 8; h++)
        {
            if (((numbers[i - 1] & 1 << h) & (numbers[i] & 1 << h)) != 0) moveAvailable = false;
        }
        if (moveAvailable)
        {
            numbers[i] |= numbers[i - 1];
            numbers[i - 1] = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    static void FullRound()
    {
        if (numbers[0] == 0)
        {
            bool tryN = false;
            while (!tryN)
            {
                tryN = int.TryParse(Console.ReadLine(), out currNumber);
            }
            numbers[0] = currNumber;
            DrawPlayfield();
            for (int row = 1; row <= 3; row++)
            {
                char moveDirection = Convert.ToChar(Console.ReadLine());
                int moveShift = 0;
                if (moveDirection == 'R') moveShift = -1;
                else if (moveDirection == 'L') moveShift = 1;
                ifMoved = MoveDown(moveShift, row);
                int checkerFull = 0;
                if (ifMoved)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if ((numbers[row] & 1 << i) != 0) checkerFull++;
                    }
                    if (checkerFull == 8)
                    {
                        numbers[row] = 0;
                        for (int i = 0; i < 32; i++)
                        {
                            if ((currNumber & 1 << i) != 0) result += 10;
                        }
                        break;
                    }
                }
                DrawPlayfield();
                if (((checkerFull != 8) && !ifMoved) || row == 3)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if ((currNumber & 1 << i) != 0) result += 1;
                    }
                    break;
                }
            }
        }
    }

    static void DrawPlayfield()
    {
        Console.WriteLine();
        Console.WriteLine(Convert.ToString(numbers[0], 2).PadLeft(8, '0'));
        Console.WriteLine(Convert.ToString(numbers[1], 2).PadLeft(8, '0'));
        Console.WriteLine(Convert.ToString(numbers[2], 2).PadLeft(8, '0'));
        Console.WriteLine(Convert.ToString(numbers[3], 2).PadLeft(8, '0'));
    }
}