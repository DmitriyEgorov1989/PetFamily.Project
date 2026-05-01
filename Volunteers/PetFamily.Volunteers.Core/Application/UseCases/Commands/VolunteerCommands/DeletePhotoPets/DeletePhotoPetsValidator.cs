using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeletePhotoPets;

public class DeletePhotoPetsValidator : AbstractValidator<DeletePhotoPetsCommand>
{
    public DeletePhotoPetsValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
        RuleFor(c => c.PetId).MustBeValueObject(PetId.Create);
        RuleFor(c => c.FileName).MustBeValueObject(PetPhoto.Create);
    }
}