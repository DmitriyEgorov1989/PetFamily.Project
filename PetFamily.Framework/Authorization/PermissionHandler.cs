using Microsoft.AspNetCore.Authorization;
using PetFamily.Accounts.Infrastructure.Adapters.Jwt;
using PetFamily.Core.Abstractions;
using PetFamily.Framework.Authorization.PetFamily.Api.Authorization;
using Serilog;

namespace PetFamily.Framework.Authorization;

/// <summary>
/// Хендлер для проверки наличия у пользователя необходимого разрешения (permission) на основе его роли.
/// Этот класс реализует логику авторизации, которая позволяет определить,
/// имеет ли пользователь право выполнять определенные действия в системе,
/// основываясь на его роли и связанных с ней разрешениях.
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAccountServiceProvider _accountServiceProvider;
    private readonly ILogger _logger;

    public PermissionHandler(IAccountServiceProvider accountServiceProvider, ILogger logger)
    {
        _accountServiceProvider = accountServiceProvider;
        _logger = logger;
    }

    /// <summary>
    ///     Хендлер для проверки наличия у пользователя необходимого разрешения (permission) на основе его роли.
    /// </summary>
    /// <param name="context">
    ///     Контекст авторизации,
    ///     содержащий информацию о пользователе и его претензиях.
    /// </param>
    /// <param name="requirement">Требуемое разрешение для выполнения действия.</param>
    /// <returns></returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var claim =
            context.User.Claims.FirstOrDefault(c => c.Type == CustomClaims.Role);
        if (claim is null)
        {
            _logger.Warning("Role claim not found");
            context.Fail();
            return;
        }

        var roleId = claim?.Value;
        if (!Guid.TryParse(roleId, out var roleGuid))
        {
            _logger.Warning("Invalid role ID format");
            context.Fail();
            return;
        }

        var allPermissionsForRole =
            await _accountServiceProvider.GetAllPermissionsByRoleId(roleGuid);
        if (!allPermissionsForRole.Any() || !allPermissionsForRole.Contains(requirement.Permission))
        {
            _logger.Warning("Permission not found");
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}