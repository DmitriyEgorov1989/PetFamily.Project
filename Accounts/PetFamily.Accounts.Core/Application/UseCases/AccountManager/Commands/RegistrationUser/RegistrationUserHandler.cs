using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Core.Application.Extensions;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.Accounts.Core.Ports;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Extensions.Validations;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;

public class RegistrationUserHandler : IRequestHandler<RegistrationUserCommand, Result<Guid, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly UserManager<User> _userManager;
    private readonly IReadDataProvider _readDataProvider;
    private readonly IValidator<RegistrationUserCommand> _validator;


    public RegistrationUserHandler(UserManager<User> userManager,
        ILogger logger,
        IUnitOfWork unitOfWork, IValidator<RegistrationUserCommand> validator,
        IReadDataProvider readDataProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
        _readDataProvider = readDataProvider;
    }

    public async Task<Result<Guid, ErrorList>> Handle(RegistrationUserCommand command,
        CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid)
            return resultValidation.ToValidationErrorResponse(command);

        var resultGetRole =
            await _readDataProvider.GetRoleByNameAsync(RolesName.Participant, cancellationToken);
        if (resultGetRole.IsFailure)
            return (ErrorList)resultGetRole.Error;
        var role = resultGetRole.Value;

        var user =
            User.Create(
                Guid.NewGuid(),
                command.FullName,
                command.Email,
                command.UserName,
                command.PhoneNumber).Value;

        var roleParticipant = Participant.Create(user.Id);

        var resultAddRoleUser = user.AddRole(role, null, roleParticipant, null);

        if (resultAddRoleUser.IsFailure)
        {
            _logger.Warning("Error add Role Users , {error}", resultAddRoleUser.Error.Message);
        }

        var resultRegistration =
            await _userManager.CreateAsync(user, command.Password);

        if (!resultRegistration.Succeeded)
        {
            _logger.Error("User registration failed: {Errors}",
                resultRegistration.Errors);
            var result = resultRegistration.ToErrorList();
            return result;
        }

        _logger.Information("User registration succeeded: {UserId}",
            user.Id);

        return user.Id;
    }
}