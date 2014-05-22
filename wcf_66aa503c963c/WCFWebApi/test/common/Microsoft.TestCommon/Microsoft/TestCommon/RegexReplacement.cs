// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using System.Text.RegularExpressions;

    public class RegexReplacement
    {
        Regex regex;
        string replacement;

        public RegexReplacement(Regex regex, string replacement)
        {
            this.regex = regex;
            this.replacement = replacement;
        }

        public RegexReplacement(string regex, string replacement)
        {
            this.regex = new Regex(regex);
            this.replacement = replacement;
        }

        public Regex Regex
        {
            get
            {
                return this.regex;
            }
        }

        public string Replacement
        {
            get
            {
                return this.replacement;
            }
        }
    }
}
