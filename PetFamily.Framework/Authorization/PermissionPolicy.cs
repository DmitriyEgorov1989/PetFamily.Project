using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using PetFamily.Framework.Authorization.PetFamily.Api.Authorization;

namespace PetFamily.Framework.Authorization
{
    /// <summary>
    /// класс,
    /// который реализует интерфейс IAuthorizationPolicyProvider
    /// и отвечает за предоставление политик авторизации на основе разрешений (permissions).
    /// </summary>
    public class PermissionPolicy : IAuthorizationPolicyProvider
    {
        public PermissionPolicy(IOptions<AuthorizationOptions> options)
        {
            DefaultAuthorizationPolicy = new DefaultAuthorizationPolicyProvider(options);
        }

        private DefaultAuthorizationPolicyProvider DefaultAuthorizationPolicy { get; }

        /// <summary>
        /// Получает политику авторизации на основе имени политики (policyName).
        /// Если имя политики не пустое или не состоит только из пробелов,
        /// то создается новая политика авторизации,
        /// которая включает требование разрешения (PermissionAttribute) с именем политики.
        /// Если имя политики пустое или состоит только из пробелов,
        /// то возвращается политика по умолчанию,
        /// предоставляемая DefaultAuthorizationPolicyProvider.
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(policyName))
                {
                    var policyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                    policyBuilder.AddRequirements(new PermissionRequirement(policyName));
                    return policyBuilder.Build();
                }
                return await DefaultAuthorizationPolicy.GetPolicyAsync(policyName);
            }
            catch (Exception)
            {
                return await DefaultAuthorizationPolicy.GetPolicyAsync(policyName);
            }
        }
        /// <summary>
        /// GetDefaultPolicyAsync - это метод,
        /// который возвращает политику авторизации по умолчанию.
        /// </summary>
        /// <returns></returns>
        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return await DefaultAuthorizationPolicy.GetDefaultPolicyAsync();
        }
        /// <summary>
        /// Получает политику авторизации,
        /// которая будет использоваться в случае,
        /// если не удается найти политику по имени (policyName).
        /// </summary>
        /// <returns></returns>
        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return await DefaultAuthorizationPolicy.GetFallbackPolicyAsync();
        }
    }
}