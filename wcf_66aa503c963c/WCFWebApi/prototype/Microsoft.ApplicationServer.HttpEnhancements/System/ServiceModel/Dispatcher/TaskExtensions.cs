// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using Linq;
    using Reflection;
    using Threading.Tasks;
    using Microsoft.Server.Common;

    /// <summary>
    /// Extension methods for <see cref="Task"/> and its generic <see cref="Task{T}"/> counterpart.
    /// </summary>
    internal static class TaskExtensions
    {
        private const string TaskAsAsyncResultMethodName = "AsAsyncResult";
        private static MethodInfo taskAsAsyncResultMethodInfo;

        public static MethodInfo TaskAsAsyncResultMethodInfo
        {
            get
            {
                if (taskAsAsyncResultMethodInfo == null)
                {
                    taskAsAsyncResultMethodInfo = typeof(System.Runtime.TaskExtensions).GetMethods().Where(m =>
                                                   m.IsGenericMethod && m.Name == TaskAsAsyncResultMethodName).First();
                    Fx.Assert(taskAsAsyncResultMethodInfo != null, "taskAsAsyncResultMethodInfo should not be null.");
                }

                return taskAsAsyncResultMethodInfo;
            }
        }

        public static MethodInfo MakeGenericMethod(Type genericArgument)
        {
            return TaskAsAsyncResultMethodInfo.MakeGenericMethod(genericArgument);
        }
    }
}