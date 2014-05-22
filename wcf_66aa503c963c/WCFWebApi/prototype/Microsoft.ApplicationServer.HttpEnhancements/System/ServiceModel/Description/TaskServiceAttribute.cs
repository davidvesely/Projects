// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;

    using Microsoft.Server.Common;

    /// <summary>
    /// Add the <see cref="TaskServiceAttribute"/> attribute to a service class to enable <see cref="Task"/>-based service operations. 
    /// In addition, use the <see cref="TaskOperationAttribute"/> on individual service operations to indicate that they
    /// are <see cref="Task"/>-based.
    /// </summary>
    public class TaskServiceAttribute : Attribute, IServiceBehavior
    {
        /// <summary>
        /// Passes custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <remarks>This <see cref="IServiceBehavior"/> implementation does not pass any data.</remarks>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            return;
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceDescription == null)
            {
                throw Fx.Exception.ArgumentNull("serviceDescription");
            }

            if (serviceHostBase == null)
            {
                throw Fx.Exception.ArgumentNull("serviceHostBase");
            }

            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
                {
                    if (operationDescription.SyncMethod != null)
                    {
                        if (ServiceReflector.taskType.IsAssignableFrom(operationDescription.SyncMethod.ReturnType))
                        {
                            foreach (MessageDescription messageDescription in operationDescription.Messages)
                            {
                                if (messageDescription.Direction == MessageDirection.Output)
                                {
                                    MessagePartDescription returnValue = messageDescription.Body.ReturnValue;
                                    if (returnValue != null &&
                                        ServiceReflector.taskType.IsAssignableFrom(returnValue.Type))
                                    {
                                        messageDescription.Body.ReturnValue.Type = ExtractTaskResultType(returnValue.Type);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <remarks>This <see cref="IServiceBehavior"/> implementation does not do any validation.</remarks>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Extracts the <see cref="Type"/> of the <see cref="Task{T}"/> or <c>void</c> in the case of
        /// the non-generic <see cref="Task"/>.
        /// </summary>
        /// <param name="taskType">Type of the task.</param>
        /// <returns></returns>
        internal static Type ExtractTaskResultType(Type taskType)
        {
            Fx.Assert(taskType != null, "taskType cannot be null");
            return taskType.IsGenericType ? taskType.GetGenericArguments()[0] : ServiceReflector.VoidType;
        }
    }
}