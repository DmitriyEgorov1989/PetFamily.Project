using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeleteVolunteer;

public class DeleteVolunteerValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
    }
}