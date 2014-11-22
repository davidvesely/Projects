using System;
using System.Collections.Generic;
using System.Linq;

class NeuronMapping
{
    static void Main()
    {
        List<uint> grid = new List<uint>();
        string input;
        while ((input = Console.ReadLine()) != "-1")
        {
            grid.Add(uint.Parse(input));
        }

        List<uint> outputGrid = new List<uint>(grid.Count);
        foreach (uint number in grid)
        {
            int indexStart = -1, indexEnd = -1;
            for (int i = 0; i < 31; i++)
            {
                bool bitCurrent = GetBit(number, i);
                bool bitNext = GetBit(number, i + 1);

                // for continious 1 bits
                if ((indexStart >= 0) && (indexEnd < 0) && bitCurrent && bitNext)
                {
                    indexStart++;
                }

                // Find the start
                if ((indexStart < 0) && (bitCurrent && !bitNext))
                {
                    indexStart = i + 1;
                    continue;
                }

                // Find the end
                if ((indexStart >= 0) && (!bitCurrent && bitNext))
                {
                    indexEnd = i;
                }
            }

            if (indexStart >= 0 && indexEnd >= 0 && (indexStart <= indexEnd))
            {
                uint neuronRow = 0;
                for (int i = indexStart; i <= indexEnd; i++)
                {
                    neuronRow = SetBit(neuronRow, i, true);
                }
                outputGrid.Add(neuronRow);
            }
            else
            {
                outputGrid.Add(0);
            }
        }

        foreach (uint num in outputGrid)
        {
            //string numStr = Convert.ToString(num, 2).PadLeft(32, '0').Replace('0', '.');
            Console.WriteLine(num);
            //Console.WriteLine(numStr);
        }
    }

    private static bool GetBit(uint number, int position)
    {
        uint mask = 1;
        mask = mask << position;
        return ((number & mask) != 0);
    }

    private static uint SetBit(uint number, int position, bool bitValue)
    {
        if (bitValue)
        {
            uint mask = 1;
            mask = mask << position;
            number = number | mask;
        }
        else
        {
            uint mask = 1;
            mask = ~(mask << position);
            number = number & mask;
        }

        return number;
    }
}
