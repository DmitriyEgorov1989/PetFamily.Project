using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;

public class MakeMainPhotoPetValidator : AbstractValidator<MakeMainPhotoPetCommand>
{
    public MakeMainPhotoPetValidator()
    {
        RuleFor(x => x.VolunteerId)
            .MustBeValueObject(VolunteerId.Create);
        RuleFor(x => x.PetId)
            .MustBeValueObject(PetId.Create);
        RuleFor(x => x.PathStorage)
            .NotEmpty()
            .NotNull()
            .WithError(GeneralErrors.ValueIsInvalid(nameof(MakeMainPhotoPetCommand.PathStorage)));
    }
}