using System;
using System.Collections.Generic;
using System.Linq;

class BinaryDigitsCount
{
    static void Main()
    {
        char binaryDigit = Console.ReadLine()[0];
        int numberCount = int.Parse(Console.ReadLine());
        int[] counter = new int[numberCount];
        for (int i = 0; i < numberCount; i++)
        {
            uint number = uint.Parse(Console.ReadLine());
            string binary = Convert.ToString(number, 2);
            for (int j = 0; j < binary.Length; j++)
            {
                if (binaryDigit == binary[j])
                {
                    counter[i]++;
                }
            }
        }

        foreach (int count in counter)
        {
            Console.WriteLine(count);
        }
    }
}