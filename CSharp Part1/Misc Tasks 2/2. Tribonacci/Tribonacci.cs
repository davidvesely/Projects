using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class Tribonacci
{
    static void Main()
    {
        BigInteger N1 = BigInteger.Parse(Console.ReadLine());
        BigInteger N2 = BigInteger.Parse(Console.ReadLine());
        BigInteger N3 = BigInteger.Parse(Console.ReadLine());
        BigInteger N4 = 0;
        int nth = int.Parse(Console.ReadLine());
        for (int i = 4; i <= nth; i++)
        {
            N4 = N3 + N2 + N1;
            N1 = N2;
            N2 = N3;
            N3 = N4;
        }

        switch (nth)
        {
            case 1:
                Console.WriteLine(N1);
                break;
            case 2:
                Console.WriteLine(N2);
                break;
            case 3:
                Console.WriteLine(N3);
                break;
            default:
                Console.WriteLine(N4);
                break;
        }
    }
}