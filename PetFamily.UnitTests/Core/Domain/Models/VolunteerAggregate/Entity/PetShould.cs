using PetFamily.Core.Domain.Models.VolunteerAggregate.Enum;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using Xunit;

namespace PetFamily.UnitTests.Core.Domain.Models.VolunteerAggregate.Entity;

public class PetShould
{
    [Fact]
    public void BeUpdatePetPhotosReturnIsSucсess()
    {
        //arrange
        var pet = CreateTestPet();
        var petPhoto1 = PetPhoto.Create(100, "testPathStorage.jpg").Value;

        //act
        var result = pet.UploadPetPhotos([petPhoto1]);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet.Photos.ToList()[0].Should().Be(petPhoto1);
    }

    [Fact]
    public void BeDeletePetPhotosReturnSuccessAndInStorageZeroPhoto()
    {
        //arrange
        var pet = CreateTestPet();
        var petPhoto1 = PetPhoto.Create(100, "testPathStorage.jpg").Value;
        pet.UploadPetPhotos([petPhoto1]);

        //act
        var result = pet.DeletePetPhotos(petPhoto1);

        //assert
        result.IsSuccess.Should().BeTrue();
        pet.Photos.ToList().Count.Should().Be(0);
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