using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.ChangeStatusPet;

public class ChangeStatusPetValidator : AbstractValidator<ChangeStatusPetCommand>
{
    public ChangeStatusPetValidator()
    {
        RuleFor(x => x.VolunteerId)
            .MustBeValueObject(VolunteerId.Create);
        RuleFor(x => x.PetId)
            .MustBeValueObject(PetId.Create);
        RuleFor(x => x.HelpStatus)
            .InclusiveBetween(0, 4)
            .WithError(GeneralErrors.ValueIsInvalid(nameof(MakeMainPhotoPetCommand.PathStorage)));
    }
}