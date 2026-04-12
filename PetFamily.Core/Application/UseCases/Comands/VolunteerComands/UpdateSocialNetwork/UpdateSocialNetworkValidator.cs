using FluentValidation;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Extensions.Validations;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateSocialNetwork
{
    public class UpdateSocialNetworkValidator : AbstractValidator<UpdateSocialNetworkCommand>
    {
        public UpdateSocialNetworkValidator()
        {
            RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
            RuleFor(c => c.VolunteerId).MustBeValueObject(VolunteerId.Create);
            RuleFor(c => c.SocialNetworks).Custom((socialNetworks, context) =>
            {
                if (socialNetworks is null || !socialNetworks.Any())
                {
                    context.AddFailure(GeneralErrors.ValueIsInvalid(nameof(SocialNetworks)).Serialize());
                }
            });

            RuleForEach(c => c.SocialNetworks)
                .MustBeValueObject(sn => SocialNetwork.Create(sn.Name, sn.Link))
                .When(c => c.SocialNetworks is not null
                                   && c.SocialNetworks.Any());
        }
    }
}