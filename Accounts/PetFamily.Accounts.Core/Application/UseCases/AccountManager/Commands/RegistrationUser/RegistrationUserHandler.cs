using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Core.Application.Extensions;
using PetFamily.Accounts.Core.Domain.Models.AccountAggregate;
using PetFamily.Accounts.Core.Ports;
using PetFamily.SharedKernel.Extensions.Validations;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;

public class RegistrationUserHandler : IRequestHandler<RegistrationUserCommand, Result<Guid, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegistrationUserCommand> _validator;


    public RegistrationUserHandler(UserManager<User> userManager,
        ILogger logger,
        IUnitOfWork unitOfWork, IValidator<RegistrationUserCommand> validator)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> Handle(RegistrationUserCommand command,
        CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid)
            return resultValidation.ToValidationErrorResponse(command);

        var user = new User
        {
            Email = command.Email,
            UserName = command.UserName
        };
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