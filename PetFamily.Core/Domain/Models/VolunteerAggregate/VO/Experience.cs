using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO
{
    /// <summary>
    /// Value Object - Опыт в годах
    /// </summary>
    public class Experience : ValueObject
    {
        [ExcludeFromCodeCoverage]
        private Experience() { }

        private Experience(int year)
        {
            Year = year;
        }
        public int Year { get; }

        public override string? ToString()
        {
            return $"{Year} лет";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Year;
        }

        public static Result<Experience, Error> Create(int year)
        {
            if (year <= 0)
                return GeneralErrors.ValueIsInvalid(nameof(year));
            return new Experience(year);
        }
    }
}