using PetFamily.SharedKernel.DomainModels.Ids;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models.Accounts;

/// <summary>
///     Сущность, представляющая участника в системе.
/// </summary>
public class Participant
{
    [ExcludeFromCodeCoverage]
    private Participant()
    {
    }
    public Guid Id { get; set; }

    /// <summary>
    ///     UserId участника,
    ///     который является уникальным идентификатором для каждого участника в системе.
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; }

    /// <summary>
    /// Список идентификаторов любимых питомцев участника. 
    /// Инициализируется пустым списком.
    /// </summary>
    public List<PetId> FavoritePets { get; set; } = [];

    /// <summary>
    ///     Метод для создания нового участника с заданным UserId.
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя, который является участником.</param>
    /// <returns>Новый объект Participant с заданным UserId.</returns>
    public static Participant Create(Guid userId)
    {
        return new Participant
        {
            UserId = userId
        };
    }
}