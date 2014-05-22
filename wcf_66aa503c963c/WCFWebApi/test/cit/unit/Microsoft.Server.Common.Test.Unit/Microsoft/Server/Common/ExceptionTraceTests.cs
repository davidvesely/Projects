// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading;
    ////using Microsoft.Server.Common.Diagnostics.Moles;
    ////using Microsoft.Server.Common.Moles;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExceptionTraceTests : UnitTest<ExceptionTrace>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExceptionTrace is public, concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors


        #endregion Constructors

        #region Methods

        #region AsError(Exception)

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception) traces and returns the exception.")]
        ////public void AsErrorTracesAndReturnsException()
        ////{
        ////    InvalidOperationException exception = new InvalidOperationException();
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(exception, new ExceptionTrace().AsError(exception), "The exception was not traced"); },
        ////        (eventSource, ex) => { Assert.AreSame(exception, ex, "The exception was not traced."); });
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception) with TargetInvocationException traces and returns the inner exception.")]
        ////public void AsErrorWithTargetInvocationExceptionTracesAndReturnsInner()
        ////{
        ////    InvalidOperationException inner = new InvalidOperationException();
        ////    TargetInvocationException tieException = new TargetInvocationException(inner);
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(inner, new ExceptionTrace().AsError(tieException), "The exception was not traced"); },
        ////        (eventSource, ex) => { Assert.AreSame(inner, ex, "The exception was not traced."); });
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception) with AggregateException traces and returns the inner exception.")]
        ////public void AsErrorWithAggregateExceptionTracesAndReturnsInner()
        ////{
        ////    InvalidOperationException inner = new InvalidOperationException();
        ////    AggregateException aggregateException = new AggregateException(inner);
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(inner, new ExceptionTrace().AsError(aggregateException), "The exception was not traced"); },
        ////        (eventSource, ex) => { Assert.AreSame(inner, ex, "The exception was not traced."); });
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception) with AggregateException traces multiple inner exceptions and returns the first inner exception.")]
        ////public void AsErrorWithAggregateExceptionTracesMultipleAndReturnsFirstInner()
        ////{
        ////    List<Exception> tracedExceptions = new List<Exception>();
        ////    InvalidOperationException inner1 = new InvalidOperationException("first");
        ////    InvalidOperationException inner2 = new InvalidOperationException("second");
        ////    AggregateException aggregateException = new AggregateException(inner1, inner2);

        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () =>
        ////        {
        ////            Exception returnedException = new ExceptionTrace().AsError(aggregateException);
        ////            Assert.AreSame(inner1, returnedException, "The first exception was not returned");
        ////        },
        ////        (eventSource, ex) =>
        ////        {
        ////            tracedExceptions.Add(ex);
        ////        });

        ////    CollectionAssert.Contains(tracedExceptions, inner1, "First inner was not traced");
        ////    CollectionAssert.Contains(tracedExceptions, inner2, "Second inner was not traced");
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception) with AggreateException wrapping a TargetInvocationException traces and returns the innermost exception.")]
        ////public void AsErrorWithAggregateExceptionWrappingTargetInvocationExceptionTracesReturnsInner()
        ////{
        ////    InvalidOperationException inner = new InvalidOperationException();
        ////    TargetInvocationException tieException = new TargetInvocationException(inner);
        ////    AggregateException aggregateException = new AggregateException(tieException);
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(inner, new ExceptionTrace().AsError(aggregateException), "The exception was not traced"); },
        ////        (eventSource, ex) => { Assert.AreSame(inner, ex, "The exception was not traced."); });
        ////}

        ////#endregion AsError(Exception)

        ////#region AsError(Exception, string)

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception, string) traces and returns the exception.")]
        ////public void AsError1TracesAndReturnsException()
        ////{
        ////    InvalidOperationException exception = new InvalidOperationException();
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(exception, new ExceptionTrace().AsError(exception, "source"), "The exception was not traced"); },
        ////        (eventSource, ex) => { 
        ////            Assert.AreSame(exception, ex, "The exception was not traced.");
        ////            Assert.AreEqual("source", eventSource, "The eventSource was not traced");
        ////        });
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception, string) with TargetInvocationException traces and returns the inner exception.")]
        ////public void AsError1WithTargetInvocationExceptionTracesAndReturnsInner()
        ////{
        ////    InvalidOperationException inner = new InvalidOperationException();
        ////    TargetInvocationException tieException = new TargetInvocationException(inner);
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(inner, new ExceptionTrace().AsError(tieException, "source"), "The exception was not traced"); },
        ////        (eventSource, ex) => { 
        ////            Assert.AreSame(inner, ex, "The exception was not traced.");
        ////            Assert.AreEqual("source", eventSource, "The eventSource was not traced");
        ////        });
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception, string) with AggregateException traces and returns the inner exception.")]
        ////public void AsError1WithAggregateExceptionTracesAndReturnsInner()
        ////{
        ////    InvalidOperationException inner = new InvalidOperationException();
        ////    AggregateException aggregateException = new AggregateException(inner);
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(inner, new ExceptionTrace().AsError(aggregateException, "source"), "The exception was not traced"); },
        ////        (eventSource, ex) => { 
        ////            Assert.AreSame(inner, ex, "The exception was not traced.");
        ////            Assert.AreEqual("source", eventSource, "The eventSource was not traced");
        ////        });
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception, string) with AggregateException traces multiple inner exceptions and returns the first inner exception.")]
        ////public void AsError1WithAggregateExceptionTracesMultipleAndReturnsFirstInner()
        ////{
        ////    List<Exception> tracedExceptions = new List<Exception>();
        ////    InvalidOperationException inner1 = new InvalidOperationException("first");
        ////    InvalidOperationException inner2 = new InvalidOperationException("second");
        ////    AggregateException aggregateException = new AggregateException(inner1, inner2);
           
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { 
        ////            Exception returnedException = new ExceptionTrace().AsError(aggregateException, "source");
        ////            Assert.AreSame(inner1, returnedException, "The first exception was not returned"); 
        ////        },
        ////        (eventSource, ex) =>
        ////        {
        ////            tracedExceptions.Add(ex);
        ////            Assert.AreEqual("source", eventSource, "The eventSource was not traced");
        ////        });

        ////    CollectionAssert.Contains(tracedExceptions, inner1, "First inner was not traced");
        ////    CollectionAssert.Contains(tracedExceptions, inner2, "Second inner was not traced");
        ////}

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsError(Exception, string) with AggreateException wrapping a TargetInvocationException traces and returns the innermost exception.")]
        ////public void AsError1WithAggregateExceptionWrappingTargetInvocationExceptionTracesReturnsInner()
        ////{
        ////    InvalidOperationException inner = new InvalidOperationException();
        ////    TargetInvocationException tieException = new TargetInvocationException(inner);
        ////    AggregateException aggregateException = new AggregateException(tieException);
        ////    Asserters.DiagnosticTrace.ThrowingException(
        ////        () => { Assert.AreSame(inner, new ExceptionTrace().AsError(aggregateException, "source"), "The exception was not traced"); },
        ////        (eventSource, ex) => { 
        ////            Assert.AreSame(inner, ex, "The exception was not traced.");
        ////            Assert.AreEqual("source", eventSource, "The eventSource was not traced");
        ////        });
        ////}

        ////#endregion AsError(Exception, string)

        ////#region AsInformation(Exception)

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsInformation(Exception) traces the exception.")]
        ////public void AsInformationTracesException()
        ////{
        ////    InvalidOperationException exception = new InvalidOperationException();
        ////    Asserters.DiagnosticTrace.HandledException(
        ////        () => { new ExceptionTrace().AsInformation(exception); },
        ////        (ex) => { Assert.AreSame(exception, ex, "The exception was not traced."); });
        ////}

        ////#endregion AsInformation(Exception)

        ////#region AsWarning(Exception)

        ////[TestMethod]
        ////[HostType("Moles")]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("AsWarning(Exception) traces the exception.")]
        ////public void AsWarningTracesException()
        ////{
        ////    InvalidOperationException exception = new InvalidOperationException();
        ////    Asserters.DiagnosticTrace.HandledExceptionWarning(
        ////        () => { new ExceptionTrace().AsWarning(exception); },
        ////        (ex) => { Assert.AreSame(exception, ex, "The exception was not traced."); });
        ////}

        #endregion AsWarning(Exception)

        #endregion Methods
    }
}
