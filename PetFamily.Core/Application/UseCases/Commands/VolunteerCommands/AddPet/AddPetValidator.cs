using FluentValidation;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;
using PetFamily.Core.Domain.Models.PetAggregate;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.SpeciesAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Extensions.Validations;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPet
{
    public class AddPetValidator : AbstractValidator<AddPetCommand>
    {
        public AddPetValidator()
        {
            RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);

            RuleFor(c => c.Pet.Name).Custom((value, context) =>
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    var error = GeneralErrors.ValueIsInvalid(nameof(value));

                    context.AddFailure(error.Serialize());
                }
            });
            RuleFor(c => c.Pet.Description).Custom((value, context) =>
            {
                if (string.IsNullOrEmpty(value) || value.Length < 10)
                {
                    var error = GeneralErrors.ValueIsInvalid(nameof(value));

                    context.AddFailure(error.Serialize());
                }
            });
            RuleFor(c => c.Pet.SpeciesInfo).Custom((value, context) =>
            {
                if (BreedId.Create(value.BreedId).IsFailure
                || SpeciesId.Create(value.SpecieId).IsFailure)
                {
                    var error = GeneralErrors.ValueIsInvalid(nameof(value));

                    context.AddFailure(error.Serialize());
                }
            });
            RuleFor(c => c.Pet.Color).MustBeValueObject(Color.Create);
            RuleFor(c => c.Pet.HealthInfo).MustBeValueObject(HealthInfo.Create);
            RuleFor(c => c.Pet.Address)
                .MustBeValueObject(a => Address.Create(a.City, a.Region, a.House));
            RuleFor(c => c.Pet.Weight).
                GreaterThan(0)
                .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.Weight)));
            RuleFor(c => c.Pet.Height).
                GreaterThan(0)
                .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.Height)));
            RuleFor(c => c.Pet.PhoneNumber).MustBeValueObject(PhoneNumber.Create);
            RuleFor(c => c.Pet.BirthDate)
                .NotEqual(default(DateTime))
                 .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.BirthDate)));
            RuleFor(c => c.Pet.PetHelpStatus).
                GreaterThan(0)
                .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.PetHelpStatus)));

        }
    }
}
