using System;
using System.Collections.Generic;

namespace ArgumentsParser.Marshalers
{
    public class BooleanArgumentMarshaler : IArgumentMarshaler
    {
        private bool booleanValue = false;

        public void Set(IEnumerator<string> currentArgument)
        {
            booleanValue = true;
        }

        public static bool GetValue(IArgumentMarshaler am)
        {
            if (am != null && am is BooleanArgumentMarshaler)
            {
                return ((BooleanArgumentMarshaler)am).booleanValue;
            }
            else
            {
                return false;
            }
        }
    }
}