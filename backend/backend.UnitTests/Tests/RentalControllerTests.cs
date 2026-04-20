using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Controllers;
using backend.DTOs.Rental;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.UnitTests.Tests
{
    [TestClass]
    public sealed class RentalControllerTests
    {
        TestingEnvironment _environment;
        private RentalController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            _environment = TestHandler.CreateEnvironment();
            _controller = new RentalController(
                _environment.Context,
                _environment.AuthService,
                _environment.ResourceService,
                _environment.RentalService,
                _environment.FakeTimeProvider
            );
        }

        // TODO: Meg tesztek a tobbi endpointhoz
        
        [TestMethod]
        public async Task ChainTest()
        {
            // Nem lehet utolag berelni
            _environment.FakeTimeProvider.SetUtcNow(new DateTime(2026, 03, 22));
            // Itt az oradij 400, igy ez 14 + 24 + 10
            // 48 * 400 = 19 200 Ft
            // erre jon meg a szolgaltatas dija ami 5%
            // a teljes levont ar igy 20 160 Ft
            var rentalStart = new DateTime(2026, 04, 01, 10, 00, 00);
            var rentalEnd = new DateTime(2026, 04, 03, 10, 00, 00);

            var renter = _environment.Context.Users.First(x => x.Id == 1);
            var owner = _environment.Context.Users.First(x => x.Id == 2);
            
            #region CreateOffer
            _controller.SetAuthUser(1, UserRole.User);
            var postResult = await _controller!.Post(new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.RenterOffer,
            }) as CreatedResult;
            Assert.IsNotNull(postResult);

            var rental = await _environment.Context.Rentals
                .OrderByDescending(x => x.Id)
                .FirstAsync();
            var rentalId = rental.Id;

            Assert.AreEqual(RentalStatus.RenterOffer, rental.Status);
            #endregion

            #region OfferAccepted
            _controller.SetAuthUser(2, UserRole.User);
            var acceptResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.OfferAccepted,
            }) as OkObjectResult;
            Assert.IsNotNull(acceptResult);

            Assert.AreEqual(RentalStatus.OfferAccepted, rental.Status);
            Assert.AreEqual(30000 - 20160, renter.Balance);
            Assert.AreEqual(19200, owner.Balance);
            #endregion

            _environment.FakeTimeProvider.SetUtcNow(
                rentalStart.AddMinutes(5)
            );

            #region RenterPickupAccepted
            _controller.SetAuthUser(1, UserRole.User);
            var renterPickupResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.Active,
            }) as OkObjectResult;
            Assert.IsNotNull(renterPickupResult);

            Assert.AreEqual(RentalStatus.RenterPickupAccepted, rental.Status);
            #endregion

            #region OwnerPickupAccepted
            _controller.SetAuthUser(2, UserRole.User);
            var ownerPickupResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.Active,
            }) as OkObjectResult;
            Assert.IsNotNull(ownerPickupResult);

            Assert.AreEqual(RentalStatus.Active, rental.Status);
            #endregion

            #region RenterFinishAccepted
            _controller.SetAuthUser(1, UserRole.User);
            var renterFinishResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.Finished,
            }) as OkObjectResult;
            Assert.IsNotNull(renterFinishResult);

            Assert.AreEqual(RentalStatus.RenterFinishAccepted, rental.Status);
            #endregion

            #region OwnerFinishAccepted
            _controller.SetAuthUser(2, UserRole.User);
            var ownerFinishResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.Finished,
            }) as OkObjectResult;
            Assert.IsNotNull(ownerFinishResult);

            Assert.AreEqual(RentalStatus.Finished, rental.Status);
            #endregion

            #region RenterRatingOwner 
            _controller.SetAuthUser(1, UserRole.User);
            var renterRatingOwnerResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.Finished,
                OwnerRating = 5.0,
            }) as OkObjectResult;
            Assert.IsNotNull(renterRatingOwnerResult);

            Assert.AreEqual(5.0, rental.OwnerRating);
            #endregion

            #region OwnerRating 
            _controller.SetAuthUser(2, UserRole.User);
            var ownerRatingRenterResult = await _controller.Put(rentalId, new RentalDTO
            {
                VehicleId = 2,
                Start = rentalStart,
                End = rentalEnd,
                PickupLocation = "9700 Szombathely, Fő tér 1.",
                Status = RentalStatus.Finished,
                RenterRating = 4.5,
            }) as OkObjectResult;
            Assert.IsNotNull(ownerRatingRenterResult);

            Assert.AreEqual(4.5, rental.RenterRating);
            #endregion
        }
    }
}
