using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
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

    public static Result<User, Error> Create(Guid id, FullName fullName, string email,
        string userName, string phoneNumber)
    {
        if (id == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(id));

        return new User
        {
            Id = id,
            FullName = fullName,
            Email = email,
            UserName = userName,
            PhoneNumber = phoneNumber
        };
    }

    public void AddPhoto(Photo photo)
    {
        Photos.Add(photo);
    }

    public UnitResult<Error> AddRole(Role role, Admin? admin, Participant? participant, Volunteer? volunteer)
    {
        if (admin != null)
        {
            RoleId = role.Id;
            Admin = admin;
            return UnitResult.Success<Error>();
        }

        if (participant != null)
        {
            RoleId = role.Id;
            Participant = participant;
            return UnitResult.Success<Error>();
        }

        if (volunteer != null)
        {
            RoleId = role.Id;
            Volunteer = volunteer;
            return UnitResult.Success<Error>();
        }

        return UnitResult.Failure<Error>(
            GeneralErrors.Failure("Error add role"));
    }
}