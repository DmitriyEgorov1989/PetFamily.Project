using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdatePet;

public class UpdatePetValidator : AbstractValidator<UpdatePetCommand>
{
    public UpdatePetValidator()
    {
        RuleFor(c => c.PetId).MustBeValueObject(PetId.Create);
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);

        RuleFor(c => c.Name)
            .MinimumLength(5).WithError(GeneralErrors.ValueIsInvalid("Name"))
            .When(c => !string.IsNullOrWhiteSpace(c.Name));

        RuleFor(c => c.Description)
            .MinimumLength(10).WithError(GeneralErrors.ValueIsInvalid("Description"))
            .When(c => !string.IsNullOrWhiteSpace(c.Description));

        RuleFor(c => c.Color).MustBeValueObject(Color.Create).When(c => !string.IsNullOrWhiteSpace(c.Color));

        RuleFor(c => c.HealthInfo).MustBeValueObject(HealthInfo.Create)
            .When(c => !string.IsNullOrWhiteSpace(c.HealthInfo));

        RuleFor(c => c.Address).Custom((address, context) =>
        {
            if (address is null)
                return;

            var hasAnyValue =
                !string.IsNullOrWhiteSpace(address.City) ||
                !string.IsNullOrWhiteSpace(address.Region) ||
                !string.IsNullOrWhiteSpace(address.House);

            if (!hasAnyValue) context.AddFailure(GeneralErrors.ValueIsInvalid("Address").Serialize());
        });

        RuleFor(c => c.Weight)
            .GreaterThan(0)
            .WithError(GeneralErrors.ValueIsInvalid("Weight"))
            .When(c => c.Weight.HasValue);

        RuleFor(c => c.Height)
            .GreaterThan(0)
            .WithError(GeneralErrors.ValueIsInvalid("Weight"))
            .When(c => c.Height.HasValue);

        RuleFor(c => c.PhoneNumber).MustBeValueObject(PhoneNumber.Create)
            .When(c => !string.IsNullOrWhiteSpace(c.PhoneNumber));
    }
}