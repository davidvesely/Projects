/*
 *  .....# #.....
    ....#. .#....
    ...#.. ..#...
    ..#... ...#..
    .#.... ....#.
    #..... .....#
 
    #..... .....#
    .#.... ....#.
    ..#... ...#..
    ------ ------
    \\\\\\ //////
    .\\\\\ /////.
    ..\\\\ ////..
    ...\\\ ///...
    ....\\ //....
    .....\ /.....
 */

using System;
using System.Linq;

class Fire
{
    static void Main()
    {
        int N = int.Parse(Console.ReadLine());

        for (int i = N/2 - 1; i >= 0; i--)
        {
            string row = new string('.', i) + "#" + new string('.', N/2 - 1 - i);
            Console.WriteLine("{0}{1}", row, new String(row.Reverse().ToArray()));
        }

        for (int i = 0; i < N/4; i++)
        {
            string row = new string('.', i) + "#" + new string('.', N/2 - 1 - i);
            Console.WriteLine("{0}{1}", row, new String(row.Reverse().ToArray()));
        }

        Console.WriteLine(new string('-', N));

        for (int i = 0; i < N/2; i++)
        {
            string row = new string('.', i) + new string('\\', N/2 - i);
            Console.WriteLine("{0}{1}", row, new String(row.Replace('\\', '/').Reverse().ToArray()));
        }
    }
}