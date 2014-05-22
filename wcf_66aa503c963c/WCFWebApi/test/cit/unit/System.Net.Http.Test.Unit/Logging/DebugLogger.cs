using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace System.Net.Test.Common.Logging
{
    internal class DebugLogger : BaseLogger
    {
        public override void WriteLine(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Debug.WriteLine(TestUtils.Format("INFO: {0}", message));
                    break;
                case LogLevel.Warn:
                    Debug.WriteLine(TestUtils.Format("WARN: {0}", message));
                    break;
                case LogLevel.Error:
                    Debug.WriteLine(TestUtils.Format("ERROR: {0}", message));
                    break;
                default:
                    throw new InvalidOperationException(TestUtils.Format("Unknown LogLevel value '{0}'", 
                        Enum.GetName(typeof(LogLevel), level)));
            }
        }
    }
}
