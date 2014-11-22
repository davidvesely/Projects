using System;

namespace CoffeeMachine
{
    class Program
    {
        static void Main()
        {
            // Amount of coins in the machine
            int N1, N2, N3, N4, N5;
            // Input money and price of the drink
            double inMoney, price;

            N1 = int.Parse(Console.ReadLine());
            N2 = int.Parse(Console.ReadLine());
            N3 = int.Parse(Console.ReadLine());
            N4 = int.Parse(Console.ReadLine());
            N5 = int.Parse(Console.ReadLine());
            inMoney = double.Parse(Console.ReadLine());
            price = double.Parse(Console.ReadLine());

            // Calculate the amount of coins in the machine
            double totalAmount = (N1 * 0.05) +
                (N2 * 0.1) + (N3 * 0.2) + (N4 * 0.5) + N5;

            // Enough money, calculate the change
            if (inMoney >= price)
            {
                double difference = inMoney - price;
                if (totalAmount >= difference)
                {
                    double leftMoney = totalAmount - difference;
                    Console.WriteLine("Yes {0:0.00}", leftMoney);
                }
                else
                {
                    double insufficient = difference - totalAmount;
                    Console.WriteLine("No {0:0.00}", insufficient);
                }
            }
            // Not enough money
            else
            {
                double additionalMoney = price - inMoney;
                Console.WriteLine("More {0:0.00}", additionalMoney);
            }
        }
    }
}
