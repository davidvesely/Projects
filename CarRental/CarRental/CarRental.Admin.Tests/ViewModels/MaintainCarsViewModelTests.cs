using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CarRental.Admin.ViewModels;
using CarRental.Client.Contracts;
using CarRental.Client.Entities;
using Core.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarRental.Admin.Tests
{
    [TestClass]
    public class MaintainCarsViewModelTests
    {
        [TestMethod]
        public void TestViewLoaded()
        {
            Car[] data = new List<Car>()
                {
                    new Car() { CarId = 1 },
                    new Car() { CarId = 2 }
                }.ToArray();

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().GetAllCars()).Returns(data);

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);

            Assert.IsTrue(viewModel.Cars == null);

            object loaded = viewModel.ViewLoaded; // fires off the OnViewLoaded protected method

            Assert.IsTrue(viewModel.Cars != null && viewModel.Cars.Count == data.Length && viewModel.Cars[0] == data[0]);
        }

        [TestMethod]
        public void TestCurrentCarSetting()
        {
            Car car = new Car() { CarId = 1 };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);

            Assert.IsTrue(viewModel.CurrentCarViewModel == null);

            viewModel.EditCarCommand.Execute(car);

            Assert.IsTrue(viewModel.CurrentCarViewModel != null && viewModel.CurrentCarViewModel.Car.CarId == car.CarId);
        }

        [TestMethod]
        public void TestEditCarCommand()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);

            viewModel.Cars = new ObservableCollection<Car>()
                {
                    car
                };

            Assert.IsTrue(viewModel.Cars[0].Color == "White");
            Assert.IsTrue(viewModel.CurrentCarViewModel == null);

            viewModel.EditCarCommand.Execute(car);

            Assert.IsTrue(viewModel.CurrentCarViewModel != null);

            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().UpdateCar(It.IsAny<Car>())).Returns(viewModel.CurrentCarViewModel.Car);

            viewModel.CurrentCarViewModel.Car.Color = "Black";
            viewModel.CurrentCarViewModel.SaveCommand.Execute(null);

            Assert.IsTrue(viewModel.Cars[0].Color == "Black");
        }

        [TestMethod]
        public void TestDeleteCarCommand()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().IsCarCurrentlyRented(car.CarId)).Returns(false);
            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().DeleteCar(car.CarId));

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);
            viewModel.Cars = new ObservableCollection<Car>()
                {
                    car
                };

            viewModel.ConfirmDelete += (s, e) => e.Cancel = false;

            Assert.IsTrue(viewModel.Cars.Count == 1);

            viewModel.DeleteCarCommand.Execute(car);

            Assert.IsTrue(viewModel.Cars.Count == 0);
        }

        [TestMethod]
        public void TestDeleteCarCommandWithCancel()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().IsCarCurrentlyRented(car.CarId)).Returns(false);
            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().DeleteCar(car.CarId));

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);
            viewModel.Cars = new ObservableCollection<Car>()
                {
                    car
                };

            viewModel.ConfirmDelete += (s, e) => e.Cancel = true; // cancel the deletion

            Assert.IsTrue(viewModel.Cars.Count == 1);

            viewModel.DeleteCarCommand.Execute(car);

            Assert.IsTrue(viewModel.Cars.Count == 1);
        }

        [TestMethod]
        public void TestDeleteCarCommandWithError()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().IsCarCurrentlyRented(car.CarId)).Returns(true); // currently rented
            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().DeleteCar(car.CarId));

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);
            viewModel.Cars = new ObservableCollection<Car>()
            {
                car
            };

            bool errorOccured = false;
            viewModel.ErrorOccured += (s, e) => errorOccured = true;

            Assert.IsTrue(viewModel.Cars.Count == 1);

            viewModel.DeleteCarCommand.Execute(car);

            Assert.IsTrue(errorOccured && viewModel.Cars.Count == 1);
        }

        [TestMethod]
        public void TestAddCarCommand()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            MaintainCarsViewModel viewModel = new MaintainCarsViewModel(mockServiceFactory.Object);
            viewModel.Cars = new ObservableCollection<Car>();

            Assert.IsTrue(viewModel.CurrentCarViewModel == null);

            viewModel.AddCarCommand.Execute(car);

            Assert.IsTrue(viewModel.CurrentCarViewModel != null);

            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().UpdateCar(It.IsAny<Car>())).Returns(viewModel.CurrentCarViewModel.Car);

            viewModel.CurrentCarViewModel.SaveCommand.Execute(null);

            Assert.IsTrue(viewModel.Cars != null && viewModel.Cars.Count == 1);
        }
    }
}
