using FluentValidation;
using TrafficIncidentsOpitech.Domain.Enums;

namespace TrafficIncidentsOpitech.Application.Incidents.Commands.RegisterTrafficIncident;

public sealed class RegisterTrafficIncidentCommandValidator : AbstractValidator<RegisterTrafficIncidentCommand>
{
    public RegisterTrafficIncidentCommandValidator()
    {
        RuleFor(x => x.OccurredAt)
            .NotEmpty()
            .WithMessage("OccurredAt es requerido.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department es requerido.")
            .MaximumLength(100).WithMessage("Department no puede superar 100 caracteres.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City es requerido.")
            .MaximumLength(100).WithMessage("City no puede superar 100 caracteres.");

        RuleFor(x => x.VictimCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("VictimCount no puede ser negativo.");

        RuleFor(x => x.Vehicles)
            .NotNull().WithMessage("Vehicles es requerido.")
            .Must(v => v is not null && v.Count > 0)
            .WithMessage("Se requiere al menos un vehículo involucrado.");

        RuleForEach(x => x.Vehicles).ChildRules(v =>
        {
            v.RuleFor(x => x.VehicleType)
                .IsInEnum()
                .NotEqual(VehicleType.Unknown)
                .WithMessage("VehicleType es requerido.");

            v.RuleFor(x => x.Plate)
                .NotEmpty()
                .WithMessage("Plate es requerido.")
                .MaximumLength(20)
                .WithMessage("Plate no puede superar 20 caracteres.");

            v.RuleFor(x => x.Notes)
                .NotEmpty()
                .WithMessage("Notes es requerido.")
                .MaximumLength(200)
                .WithMessage("Notes no puede superar 200 caracteres.");
        });
    }
}
