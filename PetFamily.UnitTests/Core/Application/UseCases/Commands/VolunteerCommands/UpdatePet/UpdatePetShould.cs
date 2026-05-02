using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdatePet;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using Xunit;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.UpdatePet;

public class UpdatePetShould
{
    private readonly ILogger _logger =
        Substitute.For<ILogger>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IValidator<UpdatePetCommand> _validator =
        Substitute.For<IValidator<UpdatePetCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    [Fact]
    public async Task BeSucessDataOrFieldIsNotNull()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var pet = ExistedPet();
        volunteer.AddPet(pet);
        var newColor = "зеленый";
        var newName = "новое имя";
        var newDescription = "новое описание";
        var newRegion = "новый регион";

        _volunteerRepository
            .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<UpdatePetCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command = new UpdatePetCommand(
            pet.Id.Id,
            volunteer.Id.Id,
            newName,
            newDescription,
            newColor,
            null,
            new AddressDto(null, newRegion, null),
            null,
            null,
            null,
            null,
            null,
            null
        );
        var handler =
            new UpdatePetHandler(_volunteerRepository, _logger, _validator, _unitOfWork);
        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet.Name.Should().Be(newName);
        pet.Description.Should().Be(newDescription);
        pet.Color.Name.Should().Be(newColor);
        pet.Address.Region.Should().Be(newRegion);
        await _validator.Received(1).ValidateAsync(Arg.Any<UpdatePetCommand>(),
            Arg.Any<CancellationToken>());
    }

    public Pet ExistedPet()
    {
        return Pet.Create(
            PetId.NewId(),
            "Барсик",
            "Очень хороший кот",
            PetSpeciesInfo.Create(SpeciesId.NewId(), BreedId.NewId()).Value, // или как у тебя создаётся
            Color.Create("Black").Value,
            HealthInfo.Create("description").Value,
            Address.Create("Москва", "Московский", "7").Value,
            4.5m,
            25,
            PhoneNumber.Create("+79999999999").Value,
            true,
            new DateTime(2020, 1, 1),
            true,
            PetHelpStatus.FoundHome,
            HelpRequisites.Create(null),
            VolunteerId.NewId()
        ).Value;
    }

    private Volunteer ExistedVolunteer()
    {
        return Volunteer.Create(
            VolunteerId.NewId(),
            FullName.Create("ivan", "ivanovich", "ivanov").Value,
            Email.Create("diman@mail.ru").Value,
            "description",
            Experience.Create(5).Value,
            PhoneNumber.Create("89258761315").Value,
            HelpRequisites.Create(null),
            SocialNetworks.Create(null)).Value;
    }
}