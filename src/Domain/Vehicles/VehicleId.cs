namespace Domain.Vehicles;

public record VehicleId(Guid Value)
{
    public static VehicleId Empty() => new(Guid.Empty);
    public static VehicleId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}