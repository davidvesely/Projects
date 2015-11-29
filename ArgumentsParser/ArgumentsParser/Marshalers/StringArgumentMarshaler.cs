using System;
using System.Collections.Generic;

namespace ArgumentsParser.Marshalers
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        private string stringValue = string.Empty;

        public void Set(IEnumerator<string> currentArgument)
        {
            try
            {
                currentArgument.MoveNext();
                stringValue = currentArgument.Current;
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MISSING_STRING);
            }
        }

        public static string GetValue(IArgumentMarshaler am)
        {
            if (am != null && am is StringArgumentMarshaler)
            {
                return ((StringArgumentMarshaler)am).stringValue;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}