using CSharpFunctionalExtensions;
using Primitives;

namespace PetFamily.Core.Domain.Models.SharedKernel.VO
{
    public sealed class HelpRequisite : ValueObject
    {
        public const int MAX_LENGTH_NAME = 100;
        public const int MAX_LENGTH_DESCRIPTION = 1000;
        private HelpRequisite() { }

        private HelpRequisite(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public string Name { get; }
        public string Description { get; }

        public static Result<HelpRequisite, Error> Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                return GeneralErrors.ValueIsInvalid(nameof(description));

            return new HelpRequisite(name, description);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Description;
        }
    }
}