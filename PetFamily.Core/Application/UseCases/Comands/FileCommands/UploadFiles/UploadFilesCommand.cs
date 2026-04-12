using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Comands.FileCommands.UploadFiles
{
    public record UploadFilesCommand(List<CreateFileDto> FileDtos) : IRequest<UnitResult<Error>>;
}