using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Accounts.Core.Ports;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetMe
{
    public class GetMeHandler :
         IRequestHandler<GetMeRequest, Result<GetMeResponse, ErrorList>>
    {
        private readonly IReadDataProvider _readDataProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger _logger;
        public GetMeHandler(IReadDataProvider readDataProvider,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger logger)
        {
            _readDataProvider = readDataProvider;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<Result<GetMeResponse, ErrorList>> Handle(
            GetMeRequest request, CancellationToken cancellationToken)
        {
            var resultGetStoredRefreshToken =
                 await _refreshTokenRepository.GetByTokenAsync(
                     request.RefreshToken, cancellationToken);

            if (resultGetStoredRefreshToken.IsFailure)
            {
                _logger.Error(
                    resultGetStoredRefreshToken.Error.Message, request.RefreshToken);
                return (ErrorList)resultGetStoredRefreshToken.Error;
            }
            var storedRefreshToken = resultGetStoredRefreshToken.Value!;

            var userId =
                storedRefreshToken.UserId;

            var resultGetuserInfo =
                await _readDataProvider.GetUserInfoByIdAsync(userId, cancellationToken);
            if (resultGetuserInfo.IsFailure)
            {
                _logger.Error("User with id {UserId} not found", userId);
                return (ErrorList)resultGetuserInfo.Error;
            }
            var userInfo = resultGetuserInfo.Value;
            return new GetMeResponse(userInfo);

        }
    }
}