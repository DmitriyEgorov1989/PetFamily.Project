using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.SharedKernel.DomainModels.VO;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;

public record RegistrationUserCommand(
    string Email,
    string UserName,
    FullName FullName,
    string PhoneNumber,
    string Password)
    : IRequest<Result<Guid, ErrorList>>;