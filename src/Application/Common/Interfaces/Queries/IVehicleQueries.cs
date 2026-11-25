using Domain.Vehicles;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IVehicleQueries
{
    Task<Option<Vehicle>> GetById(VehicleId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Vehicle>> GetAll(CancellationToken cancellationToken = default);
}