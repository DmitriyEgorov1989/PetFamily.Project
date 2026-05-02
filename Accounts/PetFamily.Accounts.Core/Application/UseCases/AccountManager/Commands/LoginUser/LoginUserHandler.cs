using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Core.Domain.Models.AccountAggregate;
using PetFamily.Accounts.Core.Ports;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using Serilog;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<string, Error.ErrorList>>
{
    private readonly ILogger _logger;
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<LoginUserCommand> _validator;

    public LoginUserHandler(ILogger logger,
        UserManager<User> userManager,
        IValidator<LoginUserCommand> validator, ITokenProvider tokenProvider)
    {
        _logger = logger;
        _userManager = userManager;
        _validator = validator;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<string, Error.ErrorList>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid)
            return resultValidation.ToValidationErrorResponse(command);

        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
        {
            _logger.Warning("User with email {Email} not found", command.Email);
            return (Error.ErrorList)new Error(
                "user.not.found",
                $"User witt Email{command.Email} not found",
                ErrorType.NotFound);
        }

        var checkPassword =
            await _userManager.CheckPasswordAsync(user, command.Password);
        if (!checkPassword)
        {
            _logger.Warning("Invalid password for user with email {Email}",
                command.Email);
            return (Error.ErrorList)GeneralErrors.InvalidCredentials();
        }

        _logger.Information("User with email {Email} logged in successfully",
            command.Email);

        var token = _tokenProvider.GenerateAccessToken(user);

        return token;
    }
}