using System.ComponentModel;
using Domain.Vehicles;
using LanguageExt;

namespace Application.Common.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<Vehicle> Update(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task Delete(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<Option<Vehicle>> GetById(VehicleId id, CancellationToken cancellationToken);
    Task<Option<Vehicle>> GetByName(string name, CancellationToken cancellationToken);
}