namespace PetFamily.Core.Application.UseCases.Comands.SharedKernelDto
{
    public record CreateFileDto(Stream Stream, FileData FileData);

    public record FileData(string FileName, string ContentType);
}
