using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.SharedKernelDto;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets;
using PetFamily.Core.Domain.Models.PetAggregate;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.SpeciesAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets
{
    public class UploadPhotoPetsHandlerShould
    {
        private readonly IVolunteerRepository _volunteerRepository =
            Substitute.For<IVolunteerRepository>();
        private readonly IFileStorageProvider _fileStorageProvider =
            Substitute.For<IFileStorageProvider>();
        private readonly IValidator<UploadPhotoPetsCommand> _validator =
            Substitute.For<IValidator<UploadPhotoPetsCommand>>();
        private readonly ILogger _logger =
            Substitute.For<ILogger>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        public readonly IMessageQueueService<IEnumerable<PetPhotoDto>> _queueService =
            Substitute.For<IMessageQueueService<IEnumerable<PetPhotoDto>>>();

        private readonly UploadPhotoPetsHandler _handler;

        public UploadPhotoPetsHandlerShould()
        {
            _handler = new UploadPhotoPetsHandler(
                _validator,
                _volunteerRepository,
                _fileStorageProvider,
                 _logger,
                 _unitOfWork,
                _queueService);
        }
        /// <summary>
        /// Проверяет при добавлении фото,что фото добавилось 
        /// и у питомца в списке фото лежит именно то фото которое добавили
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BeAddPhotoPetsReturnIsSuccessAndGuidPet()
        {
            //arrange
            var volunteer = ExistedVolunteer().Value;
            var pet = ExictedPet();
            volunteer.AddPet(pet);

            _volunteerRepository
                             .GetByIdAsync(Arg.Any<VolunteerId>(), Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(volunteer));

            _validator.ValidateAsync(Arg.Any<UploadPhotoPetsCommand>(), Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(
                                       new ValidationResult()));
            _fileStorageProvider
                .UploadAsync(Arg.Any<CreateFileDto>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success<string, Error>("upload")));
            _queueService.WriteAsync(Arg.Any<IEnumerable<PetPhotoDto>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult);

            var createFileDto = new CreateFileDto(new MemoryStream(), new FileData("testName", "jpg"));
            var command = new UploadPhotoPetsCommand((Guid)volunteer.Id, (Guid)pet.Id, [createFileDto]);

            //act
            var result = await _handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.Should().BeTrue();
            pet.Photos.ListPetPhotos[0].Should().NotBeNull();
            result.Value.Should().Be((Guid)pet.Id);
            await _fileStorageProvider.Received(1)
                 .UploadAsync(Arg.Any<CreateFileDto>(), Arg.Any<CancellationToken>());
        }

        private Maybe<Volunteer> ExistedVolunteer()
        {
            return Volunteer.Create(
                VolunteerId.NewId(),
                FullName.Create("ivan", "ivanivich", "ivanov").Value,
                Email.Create("diman@mail.ru").Value,
                "description",
                Experience.Create(5).Value,
                PhoneNumber.Create("89258761315").Value,
                HelpRequisites.Create(null),
                SocialNetworks.Create(null)).Value;
        }

        private Pet ExictedPet()
        {
            return Pet.Create(
                PetId.NewId(),
                "test",
                "description",
                PetSpeciesInfo.Create(SpeciesId.NewId(), BreedId.NewId()).Value,
                Color.Create("test").Value,
                HealthInfo.Create("test").Value,
                Address.Create("testCity", "testRegion", "testHouse").Value,
                10,
                5,
                PhoneNumber.Create("89258761315").Value,
                true,
                DateTime.UtcNow,
                true,
                PetHelpStatus.OnTreatment,
                HelpRequisites.Create(null),
                null,
                VolunteerId.NewId()).Value;
        }
    }
}
