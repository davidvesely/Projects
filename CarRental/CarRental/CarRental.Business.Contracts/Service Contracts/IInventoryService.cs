using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CarRental.Business.Entities;
using Core.Common.Exceptions;

namespace CarRental.Business.Contracts
{
    [ServiceContract]
    public interface IInventoryService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Car UpdateCar(Car car);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteCar(int carId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Car GetCar(int carId);

        [OperationContract]
        Car[] GetAllCars();

        [OperationContract]
        Car[] GetAvailableCars(DateTime pickupDate, DateTime returnDate);
    }
}
