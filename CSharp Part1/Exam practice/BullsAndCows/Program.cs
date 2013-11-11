using System;
using System.Text;

namespace BullsAndCows
{
    class Program
    {
        static void Main()
        {
            StringBuilder output = new StringBuilder();

            string secretNumber = Console.ReadLine();
            int bulls = int.Parse(Console.ReadLine());
            int cows = int.Parse(Console.ReadLine());

            for (int i = 1111; i <= 9999; i++)
            {
                string guessStr = i.ToString();
                bool[] bullsArr = new bool[4];
                int currBull = 0, currCow = 0;

                // Check for bulls
                for (int j = 0; j <= 3; j++)
                {
                    if (guessStr[j] == secretNumber[j])
                    {
                        bullsArr[j] = true;
                        currBull++;
                    }
                }

                // Check for cows
                for (int j = 0; j <= 3; j++)
                {
                    if (bullsArr[j])
                    {
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k <= 3; k++)
                        {
                            if (bullsArr[k])
                            {
                                continue;
                            }
                            else if (secretNumber[j] == guessStr[k])
                            {
                                currCow++;
                                break;
                            }
                        }
                    }
                }

                if ((currBull == bulls) && (currCow == cows) && (!guessStr.Contains("0")))
                {
                    output.Append(guessStr);
                    output.Append(" ");
                }
            }

            if (output.Length > 0)
            {
                output.Remove(output.Length - 1, 1);
                Console.WriteLine(output);
            }
            else
            {
                Console.WriteLine("No");
            }
        }
    }
}
