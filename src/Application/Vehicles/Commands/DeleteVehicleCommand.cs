using Application.Common.Interfaces;
using Application.Vehicles.Exceptions;
using Domain.Vehicles;
using LanguageExt;
using MediatR;

namespace Application.Vehicles.Commands;

public record DeleteVehicleCommand : IRequest<Either<VehicleException, Vehicle>>
{
    public required Guid Id { get; init; }
}

public class DeleteVehicleCommandHandler(IVehicleRepository vehicleRepository)
    : IRequestHandler<DeleteVehicleCommand, Either<VehicleException, Vehicle>>
{
    public async Task<Either<VehicleException, Vehicle>> Handle(
        DeleteVehicleCommand request, 
        CancellationToken cancellationToken)
    {
        var vehicleId = new VehicleId(request.Id);
        var vehicleOption = await vehicleRepository.GetById(vehicleId, cancellationToken);

        if (vehicleOption.IsNone)
        {
            return new VehicleNotFoundException(request.Id);
        }

        var vehicle = (Vehicle)vehicleOption;
        await vehicleRepository.Delete(vehicle, cancellationToken);

        return vehicle;
    }
}