using System;
using System.Collections.Generic;
using System.Linq;

class LeastMajorityMultiple
{
    static void Main()
    {
        int[] numbers = new int[5];
        for (int i = 0; i < 5; i++)
        {
            numbers[i] = int.Parse(Console.ReadLine());
        }

        int countLeastMajority = 0;
        int currentNumber = 0;
        while (countLeastMajority < 3)
        {
            countLeastMajority = 0;
            currentNumber++;
            for (int i = 0; i < 5; i++)
            {
                if (currentNumber % numbers[i] == 0)
                {
                    countLeastMajority++;
                    if (countLeastMajority == 3)
                    {
                        break;
                    }
                }
            }
        }

        Console.WriteLine(currentNumber);
    }
}