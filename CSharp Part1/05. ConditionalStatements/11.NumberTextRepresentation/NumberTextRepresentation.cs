using System;

namespace ConditionalStatements
{
    class NumberTextRepresentation
    {
        static void Main()
        {
            Console.WriteLine("Hello, this is converter of numbers to their text representation.");
            ConvertNumberToTextRepresentation();
        }

        private static void ConvertNumberToTextRepresentation()
        {
            string[] singleNumbers = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] teenNumbers = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] decimalNumbers = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            string numberStr;
            int number, index;

            do
            {
                Console.Write("Enter a number in range [0..999]: ");
                numberStr = Console.ReadLine();
            } while (!int.TryParse(numberStr, out number) || 
                (number < 0) || (number > 999));

            string numberTextRepresentation = string.Empty;

            switch (numberStr.Length)
            {
                case 1: // 0-9
                    index = int.Parse(numberStr[0].ToString());
                    numberTextRepresentation = singleNumbers[index];
                    break;
                case 2: // 10-99
                    if (number <= 19) // 10-19
                    {
                        index = int.Parse(numberStr[1].ToString());
                        numberTextRepresentation = teenNumbers[index];
                    }
                    else // 20-99
                    {
                        int index1 = int.Parse(numberStr[0].ToString());
                        int index2 = int.Parse(numberStr[1].ToString());
                        numberTextRepresentation = decimalNumbers[index1] + " " + singleNumbers[index2];
                    }
                    break;
                case 3: // 100-999
                    index = int.Parse(numberStr[0].ToString());
                    numberTextRepresentation = singleNumbers[index] + " hundred";

                    // 101 - 109, 201-209...
                    if ((numberStr[1] == '0') && (numberStr[2] != '0'))
                    {
                        index = int.Parse(numberStr[2].ToString());
                        numberTextRepresentation += " and " + singleNumbers[index];
                    }
                    else if (numberStr[1] == '1') // 110 - 119, 210-219...
	                {
		                index = int.Parse(numberStr[2].ToString());
                        numberTextRepresentation += " and " + teenNumbers[index];
	                }
                    else if ((numberStr[1] != '0') && (numberStr[2] == '0')) // 120, 130, 140....
	                {
		                index = int.Parse(numberStr[1].ToString());
                        numberTextRepresentation += " and " + decimalNumbers[index];
	                }
                    else if ((numberStr[1] != '0') && (numberStr[2] != '0')) // everything else
	                {
                        int index1 = int.Parse(numberStr[1].ToString());
                        int index2 = int.Parse(numberStr[2].ToString());
                        numberTextRepresentation += " " + decimalNumbers[index1] + " " + singleNumbers[index2];
	                }
                    break;
                default:
                    Console.WriteLine("You've entered incorrect number");
                    break;
            }

            Console.WriteLine("{0} -> {1}\n", number, numberTextRepresentation);
        }
    }
}