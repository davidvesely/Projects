using System;

namespace DrunkenNumbers
{
    class Program
    {
        static void Main()
        {
            int n = Math.Abs(int.Parse(Console.ReadLine()));
            int mitkoBeers = 0, vaskoBeers = 0;

            for (int i = 0; i < n; i++)
            {
                string drunkNumber = Console.ReadLine().TrimStart('0', '-');
                for (int j = 0; j < drunkNumber.Length; j++)
                {
                    if (j < drunkNumber.Length / 2)
                    {
                        mitkoBeers += drunkNumber[j] - '0';
                    }
                    else
                    {
                        vaskoBeers += drunkNumber[j] - '0';
                    }
                }

                if (drunkNumber.Length % 2 == 1)
                {
                    int middleDigit = int.Parse(drunkNumber[drunkNumber.Length / 2].ToString());
                    mitkoBeers += middleDigit;
                }
            }

            // Determine the winner
            if (mitkoBeers > vaskoBeers)
            {
                Console.WriteLine("M {0}", (mitkoBeers - vaskoBeers));
            }
            else if (mitkoBeers < vaskoBeers)
            {
                Console.WriteLine("V {0}", (vaskoBeers - mitkoBeers));
            }
            else
            {
                Console.WriteLine("No {0}", mitkoBeers + vaskoBeers);
            }
        }
    }
}
