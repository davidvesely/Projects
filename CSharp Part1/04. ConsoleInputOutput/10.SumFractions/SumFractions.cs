/*
 * Write a program to calculate the sum (with
 * accuracy of 0.001): 1 + 1/2 - 1/3 + 1/4 - 1/5 + ...
 */

using System;

namespace ConsoleInputOutput
{
    class SumFractions
    {
        static void Main()
        {
            const float accuracy = 0.001f;
            float sum = 1, sumPrevious = 1;
            int fractionElement = 1;
            int sign = 1;

            // Calculate the sum until it is accurate enough
            do
            {
                // Save the previous sum for estimating the difference
                sumPrevious = sum;
                fractionElement++;
                float currentFraction = 1f / fractionElement * sign;
                sum += currentFraction;
                // Change the sign every time + - + - + ...
                sign *= -1;
            } while (Math.Abs(sum - sumPrevious) > accuracy);
            Console.WriteLine("The sum is {0}, iterations: {1}", sum, fractionElement - 2);
        }
    }
}
