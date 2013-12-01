using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class WeAllLoveBits
{
    static void Main()
    {
        int N = int.Parse(Console.ReadLine());
        int[] magicalNumbers = new int[N];
        for (int i = 0; i < N; i++)
        {
            int input = int.Parse(Console.ReadLine());
            int PNew = (input ^ PTild(input)) & PDoubleDot(input);
            magicalNumbers[i] = PNew;
        }

        foreach (int num in magicalNumbers)
        {
            Console.WriteLine(num);
        }
    }

    static int PDoubleDot(int num)
    {
        string numStr = Convert.ToString(num, 2);
        StringBuilder numResult = new StringBuilder();
        for (int i = numStr.Length - 1; i >= 0; i--)
        {
            numResult.Append(numStr[i]);
        }
        int result = Convert.ToInt32(numResult.ToString(), 2);
        return result;
    }

    static int PTild(int num)
    {
        string numStr = Convert.ToString(num, 2);
        StringBuilder numResult = new StringBuilder();
        foreach (char element in numStr)
        {
            switch (element)
            {
                case '0':
                    numResult.Append('1');
                    break;
                case '1':
                    numResult.Append('0');
                    break;
                default:
                    break;
            }
        }
        int result = Convert.ToInt32(numResult.ToString(), 2);
        return result;
    }
}