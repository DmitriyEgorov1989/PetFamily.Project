using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;

public record RegistrationUserCommand(
    string Email,
    string UserName,
    string Password)
    : IRequest<Result<Guid, ErrorList>>;