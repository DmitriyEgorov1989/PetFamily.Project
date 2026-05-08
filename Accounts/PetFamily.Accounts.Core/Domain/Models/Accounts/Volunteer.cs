using CSharpFunctionalExtensions;
using PetFamily.Accounts.Core.Domain.Models.VO;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models.Accounts;

/// <summary>
///   Сущность, представляющая волонтера в системе.
/// </summary>
public class Volunteer
{

    [ExcludeFromCodeCoverage]
    private Volunteer()
    {
    }
    public Guid Id { get; set; }
    /// <summary>
    ///     UserId волонтера,
    ///     который является уникальным идентификатором для каждого волонтера в системе.
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; }

    /// <summary>
    /// Список реквизитов помощи, которые волонтер может предоставить.
    /// </summary>
    public List<HelpRequisite> HelpRequisites { get; set; } = [];

    /// <summary>
    /// Опыт волонтера, 
    /// который может включать в себя информацию о том, 
    /// сколько времени он уже занимается волонтерской деятельностью,
    /// </summary>
    public Experience Experience { get; set; }

    /// <summary>
    /// Фабричный метод для создания нового волонтера. 
    /// Он принимает UserId и Experience в качестве параметров и возвращает результат, 
    /// который может быть либо успешным с новым объектом Volunteer,
    /// либо ошибкой, если входные данные недопустимы.
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя, который является волонтером.</param>
    /// <param name="experience">Опыт волонтера.</param>
    /// <returns>Результат создания волонтера, который может быть успешным или содержать ошибку.</returns>
    public static Result<Volunteer, Error> Create(Guid userId, Experience experience)
    {
        if (userId == Guid.Empty)
        {
            return GeneralErrors.ValueIsRequired(nameof(userId));
        }
        var volunteer = new Volunteer
        {
            UserId = userId,
            Experience = experience,
        };
        return volunteer;
    }
}