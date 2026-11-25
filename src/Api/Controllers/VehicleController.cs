using System.Collections.Immutable;
using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Vehicles.Commands;
using Domain.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("vehicles")]
public class VehicleController(IVehicleQueries vehicleQueries, ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VehicleResponse>>> GetAllVehicles(CancellationToken ct = default)
    {
        var vehicles = await vehicleQueries.GetAll(ct);
        var response = vehicles.Select(VehicleResponse.FromDomainModel).ToList();
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleResponse>> GetVehicle(Guid id, CancellationToken ct = default)
    {
        var vehicleOption = await vehicleQueries.GetById(new VehicleId(id), ct);

        return vehicleOption.Match<ActionResult<VehicleResponse>>(
            vehicle => Ok(VehicleResponse.FromDomainModel(vehicle)),
            () => NotFound()
        );
    }

    // 3. Створення авто через Either.Match (без try/catch)
    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> CreateVehicle(
        [FromBody] CreateVehicleDto dto, 
        CancellationToken ct = default)
    {
        var command = new CreateVehicleCommand
        {
            Name = dto.Name,
            PricePerDay = dto.PricePerDay,
            UserId = dto.UserId,
            IsActive = dto.IsActive
        };

        var result = await sender.Send(command, ct);

        return result.Match<ActionResult<VehicleResponse>>(
            vehicle =>
            {
                var response = VehicleResponse.FromDomainModel(vehicle);
                return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id.Value }, response);
            },
            error => VehicleErrorFactory.ToObjectResult(error)
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleResponse>> UpdateVehicle(
        Guid id, 
        [FromBody] UpdateVehicleDto dto, 
        CancellationToken ct = default)
    {
        var command = new UpdateVehicleCommand
        {
            Id = id,
            Name = dto.Name,
            PricePerDay = dto.PricePerDay,
            IsActive = dto.IsActive
        };

        var result = await sender.Send(command, ct);

        return result.Match<ActionResult<VehicleResponse>>(
            vehicle => Ok(VehicleResponse.FromDomainModel(vehicle)),
            error => VehicleErrorFactory.ToObjectResult(error)
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(Guid id, CancellationToken ct = default)
    {
        var command = new DeleteVehicleCommand { Id = id };
        var result = await sender.Send(command, ct);

        return result.Match<ActionResult>(
            _ => NoContent(),
            error => VehicleErrorFactory.ToObjectResult(error)
        );
    }
}