using System.ComponentModel.DataAnnotations;
using Domain.Vehicles;

namespace Api.Dtos;

public record VehicleResponse(VehicleId Id, string Name, decimal PricePerDay, Guid UserId, bool IsActive, DateTime CreatedAt)
{
    public static VehicleResponse FromDomainModel(Vehicle vehicle) =>
        new VehicleResponse(vehicle.Id, vehicle.Name, vehicle.PricePerDay, vehicle.UserId, vehicle.IsActive, vehicle.CreatedAt);
}

public record CreateVehicleDto(
    string Name,
    decimal PricePerDay,
    Guid UserId,
    bool IsActive = true
    );

public record UpdateVehicleDto(
    string Name,
    decimal PricePerDay,
    bool IsActive
);
