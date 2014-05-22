using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Test.Common
{
    public static class ExceptionAssert
    {
        public static void ThrowsObjectDisposed(Action action, string failureText)
        {
            Throws<ObjectDisposedException>(action, failureText);
        }

        public static void ThrowsFormat(Action action, string failureText)
        {            
            Throws<FormatException>(action, failureText);
        }

        public static void ThrowsInvalidOperation(Action action, string failureText)
        {
            Throws<InvalidOperationException>(action, failureText);
        }

        public static void Throws<T>(Action action, string failureText) where T : Exception
        {
            try
            {
                action();
                Assert.Fail("Expected '{0}' but no exception was thrown: {1}", typeof(T).Name, failureText);
            }
            catch (T)
            {
                // This is the expected exception. Just catch it.
            }
            catch (AssertFailedException)
            {
                // If we get an AssertFailedException, let it bubble up.
                throw;
            }
            catch (Exception e)
            {
                // If an unexpected exception was thrown, fail.
                Assert.Fail("Expected '{0}' but '{1}' was thrown: {2}\r\nException: {3}", typeof(T).Name,
                    e.GetType().Name, failureText, e);
            }
        }
    }
}
