using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePhotoPets;

public class DeletePhotoPetsValidator : AbstractValidator<DeletePhotoPetsCommand>
{
    public DeletePhotoPetsValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
        RuleFor(c => c.PetId).MustBeValueObject(PetId.Create);
        RuleFor(c => c.FileName).MustBeValueObject(PetPhoto.Create);
    }
}