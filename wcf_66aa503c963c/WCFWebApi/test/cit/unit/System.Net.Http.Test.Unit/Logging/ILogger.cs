using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Test.Common.Logging
{
    public interface ILogger
    {
        void WriteLine(LogLevel level, string message);
        void DumpException(Exception e);
    }
}
