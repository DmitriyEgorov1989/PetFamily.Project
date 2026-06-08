using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetMe
{
    public record class GetMeRequest(string RefreshToken) : IRequest<Result<GetMeResponse, ErrorList>>;
}