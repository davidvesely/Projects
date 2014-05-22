// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common
{
    using System;
    ///using Microsoft.Server.Common.Moles;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Asserter pattern that provides test utilities to verify ETW diagnostic tracing
    /// is called for actions taken by the client test code.
    /// </summary>
    public class DiagnosticTraceAssert
    {
        private static DiagnosticTraceAssert singleton = new DiagnosticTraceAssert();

        public static DiagnosticTraceAssert Singleton { get { return singleton; } }

        //// TODO RONCAIN Moles
        /////// <summary>
        /////// Asserts that <paramref name="actionThatTraces"/> causes <see cref="TraceCore.ThrowingException"/>
        /////// is called.
        /////// </summary>
        /////// <param name="actionThatTraces">Some test action that will trigger a trace.</param>
        /////// <param name="traceCallback">Test handler that will be called with the exception that was traced.</param>
        ////public void ThrowingException(Action actionThatTraces, Action<string, Exception> traceCallback)
        ////{
        ////    try
        ////    {
        ////        bool traceReceived = false;

        ////        MTraceCore.ThrowingExceptionEtwDiagnosticTraceStringException = (edt, str, ex) =>
        ////            {
        ////                traceReceived = true;
        ////                traceCallback(str, ex);
        ////            };

        ////        actionThatTraces();

        ////        Assert.IsTrue(traceReceived, "TraceHandledException was not called on EtwDiagnosticTrace");
        ////    }
        ////    finally
        ////    {
        ////        MTraceCore.BehaveAsCurrent();
        ////    }
        ////}

        /////// <summary>
        /////// Asserts that <paramref name="actionThatTraces"/> causes <see cref="TraceCore.HandleExceptionWarning"/>
        /////// is called.
        /////// </summary>
        /////// <param name="actionThatTraces">Some test action that will trigger a trace.</param>
        /////// <param name="traceCallback">Test handler that will be called with the exception that was traced.</param>
        ////public void HandledExceptionWarning(Action actionThatTraces, Action<Exception> traceCallback)
        ////{
        ////    try
        ////    {
        ////        bool traceReceived = false;

        ////        MTraceCore.HandledExceptionWarningEtwDiagnosticTraceException = (edt, ex) =>
        ////        {
        ////            traceReceived = true;
        ////            traceCallback(ex);
        ////        };

        ////        actionThatTraces();

        ////        Assert.IsTrue(traceReceived, "TraceHandledException was not called on EtwDiagnosticTrace");
        ////    }
        ////    finally
        ////    {
        ////        MTraceCore.BehaveAsCurrent();
        ////    }
        ////}

        /////// <summary>
        /////// Asserts that <paramref name="actionThatTraces"/> causes <see cref="TraceCore.HandleException"/>
        /////// is called.
        /////// </summary>
        /////// <param name="actionThatTraces">Some test action that will trigger a trace.</param>
        /////// <param name="traceCallback">Test handler that will be called with the exception that was traced.</param>
        ////public void HandledException(Action actionThatTraces, Action<Exception> traceCallback)
        ////{
        ////    try
        ////    {
        ////        bool traceReceived = false;

        ////        MTraceCore.HandledExceptionEtwDiagnosticTraceException = (edt, ex) =>
        ////        {
        ////            traceReceived = true;
        ////            traceCallback(ex);
        ////        };

        ////        actionThatTraces();

        ////        Assert.IsTrue(traceReceived, "TraceHandledException was not called on EtwDiagnosticTrace");
        ////    }
        ////    finally
        ////    {
        ////        MTraceCore.BehaveAsCurrent();
        ////    }
        ////}
    }
}
