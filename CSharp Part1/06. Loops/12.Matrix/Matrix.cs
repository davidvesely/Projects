/*
 * Write a program that reads from the console a positive
 * integer number N (N < 20) and outputs a matrix
 */

using System;

class Matrix
{
    static void Main()
    {
        int size;
        Console.Write("Provide size of the matrix: ");
        if (!int.TryParse(Console.ReadLine(), out size) || (size >= 20) || (size < 0))
        {
            Console.WriteLine("Bad input");
            return;
        }

        for (int i = 1; i <= size; i++) // rows
        {
            for (int j = 0; j < size; j++) // cols
            {
                Console.Write("{0,3}", i + j);
            }
            Console.WriteLine();
        }
    }
}