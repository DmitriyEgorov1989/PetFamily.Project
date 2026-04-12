using Minio.Exceptions;
using Primitives;
using MinioExceptions = Minio.Exceptions;

namespace PetFamily.Infrastructure.Adapters.Minio.ExceptionHandling;

public static class MinioExceptionHandler
{
    public static Error ToError(this Exception ex)
    {
        return ex switch
        {
            //доступ
            MinioExceptions.AuthorizationException =>
                new Error("minio.auth", "Authorization failed", ErrorType.Authorization),

            MinioExceptions.AccessDeniedException =>
                new Error("minio.access.denied", "Access denied", ErrorType.Authorization),

            //bucket
            MinioExceptions.InvalidBucketNameException =>
                new Error("minio.bucket.invalid", "Invalid bucket name", ErrorType.Validation),

            MinioExceptions.BucketNotFoundException =>
                new Error("minio.bucket.notfound", "Bucket not found", ErrorType.NotFound),

            //object / file
            MinioExceptions.InvalidObjectNameException =>
                new Error("minio.object.invalid", "Invalid object name", ErrorType.Validation),

            MinioExceptions.ConnectionException =>
           new Error("minio.error.connect", "Error connect", ErrorType.Failure),

            FileNotFoundException =>
                new Error("minio.file.notfound", "File not found", ErrorType.NotFound),

            //runtime / system
            ObjectDisposedException =>
                new Error("minio.disposed", "Stream already disposed", ErrorType.Failure),

            ObjectNotFoundException =>
                            new Error("minio.object.notfound", "Object Not Found", ErrorType.NotFound),

            NotSupportedException =>
                new Error("minio.notsupported", "Operation not supported", ErrorType.Failure),

            InvalidOperationException =>
                new Error("minio.invalid.operation", "Invalid operation", ErrorType.Failure),

            ArgumentException =>
               new Error("minio.invalid.argument", "Invalid argument", ErrorType.ValueIsInvalid),

            // fallback
            _ =>
                new Error("minio.unknown", ex.Message, ErrorType.Failure)
        };
    }
}