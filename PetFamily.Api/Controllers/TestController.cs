//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Minio;
//using PetFamily.Api.Common.Processors;
//using PetFamily.Api.Extensions;
//using PetFamily.Core.Application.UseCases.Comands.FileCommands.DeleteFile;
//using PetFamily.Core.Application.UseCases.Comands.FileCommands.UploadFiles;
//using PetFamily.Core.Application.UseCases.Queries.Files.GetPresignedUrl;

//namespace PetFamily.Api.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class TestController : ControllerBase
//    {
//        private readonly IMediator _mediator;
//        private IMinioClient _minioClient;
//        public TestController(IMediator mediator, IMinioClient minioClient)
//        {
//            _mediator = mediator;
//            _minioClient = minioClient;
//        }

//    //    [HttpPost("Upload")]
//    //    public async Task<ActionResult> UploadPhotoAsync(
//    //        [FromForm] List<IFormFile> fileCollections,
//    //       CancellationToken cancellationToken = default)
//    //    {
//    //        var fileProcessor = new FileProcessor();
//    //        var createFileDtos = fileProcessor.Process(fileCollections);

//    //        var resultResponse = await _mediator.Send(new UploadFilesCommand(createFileDtos));

//    //        return resultResponse.ToResponseOkOrError();
//    //    }

//    //    [HttpGet("Url")]
//    //    public async Task<ActionResult<GetPresignedUrlResponse>> GetPresignedUrlAsync(
//    //       [FromQuery] GetPresignedUrlRequest request,
//    //      CancellationToken cancellationToken = default)
//    //    {
//    //        var response = await _mediator.Send(request, cancellationToken);

//    //        return response.ToResponseErrorOrResult();
//    //    }

//    //    [HttpDelete]
//    //    public async Task<ActionResult<string>> DeleteAsync(
//    //       [FromQuery] DeleteFileRequest request,
//    //      CancellationToken cancellationToken = default)
//    //    {
//    //        var response = await _mediator.Send(request, cancellationToken);

//    //        return response.ToResponseOkOrError();
//    //    }
//    //}
//}