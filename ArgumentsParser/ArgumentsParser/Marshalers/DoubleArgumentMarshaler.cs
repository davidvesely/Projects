using System;
using System.Collections.Generic;

namespace ArgumentsParser.Marshalers
{
    public class DoubleArgumentMarshaler : IArgumentMarshaler
    {
        private double doubleValue;

        public void Set(IEnumerator<string> currentArgument)
        {
            string parameter = null;
            try
            {
                currentArgument.MoveNext();
                parameter = currentArgument.Current;
                doubleValue = double.Parse(parameter);
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MISSING_DOUBLE);
            }
            catch (FormatException)
            {
                throw new ArgsException(ErrorCode.INVALID_DOUBLE, parameter);
            }
        }

        public static double GetValue(IArgumentMarshaler am)
        {
            if (am != null && am is DoubleArgumentMarshaler)
            {
                return ((DoubleArgumentMarshaler)am).doubleValue;
            }
            else
            {
                return 0;
            }
        }
    }
}