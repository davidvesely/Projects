using System;
using System.Collections.Generic;
using System.Linq;

class FirTree
{
    static void Main()
    {
        int height = int.Parse(Console.ReadLine());
        string firstRow = null;
        for (int i = 0; i < height - 1; i++)
        {
            string dotPart = new string('.', (height + 1) / 2 - i);
            string asteriskPart = new string('*', i);
            string row = string.Format("{0}{1}*{2}{3}", dotPart, asteriskPart, asteriskPart, dotPart);
            if (i == 0)
            {
                firstRow = row;
            }
            Console.WriteLine(row);
        }
        Console.WriteLine(firstRow);
    }
}