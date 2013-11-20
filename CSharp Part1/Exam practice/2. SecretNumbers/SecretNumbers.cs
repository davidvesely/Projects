using System;
using System.Collections.Generic;
using System.Linq;

class SecretNumbers
{
    static void Main()
    {
        string inputNumber = Console.ReadLine().TrimStart('-');
        // Calc the special sum
        long specialSum = 0;
        int digitPosCounter = 1;
        for (int i = inputNumber.Length - 1; i >= 0 ; i--)
        {
            int digit = inputNumber[i] - '0';
            if (digitPosCounter % 2 == 0) // Even
            {
                specialSum += digit * digit * digitPosCounter;
            }
            else
            {
                specialSum += digit * digitPosCounter * digitPosCounter;
            }
            digitPosCounter++;
        }
        Console.WriteLine(specialSum);

        // Find the sequence
        int secretSeqLen = (int)specialSum % 10;
        if (secretSeqLen == 0)
        {
            Console.WriteLine("{0} has no secret alpha-sequence", inputNumber);
        }
        else
        {
            int sequenceCounter = (int)specialSum % 26;
            char[] sequence = new char[secretSeqLen];
            for (int i = 0; i < secretSeqLen; i++)
            {
                sequence[i] = (char)('A' + sequenceCounter);
                sequenceCounter++;
                if (sequenceCounter == 26)
                {
                    sequenceCounter = 0;
                }
            }
            Console.WriteLine(sequence);
        }
    }
}