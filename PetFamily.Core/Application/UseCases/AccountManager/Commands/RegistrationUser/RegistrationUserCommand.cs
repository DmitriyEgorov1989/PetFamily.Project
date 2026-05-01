using CSharpFunctionalExtensions;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;

public record RegistrationUserCommand(
    string Email,
    string UserName,
    string Password)
    : IRequest<Result<Guid, ErrorList>>;