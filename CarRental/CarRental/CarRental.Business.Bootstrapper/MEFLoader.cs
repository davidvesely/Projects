using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CarRental.Business.Managers;
using CarRental.Data;

namespace CarRental.Business.Bootstrapper
{
    public static class MEFLoader
    {
        public static CompositionContainer Init()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(CarRentalEngine).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(AccountRepository).Assembly));
            
            CompositionContainer container = new CompositionContainer(catalog);

            return container;
        }

    }
}
