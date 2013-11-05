using System;

class Fibonacci
{
    static decimal CalculateFibonacci(int nth)
    {
        if (nth <= 2)
        {
            return 1;
        }
        int temp = nth / 2;
        decimal a = CalculateFibonacci(temp + 1);
        decimal b = CalculateFibonacci(temp);
        decimal next;
        if (nth % 2 == 1)
        {
            next = a * a + b * b;
        }
        else
        {
            next = b * (2 * a - b);
        }
        return next;
    }

    static void Main(string[] args)
    {
        Console.Write("n = ");
        int n = int.Parse(Console.ReadLine());
        decimal nthFibonacciNumber = CalculateFibonacci(n);
        Console.WriteLine(nthFibonacciNumber);
    }
}