using CSharpFunctionalExtensions;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using Primitives;

namespace PetFamily.Core.Ports
{
    /// <summary>
    /// Порт,работающий с файловым хранилищем, например, S3, Azure Blob Storage и т.д.
    /// </summary>
    public interface IFileStorageProvider
    {
        /// <summary>
        /// Удаление файла из хранилища, по имени файла, которое было возвращено при загрузки   
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Sucess or Failure</returns>
        Task<UnitResult<Error>> DeleteFileAync(
            string fileName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение presigned url для загрузки файла, по имени файла, которое было возвращено при загрузки.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Presigned URL or Error</returns>
        Task<Result<string, Error>> GetPresignedUrlAync(
            string fileName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Загрузка файло в хранилище,
        /// возвращает имя в хранилище, которое может отличаться от имени файла на диске
        /// </summary>
        /// <param name="fileDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>string filePath or Error </returns>
        Task<Result<PetPhotoDto, Error>> UploadAsync(
            CreateFileDto fileDto, CancellationToken cancellationToken = default);
    }
}