// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Advanced
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using Microsoft.ApplicationServer.Http;

    [CLSCompliant(false)]
    public class MefConfiguration : WebApiConfiguration
    {
        public MefConfiguration(CompositionContainer container)
        {
            CreateInstance = (t, i, m) =>
            {
                var contract = AttributedModelServices.GetContractName(t);
                var identity = AttributedModelServices.GetTypeIdentity(t);

                // force non-shared so that every service doesn't need to have a [PartCreationPolicy] attribute.
                var definition = new ContractBasedImportDefinition(contract, identity, null, ImportCardinality.ExactlyOne, false, false, CreationPolicy.NonShared);
                return container.GetExports(definition).First().Value;
            }; 
        }
    }
}