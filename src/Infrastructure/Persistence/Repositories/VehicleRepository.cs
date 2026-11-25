using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Domain.Vehicles;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleRepository(ApplicationDbContext context) : IVehicleRepository, IVehicleQueries
{
    public async Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await context.Vehicles.AddAsync(vehicle, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task<IReadOnlyList<Vehicle>> GetAll(CancellationToken cancellationToken = default)
    {
        return await context.Vehicles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Vehicle>> GetById(VehicleId id, CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle ?? Option<Vehicle>.None;
    }
    
    public async Task<Option<Vehicle>> GetByName(string name, CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Name == name, cancellationToken);

        return vehicle ?? Option<Vehicle>.None;
    }

    public async Task<Vehicle> Update(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Update(vehicle);
        await context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task Delete(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Remove(vehicle);
        await context.SaveChangesAsync(cancellationToken);
    }
}
