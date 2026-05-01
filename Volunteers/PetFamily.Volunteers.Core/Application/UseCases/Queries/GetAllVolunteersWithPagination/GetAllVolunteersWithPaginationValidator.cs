using FluentValidation;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public class GetAllVolunteersWithPaginationValidator :
    AbstractValidator<GetAllVolunteersWithPaginationQuery>
{
    public GetAllVolunteersWithPaginationValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0)
            .WithError(GeneralErrors.ValueIsInvalid("PageNumber"));
        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, 100)
            .WithError(GeneralErrors.ValueIsInvalid("PageSize"));
    }
}