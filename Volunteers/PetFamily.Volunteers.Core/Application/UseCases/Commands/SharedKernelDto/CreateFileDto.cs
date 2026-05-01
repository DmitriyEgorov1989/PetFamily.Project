namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.SharedKernelDto;

public record CreateFileDto(Stream Stream, FileData FileData);

public record FileData(string FileName, string ContentType);