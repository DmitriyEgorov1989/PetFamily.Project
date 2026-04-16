using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;

namespace PetFamily.Core.Application.UseCases.Comands.FileCommands.DeleteFile
{
    public class DeleteFileHandler : IRequestHandler<DeleteFileRequest, UnitResult<Error>>
    {
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly ILogger _logger;

        public DeleteFileHandler(IFileStorageProvider fileStorageProvider, ILogger logger)
        {
            _fileStorageProvider = fileStorageProvider;
            _logger = logger;
        }

        public async Task<UnitResult<Error>> Handle(DeleteFileRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(request)));

            var resultDeleteFile =
                await _fileStorageProvider.DeleteFileAsync(request.FileName, cancellationToken);

            if (resultDeleteFile.IsFailure)
                return UnitResult.Failure(resultDeleteFile.Error);

            return UnitResult.Success<Error>();
        }
    }
}