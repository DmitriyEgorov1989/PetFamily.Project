using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) :
    IRequest<Result<string, ErrorList>>;