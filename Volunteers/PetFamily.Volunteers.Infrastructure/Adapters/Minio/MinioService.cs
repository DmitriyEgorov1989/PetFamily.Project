using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Core.Ports;
using PetFamily.Infrastructure.Options;
using PetFamily.SharedKernel.Errors;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.SharedKernelDto;
using PetFamily.Volunteers.Infrastructure.Adapters.Minio.ExceptionHandling;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Minio;

public class MinioService : IFileStorageProvider
{
    private const int LIFE_TIME_URL_SEC = 100;
    private readonly string _bucketName;
    private readonly ILogger _logger;
    private readonly IMinioClient _minioClient;

    public MinioService(
        IMinioClient minioClient, ILogger logger, IOptions<MinioOptions> minioOptions)
    {
        _minioClient = minioClient;
        _logger = logger;
        _bucketName = minioOptions.Value.DefaultBucket;
    }

    public async Task<Result<string, Error>> UploadAsync(
        CreateFileDto file, CancellationToken cancellationToken)
    {
        try
        {
            await OrIfBucketNotFoundCreateAsync(cancellationToken);

            var extension = Path.GetExtension(file.FileData.FileName);

            var path = Guid.NewGuid() + extension;

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithStreamData(file.Stream)
                .WithObjectSize(file.Stream.Length)
                .WithObject(path)
                .WithContentType(file.FileData.ContentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            _logger.Information("File download in storage with path {name}", path);
            return Result.Success<string, Error>(path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to upload file to storage");

            return ex.ToError();
        }
    }

    public async Task<Result<string, Error>> GetPresignedUrlAsync(
        string fileName, CancellationToken cancellationToken)
    {
        try
        {
            var resultFileExist = await FileExistAsync(fileName, cancellationToken);
            if (resultFileExist.IsFailure)
                return resultFileExist.Error;

            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithExpiry(LIFE_TIME_URL_SEC);

            var presignedUrl = await _minioClient.PresignedGetObjectAsync(args);

            _logger.Information("Get presigned Url success");
            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.Error("Error get presigned Url:{error}", ex.ToError().Message);
            return ex.ToError();
        }
    }

    public async Task<UnitResult<Error>> DeleteFileAsync(
        string fileName, CancellationToken cancellationToken)
    {
        try
        {
            var resultFileExist = await FileExistAsync(fileName, cancellationToken);
            if (resultFileExist.IsFailure)
                return resultFileExist.Error;

            var args = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(args, cancellationToken);

            _logger.Information("File with name {fileName} remove", fileName);
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.Error("Error remove file with name:{name}", ex.ToError().Message);
            return UnitResult.Failure(ex.ToError());
        }
    }

    private async Task OrIfBucketNotFoundCreateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var existBucket =
            await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName), cancellationToken);

        if (!existBucket)
        {
            _logger.Information("Bucket does not exist,create bucket with name {name}", _bucketName);
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName), cancellationToken);
        }
    }

    private async Task<UnitResult<Error>> FileExistAsync(
        string fileName, CancellationToken cancellationToken)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);
            await _minioClient.StatObjectAsync(args, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.Error("Error operation file exist : {error}", ex.ToError().Message);
            return UnitResult.Failure(ex.ToError());
        }
    }
}