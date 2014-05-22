//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    //An AsyncResult that completes as soon as it is instantiated.
    public class CompletedAsyncResult : AsyncResult
    {
        public CompletedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
            Complete(true);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Result is validated.")]
        [Fx.Tag.GuaranteeNonBlocking]
        public static void End(IAsyncResult result)
        {
            Fx.AssertAndFailFast(result != null, "CompletedAsyncResult was null.");
            Fx.AssertAndFailFast(result.IsCompleted, "CompletedAsyncResult was not completed!");
            AsyncResult.End<CompletedAsyncResult>(result);
        }
    }

    public class CompletedAsyncResult<T> : AsyncResult
    {
        T data;

        public CompletedAsyncResult(T data, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.data = data;
            Complete(true);
        }

        [Fx.Tag.GuaranteeNonBlocking]
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Existing API")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Result is validated.")]
        public static T End(IAsyncResult result)
        {
            Fx.AssertAndFailFast(result != null, "CompletedAsyncResult<T> was null.");
            Fx.AssertAndFailFast(result.IsCompleted, "CompletedAsyncResult<T> was not completed!");
            CompletedAsyncResult<T> completedResult = AsyncResult.End<CompletedAsyncResult<T>>(result);
            return completedResult.data;
        }
    }

    public class CompletedAsyncResult<TResult, TParameter> : AsyncResult
    {
        TResult resultData;
        TParameter parameter;

        public CompletedAsyncResult(TResult resultData, TParameter parameter, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.resultData = resultData;
            this.parameter = parameter;
            Complete(true);
        }

        [Fx.Tag.GuaranteeNonBlocking]
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Existing API")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Out parameter is fine here.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Result is validated.")]
        public static TResult End(IAsyncResult result, out TParameter parameter)
        {
            Fx.AssertAndFailFast(result != null, "CompletedAsyncResult<T> was null.");
            Fx.AssertAndFailFast(result.IsCompleted, "CompletedAsyncResult<T> was not completed!");
            CompletedAsyncResult<TResult, TParameter> completedResult = AsyncResult.End<CompletedAsyncResult<TResult, TParameter>>(result);
            parameter = completedResult.parameter;
            return completedResult.resultData;
        }
    }
}
