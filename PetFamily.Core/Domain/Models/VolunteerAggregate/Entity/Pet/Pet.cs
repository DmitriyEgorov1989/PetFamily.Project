using CSharpFunctionalExtensions;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Interfaces;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.PetAggregate
{
    /// <summary>
    /// Аггрегат животное
    /// </summary>
    public sealed class Pet : Entity<PetId>, ISoftDelete
    {
        private readonly List<PetPhoto> _photos = new();
        [ExcludeFromCodeCoverage]
        private Pet() { }

        private Pet(PetId id, string name,
                    string description,
                    PetSpeciesInfo speciesInfo,
                    Color color,
                    HealthInfo healthInfo,
                    Address address,
                    decimal weight,
                    int height,
                    PhoneNumber phoneNumber,
                    bool isSterilized,
                    DateTime birthDate,
                    bool isVaccined,
                    PetHelpStatus helpStatus,
                    HelpRequisites petHelpRequisites,
                    PetPhotos petPhotos,
                    VolunteerId volunteerId
                    )
        {
            Id = id;
            Name = name;
            Description = description;
            SpeciesInfo = speciesInfo;
            Color = color;
            HealthInfo = healthInfo;
            Address = address;
            Weight = weight;
            Height = height;
            PhoneNumber = phoneNumber;
            IsSterilized = isSterilized;
            BirthDate = birthDate;
            IsVaccined = isVaccined;
            PetHelpStatus = helpStatus;
            PetHelpRequisites = petHelpRequisites.ListHelpRequisites;
            CreatedOtc = DateTime.UtcNow;
            Photos = petPhotos;
            VolunteerId = volunteerId;
        }
        /// <summary>
        ///Кличка
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Позиция питомца у волонтера
        /// </summary>
        public Position Position { get; private set; }
        /// <summary>
        /// Общее описание
        /// </summary>      
        public string Description { get; private set; }
        /// <summary>
        /// Информация вида животного
        /// </summary>     
        public PetSpeciesInfo SpeciesInfo { get; private set; }
        /// <summary>
        /// Окрас
        /// </summary>
        public Color Color { get; private set; }
        /// <summary>
        /// Информация о здоровье питомца
        /// </summary>
        public HealthInfo HealthInfo { get; private set; }
        /// <summary>
        /// Адрес, по которому можно найти животное
        /// </summary>
        public Address Address { get; private set; }
        /// <summary>
        /// Вес животного
        /// </summary>
        public decimal Weight { get; private set; }
        /// <summary>
        /// Рост животного
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Номер телефона для связи с владельцем животного
        /// </summary>
        public PhoneNumber PhoneNumber { get; private set; }
        /// <summary>
        /// Информация о том, стерилизовано ли животное
        /// </summary>
        public bool IsSterilized { get; private set; }
        /// <summary>
        /// Дата рождения животного
        /// </summary>
        public DateTime BirthDate { get; private set; }
        /// <summary>
        /// Вакцинировано ли животное
        /// </summary>
        public bool IsVaccined { get; private set; }
        /// <summary>
        /// Статус помощи - Нуждается в помощи/Ищет дом/Нашел дом
        /// </summary>
        public PetHelpStatus PetHelpStatus { get; private set; }
        /// <summary>
        /// Реквизиты помощи - корм, лекарства, передержка и т.д.
        /// </summary>
        public IReadOnlyCollection<HelpRequisite> PetHelpRequisites { get; private set; }
        /// <summary>
        /// Дата и время создания карточки животного в системе (в формате UTC)
        /// </summary>
        public DateTime CreatedOtc { get; private set; }
        /// <summary>
        /// Навигационное свойство на волонтера
        /// </summary>
        public Volunteer VolunteerNavigation { get; private set; }
        /// <summary>
        /// VolunteerId волантера за которым закреплено животное
        /// </summary>
        public VolunteerId VolunteerId { get; private set; }
        /// <summary>
        /// Коллекция фото питомца
        /// </summary>
        public PetPhotos Photos { get; set; }

        public bool IsDelete { get; private set; } = false;

        public DateTime DateDelete { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Фабричный метод для создания экземпляра класса PetCard с валидацией входных данных. 
        /// </summary>       
        /// <returns>
        /// Возвращает результат, содержащий либо созданный объект PetCard, либо ошибку в случае неудачи.
        /// </returns>
        public static Result<Pet, Error> Create(PetId id,
            string name,
            string description,
            PetSpeciesInfo speciesInfo,
            Color color,
            HealthInfo healthInfo,
            Address address,
          decimal weight,
            int height,
            PhoneNumber phoneNumber,
            bool isSterilized,
            DateTime birthDate,
            bool isVaccined,
            PetHelpStatus helpStatus,
            HelpRequisites petHelpRequisite,
            PetPhotos photos,
            VolunteerId volunteerId
            )
        {
            if (string.IsNullOrEmpty(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));
            if (string.IsNullOrEmpty(description))
                return GeneralErrors.ValueIsInvalid(nameof(description));
            if (weight == 0)
                return GeneralErrors.ValueIsInvalid(nameof(weight));
            if (height == 0)
                if (birthDate == default)
                    return GeneralErrors.ValueIsInvalid(nameof(birthDate));

            return new Pet(id, name, description, speciesInfo, color, healthInfo, address,
                           weight, height, phoneNumber, isSterilized, birthDate, isVaccined, helpStatus, petHelpRequisite, photos, volunteerId);
        }

        public void UpdateInfo(
            string? name,
            string? description,
            string? color,
            string? city,
            string? region,
            string? house,
            decimal? weight,
            int? height,
            bool? isSterilized,
            DateTime? birthdate,
            bool? isVacined)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;
            if (!string.IsNullOrWhiteSpace(description))
                Description = description;
            if (!string.IsNullOrWhiteSpace(color))
                Color = Color.Create(color).Value;

            AddressUpdate(city, region, house);

            if (weight is not null)
                Weight = weight.Value;
            if (isSterilized is not null)
                IsSterilized = isSterilized.Value;
            if (birthdate is not null)
                BirthDate = birthdate.Value;
            if (isVacined is not null)
                isVacined = isVacined.Value;
        }

        private void AddressUpdate(string? city, string? region, string? house)
        {
            var newCity = string.IsNullOrWhiteSpace(city)
                ? Address.City
                : city;

            var newRegion = string.IsNullOrWhiteSpace(region)
                ? Address.Region
                : region;

            var newHouse = string.IsNullOrWhiteSpace(house)
                ? Address.House
                : house;

            var result = Address.Create(newCity, newRegion, newHouse);
            Address = result.Value;

        }
        public void Delete()
        {
            IsDelete = true;
            DateDelete = DateTime.UtcNow;
        }

        public void Restore()
        {
            IsDelete = false;
            DateDelete = DateTime.MinValue;
        }

        /// <summary>
        /// Convert number in enum
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static PetHelpStatus ToHelpStatus(int number)
        {
            return number switch
            {
                1 => PetHelpStatus.NeedsHelp,
                2 => PetHelpStatus.LookingForHome,
                3 => PetHelpStatus.FoundHome,
                4 => PetHelpStatus.OnTreatment,
                _ => PetHelpStatus.Unknown
            };
        }
        /// <summary>
        /// Добавдение фото пиоомца
        /// </summary>
        /// <param name="photos">список фото</param>
        /// <returns>Success or Error</returns>
        public UnitResult<Error> UploadPetPhotos(IEnumerable<PetPhoto> photos)
        {
            if (photos == null || !photos.Any())
                return GeneralErrors.ValueIsInvalid(nameof(photos));

            Photos = PetPhotos.Create(photos);

            return UnitResult.Success<Error>();
        }
        /// <summary>
        /// Удаление фото пиоомца
        /// </summary>
        /// <param name="petPhoto">название фото</param>
        /// <returns>Success or Error</returns>
        public UnitResult<Error> DeletePetPhotos(PetPhoto petPhoto)
        {
            if (petPhoto == null)
                return GeneralErrors.ValueIsInvalid(nameof(petPhoto));

            Photos.Remove(petPhoto);

            return UnitResult.Success<Error>();
        }
        /// <summary>
        /// Изменение позиции питомца на одну вперед
        /// </summary>
        /// <returns>Sucess or Error</returns>
        public UnitResult<Error> MoveForward()
        {
            var newPosition = Position.Forward();
            if (newPosition.IsFailure)
                return newPosition.Error;
            SetPosition(newPosition.Value);
            return UnitResult.Success<Error>();
        }
        /// <summary>
        /// Изменение позиции питомца на одну назад
        /// </summary>
        /// <returns>Sucess or Error</returns>
        public UnitResult<Error> MoveBackward()
        {
            var newPosition = Position.Backward();
            if (newPosition.IsFailure)
                return newPosition.Error;
            SetPosition(newPosition.Value);
            return UnitResult.Success<Error>();
        }
        /// <summary>
        /// Изменение позиции питомца
        /// </summary>
        /// <param name="newPosition">Новая позиция</param>
        public void MoveToNewPosition(Position newPosition) =>
            Position = newPosition;
        /// <summary>
        /// Установка позиции питомца при его создании. 
        /// Позиция устанавливается в конец списка питомцев волонтера, то есть на последнюю позицию.
        /// </summary>
        /// <param name="position">Позиция питомца</param>
        public void SetPosition(Position position) =>
            Position = position;
    }
}