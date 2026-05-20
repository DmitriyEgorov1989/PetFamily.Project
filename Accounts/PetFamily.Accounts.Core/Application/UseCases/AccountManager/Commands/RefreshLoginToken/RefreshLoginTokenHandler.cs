using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Ports;
using PetFamily.Accounts.Infrastructure.Adapters.Jwt;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RefreshLoginToken
{
    public class RefreshLoginTokenHandler : IRequestHandler<RefreshLoginTokenCommand, Result<LoginResponse, ErrorList>>
    {
        private readonly IValidator<RefreshLoginTokenCommand> _validator;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<User> _userManager;

        public RefreshLoginTokenHandler(
            IValidator<RefreshLoginTokenCommand> validator,
            ILogger logger,
            IUnitOfWork unitOfWork,
            ITokenProvider tokenProvider,
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<User> userManager)
        {
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
        }

        public async Task<Result<LoginResponse, ErrorList>> Handle(
            RefreshLoginTokenCommand command, CancellationToken cancellationToken)
        {
            var resultValidation =
                await _validator.ValidateAsync(command, cancellationToken);

            if (!resultValidation.IsValid)
                return resultValidation.ToValidationErrorResponse(command);

            var resultGetValidateToken =
                await _tokenProvider.GetPrincipalFromToken(command.AccessToken, cancellationToken);
            if (resultGetValidateToken.IsFailure)
            {
                _logger.Error(
                    resultGetValidateToken.Error.Message, command.AccessToken);
                return (ErrorList)resultGetValidateToken.Error;
            }

            var validatedToken = resultGetValidateToken.Value;
            if (validatedToken == null)
            {
                _logger.Error("Error get claims principal for validate token,acess token invalid");
                return (ErrorList)GeneralErrors.Failure($"Token invalid {command.AccessToken}");
            }

            var jti = validatedToken.Claims
             .SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti))
            {
                _logger.Error("Error get Jti for validate token,acess token invalid");
                return (ErrorList)GeneralErrors.Failure($"Token invalid {command.AccessToken}");
            }

            var resultGetStoredRefreshToken =
                await _refreshTokenRepository.GetByTokenAsync(
                    command.RefreshToken, cancellationToken);

            if (resultGetStoredRefreshToken.IsFailure)
            {
                _logger.Error(
                    resultGetStoredRefreshToken.Error.Message, command.RefreshToken);
                return (ErrorList)resultGetStoredRefreshToken.Error;
            }
            var storedRefreshToken = resultGetStoredRefreshToken.Value!;

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                _logger.Error(
                    "Refresh token expired, refresh token: {RefreshToken}",
                    command.RefreshToken);
                return (ErrorList)GeneralErrors.Failure("Refresh token expired");
            }

            if (storedRefreshToken.Invalidated)
            {
                _logger.Error(
                    "Refresh token invalidated, refresh token: {RefreshToken}",
                    command.RefreshToken);
                return (ErrorList)GeneralErrors.Failure("Refresh token invalidated");
            }

            if (storedRefreshToken.JwtId != jti)
            {
                _logger.Error("This refresh token does not match this JWT: {RefreshToken}",
                    command.RefreshToken);
                return (ErrorList)GeneralErrors.Failure("Refresh token does not match this JWT");
            }

            var userId =
                validatedToken.Claims.FirstOrDefault(c => c.Type == CustomClaims.UserId);

            if (userId is null)
            {
                _logger.Error("Error get userId for validate token,acess token invalid");
                return (ErrorList)GeneralErrors.Failure($"Token invalid {command.AccessToken}");
            }

            var user = await _userManager.FindByIdAsync(userId.Value);
            if (user is null)
            {
                _logger.Error("Error get Jti for validate token,acess token invalid");
                return (ErrorList)GeneralErrors.NotFound(userId.ToString());
            }

            var acessToken = _tokenProvider.GenerateAccessToken(user);
            var resultGenerateRefreshToken =
                await _tokenProvider.GenerateRefreshToken(
                    acessToken, user, command.RefreshToken, cancellationToken);

            if (resultGenerateRefreshToken.IsFailure)
            {
                _logger.Error(resultGenerateRefreshToken.Error.Message);
                return (ErrorList)resultGenerateRefreshToken.Error;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse(acessToken, resultGenerateRefreshToken.Value);
        }
    }
}