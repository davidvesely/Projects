// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Reflection;
    using System.ServiceModel;
    using Microsoft.Server.Common;

    /// <summary>
    /// This <see cref="TypeDelegator"/> helper class overrides normal Reflection for a given
    /// type to expose it as a service contract type.
    /// </summary>
    internal class ServiceContractTypeDelegator : TypeDelegator
    {
        private static readonly Type serviceContractAttributeType = typeof(ServiceContractAttribute);

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContractTypeDelegator"/> class.
        /// </summary>
        /// <param name="delegatingType">The type to expose as a service contract.</param>
        public ServiceContractTypeDelegator(Type delegatingType)
            : base(delegatingType)
        {
        }

        /// <summary>
        /// Indicates whether a custom attribute identified by <paramref name="attributeType"/> is defined.
        /// </summary>
        /// <param name="attributeType">The type of attribute to check.</param>
        /// <param name="inherit">Specifies whether to search this type's inheritance chain to find the attributes.</param>
        /// <returns>
        /// true if a custom attribute identified by <paramref name="attributeType"/> is defined; otherwise, false.
        /// </returns>
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw Fx.Exception.ArgumentNull("attributeType");
            }

            return attributeType.Equals(serviceContractAttributeType)
                    ? true
                    : base.IsDefined(attributeType, inherit);
        }

        /// <summary>
        /// Returns an array of custom attributes identified by type.
        /// </summary>
        /// <param name="attributeType">The type of attributes to get.</param>
        /// <param name="inherit">Specifies whether to search this type's inheritance chain to find the attributes.</param>
        /// <returns>
        /// An array of objects containing the custom attributes defined in this type that match the <paramref name="attributeType"/> parameter, specifying whether to search the type's inheritance chain.
        /// </returns>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw Fx.Exception.ArgumentNull("attributeType");
            }

            object[] attributes = base.GetCustomAttributes(attributeType, inherit);

            return (attributeType.Equals(serviceContractAttributeType) && (attributes == null || attributes.Length == 0))
                ? new object[] { new ServiceContractAttribute() }
                : attributes;
        }
    }
}
