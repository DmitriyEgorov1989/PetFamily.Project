using Microsoft.AspNetCore.Authorization;

namespace PetFamily.Framework.Authorization
{
    namespace PetFamily.Api.Authorization
    {
        /// <summary>
        /// Фильтр авторизации, который проверяет наличие определенного разрешения у пользователя.
        /// </summary>
        public class PermissionRequirement : IAuthorizationRequirement
        {
            public PermissionRequirement(string permission)
            {
                Permission = permission;
            }

            protected internal string Permission { get; set; }
        }
    }
}