using FluentValidation;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateHelpRequisites;

public class UpdateHelpRequisitesValidator : AbstractValidator<UpdateHelpRequisitesCommand>
{
    public UpdateHelpRequisitesValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
        RuleFor(c => c.HelpRequisites).Custom((socialNetworks, context) =>
        {
            if (socialNetworks is null || !socialNetworks.Any())
                context.AddFailure(GeneralErrors.ValueIsInvalid(nameof(UpdateHelpRequisitesCommand.HelpRequisites)).Serialize());
        });

        RuleForEach(c => c.HelpRequisites)
            .MustBeValueObject(hr => HelpRequisite.Create(hr.Name, hr.Description))
            .When(c => c.HelpRequisites is not null
                       && c.HelpRequisites.Any());
    }
}