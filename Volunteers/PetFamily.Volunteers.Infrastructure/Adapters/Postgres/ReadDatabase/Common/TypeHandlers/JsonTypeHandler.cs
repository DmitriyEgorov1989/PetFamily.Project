using System.Data;
using System.Text.Json;
using Dapper;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Postgres.ReadDatabase.Common.TypeHandlers;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, JsonOptions);
    }

    public override T Parse(object value)
    {
        if (value is null || value is DBNull)
            return default!;

        if (value is string json)
            return JsonSerializer.Deserialize<T>(json, JsonOptions)!;

        throw new DataException(
            $"Cannot convert value of type '{value.GetType().FullName}' to '{typeof(T).FullName}'.");
    }
}