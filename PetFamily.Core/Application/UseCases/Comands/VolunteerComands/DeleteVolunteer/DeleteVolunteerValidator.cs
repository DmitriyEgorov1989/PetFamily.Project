using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Extensions.Validations;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeleteVolunteer
{
    public class DeleteVolunteerValidator : AbstractValidator<DeleteVolunteerCommand>
    {
        public DeleteVolunteerValidator()
        {
            RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
        }
    }
}
