using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;

namespace PetFamily.Core.Application.UseCases.Queries.Files.GetPresignedUrl
{
    public class GetPresignedUrlHandler :
        IRequestHandler<GetPresignedUrlRequest, Result<GetPresignedUrlResponse, Error>>
    {
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly ILogger _logger;

        public GetPresignedUrlHandler(IFileStorageProvider fileStorageProvider, ILogger logger)
        {
            _fileStorageProvider = fileStorageProvider;
            _logger = logger;
        }

        public async Task<Result<GetPresignedUrlResponse, Error>> Handle(
            GetPresignedUrlRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                return GeneralErrors.ValueIsInvalid(nameof(request));

            var resultGetPresignedUrl =
                await _fileStorageProvider.GetPresignedUrlAync(request.FileName, cancellationToken);

            if (resultGetPresignedUrl.IsFailure)
                return resultGetPresignedUrl.Error;

            return new GetPresignedUrlResponse(resultGetPresignedUrl.Value);
        }
    }
}