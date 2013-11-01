/* 
 * Declare two string variables and assign them with following value:
 * 'The "use" of quotations causes difficulties.'
 * Do the above in two different ways: with and without using quoted strings.
 */

namespace PrimitiveDataTypes
{
    using System;

    class QuotedStrings
    {
        static void Main()
        {
            string sentence1 = @"The ""use"" of quotations causes difficulties.";
            string sentence2 = "The \"use\" of quotations causes difficulties.";

            Console.WriteLine(sentence1);
            Console.WriteLine(sentence2);
        }
    }
}
