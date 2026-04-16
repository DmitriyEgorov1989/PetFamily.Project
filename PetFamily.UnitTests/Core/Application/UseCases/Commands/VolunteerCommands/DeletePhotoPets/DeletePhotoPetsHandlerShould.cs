using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeletePhotoPets;
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

namespace PetFamily.UnitTests.Core.Application.UseCases.Commnands.VolunteerCommands.DeletePhotoPets
{
    public class DeletePhotoPetsHandlerShould
    {
        private readonly IVolunteerRepository _volunteerRepository =
            Substitute.For<IVolunteerRepository>();
        private readonly IFileStorageProvider _fileStorageProvider =
            Substitute.For<IFileStorageProvider>();
        private readonly IValidator<DeletePhotoPetsCommand> _validator =
            Substitute.For<IValidator<DeletePhotoPetsCommand>>();
        private readonly ILogger _logger = Substitute.For<ILogger>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        [Fact]
        public async Task BeDeletePhotoPetsReturnIsSuccessAndPathStorage()
        {
            //arrange
            var volunteer = ExistedVolunteer().Value;
            var pet = ExictedPet();
            volunteer.AddPet(pet);
            var petPhoto = PetPhoto.Create("testPath.jpg").Value;
            pet.UploadPetPhotos([petPhoto]);
            pet.Photos.ListPetPhotos.Count.Should().Be(1);

            _volunteerRepository
                             .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(volunteer));

            _validator.ValidateAsync(Arg.Any<DeletePhotoPetsCommand>(), Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(
                                       new ValidationResult()));
            _fileStorageProvider
                .DeleteFileAsync(petPhoto.PathStorage, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(UnitResult.Success<Error>()));

            var command = new DeletePhotoPetsCommand((Guid)volunteer.Id, (Guid)pet.Id, petPhoto.PathStorage);
            var handler =
                new DeletePhotoPetsHandler(
                    _volunteerRepository,
                    _fileStorageProvider,
                    _validator, _logger,
                    _unitOfWork);

            //act
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.Should().BeTrue();
            pet.Photos.ListPetPhotos.Count.Should().Be(0);
            await _fileStorageProvider.Received(1)
                 .DeleteFileAsync(petPhoto.PathStorage, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1)
                .SaveChangesAsync(Arg.Any<CancellationToken>());
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
