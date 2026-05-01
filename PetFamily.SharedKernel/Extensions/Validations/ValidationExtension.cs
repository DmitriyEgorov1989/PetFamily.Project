using FluentValidation.Results;
using PetFamily.SharedKernel.Errors;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.SharedKernel.Extensions.Validations;

public static class ValidationExtension
{
    public static ErrorList ToValidationErrorResponse<T>(this ValidationResult validationResult, T command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        List<Error> responseErrors = [];

        foreach (var validationError in validationResult.Errors)
        {
            var responseError =
                GeneralErrors.Validation(nameof(command), validationError.PropertyName);

            responseErrors.Add(responseError);
        }

        return responseErrors;
    }
}