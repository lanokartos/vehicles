namespace Application.Vehicles.Exceptions;

public abstract class VehicleException(string message) : Exception(message);

public class VehicleNotFoundException(Guid id) 
    : VehicleException($"Vehicle with id '{id}' was not found");

public class VehicleAlreadyExistsException(string name) 
    : VehicleException($"Vehicle with name '{name}' already exists");

public class VehicleUnhandledException(string message) 
    : VehicleException(message);