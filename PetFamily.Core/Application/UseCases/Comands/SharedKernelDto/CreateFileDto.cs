namespace PetFamily.Core.Application.UseCases.Comands.SharedKernelDto
{
    public record CreateFileDto(Stream Stream, string FileName, string ContentType);
}
