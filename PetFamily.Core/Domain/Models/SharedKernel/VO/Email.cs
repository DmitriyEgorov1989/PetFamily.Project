using CSharpFunctionalExtensions;
using Primitives;
using System.Text.RegularExpressions;

namespace PetFamily.Core.Domain.Models.SharedKernel.VO
{
    public record Email
    {
        private static Regex EmailRegex =
            new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)");
        private Email(string email)
        {
            Address = email;
        }
        public string Address { get; }

        public static Result<Email, Error> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return GeneralErrors.ValueIsInvalid(nameof(email));

            if (!EmailRegex.IsMatch(email))
                return GeneralErrors.ValueIsInvalid(nameof(email));

            return new Email(email);
        }
    }
}