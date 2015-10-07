using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using CarRental.Client.Contracts;
using Core.Common;
using Core.Common.Contracts;
using Core.Common.Extensions;
using Core.Common.UI.Core;

namespace CarRental.Admin.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationsViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public ReservationsViewModel(IServiceFactory serviceFactory)
        {
            _ServiceFactory = serviceFactory;

            ExecuteRentalCommand = new DelegateCommand<int>(OnExecuteRentalCommandExecute);
            CancelReservationCommand = new DelegateCommand<int>(OnCancelReservationCommandExecute);
        }

        IServiceFactory _ServiceFactory;

        public override string ViewTitle
        {
            get { return "Current Reservations"; }
        }

        ObservableCollection<CustomerReservationData> _Reservations;

        public DelegateCommand<int> ExecuteRentalCommand { get; private set; }
        public DelegateCommand<int> CancelReservationCommand { get; private set; }

        public event EventHandler RentalExecuted;
        public event EventHandler ReservationCanceled;
        public event EventHandler<ErrorMessageEventArgs> ErrorOccured;

        public ObservableCollection<CustomerReservationData> Reservations
        {
            get { return _Reservations; }
            set
            {
                if (_Reservations != value)
                {
                    _Reservations = value;
                    OnPropertyChanged(() => Reservations, false);
                }
            }
        }

        protected override void OnViewLoaded()
        {
            _Reservations = new ObservableCollection<CustomerReservationData>();

            WithClient<IRentalService>(_ServiceFactory.CreateClient<IRentalService>(), rentalClient =>
            {
                CustomerReservationData[] reservations = rentalClient.GetCurrentReservations();
                if (reservations != null)
                {
                    // convert returned data into observable collection so binding can refresh automatically
                    Reservations.Merge(reservations);
                }
            });
        }

        void OnExecuteRentalCommandExecute(int reservationId)
        {
            WithClient<IRentalService>(_ServiceFactory.CreateClient<IRentalService>(), rentalClient =>
            {
                CustomerReservationData customerReservation = _Reservations.Where(item => item.ReservationId == reservationId).FirstOrDefault();
                if (customerReservation != null)
                {
                    try
                    {
                        rentalClient.ExecuteRentalFromReservation(reservationId);
                        Reservations.Remove(customerReservation);

                        if (RentalExecuted != null)
                            RentalExecuted(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        if (ErrorOccured != null)
                            ErrorOccured(this, new ErrorMessageEventArgs(ex.Message));
                    }
                }
            });
        }

        void OnCancelReservationCommandExecute(int reservationId)
        {
            WithClient<IRentalService>(_ServiceFactory.CreateClient<IRentalService>(), rentalClient =>
            {
                CustomerReservationData customerReservation = _Reservations.Where(item => item.ReservationId == reservationId).FirstOrDefault();
                if (customerReservation != null)
                {
                    try
                    {
                        rentalClient.CancelReservation(reservationId);
                        Reservations.Remove(customerReservation);

                        if (ReservationCanceled != null)
                            ReservationCanceled(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        if (ErrorOccured != null)
                            ErrorOccured(this, new ErrorMessageEventArgs(ex.Message));
                    }
                }
            });
        }
    }
}
