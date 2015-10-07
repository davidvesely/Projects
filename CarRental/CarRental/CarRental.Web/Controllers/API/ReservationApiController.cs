using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CarRental.Client.Contracts;
using CarRental.Client.Entities;
using CarRental.Web.Core;
using CarRental.Web.Models;
using Core.Common.Contracts;

namespace CarRental.Web.Controllers.API
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Authorize]
    [RoutePrefix("api/reservation")]
    [UsesDisposableService]
    public class ReservationApiController : ApiControllerBase
    {
        [ImportingConstructor]
        public ReservationApiController(IInventoryService inventoryService, IRentalService rentalService)
        {
            _InventoryService = inventoryService;
            _RentalService = rentalService;
        }

        IInventoryService _InventoryService;
        IRentalService _RentalService;

        protected override void RegisterServices(List<IServiceContract> disposableServices)
        {
            disposableServices.Add(_InventoryService);
            disposableServices.Add(_RentalService);
        }

        [HttpGet]
        //[AllowAnonymous]
        [Route("availablecars")]
        public HttpResponseMessage GetAvailableCars(HttpRequestMessage request, DateTime pickupDate, DateTime returnDate)
        {
            return GetHttpResponse(request, () =>
            {
                Car[] cars = _InventoryService.GetAvailableCars(pickupDate, returnDate);

                return request.CreateResponse<Car[]>(HttpStatusCode.OK, cars);
            });
        }

        [HttpPost]
        [Route("reservecar")]
        public HttpResponseMessage ReserveCar(HttpRequestMessage request, [FromBody]ReservationModel reservationModel)
        {
            return GetHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                string user = User.Identity.Name; // this method is secure to only the authenticated user to reserve
                Reservation reservation = _RentalService.MakeReservation(user, reservationModel.Car, reservationModel.PickupDate, reservationModel.ReturnDate);

                response = request.CreateResponse<Reservation>(HttpStatusCode.OK, reservation);
                
                return response;
            });
        }

        [HttpGet]
        [Route("getopen")]
        public HttpResponseMessage GetOpenReservations(HttpRequestMessage request)
        {
            return GetHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                string user = User.Identity.Name; // this method is secure to only the authenticated user to reserve
                CustomerReservationData[] reservations = _RentalService.GetCustomerReservations(user);

                response = request.CreateResponse<CustomerReservationData[]>(HttpStatusCode.OK, reservations);

                return response;
            });
        }

        [HttpPost]
        [Route("cancel")]
        public HttpResponseMessage CancelReservation(HttpRequestMessage request, [FromBody]int reservationId)
        {
            return GetHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                // not that calling the WCF service here will authenticate access to the data
                Reservation reservation = _RentalService.GetReservation(reservationId);
                if (reservation != null)
                {
                    _RentalService.CancelReservation(reservationId);
                    
                    response = request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "No reservation found under that ID.");

                return response;
            });
        }
    }
}
