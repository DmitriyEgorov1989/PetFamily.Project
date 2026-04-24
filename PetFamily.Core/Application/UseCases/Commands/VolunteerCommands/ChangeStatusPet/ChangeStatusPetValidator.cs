using FluentValidation;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Extensions.Validations;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.ChangeStatusPet;

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