// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common.Diagnostics
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
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    [TestClass]
    public class EtwTraceTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExceptionTrace.AsError(Exception) traces to file.")]
        public void ExceptionTraceAsErrorTracesToFile()
        {
            TraceSessionAsserter.Execute(
                () => 
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        new ExceptionTrace().AsError(new InvalidOperationException(i.ToString()));
                        Thread.Sleep(10);   // small sleep to separate events in time
                    }
                },
                (fileName) =>
                {
                    Assert.IsTrue(File.Exists(fileName), string.Format("Failed to trace anything to {0}", fileName));

                    // The following size test is a quick test tracing was done.
                    // When tracing is not done, the file size is the default 65536.
                    // You can break here and open this file in Performance Analyzer to verify it manually.
                    FileInfo fileInfo = new FileInfo(fileName);
                    Assert.IsTrue(fileInfo.Length > 70000, string.Format("File {0} was only {1} bytes in size which is too small.", fileName, fileInfo.Length));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExceptionTrace.AsWarning(Exception) traces to file.")]
        public void ExceptionTraceAsWarningTracesToFile()
        {
            TraceSessionAsserter.Execute(
                () =>
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        new ExceptionTrace().AsWarning(new InvalidOperationException(i.ToString()));
                        Thread.Sleep(10);   // small sleep to separate events in time
                    }
                },
                (fileName) =>
                {
                    Assert.IsTrue(File.Exists(fileName), string.Format("Failed to trace anything to {0}", fileName));

                    // The following size test is a quick test tracing was done.
                    // When tracing is not done, the file size is the default 65536.
                    // You can break here and open this file in Performance Analyzer to verify it manually.
                    FileInfo fileInfo = new FileInfo(fileName);
                    Assert.IsTrue(fileInfo.Length > 70000, string.Format("File {0} was only {1} bytes in size which is too small.", fileName, fileInfo.Length));
                });
        }
    }
}
