using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Sevenland
{
    static void Main()
    {
        int number = int.Parse(Console.ReadLine());
        int numberDecimal = FromBase(number, 7);
        numberDecimal++;
        Console.WriteLine(ToBase(numberDecimal, 7));
    }

    private static int ToBase(int number, int baseNum)
    {
        StringBuilder sb = new StringBuilder();
        while (number > 0)
        {
            int digit = number % baseNum;
            sb.Append(digit);
            number /= baseNum;
        }
        sb.Reverse();
        if (sb.Length == 0)
        {
            sb.Append(0);
        }

        return int.Parse(sb.ToString());
    }

    private static int FromBase(int number, int baseNum)
    {
        int power = 0;
        int sum = 0;
        while (number > 0)
        {
            int digit = number % 10;
            sum += (int)Math.Pow(baseNum, power) * digit;

            power++;
            number /= 10;
        }

        return sum;
    }

    public static void Reverse(this StringBuilder text)
    {
        if (text.Length > 1)
        {
            int pivotPos = text.Length / 2;
            for (int i = 0; i < pivotPos; i++)
            {
                int iRight = text.Length - (i + 1);
                char rightChar = text[i];
                char leftChar = text[iRight];
                text[i] = leftChar;
                text[iRight] = rightChar;
            }
        }
    }

    public static string Reverse(this string text)
    {
        StringBuilder temp = new StringBuilder(text);
        temp.Reverse();
        return temp.ToString();
    }

}