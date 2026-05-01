using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Text.RegularExpressions;

namespace PetFamily.SharedKernel.DomainModels.VO;

public record Email
{
    private static readonly Regex EmailRegex = new(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)");

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