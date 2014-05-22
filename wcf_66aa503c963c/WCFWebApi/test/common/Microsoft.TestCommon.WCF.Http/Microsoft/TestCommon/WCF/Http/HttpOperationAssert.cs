// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel.Description;
    using System.Text;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class HttpOperationAssert
    {
        private static readonly HttpOperationAssert singleton = new HttpOperationAssert();

        public static HttpOperationAssert Singleton { get { return singleton; } }

        public void Execute<TServiceContract>(Action<HttpOperationDescription> onGetOperation)
        {
            Execute(typeof(TServiceContract), onGetOperation);
        }

        public void Execute(Type serviceContract, Action<HttpOperationDescription> onGetOperation)
        {
            Assert.IsNotNull(onGetOperation, "The 'OnGetOperations' parameter sould not be null.");

            ContractDescription contract = ContractDescription.GetContract(serviceContract);
            foreach (HttpOperationDescription operation in contract.Operations.Select(operation => operation.ToHttpOperationDescription()))
            {
                onGetOperation(operation);
            };
        }

        public void Execute<TServiceContract>(Action<IEnumerable<HttpOperationDescription>> onGetOperations)
        {
            Execute(typeof(TServiceContract), onGetOperations);
        }

        public void Execute(Type serviceContract, Action<IEnumerable<HttpOperationDescription>> onGetOperations)
        {
            Assert.IsNotNull(onGetOperations, "Test Error: onGetOperations parameter must be specified.");

            ContractDescription contract = ContractDescription.GetContract(serviceContract);
            onGetOperations(contract.Operations.Select(operation => operation.ToHttpOperationDescription()));
        }
    }
}
