using System;
using System.Collections.Generic;

namespace ArgumentsParser.Marshalers
{
    public class StringArrayArgumentMarshaler : IArgumentMarshaler
    {
        public StringArrayArgumentMarshaler()
        {
        }

        public void Set(IEnumerator<string> currentArgument)
        {
            throw new NotImplementedException();
        }

        public static string[] GetValue(IArgumentMarshaler argumentMarshaler)
        {
            throw new NotImplementedException();
        }
    }
}