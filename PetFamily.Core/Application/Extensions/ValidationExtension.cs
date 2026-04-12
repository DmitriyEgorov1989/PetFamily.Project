using FluentValidation.Results;
using Primitives;
using static Primitives.Error;

namespace PetFamily.Core.Application.Extensions
{
    public static class ValidationExtension
    {
        public static ErrorList ToValidationErrorResponse<T>(this ValidationResult validationResult, T command)
        {
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
}