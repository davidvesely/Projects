// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Description
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Threading.Tasks;

    using Microsoft.Server.Common;

    /// <summary>
    /// Add this attribute to a service operation to indicate that this operation is a <see cref="Task"/>-based operation.
    /// In addition, add the <see cref="TaskServiceAttribute"/> attribute to a service class to enable <see cref="Task"/>-based service operations. 
    /// </summary>
    public class TaskOperationAttribute : Attribute, IOperationBehavior
    {
        /// <summary>
        /// Passes data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <remarks>This <see cref="IOperationBehavior"/> implementation does not pass any data.</remarks>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="bindingParameters">The collection of objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        ///  Modifies a client across an operation.
        /// </summary>
        /// <remarks>This <see cref="IOperationBehavior"/> implementation does not affect any client operations.</remarks>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="clientOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        /// <summary>
        /// Enables support for invoking <see cref="Task"/>-based operations.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="dispatchOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            if (operationDescription == null)
            {
                throw Fx.Exception.ArgumentNull("operationDescription");
            }

            if (dispatchOperation == null)
            {
                throw Fx.Exception.ArgumentNull("dispatchOperation");
            }

            if (ServiceReflector.taskType.IsAssignableFrom(operationDescription.SyncMethod.ReturnType))
            {
                Type returnType = TaskServiceAttribute.ExtractTaskResultType(operationDescription.SyncMethod.ReturnType);
                dispatchOperation.Invoker = new TaskMethodInvoker(operationDescription.SyncMethod, returnType);
            }
        }

        /// <summary>
        /// Implement to confirm that the operation meets some intended criteria.
        /// </summary>
        /// <remarks>This <see cref="IOperationBehavior"/> implementation does not validate any operations.</remarks>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}