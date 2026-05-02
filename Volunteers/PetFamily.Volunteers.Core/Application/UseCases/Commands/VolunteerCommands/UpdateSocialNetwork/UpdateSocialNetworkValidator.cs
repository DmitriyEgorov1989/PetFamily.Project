using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateSocialNetwork;

public class UpdateSocialNetworkValidator : AbstractValidator<UpdateSocialNetworkCommand>
{
    public UpdateSocialNetworkValidator()
    {
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
        RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
        RuleFor(c => c.SocialNetworks).Custom((socialNetworks, context) =>
        {
            if (socialNetworks is null || !socialNetworks.Any())
                context.AddFailure(GeneralErrors.ValueIsInvalid(
                    nameof(UpdateSocialNetworkCommand.SocialNetworks)).Serialize());
        });

        RuleForEach(c => c.SocialNetworks)
            .MustBeValueObject(sn => SocialNetwork.Create(sn.Name, sn.Link))
            .When(c => c.SocialNetworks is not null
                       && c.SocialNetworks.Any());
    }
}