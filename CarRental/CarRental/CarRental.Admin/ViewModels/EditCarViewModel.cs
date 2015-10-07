using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CarRental.Client.Contracts;
using CarRental.Client.Entities;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using CarRental.Admin.Support;

namespace CarRental.Admin.ViewModels
{
    public class EditCarViewModel : ViewModelBase
    {
        // note that this viewmodel is instantiated on-demand from parent and not with DI

        public EditCarViewModel(IServiceFactory serviceFactory, Car car)
        {
            _ServiceFactory = serviceFactory;
            _Car = new Car()
            {
                CarId = car.CarId,
                Description = car.Description,
                Color = car.Color,
                Year = car.Year,
                RentalPrice = car.RentalPrice
            };

            _Car.CleanAll();

            SaveCommand = new DelegateCommand<object>(OnSaveCommandExecute, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommandExecute);
        }

        IServiceFactory _ServiceFactory;
        Car _Car;

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

        public event EventHandler CancelEditCar;
        public event EventHandler<CarEventArgs> CarUpdated;

        public Car Car
        {
            get { return _Car; }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Car);
        }
        
        void OnSaveCommandExecute(object arg)
        {
            ValidateModel();

            if (IsValid)
            {
                WithClient<IInventoryService>(_ServiceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool isNew = (_Car.CarId == 0);

                    var savedCar = inventoryClient.UpdateCar(_Car);
                    if (savedCar != null)
                    {
                        if (CarUpdated != null)
                            CarUpdated(this, new CarEventArgs(savedCar, isNew));
                    }
                });
            }
        }
        
        bool OnSaveCommandCanExecute(object arg)
        {
            return _Car.IsDirty;
        }

        void OnCancelCommandExecute(object arg)
        {
            if (CancelEditCar != null)
                CancelEditCar(this, EventArgs.Empty);
        }
    }
}
