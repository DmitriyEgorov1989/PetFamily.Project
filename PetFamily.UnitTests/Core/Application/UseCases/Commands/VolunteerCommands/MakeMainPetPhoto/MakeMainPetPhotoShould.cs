using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using Xunit;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPetPhoto;

public class MakeMainPetPhotoShould
{
    private readonly ILogger _logger =
        Substitute.For<ILogger>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IValidator<MakeMainPhotoPetCommand> _validator =
        Substitute.For<IValidator<MakeMainPhotoPetCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    /// <summary>
    ///     Проверить, что при успешном выполнении команды,
    ///     возвращается результат успеха,
    ///     а также устанавливает флаг IsMain в true для выбранного фото и
    ///     в false для остальных фото данного питомца.
    ///     (Так как главное фото может быть только одно)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task BeReturnSuccessAndMakeFieldIsMainInTrueAndOthersFalse()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var pet = ExistedPet();
        var photo1 = PetPhoto.Create(100, "shdghs).jpg").Value;
        var photo2 = PetPhoto.Create(100, "photo2.jpg").Value;
        var photo3 = PetPhoto.Create(100, "photo3.jpg").Value;
        ;
        photo1 = photo1.MakeMain();
        pet.UploadPetPhotos([photo1, photo2, photo3]);
        volunteer.AddPet(pet);

        photo1.IsMain.Should().Be(true);
        photo2.IsMain.Should().Be(false);
        photo3.IsMain.Should().Be(false);

        _volunteerRepository
            .GetByIdAsync(volunteer.Id,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<MakeMainPhotoPetCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(
                new ValidationResult()));

        var command =
            new MakeMainPhotoPetCommand(volunteer.Id.Id, pet.Id.Id, photo2.PathStorage);
        var handler = new MakeMainPhotoPetHandler(
            _volunteerRepository, _logger, _validator, _unitOfWork);

        //act
        var result =
            await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet.Photos.ToList()[0].IsMain.Should().Be(false);
        pet.Photos.ToList()[1].IsMain.Should().Be(true);
        pet.Photos.ToList()[2].IsMain.Should().Be(false);
        await _validator.Received(1)
            .ValidateAsync(Arg.Any<MakeMainPhotoPetCommand>(),
                Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }

    private Volunteer ExistedVolunteer()
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

    private Pet ExistedPet()
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
            VolunteerId.NewId()).Value;
    }
}