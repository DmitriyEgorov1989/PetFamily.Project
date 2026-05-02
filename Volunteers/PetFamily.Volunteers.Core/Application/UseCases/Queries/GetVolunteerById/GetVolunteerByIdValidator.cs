using FluentValidation;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetVolunteerById;

public class GetVolunteerByIdValidator : AbstractValidator<GetVolunteerByIdQuery>
{
    public GetVolunteerByIdValidator()
    {
        RuleFor(q => q.VolunteerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithError(GeneralErrors.ValueIsInvalid("VolunteerId"));
    }
}