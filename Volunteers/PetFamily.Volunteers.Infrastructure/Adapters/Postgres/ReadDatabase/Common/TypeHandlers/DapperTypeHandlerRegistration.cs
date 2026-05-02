using Dapper;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Postgres.ReadDatabase.Common.TypeHandlers;

public static class DapperTypeHandlerRegistration
{
    public static void AddDapperTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new JsonTypeHandler<HelpRequisiteDto[]>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<SocialNetworkDto[]>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<PetPhotoDto[]>());
    }
}