using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.SharedKernelDto;

namespace PetFamily.Volunteers.Core.Ports;

/// <summary>
///     Порт,работающий с файловым хранилищем, например, S3, Azure Blob Storage и т.д.
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    ///     Удаление файла из хранилища, по имени файла, которое было возвращено при загрузке
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Success or Failure</returns>
    Task<UnitResult<Error>> DeleteFileAsync(
        string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получение presigned url для загрузки файла, по имени файла, которое было возвращено при загрузке.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Presigned URL or Error</returns>
    Task<Result<string, Error>> GetPresignedUrlAsync(
        string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Загрузка файлов в хранилище,
    ///     возвращает имя в хранилище, которое может отличаться от имени файла на диске
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>string filePath or Error </returns>
    Task<Result<string, Error>> UploadAsync(
        CreateFileDto file, CancellationToken cancellationToken = default);
}