using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePhotoPets;

public class DeletePhotoPetsHandler : IRequestHandler<DeletePhotoPetsCommand, Result<string, ErrorList>>
{
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeletePhotoPetsCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public DeletePhotoPetsHandler(
        IVolunteerRepository volunteerRepository,
        IFileStorageProvider fileStorageProvider,
        IValidator<DeletePhotoPetsCommand> validator,
        ILogger logger,
        IUnitOfWork unitOfWork)
    {
        _volunteerRepository = volunteerRepository;
        _fileStorageProvider = fileStorageProvider;
        _validator = validator;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string, ErrorList>> Handle(
        DeletePhotoPetsCommand command, CancellationToken cancellationToken)
    {
        var resultValidation =
            await _validator.ValidateAsync(command, cancellationToken);
        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(command);

        _logger.Information(
            "Start handling {CommandName} with VolunteerId: {VolunteerId} and PetId: {PetId}",
            nameof(DeletePhotoPetsCommand), command.VolunteerId, command.PetId);

        var volunteerId = VolunteerId.Create(command.VolunteerId).Value;

        cancellationToken.ThrowIfCancellationRequested();
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteer == null)
            return (ErrorList)GeneralErrors.NotFound(nameof(volunteerId));

        var resultGetPet = volunteer.GetPetById(PetId.Create(command.PetId).Value);
        if (resultGetPet.IsFailure)
        {
            _logger.Error(
                "PetWrite with VolunteerId {PetId} not found for VolunteerId {VolunteerId}",
                command.PetId, command.VolunteerId);
            return (ErrorList)resultGetPet.Error;
        }

        var pet = resultGetPet.Value;

        var resultDeletePhoto =
            pet.DeletePetPhotos(PetPhoto.Create(command.FileName).Value);

        cancellationToken.ThrowIfCancellationRequested();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _fileStorageProvider.DeleteFileAsync(command.FileName, cancellationToken);
        _logger.Information("File with name {fileName} delete at pet with id {petId}",
            command.FileName, command.PetId);

        return command.FileName;
    }
}