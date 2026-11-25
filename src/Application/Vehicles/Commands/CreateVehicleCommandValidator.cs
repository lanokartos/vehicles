using FluentValidation;

namespace Application.Vehicles.Commands;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters");
        
        RuleFor(x => x.PricePerDay)
            .InclusiveBetween(1m, 1_000_000m).WithMessage("Price per day must be between 1 and 1.000.000");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}