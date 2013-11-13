using System;
using System.Collections.Generic;

class Program
{
    static void Main2()
    {
        string input = Console.ReadLine();
        List<UInt64> result = new List<UInt64>();
        while (input != "-1")
        {
            if (input == "0")
            {
                result.Add(0);
                input = Console.ReadLine();
            }
            else
            {
                uint inputNumber = uint.Parse(input);
                char[] binaryNum = Convert.ToString(inputNumber, 2).PadLeft(32, '0').ToCharArray();
                string binaryNumToString = new string(binaryNum);
                int start = binaryNumToString.IndexOf('1');
                int end = binaryNumToString.LastIndexOf('1');

                char[] neuronNum = binaryNumToString.ToCharArray();
                for (int i = start; i <= end; i++)
                {
                    neuronNum[i] = neuronNum[i] == '1' ? '0' : '1';
                }
                string output = new string(neuronNum);
                uint resultNumber = Convert.ToUInt32(new string(neuronNum), 2);
                result.Add(resultNumber);
                input = Console.ReadLine();
            }
        }
        foreach (var number in result)
        {
            Console.WriteLine(number);
        }
    }
}