using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateMainInfo;

public class UpdateMainInfoVolunteerValidator : AbstractValidator<UpdateMainInfoVolunteerCommand>
{
    public UpdateMainInfoVolunteerValidator()
    {
        RuleFor(c => c.VolunteerId).NotEmpty();

        RuleFor(c => c.UpdateMainInfo).Custom((updateMainInfo, context) =>
        {
            if (updateMainInfo is null)
                return;

            var hasAnyValue =
                !string.IsNullOrWhiteSpace(updateMainInfo.FullName.FirstName) ||
                !string.IsNullOrWhiteSpace(updateMainInfo.FullName.MiddleName) ||
                !string.IsNullOrWhiteSpace(updateMainInfo.FullName.LastName);

            if (!hasAnyValue) context.AddFailure(GeneralErrors.ValueIsInvalid("FullName").Serialize());
        });

        RuleFor(c => c.UpdateMainInfo.Email).MustBeValueObject(Email.Create!)
            .When(c => !string.IsNullOrWhiteSpace(c.UpdateMainInfo.Email));

        RuleFor(c => c.UpdateMainInfo.Description)
            .MinimumLength(10).WithError(GeneralErrors.ValueIsInvalid("Description"))
            .When(c => c.UpdateMainInfo.Description is not null);

        RuleFor(c => c.UpdateMainInfo.Experience!.Value)
            .MustBeValueObject(Experience.Create)
            .When(u => u.UpdateMainInfo.Experience.HasValue);

        RuleFor(u => u.UpdateMainInfo.PhoneNumber)
            .MustBeValueObject(PhoneNumber.Create!).When(u => !string.IsNullOrWhiteSpace(u.UpdateMainInfo.PhoneNumber));
    }
}