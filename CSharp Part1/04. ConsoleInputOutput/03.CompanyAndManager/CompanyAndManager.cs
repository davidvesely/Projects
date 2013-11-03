/*
 * A company has name, address, phone number, fax number, web site
 * and manager. The manager has first name, last name, age and a
 * phone number. Write a program that reads the information about
 * a company and its manager and prints them on the console
 */

using System;

namespace ConsoleInputOutput
{
    class CompanyAndManager
    {
        static void Main()
        {
            // Company info
            Console.WriteLine("Enter the company information:");
            Console.Write("Name: ");
            string companyName = Console.ReadLine();
            Console.Write("Address: ");
            string companyAddress = Console.ReadLine();
            Console.Write("Phone number: ");
            string companyPhone = Console.ReadLine();
            Console.Write("Fax number: ");
            string companyFax = Console.ReadLine();
            Console.WriteLine();
            // Manager info
            Console.WriteLine("Enter the manager information:");
            Console.Write("First name: ");
            string managerFirstName = Console.ReadLine();
            Console.Write("Last name: ");
            string managerLastName = Console.ReadLine();
            Console.Write("Age: ");
            byte managerAge = byte.Parse(Console.ReadLine());
            Console.Write("Phone number: ");
            string managerPhone = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("|-------------------------------------|");
            Console.WriteLine("| There is the info you have entered: |");
            Console.WriteLine("|-------------------------------------|");
            Console.WriteLine("Company name: {0}", companyName);
            Console.WriteLine("Address: {0}, Phone: {1}, Fax: {2}",
                companyAddress, companyPhone, companyFax);
            Console.WriteLine();
            Console.WriteLine("Manager: {0} {1}", managerFirstName, managerLastName);
            Console.WriteLine("Age: {0}, Phone: {1}", managerAge, managerPhone);
        }
    }
}
