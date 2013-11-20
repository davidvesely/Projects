using System;
using System.Linq;

class BatGoikoTower
{
    static void Main()
    {
        int height = Math.Abs(int.Parse(Console.ReadLine()));
        int crossbeamHeight = 0;
        int crossbeamCurrent = 0;
        for (int i = 0; i < height; i++)
        {
            string part1 = new string('.', height - i - 1);
            char part2char;
            if (crossbeamCurrent == crossbeamHeight)
            {
                part2char = '-';
                crossbeamHeight++;
                crossbeamCurrent = 0;
            }
            else
            {
                part2char = '.';
            }
            crossbeamCurrent++;
            string part2 = new string(part2char, i);
            Console.WriteLine("{0}/{1}{2}\\{3}", part1, part2, part2, part1);
        }
    }
}