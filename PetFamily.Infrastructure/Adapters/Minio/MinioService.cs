using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using PetFamily.Core.Ports;
using PetFamily.Infrastructure.Adapters.Minio.ExceptionHandling;
using PetFamily.Infrastructure.Options;
using Primitives;
using Serilog;

namespace PetFamily.Infrastructure.Adapters.Minio
{
    public class MinioService : IFileStorageProvider
    {
        private const int LIFE_TIME_URL_SEC = 100;
        private readonly IMinioClient _minioClient;
        private readonly ILogger _logger;
        private readonly string _bucketName;

        public MinioService(
            IMinioClient minioClient, ILogger logger, IOptions<MinioOptions> minioOptions)
        {
            _minioClient = minioClient;
            _logger = logger;
            _bucketName = minioOptions.Value.DefaultBucket;
        }

        public async Task<Result<PetPhotoDto, Error>> UploadAsync(
            CreateFileDto fileDto, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await OrIfBucketNotFoundCreateAsync();

                var extension = Path.GetExtension(fileDto.FileName);

                var path = Guid.NewGuid() + extension;

                var putObjectArgs = new PutObjectArgs()
                                      .WithBucket(_bucketName)
                                      .WithStreamData(fileDto.Stream)
                                      .WithObjectSize(fileDto.Stream.Length)
                                      .WithObject(path)
                                      .WithContentType(fileDto.ContentType);

                await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

                _logger.Information("File with name {name} dowload", path);
                return new PetPhotoDto(fileDto.Stream.Length, path);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToError().Message);

                return ex.ToError();
            }
        }

        public async Task<Result<string, Error>> GetPresignedUrlAync(
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

                _logger.Information("Get presigned Url sucess");
                return presignedUrl;

            }
            catch (Exception ex)
            {
                _logger.Error("Error get presigned Url:{error}", ex.ToError().Message);
                return ex.ToError();
            }
        }

        public async Task<UnitResult<Error>> DeleteFileAync(
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

                await _minioClient.RemoveObjectAsync(args);

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
               await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));

            if (!existBucket)
            {
                _logger.Information("Bucket does not exist,create bucket with name {name}", _bucketName);
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
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
                await _minioClient.StatObjectAsync(args);

                return UnitResult.Success<Error>();
            }
            catch (Exception ex)
            {
                _logger.Error("Error operation file exist : {error}", ex.ToError().Message);
                return UnitResult.Failure(ex.ToError());
            }
        }
    }
}