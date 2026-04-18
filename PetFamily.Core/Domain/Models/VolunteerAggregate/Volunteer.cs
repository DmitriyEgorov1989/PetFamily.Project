using CSharpFunctionalExtensions;
using PetFamily.Core.Domain.Models.PetAggregate;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Interfaces;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate;

/// <summary>
///     Аггрегат волонтер
/// </summary>
public sealed class Volunteer : Aggregate<VolunteerId>, ISoftDelete
{
    private readonly List<Pet> _pets = new();

    [ExcludeFromCodeCoverage]
    private Volunteer()
    {
    }

    private Volunteer(VolunteerId volunteerId,
        FullName fullName,
        Email email,
        string description,
        Experience experience,
        PhoneNumber phoneNumber,
        HelpRequisites helpRequisites,
        SocialNetworks socialNetworks
    )
    {
        Id = volunteerId;
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        HelpRequisites = helpRequisites.ListHelpRequisites;
        SocialNetworks = socialNetworks.ListSocialNetworks;
    }

    /// <summary>
    ///     Полное имя волонтера
    /// </summary>
    public FullName FullName { get; private set; }

    /// <summary>
    ///     Адрес почты
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    ///     Полное описание
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    ///     Опыт в годах
    /// </summary>
    public Experience Experience { get; private set; }

    /// <summary>
    /// номер телефона для связи
    /// </summary>
    public PhoneNumber PhoneNumber { get; private set; }

    /// <summary>
    ///     Список социальных сетей
    /// </summary>
    public IReadOnlyCollection<SocialNetwork> SocialNetworks { get; private set; }

    /// <summary>
    ///     Список реквизитов для помощи
    /// </summary>
    public IReadOnlyCollection<HelpRequisite> HelpRequisites { get; private set; }

    /// <summary>
    ///     Список питомцев волонтера
    /// </summary>
    public IReadOnlyCollection<Pet> Pets => _pets.AsReadOnly();

    public bool IsDelete { get; private set; }

    public DateTime DateDelete { get; private set; } = DateTime.MinValue;

    public void Delete()
    {
        IsDelete = true;
        DateDelete = DateTime.UtcNow;
        if (_pets.Any())
            _pets.ForEach(pet => pet.Delete());
    }

    public void Restore()
    {
        IsDelete = false;
        DateDelete = DateTime.MinValue;
        if (_pets.Any())
            _pets.ForEach(pet => pet.Restore());
    }

    /// <summary>
    ///     Фабричный метод создания аггрегата волонтера,с валидацией
    /// </summary>
    /// <returns>Возвращает либо созданного волонтера,либо ошибку</returns>
    public static Result<Volunteer, Error> Create(VolunteerId id, FullName fullName,
        Email email,
        string description,
        Experience experience,
        PhoneNumber phoneNumber,
        HelpRequisites helpRequisites,
        SocialNetworks socialNetworks)
    {
        if (string.IsNullOrWhiteSpace(description))
            return GeneralErrors.ValueIsRequired(nameof(description));

        return new Volunteer(id,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            helpRequisites,
            socialNetworks);
    }

    /// <summary>
    ///     Метод для расчета количества питомцев у волонтера в зависимости от статуса
    /// </summary>
    /// <param name="pets">Список питомцев</param>
    /// <param name="status">Статус питомца</param>
    /// <returns></returns>
    private int CountPetsByStatus(IEnumerable<Pet> pets, PetHelpStatus status)
    {
        if (!pets.Any()) return 0;
        return pets.Count(p => p.PetHelpStatus == status);
    }

    /// <summary>
    ///     Метод для расчета количества питомцев у волонтеров которые нашли свой дом
    /// </summary>
    /// <param name="pets">Общий список питомцев волонтера</param>
    /// <returns>кол-во питомцев</returns>
    public int CountFoundHomePets(IEnumerable<Pet> pets)
    {
        return CountPetsByStatus(pets, PetHelpStatus.FoundHome);
    }

    /// <summary>
    ///     Метод для расчета количества питомцев у волонтеров которые ищут свой дом
    /// </summary>
    /// <param name="pets">Общий список питомцев волонтера</param>
    /// <returns>кол-во питомцев</returns>
    public int CountLookingForHomePets(IEnumerable<Pet> pets)
    {
        return CountPetsByStatus(pets, PetHelpStatus.LookingForHome);
    }

    /// <summary>
    ///     Метод для расчета количества питомцев у волонтеров которые находятся на лечении
    /// </summary>
    /// <param name="pets">Общий список питомцев волонтера</param>
    /// <returns>Кол-во питомцев</returns>
    public int CountOnTreatmentPets(IEnumerable<Pet> pets)
    {
        return CountPetsByStatus(pets, PetHelpStatus.OnTreatment);
    }

