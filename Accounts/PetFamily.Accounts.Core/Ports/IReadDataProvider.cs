using CSharpFunctionalExtensions;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetMe;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Accounts.Core.Ports
{
    public interface IReadDataProvider
    {
        Task<Result<List<string>, Error>> GetAllPermissionsByRoleIdAsync(
            Guid id, CancellationToken cancellationToken = default);

        Task<Result<Role, Error>> GetRoleByIdAsync(
            Guid id, CancellationToken cancellationToken = default);

        Task<Result<Role, Error>> GetRoleByNameAsync(
            string name, CancellationToken cancellationToken = default);

        Task<Result<UserInfo, Error>> GetUserInfoByIdAsync(Guid userId,
            CancellationToken cancellationToken = default);
    }
}