using FluentValidation;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Extensions.Validations;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.CreateVolunteer
{
    /// <summary>
    /// Класс валидации с помощью FluenValidation волонтера
    /// </summary>
    public class CreateVolunteerValidator : AbstractValidator<CreateVolunteerCommand>
    {
        public CreateVolunteerValidator()
        {
            RuleFor(c => c.FullName)
                .MustBeValueObject(fn => FullName.Create(fn.FirstName, fn.MiddleName, fn.LastName));
            RuleFor(c => c.Email).NotEmpty().MustBeValueObject(Email.Create);
            RuleFor(c => c.Description).Custom((value, context) =>
            {
                if (string.IsNullOrEmpty(value) || value.Length < 10)
                {
                    var error = GeneralErrors.ValueIsInvalid(nameof(value));

                    context.AddFailure(error.Serialize());
                }
            });
            RuleFor(c => c.Experience)
                .MustBeValueObject(Experience.Create);
            RuleFor(c => c.PhoneNumber)
                .MustBeValueObject(PhoneNumber.Create);
            RuleForEach(c => c.SocialNetworks)
                .MustBeValueObject(sn => SocialNetwork.Create(sn.Name, sn.Link));
            RuleForEach(c => c.HelpRequisites)
                .MustBeValueObject(hr => HelpRequisite.Create(hr.Name, hr.Description));
        }
    }
}