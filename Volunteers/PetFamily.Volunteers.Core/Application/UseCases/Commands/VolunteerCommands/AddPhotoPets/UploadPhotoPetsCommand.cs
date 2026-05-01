using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.SharedKernelDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets;

public record UploadPhotoPetsCommand(
    Guid VolunteerId,
    Guid PetId,
    List<CreateFileDto> FileDtos)
    : IRequest<Result<Guid, ErrorList>>;