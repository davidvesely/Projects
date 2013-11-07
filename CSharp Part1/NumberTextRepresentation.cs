using System;

namespace ConditionalStatements
{
    class NumberTextRepresentation
    {
        private static string[] SingleNumbers = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        private static string[] TeenNumbers = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        private static string[] DecimalNumbers = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"};

        static void Main()
        {
            Console.WriteLine("Hello, this is converter of numbers to their text representation.");
            string numberStr;
            int number;

            do
            {
                Console.Write("Enter a number in range [0..999]: ");
                numberStr = Console.ReadLine();
            } while (!int.TryParse(numberStr, out number) || (number < 0) || (number > 999));

            // This removes any zeroes in front of the actual number
            numberStr = number.ToString();
            string numberTextRepresentation = string.Empty;

            // Check for hundreds
            if (numberStr.Length == 3)
            {
                int digit = int.Parse(numberStr[0].ToString());
                numberTextRepresentation = SingleNumbers[digit] + " hundred";
            }



            Console.WriteLine("{0} -> {1}", number, numberTextRepresentation);
        }
    }
}
