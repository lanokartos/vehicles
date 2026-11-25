namespace Domain.Vehicles;

public class Vehicle
{
    public VehicleId Id { get; }
    public string Name { get; private set; }
    public decimal PricePerDay { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }
    private Vehicle(VehicleId id, string name, decimal pricePerDay, Guid userId, bool isActive, DateTime createdAt) =>
        (Id, Name, PricePerDay, UserId, IsActive, CreatedAt) = (id, name, pricePerDay, userId, isActive, createdAt);

    public static Vehicle Create(VehicleId id, string name, decimal pricePerDay, Guid userId, bool isActive) =>
        new(id, name, pricePerDay, userId, isActive, DateTime.UtcNow);

    public void UpdateDetails(string name, decimal pricePerDay, bool isActive)
    {
        Name = name;
        PricePerDay = pricePerDay;
        IsActive = isActive;
    }
}