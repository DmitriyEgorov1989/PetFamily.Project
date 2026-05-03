using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.SharedKernelDto;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets;

public class UploadPhotoPetsHandler : IRequestHandler<UploadPhotoPetsCommand, Result<Guid, ErrorList>>
{
    private const int RATE_LIMITE_THREAD = 5;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger _logger;
    public readonly IMessageQueueService<IEnumerable<PetPhotoDto>> _queueService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UploadPhotoPetsCommand> _validator;

    private readonly IVolunteerRepository _volunteerRepository;

    public UploadPhotoPetsHandler(
        IValidator<UploadPhotoPetsCommand> validator,
        IVolunteerRepository volunteerRepository,
        IFileStorageProvider fileStorageProvider,
        ILogger logger,
        IUnitOfWork unitOfWork,
        IMessageQueueService<IEnumerable<PetPhotoDto>> queueService)
    {
        _volunteerRepository = volunteerRepository;
        _fileStorageProvider = fileStorageProvider;
        _validator = validator;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _queueService = queueService;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UploadPhotoPetsCommand command, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);
        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(command);

        _logger.Information("Start handling {CommandName} with VolunteerId: {VolunteerId} and PetId: {PetId}",
            nameof(UploadPhotoPetsCommand), command.VolunteerId, command.PetId);

        var volunteerId = VolunteerId.Create(command.VolunteerId).Value;
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteer == null) return (ErrorList)GeneralErrors.NotFound(nameof(volunteerId));

        var resultGetPet = volunteer.GetPetById(PetId.Create(command.PetId).Value);
        if (resultGetPet.IsFailure)
        {
            _logger.Error(
                "PetWrite with VolunteerId {PetId} not found for VolunteerId {VolunteerId}", command.PetId,
                command.VolunteerId);
            return (ErrorList)resultGetPet.Error;
        }

        var pet = resultGetPet.Value;

        var resultUploadPathsPhoto =
            await UploadPhotoInStorageAsync(command.FileDtos, cancellationToken);
        if (resultUploadPathsPhoto.IsFailure) return (ErrorList)resultUploadPathsPhoto.Error;
        var listUploadPhotos =
            resultUploadPathsPhoto.Value.Select(p =>
            {
                var photo = PetPhoto.Create(p.Size, p.PathStorage).Value;
                return photo;
            }).ToList();
        pet.UploadPetPhotos(listUploadPhotos);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (Guid)pet.Id;
    }

    private async Task<Result<List<PetPhotoDto>, Error>> UploadPhotoInStorageAsync(
        List<CreateFileDto> listPhotos, CancellationToken cancellationToken)
    {
        List<PetPhotoDto> listDtoUploadPhotos = new();
        try
        {
            var semaphoreSlim = new SemaphoreSlim(RATE_LIMITE_THREAD);

            _logger.Information("Start uploading {Count} photos to storage with rate limit of {RateLimit} threads",
                listPhotos.Count, RATE_LIMITE_THREAD);
            var uploadTasks = listPhotos.Select(async file =>
            {
                try
                {
                    await semaphoreSlim.WaitAsync(cancellationToken);

                    var resultUploadPhotos = await _fileStorageProvider.UploadAsync(file, cancellationToken);

                    if (resultUploadPhotos.IsFailure)
                    {
                        _logger.Error("Failed to upload photo {FileName} to storage: {ErrorMessage}",
                            file.FileData.FileName, resultUploadPhotos.Error.Message);

                        return UnitResult.Failure(GeneralErrors.InternalServerError
                        ($"Failed to upload photo {file.FileData.FileName} to storage: " +
                         $"{resultUploadPhotos.Error.Message}"));
                    }

                    var petPhotoDto = new PetPhotoDto(file.Stream.Length, resultUploadPhotos.Value);
                    listDtoUploadPhotos.Add(petPhotoDto);
                    return UnitResult.Success<Error>();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex,
                        "Exception occurred while uploading photo {FileName} to storage",
                        file.FileData.FileName);
                    return GeneralErrors.InternalServerError(
                        $"Exception occurred while uploading photo {file.FileData.FileName} to storage: {ex.Message}");
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });

            var resultUploadPhotos = await Task.WhenAll(uploadTasks);
            foreach (var result in resultUploadPhotos)
                if (result.IsFailure)
                {
                    if (!listDtoUploadPhotos.Any())
                        await _queueService.WriteAsync(listDtoUploadPhotos, cancellationToken);
                    _logger.Error("One or more photos failed to upload to storage");
                    return result.Error;
                }

            _logger.Information("Successfully uploaded {Count} photos to storage", listPhotos.Count);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error uploading photos to storage");
            return GeneralErrors.InternalServerError(ex.Message);
        }

        _logger.Information("All files download Success");
        return listDtoUploadPhotos;
    }
}