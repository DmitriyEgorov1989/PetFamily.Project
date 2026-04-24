using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;

public class DeletePetValidator : AbstractValidator<DeletePetCommand>
{
    public DeletePetValidator()
    {
        RuleFor(c => c.VolunteerId)
            .MustBeValueObject(VolunteerId.Create);
        RuleFor(c => c.PetId).MustBeValueObject(PetId.Create);
    }
}