using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RefreshLoginToken
{
    public record class RefreshLoginTokenCommand(string RefreshToken) :
        IRequest<Result<LoginResponse, ErrorList>>;
}