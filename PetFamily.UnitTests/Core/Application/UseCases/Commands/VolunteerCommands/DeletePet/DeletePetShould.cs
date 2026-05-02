using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using Xunit;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;

public class DeletePetShould
{
    private readonly ILogger _logger =
        Substitute.For<ILogger>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IValidator<DeletePetCommand> _validator =
        Substitute.For<IValidator<DeletePetCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    /// <summary>
    ///     При удалении питомца, который есть у волонтера,
    ///     должно возвращаться успешное выполнение команды и
    ///     корректно изменяться позиция других питомцев в списке
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task BeReturnSucessAndCorrectedPositionOtherPetsInList()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var pet1 = ExistedPet1();
        var pet2 = ExistedPet2();
        var pet3 = ExistedPet3();
        volunteer.AddPet(pet1);
        volunteer.AddPet(pet2);
        volunteer.AddPet(pet3);
        pet1.Position.Number.Should().Be(1);
        pet2.Position.Number.Should().Be(2);
        pet3.Position.Number.Should().Be(3);

        _volunteerRepository
            .GetByIdAsync(Arg.Any<VolunteerId>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<DeletePetCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command = new DeletePetCommand(volunteer.Id.Id, pet1.Id.Id);
        var handler = new DeletePetHandler(
            _logger, _volunteerRepository, _validator, _unitOfWork);

        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet2.Position.Number.Should().Be(1);
        pet3.Position.Number.Should().Be(2);
        volunteer.GetPetById(pet1.Id).IsFailure.Should().BeTrue();
        await _validator.Received(1)
            .ValidateAsync(Arg.Any<DeletePetCommand>(),
                Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }

    public Pet ExistedPet1()
    {
        return Pet.Create(
            PetId.NewId(),
            "Барсик1",
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

    public Pet ExistedPet2()
    {
        return Pet.Create(
            PetId.NewId(),
            "Барсик2",
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

    public Pet ExistedPet3()
    {
        return Pet.Create(
            PetId.NewId(),
            "Барсик3",
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