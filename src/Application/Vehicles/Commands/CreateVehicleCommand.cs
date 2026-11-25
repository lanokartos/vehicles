using Application.Common.Interfaces;
using Application.Vehicles.Exceptions;
using Domain.Vehicles;
using LanguageExt;
using MediatR;

namespace Application.Vehicles.Commands;

public record CreateVehicleCommand : IRequest<Either<VehicleException, Vehicle>>
{
    public required string Name { get; init; }
    public required decimal PricePerDay { get; init; }
    public required Guid UserId { get; init; }
    public bool IsActive { get; init; } = true;
}

public class CreateVehicleCommandHandler(IVehicleRepository vehicleRepository) :
    IRequestHandler<CreateVehicleCommand, Either<VehicleException, Vehicle>>
{
    public async Task<Either<VehicleException, Vehicle>> Handle(
        CreateVehicleCommand request, 
        CancellationToken cancellationToken)
    {
        var existingVehicle = await vehicleRepository.GetByName(request.Name, cancellationToken);
        
        if (existingVehicle.IsSome)
        {
            return new VehicleAlreadyExistsException(request.Name);
        }

        var vehicle = Vehicle.Create(
            VehicleId.New(),
            request.Name,
            request.PricePerDay,
            request.UserId,
            request.IsActive);

        return await vehicleRepository.AddAsync(vehicle, cancellationToken);
    }
}