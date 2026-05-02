using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeleteVolunteer;

public class DeleteVolunteerValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
    }
}