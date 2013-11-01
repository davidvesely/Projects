/* 
 * Declare two string variables and assign them with “Hello” and
 * “World”. Declare an object variable and assign it with the
 * concatenation of the first two variables (mind adding an interval).
 * Declare a third string variable and initialize it with the value of
 * the object variable (you should perform type casting).
 */

namespace PrimitiveDataTypes
{
    using System;

    class StringConcatenation
    {
        static void Main()
        {
            string word1 = "Hello";
            string word2 = "World";
            object concatenated = word1 + " " + word2;
            string sentence = (string)concatenated;

            Console.WriteLine(sentence);
        }
    }
}
