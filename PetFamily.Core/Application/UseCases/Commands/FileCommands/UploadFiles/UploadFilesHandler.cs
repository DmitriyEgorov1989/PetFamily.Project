using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;

namespace PetFamily.Core.Application.UseCases.Comands.FileCommands.UploadFiles
{
    public class UploadFilesHandler : IRequestHandler<UploadFilesCommand, UnitResult<Error>>
    {
        public const int RATE_LIMITE_THREAD = 3;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly ILogger _logger;

        public UploadFilesHandler(IFileStorageProvider fileStorageProvider, ILogger logger)
        {
            _fileStorageProvider = fileStorageProvider;
            _logger = logger;
        }

        public async Task<UnitResult<Error>> Handle
            (UploadFilesCommand request, CancellationToken cancellationToken)
        {
            if (request is null || !request.FileDtos.Any())
            {
                return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(request)));
            }
            var semaphoreSlim = new SemaphoreSlim(RATE_LIMITE_THREAD);

            var uploadTasks = request.FileDtos.Select(async file =>
            {
                try
                {
                    semaphoreSlim.Wait();
                    await _fileStorageProvider.UploadAsync(file, cancellationToken);
                    return file.FileData.FileName;
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });
            await Task.WhenAll(uploadTasks);

            return UnitResult.Success<Error>();
        }
    }
}
