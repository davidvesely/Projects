// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Types
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Tagging interface to indicate types which we know OData cannot serialize.
    /// </summary>
    public interface INotODataSerializable
    {
    }
}
