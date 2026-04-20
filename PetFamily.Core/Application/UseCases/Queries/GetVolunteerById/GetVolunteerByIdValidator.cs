using FluentValidation;
using PetFamily.Core.Extensions.Validations;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Queries.GetVolunteerById;

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