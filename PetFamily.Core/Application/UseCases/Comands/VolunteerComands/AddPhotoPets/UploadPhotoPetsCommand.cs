using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPhoto
{
    public record UploadPhotoPetsCommand(
        Guid VolunteerId, Guid PetId, List<CreateFileDto> FileDtos)
        : IRequest<Result<Guid, ErrorList>>;
}
