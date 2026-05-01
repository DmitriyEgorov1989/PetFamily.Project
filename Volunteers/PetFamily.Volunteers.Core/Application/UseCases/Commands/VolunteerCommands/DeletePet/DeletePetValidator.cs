using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Extensions.Validations;

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