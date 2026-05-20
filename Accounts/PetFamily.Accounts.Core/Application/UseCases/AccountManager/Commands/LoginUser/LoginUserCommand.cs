using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) :
    IRequest<Result<LoginResponse, ErrorList>>;