using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.AccountManager.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<string, ErrorList>>;