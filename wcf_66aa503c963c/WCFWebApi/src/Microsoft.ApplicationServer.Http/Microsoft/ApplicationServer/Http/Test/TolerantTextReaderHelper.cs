// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Globalization;

    internal static class TolerantTextReaderHelper
    {
        internal static readonly string[] EmptyStringArray = new string[0];

        /// <summary>
        /// this method returns one string with special structures to present one autocomplete entry to javascript client
        /// the structure is display-name\nvalue\nreplace-start-position\nfinal-caret-pos\napplicable
        /// 
        /// e.g. if request is: &lt;ab|
        /// then one autocomplete entry might look like: MyElement\nMyElement&gt;&lt;/MyElement&gt;\n-2\n-12\ntrue
        /// in dropdown list, it looks like: MyElement 
        /// after replacement, it looks like: &lt;MyElement&gt;|&lt;/MyElement&gt;
        /// </summary>
        /// <param name="displayName">mandatory, used as &lt;option&gt; name, and also used as sorting key</param>
        /// <param name="value">mandatory, used as &lt;option&gt; value (i.e. for replacement into textbox), and also used as tooltip</param>
        /// <param name="replaceStartPos">mandatory, when provided, this is the relative position to the current-pos in the request meaning where replacement in textbox should start; when omitted, javascript should use its own logic to find replace start position</param>
        /// <param name="finalCaretPos">optional, when provided, this is the relative position to the replace end position meaning where caret should be after replacement and will trigger another autocomplete; when omitted, javascript should do nothing i.e. caret stays at the end of replacement in textbox</param>
        /// <param name="applicable">optional, when provided, this means whether this &lt;option&gt; item should be grayed out; when omitted, javascript should do nothing i.e. the &lt;option&gt; item displays normal</param>
        /// <returns>one string with special structures to present one autocomplete entry to javascript client</returns>
        public static string GetExpectedValue(string displayName, string value, int replaceStartPos, int? finalCaretPos = null, bool? applicable = null)
        {
            // e.g. if request is: <ab|
            // then one autocomplete entry might look like: MyElement\nMyElement></MyElement>\n-2\n-12\ntrue
            // in dropdown list, it looks like: MyElement 
            // after replacement, it looks like: <MyElement>|</MyElement>

            // the structure here is reflected to autocomplete.js!_getAbcFromItem apis
            return string.Format(
                CultureInfo.InvariantCulture, 
                "{0}\n{1}\n{2}\n{3}\n{4}", 
                displayName,
                value,
                replaceStartPos,
                finalCaretPos,
                applicable);
        }
    }
}