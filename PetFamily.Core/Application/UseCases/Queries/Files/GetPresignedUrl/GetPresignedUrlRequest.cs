using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Queries.Files.GetPresignedUrl
{
    public record GetPresignedUrlRequest(string FileName) : IRequest<Result<GetPresignedUrlResponse, Error>>;
}
