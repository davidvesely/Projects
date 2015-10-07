using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CarRental.Admin.ViewModels;
using Core.Common;
using Core.Common.UI.Core;

namespace CarRental.Admin.Views
{
    public partial class RentalsView : UserControlViewBase
    {
        public RentalsView()
        {
            InitializeComponent();
        }

        protected override void OnUnwireViewModelEvents(ViewModelBase viewModel)
        {
            RentalsViewModel vm = viewModel as RentalsViewModel;
            if (vm != null)
            {
                vm.RentalReturned -= OnRentalReturned;
                vm.ErrorOccured -= OnErrorOccured;
            }
        }

        protected override void OnWireViewModelEvents(ViewModelBase viewModel)
        {
            RentalsViewModel vm = viewModel as RentalsViewModel;
            if (vm != null)
            {
                vm.RentalReturned += OnRentalReturned;
                vm.ErrorOccured += OnErrorOccured;
            }
        }

        void OnRentalReturned(object sender, EventArgs e)
        {
            MessageBox.Show("Rental returned.");
        }

        void OnErrorOccured(object sender, ErrorMessageEventArgs e)
        {
            MessageBox.Show(e.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
