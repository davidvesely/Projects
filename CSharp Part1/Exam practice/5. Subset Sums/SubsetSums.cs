using System;
using System.Collections.Generic;
using System.Linq;

class SubsetSums
{
    static void Main()
    {
        long sumNeeded = long.Parse(Console.ReadLine());
        int elementCount = int.Parse(Console.ReadLine());
        long[] set = new long[elementCount];
        for (int i = 0; i < elementCount; i++)
        {
            set[i] = long.Parse(Console.ReadLine());
        }

        // The count of combinations between the input variables
        int countSubsets = (int)Math.Pow(2, elementCount);
        int countSatisfiedSubsets = 0;
        for (int i = 1; i < countSubsets; i++)
        {
            long subsetSum = 0;
            for (int j = 0; j < elementCount; j++)
            {
                if (GetBit(i, j))
                {
                    subsetSum += set[j];
                }
            }

            if (subsetSum == sumNeeded)
            {
                countSatisfiedSubsets++;
            }
        }

        Console.WriteLine(countSatisfiedSubsets);
    }

    private static bool GetBit(int number, int position)
    {
        int mask = 1;
        mask = mask << position;
        return ((number & mask) != 0);
    }
}