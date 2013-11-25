using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

class MathExpression
{
    static void Main()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        double N = double.Parse(Console.ReadLine());
        double M = double.Parse(Console.ReadLine());
        double P = double.Parse(Console.ReadLine());

        double formulaCalculation = N * N + (1d / (M * P)) + 1337d;
        formulaCalculation /= (N - 128.523123123d * P);
        int intm = ((int)M) % 180;
        formulaCalculation += Math.Sin(intm);

        Console.WriteLine("{0:F6}", formulaCalculation);
    }
}