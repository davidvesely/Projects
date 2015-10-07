using System;
using System.Net.Http;
using System.Web.Http;
using CarRental.Client.Contracts;
using CarRental.Client.Entities;
using CarRental.Web.Controllers.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarRental.Web.Tests.Controllers
{
    [TestClass]
    public class ReservationApiControllerTests
    {
        [TestInitialize]
        public void Initializer()
        {
            _Request = GetRequest();
        }

        HttpRequestMessage _Request = null;
        
        [TestMethod]
        public void GetAvailableCars()
        {
            Mock<IInventoryService> mockInventoryService = new Mock<IInventoryService>();
            Mock<IRentalService> mockRentalService = new Mock<IRentalService>();

            Car[] cars = 
            {
                new Car() { CarId = 1 },
                new Car() { CarId = 2 }
            };

            mockInventoryService.Setup(obj => obj.GetAvailableCars(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(cars);

            ReservationApiController controller = new ReservationApiController(mockInventoryService.Object, mockRentalService.Object);

            HttpResponseMessage response = controller.GetAvailableCars(_Request, DateTime.Now, DateTime.Now.AddDays(1));

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);

            Car[] data = GetResponseData<Car[]>(response);

            Assert.IsTrue(data == cars);
        }

        HttpRequestMessage GetRequest()
        {
            HttpConfiguration config = new HttpConfiguration();
            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties["MS_HttpConfiguration"] = config;
            return request;
        }

        T GetResponseData<T>(HttpResponseMessage result)
        {
            ObjectContent<T> content = result.Content as ObjectContent<T>;
            if (content != null)
            {
                T data = (T)(content.Value);
                return data;
            }
            else
                return default(T);
        }
    }
}
