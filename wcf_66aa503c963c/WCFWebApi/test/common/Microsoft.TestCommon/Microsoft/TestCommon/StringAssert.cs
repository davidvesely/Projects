// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
namespace Microsoft.TestCommon
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using MStest = Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// MSTest assert class that provides string specific testing functionality such as
    /// the ability to ignore Byte Order Marks (BOM) and assert failure messages that indicate
    /// how the 'actual' string differs from the 'expected' string.
    /// </summary>
    public class StringAssert
    {
        private static string utf8BOM = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
        private static string utf16BOM = Encoding.BigEndianUnicode.GetString(Encoding.BigEndianUnicode.GetPreamble());
        private static string utf32BOM = Encoding.UTF32.GetString(Encoding.UTF32.GetPreamble());
        private static string[] BOMarray = new string[] { utf8BOM, utf16BOM, utf32BOM };

        private static StringAssert singleton = new StringAssert();

        public static StringAssert Singleton { get { return singleton; } }

        /// <summary>
        /// Asserts that the two strings are equal with the option that the presence or absence
        /// of a Byte Order Mark (BOM) on either string is ignored.
        /// </summary>
        /// <param name="expected">The expected string.</param>
        /// <param name="actual">The actual string.</param>
        /// <param name="ignoreBOM">Indicates if it acceptable to ignore the BOM on either of the strings.</param>
        /// <param name="regexReplacements">The regex replacements to perform on the 'actual' string prior to comparing with the 'expected' string.</param>
        public void AreEqual(string expected, string actual, bool ignoreBOM = false, params RegexReplacement[] regexReplacements)
        {
            if (expected == null)
            {
                if (actual == null)
                {
                    return;
                }

                MStest.Assert.Fail(string.Format("The 'actual' string was expected to be null but was instead: '{0}'.", actual));
            }

            if (actual == null)
            {
                MStest.Assert.Fail(string.Format("The 'actual' string was null but was expected to be: '{0}'.", expected));
            }

            if (regexReplacements != null)
            {
                for (int i = 0; i < regexReplacements.Length; i++)
                {
                    actual = regexReplacements[i].Regex.Replace(actual, regexReplacements[i].Replacement);
                }
            }

            int expectedLength = expected.Length;
            int actualLength = actual.Length;

            if (expectedLength == 0)
            {
                if (actualLength == 0)
                {
                    return;
                }

                MStest.Assert.Fail(string.Format("The 'actual' string was expected to be empty but was instead: '{0}'.", actual));
            }

            if (actualLength == 0)
            {
                MStest.Assert.Fail(string.Format("The 'actual' string was empty but was expected to be: '{0}'.", expected));
            }

            int expectedIndex = 0;
            int actualIndex = 0;
            if (ignoreBOM)
            {
                bool actualBOMfound = false;
                bool expectedBOMfound = false;

                foreach (string BOM in BOMarray)
                {
                    if (!actualBOMfound && actual.StartsWith(BOM))
                    {
                        actualIndex += BOM.Length;
                        actualBOMfound = true;
                    }

                    if (!expectedBOMfound && expected.StartsWith(BOM))
                    {
                        expectedIndex += BOM.Length;
                        expectedBOMfound = true;
                    }
                }
            }

            while (expectedIndex < expectedLength)
            {
                if (actualIndex >= actualLength)
                {
                    MStest.Assert.Fail("The end of the 'actual' string was reached while the 'expected' string still has additional characters to compare.");
                }

                MStest.Assert.AreEqual(expected[expectedIndex], actual[actualIndex], "The 'actual' and 'expected' strings differ starting at index '{0}' of the 'expected' string.", expectedIndex);
                expectedIndex++;
                actualIndex++;
            }

            if (actualIndex + 1 < actualLength)
            {
                MStest.Assert.Fail("The end of the 'expected' string was reached while the 'actual' string still has additional characters to compare.");
            }
        }

        /// <summary>
        /// Verifies that the first string contains the second string.
        /// </summary>
        /// <param name="value">The string that is expected to contain substring.</param>
        /// <param name="substring">The string expected to occur within value.</param>
        public void Contains(string value, string substring)
        {
            MStest.StringAssert.Contains(value, substring);
        }

        /// <summary>
        /// Verifies that the first string contains the second string. Displays a message if the assertion fails.
        /// </summary>
        /// <param name="value">The string that is expected to contain substring.</param>
        /// <param name="substring">The string expected to occur within value.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        public void Contains(string value, string substring, string message)
        {
            MStest.StringAssert.Contains(value, substring, message);
        }

        /// <summary>
        /// Verifies that the first string contains the second string. Displays a message
        /// if the assertion fails, and applies the specified formatting to it.
        /// </summary>
        /// <param name="value">The string that is expected to contain substring.</param>
        /// <param name="substring">The string expected to occur within value.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting message.</param>
        public void Contains(string value, string substring, string message, params object[] parameters)
        {
            MStest.StringAssert.Contains(value, substring, message, parameters);
        }

        /// <summary>
        /// Verifies that the first string ends with the second string.
        /// </summary>
        /// <param name="value">The string that is expected to end with substring.</param>
        /// <param name="substring">The string expected to be a suffix of value.</param>
        public void EndsWith(string value, string substring)
        {
            MStest.StringAssert.EndsWith(value, substring);
        }

        /// <summary>
        /// Verifies that the first string ends with the second string. Displays a message 
        /// if the assertion fails.
        /// </summary>
        /// <param name="value">The string that is expected to end with substring.</param>
        /// <param name="substring">The string expected to be a suffix of value.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in 
        /// the unit test results.</param>
        public void EndsWith(string value, string substring, string message)
        {
            MStest.StringAssert.EndsWith(value, substring, message);
        }

        /// <summary>
        /// Verifies that the first string ends with the second string. Displays a message
        /// if the assertion fails, and applies the specified formatting to it.
        /// </summary>
        /// <param name="value">The string that is expected to end with substring.</param>
        /// <param name="substring">The string expected to be a suffix of value.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in
        /// the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting message.</param>
        public void EndsWith(string value, string substring, string message, params object[] parameters)
        {
            MStest.StringAssert.EndsWith(value, substring, message, parameters);
        }
    }
}