using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Extensions.Validations;
using Primitives;

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