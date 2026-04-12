using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO
{
    /// <summary>
    /// Value object значение полного имени волонтера
    /// </summary>
    public sealed class FullName : ValueObject
    {
        public const int MAX_LENGTH_FULLNAME = 100;

        [ExcludeFromCodeCoverage]
        private FullName() { }

        private FullName(string name, string middleName, string lastName)
        {
            FirstName = name;
            MiddleName = middleName;
            LastName = lastName;
        }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; }

        public static Result<FullName, Error> Create(string firstName, string middleName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return GeneralErrors.ValueIsInvalid(nameof(firstName));
            if (string.IsNullOrWhiteSpace(middleName))
                return GeneralErrors.ValueIsInvalid(nameof(middleName));
            if (string.IsNullOrWhiteSpace(lastName))
                return GeneralErrors.ValueIsInvalid(nameof(lastName));

            return new FullName(firstName, middleName, lastName);
        }
        public override string? ToString()
        {
            return $"{LastName} {FirstName} {MiddleName}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LastName;
            yield return FirstName;
            yield return MiddleName;
        }
    }
}