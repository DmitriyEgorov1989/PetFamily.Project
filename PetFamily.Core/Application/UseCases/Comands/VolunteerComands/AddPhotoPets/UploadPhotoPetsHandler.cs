using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPhoto;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPhotoPets
{
    public class UploadPhotoPetsHandler : IRequestHandler<UploadPhotoPetsCommand, Result<Guid, ErrorList>>
    {
        private const int RATE_LIMITE_THREAD = 5;

        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly IValidator<UploadPhotoPetsCommand> _validator;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UploadPhotoPetsHandler(
            IValidator<UploadPhotoPetsCommand> validator,
            IVolunteerRepository volunteerRepository,
            IFileStorageProvider fileStorageProvider,
            ILogger logger,
            IUnitOfWork unitOfWork)
        {
            _volunteerRepository = volunteerRepository;
            _fileStorageProvider = fileStorageProvider;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid, ErrorList>> Handle(
            UploadPhotoPetsCommand command, CancellationToken cancellationToken)
        {
            var resultValidation = await _validator.ValidateAsync(command, cancellationToken);
            if (!resultValidation.IsValid)
            {
                return resultValidation.ToValidationErrorResponse(command);
            }

            _logger.Information("Start handling {CommandName} with VolunteerId: {VolunteerId} and PetId: {PetId}",
                nameof(UploadPhotoPetsCommand), command.VolunteerId, command.PetId);

            var volunteerId = VolunteerId.Create(command.VolunteerId).Value;
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteer == null)
            {
                return (ErrorList)GeneralErrors.NotFound(nameof(volunteerId));
            }

            var resultGetPet = volunteer.GetPetById(PetId.Create(command.PetId).Value);
            if (resultGetPet.IsFailure)
            {
                _logger.Error(
                    "Pet with Id {PetId} not found for VolunteerId {VolunteerId}", command.PetId, command.VolunteerId);
                return (ErrorList)resultGetPet.Error;
            }

            var resultUploadPatsPhoto =
                await UploadPhotoInStorageAsync(command.FileDtos, cancellationToken);
            if (resultUploadPatsPhoto.IsFailure)
                return (ErrorList)resultUploadPatsPhoto.Error;

            var listUploadPhoto =
                resultUploadPatsPhoto.Value.Select(p =>
                {
                    var photo = PetPhoto.Create(p.Size, p.PathStorage).Value;
                    return photo;
                });
            var pet = resultGetPet.Value;

            pet.UpdatePetPhotos(listUploadPhoto);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return (Guid)pet.Id;
        }

        private async Task<Result<List<PetPhotoDto>, Error>> UploadPhotoInStorageAsync(
            List<CreateFileDto> fileDtos, CancellationToken cancellationToken)
        {
            try
            {
                var semaphoreSlim = new SemaphoreSlim(RATE_LIMITE_THREAD);

                _logger.Information("Start uploading {Count} photos to storage with rate limit of {RateLimit} threads",
                    fileDtos.Count, RATE_LIMITE_THREAD);

                var uploadTasks = fileDtos.Select(async file =>
                {
                    try
                    {
                        semaphoreSlim.Wait();
                        var resultUploadPhoto = await _fileStorageProvider.UploadAsync(file, cancellationToken);
                        if (resultUploadPhoto.IsFailure)
                        {
                            _logger.Error("Failed to upload photo {FileName} to storage: {ErrorMessage}",
                                file.FileName, resultUploadPhoto.Error.Message);

                            return GeneralErrors.InternalServerError
                            ($"Failed to upload photo {file.FileName} to storage: " +
                            $"{resultUploadPhoto.Error.Message}");
                        }
                        return resultUploadPhoto;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex,
                            "Exception occurred while uploading photo {FileName} to storage", file.FileName);
                        return GeneralErrors.InternalServerError(
                            $"Exception occurred while uploading photo {file.FileName} to storage: {ex.Message}");
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });

                var resultUploadPhotos = await Task.WhenAll(uploadTasks);
                foreach (var result in resultUploadPhotos)
                {
                    if (result.IsFailure)
                    {
                        _logger.Error("One or more photos failed to upload to storage");
                        return result.Error;
                    }
                }

                _logger.Information("Successfully uploaded {Count} photos to storage", fileDtos.Count);
                var petPhotos = resultUploadPhotos.Select(r => r.Value).ToList();
                return petPhotos;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error uploading photos to storage");
                return GeneralErrors.InternalServerError(ex.Message);
            }
        }
    }
}