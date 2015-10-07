using System;
using System.Collections.Generic;
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
    public class EditCarViewModelTests
    {
        [TestMethod]
        public void TestViewModelConstruction()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            EditCarViewModel viewModel = new EditCarViewModel(mockServiceFactory.Object, car);

            Assert.IsTrue(viewModel.Car != null && viewModel.Car != car);
            Assert.IsTrue(viewModel.Car.CarId == car.CarId && viewModel.Car.Color == car.Color);
        }

        [TestMethod]
        public void TestSaveCommand()
        {
            Car car = new Car() { CarId = 1, Color = "White", Description = "Kia Optima", Year = 2013, RentalPrice = 149.00M };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            EditCarViewModel viewModel = new EditCarViewModel(mockServiceFactory.Object, car);

            mockServiceFactory.Setup(mock => mock.CreateClient<IInventoryService>().UpdateCar(It.IsAny<Car>())).Returns(viewModel.Car);

            viewModel.Car.Color = "Black";

            bool carUpdated = false;
            string color = string.Empty;
            viewModel.CarUpdated += (s, e) =>
            {
                carUpdated = true;
                color = e.Car.Color;
            };

            viewModel.SaveCommand.Execute(null);

            Assert.IsTrue(carUpdated);
            Assert.IsTrue(color == "Black");
        }

        [TestMethod]
        public void TestCanSaveCommand()
        {
            Car car = new Car() { CarId = 1, Color = "White", Description = "Kia Optima", Year = 2013, RentalPrice = 149 };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            EditCarViewModel viewModel = new EditCarViewModel(mockServiceFactory.Object, car);

            Assert.IsFalse(viewModel.SaveCommand.CanExecute(null));

            viewModel.Car.Color = "Black";

            Assert.IsTrue(viewModel.SaveCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestCarIsValid()
        {
            Car car = new Car() { CarId = 1, Color = "White", Description = "Kia Optima", Year = 2013 };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            EditCarViewModel viewModel = new EditCarViewModel(mockServiceFactory.Object, car);

            Assert.IsTrue(!viewModel.Car.IsValid);

            viewModel.Car.RentalPrice = 149;

            Assert.IsTrue(viewModel.Car.IsValid);
        }

        [TestMethod]
        public void TestCancelCommand()
        {
            Car car = new Car() { CarId = 1, Color = "White" };

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();

            EditCarViewModel viewModel = new EditCarViewModel(mockServiceFactory.Object, car);

            bool canceled = false;
            viewModel.CancelEditCar += (s, e) => canceled = true;

            Assert.IsTrue(!canceled);

            viewModel.CancelCommand.Execute(null);

            Assert.IsTrue(viewModel.CancelCommand.CanExecute(null));

            Assert.IsTrue(canceled);
        }
    }
}
