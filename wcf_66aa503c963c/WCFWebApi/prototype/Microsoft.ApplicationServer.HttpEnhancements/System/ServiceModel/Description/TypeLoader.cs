// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Description
{
    using System;
    using Reflection;

    internal class TypeLoader
    {
        public static Type GetParameterType(ParameterInfo parameterInfo)
        {
            Type parameterType = parameterInfo.ParameterType;
            if (parameterType.IsByRef)
            {
                return parameterType.GetElementType();
            }
            else
            {
                return parameterType;
            }
        }
    }
}
