using FluentValidation;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.SharedKernelDto;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets;

public class UploadPhotoPetsValidator : AbstractValidator<UploadPhotoPetsCommand>
{
    public UploadPhotoPetsValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .Must(i => i != Guid.Empty)
            .WithError(GeneralErrors.ValueIsInvalid("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .Must(i => i != Guid.Empty)
            .WithError(GeneralErrors.ValueIsInvalid("PetId"));

        RuleForEach(c => c.FileDtos).SetValidator(new CreateFileDtoValidator());
    }
}

public class CreateFileDtoValidator : AbstractValidator<CreateFileDto>
{
    public CreateFileDtoValidator()
    {
        RuleFor(d => d.FileData.ContentType)
            .NotEmpty()
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired(nameof(CreateFileDto.FileData.ContentType)));

        RuleFor(d => d.FileData.FileName)
            .NotEmpty()
            .NotNull()
            .Must(f => { return !string.IsNullOrWhiteSpace(Path.GetExtension(f)); })
            .WithError(GeneralErrors.ValueIsRequired(nameof(CreateFileDto.FileData.FileName)));

        RuleFor(d => d)
            .Must(dto =>
            {
                if (dto.Stream is null)
                    return false;

                var sizePhoto = dto.Stream.Length;
                var extension = Path.GetExtension(dto.FileData.FileName);

                var result = PetPhoto.Create(sizePhoto, extension);

                return result.IsSuccess;
            })
            .WithError(GeneralErrors.ValueIsInvalid("Photo"));
    }
}