using System.Data;
using System.Text;
using Dapper;

namespace PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Repository.Pets;

/// <summary>
///     C
/// </summary>
public class PetsQueryBuilder
{
    private readonly DynamicParameters _dynamicParameters = new();

    private readonly StringBuilder _sql = new(
        """
        SELECT
            pet_id            AS PetId,
            name              AS Name,
            description       AS Description,
            color             AS Color,
            health_info       AS HealthInfo,
            city              AS City,
            region            AS Region,
            house             AS House,
            weight            AS Weight,
            height            AS Height,
            phone_number      AS PhoneNumber,
            is_sterilized     AS IsSterilized,
            birth_date        AS BirthDate,
            is_vaccined       AS IsVaccined,
            pet_help_status   AS PetHelpStatus,
            help_requisites   AS PetHelpRequisites,
            photos            AS PetPhotos,
            species_id        AS SpeciesId,
            breed_id          AS BreedId,
            volunteer_id      AS VolunteerId,
            created_otc       AS CreatedUtc
        FROM pets as p  
        where p.is_delete = false
        """);

    public PetsQueryBuilder WithPagination(int page, int pageSize)
    {
        _dynamicParameters.Add("@OffSet", (page - 1) * pageSize, DbType.Int32);
        _dynamicParameters.Add("@PageSize", pageSize, DbType.Int32);
        _sql.Append(" offset @OffSet rows " +
                    "fetch next @PageSize rows only;");
        return this;
    }

    public PetsQueryBuilder WithGuid(Guid? volunteerId, Guid? speciesId, Guid? breedId)
    {
        if (volunteerId.HasValue)
        {
            _dynamicParameters.Add("@VolunteerId", volunteerId, DbType.Guid);
            _sql.Append(" AND p.volunteer_id = @VolunteerId");
        }

        if (speciesId.HasValue)
        {
            _dynamicParameters.Add("@SpeciesId", speciesId, DbType.Guid);
            _sql.Append(" AND p.species_id = @SpeciesId");
        }

        if (breedId.HasValue)
        {
            _dynamicParameters.Add("@BreedId", breedId, DbType.Guid);
            _sql.Append(" AND p.breed_id = @BreedId");
        }

        return this;
    }

    public PetsQueryBuilder WithName(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            _dynamicParameters.Add("@Name", name, DbType.String);
            _sql.Append(" AND p.name = @Name");
        }

        return this;
    }

    public PetsQueryBuilder WithColor(string? color)
    {
        if (!string.IsNullOrWhiteSpace(color))
        {
            _dynamicParameters.Add("@Color", color, DbType.String);
            _sql.Append(" AND p.color = @Color");
        }

        return this;
    }

    public PetsQueryBuilder WithAddress(string city, string region, string house)
    {
        if (!string.IsNullOrWhiteSpace(city))
        {
            _dynamicParameters.Add("@City", city, DbType.String);
            _sql.Append(" AND p.city = @City");
        }

        if (!string.IsNullOrWhiteSpace(region))
        {
            _dynamicParameters.Add("@Region", region, DbType.String);
            _sql.Append(" AND p.region = @Region");
        }

        if (!string.IsNullOrWhiteSpace(house))
        {
            _dynamicParameters.Add("@House", house, DbType.String);
            _sql.Append(" AND p.house = @House");
        }

        return this;
    }

    public PetsQueryBuilder WithSortBy(string sortBy, string sortDirection)
    {
        var sortColumn = sortBy?.ToLower() switch
        {
            "name" => "name",
            "color" => "color",
            "birthdate" => "birth_date",
            _ => "created_otc"
        };

        var direction = sortDirection is not null && sortDirection.ToUpper() == "DESC"
            ? "DESC"
            : "ASC";
        _sql.Append($" ORDER BY p.{sortColumn} {direction}");
        return this;
    }

    public string Build()
    {
        return _sql.ToString();
    }

    public DynamicParameters Parameters()
    {
        return _dynamicParameters;
    }
}