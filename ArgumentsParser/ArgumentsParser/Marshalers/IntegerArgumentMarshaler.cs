using System;
using System.Collections.Generic;

namespace ArgumentsParser.Marshalers
{
    public class IntegerArgumentMarshaler : IArgumentMarshaler
    {
        private int integerValue;

        public void Set(IEnumerator<string> currentArgument)
        {
            string parameter = null;
            try
            {
                currentArgument.MoveNext();
                parameter = currentArgument.Current;
                integerValue = int.Parse(parameter);
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MISSING_INTEGER);
            }
            catch (FormatException)
            {
                throw new ArgsException(ErrorCode.INVALID_INTEGER, parameter);
            }
        }

        public static int GetValue(IArgumentMarshaler am)
        {
            if (am != null && am is IntegerArgumentMarshaler)
            {
                return ((IntegerArgumentMarshaler)am).integerValue;
            }
            else
            {
                return 0;
            }
        }
    }
}