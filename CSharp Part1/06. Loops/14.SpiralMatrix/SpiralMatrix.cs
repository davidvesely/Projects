/*
 * Write a program that reads a positive integer number N (N < 20)
 * from console and outputs in the console the numbers 1 ... N numbers arranged as a spiral
 */

using System;

class Program
{
    static void Main()
    {
        Console.Write("N = ");
        int n = int.Parse(Console.ReadLine());

        int[,] spiralMa3X = new int[n, n];
        int pos = 0;
        int count = n;
        int value = -n;
        int sum = -1;

        do
        {
            value = -1 * value / n;
            for (int i = 0; i < count; i++)
            {
                sum += value;
                spiralMa3X[sum / n, sum % n] = pos++;
            }
            value *= n;
            count--;
            for (int i = 0; i < count; i++)
            {
                sum += value;
                spiralMa3X[sum / n, sum % n] = pos++;
            }
        } while (count > 0);

        // Print the generated matrix
        int p = (spiralMa3X.GetLength(0) * spiralMa3X.GetLength(1) - 1).ToString().Length + 1;
        for (int i = 0; i < spiralMa3X.GetLength(0); i++)
        {
            for (int j = 0; j < spiralMa3X.GetLength(1); j++)
            {
                Console.Write(spiralMa3X[i, j].ToString().PadLeft(p, ' '));
            }
            Console.WriteLine();
        }
    }
}