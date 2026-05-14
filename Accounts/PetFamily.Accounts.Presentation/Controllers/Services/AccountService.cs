using MediatR;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetPermissionsByRoleId;
using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Presentation.Controllers.Services
{
    public class AccountService(IMediator mediator) : IAccountServiceProvider
    {
        public async Task<List<string>> GetAllPermissionsByRoleId(
            Guid id, CancellationToken cancellationToken = default)
        {
            var response =
                await mediator.Send(new GetPermissionsByRoleIdQuery(id));

            if (response.IsFailure)
                return [];

            return response.Value.Permissions;
        }
    }
}