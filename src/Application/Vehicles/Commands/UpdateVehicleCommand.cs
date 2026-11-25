using Application.Common.Interfaces;
using Application.Vehicles.Exceptions;
using Domain.Vehicles;
using LanguageExt;
using MediatR;

namespace Application.Vehicles.Commands;

public record UpdateVehicleCommand : IRequest<Either<VehicleException, Vehicle>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal PricePerDay { get; init; }
    public bool IsActive { get; init; } = true;
}

public class UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository)
    : IRequestHandler<UpdateVehicleCommand, Either<VehicleException, Vehicle>>
{
    public async Task<Either<VehicleException, Vehicle>> Handle(
        UpdateVehicleCommand request, 
        CancellationToken cancellationToken)
    {
        var vehicleId = new VehicleId(request.Id);

        var vehicleOption = await vehicleRepository.GetById(vehicleId, cancellationToken);
        
        if (vehicleOption.IsNone)
        {
            return new VehicleNotFoundException(request.Id);
        }

        var vehicle = (Vehicle)vehicleOption;

        var duplicateOption = await vehicleRepository.GetByName(request.Name, cancellationToken);
        
        if (duplicateOption.IsSome)
        {
            var duplicate = (Vehicle)duplicateOption;
            if (duplicate.Id != vehicleId)
            {
                return new VehicleAlreadyExistsException(request.Name);
            }
        }

        vehicle.UpdateDetails(request.Name, request.PricePerDay, request.IsActive);
        await vehicleRepository.Update(vehicle, cancellationToken);

        return vehicle;
    }
}