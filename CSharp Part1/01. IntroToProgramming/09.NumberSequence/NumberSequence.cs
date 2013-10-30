// Write a program that prints the first
// 10 members of the sequence: 2, -3, 4, -5, 6, -7, ...

using System;

class NumberSequence
{
    static void Main()
    {
        for (int i = 2; i <= 10; i++)
        {
            int member = i;

            // In case the number is not even
            if (i % 2 != 0)
            {
                member *= -1;
            }
            
            // Align the sequence of numbers
            Console.WriteLine("{0,2}", member);
        }
    }
}
