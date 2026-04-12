using FluentValidation;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPhoto;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Extensions.Validations;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPhotoPets
{
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
            RuleFor(d => d.ContentType)
                .NotEmpty()
                .NotNull()
                .WithError(GeneralErrors.ValueIsRequired(nameof(CreateFileDto.ContentType)));

            RuleFor(d => d.FileName)
                .NotEmpty()
                .NotNull()
                .Must(f =>
                {
                    return !string.IsNullOrWhiteSpace(Path.GetExtension(f));
                })
                .WithError(GeneralErrors.ValueIsRequired(nameof(CreateFileDto.FileName)));

            RuleFor(d => d)
                .Must(dto =>
           {
               if (dto.Stream is null)
                   return false;

               var sizePhoto = dto.Stream.Length;
               var extension = Path.GetExtension(dto.FileName);

               var result = PetPhoto.Create(sizePhoto, extension);

               return result.IsSuccess;
           })
                .WithError(GeneralErrors.ValueIsInvalid("Photo"));
        }
    }
}