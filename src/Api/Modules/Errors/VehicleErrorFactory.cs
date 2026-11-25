using Application.Vehicles.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class VehicleErrorFactory
{
    public static ObjectResult ToObjectResult(VehicleException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                VehicleAlreadyExistsException => StatusCodes.Status409Conflict,
                VehicleNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            }
        };
    }
}