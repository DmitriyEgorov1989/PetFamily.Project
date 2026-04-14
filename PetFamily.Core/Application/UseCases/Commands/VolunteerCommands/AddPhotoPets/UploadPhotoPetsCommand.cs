using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets
{
    public record UploadPhotoPetsCommand(
        Guid VolunteerId, Guid PetId, List<CreateFileDto> FileDtos)
        : IRequest<Result<Guid, ErrorList>>;
}
