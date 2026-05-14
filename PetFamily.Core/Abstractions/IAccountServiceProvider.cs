namespace PetFamily.Core.Abstractions;

public interface IAccountServiceProvider
{
    Task<List<string>> GetAllPermissionsByRoleId(Guid id, CancellationToken cancellationToken = default);
}