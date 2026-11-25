using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Vehicles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Vehicles;

namespace Api.Tests.Integration.Vehicles;

public class VehicleControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Vehicle _firstTestVehicle;
    private readonly Vehicle _secondTestVehicle;
    private readonly Vehicle _thirdTestVehicle;
    private readonly string _createVehicleRoute;
    private readonly string _updateVehicleRoute;

    public VehicleControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstTestVehicle = VehiclesTestData.FirstTestVehicle();
        _secondTestVehicle = VehiclesTestData.SecondTestVehicle();
        _thirdTestVehicle = VehiclesTestData.ThirdTestVehicle();

        _createVehicleRoute = "vehicles";
        _updateVehicleRoute = $"vehicles/{_secondTestVehicle.Id}";
    }
    
    [Fact]
    public async Task ShouldNotCreateVehicleBecauseRequestIsInvalid()
    {
        var invalidRequest = new CreateVehicleDto(
            Name: string.Empty,
            PricePerDay: -10m,
            UserId: _firstTestVehicle.UserId,
            IsActive: true);

        var response = await Client.PostAsJsonAsync(_createVehicleRoute, invalidRequest);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbVehicle = await Context.Vehicles.FirstOrDefaultAsync(x => x.PricePerDay == -10m);
        dbVehicle.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldUpdateVehicle()
    {
        var request = new UpdateVehicleDto(
            Name: _thirdTestVehicle.Name,
            PricePerDay: _thirdTestVehicle.PricePerDay,
            IsActive: _thirdTestVehicle.IsActive);

        var response = await Client.PutAsJsonAsync(_updateVehicleRoute, request);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseDto = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().Be(_secondTestVehicle.Id);

        var dbVehicle = await Context.Vehicles.FirstOrDefaultAsync(x => x.Id == _secondTestVehicle.Id);
        dbVehicle.Should().NotBeNull();
        dbVehicle!.Name.Should().Be(_thirdTestVehicle.Name);
        dbVehicle.PricePerDay.Should().Be(_thirdTestVehicle.PricePerDay);
    }

    [Fact]
    public async Task ShouldNotUpdateVehicleBecauseNameAlreadyTaken()
    {
        await Context.Vehicles.AddAsync(_firstTestVehicle);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        
        var request = new UpdateVehicleDto(
            Name: _firstTestVehicle.Name,
            PricePerDay: _thirdTestVehicle.PricePerDay,
            IsActive: _thirdTestVehicle.IsActive);
        
        var response = await Client.PutAsJsonAsync(_updateVehicleRoute, request);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldCreateVehicle()
    {
        var request = new CreateVehicleDto(
            Name: _firstTestVehicle.Name,
            PricePerDay: _firstTestVehicle.PricePerDay,
            UserId: _firstTestVehicle.UserId,
            IsActive: _firstTestVehicle.IsActive);
        
        var response = await Client.PostAsJsonAsync(_createVehicleRoute, request);
        
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var vehicleFromDb = await Context.Vehicles.FirstOrDefaultAsync(x => x.Name == _firstTestVehicle.Name);
        vehicleFromDb.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ShouldNotUpdateVehicleBecauseVehicleNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        var updateRoute = $"vehicles/{nonExistentId}";

        var request = new UpdateVehicleDto(
            Name: _thirdTestVehicle.Name,
            PricePerDay: _thirdTestVehicle.PricePerDay,
            IsActive: _thirdTestVehicle.IsActive);

        var response = await Client.PutAsJsonAsync(updateRoute, request);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteVehicle()
    {
        var response = await Client.DeleteAsync(_updateVehicleRoute);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var deletedVehicle = await Context.Vehicles.FirstOrDefaultAsync(x => x.Id == _secondTestVehicle.Id);
        deletedVehicle.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteVehicleBecauseVehicleNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        var deleteRoute = $"vehicles/{nonExistentId}";
        
        var response = await Client.DeleteAsync(deleteRoute);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var secondVehicle = await Context.Vehicles.FirstOrDefaultAsync(x => x.Id == _secondTestVehicle.Id);
        secondVehicle.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldNotCreateVehicleBecauseNameAlreadyTaken()
    {
        var request = new CreateVehicleDto(
            Name: _secondTestVehicle.Name,
            PricePerDay: _thirdTestVehicle.PricePerDay,
            UserId: _thirdTestVehicle.UserId,
            IsActive: _thirdTestVehicle.IsActive);
        
        var response = await Client.PostAsJsonAsync(_createVehicleRoute, request);
        
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        
        var vehicleFromDb = await Context.Vehicles.SingleAsync(x => x.Name == _secondTestVehicle.Name);
        vehicleFromDb.Should().NotBeNull();
    }

    public async Task InitializeAsync()
    {
        await Context.Vehicles.AddAsync(_secondTestVehicle);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Vehicles.RemoveRange(Context.Vehicles);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
    }
}