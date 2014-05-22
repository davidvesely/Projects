// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Reflection;
    using System.Runtime;
    using System.Security;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using Microsoft.Server.Common;
    using TaskDispatcherBehavior;

    /// <summary>
    /// An invoker used when some operation contract has a return value of Task or its generic counterpart (Task of T) 
    /// </summary>
    internal class TaskMethodInvoker : IOperationInvoker
    {
        private const string FailedAuthenticationName = "FailedAuthentication";
        private const string FailedAuthenticationNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wsswssecurity-utility-1.0.xsd";
        private const string ResultMethodName = "Result";
        private MethodInfo taskMethod;
        private bool isGenericTask;
        private InvokeDelegate invokeDelegate;
        private int inputParameterCount;
        private int outputParameterCount;
        private object[] outputs;
        private MethodInfo toAsyncMethodInfo;
        private MethodInfo taskTResultGetMethod;

        public TaskMethodInvoker(MethodInfo taskMethod, Type taskType)
        {
            if (taskMethod == null)
            {
                throw Fx.Exception.ArgumentNull("taskMethod");
            }

            this.taskMethod = taskMethod;

            if (taskType != ServiceReflector.VoidType)
            {
                this.toAsyncMethodInfo = TaskExtensions.MakeGenericMethod(taskType);
                this.taskTResultGetMethod = ((PropertyInfo)taskMethod.ReturnType.GetMember(ResultMethodName)[0]).GetGetMethod();
                this.isGenericTask = true;
            }
        }

        public bool IsSynchronous
        {
            get { return false; }
        }

        public MethodInfo TaskMethod
        {
            get { return this.taskMethod; }
        }

        private InvokeDelegate InvokeDelegate
        {
            get
            {
                this.EnsureIsInitialized();
                return this.invokeDelegate;
            }
        }

        private int InputParameterCount
        {
            get
            {
                this.EnsureIsInitialized();
                return this.inputParameterCount;
            }
        }

        private int OutputParameterCount
        {
            get
            {
                this.EnsureIsInitialized();
                return this.outputParameterCount;
            }
        }

        public object[] AllocateInputs()
        {
            return EmptyArray.Allocate(this.InputParameterCount);
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            throw Fx.Exception.AsError(new NotImplementedException());
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            if (instance == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR2.SFxNoServiceObject));
            }

            if (inputs == null)
            {
                if (this.InputParameterCount > 0)
                {
                    throw Fx.Exception.AsError(new InvalidOperationException(SR2.SFxInputParametersToServiceNull(this.InputParameterCount)));
                }
            }
            else if (inputs.Length != this.InputParameterCount)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR2.SFxInputParametersToServiceInvalid(this.InputParameterCount, inputs.Length)));
            }

            this.outputs = EmptyArray.Allocate(this.OutputParameterCount);

            IAsyncResult returnValue;

            try
            {
                object taskReturnValue = this.InvokeDelegate(instance, inputs, this.outputs);

                if (taskReturnValue == null)
                {
                    throw Fx.Exception.ArgumentNull("task");
                }
                else if (this.isGenericTask)
                {
                    returnValue = (IAsyncResult)this.toAsyncMethodInfo.Invoke(null, new object[] { taskReturnValue, callback, state });
                }
                else
                {
                    returnValue = ((Task)taskReturnValue).AsAsyncResult(callback, state);
                }
            }
            catch (System.Security.SecurityException)
            {
                throw Fx.Exception.AsError(CreateAccessDeniedFaultException());
            }

            return returnValue;
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            object returnVal;

            if (instance == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR2.SFxNoServiceObject));
            }

            try
            {
                Task task = result as Task;

                Fx.Assert(task != null, "InvokeEnd needs to be called with the result returned from InvokeBegin.");
                if (task.IsFaulted)
                {
                    Fx.Assert(task.Exception != null, "Task.IsFaulted guarantees non-null exception.");

                    // If FaultException is thrown, we will get 'callFaulted' behavior below.
                    // Any other exception will retain 'callFailed' behavior.
                    throw Fx.Exception.AsError<FaultException>(task.Exception);
                }

                // Task cancellation without an exception indicates failure but we have no
                // additional information to provide.  Accessing Task.Result will throw a
                // TaskCanceledException.   For consistency between void Tasks and Task<T>,
                // we detect and throw here.
                if (task.IsCanceled)
                {
                    throw Fx.Exception.AsError(new TaskCanceledException(task));
                }

                outputs = this.outputs;
                if (this.isGenericTask)
                {
                    returnVal = this.taskTResultGetMethod.Invoke(result, Type.EmptyTypes);
                }
                else
                {
                    returnVal = null;
                }
            }
            catch (SecurityException)
            {
                throw Fx.Exception.AsError(CreateAccessDeniedFaultException());
            }

            return returnVal;
        }

        private void EnsureIsInitialized()
        {
            if (this.invokeDelegate == null)
            {
                // Only pass locals byref because InvokerUtil may store temporary results in the byref.
                // If two threads both reference this.count, temporary results may interact.
                int inputParameterCount;
                int outputParameterCount;
                InvokeDelegate invokeDelegate = new InvokerUtil().GenerateInvokeDelegate(this.taskMethod, out inputParameterCount, out outputParameterCount);
                this.inputParameterCount = inputParameterCount;
                this.outputParameterCount = outputParameterCount;
                this.invokeDelegate = invokeDelegate;  // must set this last due to race
            }
        }

        private static Exception CreateAccessDeniedFaultException()
        {
            FaultCode code = FaultCode.CreateSenderFaultCode(FailedAuthenticationName, FailedAuthenticationNamespace);
            return new FaultException(new FaultReason(SR2.AccessDenied), code);
        }
    }
}