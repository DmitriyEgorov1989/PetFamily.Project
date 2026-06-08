using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Ports;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResponse, Error.ErrorList>>
{
    private readonly ILogger _logger;
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<LoginUserCommand> _validator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserHandler(ILogger logger,
        UserManager<User> userManager,
        IValidator<LoginUserCommand> validator, ITokenProvider tokenProvider,
        IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _validator = validator;
        _tokenProvider = tokenProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse, ErrorList>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid)
            return resultValidation.ToValidationErrorResponse(command);

        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
        {
            _logger.Warning("User with email {Email} not found", command.Email);
            return (ErrorList)new Error(
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
            return (ErrorList)GeneralErrors.InvalidCredentials();
        }

        _logger.Information("User with email {Email} logged in successfully",
            command.Email);

        var acessToken = _tokenProvider.GenerateAccessToken(user);

        var resultGenerateRefreshToken =
            await _tokenProvider.GenerateRefreshToken(acessToken, user, cancellationToken);

        if (resultGenerateRefreshToken.IsFailure)
        {
            _logger.Error(resultGenerateRefreshToken.Error.Message);
            return (ErrorList)resultGenerateRefreshToken.Error;
        }

        await _refreshTokenRepository.CreateTokenAsync(
            resultGenerateRefreshToken.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshToken = resultGenerateRefreshToken.Value;

        return new LoginResponse(acessToken, refreshToken.Token);
    }
}