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
                string guesNumber = i.ToString();
                char[] secretNumCh = secretNumber.ToCharArray();
                char[] guessNumCh = guesNumber.ToCharArray();
                if ((guessNumCh[0] == '0') || (guessNumCh[1] == '0') ||
                    (guessNumCh[2] == '0') || (guessNumCh[3] == '0'))
                {
                    continue;
                }

                int currBull = 0, currCow = 0;

                // Check for bulls
                for (int j = 0; j < 4; j++)
                {
                    if (guessNumCh[j] == secretNumCh[j])
                    {
                        // Mark them as bulls
                        guessNumCh[j] = '@';
                        secretNumCh[j] = '@';
                        currBull++;
                    }
                }

                // Check for cows
                for (int j = 0; j < 4; j++)
                {
                    if (guessNumCh[j] != '@')
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if ((secretNumCh[k] != '@') &&
                                (secretNumCh[k] == guessNumCh[j]))
                            {
                                guessNumCh[j] = '@';
                                secretNumCh[k] = '@';
                                currCow++;
                                break;
                            }
                        }
                    }
                }

                if ((currBull == bulls) && (currCow == cows))
                {
                    output.Append(i.ToString());
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
