using Domain.Vehicles;

namespace Tests.Data.Vehicles;

public class VehiclesTestData
{
    public static Vehicle FirstTestVehicle() => 
        Vehicle.Create(
            VehicleId.New(), 
            "Audi A6", 
            120m, 
            Guid.NewGuid(), 
            true);

    public static Vehicle SecondTestVehicle() => 
        Vehicle.Create(
            VehicleId.New(), 
            "BMW M5", 
            200m, 
            Guid.NewGuid(), 
            true);

    public static Vehicle ThirdTestVehicle() => 
        Vehicle.Create(
            VehicleId.New(), 
            "Mercedes E-Class", 
            180m, 
            Guid.NewGuid(), 
            true);
}