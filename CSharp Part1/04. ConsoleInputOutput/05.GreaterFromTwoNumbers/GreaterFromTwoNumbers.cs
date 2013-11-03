/*
 * Write a program that gets two numbers from the console
 * and prints the greater of them. Don’t use if statements
 */

using System;

namespace ConsoleInputOutput
{
    class GreaterFromTwoNumbers
    {
        static void Main()
        {
            // *********** Variant 1 *********************
            int number1, number2;
            Console.Write("Number 1: ");
            int.TryParse(Console.ReadLine(), out number1);
            Console.Write("Number 2: ");
            int.TryParse(Console.ReadLine(), out number2);

            // This looks very weird and $@#$@! :)
            string result = (number1 > number2) ? "greater than" : 
                ((number1 < number2) ? "lower than" : "equal to");
            Console.WriteLine("{0} is {2} {1}", number1, number2, result);

            // *********** Variant 2 *********************
            // Clearer variant, but no equality check
            Console.WriteLine("Greater than these number is {0}",
                Math.Max(number1, number2));
        }
    }
}
