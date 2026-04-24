using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Ports.DataBaseForRead;
using Primitives;
using Serilog;

namespace PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Repository.Pets
{
    public class PetsRepository : IPetsReadRepository
    {
        private readonly ILogger _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly PetsQueryBuilder _queryBuilder;

        public PetsRepository(ILogger logger,
            IDbConnectionFactory connectionFactory,
            PetsQueryBuilder queryBuilder)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _queryBuilder = queryBuilder;
        }

        public async Task<Result<List<PetDto>, Error>> GetAllWithPaginationAndFiltersAsync(
            PaginationData paginationData,
            FilterByData filterByData,
            SortByData sortByData,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var sql = _queryBuilder
                    .WithAddress(
                        filterByData?.Address.City,
                        filterByData?.Address.Region,
                        filterByData?.Address.House)
                    .WithColor(filterByData?.Color)
                    .WithGuid(
                        filterByData?.VolunteerId,
                        filterByData?.SpeciesId,
                        filterByData?.BreedId)
                    .WithName(filterByData?.Name)
                    .WithSortBy(sortByData?.SortBy, sortByData?.SortDirection)
                    .WithPagination(paginationData.Page, paginationData.PageSize)
                    .Build();

                var command = new CommandDefinition(
                    sql,
                    _queryBuilder.Parameters(),
                    cancellationToken: cancellationToken);

                var connection =
                    await _connectionFactory.CreateConnectionAsync(cancellationToken);

                var query = (await connection.QueryAsync<PetDto>(command)).ToList();
                if (!query.Any())
                {
                    _logger.Information("Pets with dates parameters not found");
                    return GeneralErrors.NotFound();
                }
                return query;
            }
            catch (Exception ex)
            {
                _logger.Error("Error get pets with pagination.{ex}"
                    , ex.Message);
                return GeneralErrors.InternalServerError(ex.Message);
            }
        }

        public Task<Maybe<PetDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
