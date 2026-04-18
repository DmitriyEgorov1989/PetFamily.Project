using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Ports.DataBaseForRead;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteers;

public class GetAllVolunteersHandler : IRequestHandler<GetAllVolunteersQuery, Result<GetAllVolunteersResponse, ErrorList>>
{
    private readonly IReadRepository _readRepository;
    private readonly ILogger _logger;

    public GetAllVolunteersHandler(IReadRepository readRepository, ILogger logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllVolunteersResponse, ErrorList>> Handle
        (GetAllVolunteersQuery request, CancellationToken cancellationToken)
    {
        var resultGetVolunteers =
            await _readRepository.GetAllVolunteersAsync(cancellationToken);
        if (resultGetVolunteers.IsFailure)
            return (ErrorList)resultGetVolunteers.Error;

        var listVolunteersDto = resultGetVolunteers.Value.ToList();

        return new GetAllVolunteersResponse(listVolunteersDto);

    }
}