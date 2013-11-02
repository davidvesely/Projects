/* 
 * A bank account has a holder name (first name, middle name and
 * last name), available amount of money (balance), bank name, IBAN,
 * BIC code and 3 credit card numbers associated with the account.
 * Declare the variables needed to keep the information for a single
 * bank account using the appropriate data types and descriptive names.
 */
namespace PrimitiveDataTypes
{
    using System;

    class BankAccount
    {
        private static void Main()
        {
            string holderFirstName = "Pesho";
            string holderMiddleName = "Ivanov";
            string holderLastName = "Goshev";
            decimal ballance = 12000;
            string bankName = "Fibank";
            string IBAN = null;
            string BIC = null;
            long creditCard1 = 1234567887654321;
            long creditCard2 = 1111222233334444;
            long creditCard3 = 5555666677778888;

            Console.WriteLine("{0} {1} {2}", creditCard1, creditCard2, creditCard3);
        }
    }
}
