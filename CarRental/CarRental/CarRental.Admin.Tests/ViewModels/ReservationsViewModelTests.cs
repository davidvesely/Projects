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
    public class ReservationsViewModelTests
    {
        [TestMethod]
        public void TestViewLoaded()
        {
            CustomerReservationData[] data = new List<CustomerReservationData>()
            {
                new CustomerReservationData(),
                new CustomerReservationData()
            }.ToArray();

            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().GetCurrentReservations()).Returns(data);

            ReservationsViewModel viewModel = new ReservationsViewModel(mockServiceFactory.Object);

            Assert.IsTrue(viewModel.Reservations == null);

            object loaded = viewModel.ViewLoaded; // fires off the OnViewLoaded protected method

            Assert.IsTrue(viewModel.Reservations != null && viewModel.Reservations.Count == data.Length && viewModel.Reservations[0] == data[0]);            
        }

        [TestMethod]
        public void TestExecuteRentalCommand()
        {
            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().ExecuteRentalFromReservation(1));

            ReservationsViewModel viewModel = new ReservationsViewModel(mockServiceFactory.Object);

            viewModel.Reservations = new ObservableCollection<CustomerReservationData>()
            {
                new CustomerReservationData() { ReservationId = 1 },
                new CustomerReservationData() { ReservationId = 2 }
            };

            bool rentalExecuted = false;
            bool errorOccured = false;

            viewModel.RentalExecuted += (s, e) => rentalExecuted = true;
            viewModel.ErrorOccured += (s, e) => errorOccured = true;

            viewModel.ExecuteRentalCommand.Execute(1);

            Assert.IsTrue(rentalExecuted && !errorOccured);
            Assert.IsTrue(viewModel.Reservations.Count == 1 && viewModel.Reservations[0].ReservationId == 2);
        }

        [TestMethod]
        public void TestExecuteRentalCommandWithError()
        {
            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().ExecuteRentalFromReservation(1)).Throws(new Exception());

            ReservationsViewModel viewModel = new ReservationsViewModel(mockServiceFactory.Object);

            viewModel.Reservations = new ObservableCollection<CustomerReservationData>()
            {
                new CustomerReservationData() { ReservationId = 1 },
                new CustomerReservationData() { ReservationId = 2 }
            };

            bool rentalExecuted = false;
            bool errorOccured = false;

            viewModel.RentalExecuted += (s, e) => rentalExecuted = true;
            viewModel.ErrorOccured += (s, e) => errorOccured = true;

            viewModel.ExecuteRentalCommand.Execute(1);

            Assert.IsTrue(!rentalExecuted && errorOccured);
        }

        [TestMethod]
        public void TestCancelReservationCommand()
        {
            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().CancelReservation(1));

            ReservationsViewModel viewModel = new ReservationsViewModel(mockServiceFactory.Object);

            viewModel.Reservations = new ObservableCollection<CustomerReservationData>()
            {
                new CustomerReservationData() { ReservationId = 1 },
                new CustomerReservationData() { ReservationId = 2 }
            };

            bool reservationCanceled = false;
            bool errorOccured = false;

            viewModel.ReservationCanceled += (s, e) => reservationCanceled = true;
            viewModel.ErrorOccured += (s, e) => errorOccured = true;

            viewModel.CancelReservationCommand.Execute(1);

            Assert.IsTrue(reservationCanceled && !errorOccured);
            Assert.IsTrue(viewModel.Reservations.Count == 1 && viewModel.Reservations[0].ReservationId == 2);
        }

        [TestMethod]
        public void TestCancelReservationCommandWithError()
        {
            Mock<IServiceFactory> mockServiceFactory = new Mock<IServiceFactory>();
            mockServiceFactory.Setup(mock => mock.CreateClient<IRentalService>().CancelReservation(1)).Throws(new Exception());

            ReservationsViewModel viewModel = new ReservationsViewModel(mockServiceFactory.Object);

            viewModel.Reservations = new ObservableCollection<CustomerReservationData>()
            {
                new CustomerReservationData() { ReservationId = 1 },
                new CustomerReservationData() { ReservationId = 2 }
            };

            bool reservationCanceled = false;
            bool errorOccured = false;

            viewModel.ReservationCanceled += (s, e) => reservationCanceled = true;
            viewModel.ErrorOccured += (s, e) => errorOccured = true;

            viewModel.CancelReservationCommand.Execute(1);

            Assert.IsTrue(!reservationCanceled && errorOccured);
        }
    }
}
