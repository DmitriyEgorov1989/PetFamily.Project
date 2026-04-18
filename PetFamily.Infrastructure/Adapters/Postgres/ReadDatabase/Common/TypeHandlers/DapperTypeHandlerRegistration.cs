using Dapper;
using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Common.TypeHandlers;

public static class DapperTypeHandlerRegistration
{
    public static void AddDapperTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new JsonTypeHandler<HelpRequisiteDto[]>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<SocialNetworkDto[]>());
    }
}