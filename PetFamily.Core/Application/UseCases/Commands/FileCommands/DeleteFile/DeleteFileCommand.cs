using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Comands.FileCommands.DeleteFile
{
    public record DeleteFileRequest(string FileName) : IRequest<UnitResult<Error>>;
}