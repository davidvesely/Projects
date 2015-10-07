using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Common.UI.Core;

namespace CarRental.Admin.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ViewModelBase
    {
        [Import]
        public DashboardViewModel DashboardViewModel { get; private set; }

        [Import]
        public MaintainCarsViewModel MaintainCarsViewModel { get; private set; }

        [Import]
        public ReservationsViewModel ReservationsViewModel { get; private set; }

        [Import]
        public RentalsViewModel RentalsViewModel { get; private set; }
    }
}
