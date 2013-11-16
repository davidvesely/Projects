/*
 * Write a program that, depending on the user's choice inputs int,
 * double or string variable. If the variable is integer or double,
 * increases it with 1. If the variable is string, appends "*" at
 * its end. The program must show the value of that variable as a
 * console output. Use switch statement
 */

using System;

class DependingOfInput
{
    static void Main()
    {
        char choice;
        do
        {
            Console.Clear();
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("  1. Integer input");
            Console.WriteLine("  2. Double input");
            Console.WriteLine("  3. String input");
            ConsoleKeyInfo consoleKey = Console.ReadKey(true);
            choice = consoleKey.KeyChar;
        } while ((choice < '1') || (choice > '3'));

        Console.Write("Enter your chosen type variable: ");
        string input = Console.ReadLine();
        string output = string.Empty;
        bool isSuccessful;

        switch (choice)
        {
            case '1':
                int inputI;
                isSuccessful = int.TryParse(input, out inputI);
                if (isSuccessful)
                {
                    inputI++;
                    output = inputI.ToString();
                }
                break;
            case '2':
                double inputD;
                isSuccessful = double.TryParse(input, out inputD);
                if (isSuccessful)
                {
                    inputD++;
                    output = inputD.ToString();
                }
                break;
            case '3':
                output = input + "*";
                isSuccessful = true;
                break;
            default:
                isSuccessful = false;
                break;
        }

        if (isSuccessful)
        {
            Console.WriteLine("Result: {0}", output);
        }
        else
        {
            Console.WriteLine("Wrong input");
        }
    }
}
