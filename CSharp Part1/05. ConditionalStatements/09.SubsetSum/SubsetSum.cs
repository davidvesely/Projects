/*
 * We are given 5 integer numbers. Write a program that checks
 * if the sum of some subset of them is 0.
 * Example: 3, -2, 1, 1, 8  1+1-2=0
 */

using System;

class SubsetSum
{
    static void Main()
    {
        const int ElementCount = 5;
        long[] set = new long[ElementCount];
        bool isValid = true;
        Console.WriteLine("Please provide five numbers:");
        for (int i = 0; i < ElementCount; i++)
        {
            Console.Write("Element {0}: ", i + 1);
            isValid &= long.TryParse(Console.ReadLine(), out set[i]);
        }

        // If input is not valid
        if (!isValid)
        {
            Console.WriteLine("Wrong input values");
            return;
        }

        // The count of combinations between the input five variables
        int countSubsets = (int)Math.Pow(2, ElementCount);
        for (int i = 1; i < countSubsets; i++)
        {
            string subset = string.Empty;
            long subsetSum = 0;
            for (int j = 0; j < ElementCount; j++)
            {
                // Each number 'i' in binary format will give us
                // the current combinations, thus all combinations will be checked
                // They are from '00001' to '11111'
                if (GetBit(i, j))
                {
                    subsetSum += set[j];
                    subset += " " + set[j];
                }
            }

            if (subsetSum == 0)
            {
                Console.WriteLine("Subset{0} has sum of 0", subset);
            }
        }
    }

    private static bool GetBit(int number, int position)
    {
        int mask = 1;
        mask = mask << position;
        return ((number & mask) != 0);
    }
}