    /// <summary>
    ///     Метод для обновления инфы волонтера
    /// </summary>
    /// <param name="firstName">Имя</param>
    /// <param name="middleName">Отчество</param>
    /// <param name="lastName">Фамилия</param>
    /// <param name="email">аддрес почты</param>
    /// <param name="description">описание</param>
    /// <param name="experience">Опыт</param>
    /// <param name="phoneNumber">номер телефона</param>
    /// <returns>Возвращает Success или Failure с ошибкой</returns>
    public UnitResult<Error> UpdateMainIfo(
        string? firstName,
        string? middleName,
        string? lastName,
        string? email,
        string? description,
        int? experience,
        string? phoneNumber)
    {
        try
        {
            var resultFulNameUpdate = FullNameUpdate(firstName, middleName, lastName);

            if (resultFulNameUpdate.IsFailure)
                return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(FullName)));

            EmailUpdate(email);
            DescriptionUpdate(description);
            PhoneNumberUpdate(phoneNumber);
            ExperienceUpdate(experience);

            return UnitResult.Success<Error>();
        }

        catch (Exception ex)
        {
            return UnitResult.Failure(
                new Error("error.update.info", ex.Message, ErrorType.InternalServerError));
        }
    }

    private UnitResult<Error> FullNameUpdate(string? firstName, string? middleName, string? lastName)
    {
        var newFirstName = string.IsNullOrWhiteSpace(firstName)
            ? FullName.FirstName
            : firstName;

        var newMiddleName = string.IsNullOrWhiteSpace(middleName)
            ? FullName.MiddleName
            : middleName;

        var newLastName = string.IsNullOrWhiteSpace(lastName)
            ? FullName.LastName
            : lastName;

        var result = FullName.Create(newFirstName, newMiddleName, newLastName);

        if (result.IsFailure)
            return UnitResult.Failure(result.Error);

        FullName = result.Value;
        return UnitResult.Success<Error>();
    }

    private void EmailUpdate(string? email)
    {
        if (!string.IsNullOrEmpty(email)) Email = Email.Create(email).Value;
    }

    private void DescriptionUpdate(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description)) Description = description;
    }

    private void PhoneNumberUpdate(string? phoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber)) PhoneNumber = PhoneNumber.Create(phoneNumber).Value;
    }

    private void ExperienceUpdate(int? experience)
    {
        if (experience is not null) Experience = Experience.Create(experience.Value).Value;
    }

    public void UpdateSocialNetworks(SocialNetworks socialNetworks)
    {
        SocialNetworks = socialNetworks.ListSocialNetworks;
    }

    public void UpdateHelpRequisites(HelpRequisites helpRequisites)
    {
        HelpRequisites = helpRequisites.ListHelpRequisites;
    }

    /// <summary>
    ///     Метод добавления питомца
    /// </summary>
    /// <param name="pet">Новый питомец</param>
    /// <returns>Success or Failure</returns>
    public UnitResult<Error> AddPet(Pet pet)
    {
        if (pet is null)
            return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(pet)));
        var position =
            Position.Create(_pets.Count + 1);
        if (position.IsFailure)
            return UnitResult.Failure(position.Error);

        pet.SetPosition(position.Value);
        _pets.Add(pet);

        return UnitResult.Success<Error>();
    }

    public Result<Pet, Error> GetPetById(PetId petId)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return GeneralErrors.NotFound(nameof(pet));

        return pet;
    }

    /// <summary>
    ///     движенин питомца на новую позицию
    /// </summary>
    /// <param name="pet"></param>
    /// <param name="newPosition"></param>
    /// <returns></returns>
    public UnitResult<Error> MovePetToNewPosition(Pet pet, Position newPosition)
    {
        var adjustedNewPositionResult = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedNewPositionResult.IsFailure)
            return UnitResult.Failure(adjustedNewPositionResult.Error);

        newPosition = adjustedNewPositionResult.Value;

        if (newPosition.Number < pet.Position.Number)
            Pets.Where(p => p.Position.Number >= newPosition.Number
                            && p.Position.Number < pet.Position.Number)
                .Select(p => p.MoveForward())
                .ToList();
        if (newPosition.Number > pet.Position.Number)
            Pets.Where(p => p.Position.Number <= newPosition.Number
                            && p.Position.Number > pet.Position.Number)
                .Select(p => p.MoveBackward())
                .ToList();
        pet.SetPosition(newPosition);
        return UnitResult.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionIfOutOfRange(Position newPosition)
    {
        if (!_pets.Any())
            return GeneralErrors.ValueIsInvalid("У волонтера нет питомцев");

        var lastPosition =
            _pets[_pets.Count() - 1].Position;

        if (newPosition.Number >= lastPosition.Number)
            return lastPosition;

        return newPosition;
    }
}