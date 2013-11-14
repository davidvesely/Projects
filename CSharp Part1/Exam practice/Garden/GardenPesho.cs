using System;
using System.Linq;

namespace Garden
{
    class GardenPesho
    {
        static void Main()
        {
            int tomatoSeed = int.Parse(Console.ReadLine());
            int tomatoArea = int.Parse(Console.ReadLine());

            int cucumberSeed = int.Parse(Console.ReadLine());
            int cucumberArea = int.Parse(Console.ReadLine());

            int potatoSeed = int.Parse(Console.ReadLine());
            int potatoArea = int.Parse(Console.ReadLine());

            int carrotSeed = int.Parse(Console.ReadLine());
            int carrotArea = int.Parse(Console.ReadLine());

            int cabaggeSeed = int.Parse(Console.ReadLine());
            int cabaggeArea = int.Parse(Console.ReadLine());

            int beansSeed = int.Parse(Console.ReadLine());

            // Total costs: X.XX
            // Beans area: X
            double totalCost = tomatoSeed * 0.5 + carrotSeed * 0.6 + cucumberSeed * 0.4 +
                cabaggeSeed * 0.3 + potatoSeed * 0.25 + beansSeed * 0.4;
            Console.WriteLine("Total costs: {0:F2}", totalCost);

            int totalArea = tomatoArea + cucumberArea + potatoArea + carrotArea + cabaggeArea;
            if (totalArea < 250)
            {
                Console.WriteLine("Beans area: {0}", 250 - totalArea);
            }
            else if (totalArea == 250)
            {
                Console.WriteLine("No area for beans");
            }
            else
            {
                Console.WriteLine("Insufficient area");
            }
        }
    }
}
