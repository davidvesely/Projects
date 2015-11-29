using System.Collections;
using System.Collections.Generic;

namespace ArgumentsParser
{
    public interface IArgumentMarshaler
    {
        void Set(IEnumerator<string> currentArgument);
    }
}