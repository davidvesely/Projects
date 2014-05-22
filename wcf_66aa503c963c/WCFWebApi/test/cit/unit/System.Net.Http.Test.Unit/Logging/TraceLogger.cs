using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace System.Net.Test.Common.Logging
{
    internal class TraceLogger : BaseLogger
    {
        public override void WriteLine(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Trace.TraceInformation(message);
                    break;
                case LogLevel.Warn:
                    Trace.TraceWarning(message);
                    break;
                case LogLevel.Error:
                    Trace.TraceError(message);
                    break;
                default:
                    throw new InvalidOperationException(TestUtils.Format("Unknown LogLevel value '{0}'",
                        Enum.GetName(typeof(LogLevel), level)));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(typeof(TraceLogger).ToString());

            TraceListenerCollection listeners = Trace.Listeners;

            foreach (TraceListener listener in listeners)
            {
                sb.Append("  ");
                sb.Append(listener.GetType().ToString());
                if (!string.IsNullOrEmpty(listener.Name))
                {
                    sb.Append(": ");
                    sb.AppendLine(listener.Name);
                }
            }

            return sb.ToString();
        }
    }
}
