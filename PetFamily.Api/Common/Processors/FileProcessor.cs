namespace PetFamily.Api.Common.Processors;

public class FileProcessor : IAsyncDisposable
{
    private readonly List<CreateFileDto> _createFileDtos = [];

    public async ValueTask DisposeAsync()
    {
        foreach (var file in _createFileDtos) await file.Stream.DisposeAsync();
    }

    public List<CreateFileDto> Process(List<IFormFile> files)
    {
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            var fileDto = new CreateFileDto(stream, new FileData(file.FileName, file.ContentType));
            _createFileDtos.Add(fileDto);
        }

        return _createFileDtos;
    }
}