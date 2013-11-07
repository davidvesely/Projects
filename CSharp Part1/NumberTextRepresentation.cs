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
            while (true)
            {
                ConvertNumberToTextRepresentation();
            }
        }

        private static void ConvertNumberToTextRepresentation()
        {
            string numberStr;
            int wholeNumber;

            do
            {
                Console.Write("Enter a number in range [0..999]: ");
                numberStr = Console.ReadLine();
            } while (!int.TryParse(numberStr, out wholeNumber) || (wholeNumber < 0) || (wholeNumber > 999));

            // This removes any zeroes in front of the actual number
            numberStr = wholeNumber.ToString();
            string numberTextRepresentation = string.Empty;

            // Check for hundreds
            if (numberStr.Length == 3)
            {
                int digit = int.Parse(numberStr[0].ToString());
                numberTextRepresentation = SingleNumbers[digit] + " hundred";

                // Truncate the hundreds digit
                numberStr = numberStr.Substring(1, 2);
            }

            // Check two-digit numbers
            if (numberStr.Length == 2)
            {
                int number = int.Parse(numberStr);
                // Check if 'and' should be added
                if (wholeNumber >= 100)
                {
                    if ((1 <= number) && (number <= 19))
                    {
                        numberTextRepresentation += " and ";
                    }
                    else
                    {
                        numberTextRepresentation += " ";
                    }
                }

                if ((1 <= number) && (number <= 9))
                {
                    numberTextRepresentation += SingleNumbers[number];
                }
                else if ((10 <= number) && (number <= 19))
                {
                    // convert the number to index of TeenNumbers array
                    number -= 10;
                    numberTextRepresentation += TeenNumbers[number];
                }
                else // between 20 and 99
                {
                    // Convert the number to index from array DecimalArray
                    number /= 10;
                    number -= 2;
                    numberTextRepresentation += DecimalNumbers[number];
                    numberStr = numberStr.Substring(1, 1);
                }
            }

            if (numberStr.Length == 1)
            {
                int number = int.Parse(numberStr);
                if ((wholeNumber > 10) && (number > 0))
                {
                    numberTextRepresentation += " ";
                }

                numberTextRepresentation += SingleNumbers[number];
            }

            Console.WriteLine("{0} -> {1}\n", wholeNumber, numberTextRepresentation);
        }
    }
}
