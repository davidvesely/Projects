using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Common.Contracts;
using Core.Common.Core;

namespace CarRental.Data
{
    [Export(typeof(IDataRepositoryFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        T IDataRepositoryFactory.GetDataRepository<T>()
        {
            return ObjectBase.Container.GetExportedValue<T>();
        }
    }
}
