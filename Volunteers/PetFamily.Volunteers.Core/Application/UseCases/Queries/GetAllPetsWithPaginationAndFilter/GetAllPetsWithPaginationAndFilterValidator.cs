using FluentValidation;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

public class GetAllPetsWithPaginationAndFilterValidator :
    AbstractValidator<GetAllPetsWithPaginationAndFilterQuery>
{
    public GetAllPetsWithPaginationAndFilterValidator()
    {
        RuleFor(q => q.PaginationData.Page)
            .GreaterThan(0)
            .WithError(
                GeneralErrors.ValueIsInvalid(
                    nameof(GetAllPetsWithPaginationAndFilterQuery.PaginationData.Page)));

        RuleFor(q => q.PaginationData.PageSize)
            .InclusiveBetween(1, 100)
            .WithError(
                GeneralErrors.ValueIsInvalid(
                    nameof(GetAllPetsWithPaginationAndFilterQuery.PaginationData.PageSize)));

        RuleFor(q => q.FilterByData!.Name);

        RuleFor(q => q.FilterByData!.Color).MustBeValueObject(Color.Create)
            .When(q =>
                !string.IsNullOrWhiteSpace(q.FilterByData!.Color));
        ;

        RuleFor(q => q.FilterByData.Address)
            .Custom((address, context) =>
            {
                if (address is null)
                    return;

                var hasAnyValue =
                    !string.IsNullOrWhiteSpace(address.City) ||
                    !string.IsNullOrWhiteSpace(address.Region) ||
                    !string.IsNullOrWhiteSpace(address.House);

                if (!hasAnyValue)
                    return;

                if (address.City?.Length < 2)
                    context.AddFailure(
                        GeneralErrors.ValueIsInvalid("City").Serialize());

                if (address.Region?.Length < 2)
                    context.AddFailure(
                        GeneralErrors.ValueIsInvalid("Region").Serialize());

                if (address.House?.Length < 1)
                    context.AddFailure(
                        GeneralErrors.ValueIsInvalid("House").Serialize());
            });

        RuleFor(q => q.FilterByData!.VolunteerId)
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsInvalid("VolunteerId"))
            .When(q => q.FilterByData is not null
                       && q.FilterByData.VolunteerId.HasValue);

        RuleFor(q => q.FilterByData!.SpeciesId)
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsInvalid("SpeciesId"))
            .When(q => q.FilterByData is not null
                       && q.FilterByData.SpeciesId.HasValue);

        RuleFor(q => q.FilterByData!.BreedId)
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsInvalid("BreedId"))
            .When(q => q.FilterByData is not null
                       && q.FilterByData.BreedId.HasValue);

        RuleFor(q => q.SortByData)
            .SetValidator(new SortByValidator()!).When(q => q.SortByData is not null);
    }

    public class SortByValidator : AbstractValidator<SortByData>
    {
        private static readonly string[] AllowedSortBy =
        [
            "name",
            "color",
            "birthdate",
            "createdutc"
        ];

        private static readonly string[] AllowedSortDirection =
        [
            "asc",
            "desc"
        ];

        public SortByValidator()
        {
            RuleFor(x => x.SortBy)
                .Must(x => AllowedSortBy.Contains(x!.Trim().ToLower()))
                .WithError(GeneralErrors.ValueIsInvalid("SortBy"))
                .When(q => !string.IsNullOrWhiteSpace(q.SortBy));

            RuleFor(x => x.SortDirection)
                .Must(x => AllowedSortDirection.Contains(x!.Trim().ToLower()))
                .WithError(GeneralErrors.ValueIsInvalid("SortDirection"))
                .When(q => !string.IsNullOrWhiteSpace(q.SortDirection));
        }
    }
}