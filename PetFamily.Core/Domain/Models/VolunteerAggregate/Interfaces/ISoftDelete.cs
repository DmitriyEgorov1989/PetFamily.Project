namespace PetFamily.Core.Domain.Models.VolunteerAggregate.Interfaces
{
    /// <summary>
    /// Интерфейс для мягкого удаления,
    /// полночтью не удаляет обьект,а ставит флаг isDelete = true
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// Флаг показывает что обьект удален или нет
        /// </summary>
        bool IsDelete { get; }
        /// <summary>
        /// Дата удаления
        /// </summary>
        DateTime DateDelete { get; }
        /// <summary>
        /// Метод для мягкого удаления,ставит флаг и дату установки флага
        /// </summary>
        void Delete();
        /// <summary>
        /// Метод для восстановления,устанавливает флаг как false,и убирает дату удаления 
        /// </summary>
        void Restore();
    }
}