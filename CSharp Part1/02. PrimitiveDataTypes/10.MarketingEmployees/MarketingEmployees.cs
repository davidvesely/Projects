/* 
 * A marketing firm wants to keep record of its employees. Each
 * record would have the following characteristics – first name,
 * family name, age, gender (m or f), ID number, unique employee
 * number (27560000 to 27569999). Declare the variables needed to
 * keep the information for a single employee using appropriate
 * data types and descriptive names.
 */

namespace PrimitiveDataTypes
{
    using System;

    class MarketingEmployees
    {
        static void Main()
        {
            // Declaring variables
            string firstName, familyName;
            byte age;
            char gender;
            int idNumber, employeeNumber;

            // Fill a single record of data
            firstName = "Nikoleta";
            familyName = "Pavlova";
            age = 21;
            gender = 'f';
            idNumber = 20563412;
            employeeNumber = 27569999;

            // Print the data
            Console.WriteLine("********* Employee ********");
            Console.WriteLine("Name: {0} {1}", firstName, familyName);
            Console.WriteLine("Age: {0}, Gender: {1}", age, gender);
            Console.WriteLine("ID Number: {0}", idNumber);
            Console.WriteLine("Employee number: {0}", employeeNumber);
        }
    }
}
