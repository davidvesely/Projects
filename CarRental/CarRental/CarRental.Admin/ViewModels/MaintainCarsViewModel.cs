using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CarRental.Client.Contracts;
using CarRental.Client.Entities;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Core.Common;

namespace CarRental.Admin.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MaintainCarsViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public MaintainCarsViewModel(IServiceFactory serviceFactory)
        {
            _ServiceFactory = serviceFactory;

            EditCarCommand = new DelegateCommand<Car>(OnEditCarCommand);
            DeleteCarCommand = new DelegateCommand<Car>(OnDeleteCarCommand);
            AddCarCommand = new DelegateCommand<object>(OnAddCarCommand);
        }

        IServiceFactory _ServiceFactory;

        EditCarViewModel _CurrentCarViewModel;

        public DelegateCommand<Car> EditCarCommand { get; private set; }
        public DelegateCommand<Car> DeleteCarCommand { get; private set; }
        public DelegateCommand<object> AddCarCommand { get; private set; }
        
        public override string ViewTitle
        {
            get { return "Maintain Cars"; }
        }

        public event CancelEventHandler ConfirmDelete;
        public event EventHandler<ErrorMessageEventArgs> ErrorOccured;

        public EditCarViewModel CurrentCarViewModel
        {
            get { return _CurrentCarViewModel; }
            set
            {
                if (_CurrentCarViewModel != value)
                {
                    _CurrentCarViewModel = value;
                    OnPropertyChanged(() => CurrentCarViewModel, false);
                }
            }
        }

        ObservableCollection<Car> _Cars;

        public ObservableCollection<Car> Cars
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
        
        protected override void OnViewLoaded()
        {
            _Cars = new ObservableCollection<Car>();

            WithClient<IInventoryService>(_ServiceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                Car[] cars = inventoryClient.GetAllCars();
                if (cars != null)
                {
                    foreach (Car car in cars)
                        _Cars.Add(car);
                }
            });
        }

        void OnEditCarCommand(Car car)
        {
            if (car != null)
            {
                CurrentCarViewModel = new EditCarViewModel(_ServiceFactory, car);
                CurrentCarViewModel.CarUpdated += CurrentCarViewModel_CarUpdated;
                CurrentCarViewModel.CancelEditCar += CurrentCarViewModel_CancelEvent;
            }
        }

        void OnAddCarCommand(object arg)
        {
            Car car = new Car();
            CurrentCarViewModel = new EditCarViewModel(_ServiceFactory, car);
            CurrentCarViewModel.CarUpdated += CurrentCarViewModel_CarUpdated;
            CurrentCarViewModel.CancelEditCar += CurrentCarViewModel_CancelEvent;
        }

        void CurrentCarViewModel_CarUpdated(object sender, Support.CarEventArgs e)
        {
            if (!e.IsNew)
            {
                Car car = _Cars.Where(item => item.CarId == e.Car.CarId).FirstOrDefault();
                if (car != null)
                {
                    car.Description = e.Car.Description;
                    car.Color = e.Car.Color;
                    car.Year = e.Car.Year;
                    car.RentalPrice = e.Car.RentalPrice;
                }
            }
            else
                _Cars.Add(e.Car);

            CurrentCarViewModel = null;
        }

        void CurrentCarViewModel_CancelEvent(object sender, EventArgs e)
        {
            CurrentCarViewModel = null;
        }

        void OnDeleteCarCommand(Car car)
        {
            bool carIsRented = false;

            // check to see if car is currently rented
            WithClient<IRentalService>(_ServiceFactory.CreateClient<IRentalService>(), rentalClient =>
            {
                carIsRented = rentalClient.IsCarCurrentlyRented(car.CarId);
            });

            if (!carIsRented)
            {
                CancelEventArgs args = new CancelEventArgs();
                if (ConfirmDelete != null)
                    ConfirmDelete(this, args);

                if (!args.Cancel)
                {
                    WithClient<IInventoryService>(_ServiceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                    {
                        inventoryClient.DeleteCar(car.CarId);
                        _Cars.Remove(car);
                    });
                }
            }
            else
            {
                if (ErrorOccured != null)
                    ErrorOccured(this, new ErrorMessageEventArgs("Cannot delete this car. It is currently rented."));
            }
        }
    }
}
