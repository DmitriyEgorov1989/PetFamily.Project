using FluentAssertions;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using Xunit;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;

namespace PetFamily.UnitTests.Core.Domain.Models.VolunteerAggregate;

public class VolunteerShould
{
    [Fact]
    public void WhenAddPetReturnIsSucсess()
    {
        //arrange
        var testVolunteer = CreateTestVolunteer();
        var pet = CreateTestPet();

        //act
        var result = testVolunteer.AddPet(pet);

        //assert
        result.IsSuccess.Should().BeTrue();
        testVolunteer.Pets.Should().HaveCount(1);
    }

    [Fact]
    public void WhenAddPetVolunteerIsAssignedPosition()
    {
        //arrange
        var testVolunteer = CreateTestVolunteer();
        var pet = CreateTestPet();
        var testPosition = Position.Create(1).Value;

        //act
        var result = testVolunteer.AddPet(pet);
        var petResult = pet.Position;

        //assert
        result.IsSuccess.Should().BeTrue();
        petResult.Should().NotBeNull();
        petResult.Should().Be(testPosition);
    }

    [Fact]
    public void WhenMovePetToNewPositionForwardAssignedPetsCorrectPosition()
    {
        //arrange
        var testVolunteer = CreateTestVolunteer();
        var pet1 = CreateTestPet();
        var pet2 = CreateTestPet();
        var pet3 = CreateTestPet();
        var pet4 = CreateTestPet();
        var pet5 = CreateTestPet();
        testVolunteer.AddPet(pet1);
        testVolunteer.AddPet(pet2);
        testVolunteer.AddPet(pet3);
        testVolunteer.AddPet(pet4);
        testVolunteer.AddPet(pet5);
        var newPositionPet = Position.Create(1).Value;

        //act
        var result = testVolunteer.MovePetToNewPosition(pet4, newPositionPet);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet1.Position.Number.Should().Be(2);
        pet2.Position.Number.Should().Be(3);
        pet3.Position.Number.Should().Be(4);
        pet4.Position.Should().Be(newPositionPet);
        pet5.Position.Number.Should().Be(5);
    }

    [Fact]
    public void WhenMovePetToNewPositionBackwardAssignedPetsCorrectPosition()
    {
        //arrange
        var testVolunteer = CreateTestVolunteer();
        var pet1 = CreateTestPet();
        var pet2 = CreateTestPet();
        var pet3 = CreateTestPet();
        var pet4 = CreateTestPet();
        var pet5 = CreateTestPet();
        testVolunteer.AddPet(pet1);
        testVolunteer.AddPet(pet2);
        testVolunteer.AddPet(pet3);
        testVolunteer.AddPet(pet4);
        testVolunteer.AddPet(pet5);
        var newPositionPet = Position.Create(4).Value;

        //act
        var result = testVolunteer.MovePetToNewPosition(pet1, newPositionPet);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet1.Position.Should().Be(newPositionPet);
        pet2.Position.Number.Should().Be(1);
        pet3.Position.Number.Should().Be(2);
        pet4.Position.Number.Should().Be(3);
        pet5.Position.Number.Should().Be(5);
    }

    [Fact]
    public void WhenMovePetToNewPositionNewPositionOutOfRangePetMoveToTheLastt()
    {
        //arrange
        var testVolunteer = CreateTestVolunteer();
        var pet1 = CreateTestPet();
        var pet2 = CreateTestPet();
        var pet3 = CreateTestPet();
        var pet4 = CreateTestPet();
        var pet5 = CreateTestPet();
        testVolunteer.AddPet(pet1);
        testVolunteer.AddPet(pet2);
        testVolunteer.AddPet(pet3);
        testVolunteer.AddPet(pet4);
        testVolunteer.AddPet(pet5);
        var newPositionPet = Position.Create(6).Value;

        //act
        var result = testVolunteer.MovePetToNewPosition(pet1, newPositionPet);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet1.Position.Number.Should().Be(5);
        pet2.Position.Number.Should().Be(1);
        pet3.Position.Number.Should().Be(2);
        pet4.Position.Number.Should().Be(3);
        pet5.Position.Number.Should().Be(4);
    }

    [Fact]
    public void BeUpdateMainIfoReturnSucessAndAssignedNewCorrectValue()
    {
        //arrange
        var testVolunteer = CreateTestVolunteer();
        var newEmail = "newEmail@mail.ru";
        var newDescription = "newDescription";
        var newPhoneNumber = "89450981413";
        var newExperience = 1;

        //act
        var result =
            testVolunteer
                .UpdateMainIfo(null, null, null, newEmail, newDescription, newExperience, newPhoneNumber);

        //assert
        result.IsSuccess.Should().BeTrue();
        testVolunteer.FullName.FirstName.Should().NotBeNull();
        testVolunteer.Email.Address.Should().Be(newEmail);
        testVolunteer.Description.Should().Be(newDescription);
        testVolunteer.PhoneNumber.Value.Should().Be(newPhoneNumber);
        testVolunteer.Experience.Year.Should().Be(newExperience);
    }

    private Volunteer CreateTestVolunteer()
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

    private Pet CreateTestPet()
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