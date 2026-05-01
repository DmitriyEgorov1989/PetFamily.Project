using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Text.RegularExpressions;

namespace PetFamily.SharedKernel.DomainModels.VO;

public record PhoneNumber
{
    private static readonly Regex PhoneRegex =
        new("^((8|\\+7)[\\- ]?)?(\\(?\\d{3}\\)?[\\- ]?)?[\\d\\- ]{7,10}$", RegexOptions.Compiled);

    private PhoneNumber(string phoneNumber)
    {
        Value = phoneNumber;
    }

    public string Value { get; }

    public static Result<PhoneNumber, Error> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return GeneralErrors.ValueIsInvalid(nameof(phoneNumber));

        if (!PhoneRegex.IsMatch(phoneNumber))
            return GeneralErrors.ValueIsInvalid(nameof(phoneNumber));

        return new PhoneNumber(phoneNumber);
    }
}