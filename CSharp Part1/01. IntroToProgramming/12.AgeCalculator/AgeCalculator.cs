// Write a program to read your age from the console
// and print how old you will be after 10 years

using System;

class AgeCalculator
{
    static void Main()
    {
        int age = 0;
        do
        {
            Console.Write("Please enter your age: ");
            string ageString = Console.ReadLine();
            int.TryParse(ageString, out age);
        } while (age <= 0);
        
        // Update the age
        int yearPeriod = 10;
        DateTime birthDate = DateTime.Now.AddYears(age * -1);
        DateTime futureDate = DateTime.Now.AddYears(yearPeriod);
        int futureAge = futureDate.Year - birthDate.Year;

        Console.WriteLine("After {0} years, you will be {1} years old.", yearPeriod, futureAge);
    }
}
