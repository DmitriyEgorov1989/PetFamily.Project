using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.SharedKernel.DomainModels.VO;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models;

/// <summary>
///     Сущность пользователя, которая наследуется от IdentityUser с типом ключа Guid.
/// </summary>
public class User : IdentityUser<Guid>
{
    /// <summary>
    ///     ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private User()
    {
    }
    /// <summary>
    ///     Фотографии пользователя.
    /// </summary>
    public List<Photo> Photos { get; set; } = [];

    /// <summary>
    ///     Коллекция социальных сетей, связанных с пользователем.
    ///     Предоставляет только доступ для чтения, чтобы предотвратить изменение списка извне класса.
    /// </summary>
    public List<SocialNetwork> SocialNetwork { get; set; } = [];

    /// <summary>
    ///     ID роли, к которой принадлежит пользователь.
    ///     Это может быть использовано для определения прав доступа и разрешений пользователя в системе.
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    ///     Полное имя пользователя, представленное в виде объекта FullName.
    /// </summary>
    public FullName FullName { get; set; }
    public Admin? Admin { get; set; }
    public Participant? Participant { get; set; }
    public Volunteer? Volunteer { get; set; }

    private static User Create(Guid id, Guid roleId, FullName fullName)
    {
        return new User
        {
            Id = id,
            RoleId = roleId,
            FullName = fullName
        };
    }
    public void AddPhoto(Photo photo)
    {
        Photos.Add(photo);
    }
}