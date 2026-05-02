using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.Entity.Pet;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;

public class AddPetValidator : AbstractValidator<AddPetCommand>
{
    public AddPetValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);

        RuleFor(c => c.PetWrite.Name).Custom((value, context) =>
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                var error = GeneralErrors.ValueIsInvalid(nameof(value));

                context.AddFailure(error.Serialize());
            }
        });
        RuleFor(c => c.PetWrite.Description).Custom((value, context) =>
        {
            if (string.IsNullOrEmpty(value) || value.Length < 10)
            {
                var error = GeneralErrors.ValueIsInvalid(nameof(value));

                context.AddFailure(error.Serialize());
            }
        });
        RuleFor(c => c.PetWrite.SpeciesInfo).Custom((value, context) =>
        {
            if (BreedId.Create(value.BreedId).IsFailure
                || SpeciesId.Create(value.SpecieId).IsFailure)
            {
                var error = GeneralErrors.ValueIsInvalid(nameof(value));

                context.AddFailure(error.Serialize());
            }
        });
        RuleFor(c => c.PetWrite.Color).MustBeValueObject(Color.Create);
        RuleFor(c => c.PetWrite.HealthInfo).MustBeValueObject(HealthInfo.Create);
        RuleFor(c => c.PetWrite.Address)
            .MustBeValueObject(a => Address.Create(a.City, a.Region, a.House));
        RuleFor(c => c.PetWrite.Weight).GreaterThan(0)
            .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.Weight)));
        RuleFor(c => c.PetWrite.Height).GreaterThan(0)
            .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.Height)));
        RuleFor(c => c.PetWrite.PhoneNumber).MustBeValueObject(PhoneNumber.Create);
        RuleFor(c => c.PetWrite.BirthDate)
            .NotEqual(default(DateTime))
            .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.BirthDate)));
        RuleFor(c => c.PetWrite.PetHelpStatus).GreaterThan(0)
            .WithError(GeneralErrors.ValueIsInvalid(nameof(Pet.PetHelpStatus)));
    }
}