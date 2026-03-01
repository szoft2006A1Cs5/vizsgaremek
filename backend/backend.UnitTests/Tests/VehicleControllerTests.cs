using backend.Controllers;
using backend.DTOs.Vehicle;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.UnitTests.Tests;

[TestClass]
public class VehicleControllerTests
{
    TestingEnvironment _environment;
    private VehicleController? _controller;
    
    [TestInitialize]
    public void Initialize()
    {
        _environment = TestHandler.CreateEnvironment();
        _controller = new VehicleController(
            _environment.Context, 
            _environment.AuthService, 
            _environment.ResourceService
        );
    }
    
    [TestMethod]
    public async Task GetVehiclesPublic_Ok()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.GetVehicles() as OkObjectResult;
        Assert.IsNotNull(result);
        List<Vehicle> vehicles = JsonConvert.DeserializeObject<List<Vehicle>>((string)result.Value);
        Assert.IsNotNull(vehicles);

        foreach (var vehicle in vehicles)
        {
            Assert.IsNotNull(vehicle.Model); // Public
            Assert.IsNull(vehicle.LicensePlate); // In relation
            Assert.IsNull(vehicle.InsuranceNumber); // Owner Only
        }
    }

    [TestMethod]
    public async Task GetVehiclesPublic_BadRequest()
    {
        _controller.SetAuthUser(null, null);

        // Forditva vannak a datumok
        var result = await _controller!.GetVehicles(
            rentalStart: new DateTime(2026, 02, 11),
            rentalEnd: new DateTime(2026, 02, 09)
        ) as BadRequestResult;
        
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task GetVehicleByIdPublic_Ok()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.GetVehicleById(1) as OkObjectResult;
        Assert.IsNotNull(result);
        
        var vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);

        Assert.AreEqual("Toyota", vehicle.Manufacturer);
        Assert.AreEqual("Corolla", vehicle.Model);
        Assert.IsNull(vehicle.LicensePlate); // In relation
        Assert.IsNull(vehicle.InsuranceNumber); // Owner Only
    }
    
    [TestMethod]
    public async Task GetVehicleByIdInRelation_Ok()
    {
        _controller.SetAuthUser(2, UserRole.User);

        var result = await _controller!.GetVehicleById(1) as OkObjectResult;
        Assert.IsNotNull(result);
        
        var vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);

        Assert.AreEqual("Toyota", vehicle.Manufacturer);
        Assert.AreEqual("Corolla", vehicle.Model);
        Assert.AreEqual("ABC123", vehicle.LicensePlate); // In relation
        Assert.IsNull(vehicle.InsuranceNumber); // Owner Only
    }
    
    [TestMethod]
    public async Task GetVehicleByIdOwner_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.GetVehicleById(1) as OkObjectResult;
        Assert.IsNotNull(result);
        
        var vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);

        Assert.AreEqual("Toyota", vehicle.Manufacturer);
        Assert.AreEqual("Corolla", vehicle.Model);
        Assert.AreEqual("ABC123", vehicle.LicensePlate); // In relation
        Assert.AreEqual("KGFB12345678", vehicle.InsuranceNumber); // Owner Only
    }
    
    [TestMethod]
    public async Task GetVehicleByIdAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);

        var result = await _controller!.GetVehicleById(1) as OkObjectResult;
        Assert.IsNotNull(result);
        
        var vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);

        Assert.AreEqual("Toyota", vehicle.Manufacturer);
        Assert.AreEqual("Corolla", vehicle.Model);
        Assert.AreEqual("ABC123", vehicle.LicensePlate); // In relation
        Assert.AreEqual("KGFB12345678", vehicle.InsuranceNumber); // Owner Only
    }

    [TestMethod]
    public async Task GetVehicleById_NotFound()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.GetVehicleById(5) as NotFoundResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AddVehicle_Created()
    {
        _controller.SetAuthUser(3, UserRole.User);

        var result = await _controller!.AddVehicle(new VehicleDTO
        {
            VIN = "ARS213ARS12315AB28",
            LicensePlate = "EGH789",
            InsuranceNumber = "KGFB245810587",
            Manufacturer = "Hyundai",
            Model = "Tucson",
            Year = 2025,
            AvgFuelConsumption = 6.7,
            Description = "",
            OdometerReading = 15000,
        }) as CreatedResult;
        
        Assert.IsNotNull(result);
        Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);
        
        Assert.AreEqual("Hyundai", vehicle.Manufacturer);
        Assert.AreEqual("Tucson", vehicle.Model);
        Assert.AreEqual("EGH789", vehicle.LicensePlate); // In relation
        Assert.AreEqual("KGFB245810587", vehicle.InsuranceNumber); // Owner Only
    }

    [TestMethod]
    public async Task AddVehicle_BadRequest()
    {
        _controller.SetAuthUser(3, UserRole.User);

        var result = await _controller!.AddVehicle(new VehicleDTO
        {
            VIN = "a", // Nem jo formatumu a VIN
            LicensePlate = "EGH789",
            InsuranceNumber = "KGFB245810587",
            Manufacturer = "Hyundai",
            Model = "Tucson",
            Year = 2025,
            AvgFuelConsumption = 6.7,
            Description = "",
            OdometerReading = 15000,
        }) as BadRequestResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AddVehicle_Conflict()
    {
        _controller.SetAuthUser(3, UserRole.User);

        var result = await _controller!.AddVehicle(new VehicleDTO
        {
            VIN = "ARS213ARS12315AB28",
            LicensePlate = "ABC123", // Mar van ilyen rendszamu jarmu a rendszerben
            InsuranceNumber = "KGFB245810587",
            Manufacturer = "Hyundai",
            Model = "Tucson",
            Year = 2025,
            AvgFuelConsumption = 6.7,
            Description = "",
            OdometerReading = 15000,
        }) as ConflictResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AddVehicle_Unauthorized()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.AddVehicle(new VehicleDTO
        {
            VIN = "ARS213ARS12315AB28",
            LicensePlate = "EGH789",
            InsuranceNumber = "KGFB245810587",
            Manufacturer = "Hyundai",
            Model = "Tucson",
            Year = 2025,
            AvgFuelConsumption = 6.7,
            Description = "",
            OdometerReading = 15000,
        }) as UnauthorizedResult;
     
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateVehicle_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.UpdateVehicle(1, new VehicleDTO
        {
            AvgFuelConsumption = 6.2,
            Description = "",
            LicensePlate = "ABC123",
            InsuranceNumber = "KGFB20260301",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2019,
            OdometerReading = 12000,
            VIN = "ABCDEF123ARS12ABC1",
        }) as OkObjectResult;
        
        Assert.IsNotNull(result);
        Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);
        
        Assert.AreEqual("KGFB20260301", vehicle.InsuranceNumber);
    }

    [TestMethod]
    public async Task UpdateVehicle_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.UpdateVehicle(5, new VehicleDTO
        {
            AvgFuelConsumption = 6.2,
            Description = "",
            LicensePlate = "ABC123",
            InsuranceNumber = "KGFB20260301",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2019,
            OdometerReading = 12000,
            VIN = "ABCDEF123ARS12ABC1",
        }) as NotFoundResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateVehicle_BadRequest()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.UpdateVehicle(1, new VehicleDTO
        {
            AvgFuelConsumption = 6.2,
            Description = "",
            LicensePlate = "ABC12", // Hibas formatumu rendszam
            InsuranceNumber = "KGFB20260301",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2019,
            OdometerReading = 12000,
            VIN = "ABCDEF123ARS12ABC1A", // Eggyel tobb betu
        }) as BadRequestResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateVehicle_Conflict()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.UpdateVehicle(1, new VehicleDTO
        {
            AvgFuelConsumption = 6.2,
            Description = "",
            LicensePlate = "DEF456", // Mar van ilyen rendszamu auto a rendszerben
            InsuranceNumber = "KGFB20260301",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2019,
            OdometerReading = 12000,
            VIN = "ABCDEF123ARS12ABC1",
        }) as ConflictResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateVehicle_Forbidden()
    {
        // 2-es user nem ferhet hozza az 1-es jarmuvehez
        _controller.SetAuthUser(2, UserRole.User);

        var result = await _controller!.UpdateVehicle(1, new VehicleDTO
        {
            AvgFuelConsumption = 6.2,
            Description = "",
            LicensePlate = "ABC123",
            InsuranceNumber = "KGFB20260301",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2019,
            OdometerReading = 12000,
            VIN = "ABCDEF123ARS12ABC1",
        }) as ForbidResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateVehicleAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        
        var result = await _controller!.UpdateVehicle(1, new VehicleDTO
        {
            AvgFuelConsumption = 6.2,
            Description = "",
            LicensePlate = "ABC123",
            InsuranceNumber = "KGFB20260301",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2019,
            OdometerReading = 12000,
            VIN = "ABCDEF123ARS12ABC1",
        }) as OkObjectResult;
        
        Assert.IsNotNull(result);
        Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>((string)result.Value);
        Assert.IsNotNull(vehicle);
        
        Assert.AreEqual("KGFB20260301", vehicle.InsuranceNumber);
    }

    [TestMethod]
    public async Task GetAvailabilities_Ok()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.GetAvailabilities(1) as OkObjectResult;
        
        Assert.IsNotNull(result);
        var availabilities = JsonConvert.DeserializeObject<List<VehicleAvailability>>((string)result.Value);
        Assert.IsNotNull(availabilities);
     
        Assert.AreEqual(2, availabilities.Count);
        Assert.AreEqual(600, availabilities[0].HourlyRate);
        Assert.AreEqual(new DateTime(2026, 02, 27), availabilities[0].Start);
        Assert.AreEqual(new DateTime(2026, 03, 18), availabilities[0].End);
    }

    private async Task AddAvailability_Ok()
    {
        var result = await _controller!.AddAvailability(1, new VehicleAvailability
        {
            Start = new DateTime(2026, 07, 11),
            End = new DateTime(2026, 07, 21),
            HourlyRate = 750,
        }) as CreatedResult;
        
        Assert.IsNotNull(result);
        var availability = JsonConvert.DeserializeObject<VehicleAvailability>((string)result.Value);
        Assert.IsNotNull(availability);
        
        Assert.AreEqual(new DateTime(2026, 07, 11), availability.Start);
        Assert.AreEqual(new DateTime(2026, 07, 21), availability.End);
        Assert.AreEqual(750, availability.HourlyRate);
    }
    
    [TestMethod]
    public async Task AddAvailabilityOwner_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);
        await AddAvailability_Ok();
    }
    
    [TestMethod]
    public async Task AddAvailiabilityAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        await AddAvailability_Ok();
    }

    [TestMethod]
    public async Task AddAvailability_BadRequest()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.AddAvailability(1, new VehicleAvailability
        {
            End = new DateTime(2026, 07, 11),
            Start = new DateTime(2026, 07, 21),
            HourlyRate = 750,
        }) as BadRequestObjectResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AddAvailability_Conflict()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.AddAvailability(1, new VehicleAvailability
        {
            Start = new DateTime(2026, 03, 11),
            End = new DateTime(2026, 04, 21),
            HourlyRate = 750,
        }) as ConflictObjectResult;
        
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task AddAvailability_Unauthorized()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.AddAvailability(1, new VehicleAvailability
        {
            Start = new DateTime(2026, 07, 11),
            End = new DateTime(2026, 07, 21),
            HourlyRate = 750,
        }) as UnauthorizedResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AddAvailability_Forbidden()
    {
        _controller.SetAuthUser(3, UserRole.User);
        
        var result = await _controller!.AddAvailability(1, new VehicleAvailability
        {
            Start = new DateTime(2026, 07, 11),
            End = new DateTime(2026, 07, 21),
            HourlyRate = 750,
        }) as ForbidResult;
        
        Assert.IsNotNull(result);
    }


    [TestMethod]
    public async Task AddAvailability_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.AddAvailability(5, new VehicleAvailability
        {
            Start = new DateTime(2026, 07, 11),
            End = new DateTime(2026, 07, 21),
            HourlyRate = 750,
        }) as NotFoundResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetAvailability_Ok()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.GetAvailability(1, 1) as OkObjectResult;
        
        Assert.IsNotNull(result);
        var availability = JsonConvert.DeserializeObject<VehicleAvailability>((string)result.Value);
        Assert.IsNotNull(availability);
     
        Assert.AreEqual(600, availability.HourlyRate);
        Assert.AreEqual(new DateTime(2026, 02, 27), availability.Start);
        Assert.AreEqual(new DateTime(2026, 03, 18), availability.End);
    }
    
    [TestMethod]
    public async Task GetAvailability_NotFound()
    {
        _controller.SetAuthUser(null, null);
        var result = await _controller!.GetAvailability(1, 5) as NotFoundResult;
        Assert.IsNotNull(result);
    }

    private async Task UpdateAvailability_Ok()
    {
        var result = await _controller!.UpdateAvailability(1, 1, new VehicleAvailability
        {
            Start = new DateTime(2026, 02, 28),
            End = new DateTime(2026, 03, 11),
            HourlyRate = 750,
        }) as OkObjectResult;
        
        Assert.IsNotNull(result);
        var availability = JsonConvert.DeserializeObject<VehicleAvailability>((string)result.Value);
        Assert.IsNotNull(availability);
        
        Assert.AreEqual(750, availability.HourlyRate);
        Assert.AreEqual(new DateTime(2026, 02, 28), availability.Start);
        Assert.AreEqual(new DateTime(2026, 03, 11), availability.End);
    }

    [TestMethod]
    public async Task UpdateAvailabilityOwner_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);
        await UpdateAvailability_Ok();
    }
    
    [TestMethod]
    public async Task UpdateAvailabilityAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        await UpdateAvailability_Ok();
    }

    [TestMethod]
    public async Task UpdateAvailability_BadRequest()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.UpdateAvailability(1, 1, new VehicleAvailability
        {
            End = new DateTime(2026, 02, 28), // Felcserelve
            Start = new DateTime(2026, 03, 11),
            HourlyRate = 750,
        }) as BadRequestObjectResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateAvailability_Conflict()
    {
        _controller.SetAuthUser(1, UserRole.User);

        var result = await _controller!.UpdateAvailability(1, 1, new VehicleAvailability
        {
            Start = new DateTime(2026, 02, 07),
            End = new DateTime(2026, 03, 11),
            HourlyRate = 750,
        }) as ConflictObjectResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateAvailability_Unauthorized()
    {
        _controller.SetAuthUser(null, null);

        var result = await _controller!.UpdateAvailability(1, 1, new VehicleAvailability
        {
            Start = new DateTime(2026, 02, 28),
            End = new DateTime(2026, 03, 11),
            HourlyRate = 750,
        }) as UnauthorizedResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateAvailability_Forbidden()
    {
        _controller.SetAuthUser(2, UserRole.User);

        var result = await _controller!.UpdateAvailability(1, 1, new VehicleAvailability
        {
            Start = new DateTime(2026, 02, 28),
            End = new DateTime(2026, 03, 11),
            HourlyRate = 750,
        }) as ForbidResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateAvailability_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);
        
        var result = await _controller!.UpdateAvailability(1, 5, new VehicleAvailability
        {
            Start = new DateTime(2026, 02, 28),
            End = new DateTime(2026, 03, 11),
            HourlyRate = 750,
        }) as NotFoundResult;
        
        Assert.IsNotNull(result);
    }

    private async Task DeleteAvailability_Ok()
    {
        var result = await _controller!.DeleteAvailability(1, 1) as NoContentResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteAvailabilityOwner_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);
        await DeleteAvailability_Ok();
    }

    [TestMethod]
    public async Task DeleteAvailabilityAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        await DeleteAvailability_Ok();
    }

    [TestMethod]
    public async Task DeleteAvailability_Unauthorized()
    {
        _controller.SetAuthUser(null, null);
        var result = await _controller!.DeleteAvailability(1, 1) as UnauthorizedResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteAvailability_Forbidden()
    {
        _controller.SetAuthUser(2, UserRole.User);
        var result = await _controller!.DeleteAvailability(1, 1) as ForbidResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteAvailability_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);
        var result = await _controller!.DeleteAvailability(1, 5) as NotFoundResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetImages_Ok()
    {
        _controller.SetAuthUser(null, null);
        var result = await _controller!.GetImages(1) as OkObjectResult;
        Assert.IsNotNull(result);
        var images = JsonConvert.DeserializeObject<List<VehicleImage>>((string)result.Value);
        Assert.IsNotNull(images);
        
        Assert.AreEqual(2, images.Count);
    }

    private async Task AddImage_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);
        var result = await _controller!.AddImage(1, new FormFile(null, 0, 0, "test", "test.jpg")) as CreatedResult;
        Assert.IsNotNull(result);
        var image = JsonConvert.DeserializeObject<VehicleImage>((string)result.Value);
        Assert.IsNotNull(image);
        
        Assert.AreEqual(3, image.ImageId);
        Assert.IsNotNull(image.Path);
    }
    
    [TestMethod]
    public async Task AddImageOwner_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);
        await AddImage_Ok();
    }
    
    [TestMethod]
    public async Task AddImageAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        await AddImage_Ok();
    }
    
    [TestMethod]
    public async Task AddImage_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);
        var result = await _controller!.AddImage(5, new FormFile(null, 0, 0, "test", "test.jpg")) as NotFoundResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task AddImage_Unauthorized()
    {
        _controller.SetAuthUser(null, null);
        var result = await _controller!.AddImage(1, new FormFile(null, 0, 0, "test", "test.jpg")) as UnauthorizedResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AddImage_Forbidden()
    {
        _controller.SetAuthUser(2, UserRole.User);
        var result = await _controller!.AddImage(1, new FormFile(null, 0, 0, "test", "test.jpg")) as ForbidResult;
        Assert.IsNotNull(result);
    }

    private async Task UpdateImage_Ok()
    {
        var result = await _controller!.UpdateImage(1, 1, 212) as OkObjectResult;
        Assert.IsNotNull(result);
        var image = JsonConvert.DeserializeObject<VehicleImage>((string)result.Value);
        Assert.IsNotNull(image);
        
        Assert.AreEqual(212, image.SortIndex);
    }

    [TestMethod]
    public async Task UpdateImageOwner_Ok()
    {
        _controller.SetAuthUser(1, UserRole.User);
        await UpdateImage_Ok();
    }

    [TestMethod]
    public async Task UpdateImageAdmin_Ok()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        await UpdateImage_Ok();
    }

    [TestMethod]
    public async Task UpdateImage_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);
        var result = await _controller!.UpdateImage(1, 5, 212) as NotFoundResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task UpdateImage_Unauthorized()
    {
        _controller.SetAuthUser(null, null);
        var result = await _controller!.UpdateImage(1, 1, 212) as UnauthorizedResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task UpdateImage_Forbidden()
    {
        _controller.SetAuthUser(2, UserRole.User);
        var result = await _controller!.UpdateImage(1, 1, 212) as ForbidResult;
        Assert.IsNotNull(result);
    }

    private async Task DeleteImage_NoContent()
    {
        var result = await _controller!.DeleteImage(1, 1) as NoContentResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteImageOwner_NoContent()
    {
        _controller.SetAuthUser(1, UserRole.User);
        await DeleteImage_NoContent();
    }

    [TestMethod]
    public async Task DeleteImageAdmin_NoContent()
    {
        _controller.SetAuthUser(4, UserRole.Administrator);
        await DeleteImage_NoContent();
    }

    [TestMethod]
    public async Task DeleteImage_NotFound()
    {
        _controller.SetAuthUser(1, UserRole.User);
        var result = await _controller!.DeleteImage(1, 5) as NotFoundResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task DeleteImage_Unauthorized()
    {
        _controller.SetAuthUser(null, null);
        var result = await _controller!.DeleteImage(1, 1) as UnauthorizedResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteImage_Forbidden()
    {
        _controller.SetAuthUser(2, UserRole.User);
        var result = await _controller!.DeleteImage(1, 1) as ForbidResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ChainTest()
    {
        _controller.SetAuthUser(3, UserRole.User);
        
        #region AddVehicle
        _controller.SetAuthUser(3, UserRole.User);

        var addVehicleResult = await _controller!.AddVehicle(new VehicleDTO
        {
            VIN = "ARS213ARS12315AB28",
            LicensePlate = "EGH789",
            InsuranceNumber = "KGFB245810587",
            Manufacturer = "Hyundai",
            Model = "Tucson",
            Year = 2025,
            AvgFuelConsumption = 6.7,
            Description = "",
            OdometerReading = 15000,
        }) as CreatedResult;
        
        Assert.IsNotNull(addVehicleResult);
        Vehicle vehicleCreated = JsonConvert.DeserializeObject<Vehicle>((string)addVehicleResult.Value);
        Assert.IsNotNull(vehicleCreated);
        var vehicleId = vehicleCreated.Id;
        #endregion
        
        #region AddImage
        var imageRes = await _controller!.AddImage(
            vehicleId, 
            new FormFile(null, 0, 0, "test", "test.jpg")
        ) as CreatedResult;
        Assert.IsNotNull(imageRes);
        #endregion
        
        #region AddAvailability
        var addAvailabiltyRes = await _controller!.AddAvailability(vehicleId, new VehicleAvailability
        {
            Start = new DateTime(2026, 03, 02, 6, 00, 00),
            End = new DateTime(2026, 03, 07, 17, 00, 00),
            HourlyRate = 800,
        }) as CreatedResult;
        Assert.IsNotNull(addAvailabiltyRes);
        #endregion
        
        #region AddOtherAvailability
        var addOtherAvailabiltyRes = await _controller!.AddAvailability(vehicleId, new VehicleAvailability
        {
            Start = new DateTime(2026, 04, 02, 6, 00, 00),
            End = new DateTime(2026, 04, 07, 17, 00, 00),
            HourlyRate = 800,
        }) as CreatedResult;
        Assert.IsNotNull(addOtherAvailabiltyRes);
        #endregion
        
        #region UpdateOtherAvailability
        var updateOtherAvailRes = await _controller!.UpdateAvailability(vehicleId, 2, new VehicleAvailability
        {
            Start = new DateTime(2026, 04, 02, 6, 00, 00),
            End = new DateTime(2026, 04, 07, 17, 00, 00),
            HourlyRate = 1000,
        }) as OkObjectResult;
        Assert.IsNotNull(updateOtherAvailRes);
        #endregion
        
        #region GetOtherAvailability
        var getOtherAvailRes = await _controller!.GetAvailability(vehicleId, 2) as OkObjectResult;
        Assert.IsNotNull(getOtherAvailRes);
        var otherAvail = JsonConvert.DeserializeObject<VehicleAvailability>((string)getOtherAvailRes.Value);
        Assert.IsNotNull(otherAvail);
        Assert.AreEqual(1000, otherAvail.HourlyRate);
        #endregion
        
        #region DeleteOtherAvailability
        var delOtherAvailRes = await _controller!.DeleteAvailability(vehicleId, 2) as NoContentResult;
        Assert.IsNotNull(delOtherAvailRes);
        #endregion
        
        #region GetAvailabilities
        var getAvailsRes = await _controller!.GetAvailabilities(vehicleId) as OkObjectResult;
        Assert.IsNotNull(getAvailsRes);
        var avails = JsonConvert.DeserializeObject<List<VehicleAvailability>>((string)getAvailsRes.Value);
        Assert.IsNotNull(avails);
        Assert.AreEqual(1, avails.Count);
        #endregion
        
        #region AddImage
        var imgResult = await _controller!.AddImage(
            vehicleId,
            new FormFile(null, 0, 0, "test", "test.jpg")
        ) as CreatedResult;
        
        Assert.IsNotNull(imgResult);
        #endregion
        
        #region UpdateImage
        var updateResult = await _controller!.UpdateImage(vehicleId, 1, 123) as OkObjectResult;
        Assert.IsNotNull(updateResult);
        #endregion
        
        #region UpdateVehicle
        var updateVehicleResult = await _controller!.UpdateVehicle(vehicleId, new VehicleDTO
        {
            VIN = "ARS213ARS12315AB28",
            LicensePlate = "EGH789",
            InsuranceNumber = "KGFB245810587",
            Manufacturer = "Hyundai",
            Model = "Tucson",
            Year = 2025,
            AvgFuelConsumption = 6.7,
            Description = "",
            OdometerReading = 16000,
        }) as OkObjectResult;
        Assert.IsNotNull(updateVehicleResult);
        #endregion
        
        #region GetVehicle
        var getVehicleRes = await _controller!.GetVehicleById(vehicleId) as OkObjectResult;
        Assert.IsNotNull(getVehicleRes);
        var vehicle = JsonConvert.DeserializeObject<Vehicle>((string)getVehicleRes.Value);
        Assert.IsNotNull(vehicle);
        Assert.AreEqual("Hyundai", vehicle.Manufacturer);
        Assert.AreEqual("Tucson", vehicle.Model);
        Assert.AreEqual(16000, vehicle.OdometerReading);
        #endregion
    }
}
