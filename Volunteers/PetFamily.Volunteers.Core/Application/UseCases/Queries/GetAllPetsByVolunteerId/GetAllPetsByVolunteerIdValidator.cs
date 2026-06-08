using FluentValidation;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsByVolunteerId;

public class GetAllPetsByVolunteerIdValidator :
    AbstractValidator<GetAllPetsByVolunteerIdQuery>
{
    public GetAllPetsByVolunteerIdValidator()
    {
        RuleFor(q => q.VolunteerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithError(GeneralErrors.ValueIsInvalid("VolunteerId"));
    }
}
