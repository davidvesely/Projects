using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CarRental.Client.Contracts;
using CarRental.Client.Entities;
using Core.Common.Contracts;
using Core.Common.UI.Core;

namespace CarRental.Admin.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DashboardViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public DashboardViewModel(IServiceFactory serviceFactory)
        {
            _ServiceFactory = serviceFactory;
        }

        IServiceFactory _ServiceFactory;
        
        public override string ViewTitle
        {
            get { return "Dashboard"; }
        }

        protected override void OnViewLoaded()
        {
            // can check properties for null here if not want to re-get every time view shows
            
            WithClient<IInventoryService>(_ServiceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                Cars = inventoryClient.GetAllCars();
            });
        }

        Car[] _Cars;
        CustomerRentalData[] _CurrentlyRented;

        public Car[] Cars
        {
            get { return _Cars; }
            set
            {
                if (_Cars != value)
                {
                    _Cars = value;
                    OnPropertyChanged(() => Cars, false);
                }
            }
        }
    }
}
