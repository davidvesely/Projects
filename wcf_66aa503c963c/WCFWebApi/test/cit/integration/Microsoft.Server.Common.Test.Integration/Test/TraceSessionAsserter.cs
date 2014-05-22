// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TraceSessionAsserter
    {
        public static void Execute(Action actionThatTraces, Action<string> actionToProcessEtlFile)
        {
            string tempFileName = Path.GetTempFileName();
            string fileName = tempFileName.Replace(".tmp", ".etl");
            File.Move(tempFileName, fileName);

            TraceSession traceSession = new TraceSession(fileName);
            traceSession.Start(); //don't print output as it contains a handle id
            traceSession.EnableProvider(EtwDiagnosticTrace.ImmutableDefaultEtwProviderId, 255);
            try
            {
                actionThatTraces();
            }
            finally
            {
                traceSession.Flush();
                traceSession.Stop();

                try
                {
                    actionToProcessEtlFile(fileName);
                }
                finally
                {
                    File.Delete(fileName);
                }
            }
        }
    }
}
