using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace System.Net.Test.Common.Logging
{
    internal abstract class BaseLogger : ILogger
    {
        #region ILogger Members

        public abstract void WriteLine(LogLevel level, string message);

        public void DumpException(Exception e)
        {
            if (e == null)
            {
                WriteLine(LogLevel.Error, "<no exception>");
            }
            else
            {
                Exception temp = e;

                while (temp != null)
                {
                    WriteLine(LogLevel.Error, TestUtils.Format("{0}: {1}", temp.GetType(), temp.Message));

                    if (!string.IsNullOrEmpty(temp.StackTrace))
                    {
                        WriteLine(LogLevel.Error, temp.StackTrace);
                    }

                    temp = temp.InnerException;

                    if (temp != null)
                    {
                        WriteLine(LogLevel.Error, "--- InnerException:");
                    }
                }
            }
        }

        #endregion
    }
}
