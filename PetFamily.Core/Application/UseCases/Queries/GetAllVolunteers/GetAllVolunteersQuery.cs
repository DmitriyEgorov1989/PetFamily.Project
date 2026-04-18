using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;


namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteers;

public sealed class GetAllVolunteersQuery : IRequest<Result<GetAllVolunteersResponse, ErrorList>>;