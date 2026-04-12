using CSharpFunctionalExtensions;
using Primitives;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet
{
    public record HealthInfo
    {
        public HealthInfo(string description)
        {
            Description = description;
        }
        public string Description { get; }

        public static Result<HealthInfo, Error> Create(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return GeneralErrors.ValueIsInvalid(nameof(description));
            return new HealthInfo(description);
        }
    }
}